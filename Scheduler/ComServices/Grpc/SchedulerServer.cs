using Grpc.Core;
using Scheduler;
using System;
using System.Collections.Generic;
using System.Text;

namespace Scheduler_2_Dispatcher_Bridge.ComServices.Grpc
{
    public class SchedulerServer
    {
        static SchedulerServer Scheduleserver;

        static SchedulerServer()
        {
            Scheduleserver = new SchedulerServer();
        }

        SchedulerServer()
        {

        }

        Server Files_Svc;
        public static void Start()
        {
            appExtX.Log.Info("SchedulerServer::start()");

            try
            {
                var ip = dncCore.Params.HardGet(Define.SchedulerGrpcIpKey);

                var port = (int)dncCore.Params.GetAsUint(   Define.SchedulerGrpcPortKey
                                                          , Define.SchedulerGrpcPortValue
                                                        );
                
                Scheduleserver.Files_Svc = new Server()
                {
                    Services = { SchedulerGrpc.Files.BindService(new FilesImpl()) },
                    Ports = { new ServerPort(ip, port,  ServerCredentials.Insecure)
                          }
                };

                Scheduleserver.Files_Svc.Services.Add(SchedulerGrpc.SvcSlicerProfile.BindService(new SvcSlicerProfileImpl()));

                Scheduleserver.Files_Svc.Start();
            }
            catch(Exception ex)
            {
                appExtX.Log.Exception($"Start grpc Server {ex.Message}");
            }
        }
    }
}
