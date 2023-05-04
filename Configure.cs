using System;
using System.Data.Common;
using System.Reflection;
using System.Threading.Tasks;

namespace ORM_1_21_
{
    /// <summary>
    ///     Class Base Configure
    /// </summary>
    public sealed partial class Configure
    {
        internal static DbProviderFactory CurFactory;
        //internal static bool UsageCache;

        internal static string ConnectionString;


        private static Configure _configure;

        private static readonly object Locker = new object();

        /// <summary>
        ///     Constructor (start App)
        /// </summary>
        /// <param name="connectionString">Database connection string</param>
        /// <param name="provider">Base connection provider</param>
        public Configure(string connectionString, ProviderName provider) : this(connectionString, provider, null)
        {
        }


        void CreateFactoryPostgres()
        {
            try
            {
                var a = AppDomain.CurrentDomain.Load("Npgsql");
                var b = a.GetType("Npgsql.NpgsqlFactory");
                var field = b.GetField("Instance", BindingFlags.Static | BindingFlags.Public);
                CurFactory = (DbProviderFactory)field.GetValue(null);
                AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
                AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
            }
            catch (Exception e)
            {
                MySqlLogger.Info($"{e.Message}{Environment.NewLine}{e}");
                throw;
            }
        }

        private void CreateFactoryMysql()
        {
            try
            {
                var a = AppDomain.CurrentDomain.Load("Mysql.Data");
                var b = a.GetType("MySql.Data.MySqlClient.MySqlClientFactory");
                CurFactory = (DbProviderFactory)b.GetField("Instance").GetValue(null);
            }
            catch (Exception e)
            {
                MySqlLogger.Info($"{e.Message}{Environment.NewLine}{e}");
                throw;
            }
        }

        private void CreateFactorySqlite()
        {
            try
            {
                var a = AppDomain.CurrentDomain.Load("System.Data.SQLite");
                var b = a.GetType("System.Data.SQLite.SQLiteFactory");
                var field = b.GetField("Instance", BindingFlags.Static | BindingFlags.Public);
                CurFactory = (DbProviderFactory)field.GetValue(null);
            }
            catch (Exception e)
            {
                MySqlLogger.Info($"{e.Message}{Environment.NewLine}{e}");
                throw;
            }
        }

        private void CreateFactoryMsSql()
        {
            try
            {
                var a = AppDomain.CurrentDomain.Load("System.Data.SqlClient");
                var b = a.GetType("System.Data.SqlClient.SqlClientFactory");
                CurFactory = (DbProviderFactory)b.GetField("Instance").GetValue(null);
            }
            catch (Exception e)
            {
                MySqlLogger.Info($"{e.Message}{Environment.NewLine}{e}");
                throw;
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="connectionString">Database connection string</param>
        /// <param name="provider">Base connection provider</param>
        /// <param name="logFileName">Path to log file is  null - disable writing to log file.</param>
        /// <param name="isSearchGac"> true:Search data provider in storage GAC, default - false</param>
        public Configure(string connectionString, ProviderName provider, string logFileName, bool isSearchGac = false)
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

#if NET461
                        try
                        {
                            if (!isSearchGac1) throw new Exception("disable usage GAC");
                            CurFactory = DbProviderFactories.GetFactory("Npgsql");
                        }
                        catch
                        {
                            CreateFactoryPostgres();
                        }
#elif NETSTANDARD2_0
                        CreateFactoryPostgres();
#endif


                        break;
                    }
                case ProviderName.MySql:
                    {

#if NET461
                        try
                        {
                            if (!isSearchGac1) throw new Exception("Search ban GAC");
                            CurFactory = DbProviderFactories.GetFactory("MySql.Data.MySqlClient");
                        }
                        catch
                        {
                            CreateFactoryMysql();
                        }
#elif NETSTANDARD2_0
                          CreateFactoryMysql();
#endif


                        break;
                    }
                case ProviderName.SqLite:
                    {
#if NET461
                        try
                        {
                            if (!isSearchGac1) throw new Exception("Search ban  GAC");
                            CurFactory = DbProviderFactories.GetFactory("System.Data.SQLite.SQLiteFactory");
                        }
                        catch
                        {
                            CreateFactorySqlite();
                        }
#elif NETSTANDARD2_0
                         CreateFactorySqlite();
#endif


                        break;
                    }
                case ProviderName.MsSql:
                    {
#if NET461
                        try
                        {
                            if (!isSearchGac1) throw new Exception("Search ban GAC");
                            CurFactory = DbProviderFactories.GetFactory("System.Data.SqlClient");
                        }
                        catch
                        {
                            CreateFactoryMsSql();
                        }
#elif NETSTANDARD2_0
                        CreateFactoryMsSql();
#endif


                        break;
                    }
            }
        }

        /// <summary>
        ///     Path to log file
        /// </summary>
        public static string LogFileName { get; private set; }

        /// <summary>
        ///     The provider that is currently using the orm
        /// </summary>
        public static ProviderName Provider { get; private set; }


        /// <summary>
        ///     Getting default session.
        ///     If you want to address to other database, use GetSession&lt;&gt;()
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static ISession Session
        {
            get
            {
                if (_configure == null)
                {
                    MySqlLogger.Info(
                        $" {Environment.NewLine}\"ISession GetInnerSession error _configure==null\"{Environment.NewLine}");
                    throw new Exception("ISession GetInnerSession error _configure==null");
                }

                return _configure.GetInnerSession();
            }
        }

        /// <summary>
        /// </summary>
        /// <exception cref="Exception"></exception>
        public static Task<ISession> SessionAsync
        {
            get
            {
                if (_configure == null)
                {
                    MySqlLogger.Info(
                        $" {Environment.NewLine}\"ISession GetInnerSession error _configure==null\"{Environment.NewLine}");
                    throw new Exception("ISession GetInnerSession error _configure==null");
                }

                return _configure.GetInnerSessionAsync();
            }
        }

        /// <summary>
        ///     DataBase Connection string
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
        ///     Getting a session to work with another database
        /// </summary>
        /// <typeparam name="TF">
        ///     The type that the interface must implement IOtherDataBaseFactory and
        ///     have a default constructor
        /// </typeparam>
        /// <returns></returns>
        public static ISession GetSession<TF>() where TF : IOtherDataBaseFactory, new()
        {
            if (_configure == null)
            {
                MySqlLogger.Info($" {Environment.NewLine}ISession GetInnerSession error _configure==null");
                throw new Exception("ISession GetInnerSession error _configure==null");
            }

            //lock (_o)
            //{
            return _configure.GetInnerSession<TF>();

            //}
        }

        /// <summary>
        ///     Getting a session to work with another database
        /// </summary>
        /// <typeparam name="TF">
        ///     The type that the interface must implement IOtherDataBaseFactory and
        ///     have a default constructor
        /// </typeparam>
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
            if (string.IsNullOrWhiteSpace(fileNameLogFile)) return;
            MySqlLogger.RunLogger(fileNameLogFile);
        }


        private ISession GetInnerSession()
        {
            lock (Locker)
            {
                return new Session(ConnectionString);
            }
        }

        private Task<ISession> GetInnerSessionAsync()
        {
            var tk = new TaskCompletionSource<ISession>(TaskCreationOptions.RunContinuationsAsynchronously);
            lock (Locker)
            {
                tk.SetResult(new Session(ConnectionString));
                return tk.Task;
            }
        }

        private ISession GetInnerSession<TF>()
        {
            var res = GetDataBaseFactory<TF>();
            return new Session(res);
        }

        private Task<ISession> GetInnerSessionAsync<TF>()
        {
            var tk = new TaskCompletionSource<ISession>(TaskCreationOptions.RunContinuationsAsynchronously);
            var res = GetDataBaseFactory<TF>();
            tk.SetResult(new Session(res));
            return tk.Task;
        }

        private IOtherDataBaseFactory GetDataBaseFactory<TF>()
        {
            var o = StorageOtherBaseFactory<TF>.GetDataBaseFactory();
            return o;
        }
    }
}