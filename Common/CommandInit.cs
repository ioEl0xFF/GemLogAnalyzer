using GemLogAnalyzer.ViewModels;
using GemLogAnalyzer.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace GemLogAnalyzer.Common
{
    internal class CommandInit : ICommand
    {
        /// <summary>
        /// TitleWindowViewModelのパラメータ
        /// </summary>
        TitleWindowViewModel m_vm;

        /// <summary>
        /// GeneralClassのインスタンス
        /// </summary>
        GeneralClass m_GeneralClass;

        public CommandInit(TitleWindowViewModel vm)
        {
            m_vm = vm;
            m_GeneralClass = GeneralClass.Instance;
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove {  CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// コマンドが利用可能かどうか
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool CanExecute( object? parameter )
        {
            return true;
        }

        /// <summary>
        /// 実行時の処理
        /// </summary>
        /// <param name="parameter"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Execute(object? parameter)
        {
            // タイトル画面を見せたいがためにタイマーを使用する
            Stopwatch stopwatch = Stopwatch.StartNew();

            InitGemLogAnalyzerConfig();

            // タイマーが1秒経つまで待機
            if( stopwatch.ElapsedMilliseconds < 3000 )
            {
                int remainingTime = 3000 - (int)stopwatch.ElapsedMilliseconds;
                Thread.Sleep( remainingTime );
            }

            //タイマー停止
            stopwatch.Stop();

            // ウィンドウクローズイベント
            if( Application.Current.MainWindow is TitleWindow titleWindow )
            {
                titleWindow.Close();
            }
        }

        public void InitGemLogAnalyzerConfig()
        {
            // カレントディレクトリにConfigフォルダがあるか確認
            string dirName = GeneralClass.GemLogAnalyzerConfigDirName;
            if( !Directory.Exists(dirName) )
            {
                // 無ければ作成
                Directory.CreateDirectory(dirName);
            }

            // ConfigフォルダにGemLogAnalyzerConf.jsonがあるか確認
            string filePath = $"{GeneralClass.GemLogAnalyzerConfigDirName}\\{GeneralClass.GemLogAnalyzerConfigFileName}";
            if( !File.Exists(filePath) )
            {
                // 無ければ作成して終了
                SaveGemLogAnalyzerConfig();
                return;
            }

            // Configファイル読み込み
            LoadGemLogAnalyzerConfig();
        }

        /// <summary>
        /// GemLogAnalyzerConf.jsonに保存
        /// </summary>
        public void SaveGemLogAnalyzerConfig()
        {
            var options = new JsonSerializerOptions
            {
                // インデント処理を行う
                WriteIndented = true,
            };
            string jsonStr = JsonSerializer.Serialize( m_GeneralClass.AnaConf, options );

            string filePath = $"{GeneralClass.GemLogAnalyzerConfigDirName}\\{GeneralClass.GemLogAnalyzerConfigFileName}";
            File.WriteAllText( filePath, jsonStr );
        }

        /// <summary>
        /// GemLogAnalyzerConf.jsonの読み込み
        /// </summary>
        public void LoadGemLogAnalyzerConfig()
        {
            string? jsonStr = string.Empty;
            string filePath = $"{GeneralClass.GemLogAnalyzerConfigDirName}\\{GeneralClass.GemLogAnalyzerConfigFileName}";
            using( StreamReader sr = new StreamReader( filePath ) )
            {
                jsonStr = sr.ReadToEnd();
            }

            if( !string.IsNullOrEmpty( jsonStr ) )
            {
                ClassGemAnalyzerConfig cfg = JsonSerializer.Deserialize<ClassGemAnalyzerConfig>( jsonStr );
                if( cfg != null )
                {
                    m_GeneralClass.AnaConf = cfg;
                }
            }
        }
    }
}
