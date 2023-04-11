using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ORM_1_21_
{
    /// <summary>
    ///    Class Base Configure
    /// </summary>
    public sealed partial class Configure
    {
        internal static DbProviderFactory CurFactory;
        //internal static bool UsageCache;

        internal static string ConnectionString;

        /// <summary>
        /// DataBase Connection string
        /// </summary>
        /// <returns></returns>
        public static string GetConnectionString()
        {
            lock (Locker)
            {
                return ConnectionString;
            }
        }

        /// <summary>
        /// Path to log file
        /// </summary>
        public static string LogFileName { get; private set; }


        private static Configure _configure;

        private static readonly object Locker = new object();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString">Database connection string</param>
        /// <param name="provider">Base connection providerй</param>
        /// <param name="logFileName">Path to log file is  null - disable writing to log file.</param>
        /// <param name="isSearchGac"> true:Search data provider in storage GAC, default - false</param>
        public Configure(string connectionString, ProviderName provider, string logFileName, bool isSearchGac=false)
        {
            _configure = this;
            CurFactory = null;
            ConnectionString = connectionString;
            var isSearchGac1 = isSearchGac;
            Provider = provider;
            LogFileName = logFileName;
            LogFileName = logFileName;
            ActivateLogger(logFileName);
            switch (provider)
            {
                case ProviderName.PostgreSql:
                    {
                        try
                        {
                            if (!isSearchGac1) throw new Exception("disable usage GAC");
                            CurFactory = DbProviderFactories.GetFactory("Npgsql");

                        }
                        catch 
                        {
                            try
                            {
                                var a = AppDomain.CurrentDomain.Load("Npgsql");
                                var b = a.GetType("Npgsql.NpgsqlFactory");
                                var field1 = b.GetField("Instance", BindingFlags.Static | BindingFlags.Public);
                                CurFactory = (DbProviderFactory)field1.GetValue(null);
                                AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
                                AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
                            }
                            catch (Exception e)
                            {
                                MySqlLogger.Info($"{e.Message}{Environment.NewLine}{e}");
                                throw;
                            }
                           
                        }
                       
                       
                       
                        break;
                    }
                case ProviderName.MySql:
                    {
                        try
                        {
                            if (!isSearchGac1) throw new Exception("Search ban GAC");
                            CurFactory = DbProviderFactories.GetFactory("MySql.Data.MySqlClient");
                        }
                        catch 
                        {

                            var a = AppDomain.CurrentDomain.Load("Mysql.Data");
                            var b = a.GetType("MySql.Data.MySqlClient.MySqlClientFactory");
                            CurFactory = (DbProviderFactory)b.GetField("Instance").GetValue(null);
                        }
                     
                        break;
                    }
                case ProviderName.SqLite:
                    {
                        try
                        {
                            if (!isSearchGac1) throw new Exception("Search ban  GAC");
                            CurFactory = DbProviderFactories.GetFactory("System.Data.SQLite.SQLiteFactory");
                        }
                        catch
                        {
                            var a = AppDomain.CurrentDomain.Load("System.Data.SQLite");
                            var b = a.GetType("System.Data.SQLite.SQLiteFactory");
                            var field1 = b.GetField("Instance", BindingFlags.Static | BindingFlags.Public);
                            CurFactory = (DbProviderFactory)field1.GetValue(null);
                         
                        }
                        break;
                   
                    }
                case ProviderName.MsSql:
                    {
                        try
                        {
                            if (!isSearchGac1) throw new Exception("Search ban GAC");
                            CurFactory = DbProviderFactories.GetFactory("System.Data.SqlClient");
                        }
                        catch 
                        {
                            var a = AppDomain.CurrentDomain.Load("System.Data.SqlClient");
                            var b = a.GetType("System.Data.SqlClient.SqlClientFactory");
                            CurFactory = (DbProviderFactory)b.GetField("Instance").GetValue(null);
                        }
                       
                        break;

                    }
            }

           
        }

        /// <summary>
        /// The provider that is currently using the orm
        /// </summary>
        public static ProviderName Provider { get; private set; }

       

        /// <summary>
        ///Getting default session
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static ISession Session
        {
            get
            {
                if (_configure == null)
                {
                    MySqlLogger.Info($" {Environment.NewLine}\"ISession GetInnerSession error _configure==null\"{Environment.NewLine}");
                    throw new Exception("ISession GetInnerSession error _configure==null");
               
                }

                return _configure.GetInnerSession();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="Exception"></exception>
        public static Task<ISession> SessionAsync
        {
            get
            {
                if (_configure == null)
                {
                    MySqlLogger.Info($" {Environment.NewLine}\"ISession GetInnerSession error _configure==null\"{Environment.NewLine}");
                    throw new Exception("ISession GetInnerSession error _configure==null");
                }
                return _configure.GetInnerSessionAsync();
            }
        }

        /// <summary>
        /// Getting a session to work with another database
        /// </summary>
        /// <typeparam name="TF">The type that the interface must implement IOtherDataBaseFactory and
        /// have a default constructor</typeparam>
        /// <returns></returns>
        public static ISession GetSession<TF>() where TF : IOtherDataBaseFactory ,new()
        {
            if (_configure == null)
            {
                MySqlLogger.Info($" {Environment.NewLine}ISession GetInnerSession error _configure==null");
                throw new Exception("ISession GetInnerSession error _configure==null");
            }

            return _configure.GetInnerSession<TF>();
        }

        /// <summary>
        /// Getting a session to work with another database
        /// </summary>
        /// <typeparam name="TF">The type that the interface must implement IOtherDataBaseFactory and
        /// have a default constructor</typeparam>
        public static async Task<ISession> GetSessionAsync<TF>() where TF : IOtherDataBaseFactory, new()
        {
            if (_configure == null)
            {
                MySqlLogger.Info($" {Environment.NewLine}ISession GetInnerSession error _configure==null");
                throw new Exception("ISession GetInnerSession error _configure==null");
            }

            return await _configure.GetInnerSessionAsync<TF>();
        }


        private static void ActivateLogger(string fileNameLogFile)
        {
            if (LogFileName == null) return;
            if (!File.Exists(fileNameLogFile))
                using (File.Create(fileNameLogFile))
                {
                }
            MySqlLogger.StopLogger();

            Task.Run(async ()=> await MySqlLogger.RunLogger(fileNameLogFile));
        }
       

        private ISession GetInnerSession()
        {
            lock (Locker)
            {
                return new Sessione(ConnectionString);
            }
        }

        private Task<ISession> GetInnerSessionAsync()
        {
            var tk = new TaskCompletionSource<ISession>(TaskCreationOptions.RunContinuationsAsynchronously);
            lock (Locker)
            {
                tk.SetResult(new Sessione(ConnectionString));
                return  tk.Task;
            }
        }
        private ISession GetInnerSession<TF>()
        {
            var res = GetDataBaseFactory<TF>(); 
            return new Sessione(res);
        }
        private Task<ISession> GetInnerSessionAsync<TF>()
        {
            var tk = new TaskCompletionSource<ISession>(TaskCreationOptions.RunContinuationsAsynchronously);
            var res = GetDataBaseFactory<TF>();
            tk.SetResult(new Sessione(res));
            return tk.Task;
        }

        private IOtherDataBaseFactory GetDataBaseFactory<TF>()
        {
            var o = StorageOtherBaseFactory<TF>.GetDataBaseFactory();
            return o;
        }


       
        

    }
}