using ORM_1_21_.Transaction;
using System;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace ORM_1_21_
{
    public sealed partial class Sessione
    {

        private bool _isDispose;
        internal readonly Transactionale Transactionale = new Transactionale();

        internal IDbTransaction Transaction
        {
            get => Transactionale.Transaction;
            set => Transactionale.Transaction = value;
        }


        /// <summary>
        /// Конструктор 
        /// </summary>
        /// <param name="connectionString">Строка соединения с базой</param>
        public Sessione(string connectionString)
        {
            _connect = ProviderFactories.GetConnect();// dF.CreateConnection();
            _connect.ConnectionString = connectionString;
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



        /// <summary>
        /// Получение объекта ITransaction с одновременно началом транзакции
        /// </summary>
        /// <returns>ITransaction</returns>
        public ITransaction BeginTransaction()
        {
            Transactionale.IsOccupied = true;
            Transactionale.Connection = _connect;
            return Transactionale;
        }

        /// <summary>
        ///  Получение объекта ITransaction с одновременно началом транзакции
        /// </summary>
        /// <param name="value">Уровни изоляции</param>
        /// <returns>ITransaction</returns>
        public ITransaction BeginTransaction(IsolationLevel value)
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


            Transactionale.ListDispose.ForEach(a => a.Dispose());
            if (_connect != null)
                _connect.Dispose();
            GC.SuppressFinalize(this);
            _isDispose = true;
            foreach (var dbCommand in _dbCommands)
            {
                dbCommand.Dispose();
            }
            _dbCommands.Clear();
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

        internal static void AddParam(IDbCommand com, object[] obj)
        {
            if (obj == null) return;
            string sql = com.CommandText;
            var ss = sql.Split(Utils.Prefparam.ToArray(), StringSplitOptions.RemoveEmptyEntries);
            if (ss.Length - 1 != obj.Length)
                throw new ArgumentException("не совпадает количество параметров");

            var list = Regex.Matches(sql, @"\" + Utils.Prefparam + @"\w+").Cast<Match>().Select(m => m.Value).ToList();
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
            Transactionale.ListDispose.ForEach(a => a.Dispose());
            try
            {
                _connect?.Dispose();

            }
            catch (Exception)
            {
                //ignored
            }

        }

        /// <summary>
        /// Запись в лог файл
        /// </summary>
        /// <param name="message">текст сообщения</param>
        public void WriteLogFile(string message)
        {
            MySqlLogger.Info(message);

        }

        /// <summary>
        /// Писать в лог файл напрямую sql запрос
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void WriteLogFile(IDbCommand command)
        {
            MySqlLogger.Info(Utils.GetStringSql(command));

        }

        /// <summary>
        /// Определяет, откуда объект
        /// </summary>
        /// <returns></returns>
        public bool IsPersistent(object obj)
        {
            return Utils.IsPersistent(obj);
        }

        /// <summary>
        /// Пометить объект, что он получен из базы
        /// </summary>
        public void ToPersistent(object obj)
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
    }
}
