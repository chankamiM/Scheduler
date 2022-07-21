using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using SchedulerGrpc;
using Scheduler.Db_Models;
using Scheduler;

namespace Scheduler_2_Dispatcher_Bridge.ComServices.Grpc
{
    public class SvcSlicerProfileImpl : SchedulerGrpc.SvcSlicerProfile.SvcSlicerProfileBase
    {
        public override Task<ManySlicerProfiles> GetAll(Empty request, ServerCallContext context)
        {
            ManySlicerProfiles resp = null;
            try
            {
                resp = MssqlConnector.SlicerProfile_GetAll();

                return Task.FromResult(resp);
            }
            catch(Exception ex)
            {
                appExtX.Log.Exception($"SvcSlicerProfileImpl::GetAll() {ex.Message}");
            }

            return Task.FromResult(resp);
        }

        public override Task<simpleRely> New(New_SlicerProfile request, ServerCallContext context)
        {
            var resp = new simpleRely();

            MssqlConnector.SlicerProfile_New(request.CreatedBy, request.Data);

            return Task.FromResult(resp);
        }

        public override Task<ManySlicerProfiles> GetByName(simpleStringRely request, ServerCallContext context)
        {
            var resp = new ManySlicerProfiles();

            return Task.FromResult(resp);
        }

        public override Task<ManySlicerProfiles> GetByDateTimeRange(DateTimeRange request, ServerCallContext context)
        {
            var resp = new ManySlicerProfiles();

            return Task.FromResult(resp);
        }


    }
}
