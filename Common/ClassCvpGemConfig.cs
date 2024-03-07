using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GemLogAnalyzer.Common
{
    public class ClassCvpGemConfig
    {
        public Alarmmodel alarmModel { get; set; }
        public Commmodel commModel { get; set; }
        public Ctrlmodel ctrlModel { get; set; }
        public Eventmodel eventModel { get; set; }
        public Hsms hsms { get; set; }


        [JsonConstructor]
        public ClassCvpGemConfig() 
        {
            alarmModel = new Alarmmodel();
            commModel = new Commmodel();
            ctrlModel = new Ctrlmodel();
            eventModel = new Eventmodel();
            hsms = new Hsms();
        }


        public class Alarmmodel
        {
            public Alarm[] alarms { get; set; }

            public Alarmmodel()
            {
                alarms = new Alarm[0];
            }
        }

        public class Alarm
        {
            public int code { get; set; }
            public bool enable { get; set; }
            public int id { get; set; }
            public string text { get; set; }

            public Alarm()
            {
                code = 0;
                enable = false;
                id = 0;
                text = string.Empty;
            }
        }

        public class Commmodel
        {
            public bool establishComm { get; set; }
            public int initialState { get; set; }
            public string mdln { get; set; }
            public string softrev { get; set; }

            public Commmodel()
            {
                establishComm = false;
                initialState = 0;
                mdln = string.Empty;
                softrev = string.Empty;
            }
        }

        public class Ctrlmodel
        {
            public int initialOfflineState { get; set; }
            public int initialState { get; set; }
            public int offlineState { get; set; }
            public int onlineState { get; set; }

            public Ctrlmodel()
            {
                initialOfflineState = 0;
                initialState = 0;
                offlineState = 0;
                onlineState = 0;
            }
        }

        public class Eventmodel
        {
            public Event[] events { get; set; }
            public Report[] reports { get; set; }
            public Variable[] variables { get; set; }

            public Eventmodel()
            {
                events = new Event[0];
                reports = new Report[0];
                variables = new Variable[0];
            }
        }

        public class Event
        {
            public int definition { get; set; }
            public string description { get; set; }
            public bool enable { get; set; }
            public int id { get; set; }
            public int[] reports { get; set; }

            public Event()
            {
                definition = 0;
                description = string.Empty;
                enable = false;
                id = 0;
                reports = new int[0];
            }
        }

        public class Report
        {
            public string description { get; set; }
            public int id { get; set; }
            public int[] variables { get; set; }

            public Report()
            {
                description = string.Empty;
                id = 0;
                variables = new int[0];
            }
        }

        public class Variable
        {
            public string defaultValue { get; set; }
            public int definition { get; set; }
            public string description { get; set; }
            public int id { get; set; }
            public string max { get; set; }
            public string min { get; set; }
            public int secsType { get; set; }
            public string sml { get; set; }
            public int type { get; set; }
            public string unit { get; set; }

            public Variable()
            {
                defaultValue = string.Empty;
                definition = 0;
                description = string.Empty;
                id = 0;
                max = string.Empty;
                min = string.Empty;
                secsType = 0;
                sml = string.Empty;
                type = 0;
                unit = string.Empty;
            }

            public Variable Clone()
            {
                return new Variable
                {
                    defaultValue = this.defaultValue,
                    definition = this.definition,
                    description = this.description,
                    id = this.id,
                    max = this.max,
                    min = this.min,
                    secsType = this.secsType,
                    sml = this.sml,
                    type = this.type,
                    unit = this.unit
                };
            }
    
        }

        public class Hsms
        {
            public bool autoSessionID { get; set; }
            public bool autoSystemBytes { get; set; }
            public bool handleCtrlMessage { get; set; }
            public int heartbeat { get; set; }
            public string mdln { get; set; }
            public bool sendSelectReq { get; set; }
            public int sessionID { get; set; }
            public Socket socket { get; set; }
            public string softrev { get; set; }
            public int t3 { get; set; }
            public int t5 { get; set; }
            public int t6 { get; set; }
            public int t7 { get; set; }
            public int t8 { get; set; }

            public Hsms()
            {
                autoSessionID = false;
                autoSystemBytes = false;
                handleCtrlMessage = false;
                heartbeat = 0;
                mdln = string.Empty;
                sessionID = 0;
                socket = new Socket();
                softrev = string.Empty;
                t3 = 0;
                t5 = 0;
                t6 = 0;
                t7 = 0;
                t8 = 0;
            }
        }

        public class Socket
        {
            public string ip { get; set; }
            public bool ipv4 { get; set; }
            public string port { get; set; }
            public bool server { get; set; }

            public Socket()
            {
                ip = string.Empty;
                ipv4 = false;
                port = string.Empty;
                server = false;
            }
        }

        /// <summary>
        /// CEIDからイベント構成を取得する
        /// </summary>
        /// <param name="argEvents">イベント構成設定</param>
        /// <param name="argCeid">取得したいCEID</param>
        /// <returns>イベントを返す</returns>
        public Event GetEventFromCeid( Event[] argEvents, int argCeid )
        {
            Event eventClass = new Event();
            int eventCount = 0;
            while(argEvents.Length > eventCount )
            {
                if( argEvents[eventCount].id == argCeid )
                {
                    eventClass = argEvents[eventCount];
                    break;
                }
                eventCount++;
            }

            return eventClass;
        }

        /// <summary>
        /// レポートに登録されているVIDをリストにして返す。
        /// </summary>
        /// <param name="argVariables">VID構成設定</param>
        /// <param name="argReportNo">取得したいレポート番号</param>
        /// <returns>VID構成のリスト</returns>
        public List<Variable>? GetVariableListFromReportNo( int argReportNo )
        {
            List<Variable> valiablesList = new List<Variable>();

            var reportDic = eventModel.reports.ToDictionary( r => r.id );
            var variableDic = eventModel.variables.ToDictionary( r => r.id );

            if( !reportDic.ContainsKey( argReportNo ) )
            {
                // Reportが見つからなかったら終了。
                return null;
            }

            int[] valiableNo = reportDic[argReportNo].variables;
            for(int VidCount = 0; VidCount < valiableNo.Length; VidCount++ )
            {
                if( !variableDic.ContainsKey(valiableNo[VidCount]) )
                {
                    // 一つでも見つからなかったら終了
                    return null;
                }
                valiablesList.Add( variableDic[valiableNo[VidCount]]);
            }

            return valiablesList;
        }

    }
}
