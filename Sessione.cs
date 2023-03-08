
using ORM_1_21_.Linq;
using ORM_1_21_.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ORM_1_21_.Transaction;

namespace ORM_1_21_
{
    ///<summary>
    ///</summary>
    public sealed partial class Sessione : ISession, IServiceSessions
    {
        readonly List<IDbCommand> _dbCommands = new List<IDbCommand>();

        IDbCommand IServiceSessions.CommandForLinq
        {
            get
            {
                var com = ProviderFactories.GetCommand(_factory, ((ISession)this).IsDispose);
                com.Connection = _connect;
                return com;
            }
        }

        ProviderName IServiceSessions.CurrentProviderName => MyProviderName;

        object IServiceSessions.Locker { get; } = new object();

        int ISession.Delete<T>(T item)
        {
            if (!UtilsCore.IsPersistent(item)) return 0;
            var com = ProviderFactories.GetCommand(_factory, ((ISession)this).IsDispose);
            com.Connection = _connect;
            AttributesOfClass<T>.CreateDeleteCommand(com, item, MyProviderName);
            try
            {
                NotificBefore(item, ActionMode.Delete);
                OpenConnectAndTransaction(com);
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
                NotificAfter(item, ActionMode.Delete);
            }
        }

        IEnumerable<T> ISession.GetListMonster<T>(IDataReader reader)
        {
            return AttributesOfClass<T>.GetEnumerableObjects(reader, MyProviderName);
        }
       
        int ISession.Save<T>(T item)
        {
            if (item == null) throw new ArgumentException("The object to save is Null");
            return SaveNew(item);
        }
       
        int ISession.TableCreate<T>()
        {
            var ss = new FactoryCreatorTable().SqlCreate<T>(MyProviderName);
            var com = ProviderFactories.GetCommand(_factory, ((ISession)this).IsDispose);
            com.Connection = _connect;

            com.CommandText = ss;
            try
            {
                OpenConnectAndTransaction(com);
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

        IDbCommand ISession.GeDbCommand()
        {
            if (_factory != null)
            {
                return _factory.GetDbProviderFactories().CreateCommand();
            }
            return ProviderFactories.GetCommand(_factory, ((ISession)this).IsDispose);
        }

        int ISession.DropTable<T>()
        {
            var com = ProviderFactories.GetCommand(_factory, ((ISession)this).IsDispose);
            com.Connection = _connect;

            com.CommandText = $"DROP TABLE {AttributesOfClass<T>.TableName(MyProviderName)}";
            try
            {
                OpenConnectAndTransaction(com);
                return com.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                MySqlLogger.Error(UtilsCore.GetStringSql(com), ex);
                throw;
            }
            finally
            {
                InnerWriteLogFile($"DropTable: {com.CommandText}");
                ComDisposable(com);
            }
        }

        bool ISession.TableExists<T>()
        {
            var com = ProviderFactories.GetCommand(_factory, ((ISession)this).IsDispose);
            try
            {
                if (MyProviderName == ProviderName.Postgresql)
                {
                    com.Connection = _connect;
                    var tableName = UtilsCore.ClearTrim(AttributesOfClass<T>.TableName(MyProviderName));
                    com.CommandText =
                        $"SELECT count(*) FROM pg_tables WHERE   tablename  = '{tableName}';";

                    OpenConnectAndTransaction(com);
                    long res = (long)com.ExecuteScalar();
                    return res != 0;
                }
                else
                {
                    com = ProviderFactories.GetCommand(_factory, ((ISession)this).IsDispose);
                    com.Connection = _connect;
                    com.CommandText = $"select 1 from {AttributesOfClass<T>.TableName(MyProviderName)};";
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

        IDataReader ISession.ExecuteReader(string sql, object[] @params)
        {
            if (sql == null) throw new ArgumentNullException(nameof(sql));
            var com = ProviderFactories.GetCommand(_factory, ((ISession)this).IsDispose);
            com.Connection = _connect;

            com.CommandText = sql;
            UtilsCore.AddParam(com, MyProviderName, @params);
            OpenConnectAndTransaction(com);
            return com.ExecuteReader();
        }

        private void SetTimeOut(IDbCommand com, int timeOut)
        {
            if (timeOut > 0)
            {
                com.CommandTimeout = timeOut;
            }
        }

        IDataReader ISession.ExecuteReaderT(string sql, int timeOut, params object[] param)
        {
            if (sql == null) throw new ArgumentNullException(nameof(sql));
            var com = ProviderFactories.GetCommand(_factory, ((ISession)this).IsDispose);
            com.Connection = _connect;
            com.CommandText = sql;
            SetTimeOut(com, timeOut);

            UtilsCore.AddParam(com, MyProviderName, param);
            OpenConnectAndTransaction(com);
            return com.ExecuteReader();
        }

        DataTable ISession.GetDataTable(string sql, int timeOut)
        {
            if (sql == null) throw new ArgumentNullException(nameof(sql));
            var table = new DataTable();

            var com = ProviderFactories.GetCommand(_factory, ((ISession)this).IsDispose);
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
                InnerWriteLogFile($"GetDataTable: {com.CommandText}");
                return table;
            }
            catch (Exception ex)
            {
                MySqlLogger.Error(com.CommandText, ex);
                throw;
            }
            finally
            {
                ComDisposable(com);
            }
        }

        List<string> ISession.GetTableNames()
        {

            var com = ProviderFactories.GetCommand(_factory, ((ISession)this).IsDispose);

            int index;

            switch (MyProviderName)
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
                MySqlLogger.Error(UtilsCore.GetStringSql(com), ex);
                throw;
            }
            finally
            {
                ComDisposable(com);
            }

            return result;
        }

        int ISession.CreateBase(string baseName)
        {
            var com = ProviderFactories.GetCommand(_factory, ((ISession)this).IsDispose);
            com.Connection = _connect;


            try
            {
                switch (MyProviderName)
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
                MySqlLogger.Error(com.CommandText, ex);
                throw;
              
            }
            finally
            {
                ComDisposable(com);
            }
        }

        int ISession.InsertBulk<T>(IEnumerable<T> list, int timeOut)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            var com = ProviderFactories.GetCommand(_factory, ((ISession)this).IsDispose);
            com.Connection = _connect;
            switch (MyProviderName)
            {
                case ProviderName.MsSql:
                    com.CommandText = new UtilsBulkMsSql(ProviderName.MsSql).GetSql(list);
                    break;
                case ProviderName.MySql:
                    com.CommandText = new UtilsBulkMySql(ProviderName.MySql).GetSql(list);
                    break;
                case ProviderName.Postgresql:
                    com.CommandText = new UtilsBulkPostgres(ProviderName.Postgresql).GetSql(list);
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

      

        int ISession.InsertBulkFromFile<T>(string fileCsv, string fieldterminator, int timeOut)
        {
            if (fileCsv == null) throw new ArgumentNullException(nameof(fileCsv));
            var com = ProviderFactories.GetCommand(_factory, ((ISession)this).IsDispose);
            com.Connection = _connect;
            SetTimeOut(com, timeOut);
            switch (MyProviderName)
            {
                case ProviderName.MsSql:
                    com.CommandText = UtilsBulkMsSql.InsertFile<T>(fileCsv, fieldterminator, MyProviderName);
                    break;
                case ProviderName.MySql:
                    com.CommandText = UtilsBulkMySql.InsertFile<T>(fileCsv, fieldterminator, MyProviderName);
                    break;
                case ProviderName.Postgresql:
                    com.CommandText = UtilsBulkPostgres.InsertFile<T>(fileCsv, fieldterminator, MyProviderName);
                    break;
                case ProviderName.Sqlite:
                    com.CommandText = UtilsBulkMySql.InsertFile<T>(fileCsv, fieldterminator, MyProviderName);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            try
            {
                OpenConnectAndTransaction(com);
                com.CommandTimeout = 30;
                return com.ExecuteNonQuery();
            }
            catch (Exception ex)
            {

                MySqlLogger.Error(UtilsCore.GetStringSql(com), ex);
                return -100;
            }
            finally
            {
                ComDisposable(com);
            }
        }

        object ISession.ExecuteScalar(string sql, params object[] param)
        {
            if (sql == null) throw new ArgumentNullException(nameof(sql));
            var com = ProviderFactories.GetCommand(_factory, ((ISession)this).IsDispose);
            com.Connection = _connect;
            com.CommandType = CommandType.Text;
            com.CommandText = sql;
            UtilsCore.AddParam(com, MyProviderName, param);

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

        object ISession.ExecuteScalarT(string sql, int timeOut, params object[] param)
        {
            if (sql == null) throw new ArgumentNullException(nameof(sql));
            var com = ProviderFactories.GetCommand(_factory, ((ISession)this).IsDispose);
            com.Connection = _connect;
            com.CommandType = CommandType.Text;
            com.CommandText = sql;
            UtilsCore.AddParam(com, MyProviderName, param);
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

        int ISession.TruncateTable<T>()
        {
            var com = ProviderFactories.GetCommand(_factory, ((ISession)this).IsDispose);
            com.Connection = _connect;
            com.CommandType = CommandType.Text;
            if (MyProviderName == ProviderName.Sqlite)
                com.CommandText = $"DELETE FROM {AttributesOfClass<T>.TableName(MyProviderName)};";
            else
                com.CommandText = $"TRUNCATE TABLE {AttributesOfClass<T>.TableName(MyProviderName)};";

            try
            {
                OpenConnectAndTransaction(com);
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

        Query<T> ISession.Query<T>()
        {
            //AttributesOfClass<T>.CurProvider = _factory != null ? _factory.GetProviderName() : Configure.Provider;
            QueryProvider p = new DbQueryProvider<T>(this);
            return new Query<T>(p);
        }

        IDbCommand ISession.GetCommand()
        {
            var com = ProviderFactories.GetCommand(_factory, ((ISession)this).IsDispose);
            com.Connection = _connect;
            return com;
        }

        IDbConnection ISession.GetConnection()
        {
            return ProviderFactories.GetConnect(_factory);
        }

        IDbDataAdapter ISession.GetDataAdapter()
        {
            return ProviderFactories.GetDataAdapter(_factory);
        }

        string ISession.GetConnectionString()
        {
            return _connect.ConnectionString;
        }

        int ISession.ExecuteNonQuery(string sql, params object[] param)
        {
            if (sql == null) throw new ArgumentNullException(nameof(sql));
            var com = ProviderFactories.GetCommand(_factory, ((ISession)this).IsDispose);
            com.Connection = _connect;
            com.CommandText = sql;
            UtilsCore.AddParam(com, MyProviderName, param);
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

        int ISession.ExecuteNonQueryT(string sql, int timeOut, params object[] param)
        {
            if (sql == null) throw new ArgumentNullException(nameof(sql));

            var com = ProviderFactories.GetCommand(_factory, ((ISession)this).IsDispose);
            com.Connection = _connect;
            com.CommandText = sql;
            UtilsCore.AddParam(com, MyProviderName, param);
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

        string ISession.TableName<T>()
        {
            return AttributesOfClass<T>.TableName(MyProviderName);
        }

        private int SaveNew<T>(T item) where T : class
        {
            var rez = 0;
            IDbCommand com = ProviderFactories.GetCommand(_factory, ((ISession)this).IsDispose);


            com.Connection = _connect;
            com.CommandText = String.Empty;

            try
            {
                if (UtilsCore.IsPersistent(item))
                {
                    NotificBefore(item, ActionMode.Update);
                    if (MyProviderName == ProviderName.Postgresql || MyProviderName == ProviderName.Sqlite)
                        AttributesOfClass<T>.CreateUpdateCommandPostgres(com, item, MyProviderName);
                    else
                        AttributesOfClass<T>.CreateUpdateCommandMysql(com, item, MyProviderName);


                    OpenConnectAndTransaction(com);
                    rez = com.ExecuteNonQuery();
                    NotificAfter(item, ActionMode.Update);
                }
                else
                {
                    NotificBefore(item, ActionMode.Insert);
                    AttributesOfClass<T>.CreateInsetCommand(com, item, MyProviderName);
                    OpenConnectAndTransaction(com);
                    var val = com.ExecuteScalar();
                    if (val != null)
                    {
                        AttributesOfClass<T>.RedefiningPrimaryKey(item, val, MyProviderName);
                        rez = 1;
                    }
                    UtilsCore.SetPersistent(item);
                    NotificAfter(item, ActionMode.Insert);
                }
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

            return rez;
        }

        private T GetReal<T>(object id) where T : class
        {
            var sqlAll = string.Format("{0} WHERE {1}.{2} = '{3}'", AttributesOfClass<T>.SimpleSqlSelect(MyProviderName),
                AttributesOfClass<T>.TableName(MyProviderName), AttributesOfClass<T>.PkAttribute(MyProviderName).GetColumnName(MyProviderName), id);
            var com = ProviderFactories.GetCommand(_factory, ((ISession)this).IsDispose);
            com.Connection = _connect;
            com.CommandText = sqlAll;
            try
            {
                OpenConnectAndTransaction(com);
                var res = AttributesOfClass<T>.GetEnumerableObjects(com.ExecuteReader(), MyProviderName);

                var enumerable = res as T[] ?? res.ToArray();
                return enumerable.Any() ? enumerable.First() : null;
            }
            catch (Exception ex)
            {
                MySqlLogger.Error(UtilsCore.GetStringSql(com), ex);
                throw;

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
                InnerWriteLogFile(com);
            }
            finally
            {
                if (Transactionale.MyStateTransaction == StateTransaction.None||
                    Transactionale.MyStateTransaction == StateTransaction.Commit||
                    Transactionale.MyStateTransaction == StateTransaction.Rollback)
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

        string ISession.ColumnName<T>(Expression<Func<T, object>> property)
        {
            LambdaExpression lambda = property;
            MemberExpression memberExpression;

            if (lambda.Body is UnaryExpression expression)
            {
                UnaryExpression unaryExpression = expression;
                memberExpression = (MemberExpression)(unaryExpression.Operand);
            }
            else
            {
                memberExpression = (MemberExpression)(lambda.Body);
            }
            string name = ((PropertyInfo)memberExpression.Member).Name;
            foreach (var dal in AttributesOfClass<T>.CurrentTableAttributeDall(MyProviderName))
            {
                if (dal.PropertyName == name)
                {
                    return dal.GetColumnName(MyProviderName);
                }
            }
            if (AttributesOfClass<T>.PkAttribute(MyProviderName).PropertyName == name)
            {
                return AttributesOfClass<T>.PkAttribute(MyProviderName).GetColumnName(MyProviderName);
            }
            throw new Exception($"Не могу определить поле таблицы для типа {typeof(T)}");

        }

        string ISession.GetSqlInsertCommand<T>(T t)
        {
            if (t == null) throw new ArgumentNullException(nameof(t));
            switch (MyProviderName)
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

        string ISession.GetSqlDeleteCommand<T>(T t)
        {
            if (t == null) throw new ArgumentNullException(nameof(t));
            switch (MyProviderName)
            {
                case ProviderName.MsSql:
                    throw new Exception("not implemented");
                case ProviderName.MySql:
                    throw new Exception("not implemented");
                case ProviderName.Postgresql:
                    return new CommandNativePostgres(ProviderName.Postgresql).GetDeleteSql(t);
                case ProviderName.Sqlite:
                    throw new Exception("not implemented");
                default:
                    throw new ArgumentOutOfRangeException();
            }
            // 
        }

        DataTable ISession.GetDataTable(string sql, int timeOut, params object[] param)
        {
            if (sql == null) throw new ArgumentNullException(nameof(sql));
            if (string.IsNullOrEmpty(sql))
            {
                throw new ArgumentException($@"""{nameof(sql)}"" cannot be null or empty.", nameof(sql));
            }

            if (param is null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            var table = new DataTable();

            var com = ProviderFactories.GetCommand(_factory, ((ISession)this).IsDispose);
            com.Connection = _connect;
            SetTimeOut(com, timeOut);

            com.CommandText = sql;


            try
            {
                UtilsCore.AddParam(com, MyProviderName, param);
                OpenConnectAndTransaction(com);
                var reader = com.ExecuteReader();
                table.BeginLoadData();
                table.Load(reader);
                table.EndLoadData();
                InnerWriteLogFile($"GetDataTable: {com.CommandText}");
                return table;
            }
            catch (Exception ex)
            {
                MySqlLogger.Error(com.CommandText, ex);
                throw;
            }
            finally
            {
                ComDisposable(com);
            }
        }

        T ISession.Clone<T>(T ob)
        {
            if (ob == null) throw new ArgumentNullException(nameof(ob));
            try
            {

               return UtilsCore.Clone(ob);

            }
            catch (Exception ex)
            {
                MySqlLogger.Error("Clone", ex);
                throw;

            }
        }

        string ISession.GetSqlForInsertBulk<T>(IEnumerable<T> list)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            switch (MyProviderName)
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


    }
}