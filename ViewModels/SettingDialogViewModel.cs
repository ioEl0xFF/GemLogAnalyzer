using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GemLogAnalyzer.Common;
using GemLogAnalyzer.Models;
using GemLogAnalyzer.Views;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GemLogAnalyzer.ViewModels
{
    public class SettingDialogViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// モデルパラメータ格納用
        /// </summary>
        private SettingDialogModel m_SettingDialogModel;

        /// <summary>
        /// GeneralClassのインスタンス
        /// </summary>
        private GeneralClass m_GeneralClass;
        
        // Command ///////////////////
        /// <summary>
        /// CvpGem.log 読み込みコマンド
        /// </summary>
        private CommandLoadCvpConf m_CommandReadConfig;

        /// <summary>
        /// ファイル選択ダイアログ表示コマンド。
        /// </summary>
        private CommandOpenFileDialog m_CommandOpenFileDialog;

        public SettingDialogViewModel()
        {
            m_SettingDialogModel = new SettingDialogModel();
            m_GeneralClass = GeneralClass.Instance;
            m_CommandReadConfig = new CommandLoadCvpConf(this);
            m_CommandOpenFileDialog = new CommandOpenFileDialog(this);

            CvpGemConfigFilePath = m_GeneralClass.AnaConf.GemConfPath;
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

        /// <summary>
        /// CvpGem.logファイルのパス
        /// </summary>
        public string CvpGemConfigFilePath
        {
            get { return m_SettingDialogModel.CvpGemConfigFilePath; }
            set
            {
                if( m_SettingDialogModel.CvpGemConfigFilePath !=  value )
                {
                    m_SettingDialogModel.CvpGemConfigFilePath = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// TextBoxCvpGemConfigFilePathでEnterが押されたらコマンド実行
        /// </summary>
        public CommandLoadCvpConf CommandReadConfig => m_CommandReadConfig;

        /// <summary>
        /// ファイル選択ダイアログ表示コマンド。
        /// </summary>
        public CommandOpenFileDialog CommandOpenFileDialog => m_CommandOpenFileDialog;
    }
}
