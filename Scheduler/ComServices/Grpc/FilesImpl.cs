using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using SchedulerGrpc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler
{
    public class FilesImpl : SchedulerGrpc.Files.FilesBase
    {
        public override Task<NewFileId> FreshFile(freshfile request, ServerCallContext context)
        {
            appExtX.Log.Info($"FilesImpl::FreshFile() {request.Filenamepath}");
            var resp = new NewFileId();
            try
            {
                var uid = Scheduler.MssqlConnector.FreshFile(request.Filenamepath);
               
                resp.Id = uid;
                
            }
            catch(Exception eex)
            {
                appExtX.Log.Exception($"Grpc::FreshFile() {eex.Message}");
                resp.Id = null;
            }
            finally
            {
                
            }
            return Task.FromResult(resp);
        }

        public override Task<NewFileId> FreshFile2(freshfile request, ServerCallContext context)
        {
            appExtX.Log.Info($"FilesImpl::FreshFile2() {request.Filenamepath}");
            var resp = new NewFileId();
            try
            {
                var uid = Scheduler.MssqlConnector.FreshFile2(request.Filenamepath);

                resp.Id = uid;

            }
            catch (Exception eex)
            {
                appExtX.Log.Exception($"Grpc::FreshFile() {eex.Message}");
                resp.Id = null;
            }
            finally
            {

            }
            return Task.FromResult(resp);
        }

        public override Task<simpleRely> SetFixFileStatus(FixFileStatus request, ServerCallContext context)
        {
            appExtX.Log.Info($"FilesImpl::SetFixFileStatus(), status:{request.Success}, Id:{request.Id}, filepathname:{request.Fixerpath}");
            
            simpleRely resp = new simpleRely();

            try
            {
                Scheduler.MssqlConnector.Set_File_Fix_Status(request.Success, request.Id, request.Fixerpath);

                resp.Status = 1;

            }
            catch (Exception eex)
            {
                resp.Status = 0;
            }
            finally
            {

            }


            return Task.FromResult(resp);
        }
    
        public override Task<ManyFiles> GetAll(Empty request, ServerCallContext context)
       {
            appExtX.Log.Info($"FilesImpl::GetAll()");


           ManyFiles resp = new ManyFiles();

            var res = Scheduler.MssqlConnector.GetAll();

            res.ForEach(
                x => {

                    File f = new File();
                    f.Rid = x.rid;
                    f.Uid =  x.Uid;
                    f.Name= x.Name;
                    f.RegisterDT = x.RegisterDT;
                    f.Progress = x.progress;
                    f.Fixstatus = x.fixstatus;
                    f.FixedPath = x.fixedPath;
                    f.FixedDT = x.fixedDT;
                    f.FixingDT = x.fixingDT;
                    f.SlicingDT = x.slicingDT;
                    f.SlicedDT = x.slicedDT;
                    f.SlicedStatus = x.slicedStatus;
                    f.SlicedPath = x.slicedPath;
                    f.PrintStatus = x.printStatus;
                    f.Description =  x.Descriptions;
                    resp.Files.Add(f);
                }
                );

            return Task.FromResult(resp);
       }

        public override Task<ManyFiles> GetFilesByStatus(stringReq request, ServerCallContext context)
        {
            appExtX.Log.Info($"FilesImpl::GetFilesByStatus() status:{request.Str}");
            ManyFiles resp = new ManyFiles();

            var res = Scheduler.MssqlConnector.GetFilesByStatus(request.Str);

            res.ForEach(
                x => {

                    File f = new File();
                    f.Rid = x.rid;
                    f.Uid = x.Uid;
                    f.Name = x.Name;
                    f.RegisterDT = x.RegisterDT;
                    f.Progress = x.progress;
                    f.Fixstatus = x.fixstatus;
                    f.FixedPath = x.fixedPath;
                    f.FixedDT = x.fixedDT;
                    f.FixingDT = x.fixingDT;
                    f.SlicingDT = x.slicingDT;
                    f.SlicedDT = x.slicedDT;
                    f.SlicedStatus = x.slicedStatus;
                    f.SlicedPath = x.slicedPath;
                    f.PrintStatus = x.printStatus;
                    f.Description = x.Descriptions;
                    resp.Files.Add(f);
                }
                );

            return Task.FromResult(resp);
        }

        public override Task<simpleRely> StartFixing(stringReq request, ServerCallContext context)
        {
            simpleRely resp = new simpleRely();
            appExtX.Log.Info($"FilesImpl::StartFixing() Id:{request.Str}");
            try
            {
                resp.Status = Scheduler.MssqlConnector.StartFixing(request.Str);

                //resp.Status = 1;

            }
            catch (Exception eex)
            {
                resp.Status = -1;
                appExtX.Log.Exception($"GRPC::StartFixing {eex.Message}");
            }
            finally
            {

            }


            return Task.FromResult(resp);
        }

        public override Task<ManyFiles> GetFilesInFixingStatus(Empty request, ServerCallContext context)
        {
            ManyFiles resp = new ManyFiles();

            appExtX.Log.Info($"FilesImpl::GetFilesInFixingStatus()");

            try
            {

                var res = Scheduler.MssqlConnector.GetFilesInFixingStatus();

                res.ForEach(
                    x =>
                    {

                        File f = new File();
                        f.Rid = x.rid;
                        f.Uid = x.Uid;
                        f.Name = x.Name;
                        f.RegisterDT = x.RegisterDT;
                        f.Progress = x.progress;
                        f.Fixstatus = x.fixstatus;
                        f.FixedPath = x.fixedPath;
                        f.FixedDT = x.fixedDT;
                        f.FixingDT = x.fixingDT;
                        f.SlicingDT = x.slicingDT;
                        f.SlicedDT = x.slicedDT;
                        f.SlicedStatus = x.slicedStatus;
                        f.SlicedPath = x.slicedPath;
                        f.PrintStatus = x.printStatus;
                        f.Description = x.Descriptions;
                        resp.Files.Add(f);
                    }
                    );
            }
            catch(Exception ex)
            {
                appExtX.Log.Exception($"GRPC::GetFilesInFixingStatus {ex.Message}");
            }
            return Task.FromResult(resp);
        }
    }
}
