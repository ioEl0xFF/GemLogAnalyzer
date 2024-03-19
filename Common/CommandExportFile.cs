using GemLogAnalyzer.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GemLogAnalyzer.Common
{
    public class CommandExportFile : ICommand
    {
        /// <summary>
        /// MainViewModelのパラメータ
        /// </summary>
        MainViewModel m_MainViewModel;

        /// <summary>
        /// GeneralClass
        /// </summary>
        GeneralClass m_GeneralClass;

        /// <summary>
        /// Tempファイルに保存したファイル名
        /// </summary>
        string m_TempFilePath;

        public CommandExportFile( MainViewModel argMainViewModel )
        {
            m_MainViewModel = argMainViewModel;
            m_GeneralClass = GeneralClass.Instance;
            m_TempFilePath = string.Empty;

            // Shift-Jisで保存するのに必要
            Encoding.RegisterProvider( CodePagesEncodingProvider.Instance );
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
            // Exportフォルダがあるか確認
            string exportDir = $"{Directory.GetCurrentDirectory()}\\{GeneralClass.ExportDirName}";
            if( !Directory.Exists( exportDir ) )
            {
                // 無かったら作成
                Directory.CreateDirectory( exportDir );
            }

            // Tempフォルダがあるか確認
            string tmpDir = $"{Directory.GetCurrentDirectory()}\\{GeneralClass.TempDirName}";
            if( !Directory.Exists( tmpDir ) )
            {
                // 無かったら作成
                Directory.CreateDirectory( tmpDir );
            }

            string logFilePath = m_GeneralClass.AnaConf.LogFilePath;
            if( !Path.Exists( logFilePath ) )
            {
                // ログファイルが存在してなかったら終了。
                return false;
            }

            // 現在読み込んでるログファイルをtmpフォルダにコピー
            string fileName = Path.GetFileName( logFilePath );
            m_TempFilePath = $"{tmpDir}\\{fileName}";
            if( File.Exists( m_TempFilePath ) )
            {
                // 既にコピー先に同じ名前のtmpファイルがあれば消す。
                File.Delete( m_TempFilePath );
            }
            File.Copy( logFilePath, m_TempFilePath );
            return true;
        }

        /// <summary>
        /// 実行時の処理
        /// </summary>
        /// <param name="parameter"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Execute( object? parameter )
        {
            // ログファイルを全て読み込む
            List<string> LogData = new List<string>( File.ReadAllLines( m_TempFilePath, Encoding.GetEncoding( "shift_jis" ) ) );

            if( LogData.Count > 0 )
            {
                if( ExportLogFile( LogData ) )
                {
                    string exportPath = $"{Directory.GetCurrentDirectory()}\\{GeneralClass.ExportDirName}\\{Path.GetFileName( m_TempFilePath )}";
                    if( File.Exists( exportPath ) )
                    {
                        // 既に同じ名前のファイルが有れば消す
                        File.Delete( exportPath );
                    }
                    File.WriteAllLines( exportPath, LogData, Encoding.GetEncoding( "shift_jis" ) );
                }
            }
        }

        private bool ExportLogFile( List<string> argLogDataList )
        {
            bool ret = true;

            for( int logDataCount = 0; logDataCount < argLogDataList.Count; logDataCount++ )
            {
                string logData = argLogDataList[logDataCount];
                Regex pattern = new Regex( @"S(\d+)F(\d+)" );
                Match match = pattern.Match( logData );

                // S6F11の時はイベント名とVID名を追記
                if( match.Success && match.Groups[1].Value == "6" && match.Groups[2].Value == "11" )
                {
                    logDataCount = AddDetails( argLogDataList, logDataCount, 35 );
                }
            }

            return ret;
        }

        private int AddDetails( List<string> argLogDataList, int argLogDataCount, int argMaxLength )
        {
            // S6F11の最後を取得
            int lastLine = argLogDataCount;
            while( argLogDataList[lastLine] != "" )
            {
                if( argLogDataList.Count <= lastLine )
                {
                    // ファイルが最後までいったらそのデータは破棄。
                    return 0;
                }

                lastLine++;
            }

            // イベント番号取得
            Regex ceidPattern = new Regex( @"<u4 (\d+)>$", RegexOptions.Compiled );
            Regex reportPattern = new Regex( @"^      <u4 (\d+)>$", RegexOptions.Compiled );

            // イベント構成設定を取得
            ClassCvpGemConfig config = m_GeneralClass.CvpConf;

            // CEIDから設定内容を取得
            argLogDataCount += (int)eEventStruct.CEID;
            Match ceidMatch = ceidPattern.Match( argLogDataList[argLogDataCount] );
            if( !ceidMatch.Success )
            {
                return lastLine; // CEIDが無効の場合、メソッドを終了
            }
            int ceid = int.Parse( ceidMatch.Groups[1].Value );

            ClassCvpGemConfig.Event eventModel = config.GetEventFromCeid( config.eventModel.events, ceid );
            if( eventModel == null )
            {
                return lastLine; // イベントモデルが見つからない場合、メソッドを終了
            }

            // イベント名を追記
            // argLogDataList.Insert(logDataCount, $"[{eventModel.description}]" );
            // logDataCount++;
            // lastLine++;
            argLogDataList[argLogDataCount] = argLogDataList[argLogDataCount].PadRight( argMaxLength + 5 ) + "// " + $"{eventModel.description}";


            // CVP側でイベント名を記載していた場合は削除
            if( argLogDataCount - 5 > 0 )
            {
                Regex ceidNamePattern = new Regex( eventModel.description );
                Match ceidNameMatch = ceidNamePattern.Match( argLogDataList[argLogDataCount - 5] );
                if(  ceidNameMatch.Success )
                {
                    argLogDataList.Remove( argLogDataList[argLogDataCount - 5] );
                    argLogDataCount--;
                    lastLine--;
                }
            }

            // VID領域を探索
            while( lastLine >= argLogDataCount )
            {
                // レポート番号を取得
                Match reportMatch = reportPattern.Match( argLogDataList[argLogDataCount] );
                if( !reportMatch.Success )
                {
                    // マッチしない場合は次のメッセージへスキップ
                    argLogDataCount++;
                    continue;
                }

                int reportNo = int.Parse( reportMatch.Groups[1].Value );
                if( !eventModel.reports.Contains( reportNo ) )
                {
                    // レポート番号がイベントモデルに含まれていない場合、スキップ
                    argLogDataCount++;
                    continue;
                }

                List<ClassCvpGemConfig.Variable>? variableList = config.GetVariableListFromReportNo( reportNo );
                if( variableList == null )
                {
                    // レポート番号にリンクされた変数が無ければデータを破棄。
                    return lastLine;
                }

                argLogDataCount += eEventStruct.StartVID - eEventStruct.StartReportID;
                foreach( ClassCvpGemConfig.Variable variable in variableList )
                {
                    if( argLogDataList.Count <= argLogDataCount )
                    {
                        // データが足りない場合は終了
                        return argLogDataList.Count;
                    }

                    // VID名を追記
                    if( variable != null && !string.IsNullOrEmpty( variable.description ) )
                    {
                        argLogDataList[argLogDataCount] = argLogDataList[argLogDataCount].PadRight(argMaxLength + 5) + "// " + $"{variable.id}".PadLeft(3) + $" : {variable.description}";
                    }
                    argLogDataCount++;
                }
            }

            return lastLine;
        }
    }
}
