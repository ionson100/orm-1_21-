using ORM_1_21_.Transaction;
using ORM_1_21_.Utils;
using System;
using System.Collections.Generic;
using System.Data;

namespace ORM_1_21_
{
    internal sealed partial class Sessione
    {
        private readonly IOtherDataBaseFactory _factory;
        private bool _isDispose;
        internal readonly Transactionale Transactionale = new Transactionale();

        internal IDbTransaction Transaction
        {
            get => Transactionale.Transaction;
            set => Transactionale.Transaction = value;
        }

        internal ProviderName MyProviderName
        {
            get
            {
                if (_factory == null)
                {
                    return Configure.Provider;
                }
                else
                {
                    return _factory.GetProviderName();
                }
            }
        }


        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        public Sessione(string connectionString)
        {
            Check.NotEmpty(connectionString, "connectionString");
            _connect = ProviderFactories.GetConnect(null);
            _connect.ConnectionString = connectionString;
        }
        /// <summary>
        /// Constructor for connecting to another database
        /// </summary>
        /// <param name="factory">Object implementing IOtherDataBaseFactory</param>
        public Sessione(IOtherDataBaseFactory factory)
        {
            _factory = factory;
            _connect = factory.GetDbProviderFactories().CreateConnection();
            if (_connect == null) throw new Exception("Can't connect to another database");
            _connect.ConnectionString = factory.GetConnectionString();
            if (factory.GetProviderName() == ProviderName.Postgresql)
            {
                AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
                AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
            }

        }

        private readonly IDbConnection _connect;


        private static void NotificAfter<TSource>(TSource source, ActionMode mode) where TSource : class
        {
            if (mode == ActionMode.None) return;
            if (!(source is IMapAction<TSource> actionDal)) return;
            switch (mode)
            {
                case ActionMode.None:
                    break;
                case ActionMode.Insert:
                    actionDal.ActionCommand(source,CommandMode.AfterInsert);
                    break;
                case ActionMode.Update:
                    actionDal.ActionCommand(source, CommandMode.AfterUpdate);
                    break;
                case ActionMode.Delete:
                    actionDal.ActionCommand(source, CommandMode.AfterDelete);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }

        private static void NotificBefore<TSource>(TSource source, ActionMode mode) where TSource : class
        {

          
            if (mode == ActionMode.None) return;
            if (!(source is IMapAction<TSource> actionDal)) return;
            switch (mode)
            {
                case ActionMode.None:
                    break;
                case ActionMode.Insert:
                    actionDal.ActionCommand(source, CommandMode.BeforeInsert);
                    break;
                case ActionMode.Update:
                    actionDal.ActionCommand(source, CommandMode.BeforeUpdate);
                    break;
                case ActionMode.Delete:
                    actionDal.ActionCommand(source, CommandMode.BeforeDelete);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }

        }

        ITransaction ISession.BeginTransaction()
        {
            if (Transactionale.MyStateTransaction == StateTransaction.Begin)
            {
                Transactionale.isError = true;
                throw new Exception("Transaction opened earlier");
            }
            Transactionale.MyStateTransaction = StateTransaction.Begin;
            Transactionale.IsolationLevel = null;
            Transactionale.Connection = _connect;
            Transactionale.isError = false;
            return Transactionale;
        }


        ITransaction ISession.BeginTransaction(IsolationLevel? value)
        {
            if (Transactionale.MyStateTransaction == StateTransaction.Begin)
            {
                Transactionale.isError = true;
                throw new Exception("Transaction opened earlier");
            }
            Transactionale.MyStateTransaction = StateTransaction.Begin;
            Transactionale.Connection = _connect;
            Transactionale.IsolationLevel = value;
            Transactionale.isError = false;
            return Transactionale;
        }

        ///<summary>
        /// Disposing
        ///</summary>
        public void Dispose()
        {
            InnerDispose();
        }

        internal void OpenConnectAndTransaction(IDbCommand com)
        {
            if (com.Connection.State == ConnectionState.Closed)
            {
                com.Connection.Open();
                if (Transactionale.MyStateTransaction == StateTransaction.Begin)
                {

                    if (Transactionale.IsolationLevel == null)
                    {
                        Transaction = _connect.BeginTransaction();
                    }
                    else
                    {
                        Transaction = _connect.BeginTransaction(Transactionale.IsolationLevel.Value);
                    }
                    Transactionale.ListDispose.Add(com);
                    com.Transaction = Transaction;
                }
            }
            else
            {
                if (Transactionale.MyStateTransaction == StateTransaction.Begin)
                {
                    com.Transaction = Transaction;
                }
            }
        }

        bool ISession.IsDispose => _isDispose;


        /// <summary>
        /// finally call dispose
        /// </summary>
        ~Sessione()
        {
            InnerDispose(true);
        }

        void InnerDispose(bool isFinalize = false)
        {
            if (_isDispose) return;
            try
            {
                if (Transactionale.Transaction != null)
                {
                    if (Transactionale.MyStateTransaction == StateTransaction.Begin)
                    {
                        Transactionale.Transaction.Rollback();
                    }
                }
                Transactionale?.ListDispose.ForEach(a => a.Dispose());
                if (_connect != null)
                    _connect.Dispose();

                _isDispose = true;
                foreach (var dbCommand in _dbCommands)
                {
                    dbCommand.Dispose();
                }
                _dbCommands.Clear();
                if (isFinalize == false)
                    GC.SuppressFinalize(this);
            }
            catch (Exception)
            {
                //ignored
            }
        }

        void ISession.WriteLogFile(string message)
        {
            InnerWriteLogFile($"WriteLogFile: {message}");
        }

        private void InnerWriteLogFile(string message)
        {
            MySqlLogger.Info(message);
        }

        private void InnerWriteLogFile(IDbCommand command)
        {
            MySqlLogger.Info(UtilsCore.GetStringSql(command));

        }


        void ISession.WriteLogFile(IDbCommand command)
        {
            InnerWriteLogFile($"WriteLogFile: {command}");

        }

        bool ISession.IsPersistent<TSource>(TSource source)
        {
            return UtilsCore.IsPersistent(source);
        }

        void ISession.ToPersistent<TSource>(TSource source)
        {
            UtilsCore.SetPersistent(source);
        }

        IEnumerable<TableColumn> ISession.GetTableColumns(string tableName)
        {
            Check.NotEmpty(tableName, "tableName",() => Transactionale.isError = true);
            var com = ProviderFactories.GetCommand(_factory, ((ISession)this).IsDispose);
            com.Connection = _connect;
            return ColumnsTableFactory.GeTableColumns(MyProviderName, com, UtilsCore.ClearTrim(tableName.Trim()));
        }
    }
}
