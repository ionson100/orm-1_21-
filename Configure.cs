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
    ///     Базовый класс для конфигурации
    /// </summary>
    public sealed partial class Configure
    {
        internal static DbProviderFactory _curFactory;
        //internal static bool UsageCache;

        internal static string ConnectionString;
        private readonly bool _isSearchGac;

        /// <summary>
        /// Строка соединения к базе данных
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
        /// Название файла лога
        /// </summary>
        public static string LogFileName { get; private set; }


        private static Configure _configure;

        private static readonly object Locker = new object();

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="connectionString">Строка соединения с базой</param>
        /// <param name="provider">Провайдер соединения с базой</param>
        /// <param name="logFileName">Путь и название файла, куда будем писать логи, его отсутствие (null) отменяет запись в файл.</param>
        /// <param name="isSearchGac"> true:Искать поставшика данных в хранилище GAC</param>
        public Configure(string connectionString, ProviderName provider, string logFileName, bool isSearchGac=false)
        {

            _curFactory = null;
            ConnectionString = connectionString;
            _isSearchGac = isSearchGac;
            Provider = provider;
            LogFileName = logFileName;
            LogFileName = logFileName;
            ActivateLogger(logFileName);
            switch (provider)
            {
                case ProviderName.Postgresql:
                    {
                        try
                        {
                            if (!_isSearchGac) throw new Exception("Запрет на поиск в GAC");
                            _curFactory = DbProviderFactories.GetFactory("Npgsql");

                        }
                        catch 
                        {
                            try
                            {
                                var a = AppDomain.CurrentDomain.Load("Npgsql");
                                var b = a.GetType("Npgsql.NpgsqlFactory");
                                var field1 = b.GetField("Instance", BindingFlags.Static | BindingFlags.Public);
                                _curFactory = (DbProviderFactory)field1.GetValue(null);
                                AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
                                AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
                            }
                            catch (Exception e)
                            {
                               SendError(string.Empty,e);
                            }
                           
                        }
                       
                       
                       
                        break;
                    }
                case ProviderName.MySql:
                    {
                        try
                        {
                            if (!_isSearchGac) throw new Exception("Запрет на поиск в GAC");
                            _curFactory = DbProviderFactories.GetFactory("MySql.Data.MySqlClient");
                        }
                        catch 
                        {

                            var a = AppDomain.CurrentDomain.Load("Mysql.Data");
                            var b = a.GetType("MySql.Data.MySqlClient.MySqlClientFactory");
                            _curFactory = (DbProviderFactory)b.GetField("Instance").GetValue(null);
                        }
                     
                        break;
                    }
                case ProviderName.Sqlite:
                    {
                        try
                        {
                            if (!_isSearchGac) throw new Exception("Запрет на поиск в GAC");
                            _curFactory = DbProviderFactories.GetFactory("System.Data.SQLite.SQLiteFactory");
                        }
                        catch
                        {
                            var a = AppDomain.CurrentDomain.Load("System.Data.SQLite");
                            var b = a.GetType("System.Data.SQLite.SQLiteFactory");
                            var field1 = b.GetField("Instance", BindingFlags.Static | BindingFlags.Public);
                            _curFactory = (DbProviderFactory)field1.GetValue(null);
                         
                        }
                        break;
                   
                    }
                case ProviderName.MsSql:
                    {
                        try
                        {
                            if (!_isSearchGac) throw new Exception("Запрет на поиск в GAC");
                            _curFactory = DbProviderFactories.GetFactory("System.Data.SqlClient");
                        }
                        catch 
                        {
                            var a = AppDomain.CurrentDomain.Load("System.Data.SqlClient");
                            var b = a.GetType("System.Data.SqlClient.SqlClientFactory");
                            _curFactory = (DbProviderFactory)b.GetField("Instance").GetValue(null);
                        }
                       
                        break;

                    }
            }

            _configure = this;
        }

        /// <summary>
        /// Провайдер, который использует орм в текущий момент
        /// </summary>
        public static ProviderName Provider { get; private set; }

        /// <summary>
        ///Получение сессии
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static ISession Session
        {
            get
            {
                if (_configure == null)
                {
                    SendError(null, new Exception("ISession GetInnerSession error _configure==null"));
                    return null;
                }

                return _configure.GetInnerSession();
            }
        }

        /// <summary>
        /// Получение сессии к другой базе данных
        /// </summary>
        /// <typeparam name="TF">Тип который должен реализовать интерфейс IOtherDataBaseFactory и
        /// иметь конструктор по умолчанию</typeparam>
        /// <returns></returns>
        public static ISession GetSession<TF>()
        {
            if (_configure == null)
            {
                SendError(null, new Exception("ISession GetInnerSession error _configure==null"));
                return null;
            }
            Type type = typeof(TF);
            var any = type.GetInterfaces().Any(si => si == typeof(IOtherDataBaseFactory));
            if (any == false)
            {
                throw new Exception($"Тип {type.Name} не реализует интерфейс IOtherDataBaseFactory");
            }
            var isExistCtor = type.GetConstructor(Type.EmptyTypes);
            if (isExistCtor == null)
            {
                throw new Exception($"Тип {type.Name} не имеет конструктора по умолчанию");
            }

            return _configure.GetInnerSession<TF>();
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
        private ISession GetInnerSession<TF>()
        {
            var res = GetDataBaseFactory<TF>(); 
            return new Sessione(res);
        }

        private IOtherDataBaseFactory GetDataBaseFactory<TF>()
        {
            var o = StorageOtherBaseFactory<TF>.GetDataBaseFactory();
            return o;
        }


        private void OnOnErrorOrm(ErrorOrmEventArgs args)
        { 
            string errorMessage = args.ErrorMessage + Environment.NewLine + args.Sql;
            if (OnErrorOrm == null)
            {
                MySqlLogger.Info($"OnOnErrorOrm: {errorMessage}");
                throw new Exception(errorMessage);

            }

            OnErrorOrm.Invoke(this, args);

        }
        

    }
}