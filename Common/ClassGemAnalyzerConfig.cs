using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GemLogAnalyzer.Common
{
    public class ClassGemAnalyzerConfig
    {
        public string LogFilePath { get; set; } = string.Empty;
        public string GemConfPath { get; set; } = string.Empty;
        public DateTime LogFileDate { get; set; } = DateTime.MinValue;

        public ClassGemAnalyzerConfig()
        {
            LogFilePath = string.Empty;
            GemConfPath = string.Empty;
            LogFileDate = DateTime.MinValue;
        }
    }
}
