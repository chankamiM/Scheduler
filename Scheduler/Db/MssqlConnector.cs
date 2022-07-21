using appExtX;
using dBExt_x;
using dBExt_x.MSSQL;

using SchedulerGrpc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Data.SqlClient;
using Scheduler.Db_Models;

namespace Scheduler
{
    public class MssqlConnector : Connector 
                                , IDisposable
    {
        private bool disposedValue;

        public MssqlConnector(ConnectParam connectparam)
            : base(connectparam)
        {

        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if(Connected == DB_STATUS.Connected)
                    {
                        Close();
                        Thread.Sleep(1000);
                    }
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~MssqlConnector()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }


        static dBExt_x.MSSQL.Connector mssql;
        static ConnectParam _connectparam;
        public static void Connect(ConnectParam connectparam)
        {
            appExtX.Log.Info($"MssqlConnector::Connect()");
            _connectparam = connectparam;

            mssql = new Connector(connectparam);

            try
            {
                mssql.Connect();
            }
            catch (Exception ex)
            {
                appExtX.Log.Exception($"connecting to sql Server; ${ex.Message}");
            }
        }
        
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static string FreshFile( string freshfielpath_N_name)
        {
            appExtX.Log.Info($"MssqlConnector::FreshFile({freshfielpath_N_name})");

            using (SqlCommand cmd = new SqlCommand("dbo.FreshFile", mssql.Conn))
            {
                //  cmd.CommandText = parameterStatement.getQuery();
                cmd.CommandType = CommandType.StoredProcedure;

                var _1 = new SqlParameter("@_filename", SqlDbType.VarChar);
                _1.Direction = ParameterDirection.Input;
                _1.Value = freshfielpath_N_name;
                cmd.Parameters.Add(_1);

                var uid = Guid.NewGuid().ToString();
                var _2 = new SqlParameter("@_Uid", SqlDbType.VarChar);
                _2.Direction = ParameterDirection.Input;
                _2.Value = uid;
                cmd.Parameters.Add(_2);

                cmd.ExecuteNonQuery();

                return uid;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        
        public static string FreshFile2(string filepathname)
        {
            string query = $"INSERT INTO dbo.FreshFile (_Name, _XId, _Status, _Register) VALUES (@Name, @XId, @Status, @Register)";

            appExtX.Log.Info($"MssqlConnector::FreshFile2() query:{query}");

            using (SqlCommand cmd = new SqlCommand(query, mssql.Conn))
            {
                var uid = Guid.NewGuid().ToString();

                cmd.Parameters.AddWithValue("@Name", filepathname);
                cmd.Parameters.AddWithValue("@XId", uid);
                cmd.Parameters.AddWithValue("@Status", "fresh");

                cmd.Parameters.AddWithValue("@Register", dncCore.Utilties.DateTime.Helper.YYYYMMDDHHMMSS());

                cmd.ExecuteNonQuery();

                return uid;
            }
        }

        public static List<FixedFileInfo> Get_Files_For_Fixing(SqlConnection conn)
        {
            List<FixedFileInfo> l = new List<FixedFileInfo>();
            try
            {
                using (SqlCommand cmd = new SqlCommand("dbo.Get_Files_For_Fixing", conn))
                {
                    //  cmd.CommandText = parameterStatement.getQuery();
                    cmd.CommandType = CommandType.StoredProcedure;

                    //conn.Open();
                    var reader = cmd.ExecuteReader();
                    //var result = returnParameter.Value;
                    try
                    {
                        while (reader.Read())
                        {
                            var item = new FixedFileInfo();
                            item.uid = reader[0].ToString();
                            item.filepathname = reader["_location"].ToString();
                            item.DBIndex = Convert.ToInt32(reader[1].ToString());

                            //Console.WriteLine($"Get File for FIxing  {item.uid}  {item.index} {item.filepathname}");

                            l.Add(item);
                        }
                    }
                    catch (Exception ex)
                    {
                        appExtX.Log.Exception($"Get_Files_For_Fixing() {ex.Message}");
                    }
                    finally
                    {
                        reader.Close();
                    }
                }
            }
            catch(Exception ex)
            {
                appExtX.Log.Exception($"Get_Files_For_Fixing() {ex.Message}");
            }

            return l;
        }

        public static List<FixedFileInfo> Get_Files_For_Fixing2(SqlConnection conn)
        {
            List<FixedFileInfo> l = new List<FixedFileInfo>();

            string query = $"Select * from dbo.Get_Files_For_Fixing where _Status = 'fresh'";

            try
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    //  cmd.CommandText = parameterStatement.getQuery();
                    cmd.CommandType = CommandType.StoredProcedure;

                    //conn.Open();
                    var reader = cmd.ExecuteReader();
                    //var result = returnParameter.Value;
                    try
                    {
                        while (reader.Read())
                        {
                            var item = new FixedFileInfo();
                            item.uid = reader[0].ToString();
                            item.filepathname = reader["_Name"].ToString();
                            item.DBIndex = Convert.ToInt32(reader[1].ToString());

                            //Console.WriteLine($"Get File for FIxing  {item.uid}  {item.index} {item.filepathname}");

                            l.Add(item);
                        }
                    }
                    catch (Exception ex)
                    {
                        appExtX.Log.Exception($"Get_Files_For_Fixing() {ex.Message}");
                    }
                    finally
                    {
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                appExtX.Log.Exception($"Get_Files_For_Fixing() {ex.Message}");
            }

            return l;
        }

        public static void Set_File_Fix_Status(bool status, string Xid, string fixerpath)
        {

            

                using (SqlCommand cmd = new SqlCommand("dbo.Set_File_Fix_Status", mssql.Conn))
                {
                    //  cmd.CommandText = parameterStatement.getQuery();
                    cmd.CommandType = CommandType.StoredProcedure;

                    var _1 = new SqlParameter("@_status", SqlDbType.Bit);
                    _1.Direction = ParameterDirection.Input;
                    _1.Value = status;
                    cmd.Parameters.Add(_1);

                    var _2 = new SqlParameter("@_Xid", SqlDbType.VarChar);
                    _2.Direction = ParameterDirection.Input;
                    _2.Size = 255;
                    _2.Value = Xid;
                    cmd.Parameters.Add(_2);

                    var _3 = new SqlParameter("@_filepathName", SqlDbType.VarChar);
                    _3.Direction = ParameterDirection.Input;
                    _3.Size = 255;
                    _3.Value = fixerpath;//r.filepathname;
                    cmd.Parameters.Add(_3);

                    cmd.ExecuteNonQuery();

                    //Console.WriteLine($"FIxing Done  {r.uid}  {r.index} C:\\FixedDone\\123.stl");

                }
        }

        public static void Set_File_Fix_Status2(bool status, string Xid, string fixerpath)
        {
            var s = status ? "fixing-succeed" : "fixing-failed";
            string query = $@"UPDATE Files SET _Fixed_PASS={status} , _Fixed_locationName={fixerpath}, _Status = {s},  _Fixed_DT = {dncCore.Utilties.DateTime.Helper.YYYYMMDDHHMMSS()}";
            using (SqlCommand cmd = new SqlCommand("dbo.Set_File_Fix_Status", mssql.Conn))
            {
                //  cmd.CommandText = parameterStatement.getQuery();
                cmd.CommandType = CommandType.StoredProcedure;

                var _1 = new SqlParameter("@_status", SqlDbType.Bit);
                _1.Direction = ParameterDirection.Input;
                _1.Value = status;
                cmd.Parameters.Add(_1);

                var _2 = new SqlParameter("@_Xid", SqlDbType.VarChar);
                _2.Direction = ParameterDirection.Input;
                _2.Size = 255;
                _2.Value = Xid;
                cmd.Parameters.Add(_2);

                var _3 = new SqlParameter("@_filepathName", SqlDbType.VarChar);
                _3.Direction = ParameterDirection.Input;
                _3.Size = 255;
                _3.Value = fixerpath;//r.filepathname;
                cmd.Parameters.Add(_3);

                cmd.ExecuteNonQuery();

                //Console.WriteLine($"FIxing Done  {r.uid}  {r.index} C:\\FixedDone\\123.stl");

            }
        }

        public static SlicedFileInfo Get_A_File_For_Slicing()
        {
            SlicedFileInfo item = null;
            using (SqlCommand cmd = new SqlCommand("dbo.Get_A_File_For_Slicing", mssql.Conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    item = new SlicedFileInfo();
                    item.uid = reader[0].ToString();
                    item.filepathname = reader["_location"].ToString();
                    item.DBIndex = Convert.ToInt32(reader[1].ToString());

                    //Console.WriteLine($"Get File for Slicing {item.uid}  {item.index} {item.filepathname}");

                    //l2.Add(item);
                }

                reader.Close();
            }

            return item;
        }

        public static void Set_Slicing_Status(bool status, string Uid, string spjfilepath)
        {

            using (SqlCommand cmd = new SqlCommand("dbo.Set_Slicing_Status", mssql.Conn))
            {
                //  cmd.CommandText = parameterStatement.getQuery();
                cmd.CommandType = CommandType.StoredProcedure;

                var _1 = new SqlParameter("@_status", SqlDbType.Bit);
                _1.Direction = ParameterDirection.Input;
                _1.Value = status;
                cmd.Parameters.Add(_1);

                var _2 = new SqlParameter("@_Xid", SqlDbType.VarChar);
                _2.Direction = ParameterDirection.Input;
                _2.Size = 255;
                _2.Value = Uid;
                cmd.Parameters.Add(_2);

                var _3 = new SqlParameter("@_filepathName", SqlDbType.VarChar);
                _3.Direction = ParameterDirection.Input;
                _3.Size = 255;
                _3.Value = spjfilepath;
                cmd.Parameters.Add(_3);

                //Console.WriteLine($"Get File for Slicing {r.uid} C:\\....somewhere.spj");

                //cmd.ExecuteNonQuery();

            }

        }
    
        public static List<Scheduler.Db_Models.Files> GetFilesByStatus(string status)
        {
            var l = new List<Scheduler.Db_Models.Files>();
            
            using (SqlCommand cmd = new SqlCommand("dbo.GetFilesByStatus", mssql.Conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;


                var _1 = new SqlParameter("@_status", SqlDbType.VarChar);
                _1.Direction = ParameterDirection.Input;
                _1.Value = status;
                cmd.Parameters.Add(_1);

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var item  = new Scheduler.Db_Models.Files();
                    item.rid  = Cast.To<int>(reader["rid"], -1);        Console.WriteLine($"rid {reader["rid"]}");
                    item.Name = reader["Name"].ToString();              Console.WriteLine($"Name {reader["Name"]}");
                    item.RegisterDT = reader["RegisterDT"].ToString();  Console.WriteLine($"RegisterDT {reader["RegisterDT"]}");
                    item.progress   = reader["progress"].ToString();    Console.WriteLine($"status {reader["progress"]}");

                    item.fixstatus = Cast.To<int>(reader["fixstatus"], -1);
                    
                    var s = Cast.To<string>(reader["fixstatus"], "-");
                    Console.WriteLine($"fixstatus {s}");

                    item.fixedPath = Cast.To<string>(reader["fixedPath"], "");
                    s = Cast.To<string>(reader["fixedPath"], "-");
                    Console.WriteLine($"fixedPath {s}");

                    item.fixedDT = Cast.To<string>(reader["fixedDT"], "");
                    s = Cast.To<string>(reader["fixedDT"], "-");
                    Console.WriteLine($"fixedDT {s}");

                    item.fixingDT = Cast.To<string>(reader["fixingDT"], "");
                    s = Cast.To<string>(reader["fixingDT"], "-");
                    Console.WriteLine($"fixingDT {s}");

                    item.slicingDT = Cast.To<string>(reader["slicingDT"], "");
                    s = Cast.To<string>(reader["slicingDT"], "-");
                    Console.WriteLine($"slicingDT {reader["slicingDT"]}");

                    item.slicedDT = Cast.To<string>(reader["slicedDT"], "");
                    s = Cast.To<string>(reader["slicedDT"], "-");
                    Console.WriteLine($"slicedDT {s}");

                    item.slicedStatus = Cast.To<int>(reader["slicedStatus"], -1);
                    s = Cast.To<string>(reader["slicedStatus"], "-");
                    Console.WriteLine($"slicedStatus {s}");

                    item.slicedPath = Cast.To<string>(reader["slicedPath"], "");
                    s = Cast.To<string>(reader["slicedPath"], "-");
                    Console.WriteLine($"scannedPath {reader["slicedPath"]}");

                    item.printStatus = Cast.To<int>(reader["printStatus"], -1);
                    Console.WriteLine($"printStatus {reader["printStatus"]}");


                    l.Add(item);
                }

                reader.Close();
            }

            return l;
        }

        public static List<Scheduler.Db_Models.Files> GetAll()
        {
            var l = new List<Scheduler.Db_Models.Files>();

            using (SqlCommand cmd = new SqlCommand("dbo.GetAll", mssql.Conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var item = new Scheduler.Db_Models.Files();
                    item.rid = Cast.To<int>(reader["rid"], -1);
                    item.Name = reader["Name"].ToString(); 
                    item.RegisterDT = reader["RegisterDT"].ToString(); 
                    item.progress = reader["progress"].ToString(); 

                    item.fixstatus = Cast.To<int>(reader["fixstatus"], -1);

                    var s = Cast.To<string>(reader["fixstatus"], "-");

                    item.fixedPath = Cast.To<string>(reader["fixedPath"], "");
                    s = Cast.To<string>(reader["fixedPath"], "-");

                    item.fixedDT = Cast.To<string>(reader["fixedDT"], "");
                    s = Cast.To<string>(reader["fixedDT"], "-");

                    item.fixingDT = Cast.To<string>(reader["fixingDT"], "");
                    s = Cast.To<string>(reader["fixingDT"], "-");

                    item.slicingDT = Cast.To<string>(reader["slicingDT"], "");
                    s = Cast.To<string>(reader["slicingDT"], "-");

                    item.slicedDT = Cast.To<string>(reader["slicedDT"], "");
                    s = Cast.To<string>(reader["slicedDT"], "-");

                    item.slicedStatus = Cast.To<int>(reader["slicedStatus"], -1);
                    s = Cast.To<string>(reader["slicedStatus"], "-");

                    item.slicedPath = Cast.To<string>(reader["slicedPath"], "");
                    s = Cast.To<string>(reader["slicedPath"], "-");

                    item.printStatus = Cast.To<int>(reader["printStatus"], -1);

                    item.Descriptions = Cast.To<string>(reader["Descriptions"], "");
                    l.Add(item);

                }

                reader.Close();
            }

            return l;
        }

        public static int StartFixing(string uID)
        {
            using (SqlCommand cmd = new SqlCommand("dbo.Start_Fixing", mssql.Conn))
            {
                //  cmd.CommandText = parameterStatement.getQuery();
                cmd.CommandType = CommandType.StoredProcedure;

                var _2 = new SqlParameter("@_Xid", SqlDbType.VarChar);
                _2.Direction = ParameterDirection.Input;
                _2.Size = 255;
                _2.Value = uID;
                cmd.Parameters.Add(_2);

                return cmd.ExecuteNonQuery();

            }

        }

        public static int StartFixing2(string uID)
        {
            string query = $@"UPDATE Files where ";
            using (SqlCommand cmd = new SqlCommand("dbo.Start_Fixing", mssql.Conn))
            {
                //  cmd.CommandText = parameterStatement.getQuery();
                cmd.CommandType = CommandType.StoredProcedure;

                var _2 = new SqlParameter("@_Xid", SqlDbType.VarChar);
                _2.Direction = ParameterDirection.Input;
                _2.Size = 255;
                _2.Value = uID;
                cmd.Parameters.Add(_2);

                return cmd.ExecuteNonQuery();

            }

        }

        public static List<Scheduler.Db_Models.Files> GetFilesInFixingStatus()
        {
            var  l = new List<Scheduler.Db_Models.Files>();
            using (SqlCommand cmd = new SqlCommand("dbo.Get_Files_In_Fixing_Status", mssql.Conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var item = new Scheduler.Db_Models.Files();
                    item.rid = Cast.To<int>(reader["rid"], -1); 
                    item.Uid = reader["uid"].ToString();
                    item.Name = reader["Name"].ToString(); 
                    item.RegisterDT = reader["RegisterDT"].ToString(); 
                    item.progress = reader["progress"].ToString(); 

                    item.fixstatus = Cast.To<int>(reader["fixstatus"], -1);

                    var s = Cast.To<string>(reader["fixstatus"], "-");

                    item.fixedPath = Cast.To<string>(reader["fixedPath"], "");
                    s = Cast.To<string>(reader["fixedPath"], "-");

                    item.fixedDT = Cast.To<string>(reader["fixedDT"], "");
                    s = Cast.To<string>(reader["fixedDT"], "-");

                    item.fixingDT = Cast.To<string>(reader["fixingDT"], "");
                    s = Cast.To<string>(reader["fixingDT"], "-");

                    item.slicingDT = Cast.To<string>(reader["slicingDT"], "");
                    s = Cast.To<string>(reader["slicingDT"], "-");

                    item.slicedDT = Cast.To<string>(reader["slicedDT"], "");
                    s = Cast.To<string>(reader["slicedDT"], "-");

                    item.slicedStatus = Cast.To<int>(reader["slicedStatus"], -1);
                    s = Cast.To<string>(reader["slicedStatus"], "-");

                    item.slicedPath = Cast.To<string>(reader["slicedPath"], "");
                    s = Cast.To<string>(reader["slicedPath"], "-");

                    item.printStatus = Cast.To<int>(reader["printStatus"], -1);

                    item.Descriptions = Cast.To<string>(reader["Descriptions"], "");
                    l.Add(item);
                }

                reader.Close();
            }

            return l;
        }

        public static ManySlicerProfiles SlicerProfile_GetAll()
        {
            ManySlicerProfiles many = new ManySlicerProfiles();
            using (SqlCommand cmd = new SqlCommand("dbo.SlicerProfile_GetAll", mssql.Conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var item = new SlicerProfile();
                    item.Rid = Cast.To<int>(reader["rid"], -1); 
                    item.Uid = reader["uid"].ToString();

                    InternalHelper.GetTime(  reader["registerDT"].ToString()
                                           , (DateTime dt) => { item.RegisterDT = Timestamp.FromDateTime(dt); }
                                           , () => { item.RegisterDT = null; }
                                          );

                    item.CreatedBy = reader["created_by"].ToString(); 

                    item.MotifiedBy = reader["modified_by"].ToString();

                    InternalHelper.GetTime(reader["modifiedDT"].ToString()
                                           , (DateTime dt) => { item.ModifiedDT = Timestamp.FromDateTime(dt); }
                                           , () => { item.ModifiedDT = null; }
                                          );

                    many.Profile.Add(item);
                }

                reader.Close();
            }
            return many;
        }

        public static void SlicerProfile_New(string created_by, string data)
        {

        }

        public static void SlicerProfile_GetByName()
        {

        }

        public static void SlicerProfile_GetByDateTimeRange()
        {

        }
    }
}
