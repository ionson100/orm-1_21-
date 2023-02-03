
using Newtonsoft.Json;
using ORM_1_21_.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ORM_1_21_
{
    ///<summary>
    ///</summary>
    public sealed partial class Sessione : ISession, IServiceSessions
    {
        List<IDbCommand> _dbCommands = new List<IDbCommand>();
        IDbCommand IServiceSessions.CommandForLinq
        {
            get
            {
                if (_factory != null)
                {
                    IDbCommand comf = _factory.GetDbCommand();
                    comf.Connection = _connect;
                    return comf;
                }
                var com = ProviderFactories.GetCommand(GetCurrentProviderName());
                com.Connection = _connect;
                return com;

            }
        }

        ProviderName IServiceSessions.CurrentProviderName => GetCurrentProviderName();

        object IServiceSessions.Locker { get; } = new object();



        //Dictionary<int, BoxCache> IServiceSessions.CacheFirstLevel { get; } = new Dictionary<int, BoxCache>();

        /// <summary>
        ///     Удаление объекта из базы
        /// </summary>
        /// <typeparam name="T">Тип удаляемого объекта</typeparam>
        /// <param name="item">Удаляемый объект</param>
        public int Delete<T>(T item) where T : class
        {
            if (!Utils.IsPersistent(item)) return 0;
            var com = ProviderFactories.GetCommand(GetCurrentProviderName());
            com.Connection = _connect;
            AttributesOfClass<T>.CreateDeleteCommand(com, item,GetCurrentProviderName());
            try
            {
                NotificBefore(item, ActionMode.Delete);
                OpenConnectAndTransaction(com);
                return com.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Configure.SendError(Utils.GetStringSql(com), ex);
                return 0;
            }
            finally
            {
                ComDisposable(com);
                NotificAfter(item, ActionMode.Delete);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> GetListMonster<T>(IDataReader reader) where T : class
        {
            return AttributesOfClass<T>.GetEnumerableObjects(reader,GetCurrentProviderName());
        }

        /// <summary>
        ///     Сохранение или обновление объекта в базе
        /// </summary>
        /// <typeparam name="T">Тип объекта</typeparam>
        /// <param name="item">Cохраняемый или обновляемый объект</param>
        public int Save<T>(T item) where T : class
        {
            if (item == null) throw new ArgumentException("Объект для сохранения равен Null");
            return SaveNew(item);
        }

        /// <summary>
        ///     Получение объекта по первичному ключу
        /// </summary>
        /// <typeparam name="T">Тип объекта</typeparam>
        /// <param name="id">Значение первичного ключа</param>
        /// <returns>Полученный объект, в случае отсутствия  в базe - NULL</returns>
        public T Get<T>(object id) where T : class
        {
            if (id == null) throw new ArgumentException("Объект первичного ключа, равен равен Null");
            return GetReal<T>(id);
        }

        /// <summary>
        /// Запрос на выборку с параметрами
        /// </summary>
        /// <param name="sqlWhere">Запрос на выборку, начиная с после where  с параметрами, можно поставить: 1=1</param>
        /// <param name="param">Список параметров в той последовательности в которой они расположены в запросе.</param>
        /// <typeparam name="T">Тип сущности</typeparam>
        /// <returns></returns>
        public IEnumerable<T> GetList<T>(string sqlWhere, params object[] param) where T : class
        {
            if (sqlWhere == null) sqlWhere = "";
            var sqlAll = AttributesOfClass<T>.SimpleSqlSelect(GetCurrentProviderName()) + AttributesOfClass<T>.AddSqlWhere(sqlWhere);
            var com = ProviderFactories.GetCommand(GetCurrentProviderName());
            com.CommandText = sqlAll;
            AddParam(com,GetCurrentProviderName(), param);
            com.Connection = _connect;
            IEnumerable<T> res;
            try
            {
                //var res1 = GetCache<T>(true, com);
                // if (res1 != null) return (IEnumerable<T>) res1;
                OpenConnectAndTransaction(com);
                res = AttributesOfClass<T>.GetEnumerableObjects(com.ExecuteReader(),GetCurrentProviderName());
                //SetCache<T>(true, com, res);
            }
            catch (Exception ex)
            {
                Configure.SendError(Utils.GetStringSql(com), ex);
                return null;
            }
            finally
            {
                ComDisposable(com);
            }
            return res;
        }

        /// <summary>
        ///     Создание таблицы
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void TableCreate<T>() where T : class
        {
            var ss = new FactoryCreatorTable().SqlCreate<T>(GetCurrentProviderName());
            var com = ProviderFactories.GetCommand(GetCurrentProviderName());
            com.Connection = _connect;

            com.CommandText = ss;
            try
            {
                OpenConnectAndTransaction(com);
                com.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Configure.SendError(Utils.GetStringSql(com), ex);
            }
            finally
            {
                ComDisposable(com);
                WriteLogFile(ss);
            }
        }

        /// <summary>
        ///     Получение IDbCommand,Закрывать соединение вручную
        /// </summary>
        /// <returns></returns>
        public IDbCommand GeDbCommand()
        {
            return ProviderFactories.GetCommand(GetCurrentProviderName());
        }

        /// <summary>
        ///     Удаление таблицы
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void DropTable<T>() where T : class
        {
            var com = ProviderFactories.GetCommand(GetCurrentProviderName());
            com.Connection = _connect;

            com.CommandText = $"DROP TABLE {AttributesOfClass<T>.TableName}";
            try
            {
                OpenConnectAndTransaction(com);
                com.ExecuteNonQuery();
                WriteLogFile(com.CommandText);
            }
            catch (Exception ex)
            {
                Configure.SendError(Utils.GetStringSql(com), ex);
            }
            finally
            {
                ComDisposable(com);
            }
        }

        /// <summary>
        ///     Проверка на существование таблицы
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool TableExists<T>() where T : class
        {
            var com = ProviderFactories.GetCommand(GetCurrentProviderName());
            try
            {
                if (GetCurrentProviderName() == ProviderName.Postgresql)
                {
                    com.Connection = _connect;
                    var tableName = Utils.ClearTrim(AttributesOfClass<T>.TableName);
                    com.CommandText =
                        $"SELECT count(*) FROM pg_tables WHERE   tablename  = '{tableName}';";

                    OpenConnectAndTransaction(com);
                    long res = (long)com.ExecuteScalar();
                    return res != 0;
                }
                else
                {
                    com = ProviderFactories.GetCommand(GetCurrentProviderName());
                    com.Connection = _connect;
                    com.CommandText = $"select 1 from {AttributesOfClass<T>.TableName};";
                    OpenConnectAndTransaction(com);
                    com.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                ComDisposable(com);
            }
        }

        /// <summary>
        ///     Пожарный шланг данных
        /// </summary>
        /// <param name="sql">Строка запроса</param>
        /// <param name="param">Список параметров в той последовательности в которой они расположены в запросе.</param>
        /// <returns></returns>
        public IDataReader ExecuteReader(string sql, object[] param)
        {
            var com = ProviderFactories.GetCommand(GetCurrentProviderName());
            com.Connection = _connect;

            com.CommandText = sql;
            AddParam(com, GetCurrentProviderName(), param);
            OpenConnectAndTransaction(com);
            return com.ExecuteReader();
        }

        private void SetTimeOut(IDbCommand com, int timeOut)
        {
            com.CommandTimeout = timeOut < 30 ? 30 : timeOut;
        }
        /// <summary>
        /// IDataReader
        /// </summary>
        /// <param name="sql">Строка запроса</param>
        /// <param name="timeOut">Время ожидания выполнения команды (30 сек)</param>
        /// <param name="param">Список параметров в той последовательности в которой они расположены в запросе.</param>
        /// <returns></returns>
        public IDataReader ExecuteReaderT(string sql, int timeOut = 30, params object[] param)
        {
            var com = ProviderFactories.GetCommand(GetCurrentProviderName());
            com.Connection = _connect;
            com.CommandText = sql;
            SetTimeOut(com, timeOut);
           
            AddParam(com, GetCurrentProviderName(), param);
            OpenConnectAndTransaction(com);
            return com.ExecuteReader();
        }

        /// <summary>
        ///     получение DataTable
        /// </summary>
        /// <param name="sql">Строка запроса</param>
        /// <param name="timeOut">Время ожидания выполнения команды (30 сек)</param>
        /// <returns></returns>
        public DataTable GetDataTable(string sql, int timeOut = 30)
        {
            var table = new DataTable();

            var com = ProviderFactories.GetCommand(GetCurrentProviderName());
            com.Connection = _connect;
            SetTimeOut(com, timeOut);

            com.CommandText = sql;
            try
            {
                OpenConnectAndTransaction(com);
                var reader = com.ExecuteReader();
                table.BeginLoadData();
                table.Load(reader);
                table.EndLoadData();
                WriteLogFile(com.CommandText);
                return table;
            }
            catch (Exception ex)
            {
                Configure.SendError(com.CommandText, ex);
                return null;
            }
            finally
            {
                ComDisposable(com);
            }
        }

        /// <summary>
        /// Список таблиц в базе
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public List<string> GetTableNames()
        {

            var com = ProviderFactories.GetCommand(GetCurrentProviderName());

            var index = 2;

            switch (GetCurrentProviderName())
            {
                case ProviderName.Sqlite:
                    {
                        index = 0;
                        com.CommandText = "SELECT name FROM sqlite_master WHERE type='table';";
                        break;
                    }
                case ProviderName.MsSql:
                    {
                        com.CommandText = "SELECT * FROM information_schema.tables";
                        index = 2;
                        break;
                    }

                case ProviderName.MySql:
                    {
                        index = 0;
                        var strs = Configure.ConnectionString.Split(';');
                        foreach (var str in strs)
                            if (str.ToUpper().Contains("DATABASE"))
                            {
                                var nameBase = str.Split('=')[1].Trim();
                                com.CommandText =
                                    $"SELECT table_name FROM information_schema.tables WHERE table_schema = '{nameBase}';";
                            }
                        break;
                    }

                case ProviderName.Postgresql:
                    {
                        index = 0;
                        com.CommandText = "SELECT table_name FROM information_schema.tables where table_schema='public'";
                        break;
                    }

                default:
                    throw new ArgumentOutOfRangeException();
            }


            com.Connection = _connect;

            var result = new List<string>();
            try
            {
                OpenConnectAndTransaction(com);

                var reader = com.ExecuteReader();
                while (reader.Read()) result.Add(reader.GetString(index));
                reader.Close();
            }
            catch (Exception ex)
            {
                Configure.SendError(Utils.GetStringSql(com), ex);
            }
            finally
            {
                ComDisposable(com);
            }

            return result;
        }

        /// <summary>
        /// Создает базу данных
        /// </summary>
        /// <param name="baseName">название базы для Mysql  , путь до базы для Postgesql,Sqlite,MSSql</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public int CreateBase(string baseName)
        {
            var com = ProviderFactories.GetCommand(GetCurrentProviderName());
            com.Connection = _connect;


            try
            {
                switch (GetCurrentProviderName())
                {
                    case ProviderName.MsSql:
                        if (File.Exists(baseName))
                            return 0;
                        var bName = Path.GetFileName(baseName).Substring(0, length: Path.GetFileName(baseName).IndexOf('.'));
                        com.CommandText = $"CREATE DATABASE {bName} ON(NAME = {bName}, FILENAME = '{baseName}')";
                        break;
                    case ProviderName.MySql:
                        com.CommandText = $"CREATE DATABASE [IF NOT EXISTS] {baseName}";
                        break;
                    case ProviderName.Postgresql:
                        com.CommandText = $"CREATE DATABASE {baseName};";
                        return -1;
                    case ProviderName.Sqlite:
                        if (File.Exists(baseName) == false)
                        {
                            com.Connection.GetType().GetMethod("CreateFile", BindingFlags.Static | BindingFlags.Public)
                                ?.Invoke(com.Connection, new object[] { baseName });
                            return -1;
                        }

                        return 0;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                OpenConnectAndTransaction(com);
                return com.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Configure.SendError(com.CommandText, ex);
                return 0;
            }
            finally
            {
                ComDisposable(com);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="list"></param>
        /// <param name="timeOut">Время ожидания выполнения команды (30 сек)</param>
        /// <typeparam name="T"></typeparam>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void InsertBulk<T>(IEnumerable<T> list, int timeOut = 30)
        {
            var com = ProviderFactories.GetCommand(GetCurrentProviderName());
            com.Connection = _connect;
            switch (GetCurrentProviderName())
            {
                case ProviderName.MsSql:
                    
                    com.CommandText = new UtilsBulkMsSql(ProviderName.MsSql).GetSql(list);
                    break;
                case ProviderName.MySql:
                    com.CommandText = new UtilsBulkMySql(ProviderName.MySql).GetSql(list);
                    break;
                case ProviderName.Postgresql:
                    com.CommandText =new  UtilsBulkPostgres(ProviderName.Postgresql).GetSql(list);
                    break;
                case ProviderName.Sqlite:
                    com.CommandText = new UtilsBulkMySql(ProviderName.Sqlite).GetSql(list);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            try
            {
                OpenConnectAndTransaction(com);
                com.CommandTimeout = 30000;
                SetTimeOut(com, timeOut);
                com.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Configure.SendError(Utils.GetStringSql(com), ex);
            }
            finally
            {
                ComDisposable(com);
            }
        }

        /// <summary>
        /// Insert Bulk Fom File
        /// </summary>
        /// <param name="fileCsv"></param>
        /// <param name="fieldterminator"></param>
        /// <param name="timeOut">Время ожидания выполнения команды (30 сек)</param>
        /// <typeparam name="T"></typeparam>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void InsertBulkFromFile<T>(string fileCsv, string fieldterminator = ";", int timeOut = 30)
        {
            var com = ProviderFactories.GetCommand(GetCurrentProviderName());
            com.Connection = _connect;
            SetTimeOut(com, timeOut);
            switch (GetCurrentProviderName())
            {
                case ProviderName.MsSql:
                    com.CommandText = UtilsBulkMsSql.InsertFile<T>(fileCsv, fieldterminator);
                    break;
                case ProviderName.MySql:
                    com.CommandText = UtilsBulkMySql.InsertFile<T>(fileCsv, fieldterminator);
                    break;
                case ProviderName.Postgresql:
                    com.CommandText = UtilsBulkPostgres.InsertFile<T>(fileCsv, fieldterminator);
                    break;
                case ProviderName.Sqlite:
                    com.CommandText = UtilsBulkMySql.InsertFile<T>(fileCsv, fieldterminator);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            try
            {
                OpenConnectAndTransaction(com);
                com.CommandTimeout = 30;
                com.ExecuteReader();
            }
            catch (Exception ex)
            {
                Configure.SendError(Utils.GetStringSql(com), ex);
            }
            finally
            {
                ComDisposable(com);
            }
        }

        /// <summary>
        /// ExecuteScalar
        /// </summary>
        /// <param name="sql">Строка запроса</param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public object ExecuteScalar(string sql, params object[] obj)
        {
            var com = ProviderFactories.GetCommand(GetCurrentProviderName());
            com.Connection = _connect;
            com.CommandType = CommandType.Text;
            com.CommandText = sql;
            AddParam(com, GetCurrentProviderName(), obj);

            try
            {
                OpenConnectAndTransaction(com);
                var res = com.ExecuteScalar();
                return res;
            }
            catch (Exception ex)
            {
                Configure.SendError(Utils.GetStringSql(com), ex);
                return null;
            }
            finally
            {
                ComDisposable(com);
            }
        }

        /// <summary>
        /// ExecuteScalarT (timeout)
        /// </summary>
        /// <param name="sql">Строка запроса</param>
        /// <param name="timeOut">Время ожидания выполнения команды (30 сек)</param>
        /// <param name="param">Список параметров в той последовательности в которой они расположены в запросе.</param>
        /// <returns></returns>
        public object ExecuteScalarT(string sql, int timeOut = 30, params object[] param)
        {
            var com = ProviderFactories.GetCommand(GetCurrentProviderName());
            com.Connection = _connect;
            com.CommandType = CommandType.Text;
            com.CommandText = sql;
            AddParam(com, GetCurrentProviderName(), param);
            SetTimeOut(com, timeOut);

            try
            {
                OpenConnectAndTransaction(com);
                var res = com.ExecuteScalar();
                return res;
            }
            catch (Exception ex)
            {
                Configure.SendError(Utils.GetStringSql(com), ex);
                return null;
            }
            finally
            {
                ComDisposable(com);
            }
        }

        /// <summary>
        /// Очистка таблицы
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public int TruncateTable<T>()
        {
            var com = ProviderFactories.GetCommand(GetCurrentProviderName());
            com.Connection = _connect;
            com.CommandType = CommandType.Text;
            if (GetCurrentProviderName() == ProviderName.Sqlite)
                com.CommandText = $"DELETE FROM {AttributesOfClass<T>.TableName};";
            else
                com.CommandText = $"TRUNCATE TABLE {AttributesOfClass<T>.TableName};";

            try
            {
                OpenConnectAndTransaction(com);
                return com.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Configure.SendError(Utils.GetStringSql(com), ex);
                return 0;
            }
            finally
            {
                ComDisposable(com);
            }
        }

        /// <summary>
        ///     Получения выражения ling to SQL 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Query<T> Querion<T>()
        {
            AttributesOfClass<T>.CurProvider = _factory != null ? _factory.GetProviderName() : Configure.Provider;
            QueryProvider p = new DbQueryProvider<T>(this);
            return new Query<T>(p);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IDbCommand GetCommand()
        {
            var com = ProviderFactories.GetCommand(GetCurrentProviderName());
            com.Connection = _connect;
            return com;
        }

        /// <summary>
        /// DbConnection
        /// </summary>
        /// <returns></returns>
        public IDbConnection GetConnection()
        {
            return ProviderFactories.GetConnect(GetCurrentProviderName());
        }

        /// <summary>
        /// IDbDataAdapter
        /// </summary>
        /// <returns></returns>
        public IDbDataAdapter GetDataAdapter()
        {
            return ProviderFactories.GetDataAdapter(GetCurrentProviderName());
        }

        /// <summary>
        /// IDataParameter
        /// </summary>
        /// <returns></returns>
        public IDataParameter GetDataParameter()
        {
            return ProviderFactories.GetParameter(GetCurrentProviderName());
        }


        /// <summary>
        ///     Строка подключения
        /// </summary>
        /// <returns></returns>
        public string GetConnectionString()
        {
            return _connect.ConnectionString;
        }

        /// <summary>
        ///     Без возврата результата
        /// </summary>
        /// <param name="sql">Строка запроса</param>
        /// <param name="param">Список параметров в той последовательности в которой они расположены в запросе.</param>
        public int ExecuteNonQuery(string sql, params object[] param)
        {
            var com = ProviderFactories.GetCommand(GetCurrentProviderName());
            com.Connection = _connect;
            com.CommandText = sql;
            AddParam(com, GetCurrentProviderName(), param);
            try
            {
                OpenConnectAndTransaction(com);
                com.CommandTimeout = 30000;
                return com.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Configure.SendError(Utils.GetStringSql(com), ex);
                return -1;
            }
            finally
            {
                ComDisposable(com);
            }
        }

        /// <summary>
        /// Выполнение запроса с параметрами и TimeOut
        /// </summary>
        /// <param name="sql">Строка запроса</param>
        /// <param name="timeOut">Время ожидания выполнения команды (30 сек)</param>
        /// <param name="param">Список параметров в той последовательности в которой они расположены в запросе.</param>
        /// <returns></returns>
        public int ExecuteNonQueryT(string sql, int timeOut = 30, params object[] param)
        {

            var com = ProviderFactories.GetCommand(GetCurrentProviderName());
            com.Connection = _connect;
            com.CommandText = sql;
            AddParam(com, GetCurrentProviderName(), param);
            try
            {
                OpenConnectAndTransaction(com);
                com.CommandTimeout = 30000;
                SetTimeOut(com, timeOut);
                return com.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Configure.SendError(Utils.GetStringSql(com), ex);
                return -1;
            }
            finally
            {
                ComDisposable(com);
            }
        }

        /// <summary>
        /// Table name from base
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public string TableName<T>()
        {
            return AttributesOfClass<T>.TableName;
        }


        /// <summary>
        /// Получения списка прототипов объекта
        /// </summary>
        /// <param name="obj">прототип</param>
        /// <param name="sqlWhere">запрос Where включительно</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public IEnumerable<T> GetList<T>(T obj, string sqlWhere) where T : class
        {
            IEnumerable<T> lResult;
            var com = ProviderFactories.GetCommand(GetCurrentProviderName());
            com.Connection = _connect;
            com.CommandText = AttributesOfClass<T>.SimpleSqlSelect(GetCurrentProviderName()) + AttributesOfClass<T>.AddSqlWhere(sqlWhere);
            try
            {
                OpenConnectAndTransaction(com);
                lResult = AttributesOfClass<T>.GetEnumerableObjects(com.ExecuteReader(),GetCurrentProviderName());
            }
            catch (Exception ex)
            {
                Configure.SendError(Utils.GetStringSql(com), ex);
                return null;
            }
            finally
            {
                ComDisposable(com);
            }

            return lResult;
        }

        /// <summary>
        /// Возвращает лист объектов  табличной сущности
        /// </summary>
        /// <typeparam name="T">Тип класса сущности</typeparam>
        /// <param name="sqlWhere">
        /// Запрос на выборку начиная с Where, Where включительно
        /// для полной выборки можно указать "" или NULL
        /// </param>
        /// <returns>Лист объектов сущности</returns>
        public IEnumerable<T> GetList<T>(string sqlWhere) where T : class
        {
            var sqlAll = AttributesOfClass<T>.SimpleSqlSelect(GetCurrentProviderName()) + AttributesOfClass<T>.AddSqlWhere(sqlWhere);

            IEnumerable<T> lResul;
            var com = ProviderFactories.GetCommand(GetCurrentProviderName());
            com.Connection = _connect;
            com.CommandText = sqlAll;
            try
            {

                OpenConnectAndTransaction(com);
                lResul = AttributesOfClass<T>.GetEnumerableObjects(com.ExecuteReader(),GetCurrentProviderName());

            }
            catch (Exception ex)
            {
                Configure.SendError(Utils.GetStringSql(com), ex);
                return null;
            }
            finally
            {
                ComDisposable(com);
            }

            return lResul;
        }

        private int SaveNew<T>(T item) where T : class
        {
            var rez = 0;
            var com = ProviderFactories.GetCommand(GetCurrentProviderName());
            com.Connection = _connect;

            try
            {
                if (Utils.IsPersistent(item))
                {
                    NotificBefore(item, ActionMode.Update);
                    if (GetCurrentProviderName() == ProviderName.Postgresql || GetCurrentProviderName() == ProviderName.Sqlite)
                        AttributesOfClass<T>.CreateUpdateCommandPostgres(com, item, GetCurrentProviderName());
                    else
                        AttributesOfClass<T>.CreateUpdateCommandMysql(com, item, GetCurrentProviderName());


                    OpenConnectAndTransaction(com);
                    rez = com.ExecuteNonQuery();
                    NotificAfter(item, ActionMode.Update);
                }
                else
                {
                    NotificBefore(item, ActionMode.Insert);
                    AttributesOfClass<T>.CreateInsetCommand(com, item, GetCurrentProviderName());
                    OpenConnectAndTransaction(com);
                    var val = com.ExecuteScalar();
                    if (val != null)
                    {
                        AttributesOfClass<T>.RedefiningPrimaryKey(item, val);
                        rez = 1;
                    }
                    Utils.SetPersisten(item);
                    NotificAfter(item, ActionMode.Insert);
                }
            }
            catch (Exception ex)
            {
                Configure.SendError(Utils.GetStringSql(com), ex);
            }
            finally
            {
                ComDisposable(com);
            }

            return rez;
        }

       
        private T GetReal<T>(object id) where T : class
        {
            var sqlAll = string.Format("{0} WHERE {1}.{2}='{3}'", AttributesOfClass<T>.SimpleSqlSelect(GetCurrentProviderName()),
                AttributesOfClass<T>.TableName, AttributesOfClass<T>.PkAttribute.GetColumnName(GetCurrentProviderName()), id);
            var com = ProviderFactories.GetCommand(GetCurrentProviderName());
            com.Connection = _connect;
            com.CommandText = sqlAll;
            try
            {
                OpenConnectAndTransaction(com);
                var res = AttributesOfClass<T>.GetEnumerableObjects(com.ExecuteReader(),GetCurrentProviderName());

                var enumerable = res as T[] ?? res.ToArray();
                return enumerable.Any() ? enumerable.First() : null;
            }
            catch (Exception ex)
            {
                Configure.SendError(Utils.GetStringSql(com), ex);

                return null;
            }
            finally
            {
                ComDisposable(com);
                //GetCache<T>(false, com);
            }
        }

        internal void ComDisposable(IDbCommand com)
        {
            try
            {
                WriteLogFile(com);
            }
            finally
            {
                if (Transactionale.IsOccupied == false)
                {
                    com.Connection.Close();

                    com.Dispose();
                }
                else
                {
                    _dbCommands.Add(com);
                }
            }
        }

        /// <summary>
        ///Возвращает лист объектов  табличной сущности
        /// </summary>
        /// <typeparam name="T">Тип класса сущности</typeparam>
        /// <returns>Лист объектов сущности</returns>
        public IEnumerable<T> GetList<T>() where T : class
        {
            return GetList<T>("");
        }



        /// <summary>
        ///Возвращает название поля для таблицы
        /// </summary>
        /// <param name="property"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string ColumnName<T>(Expression<Func<T, object>> property)
        {
            LambdaExpression lambda = property;
            MemberExpression memberExpression;

            var expression = lambda.Body as UnaryExpression;
            if (expression != null)
            {
                UnaryExpression unaryExpression = expression;
                memberExpression = (MemberExpression)(unaryExpression.Operand);
            }
            else
            {
                memberExpression = (MemberExpression)(lambda.Body);
            }
            string name = ((PropertyInfo)memberExpression.Member).Name;
            foreach (var dal in AttributesOfClass<T>.CurrentTableAttributeDall)
            {
                if (dal.PropertyName == name)
                {
                    return dal.GetColumnName(GetCurrentProviderName());
                }
            }
            if (AttributesOfClass<T>.PkAttribute.PropertyName == name)
            {
                return AttributesOfClass<T>.PkAttribute.GetColumnName(GetCurrentProviderName());
            }
            throw new Exception($"Не могу определить поле таблицы для типа {typeof(T)}");

        }

        /// <summary>
        /// Получает SQL строку Insert 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>string sql from insert</returns>
        public string GetSqlInsertCommand<T>(T t)
        {
            switch (GetCurrentProviderName())
            {
                case ProviderName.MsSql:
                    throw new Exception("не рализовано");
                case ProviderName.MySql:
                    throw new Exception("не рализовано");
                case ProviderName.Postgresql:
                    return new CommandNativePostgres(ProviderName.Postgresql).GetInsertSql(t);
                case ProviderName.Sqlite:
                    throw new Exception("не рализовано");
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        /// <summary>
        /// Строка запроса на удаление
        /// </summary>
        /// <param name="t"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>string sql for detete</returns>
        public string GetSqlDeleteCommand<T>(T t)
        {
            switch (GetCurrentProviderName())
            {
                case ProviderName.MsSql:
                    throw new Exception("не рализовано");
                case ProviderName.MySql:
                    throw new Exception("не рализовано");
                case ProviderName.Postgresql:
                    return new CommandNativePostgres(ProviderName.Postgresql).GetDeleteSql(t);
                case ProviderName.Sqlite:
                    throw new Exception("не рализовано");
                default:
                    throw new ArgumentOutOfRangeException();
            }
            // 
        }
        /// <summary>
        /// Get DataTable
        /// </summary>
        /// <param name="sql">Строка запроса</param>
        /// <param name="timeOut">Время ожидания выполнения команды (30 сек)</param>
        /// <param name="param">Список параметров в той последовательности в которой они расположены в запросе.</param>
        /// <returns>DataTable</returns>
        /// <exception cref="NotImplementedException"></exception>
        public DataTable GetDataTable(string sql, int timeOut = 30, params object[] param)
        {
            var table = new DataTable();

            var com = ProviderFactories.GetCommand(GetCurrentProviderName());
            com.Connection = _connect;
            SetTimeOut(com, timeOut);

            com.CommandText = sql;


            try
            {
                AddParam(com, GetCurrentProviderName(), param);
                OpenConnectAndTransaction(com);
                var reader = com.ExecuteReader();
                table.BeginLoadData();
                table.Load(reader);
                table.EndLoadData();
                WriteLogFile(com.CommandText);
                return table;
            }
            catch (Exception ex)
            {
                Configure.SendError(com.CommandText, ex);
                return null;
            }
            finally
            {
                ComDisposable(com);
            }
        }

        /// <summary>
        /// Клонирование объекта
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ob"></param>
        /// <returns>T</returns>
        /// <exception cref="NotImplementedException"></exception>
        public T Clone<T>(T ob)
        {
            try
            {
                var str = JsonConvert.SerializeObject(ob);
                return JsonConvert.DeserializeObject<T>(str);

            }
            catch (Exception ex)
            {
                Configure.SendError("Clone", ex);
                throw;

            }
        }

        /// <summary>
        /// Получение sql для запроса  вставки пакетом
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public string GetSqlForInsertBulk<T>(IEnumerable<T> list)
        {
            switch (GetCurrentProviderName())
            {
                case ProviderName.MsSql:
                    return new UtilsBulkMsSql(ProviderName.MsSql).GetSql(list);
                case ProviderName.MySql:
                    return new UtilsBulkMySql(ProviderName.MySql).GetSql(list);
                case ProviderName.Postgresql:
                    return new UtilsBulkPostgres(ProviderName.Postgresql).GetSql(list);
                case ProviderName.Sqlite:
                    return new UtilsBulkMySql(ProviderName.Sqlite).GetSql(list);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Получение перечисления из чужой базы
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="factory">Объект IOtherBaseCommandFactory</param>
        /// <param name="sql">Строка запроса</param>
        /// <param name="param">Список параметров в той последовательности в которой они расположены в запросе.</param>
        /// <returns></returns>
        public IEnumerable<T> GetListOtherBase<T>(IOtherDataBaseFactory factory, string sql, params object[] param)
        {

           
            var com = factory.GetDbCommand();
            com.CommandText += sql;
            AddParam(com, GetCurrentProviderName(), param);
           
            IEnumerable<T> res;
            try
            {
                com.Connection.Open();
                res = AttributesOfClass<T>.GetEnumerableObjects(com.ExecuteReader(),GetCurrentProviderName());
            }
            catch (Exception ex)
            {
                Configure.SendError(Utils.GetStringSql(com), ex);
                return null;
            }
            finally
            {
                com.Connection.Close();
                com.Dispose();
                WriteLogFile(com);
                
            }
            return res;
        }

        /// <summary>
        /// Выполнение ExecuteScalar  к чужой базе
        /// </summary>
        /// <param name="factory">Объект IOtherBaseCommandFactory</param>
        /// <param name="sql">Строка запроса</param>
        /// <param name="param">Список параметров в той последовательности в которой они расположены в запросе.</param>
        /// <returns></returns>
        public object GetObjectOtherBase(IOtherDataBaseFactory factory, string sql, params object[] param)
        {
            var com = factory.GetDbCommand();
            com.CommandText += sql;
            AddParam(com, GetCurrentProviderName(), param);
            try
            { 
                com.Connection.Open();
               return  com.ExecuteScalar();
            }
            catch (Exception ex)
            {
                Configure.SendError(Utils.GetStringSql(com), ex);
                return null;
            }
            finally
            {
                com.Connection.Close();
                com.Dispose();
                WriteLogFile(com);
            }
           
        }
        /// <summary>
        /// Выполнение запроса ExecuteNonQuery к чужой базе
        /// </summary>
        /// <param name="factory">Объект IOtherBaseCommandFactory</param>
        /// <param name="sql">Строка запроса</param>
        /// <param name="param">Список параметров в той последовательности в которой они расположены в запросе.</param>
        /// <returns></returns>
        public int ExecuteNonQueryOtherBase(IOtherDataBaseFactory factory, string sql, params object[] param)
        {
            var com = factory.GetDbCommand();
            com.CommandText += sql;
            AddParam(com, GetCurrentProviderName(), param);
            try
            {
                com.Connection.Open();
                return com.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Configure.SendError(Utils.GetStringSql(com), ex);
                return 0;
            }
            finally
            {
                com.Connection.Close();
                com.Dispose();
                WriteLogFile(com);
            }
        }
    }
}