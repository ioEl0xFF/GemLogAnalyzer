using GemLogAnalyzer.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GemLogAnalyzer.Common
{
    public class CommandOpenFileDialog : ICommand
    {
        /// <summary>
        /// MainViewModelのパラメータ
        /// </summary>
        MainViewModel m_MainViewModel;

        /// <summary>
        /// SettingDialogViewModelのパラメータ
        /// </summary>
        SettingDialogViewModel m_SettingDialogViewViewModel;

        /// <summary>
        /// GeneralClass
        /// </summary>
        GeneralClass m_GeneralClass;

        private readonly IDialogService m_dialogService;

        public CommandOpenFileDialog( MainViewModel vm )
        {
            m_MainViewModel = vm;
            m_GeneralClass = GeneralClass.Instance;
            m_dialogService = new DialogService();
        }

        public CommandOpenFileDialog( SettingDialogViewModel vm )
        {
            m_SettingDialogViewViewModel = vm;
            m_GeneralClass = GeneralClass.Instance;
            m_dialogService = new DialogService();
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
            return true;
        }

        /// <summary>
        /// 実行時の処理
        /// </summary>
        /// <param name="parameter"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Execute( object? parameter )
        {
            // デフォルトでMyDocumentsを指定
            string filePath = Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments );
            string filter = string.Empty;

            // どこから呼ばれたか
            string from = string.Empty;
            if( parameter is string paramStr )
            {
                from = paramStr;
            }

            if( File.Exists( m_GeneralClass.AnaConf.LogFilePath ) && from == "MainWindow" )
            {
                filePath = m_GeneralClass.AnaConf.LogFilePath;
                filter = "Log file|*.log";
            }else
            if( File.Exists( m_GeneralClass.AnaConf.GemConfPath ) && from == "SettingDialog")
            {
                filePath = m_GeneralClass.AnaConf.GemConfPath;
                filter = "Json file|*.json";
            }

            filePath = Path.GetDirectoryName( filePath );

            filePath = m_dialogService.OpenFileDialog( filePath, filter );

            if( File.Exists( filePath ) )
            {
                if( from == "MainWindow" )
                {
                    m_MainViewModel.CvpGemLogFilePath = filePath;
                    m_GeneralClass.AnaConf.LogFilePath = filePath;
                    CommandReadLog commandReadLog = new CommandReadLog( m_MainViewModel );
                    if( commandReadLog.CanExecute( parameter ) )
                    {
                        commandReadLog.Execute( parameter );
                    }
                }else
                if( from == "SettingDialog" )
                {
                    m_SettingDialogViewViewModel.CvpGemConfigFilePath = filePath;
                    m_GeneralClass.AnaConf.GemConfPath = filePath;
                    CommandLoadCvpConf commandLoadCvpConf = new CommandLoadCvpConf( m_SettingDialogViewViewModel );
                    if( commandLoadCvpConf.CanExecute( parameter ) )
                    {
                        commandLoadCvpConf.Execute( parameter );
                    }
                }
            }
        }
    }

    public interface IDialogService
    {
        string OpenFileDialog( string defaultPath, string filter);
    }

    public class DialogService : IDialogService
    {
        public string OpenFileDialog( string defaultPath, string filter)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                InitialDirectory = defaultPath
            };

            if( !string.IsNullOrEmpty(filter) )
            {
                openFileDialog.Filter = filter;
            }

            return openFileDialog.ShowDialog() == true ? openFileDialog.FileName : string.Empty;
        }
    }


}
