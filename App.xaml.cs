using GemLogAnalyzer.Common;
using GemLogAnalyzer.ViewModels;
using GemLogAnalyzer.Views;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;

namespace GemLogAnalyzer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// GeneralClassのインスタンス
        /// </summary>
        GeneralClass m_GeneralClass;

        protected override void OnStartup( StartupEventArgs e )
        {
            base.OnStartup( e );

            m_GeneralClass = GeneralClass.Instance;

            // タイトル画面を表示
            TitleWindow titleWindow = new TitleWindow();
            titleWindow.Show();

            // タイトル画面を見せたいがためにタイマーを使用する
            Stopwatch stopwatch = Stopwatch.StartNew();

            InitGemLogAnalyzer();
            // タイマーが3秒経つまで待機
            if( stopwatch.ElapsedMilliseconds < 3000 )
            {
                int remainingTime = 3000 - (int)stopwatch.ElapsedMilliseconds;
                Thread.Sleep( remainingTime );
            }

            //タイマー停止
            stopwatch.Stop();

            Application.Current.Dispatcher.Invoke( () =>
            {
                // MainWindowの新しいインスタンス作成
                MainWindow mainWindow = new MainWindow();

                if( !string.IsNullOrEmpty(m_GeneralClass.AnaConf.GemConfPath) )
                {
                    // GemConfPathにパスが入っていれば読み込み
                }

                if( !string.IsNullOrEmpty(m_GeneralClass.AnaConf.LogFilePath) )
                {
                    // LogFilePathにパスが入っていれば読み込み
                }


                // MainWindowを中央に表示
                mainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                mainWindow.Show();

                // 新しいメインウィンドウをアプリケーションのメインウィンドウに設定
                Application.Current.MainWindow = mainWindow;

                // TitleWindowを閉じる
                titleWindow.Close();
            } );
            // ウィンドウクローズイベント
            titleWindow.Close();

        }

        public void InitGemLogAnalyzer()
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
