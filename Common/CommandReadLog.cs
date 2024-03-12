using GemLogAnalyzer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Input;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Media;
using System.Globalization;
using GemLogAnalyzer.Models;
using System.Runtime.CompilerServices;

namespace GemLogAnalyzer.Common
{
    public class CommandReadLog : ICommand
    {
        /// <summary>
        /// MainViewModelのパラメータ
        /// </summary>
        MainViewModel m_vm;

        /// <summary>
        /// GeneralClass
        /// </summary>
        GeneralClass m_GeneralClass;

        /// <summary>
        /// ログファイル最終読み込み地点
        /// </summary>
        private long m_LastFilePoint;


        public CommandReadLog( MainViewModel vm )
        {
            m_vm = vm;
            m_GeneralClass = GeneralClass.Instance;
            m_LastFilePoint = 0;
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// コマンドが利用可能かどうか
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool CanExecute( object? parameter )
        {
            // ファイルが存在するか確認
            string filePath = m_vm.CvpGemLogFilePath;
            if( filePath.Length <= 0 )
            {
                return false;
            }

            if( !File.Exists( filePath ) )
            {
                return false;
            }

            // ファイル名を取得
            string fileName = System.IO.Path.GetFileName( filePath );

            // フィルタにかける正規表現
            string pattern = @"^CvpGem.*\.log$";

            // ファイル名が正規表現(^CvpGem.*\.log$)に当てはまるか確認
            if( !Regex.IsMatch( fileName, pattern ) )
            {
                return false;
            }

            // ファイル名が正しそうやったら読み込み
            if( filePath != m_GeneralClass.AnaConf.LogFilePath )
            {
                // 前回読み込んだファイルと違っていたら全読み込み
                m_GeneralClass.AnaConf.LogFilePath = filePath;
                m_LastFilePoint = 0;

                // DataGridをクリア
                m_vm.LogDatas.Clear();
                // Logデータもクリア
                m_GeneralClass.LogDatas.Clear();
            }
            return true;
        }

        /// <summary>
        /// 実行時の処理
        /// </summary>
        /// <param name="parameter"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Execute( object? parameter )
        {
            // ログを読み込む
            ReadCvpGemLogFile( m_GeneralClass.AnaConf.LogFilePath );

            m_GeneralClass.AnaConf.LogFileDate = File.GetLastWriteTime( m_GeneralClass.AnaConf.LogFilePath );
        }

        /// <summary>
        /// 指定されたファイルパスのCVP GEMログファイルを読み込み、データを解析します。
        /// </summary>
        /// <param name="argFilePath">読み込むファイルのパス。</param>
        private void ReadCvpGemLogFile( string argFilePath )
        {
            using( FileStream stream = new FileStream( argFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite ) )
            {
                // ファイルサイズが減っていた場合、または同じであるが開始位置ではない場合はリセット
                if( stream.Length <= m_LastFilePoint && m_LastFilePoint != 0 )
                {
                    m_LastFilePoint = 0;
                    m_vm.LogDatas.Clear(); // DataGridをクリア
                    m_GeneralClass.LogDatas.Clear(); // ログデータをクリア
                }

                int logDataCount = ( m_vm.LogDatas.Count > 0 ) ? m_vm.LogDatas.LastOrDefault().DataNo + 1 : 0;
                if( m_LastFilePoint > 0 )
                {
                    // 最後に読んだファイルの位置に移動
                    stream.Seek( m_LastFilePoint, SeekOrigin.Begin );
                }

                try
                {
                    using( StreamReader reader = new StreamReader( stream ) )
                    {
                        string? line;
                        while( ( line = reader.ReadLine() ) != null )
                        {
                            ClassCvpGemLogData data = ParseLine( line, reader );
                            if( data != null ) // データが正常に解析された場合
                            {
                                ReflectToView( data, logDataCount++ );
                            }
                        }
                        m_LastFilePoint = stream.Length; // 最後に読んだ位置を更新
                    }
                }
                catch( IOException e )
                {
                    Console.WriteLine( e.Message );
                }
            }
        }

        /// <summary>
        /// 読み込んだ行からCvpGemLogDataオブジェクトを生成し、解析します。
        /// </summary>
        /// <param name="line">読み込んだ行のテキスト。</param>
        /// <param name="reader">StreamReaderインスタンス。</param>
        /// <returns>解析されたClassCvpGemLogDataオブジェクト。解析に失敗した場合はnullを返します。</returns>
        private ClassCvpGemLogData? ParseLine( string line, StreamReader reader )
        {
            if( !line.Contains( "<Send>" ) && !line.Contains( "<Receive>" ) ) return null;

            ClassCvpGemLogData data = new ClassCvpGemLogData
            {
                SendReceiveFlag = line.Contains( "<Send>" ) ? eSendReceiveFlag.Send : eSendReceiveFlag.Receive,
                Message = ParseMessage( reader, out string messageTitle ),
                MessageTitle = messageTitle
            };

            if( !DateTime.TryParseExact( line.Split( new[] { ' ' }, 3 )[0] + " " + line.Split( new[] { ' ' }, 3 )[1],
                                        "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture,
                                        DateTimeStyles.None, out DateTime date ) )
            {
                return null; // 日付の解析に失敗した場合はスキップ
            }

            data.Date = date;
            ExtractSAndF( data );

            // S6F11メッセージの特別な処理
            if( data.Stream == 6 && data.Function == 11 )
            {
                AnalyzeEvent( data );
            }

            return data;
        }

        /// <summary>
        /// StreamReaderからメッセージを読み込み、リストとして返します。
        /// </summary>
        /// <param name="reader">読み込むためのStreamReaderインスタンス。</param>
        /// <param name="messageTitle">読み込まれたメッセージのタイトル。</param>
        /// <returns>メッセージのリスト。</returns>
        private List<string> ParseMessage( StreamReader reader, out string messageTitle )
        {
            List<string> message = new List<string>();
            string? line;
            while( ( line = reader.ReadLine() ) != null && line != "" )
            {
                message.Add( line );
            }

            messageTitle = message.FirstOrDefault() ?? string.Empty;
            return message;
        }

        /// <summary>
        /// メッセージタイトルからStreamとFunctionの番号を抽出します。
        /// </summary>
        /// <param name="data">解析データを保持するClassCvpGemLogDataインスタンス。</param>
        private void ExtractSAndF( ClassCvpGemLogData data )
        {
            Regex pattern = new Regex( @"S(\d+)F(\d+)" );
            Match match = pattern.Match( data.MessageTitle );
            if( match.Success )
            {
                data.Stream = int.Parse( match.Groups[1].Value );
                data.Function = int.Parse( match.Groups[2].Value );
            }
        }

        /// <summary>
        /// 解析したデータをViewに反映させます。
        /// </summary>
        /// <param name="data">反映するデータ。</param>
        /// <param name="dataNo">データ番号。</param>
        private void ReflectToView( ClassCvpGemLogData data, int dataNo )
        {
            m_vm.LogDatas.Add( new DataGridLogData
            {
                Date = data.Date.ToString(),
                SendReceive = data.SendReceiveFlag.ToString(),
                Stream = data.Stream,
                Function = data.Function,
                MessageTitle = data.MessageTitle,
                DataNo = dataNo
            } );

            m_GeneralClass.LogDatas.Add( data.Clone() );
        }


        /// <summary>
        /// イベントを分析して、イベントの設定に基づいてメッセージタイトルを更新し、VIDリストを処理します。
        /// </summary>
        /// <param name="data">分析するイベントログデータ。</param>
        private void AnalyzeEvent( ClassCvpGemLogData data )
        {
            // 効率のためにRegexパターンを事前コンパイル
            Regex ceidPattern = new Regex( @"<u4 (\d+)>$", RegexOptions.Compiled );
            Regex reportPattern = new Regex( @"^      <u4 (\d+)>$", RegexOptions.Compiled );
            Regex vidPattern = new Regex( @"<.*>", RegexOptions.Compiled );

            // イベント構成設定を取得
            ClassCvpGemConfig config = m_GeneralClass.CvpConf;

            // CEIDから設定内容を取得
            Match ceidMatch = ceidPattern.Match( data.Message[(int)eEventStruct.CEID] );
            if( !ceidMatch.Success )
            {
                return; // CEIDが無効の場合、メソッドを終了
            }
            int ceid = int.Parse( ceidMatch.Groups[1].Value );
            ClassCvpGemConfig.Event eventModel = config.GetEventFromCeid( config.eventModel.events, ceid );
            if( eventModel == null )
            {
                return; // イベントモデルが見つからない場合、メソッドを終了
            }

            // イベント名でメッセージタイトルを更新
            data.MessageTitle = eventModel.description;

            int messageCount = (int)eEventStruct.StartReportID;
            while( data.Message.Count > messageCount )
            {
                Match reportMatch = reportPattern.Match( data.Message[messageCount] );
                if( !reportMatch.Success )
                {
                    messageCount++;
                    continue; // マッチしない場合、次のメッセージへスキップ
                }

                int reportNo = int.Parse( reportMatch.Groups[1].Value );
                if( !eventModel.reports.Contains( reportNo ) )
                {
                    messageCount++;
                    continue; // レポート番号がイベントモデルに含まれていない場合、スキップ
                }

                List<ClassCvpGemConfig.Variable>? variableList = config.GetVariableListFromReportNo( reportNo );
                if( variableList == null )
                {
                    return; // レポート番号にリンクされた変数がない場合、メソッドを終了
                }
                data.VidList = new List<ClassCvpGemConfig.Variable>( variableList );

                messageCount += eEventStruct.StartVID - eEventStruct.StartReportID;
                foreach( ClassCvpGemConfig.Variable variable in data.VidList )
                {
                    if( data.Message.Count <= messageCount )
                    {
                        return; // メッセージが足りない場合、メソッドを終了
                    }

                    Match vidMatch = vidPattern.Match( data.Message[messageCount] );
                    if( !vidMatch.Success )
                    {
                        break; // マッチしない場合、次のレポートへスキップ
                    }

                    // "<"の前の先頭スペースを削除
                    string sml = data.Message[messageCount].TrimStart();
                    variable.sml = sml;
                    messageCount++;
                }
            }
        }

    }
}
