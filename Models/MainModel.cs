using GemLogAnalyzer.Common;
using GemLogAnalyzer.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GemLogAnalyzer.Models
{
    public class MainModel
    {
        public string CvpGemLogFilePath { get; set; } = string.Empty;
        public ObservableCollection<DataGridLogData> LogDatas { get; set; } = new ObservableCollection<DataGridLogData>();
        public ObservableCollection<DataGridDetail> VidLists { get; set; } = new ObservableCollection<DataGridDetail>();
        public DataGridLogData SelectedItem { get; set; } = new DataGridLogData();
        public string DetailMessageTitle { get; set; } = string.Empty;
        public string DetailDate { get; set; } = string.Empty;
        public string FilterPattern {  get; set; } = string.Empty;
        public ObservableCollection<DataGridLogData> FilteredLogDatas = new ObservableCollection<DataGridLogData>();
    }
}
