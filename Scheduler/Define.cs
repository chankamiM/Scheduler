using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Scheduler
{
    public static class Define
    {
        public static string SchedulerGrpcIpKey = "SchedulerGrpcIp";
        public static readonly string SchedulerGrpcIpValue = "127.0.0.1";

        public static string SchedulerGrpcPortKey = "SchedulerGrpcPort";
        public static readonly uint SchedulerGrpcPortValue = 23;

        public static string DispatcherGrpcIpKey = "DispatcherGrpcIp";
        public static readonly string DispatcherGrpcIpValue = "127.0.0.1";

        public static string DispatcherGrpcPortKey = "DispatcherGrpcPort";
        public static readonly uint DispatcherGrpcPortValue = 24;

        public static readonly string defaultConfigFileName = ".//Scheduler.config";

        public static readonly string Scheduler2dll = ".//Scheduler2.dll";

        public static readonly string Scheduler = "Scheduler2";

        public static readonly string SCHEDULER_EP_IP_KEY = "SCHEDULER_EP_IP";
        public static readonly string SCHEDULER_EP_PORT_KEY = "SCHEDULER_EP_PORT";

        public static string DispatcherGrpcHost = "192.168.1.1:5001";
        public static readonly string DispatcherGrpcHost_KEY = "Dispatcher_GrpcHost";

        /*
        public static string []KEYS = new string[]
        {
            "DB_serverAddr",
            "DB_Name",
            "DB_User",
            "DB_Pw",
            "DB_Trust_Connection",
            "DB_TrustServerCertificate"
        };
        */

        public enum KeysEnum
        {
           DB_Conn
        }

        public static Dictionary<string, string> dict = new Dictionary<string, string>()
        { 
            {KeysEnum.DB_Conn.ToString(),"Data Source=192.168.2.11,1433;Initial Catalog=L3;User ID=structo;Password=structo123;Integrated Security=SSPI;Trusted_Connection=true;TrustServerCertificate=false;"}
        };

        public static void SetDefault()
        {
            dncCore.Params.Save("-config_file", defaultConfigFileName);
            dncCore.Params.Save(SchedulerGrpcIpKey, SchedulerGrpcIpValue);
            dncCore.Params.Save(SchedulerGrpcPortKey, SchedulerGrpcPortValue.ToString());

            dncCore.Params.Save(DispatcherGrpcIpKey, DispatcherGrpcIpValue);
            dncCore.Params.Save(DispatcherGrpcPortKey, DispatcherGrpcPortValue.ToString());

            dncCore.Params.Save(SCHEDULER_EP_IP_KEY, "127.0.0.1");
            dncCore.Params.Save(SCHEDULER_EP_PORT_KEY, "8601");

            dncCore.Params.Save(DispatcherGrpcHost_KEY, DispatcherGrpcHost);

           foreach(var r in dict)
           {
                dncCore.Params.Save(r.Key, r.Value);
           }
        }
    }

    public class FixedFileInfo
    {
        public string uid;
        public string filepathname;
        public int DBIndex;
    }

}
