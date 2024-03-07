using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GemLogAnalyzer.Common;

namespace GemLogAnalyzer
{
    public class GeneralClass
    {
        //////////////////////////////////////////////////////
        // プロパティ
        //////////////////////////////////////////////////////
        /// <summary>
        /// CvpGemConf.jsonで読み込んだ設定値
        /// </summary>
        public ClassCvpGemConfig CvpConf { get; set; } = new ClassCvpGemConfig();

        /// <summary>
        /// GemLogAnalyzer.jsonで読み込んだ設定値
        /// </summary>
        public ClassGemAnalyzerConfig AnaConf { get; set; } = new ClassGemAnalyzerConfig();

        /// <summary>
        /// ログデータ
        /// </summary>
        public List<ClassCvpGemLogData> LogDatas { get; set; } = new List<ClassCvpGemLogData>();

        //////////////////////////////////////////////////////
        // 定数
        //////////////////////////////////////////////////////
        // GemLogAnalyzerConfigのファイル名
        public const string GemLogAnalyzerConfigFileName = "GemLogAnalyzerConf.json";
        // GemLogAnalyzerConfigフォルダ名
        public const string GemLogAnalyzerConfigDirName = "Config";

        //////////////////////////////////////////////////////
        // シングルトン設定
        //////////////////////////////////////////////////////
        private static GeneralClass m_Instance = null;

        private GeneralClass()
        {
        }

        public static GeneralClass Instance
        {
            get
            {
                if( m_Instance == null )
                {
                    m_Instance = new GeneralClass();
                }
                return m_Instance;
            }
        }

        //////////////////////////////////////////////////////
        // publicメソッド
        //////////////////////////////////////////////////////
    }

    public enum eSendReceiveFlag
    {
        None = 0,
        Send,
        Receive,
    }

    public enum eEventStruct
    {
        Title = 0,
        CEID = 3,
        StartReportID = 6,
        StartVID=8,
    }
}
