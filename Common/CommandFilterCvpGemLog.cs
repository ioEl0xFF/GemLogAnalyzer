﻿using GemLogAnalyzer.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GemLogAnalyzer.Common
{
    public class CommandFilterCvpGemLog : ICommand
    {
        /// <summary>
        /// MainViewModelのパラメータ
        /// </summary>
        MainViewModel m_MainViewModel;

        public CommandFilterCvpGemLog(MainViewModel mainViewModel)
        {
            m_MainViewModel = mainViewModel;
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
            if( string.IsNullOrWhiteSpace( m_MainViewModel.FilterPattern ) )
            {
                m_MainViewModel.FilteredLogDatas = new ObservableCollection<DataGridLogData>( m_MainViewModel.LogDatas );
            } else
            {
                // 正規表現パターンをFilterPatternから取得
                string pattern = m_MainViewModel.FilterPattern;

                // フィルタリング処理
                var result = m_MainViewModel.LogDatas.Where( d =>
                {
                    // Regex.IsMatchメソッドを使用して、各フィールドが正規表現パターンに一致するかどうかを確認
                    //return Regex.IsMatch( d.Date.ToLowerInvariant(), pattern, RegexOptions.IgnoreCase )
                    //    || Regex.IsMatch( d.SendReceive.ToLowerInvariant(), pattern, RegexOptions.IgnoreCase )
                    //    || Regex.IsMatch( d.Stream.ToString(), pattern )
                    //    || Regex.IsMatch( d.Function.ToString(), pattern )
                    //    || Regex.IsMatch( d.MessageTitle.ToLowerInvariant(), pattern, RegexOptions.IgnoreCase )
                    //    || Regex.IsMatch( d.DataNo.ToString(), pattern );
                    return Regex.IsMatch( d.MessageTitle, pattern );
                } );

                m_MainViewModel.FilteredLogDatas = new ObservableCollection<DataGridLogData>( result );
            }
        }
    }
}
