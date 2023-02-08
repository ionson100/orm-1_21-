using ORM_1_21_.Transaction;
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
        /// Конструктор 
        /// </summary>
        /// <param name="connectionString">Строка соединения с базой</param>
        public Sessione(string connectionString)
        {
            _connect = ProviderFactories.GetConnect(null);
            _connect.ConnectionString = connectionString;
        }
        /// <summary>
        /// Конструктор для подключения к другой базе
        /// </summary>
        /// <param name="factory">Объект реализующий соединение к другой базе</param>
        public Sessione(IOtherDataBaseFactory factory)
        {
            _factory = factory;
            _connect = factory.GetDbProviderFactories().CreateConnection();
            if (_connect == null) throw new Exception("Не могу создать соединение к другой базе");
            _connect.ConnectionString = factory.GetConnectionString();
            if(factory.GetProviderName() == ProviderName.Postgresql)
            {
                AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
                AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
            }
            
        }

        private readonly IDbConnection _connect;

      

        private static void NotificAfter<T>(T item, ActionMode mode) where T : class
        {
            if (mode == ActionMode.None) return;
            if (!(item is IActionDal<T>)) return;
            if (mode == ActionMode.Insert)
                ((IActionDal<T>)item).AfterInsert(item);
            if (mode == ActionMode.Update)
                ((IActionDal<T>)item).AfterUpdate(item);
            if (mode == ActionMode.Delete)
                ((IActionDal<T>)item).AfterDelete(item);
        }

        private static void NotificBefore<T>(T item, ActionMode mode) where T : class
        {

            if (item is IValidateDal<T> && mode == ActionMode.Insert)
            {
                ((IValidateDal<T>)item).Validate(item);
            }
            if (mode == ActionMode.None) return;
            if (!(item is IActionDal<T>)) return;
            if (mode == ActionMode.Insert)
                ((IActionDal<T>)item).BeforeInsert(item);
            if (mode == ActionMode.Update)
                ((IActionDal<T>)item).BeforeUpdate(item);
            if (mode == ActionMode.Delete)
                ((IActionDal<T>)item).BeforeDelete(item);
        }



         ITransaction ISession.BeginTransaction()
        {
            Transactionale.IsOccupied = true;
            Transactionale.Connection = _connect;
            return Transactionale;
        }

        
         ITransaction ISession.BeginTransaction(IsolationLevel value)
        {
            Transactionale.IsOccupied = true;
            Transactionale.Connection = _connect;
            Transactionale.IsolationLevel = value;
            return Transactionale;
        }



        ///<summary>
        /// Освобождение ресурсов
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
                if (Transactionale.IsOccupied)
                {
                    Transaction = _connect.BeginTransaction(Transactionale.IsolationLevel);
                    Transactionale.ListDispose.Add(com);
                }
            }
            com.Transaction = Transaction;
        }

        internal static void AddParam(IDbCommand com,ProviderName providerName, object[] obj)
        {
            if (obj == null) return;
            string sql = com.CommandText;
            var ss = sql.Split(Utils.Prefparam(providerName).ToArray(), StringSplitOptions.RemoveEmptyEntries);
            if (ss.Length - 1 != obj.Length)
                throw new ArgumentException("не совпадает количество параметров");

            var list = Regex.Matches(sql, @"\" + Utils.Prefparam(providerName) + @"\w+").Cast<Match>().Select(m => m.Value).ToList();
            if (list.Count != obj.Length)
            {
                throw new Exception($"Количество параметров в sql запросе {list} не совпадает с количеством параметров переданных в метод {obj.Length}");
            }

            for (var index = 0; index < obj.Length; index++)
            {
                IDataParameter dp = com.CreateParameter();
                var parName = list[index];
                dp.ParameterName = parName;
                dp.Value = obj[index];

                com.Parameters.Add(dp);
            }
        }
     

        bool ISession.IsDispose => _isDispose;

        /// <summary>
        /// Позволяет объекту <see cref="T:System.Object"/> попытаться освободить ресурсы и выполнить другие операции очистки, перед тем как объект 
        /// <see cref="T:System.Object"/> будет утилизирован в процессе сборки мусора.
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
                Transactionale.ListDispose.ForEach(a => a.Dispose());
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
            MySqlLogger.Info(Utils.GetStringSql(command));

        }


        void ISession.WriteLogFile(IDbCommand command)
        {
            InnerWriteLogFile($"WriteLogFile: {command}");

        }

        
         bool ISession.IsPersistent<T>(T obj)
        {
            return Utils.IsPersistent(obj);
        }
         
         
         void ISession.ToPersistent<T>(T obj)
        {
            Utils.SetPersisten(obj);
        }

         private static void CloneItems<T>(T item, object o)
        {

            var rr = o.GetType().GetProperties();
            foreach (var propertyInfo in item.GetType().GetProperties())
            {
                var value =
                    rr.Single(a => a.Name == propertyInfo.Name && a.DeclaringType == propertyInfo.DeclaringType)
                      .GetValue(o, null);
                propertyInfo.SetValue(item, value, null);
            }
        }

      
         IEnumerable<TableColumn> ISession.GetTableColumns(string tableName)
         {
             var com = ProviderFactories.GetCommand(_factory, ((ISession)this).IsDispose);
             com.Connection = _connect;
             return ColumnsTableFactory.GeTableColumns(MyProviderName, com,Utils.ClearTrim(tableName.Trim()));


         }
    }
}
