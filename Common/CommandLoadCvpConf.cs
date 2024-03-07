using GemLogAnalyzer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Input;
using System.Text.RegularExpressions;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.IO;

namespace GemLogAnalyzer.Common
{
    public class CommandLoadCvpConf : ICommand
    {
        /// <summary>
        /// SettingDialogViewModelのパラメータ
        /// </summary>
        MainViewModel m_MainViewModel;

        /// <summary>
        /// SettingDialogViewModelのパラメータ
        /// </summary>
        SettingDialogViewModel m_SettingDialogViewModel;

        /// <summary>
        /// GeneralClass
        /// </summary>
        GeneralClass m_GeneralClass;

        public CommandLoadCvpConf(MainViewModel vm)
        {
            m_MainViewModel = vm;
            m_GeneralClass = GeneralClass.Instance;
        }
        public CommandLoadCvpConf(SettingDialogViewModel vm)
        {
            m_SettingDialogViewModel = vm;
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
        public bool CanExecute( object? parameter )
        {
            // ファイルが存在するか確認
            string filePath = m_GeneralClass.AnaConf.GemConfPath;
            string caller = parameter as string;
            switch( caller )
            {
                case "SettingDialog":
                    filePath = m_SettingDialogViewModel.CvpGemConfigFilePath;
                    break;
                default:
                    break;
            }

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
            string pattern = @"^CvpGemConf.json$";

            // ファイル名が正規表現(^CvpGem.*\.log$)に当てはまるか確認
            if( !Regex.IsMatch( fileName, pattern ) )
            {
                return false;
            }

            // ファイル名が正しそうやったら読み込み
            m_GeneralClass.AnaConf.GemConfPath = filePath;
            return true;
        }

        /// <summary>
        /// 実行時の処理
        /// </summary>
        /// <param name="parameter"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Execute(object? parameter)
        {
            string? jsonStr = string.Empty;
            using(StreamReader sr = new StreamReader( m_GeneralClass.AnaConf.GemConfPath ))
            {
                jsonStr = sr.ReadToEnd();
            }
            if( !string.IsNullOrEmpty(jsonStr))
            {
                ClassCvpGemConfig cfg = JsonSerializer.Deserialize<ClassCvpGemConfig>( jsonStr );
                if( cfg != null )
                {
                    m_GeneralClass.CvpConf = cfg;
                }
            }
        }
    }
}
