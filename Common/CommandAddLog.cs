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
    public class CommandAddLog : ICommand
    {
        /// <summary>
        /// MainViewModelのパラメータ
        /// </summary>
        MainViewModel m_MainViewModel;

        /// <summary>
        /// GeneralClass
        /// </summary>
        GeneralClass m_GeneralClass;

        public CommandAddLog(MainViewModel mainViewModel)
        {
            m_MainViewModel = mainViewModel;
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
            // ファイルが存在するか確認
            string filePath = m_MainViewModel.CvpGemLogFilePath;
            if( filePath.Length <= 0 )
            {
                return false;
            }

            if( !File.Exists( filePath ) )
            {
                return false;
            }

            if( filePath != m_GeneralClass.AnaConf.LogFilePath )
            {
                return false;
            }

            // ファイル名が正しそうやったら読み込み
            return true;
        }

        /// <summary>
        /// 実行時の処理
        /// </summary>
        /// <param name="parameter"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Execute( object? parameter )
        {

        }
    }
}
