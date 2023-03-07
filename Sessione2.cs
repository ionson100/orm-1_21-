using ORM_1_21_.Transaction;
using ORM_1_21_.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace ORM_1_21_
{
    public sealed partial class Sessione
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


        private static void NotificAfter<T>(T item, ActionMode mode) where T : class
        {
            if (mode == ActionMode.None) return;
            if (!(item is IActionDal<T> dal)) return;
            switch (mode)
            {
                case ActionMode.Insert:
                    dal.AfterInsert(item);
                    break;
                case ActionMode.Update:
                    dal.AfterUpdate(item);
                    break;
                case ActionMode.Delete:
                    dal.AfterDelete(item);
                    break;
            }
        }

        private static void NotificBefore<T>(T item, ActionMode mode) where T : class
        {
            if (item is IValidateDal<T> dal && mode == ActionMode.Insert)
            {
                dal.Validate(item);
            }
            if (mode == ActionMode.None) return;
            if (!(item is IActionDal<T> actionDal)) return;
            switch (mode)
            {
                case ActionMode.Insert:
                    actionDal.BeforeInsert(item);
                    break;
                case ActionMode.Update:
                    actionDal.BeforeUpdate(item);
                    break;
                case ActionMode.Delete:
                    actionDal.BeforeDelete(item);
                    break;
            }
        }



        ITransaction ISession.BeginTransaction()
        {
            if (Transactionale.MyStateTransaction == StateTransaction.Begin)
            {
                throw new Exception("Transaction opened earlier");
            }
            Transactionale.MyStateTransaction = StateTransaction.Begin;
            Transactionale.IsolationLevel = null;
            Transactionale.Connection = _connect;
            return Transactionale;
        }


        ITransaction ISession.BeginTransaction(IsolationLevel? value)
        {
            if (Transactionale.MyStateTransaction == StateTransaction.Begin)
            {
                throw new Exception("Transaction opened earlier");
            }
            Transactionale.MyStateTransaction = StateTransaction.Begin;
            Transactionale.Connection = _connect;
            Transactionale.IsolationLevel = value;
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

        internal static void AddParam(IDbCommand com, ProviderName providerName, object[] obj)
        {
            if (obj == null) return;
            string sql = com.CommandText;
            var ss = sql.Split(UtilsCore.PrefParam(providerName).ToArray(), StringSplitOptions.RemoveEmptyEntries);
            if (ss.Length - 1 != obj.Length)
                throw new ArgumentException("the count of parameters does not match");

            var list = Regex.Matches(sql, @"\" + UtilsCore.PrefParam(providerName) + @"\w+").Cast<Match>().Select(m => m.Value).ToList();
            if (list.Count != obj.Length)
            {
                throw new Exception($"Count of parameters in sql query {list} does not match the count of parameters passed to the method {obj.Length}");
            }

            for (var index = 0; index < obj.Length; index++)
            {
                IDataParameter dp = com.CreateParameter();
                var parName = list[index];
                dp.ParameterName = parName;
                var value = obj[index]??DBNull.Value;
                dp.Value = value;

                com.Parameters.Add(dp);
            }
        }

        internal static void AddParamCore(IDbCommand com,  Parameter[] parameters)
        {
            if(parameters==null) return;
            foreach (var par in parameters)
            {
                if (par.UserParameter != null)
                {
                    com.Parameters.Add(par.UserParameter);
                }
                IDataParameter dp = com.CreateParameter();
                dp.ParameterName = par.Name;
                dp.DbType = DbType.Object;

              

                var value = par.Value??DBNull.Value;
                if (par.DbType.HasValue)
                {
                    dp.DbType = par.DbType.Value;
                }
                dp.Value = value;

                com.Parameters.Add(dp);
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
        
        int ISession.ExecuteNonQuery(string sql, params Parameter[] param)
        {
            if (sql == null) throw new ArgumentNullException(nameof(sql));
            var com = ProviderFactories.GetCommand(_factory, ((ISession)this).IsDispose);
            com.Connection = _connect;
            com.CommandText = sql;
            AddParamCore(com,  param);
            try
            {
                OpenConnectAndTransaction(com);
                com.CommandTimeout = 30000;
                return com.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MySqlLogger.Error(UtilsCore.GetStringSql(com), ex);
                throw;
            }
            finally
            {
                ComDisposable(com);
            }
        }

         int ISession.ExecuteNonQueryT(string sql, int timeOut, params Parameter[] param)
        {
            if (sql == null) throw new ArgumentNullException(nameof(sql));

            var com = ProviderFactories.GetCommand(_factory, ((ISession)this).IsDispose);
            com.Connection = _connect;
            com.CommandText = sql;
            AddParamCore(com, param);
            try
            {
                OpenConnectAndTransaction(com);
                com.CommandTimeout = 30000;
                SetTimeOut(com, timeOut);
                return com.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MySqlLogger.Error(UtilsCore.GetStringSql(com), ex);
                throw;
            }
            finally
            {
                ComDisposable(com);
            }
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


         object ISession.ExecuteScalar(string sql, params Parameter[] param)
        {
            if (sql == null) throw new ArgumentNullException(nameof(sql));
            var com = ProviderFactories.GetCommand(_factory, ((ISession)this).IsDispose);
            com.Connection = _connect;
            com.CommandType = CommandType.Text;
            com.CommandText = sql;
            AddParamCore(com,  param);

            try
            {
                OpenConnectAndTransaction(com);
                var res = com.ExecuteScalar();
                return res;
            }
            catch (Exception ex)
            {
                MySqlLogger.Error(UtilsCore.GetStringSql(com), ex);
                throw;
            }
            finally
            {
                ComDisposable(com);
            }
        }

          object ISession.ExecuteScalarT(string sql, int timeOut, params Parameter[] param)
         {
            if (sql == null) throw new ArgumentNullException(nameof(sql));
            var com = ProviderFactories.GetCommand(_factory, ((ISession)this).IsDispose);
            com.Connection = _connect;
            com.CommandType = CommandType.Text;
            com.CommandText = sql;
            AddParamCore(com,  param);
            SetTimeOut(com, timeOut);

            try
            {
                OpenConnectAndTransaction(com);
                var res = com.ExecuteScalar();
                return res;
            }
            catch (Exception ex)
            {
                MySqlLogger.Error(UtilsCore.GetStringSql(com), ex);
                throw;
            }
            finally
            {
                ComDisposable(com);
            }
        }

         bool ISession.IsPersistent<T>(T obj)
        {
            return UtilsCore.IsPersistent(obj);
        }


        void ISession.ToPersistent<T>(T obj)
        {
            UtilsCore.SetPersistent(obj);
        }



        IEnumerable<TableColumn> ISession.GetTableColumns(string tableName)
        {
            var com = ProviderFactories.GetCommand(_factory, ((ISession)this).IsDispose);
            com.Connection = _connect;
            return ColumnsTableFactory.GeTableColumns(MyProviderName, com, UtilsCore.ClearTrim(tableName.Trim()));


        }
    }
}
