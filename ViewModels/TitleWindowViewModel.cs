using GemLogAnalyzer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using GemLogAnalyzer.Common;

namespace GemLogAnalyzer.ViewModels
{
    internal class TitleWindowViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// モデルパラメータ格納用
        /// </summary>
        private TitleWindowModel m_TitleWindowModel;

        // Command ////////////////

        public TitleWindowViewModel()
        {
            m_TitleWindowModel = new TitleWindowModel();
        }

        /// <summary>
        /// 通知イベント
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// プロパティの変更通知を起動する
        /// </summary>
        /// <param name="propertyName">プロパティ名</param>
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
