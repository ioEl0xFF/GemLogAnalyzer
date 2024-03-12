using GemLogAnalyzer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace GemLogAnalyzer.Common
{
    public class CommandSetDataGrid : ICommand
    {
        /// <sumamry>
        /// MainViewModelのパラメータ
        /// </sumamry>
        MainViewModel m_MainViewModel;

        public CommandSetDataGrid( MainViewModel mainViewModel)
        {
            m_MainViewModel = mainViewModel;
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
            if( parameter is not DataGrid )
                return false;
            return true;
        }

        /// <summary>
        /// 実行時の処理
        /// </summary>
        /// <param name="parameter"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Execute( object? parameter )
        {
            DataGrid dataGrid = parameter as DataGrid;
            m_MainViewModel.m_DataGridCvpGemLog = dataGrid;
        }

    }
}
