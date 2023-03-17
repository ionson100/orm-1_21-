using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ORM_1_21_.Linq;
using ORM_1_21_.Transaction;
using ORM_1_21_.Utils;

namespace ORM_1_21_
{
    ///<summary>
    ///</summary>
    internal sealed partial class Sessione : ISession, IServiceSessions
    {
        private readonly List<IDbCommand> _dbCommands = new List<IDbCommand>();
        private ISession _sessionImplementation;

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

        int ISession.Delete<TSource>(TSource source)
        {
            Check.NotNull(source, "source");
            if (!UtilsCore.IsPersistent(source))
                throw new Exception("You are trying to delete an object not obtained from database");
            var com = ProviderFactories.GetCommand(_factory, ((ISession)this).IsDispose);
            com.Connection = _connect;
            AttributesOfClass<TSource>.CreateDeleteCommand(com, source, MyProviderName);
            try
            {
                NotificBefore(source, ActionMode.Delete);
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
                NotificAfter(source, ActionMode.Delete);
            }
        }

        IEnumerable<TSource> ISession.GetListMonster<TSource>(IDataReader reader)
        {
            return AttributesOfClass<TSource>.GetEnumerableObjects(reader, MyProviderName);
        }

        int ISession.Save<TSource>(TSource source)
        {
            Check.NotNull(source, "source");
            return SaveNew(source);
        }

        int ISession.TableCreate<TSource>()
        {
            var ss = new FactoryCreatorTable().SqlCreate<TSource>(MyProviderName);
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
            if (_factory != null) return _factory.GetDbProviderFactories().CreateCommand();
            return ProviderFactories.GetCommand(_factory, ((ISession)this).IsDispose);
        }

        int ISession.DropTable<TSource>()
        {
            var com = ProviderFactories.GetCommand(_factory, ((ISession)this).IsDispose);
            com.Connection = _connect;

            com.CommandText = $"DROP TABLE {AttributesOfClass<TSource>.TableName(MyProviderName)}";
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

        bool ISession.TableExists<TSource>()
        {
            var com = ProviderFactories.GetCommand(_factory, ((ISession)this).IsDispose);
            try
            {
                if (MyProviderName == ProviderName.Postgresql)
                {
                    com.Connection = _connect;
                    var tableName = UtilsCore.ClearTrim(AttributesOfClass<TSource>.TableName(MyProviderName));
                    com.CommandText =
                        $"SELECT count(*) FROM pg_tables WHERE   tablename  = '{tableName}';";

                    OpenConnectAndTransaction(com);
                    var res = (long)com.ExecuteScalar();
                    return res != 0;
                }
                else
                {
                    com = ProviderFactories.GetCommand(_factory, ((ISession)this).IsDispose);
                    com.Connection = _connect;
                    com.CommandText = $"select 1 from {AttributesOfClass<TSource>.TableName(MyProviderName)};";
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
            Check.NotEmpty(sql, "sql");
            var com = ProviderFactories.GetCommand(_factory, ((ISession)this).IsDispose);
            com.Connection = _connect;

            com.CommandText = sql;
            UtilsCore.AddParam(com, MyProviderName, @params);
            OpenConnectAndTransaction(com);
            return com.ExecuteReader();
        }

        IDataReader ISession.ExecuteReaderT(string sql, int timeOut, params object[] param)
        {
            Check.NotEmpty(sql, "sql");
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
            Check.NotEmpty(sql, "sql");
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
            Check.NotEmpty(baseName, "baseName");
            var com = ProviderFactories.GetCommand(_factory, ((ISession)this).IsDispose);
            com.Connection = _connect;


            try
            {
                switch (MyProviderName)
                {
                    case ProviderName.MsSql:
                        if (File.Exists(baseName))
                            return 0;
                        var bName = Path.GetFileName(baseName).Substring(0, Path.GetFileName(baseName).IndexOf('.'));
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

        int ISession.InsertBulk<TSource>(IEnumerable<TSource> list, int timeOut)
        {
            var enumerable = list as TSource[] ?? list.ToArray();
            Check.NotNull(enumerable, "list");

            Check.NotNull(enumerable, "list");
            var com = ProviderFactories.GetCommand(_factory, ((ISession)this).IsDispose);
            com.Connection = _connect;
            switch (MyProviderName)
            {
                case ProviderName.MsSql:
                    com.CommandText = new UtilsBulkMsSql(ProviderName.MsSql).GetSql(enumerable);
                    break;
                case ProviderName.MySql:
                    com.CommandText = new UtilsBulkMySql(ProviderName.MySql).GetSql(enumerable);
                    break;
                case ProviderName.Postgresql:
                    com.CommandText = new UtilsBulkPostgres(ProviderName.Postgresql).GetSql(enumerable);
                    break;
                case ProviderName.Sqlite:
                    com.CommandText = new UtilsBulkMySql(ProviderName.Sqlite).GetSql(enumerable);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            try
            {
                OpenConnectAndTransaction(com);
                com.CommandTimeout = 30000;
                SetTimeOut(com, timeOut);
                var res = com.ExecuteNonQuery();
                foreach (var iSource in enumerable) ((ISession)this).ToPersistent(iSource);
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


        int ISession.InsertBulkFromFile<TSource>(string fileCsv, string fieldterminator, int timeOut)
        {
            Check.NotEmpty(fileCsv, "fileCsv");
            if (fileCsv == null) throw new ArgumentNullException(nameof(fileCsv));
            var com = ProviderFactories.GetCommand(_factory, ((ISession)this).IsDispose);
            com.Connection = _connect;
            SetTimeOut(com, timeOut);
            switch (MyProviderName)
            {
                case ProviderName.MsSql:
                    com.CommandText = UtilsBulkMsSql.InsertFile<TSource>(fileCsv, fieldterminator, MyProviderName);
                    break;
                case ProviderName.MySql:
                    com.CommandText = UtilsBulkMySql.InsertFile<TSource>(fileCsv, fieldterminator, MyProviderName);
                    break;
                case ProviderName.Postgresql:
                    com.CommandText = UtilsBulkPostgres.InsertFile<TSource>(fileCsv, fieldterminator, MyProviderName);
                    break;
                case ProviderName.Sqlite:
                    com.CommandText = UtilsBulkMySql.InsertFile<TSource>(fileCsv, fieldterminator, MyProviderName);
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
            Check.NotEmpty(sql, "sql");
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
            Check.NotEmpty(sql, "sql");
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

        int ISession.TruncateTable<TSource>()
        {
            var com = ProviderFactories.GetCommand(_factory, ((ISession)this).IsDispose);
            com.Connection = _connect;
            com.CommandType = CommandType.Text;
            if (MyProviderName == ProviderName.Sqlite)
                com.CommandText = $"DELETE FROM {AttributesOfClass<TSource>.TableName(MyProviderName)};";
            else
                com.CommandText = $"TRUNCATE TABLE {AttributesOfClass<TSource>.TableName(MyProviderName)};";

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

        Query<TSource> ISession.Query<TSource>()
        {
            QueryProvider p = new DbQueryProvider<TSource>(this);
            return new Query<TSource>(p);
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
            Check.NotEmpty(sql, "sql");
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
            Check.NotEmpty(sql, "sql");

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

        string ISession.TableName<TSource>()
        {
            return AttributesOfClass<TSource>.TableName(MyProviderName);
        }

        string ISession.ColumnName<TSource>(Expression<Func<TSource, object>> property)
        {
            Check.NotNull(property, "property");
            LambdaExpression lambda = property;
            MemberExpression memberExpression;

            if (lambda.Body is UnaryExpression expression)
            {
                var unaryExpression = expression;
                memberExpression = (MemberExpression)unaryExpression.Operand;
            }
            else
            {
                memberExpression = (MemberExpression)lambda.Body;
            }

            var name = ((PropertyInfo)memberExpression.Member).Name;
            foreach (var dal in AttributesOfClass<TSource>.CurrentTableAttributeDall(MyProviderName))
                if (dal.PropertyName == name)
                    return dal.GetColumnName(MyProviderName);
            if (AttributesOfClass<TSource>.PkAttribute(MyProviderName).PropertyName == name)
                return AttributesOfClass<TSource>.PkAttribute(MyProviderName).GetColumnName(MyProviderName);
            throw new Exception($"Can't determine table field for type: {typeof(TSource)}");
        }

        string ISession.GetSqlInsertCommand<TSource>(TSource source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            switch (MyProviderName)
            {
                case ProviderName.MsSql:
                    throw new Exception("не рализовано");
                case ProviderName.MySql:
                    throw new Exception("не рализовано");
                case ProviderName.Postgresql:
                    return new CommandNativePostgres(ProviderName.Postgresql).GetInsertSql(source);
                case ProviderName.Sqlite:
                    throw new Exception("не рализовано");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        string ISession.GetSqlDeleteCommand<TSource>(TSource source)
        {
            Check.NotNull(source, "source");
            switch (MyProviderName)
            {
                case ProviderName.MsSql:
                    throw new Exception("not implemented");
                case ProviderName.MySql:
                    throw new Exception("not implemented");
                case ProviderName.Postgresql:
                    return new CommandNativePostgres(ProviderName.Postgresql).GetDeleteSql(source);
                case ProviderName.Sqlite:
                    throw new Exception("not implemented");
                default:
                    throw new ArgumentOutOfRangeException();
            }
            // 
        }

        DataTable ISession.GetDataTable(string sql, int timeOut, params object[] param)
        {
            Check.NotEmpty(sql, "sql");
            if (sql == null) throw new ArgumentNullException(nameof(sql));
            if (string.IsNullOrEmpty(sql))
                throw new ArgumentException($@"""{nameof(sql)}"" cannot be null or empty.", nameof(sql));

            if (param is null) throw new ArgumentNullException(nameof(param));

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

        TSource ISession.Clone<TSource>(TSource source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            try
            {
                return UtilsCore.Clone(source);
            }
            catch (Exception ex)
            {
                MySqlLogger.Error("Clone", ex);
                throw;
            }
        }

        string ISession.GetSqlForInsertBulk<TSource>(IEnumerable<TSource> list)
        {
            var enumerable = list as TSource[] ?? list.ToArray();
            Check.NotNull(enumerable, "list");
            if (list == null) throw new ArgumentNullException(nameof(list));
            switch (MyProviderName)
            {
                case ProviderName.MsSql:
                    return new UtilsBulkMsSql(ProviderName.MsSql).GetSql(enumerable);
                case ProviderName.MySql:
                    return new UtilsBulkMySql(ProviderName.MySql).GetSql(enumerable);
                case ProviderName.Postgresql:
                    return new UtilsBulkPostgres(ProviderName.Postgresql).GetSql(enumerable);
                case ProviderName.Sqlite:
                    return new UtilsBulkMySql(ProviderName.Sqlite).GetSql(enumerable);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public int Update<TSource>(TSource source, params AppenderWhere[] whereObjects) where TSource : class, new()
        {
            Check.NotNull(source, "source");
            if (!UtilsCore.IsPersistent(source))
                throw new Exception("You are trying to update an object not obtained from database");
            return SaveNew(source, whereObjects);
        }

        private void SetTimeOut(IDbCommand com, int timeOut)
        {
            Check.NotNull(com, "com");
            if (timeOut > 0) com.CommandTimeout = timeOut;
        }

        private int SaveNew<TSource>(TSource source, params AppenderWhere[] whereObjects) where TSource : class, new()
        {
            var res = 0;
            var com = ProviderFactories.GetCommand(_factory, ((ISession)this).IsDispose);


            com.Connection = _connect;
            com.CommandText = string.Empty;

            try
            {
                if (UtilsCore.IsPersistent(source))
                {
                    NotificBefore(source, ActionMode.Update);
                    if (MyProviderName == ProviderName.Postgresql || MyProviderName == ProviderName.Sqlite)
                        AttributesOfClass<TSource>.CreateUpdateCommandPostgres(com, source, MyProviderName,
                            whereObjects);
                    else
                        AttributesOfClass<TSource>.CreateUpdateCommandMysql(com, source, MyProviderName, whereObjects);


                    OpenConnectAndTransaction(com);
                    res = com.ExecuteNonQuery();
                    if (res == 1) NotificAfter(source, ActionMode.Update);
                }
                else
                {
                    NotificBefore(source, ActionMode.Insert);
                    AttributesOfClass<TSource>.CreateInsetCommand(com, source, MyProviderName);
                    OpenConnectAndTransaction(com);
                    if (AttributesOfClass<TSource>.PkAttribute(MyProviderName).Generator == Generator.Assigned)
                    {
                        var val = com.ExecuteNonQuery();
                        if (val == 1)
                        {
                            ((ISession)this).ToPersistent(source);
                            res = 1;
                            NotificAfter(source, ActionMode.Insert);
                        }
                    }
                    else
                    {
                        var val = com.ExecuteScalar();
                        if (val != null)
                        {
                            AttributesOfClass<TSource>.RedefiningPrimaryKey(source, val, MyProviderName);
                            ((ISession)this).ToPersistent(source);
                            var tet = ((ISession)this).IsPersistent(source);
                            res = 1;
                            NotificAfter(source, ActionMode.Insert);
                        }
                    }
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

            return res;
        }


        internal void ComDisposable(IDbCommand com)
        {
            Check.NotNull(com, "com");
            try
            {
                InnerWriteLogFile(com);
            }
            finally
            {
                if (Transactionale.MyStateTransaction == StateTransaction.None ||
                    Transactionale.MyStateTransaction == StateTransaction.Commit ||
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
    }
}