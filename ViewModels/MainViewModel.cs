﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GemLogAnalyzer.Common;
using GemLogAnalyzer.Models;
using System.ComponentModel;
using System.Windows.Input;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.IO;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows;


namespace GemLogAnalyzer.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        //////////////////////////////////////////////////////
        // Field /////////////////////////////////////////////
        //////////////////////////////////////////////////////
        #region
        /// <summary>
        /// モデルパラメータを格納するためのフィールドです。
        /// </summary>
        private MainModel m_MainModel = new MainModel();

        /// <summary>
        /// シングルトンパターンで実装されたGeneralClassのインスタンス。
        /// </summary>
        private GeneralClass m_GeneralClass = GeneralClass.Instance;

        /// <summary>
        /// DataGridCvpGemLog
        /// </summary>
        public DataGrid m_DataGridCvpGemLog = new DataGrid();
        #endregion

        //////////////////////////////////////////////////////
        // Command Field /////////////////////////////////////
        //////////////////////////////////////////////////////
        #region
        /// <summary>
        /// GemLogAnalyzerConf.jsonを保存するためのコマンド。
        /// </summary>
        private CommandSaveLogAnaConf m_CommandSaveConf;

        /// <summary>
        /// GemLogAnalyzerConf.jsonを読み込むためのコマンド。
        /// </summary>
        private CommandLoadCvpConf m_CommandLoadConf;

        /// <summary>
        /// CvpGem.logを読み込むためのコマンド。
        /// </summary>
        private CommandReadLog m_CommandReadLog;

        /// <summary>
        /// 設定画面を開くためのコマンド。
        /// </summary>
        private CommandOpenSettingDialog m_CommandOpenSettingDialog;

        /// <summary>
        /// Detailを表示するコマンド。
        /// </summary>
        private CommandShowEventDetail m_CommandShowEventDetail;

        /// <summary>
        /// ファイル選択ダイアログ表示コマンド。
        /// </summary>
        private CommandOpenFileDialog m_CommandOpenFileDialog;

        /// <summary>
        /// DataGridCvpGemLog(ScrollViewer更新用)
        /// </summary>
        private CommandSetDataGrid m_CommandSetDataGrid;

        /// <summary>
        /// DataGridCvpGemLogフィルタコマンド
        /// </summary>
        private CommandFilterCvpGemLog m_CommandFilterCvpGemLog;

        /// <summary>
        /// Exportコマンド
        /// </summary>
        private CommandExportFile m_CommandExportFile;
        #endregion

        //////////////////////////////////////////////////////
        // Command Property //////////////////////////////////
        //////////////////////////////////////////////////////
        #region
        /// <summary>
        /// 設定を保存するコマンド。
        /// </summary>
        public CommandSaveLogAnaConf CommandSaveConf => m_CommandSaveConf;

        /// <summary>
        /// 設定を読み込むコマンド。
        /// </summary>
        public CommandLoadCvpConf CommandLoadConf => m_CommandLoadConf;

        /// <summary>
        /// ログを読み込むコマンド。
        /// </summary>
        public CommandReadLog CommandReadLog => m_CommandReadLog;

        /// <summary>
        /// 設定画面を開くコマンド。
        /// </summary>
        public CommandOpenSettingDialog CommandOpenSettingDialog => m_CommandOpenSettingDialog;

        /// <summary>
        /// Detailを表示するコマンド。
        /// </summary>
        public CommandShowEventDetail CommandShowEventDetail => m_CommandShowEventDetail;

        /// <summary>
        /// ファイル選択ダイアログ表示コマンド。
        /// </summary>
        public CommandOpenFileDialog CommandOpenFileDialog => m_CommandOpenFileDialog;

        /// <summary>
        /// m_DataGridCvpGemLogに登録
        /// </summary>
        public CommandSetDataGrid CommandSetDataGrid => m_CommandSetDataGrid;

        /// <summary>
        /// DataGridCvpGemLogのフィルタ処理
        /// </summary>
        public CommandFilterCvpGemLog CommandFilterCvpGemLog => m_CommandFilterCvpGemLog;

        /// <summary>
        /// ファイルExport処理
        /// </summary>
        public CommandExportFile CommandExportFile => m_CommandExportFile;
        #endregion

        //////////////////////////////////////////////////////
        // Property //////////////////////////////////////////
        //////////////////////////////////////////////////////
        #region
        /// <summary>
        /// Log用DataGridに表示されるログデータのコレクション。
        /// </summary>
        public ObservableCollection<DataGridLogData> LogDatas
        {
            get => m_MainModel.LogDatas;
            set
            {
                if( m_MainModel.LogDatas != value )
                {
                    m_MainModel.LogDatas = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// DetailVidList用DataGridに表示されるログデータのコレクション。
        /// </summary>
        public ObservableCollection<DataGridDetail> Details
        {
            get => m_MainModel.VidLists;
            set
            {
                if( m_MainModel.VidLists != value )
                {
                    m_MainModel.VidLists = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// DataGridで選択された値を保存。
        /// </summary>
        public DataGridLogData SelectedItem
        {
            get => m_MainModel.SelectedItem;
            set
            {
                if( m_MainModel.SelectedItem != value )
                {
                    m_MainModel.SelectedItem = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// CvpGem.logファイルのパスを取得または設定します。値が変更された場合は通知されます。
        /// </summary>
        public string CvpGemLogFilePath
        {
            get => m_MainModel.CvpGemLogFilePath;
            set
            {
                if( m_MainModel.CvpGemLogFilePath != value )
                {
                    m_MainModel.CvpGemLogFilePath = value;
                    NotifyPropertyChanged();
                }
            }
        }

        // DataGridCvpGemLog用フィルター //////////////
        public ObservableCollection<DataGridLogData> FilteredLogDatas
        {
            get => m_MainModel.FilteredLogDatas;
            set
            {
                m_MainModel.FilteredLogDatas = value;
                NotifyPropertyChanged();
            }
        }
        /// <summary>
        /// メッセージのフィルタ設定
        /// </summary>
        public string FilterPattern
        {
            get => m_MainModel.FilterPattern;
            set
            {
                if( m_MainModel.FilterPattern != value )
                {
                    m_MainModel.FilterPattern = value;
                    NotifyPropertyChanged();
                }
            }
        }

        // Detail /////////////////////////////////////
        /// <summary>
        /// Time
        /// </summary>
        public string DetailDate
        {
            get => m_MainModel.DetailDate;
            set
            {
                if( m_MainModel.DetailDate != value )
                {
                    m_MainModel.DetailDate = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Message
        /// </summary>
        public string DetailMessageTitle
        {
            get => m_MainModel.DetailMessageTitle;
            set
            {
                if( m_MainModel.DetailMessageTitle != value )
                {
                    m_MainModel.DetailMessageTitle = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

        /// <summary>
        /// コンストラクタ。各コマンドの初期化と初期設定を行います。
        /// </summary>
        public MainViewModel()
        {
            m_CommandSaveConf = new CommandSaveLogAnaConf( this );
            m_CommandLoadConf = new CommandLoadCvpConf( this );
            m_CommandReadLog = new CommandReadLog( this );
            m_CommandOpenSettingDialog = new CommandOpenSettingDialog( this );
            m_CommandShowEventDetail = new CommandShowEventDetail( this );
            m_CommandOpenFileDialog = new CommandOpenFileDialog( this );
            m_CommandSetDataGrid = new CommandSetDataGrid( this );
            m_CommandFilterCvpGemLog = new CommandFilterCvpGemLog( this );
            m_CommandExportFile = new CommandExportFile( this );

            // タイマーを設置して、定期的にファイルの更新を確認する。
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds( 500 );
            timer.Tick += Timer_Tick;

            timer.Start();

            // GeneralClassから設定ファイルのパスを取得し、プロパティに設定
            CvpGemLogFilePath = m_GeneralClass.AnaConf.LogFilePath;
        }

        /// <summary>
        /// INotifyPropertyChangedインターフェースの実装。プロパティが変更された際にイベントを発火させる。
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// プロパティの変更を通知するヘルパーメソッド。
        /// </summary>
        /// <param name="propertyName">変更されたプロパティの名前（コンパイラによって自動的に設定されます）。</param>
        protected void NotifyPropertyChanged( [CallerMemberName] string propertyName = "" )
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
        }

        private void Timer_Tick( object? sender, EventArgs e )
        {
            if( m_CommandReadLog.CanExecute( null ) )
            {
                DateTime? lastTime = File.GetLastWriteTime( m_GeneralClass.AnaConf.LogFilePath );
                if( lastTime == null )
                {
                    return;
                }
                if( lastTime > m_GeneralClass.AnaConf.LogFileDate )
                {
                    // ScrollViewerを取得
                    if( m_DataGridCvpGemLog == null )
                    {
                        return;
                    }
                    var scrollViewer = GetScrollViewer( m_DataGridCvpGemLog );
                    bool isAtBottomScroll = false;

                    if( scrollViewer != null )
                    {
                        // 現在のスクロール位置が一番下にあるかを判断
                        isAtBottomScroll = scrollViewer.VerticalOffset >= scrollViewer.ScrollableHeight;
                    }

                    // SelectedItemが最後のセルを選択しているか判定
                    bool isTheLastCell = false;
                    if( FilteredLogDatas.Count > 0 && SelectedItem != null )
                    {
                        isTheLastCell = FilteredLogDatas[FilteredLogDatas.Count - 1].DataNo == SelectedItem.DataNo;
                    }

                    // ログを再読み込み
                    m_CommandReadLog.Execute( null );

                    // スクロール位置が一番下だった場合、再度一番下にスクロール
                    if( isAtBottomScroll && scrollViewer != null )
                    {
                        scrollViewer.ScrollToEnd();
                    }
                    // 最後のセルを選択していた場合、再度最後のセルを選択
                    if( isTheLastCell && FilteredLogDatas.Count > 0 ) 
                    {
                        m_DataGridCvpGemLog.SelectedItem = FilteredLogDatas[FilteredLogDatas.Count - 1];
                    }
                }
            }
        }

        // DataGrid内のScrollViewerを取得するヘルパーメソッド
        private ScrollViewer GetScrollViewer( DependencyObject dependencyObject )
        {
            if( dependencyObject is ScrollViewer )
            {
                return dependencyObject as ScrollViewer;
            }

            for( int i = 0; i < VisualTreeHelper.GetChildrenCount( dependencyObject ); i++ )
            {
                var child = VisualTreeHelper.GetChild( dependencyObject, i );
                var result = GetScrollViewer( child );
                if( result != null )
                {
                    return result;
                }
            }
            return null;
        }
    }


    // Log表示用DataGrid //////////////////////////////////
    public class DataGridLogData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void RaisePropertyChanged( [CallerMemberName]string propertyName = null)
        {
            var handler = PropertyChanged;
            if( handler != null )
            {
                handler ( this, new PropertyChangedEventArgs(propertyName) );
            }
        }

        private string date;
        public string Date
        {
            get { return date; }
            set
            {
                if( date != value )
                {
                    date = value;
                    RaisePropertyChanged();
                }
            }
        }
        private string sendReceive;
        public string SendReceive
        {
            get { return sendReceive; }
            set
            {
                if( sendReceive != value )
                {
                    sendReceive = value;
                    RaisePropertyChanged();
                }
            }
        }
        private int stream;
        public int Stream
        {
            get { return stream; }
            set
            {
                if( stream != value )
                {
                    stream = value;
                    RaisePropertyChanged();
                }
            }
        }
        private int function;
        public int Function
        {
            get { return function; }
            set
            {
                if( function != value )
                {
                    function = value;
                    RaisePropertyChanged();
                }
            }
        }
        string messageTitle;
        public string MessageTitle
        {
            get { return messageTitle; }
            set
            {
                if( messageTitle != value )
                {
                    messageTitle = value;
                    RaisePropertyChanged();
                }
            }
        }

        int dataNo;
        // GeneralClassと紐づけの為の番号
        public int DataNo 
        {
            get { return dataNo; }
            set
            {
                if( dataNo != value )
                {
                    dataNo = value;
                    RaisePropertyChanged();
                }
            }
        }

        public DataGridLogData()
        {
            Date = string.Empty;
            SendReceive = string.Empty;
            Stream = 0;
            Function = 0;
            MessageTitle = string.Empty;
            DataNo = 0;
        }

        public DataGridLogData( DataGridLogData data )
        {
            Date = data.Date;
            SendReceive = data.SendReceive;
            Stream = data.Stream;
            Function = data.Function;
            MessageTitle = data.MessageTitle;
            DataNo = data.DataNo;
        }
    }

    // VIDリスト表示用DataGrid //////////////////////////////////
    public class DataGridDetail : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void RaisePropertyChanged( [CallerMemberName]string propertyName = null)
        {
            var handler = PropertyChanged;
            if( handler != null )
            {
                handler ( this, new PropertyChangedEventArgs(propertyName) );
            }
        }

        int vidNo;
        // VID番号
        public int No
        {
            get { return vidNo; }
            set
            {
                if(  vidNo != value )
                {
                    vidNo = value;
                    RaisePropertyChanged();
                }
            }
        }

        string name;
        // VID名
        public string Name
        {
            get { return name; }
            set
            {
                if( name != value )
                {
                    name = value;
                    RaisePropertyChanged();
                }
            }
        }

        string sml;
        // SML
        public string Value
        {
            get { return sml; }
            set
            {
                if( sml != value )
                {
                    sml = value;
                    RaisePropertyChanged();
                }
            }
        }

        public DataGridDetail()
        {
            No = 0;
            Name = string.Empty;
            Value = string.Empty;
        }
    }
}
