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

        public CommandReadLog(MainViewModel vm)
        {
            m_vm = vm;
            m_GeneralClass = GeneralClass.Instance;
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
        public bool CanExecute(object? parameter)
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
            m_GeneralClass.AnaConf.LogFilePath = filePath;
            return true;
        }

        /// <summary>
        /// 実行時の処理
        /// </summary>
        /// <param name="parameter"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Execute(object? parameter)
        {
            // ログを読み込む
            ReadCvpGemLogFile( m_GeneralClass.AnaConf.LogFilePath );
        }

        /// <summary>
        /// ログファイル読み込み処理
        /// </summary>
        private void ReadCvpGemLogFile(string argFilePath )
        {
            // DataGridをクリア
            m_vm.LogDatas.Clear();

            // Logデータもクリア
            m_GeneralClass.LogDatas.Clear();
            int LogDatasCount = 0;

            try
            {
                // 1行ずつ読み込み
                using( StreamReader reader = new StreamReader( argFilePath ) )
                {
                    string? readLine;
                    // ファイルの終わりまで読み込む
                    while((readLine = reader.ReadLine()) != null )
                    {
                        string line = readLine;
                        ClassCvpGemLogData data = new ClassCvpGemLogData();
                        
                        // SendReceiveFlag //////////
                        // <Send>
                        if( line.Contains("<Send>") )
                        {
                            data.SendReceiveFlag = eSendReceiveFlag.Send;
                        }

                        // <Receive>
                        if( line.Contains( "<Receive>" ) )
                        {
                            data.SendReceiveFlag = eSendReceiveFlag.Receive;
                        }

                        if( data.SendReceiveFlag != eSendReceiveFlag.None )
                        {
                            // Date /////////////////////
                            string format = "yyyy-MM-dd HH:mm:ss";

                            // 日時の部分のみを取り出すために、スペースで分割して最初の2つの要素を結合します
                            string dateTimePart = line.Split(new[] {' '}, 3)[0] + " " + line.Split(new[] {' '}, 3)[1];

                            try
                            {
                                // 日時フォーマットを指定してDateTime型に変換
                                data.Date = DateTime.ParseExact( dateTimePart, format, CultureInfo.InvariantCulture );
                            }
                            catch( FormatException )
                            {
                                continue;
                            }

                            // Message //////////
                            while( ( readLine = reader.ReadLine() ) != null )
                            {
                                if( readLine == "" )
                                {
                                    break;
                                }
                                line = readLine;
                                data.Message.Add(line);
                            }
                            data.MessageTitle = data.Message[0];

                            // S ////////////////
                            Regex pattern = new Regex( @"S(\d+)F" );
                            Match match = pattern.Match(data.MessageTitle);
                            data.Stream = int.Parse( match.Groups[1].Value );

                            // F ////////////////
                            pattern = new Regex( @"F(\d+)" );
                            match = pattern.Match(data.MessageTitle);
                            data.Function = int.Parse( match.Groups[1].Value );

                            // S6F11の時の処理
                            if( data.Stream == 6 && data.Function == 11 )
                            {
                                AnalyzeEvent( data );
                            }

                            // Viewに反映
                            m_vm.LogDatas.Add( new DataGridLogData
                            {
                                Date = data.Date.ToString(),
                                SendReceive = data.SendReceiveFlag.ToString(),
                                Stream = data.Stream,
                                Function = data.Function,
                                MessageTitle = data.MessageTitle,
                                DataNo = LogDatasCount
                            } ) ;

                            // LogDatasに保存
                            m_GeneralClass.LogDatas.Add( data.Clone() );
                            LogDatasCount++;
                        }
                    }

                }
            }
            catch(IOException e )
            {
                Console.WriteLine( e.Message );
                return;
            }
        }

        private void AnalyzeEvent(ClassCvpGemLogData data)
        {
            // イベント構成設定を取得
            ClassCvpGemConfig config = m_GeneralClass.CvpConf;
        
            // CEIDから設定内容を取得
            Regex ceidPattern = new Regex(@"<u4 (\d+)>$");
            Match ceidMatch = ceidPattern.Match(data.Message[(int)eEventStruct.CEID]);
            int ceid = int.Parse(ceidMatch.Groups[1].Value);
            ClassCvpGemConfig.Event eventModel = config.GetEventFromCeid(config.eventModel.events, ceid);
        
            // イベント名を取得し、メッセージタイトルを更新
            data.MessageTitle = eventModel.description;
        
            int messageCount = (int)eEventStruct.StartReportID;
            Regex reportPattern = new Regex(@"^      <u4 (\d+)>$");
            while (data.Message.Count > messageCount)
            {
                // レポート番号を取得
                Match reportMatch = reportPattern.Match(data.Message[messageCount]);
                if ( !reportMatch.Success )
                {
                    // パターンがマッチするまで探す
                    messageCount++;
                    continue;
                }
                int reportNo = int.Parse(reportMatch.Groups[1].Value);
        
                // レポートにリンクされているVIDをリストで取得
                List<ClassCvpGemConfig.Variable>? variableList = config.GetVariableListFromReportNo(reportNo);
                if (variableList == null)
                {
                    // VIDが見つからない場合は処理を終了
                    return;
                }
                data.VidList = new List<ClassCvpGemConfig.Variable>(variableList);
        
                messageCount += eEventStruct.StartVID - eEventStruct.StartReportID;

                Regex vidPattern = new Regex( @"<.*>" );
                foreach (ClassCvpGemConfig.Variable variable in data.VidList)
                {
                    if (data.Message.Count <= messageCount)
                    {
                        return; // メッセージ数を超えた場合は処理を終了
                    }

                    Match vidMatch = vidPattern.Match( data.Message[messageCount] );
                    if(  !vidMatch.Success )
                    {
                        break; // VIDパターンにマッチしない場合は次のレポート内容を探しに行く
                    }
        
                    // 始めのスペースを取り除く
                    string sml = data.Message[messageCount];
                    int index = sml.IndexOf("<");
                    if (index != -1)
                    {
                        // "<" の前の空白を取り除く
                        sml = sml.Substring(0, index).TrimStart() + sml.Substring(index);
                    }
        
                    variable.sml = sml;
                    messageCount++;
                }
            }
        }
    }
}
