using GemLogAnalyzer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GemLogAnalyzer.Common
{
    public class CommandShowEventDetail : ICommand
    {
        /// <summary>
        /// MainViewModelのパラメータ
        /// </summary>
        MainViewModel m_vm;

        /// <summary>
        /// GeneralClass
        /// </summary>
        GeneralClass m_GeneralClass;

        /// <summary>
        /// LogDataに対応した番号
        /// </summary>
        ClassCvpGemLogData m_LogData;

        public CommandShowEventDetail(MainViewModel vm)
        {
            m_vm = vm;
            m_GeneralClass = GeneralClass.Instance;
            m_LogData= new ClassCvpGemLogData();
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// コマンドが利用可能かどうか
        /// </summary>
        /// <param name="parameter">SelectedItem:LogData型のアイテムが入る</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool CanExecute( object? parameter )
        {
            // パラメータをLogData型に安全にキャスト
            DataGridLogData? selectedItem = parameter as DataGridLogData;

            // selectedItemがnullの場合は、コマンドを実行できない
            if( selectedItem == null || selectedItem.Date == "" ) return false;

            if( m_GeneralClass.LogDatas[selectedItem.DataNo] == null ) return false;

            m_LogData = m_GeneralClass.LogDatas[selectedItem.DataNo];

            return true;
        }

        /// <summary>
        /// 実行時の処理
        /// </summary>
        /// <param name="parameter"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Execute(object? parameter)
        {
            // 時間とタイトルを登録
            m_vm.DetailDate = m_LogData.Date.ToString();
            m_vm.DetailMessageTitle = m_LogData.MessageTitle;
            
            // VidListsをクリア
            m_vm.Details.Clear();
            if( m_LogData.Stream == 6 && m_LogData.Function == 11 )
            {
                // S6F11:VIDリストを作成。
                for( int vidCount = 0; vidCount < m_LogData.VidList.Count; vidCount++ )
                {
                    m_vm.Details.Add( new DataGridDetail
                    {
                        No = m_LogData.VidList[vidCount].id,
                        Name = m_LogData.VidList[vidCount].description,
                        Value = m_LogData.VidList[vidCount].sml
                    } );
                }
            } else
            {
                // SMLを表示
                for ( int messageCount = 0; messageCount < m_LogData.Message.Count; messageCount++ )
                {
                    m_vm.Details.Add( new DataGridDetail
                    {
                        No = messageCount,
                        Name = string.Empty,
                        Value = m_LogData.Message[messageCount]
                    } );
                }
            }
        }
    }
}
