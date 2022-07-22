using System;
using System.Collections.Generic;
using System.Text;
using dncCore;
using appExtX;
using System.Reflection;
using System.IO;
using System.Linq;
using dBExt_x.MSSQL;

namespace Scheduler
{
    internal static class Helper
    {
        static bool bApp_PostInit_Locked;


        public static List<appExtX.interfaces.SimpleStartStop> startStop = new List<appExtX.interfaces.SimpleStartStop>();

        public static void App_PreInit()
        {
            Define.SetDefault();

            appExtX.ModuleHelper.preInit();

            appExtX.Log.Info($"where am i??? {Directory.GetCurrentDirectory()}");

        }
        public static void App_Init()
        {
            
        }

        public static void App_PostInit()
        {
            if (bApp_PostInit_Locked) return;
                  bApp_PostInit_Locked = true;

            appExtX.ModuleHelper.postInit();

            
            //Scheduler.MssqlConnector.Connect(connectparam);
            Scheduler.MssqlConnector.Connect(dncCore.Params.HardGet(Define.KeysEnum.DB_Conn.ToString()));

            Scheduler_2_Dispatcher_Bridge.ComServices.Grpc.SchedulerServer.Start();

        }

        public static void App_Closing()
        {


        }

        public static void App_Closed()
        {

        }




    }
}
