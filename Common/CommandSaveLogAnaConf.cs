using GemLogAnalyzer.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using GemLogAnalyzer.ViewModels;

namespace GemLogAnalyzer.Common
{
    public class CommandSaveLogAnaConf : ICommand
    {
        /// <summary>
        /// MainViewModelのパラメータ
        /// </summary>
        MainViewModel m_vm;

        /// <summary>
        /// GeneralClassのインスタンス
        /// </summary>
        GeneralClass m_GeneralClass;

        public CommandSaveLogAnaConf(MainViewModel vm )
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

        public void Execute( object? parameter ) 
        { 
            // 現在のGeneralClassの値をjsonファイルに書き込み
            // カレントディレクトリにConfigフォルダがあるか確認
            string dirName = GeneralClass.GemLogAnalyzerConfigDirName;
            if( !Directory.Exists(dirName) )
            {
                // 無ければ作成
                Directory.CreateDirectory(dirName);
            }

            // ConfigフォルダにGemLogAnalyzerConf.jsonがあるか確認
            string filePath = $"{GeneralClass.GemLogAnalyzerConfigDirName}\\{GeneralClass.GemLogAnalyzerConfigFileName}";
            if( File.Exists(filePath) )
            {
                // あれば削除
                File.Delete(filePath);
            }
            SaveGemLogAnalyzerConfig();
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
    }
}
