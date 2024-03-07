using GemLogAnalyzer.ViewModels;
using GemLogAnalyzer.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GemLogAnalyzer.Common
{
    public class CommandOpenSettingDialog : ICommand
    {
        /// <summary>
        /// MainViewModelのパラメータ
        /// </summary>
        /// <param name="vm"></param>
        MainViewModel m_vm;

        public CommandOpenSettingDialog(MainViewModel vm)
        {
            m_vm = vm;
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
            SettingDialog settingDialog = new SettingDialog();

            if( parameter is MainWindow mainWindow )
            {
                settingDialog.Owner = mainWindow;
                settingDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            } else
            {
                settingDialog.WindowStartupLocation = WindowStartupLocation.Manual;
            }

            settingDialog.ShowDialog();
        }
    }
}
