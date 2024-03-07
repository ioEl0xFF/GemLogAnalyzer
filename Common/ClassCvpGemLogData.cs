using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GemLogAnalyzer.Common
{
    public class ClassCvpGemLogData
    {
        /// <summary>
        /// DataTime
        /// </summary>
        public DateTime Date { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Send/Receive Flag
        /// </summary>
        public eSendReceiveFlag SendReceiveFlag { get; set; } = eSendReceiveFlag.None;

        /// <summary>
        /// Stream
        /// </summary>
        public int Stream { get; set; } = 0;

        /// <summary>
        /// Function
        /// </summary>
        public int Function { get; set; } = 0;

        /// <summary>
        /// メッセージタイトル
        /// </summary>
        public string MessageTitle { get; set; } = string.Empty;

        /// <summary>
        /// メッセージ内容
        /// </summary>
        public List<string> Message { get; set; } = new List<string>();

        /// <summary>
        /// VIDリスト
        /// </summary>
        public List<ClassCvpGemConfig.Variable> VidList { get; set; } = new List<ClassCvpGemConfig.Variable>();

        public ClassCvpGemLogData()
        {
            Date = DateTime.MinValue;
            SendReceiveFlag = eSendReceiveFlag.None;
            Stream = 0;
            Function = 0;
            MessageTitle = string.Empty;
            Message = new List<string>();
            VidList = new List<ClassCvpGemConfig.Variable>();
        }

        public ClassCvpGemLogData Clone()
        {
            var clonedData = new ClassCvpGemLogData
            {
                Date = this.Date,
                SendReceiveFlag = this.SendReceiveFlag,
                Stream = this.Stream,
                Function = this.Function,
                MessageTitle = this.MessageTitle,
                // List<string>とList<ClassCvpGemConfig.Variable>の深いコピーを作成
                Message = new List<string>(this.Message),
                // Variableクラスが深いコピーをサポートしていると仮定
                VidList = this.VidList.Select(variable => variable.Clone()).ToList()
            };
            return clonedData;
        }
    }
}
