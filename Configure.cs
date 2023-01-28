using System;
using System.IO;


namespace ORM_1_21_
{
    /// <summary>
    ///     Базовый класс для конфигурации
    /// </summary>
    public sealed partial class Configure
    {
        //internal static bool UsageCache;

        internal static string ConnectionString;

        /// <summary>
        /// GetConnectionString
        /// </summary>
        /// <returns></returns>
        public static string GetConnectionString()
        {
            lock (_locker)
            {
                return ConnectionString;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string LogFileName { get; private set; }


        private static Configure _configure;

        private static readonly object _locker = new object();

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="connectionString">Строка соединения с базой</param>
        /// <param name="provider">Провайдер соединения с базой</param>
        /// <param name="logFileName">Путь и название файла, куда будем писать логи, его отсутствие (null) отменяет запиь в файл.</param>
        public Configure(string connectionString, ProviderName provider, string logFileName)
        {

            ProviderFactories.AsNullDbProviderFactory();
            ConnectionString = connectionString;
            Provider = provider;
            LogFileName = logFileName;
            LogFileName = logFileName;
            ActivateLogger(logFileName);
            switch (provider)
            {
                case ProviderName.Postgresql:
                    {
                        Utils.Assembler = AppDomain.CurrentDomain.Load("Npgsql");
                        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
                        AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
                        break;
                    }
                case ProviderName.MySql:
                    {
                        Utils.Assembler = AppDomain.CurrentDomain.Load("Mysql.Data");
                        break;
                    }
                case ProviderName.Sqlite:
                    {
                        Utils.Assembler = AppDomain.CurrentDomain.Load("System.Data.SQLite");
                        break;
                    }
                case ProviderName.MsSql:
                    {
                        Utils.Assembler = AppDomain.CurrentDomain.Load("System.Data.SqlClient");
                        break;

                    }
            }

            _configure = this;
        }

        /// <summary>
        /// Провайдер, которы использует орм в текущий момент
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

        private static void ActivateLogger(string fileNameLogFile)
        {
            if (Configure.LogFileName == null) return;
            if (!File.Exists(fileNameLogFile))
                using (File.Create(fileNameLogFile))
                {
                }

            MySqlLogger.RunLogger(fileNameLogFile);
        }

        private ISession GetInnerSession()
        {
            lock (_locker)
            {
                return new Sessione(ConnectionString);
            }
        }


        private void OnOnErrorOrm(ErrorOrmEventArgs args)
        {
            if (onErrorOrm == null)
            {
                string errorMessage = args.ErrorMessage + Environment.NewLine + args.Sql;
                MySqlLogger.Info(errorMessage);
                throw new Exception(errorMessage);

            }

            onErrorOrm.Invoke(this, args);

        }


    }
}