using ORM_1_21_.Extensions;
using ORM_1_21_.Linq;
using ORM_1_21_.Transaction;
using ORM_1_21_.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ORM_1_21_
{
    ///<summary>
    ///</summary>
    internal sealed partial class Sessione : ISession, IServiceSessions
    {
        private readonly List<IDbCommand> _dbCommands = new List<IDbCommand>();

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
            Check.NotNull(source, "source", () => Transactionale.isError = true);
            if (!UtilsCore.IsPersistent(source))
                throw new Exception("You are trying to delete an object not obtained from database");
            var com = ProviderFactories.GetCommand(_factory, ((ISession)this).IsDispose);
            com.Connection = _connect;
            AttributesOfClass<TSource>.CreateDeleteCommand(com, source, MyProviderName);
            try
            {
                NotificBefore(source, ActionMode.Delete);
                OpenConnectAndTransaction(com);
                var res = com.ExecuteNonQuery();
                if (res == 1)
                {
                    this.CacheClear<TSource>();
                    NotificAfter(source, ActionMode.Delete);
                }

                return res;

            }
            catch (Exception ex)
            {
                Transactionale.isError = true;
                MySqlLogger.Error(UtilsCore.GetStringSql(com), ex);
                throw;
            }
            finally
            {
                ComDisposable(com);
            }
        }

        IEnumerable<TSource> ISession.GetListMonster<TSource>(IDataReader reader)
        {
            return AttributesOfClass<TSource>.GetEnumerableObjects(reader, MyProviderName);
        }

        int ISession.Save<TSource>(TSource source)
        {
            Check.NotNull(source, "source", () => Transactionale.isError = true);
            return SaveNew(source);
        }

        Task<int> ISession.SaveAsync<TSource>(TSource source,CancellationToken cancellationToken)
        {
            Check.NotNull(source, "source", () => Transactionale.isError = true);
            return SaveNewAsync(source,null,cancellationToken);
        }

        int ISession.TableCreate<TSource>()
        {
            var ss = new FactoryCreatorTable().SqlCreate<TSource>(MyProviderName);
            var p = new V(ss);
            Expression callExpr = Expression.Call(
                Expression.Constant(p), p.GetType().GetMethod("TableCreate"));
            DbQueryProvider<TSource> provider = new DbQueryProvider<TSource>(this);
            return (int)provider.ExecuteExtension<int>(callExpr); ;
        }
        Task<int> ISession.TableCreateAsync<TSource>(CancellationToken cancellationToken)
        {
            var ss = new FactoryCreatorTable().SqlCreate<TSource>(MyProviderName);
            var p = new V(ss);
            Expression callExpr = Expression.Call(
                Expression.Constant(p), p.GetType().GetMethod("TableCreate"));
            DbQueryProvider<TSource> provider = new DbQueryProvider<TSource>(this);
            return provider.ExecuteExtensionAsync<int>(callExpr,null, cancellationToken); ;
        }

     

        IDbCommand ISession.GeDbCommand()
        {
            if (_factory != null) return _factory.GetDbProviderFactories().CreateCommand();
            return ProviderFactories.GetCommand(_factory, ((ISession)this).IsDispose);
        }

        int ISession.DropTable<TSource>()
        {
            var sql = $"DROP TABLE {AttributesOfClass<TSource>.TableName(MyProviderName)}";
            var p = new V(sql);
            Expression callExpr = Expression.Call(Expression.Constant(p), p.GetType().GetMethod("DropTable"));
            DbQueryProvider<TSource> provider = new DbQueryProvider<TSource>(this);
            return (int)provider.ExecuteExtension<int>(callExpr); ;

        }

        Task<int> ISession.DropTableAsync<TSource>(CancellationToken cancellationToken)
        {
            var sql = $"DROP TABLE {AttributesOfClass<TSource>.TableName(MyProviderName)}";
            var p = new V(sql);
            Expression callExpr = Expression.Call(Expression.Constant(p), p.GetType().GetMethod("DropTable"));
            DbQueryProvider<TSource> provider = new DbQueryProvider<TSource>(this);
            return provider.ExecuteExtensionAsync<int>(callExpr,null, cancellationToken);
        }

        int ISession.DropTableIfExists<TSource>()
        {
            var sql = $"DROP TABLE IF EXISTS {AttributesOfClass<TSource>.TableName(MyProviderName)}";
            var p = new V(sql);
            Expression callExpr = Expression.Call(Expression.Constant(p), p.GetType().GetMethod("DropTable"));
            DbQueryProvider<TSource> provider = new DbQueryProvider<TSource>(this);
            return (int)provider.ExecuteExtension<int>(callExpr); ;
        }

        Task<int> ISession.DropTableIfExistsAsync<TSource>(CancellationToken cancellationToken)
        {
            var sql = $"DROP TABLE IF EXISTS {AttributesOfClass<TSource>.TableName(MyProviderName)}";
            var p = new V(sql);
            Expression callExpr = Expression.Call(Expression.Constant(p), p.GetType().GetMethod("DropTable"));
            DbQueryProvider<TSource> provider = new DbQueryProvider<TSource>(this);
            return provider.ExecuteExtensionAsync<int>(callExpr,null, cancellationToken);
        }

        bool ISession.TableExists<TSource>()
        {
            string sql;
            try
            {
                switch (MyProviderName)
                {
                    case ProviderName.PostgreSql:
                    {
                        var tableName = UtilsCore.ClearTrim(AttributesOfClass<TSource>.TableName(MyProviderName));
                        sql = $"SELECT count(*) FROM pg_tables WHERE   tablename  = '{tableName}';";
                        break;
                    }

                    case ProviderName.MsSql:
                    {
                        string t = UtilsCore.ClearTrim(AttributesOfClass<TSource>.TableName(MyProviderName));
                        sql = $"SELECT OBJECT_ID('{t}', 'U');";
                        break;
                    }
                    case ProviderName.MySql:
                    case ProviderName.SqLite:
                    {
                        sql = $"select 1 from {AttributesOfClass<TSource>.TableName(MyProviderName)};";
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception ex)
            {
                Transactionale.isError = true;
                MySqlLogger.Error("", ex);
                throw;
            }
            
            var p = new V(sql);
            Expression callExpr = Expression.Call(Expression.Constant(p), p.GetType().GetMethod("TableExists"));
            DbQueryProvider<TSource> provider = new DbQueryProvider<TSource>(this);
            return (bool)provider.ExecuteExtension<bool>(callExpr); ;

          
        }

        Task<bool> ISession.TableExistsAsync<TSource>(CancellationToken cancellationToken)
        {
            string sql;
            try
            {
                switch (MyProviderName)
                {
                    case ProviderName.PostgreSql:
                    {
                        var tableName = UtilsCore.ClearTrim(AttributesOfClass<TSource>.TableName(MyProviderName));
                        sql = $"SELECT count(*) FROM pg_tables WHERE   tablename  = '{tableName}';";
                        break;
                    }

                    case ProviderName.MsSql:
                    {
                        string t = UtilsCore.ClearTrim(AttributesOfClass<TSource>.TableName(MyProviderName));
                        sql = $"SELECT OBJECT_ID('{t}', 'U');";
                        break;
                    }
                    case ProviderName.MySql:
                    case ProviderName.SqLite:
                    {
                        sql = $"select 1 from {AttributesOfClass<TSource>.TableName(MyProviderName)};";
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception ex)
            {
                Transactionale.isError = true;
                MySqlLogger.Error("", ex);
                throw;
            }

            var p = new V(sql);
            Expression callExpr = Expression.Call(Expression.Constant(p), p.GetType().GetMethod("TableExists"));
            DbQueryProvider<TSource> provider = new DbQueryProvider<TSource>(this);
            return provider.ExecuteExtensionAsync<bool>(callExpr,null,cancellationToken); ;
        }

        #region ExecuteReader
        IDataReader ISession.ExecuteReader(string sql, object[] param)
        {
            Check.NotEmpty(sql, "sql",() => Transactionale.isError=true);
            var com = ProviderFactories.GetCommand(_factory, ((ISession)this).IsDispose);
            com.Connection = _connect;
            com.CommandText = sql;
            UtilsCore.AddParam(com, MyProviderName, param);
            OpenConnectAndTransaction(com);
            try
            {
                return com.ExecuteReader();

            }
            catch (Exception ex)
            {
                Transactionale.isError = true;
                MySqlLogger.Error(com.CommandText, ex);
                throw;
            }
        }

        IDataReader ISession.ExecuteReader(string sql, int timeOut, params object[] param)
        {
            Check.NotEmpty(sql, "sql",() => Transactionale.isError=true);
            var com = ProviderFactories.GetCommand(_factory, ((ISession)this).IsDispose);
            com.Connection = _connect;
            com.CommandText = sql;
            SetTimeOut(com, timeOut);

            UtilsCore.AddParam(com, MyProviderName, param);
            OpenConnectAndTransaction(com);
            try
            {
                return com.ExecuteReader();
            }
            catch (Exception ex)
            {
                Transactionale.isError = true;
                MySqlLogger.Error(com.CommandText, ex);
                throw;
            }

        }

        public Task<IDataReader> ExecuteReaderAsync(string sql, object[] param, CancellationToken cancellationToken = default)
        {
            var tk = new TaskCompletionSource<IDataReader>();
            CancellationTokenRegistration? registration = null;
            Check.NotEmpty(sql, "sql", () => Transactionale.isError = true);
            var com = ProviderFactories.GetCommand(_factory, ((ISession)this).IsDispose);
            com.Connection = _connect;
            com.CommandText = sql;
            UtilsCore.AddParam(com, MyProviderName, param);
            OpenConnectAndTransaction(com);
            try
            {
                if (cancellationToken != default)
                {
                    registration =
                        cancellationToken.Register(UtilsCore.CancellRegistr(com, cancellationToken, Transactionale,MyProviderName));
                }

                tk.SetResult(com.ExecuteReader());
                return tk.Task;
            }
            catch (Exception ex)
            {

                Transactionale.isError = true;
                MySqlLogger.Error(com.CommandText, ex);
                throw;
            }
            finally
            {
                if (registration.HasValue)
                {
                    registration.Value.Dispose();
                }
            }
        }

        public Task<IDataReader> ExecuteReaderAsync(string sql, int timeOut, object[] param, CancellationToken cancellationToken = default)
        {
            var tk = new TaskCompletionSource<IDataReader>();
            CancellationTokenRegistration? registration = null;
            Check.NotEmpty(sql, "sql",() => Transactionale.isError=true);
            var com = ProviderFactories.GetCommand(_factory, ((ISession)this).IsDispose);
            com.Connection = _connect;
            com.CommandText = sql;
            com.CommandTimeout = timeOut;
            UtilsCore.AddParam(com, MyProviderName, param);
            OpenConnectAndTransaction(com);
            try
            {
                if (cancellationToken != default)
                {
                    registration =
                        cancellationToken.Register(UtilsCore.CancellRegistr(com, cancellationToken, Transactionale,MyProviderName));
                }

                tk.SetResult(com.ExecuteReader());
                return tk.Task;
            }
            catch (Exception ex)
            {

                Transactionale.isError = true;
                MySqlLogger.Error(com.CommandText, ex);
                throw;
            }
            finally
            {
                if (registration.HasValue)
                {
                    registration.Value.Dispose();
                }
            }
        }
        #endregion

        DataTable ISession.GetDataTable(string sql, int timeOut)
        {
            Check.NotEmpty(sql, "sql",() => Transactionale.isError=true);
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
                Transactionale.isError = true;
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
                case ProviderName.SqLite:
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

                case ProviderName.PostgreSql:
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
                Transactionale.isError = true;
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
            Check.NotEmpty(baseName, "baseName",()=>Transactionale.isError=true);
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
                    case ProviderName.PostgreSql:
                        com.CommandText = $"CREATE DATABASE {baseName};";
                        return -1;
                    case ProviderName.SqLite:
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
                Transactionale.isError = true;
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
            Check.NotNull(enumerable, "list", () => Transactionale.isError = true);
            Check.NotNull(enumerable, "list", () => Transactionale.isError = true);
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
                case ProviderName.PostgreSql:
                    com.CommandText = new UtilsBulkPostgres(ProviderName.PostgreSql).GetSql(enumerable);
                    break;
                case ProviderName.SqLite:
                    com.CommandText = new UtilsBulkMySql(ProviderName.SqLite).GetSql(enumerable);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            try
            {
                OpenConnectAndTransaction(com);
                SetTimeOut(com, timeOut);
                var res = com.ExecuteNonQuery();
                foreach (var iSource in enumerable) ((ISession)this).ToPersistent(iSource);
                return res;
            }
            catch (Exception ex)
            {
                Transactionale.isError = true;
                MySqlLogger.Error($"{ex.Message}{Environment.NewLine}{UtilsCore.GetStringSql(com)}", ex);
                throw;
            }
            finally
            {
                ComDisposable(com);
            }
        }

        public Task<int> InsertBulkAsync<TSource>(IEnumerable<TSource> list, int timeOut, CancellationToken cancellationToken = default) where TSource : class
        {

            var tcs = new TaskCompletionSource<int>();
            CancellationTokenRegistration? registration = null;
            var enumerable = list as TSource[] ?? list.ToArray();
            Check.NotNull(enumerable, "list", () => Transactionale.isError = true);
            Check.NotNull(enumerable, "list", () => Transactionale.isError = true);
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
                case ProviderName.PostgreSql:
                    com.CommandText = new UtilsBulkPostgres(ProviderName.PostgreSql).GetSql(enumerable);
                    break;
                case ProviderName.SqLite:
                    com.CommandText = new UtilsBulkMySql(ProviderName.SqLite).GetSql(enumerable);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            try
            {
                OpenConnectAndTransaction(com);
                com.CommandTimeout = timeOut;
                if (cancellationToken != default)
                {
                    
                    registration = cancellationToken.Register(UtilsCore.CancellRegistr(com, cancellationToken, Transactionale,MyProviderName));
                }
                var res = com.ExecuteNonQuery();
                if (registration.HasValue)
                {
                    registration.Value.Dispose();
                }
                foreach (var iSource in enumerable) ((ISession)this).ToPersistent(iSource);
                tcs.SetResult(res);
                return tcs.Task;
            }
            catch (Exception ex)
            {
                Transactionale.isError = true;
                MySqlLogger.Error($"{ex.Message}{Environment.NewLine}{UtilsCore.GetStringSql(com)}", ex);
                throw;
            }
            finally
            {
                ComDisposable(com);
            }
        }


        int ISession.InsertBulkFromFile<TSource>(string fileCsv, string fieldterminator, int timeOut)
        {
            Check.NotEmpty(fileCsv, "fileCsv", () => Transactionale.isError = true);
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
                case ProviderName.PostgreSql:
                    com.CommandText = UtilsBulkPostgres.InsertFile<TSource>(fileCsv, fieldterminator, MyProviderName);
                    break;
                case ProviderName.SqLite:
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
                Transactionale.isError = true;
                MySqlLogger.Error(UtilsCore.GetStringSql(com), ex);
                throw;
            }
            finally
            {
                ComDisposable(com);
            }
        }

        #region ExecuteScalar


        object ISession.ExecuteScalar(string sql, params object[] param)
        {
            Check.NotEmpty(sql, "sql",() => Transactionale.isError=true);
            var p = new V(sql);
            Expression callExpr = Expression.Call(Expression.Constant(p), p.GetType().GetMethod("ExecuteScalar"));
            var provider = new DbQueryProvider<object>(this);
            return provider.ExecuteExtension<object>(callExpr,param);

        }

        Task<object> ISession.ExecuteScalarAsync(string sql,  object[] param,CancellationToken cancellationToken)
        {
            Check.NotEmpty(sql, "sql", () => Transactionale.isError = true);
            var p = new V(sql);
            Expression callExpr = Expression.Call(Expression.Constant(p), p.GetType().GetMethod("ExecuteScalar"));
            var provider = new DbQueryProvider<object>(this);
            return provider.ExecuteExtensionAsync<object>(callExpr, param,cancellationToken);
        }

        object ISession.ExecuteScalar(string sql, int timeOut, params object[] param)
        {
            Check.NotEmpty(sql, "sql",() => Transactionale.isError=true);
            Check.NotEmpty(sql, "sql", () => Transactionale.isError = true);
            var p = new V(sql);
            Expression callExpr = Expression.Call(Expression.Constant(p), p.GetType().GetMethod("ExecuteScalar"));
            DbQueryProvider<object> provider = new DbQueryProvider<object>(this);
            provider.ListCastExpression.Add(new ContainerCastExpression
                { Timeout = timeOut, TypeRevalytion = Evolution.Timeout });
            return provider.ExecuteExtension<object>(callExpr, param);
        }

        Task<object> ISession.ExecuteScalarAsync(string sql, int timeOut,  object[] param,CancellationToken cancellationToken)
        {
            Check.NotEmpty(sql, "sql", () => Transactionale.isError = true);
            Check.NotEmpty(sql, "sql", () => Transactionale.isError = true);
            var p = new V(sql);
            Expression callExpr = Expression.Call(Expression.Constant(p), p.GetType().GetMethod("ExecuteScalar"));
            DbQueryProvider<object> provider = new DbQueryProvider<object>(this);
            provider.ListCastExpression.Add(new ContainerCastExpression
                { Timeout = timeOut, TypeRevalytion = Evolution.Timeout });
            return provider.ExecuteExtensionAsync<object>(callExpr, param,cancellationToken);
        }


        #endregion

        int ISession.TruncateTable<TSource>()
        {
            string sql;
            if (MyProviderName == ProviderName.SqLite)
                sql = $"DELETE FROM {AttributesOfClass<TSource>.TableName(MyProviderName)};";
            else
                sql = $"TRUNCATE TABLE {AttributesOfClass<TSource>.TableName(MyProviderName)};";

            var p = new V(sql);
            Expression callExpr = Expression.Call(Expression.Constant(p), p.GetType().GetMethod("TruncateTable"));
            var provider = new DbQueryProvider<object>(this);
            return provider.ExecuteExtension<int>(callExpr);

        }

         Task<int> ISession.TruncateTableAsync<TSource>(CancellationToken cancellationToken)
        {
            string sql;
            if (MyProviderName == ProviderName.SqLite)
                sql = $"DELETE FROM {AttributesOfClass<TSource>.TableName(MyProviderName)};";
            else
                sql = $"TRUNCATE TABLE {AttributesOfClass<TSource>.TableName(MyProviderName)};";

            var p = new V(sql);
            Expression callExpr = Expression.Call(Expression.Constant(p), p.GetType().GetMethod("TruncateTable"));
            var provider = new DbQueryProvider<object>(this);
            return provider.ExecuteExtensionAsync<int>(callExpr,null,cancellationToken);
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
            Check.NotEmpty(sql, "sql",() => Transactionale.isError=true);
            var p = new V(sql);
            Expression callExpr = Expression.Call(Expression.Constant(p), p.GetType().GetMethod("ExecuteNonQuery"));
            var provider = new DbQueryProvider<object>(this);
            return provider.ExecuteExtension<int>(callExpr,param);
        }

        Task<int> ISession.ExecuteNonQueryAsync(string sql,  object[] param,CancellationToken cancellationToken)
        {
            Check.NotEmpty(sql, "sql", () => Transactionale.isError = true);
            var p = new V(sql);
            Expression callExpr = Expression.Call(Expression.Constant(p), p.GetType().GetMethod("ExecuteNonQuery"));
            var provider = new DbQueryProvider<object>(this);
            return provider.ExecuteExtensionAsync<int>(callExpr, param,cancellationToken);
        }

        int ISession.ExecuteNonQuery(string sql, int timeOut, params object[] param)
        {
            
            Check.NotEmpty(sql, "sql", () => Transactionale.isError = true);
            var p = new V(sql);
            Expression callExpr = Expression.Call(Expression.Constant(p), p.GetType().GetMethod("ExecuteNonQuery"));
            var provider = new DbQueryProvider<object>(this);
            provider.ListCastExpression.Add(new ContainerCastExpression
                { Timeout = timeOut, TypeRevalytion = Evolution.Timeout });
            return provider.ExecuteExtension<int>(callExpr, param);
        }

        string ISession.TableName<TSource>()
        {
            try
            {
                return AttributesOfClass<TSource>.TableName(MyProviderName);

            }
            catch (Exception)
            {
                Transactionale.isError = true;
                throw;
            }

        }

        string ISession.ColumnName<TSource>(Expression<Func<TSource, object>> property)
        {
            Check.NotNull(property, "property", () => Transactionale.isError = true);
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
            Transactionale.isError = true;
            throw new Exception($"Can't determine table field for type: {typeof(TSource)}");
        }

        string ISession.GetSqlInsertCommand<TSource>(TSource source)
        {
            Check.NotNull(source, "source", () => Transactionale.isError = true);
            try
            {
                switch (MyProviderName)
                {
                    case ProviderName.MsSql:
                        throw new Exception("Not implemented");
                    case ProviderName.MySql:
                        throw new Exception("Not implemented");
                    case ProviderName.PostgreSql:
                        return new CommandNativePostgres(ProviderName.PostgreSql).GetInsertSql(source);
                    case ProviderName.SqLite:
                        throw new Exception("Not implemented");
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception)
            {
                Transactionale.isError = true;
                throw;
            }

        }

        string ISession.GetSqlDeleteCommand<TSource>(TSource source)
        {
            Check.NotNull(source, "source", () => Transactionale.isError = true);
            try
            {
                switch (MyProviderName)
                {
                    case ProviderName.MsSql:
                        throw new Exception("Not implemented");
                    case ProviderName.MySql:
                        throw new Exception("Not implemented");
                    case ProviderName.PostgreSql:
                        return new CommandNativePostgres(ProviderName.PostgreSql).GetDeleteSql(source);
                    case ProviderName.SqLite:
                        throw new Exception("Not implemented");
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception)
            {
                Transactionale.isError = true;
                throw;
            }

            // 
        }

        #region DataTable

        

          
        DataTable ISession.GetDataTable(string sql, int timeOut, params object[] param)
        {
            Check.NotEmpty(sql, "sql", () => Transactionale.isError = true);
            var p = new V(sql);
            Expression callExpr = Expression.Call(Expression.Constant(p), p.GetType().GetMethod("DataTable"));
            var provider = new DbQueryProvider<object>(this);
            provider.ListCastExpression.Add(new ContainerCastExpression
                { Timeout = timeOut, TypeRevalytion = Evolution.Timeout });
            return provider.ExecuteExtension<DataTable>(callExpr, param);
        }

        DataTable ISession.GetDataTable(string sql,  params object[] param)
        {
            Check.NotEmpty(sql, "sql", () => Transactionale.isError = true);
            var p = new V(sql);
            Expression callExpr = Expression.Call(Expression.Constant(p), p.GetType().GetMethod("DataTable"));
            var provider = new DbQueryProvider<object>(this);
            return provider.ExecuteExtension<DataTable>(callExpr, param);
        }

        Task<DataTable>ISession.GetDataTableAsync(string sql, int timeOut, object[] param,CancellationToken cancellationToken)
        {
            Check.NotEmpty(sql, "sql", () => Transactionale.isError = true);
            var p = new V(sql);
            Expression callExpr = Expression.Call(Expression.Constant(p), p.GetType().GetMethod("DataTable"));
            var provider = new DbQueryProvider<object>(this);
            provider.ListCastExpression.Add(new ContainerCastExpression
                { Timeout = timeOut, TypeRevalytion = Evolution.Timeout });
            return provider.ExecuteExtensionAsync<DataTable>(callExpr, param,cancellationToken);
        }

        Task<DataTable> ISession.GetDataTableAsync(string sql,  object[] param, CancellationToken cancellationToken)
        {
            Check.NotEmpty(sql, "sql", () => Transactionale.isError = true);
            var p = new V(sql);
            Expression callExpr = Expression.Call(Expression.Constant(p), p.GetType().GetMethod("DataTable"));
            var provider = new DbQueryProvider<object>(this);
           
            return provider.ExecuteExtensionAsync<DataTable>(callExpr, param, cancellationToken);
        }

        #endregion

        TSource ISession.Clone<TSource>(TSource source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            try
            {
                return UtilsCore.Clone(source);
            }
            catch (Exception ex)
            {
                Transactionale.isError = true;
                MySqlLogger.Error("Clone", ex);
                throw;
            }
        }

        string ISession.GetSqlForInsertBulk<TSource>(IEnumerable<TSource> list)
        {
            var enumerable = list as TSource[] ?? list.ToArray();
            Check.NotNull(enumerable, "list", () => Transactionale.isError = true);

            try
            {
                switch (MyProviderName)
                {
                    case ProviderName.MsSql:
                        return new UtilsBulkMsSql(ProviderName.MsSql).GetSql(enumerable);
                    case ProviderName.MySql:
                        return new UtilsBulkMySql(ProviderName.MySql).GetSql(enumerable);
                    case ProviderName.PostgreSql:
                        return new UtilsBulkPostgres(ProviderName.PostgreSql).GetSql(enumerable);
                    case ProviderName.SqLite:
                        return new UtilsBulkMySql(ProviderName.SqLite).GetSql(enumerable);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception)
            {
                Transactionale.isError = true;
                throw;
            }

        }

         int ISession.Update<TSource>(TSource source, params AppenderWhere[] whereObjects)
        {
            Check.NotNull(source, "source", () => Transactionale.isError = true);
            if (!UtilsCore.IsPersistent(source))
            {
                Transactionale.isError = true;
                throw new Exception("You are trying to update an object not obtained from database");
            }
            return SaveNew(source, whereObjects);
        }

         Task<int> ISession.UpdateAsync<TSource>(TSource source, AppenderWhere[] whereObjects,CancellationToken cancellationToken) 
        {
            Check.NotNull(source, "source", () => Transactionale.isError = true);
            if (!UtilsCore.IsPersistent(source))
            {
                Transactionale.isError = true;
                throw new Exception("You are trying to update an object not obtained from database");
            }
            return SaveNewAsync(source,whereObjects,cancellationToken);
        }

        private void SetTimeOut(IDbCommand com, int timeOut)
        {
            Check.NotNull(com, "com", () => Transactionale.isError = true);
            if (timeOut < 0)
            {
                Transactionale.isError = true;
                throw new Exception($"timeOut has an invalid value:{timeOut}");
            }
            com.CommandTimeout = timeOut;
        }

        private int SaveNew<TSource>(TSource source, params AppenderWhere[] whereObjects) where TSource : class
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
                    if (MyProviderName == ProviderName.PostgreSql || MyProviderName == ProviderName.SqLite)
                        AttributesOfClass<TSource>.CreateUpdateCommandPostgres(com, source, MyProviderName,
                            whereObjects);
                    else
                        AttributesOfClass<TSource>.CreateUpdateCommandMysql(com, source, MyProviderName, whereObjects);


                    OpenConnectAndTransaction(com);
                    res = com.ExecuteNonQuery();
                    if (res == 1)
                    {
                        this.CacheClear<TSource>();
                        NotificAfter(source, ActionMode.Update);
                    }
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
                            this.CacheClear<TSource>();
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
                            res = 1;
                            this.CacheClear<TSource>();
                            NotificAfter(source, ActionMode.Insert);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Transactionale.isError = true;
                MySqlLogger.Error(UtilsCore.GetStringSql(com), ex);
                throw;
            }
            finally
            {
                ComDisposable(com);
            }

            return res;
        }

        private Task<int> SaveNewAsync<TSource>(TSource source, AppenderWhere[] whereObjects,CancellationToken cancellationToken) where TSource : class
        {
            var tk = new TaskCompletionSource<int>();
            CancellationTokenRegistration? registration = null;
            var res = 0;
            var com = ProviderFactories.GetCommand(_factory, ((ISession)this).IsDispose);
            com.Connection = _connect;
            com.CommandText = string.Empty;
           

            try
            {
                OpenConnectAndTransaction(com);
                if (cancellationToken != default)
                {
                    registration =
                        cancellationToken.Register(UtilsCore.CancellRegistr(com, cancellationToken, Transactionale,MyProviderName));
                }
                if (UtilsCore.IsPersistent(source))
                {
                    NotificBefore(source, ActionMode.Update);
                    if (MyProviderName == ProviderName.PostgreSql || MyProviderName == ProviderName.SqLite)
                        AttributesOfClass<TSource>.CreateUpdateCommandPostgres(com, source, MyProviderName,
                            whereObjects);
                    else
                        AttributesOfClass<TSource>.CreateUpdateCommandMysql(com, source, MyProviderName, whereObjects);


                   
                    res = com.ExecuteNonQuery();
                    if (res == 1)
                    {
                        this.CacheClear<TSource>();
                        NotificAfter(source, ActionMode.Update);
                    }
                }
                else
                {
                    NotificBefore(source, ActionMode.Insert);
                    AttributesOfClass<TSource>.CreateInsetCommand(com, source, MyProviderName);
                    if (AttributesOfClass<TSource>.PkAttribute(MyProviderName).Generator == Generator.Assigned)
                    {
                        var val = com.ExecuteNonQuery();
                        if (val == 1)
                        {
                            ((ISession)this).ToPersistent(source);
                            res = 1;
                            this.CacheClear<TSource>();
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
                            res = 1;
                            this.CacheClear<TSource>();
                            NotificAfter(source, ActionMode.Insert);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Transactionale.isError = true;
                MySqlLogger.Error(UtilsCore.GetStringSql(com), ex);
                throw;
            }
            finally
            {
                if (registration.HasValue)
                {
                    registration.Value.Dispose();
                }
                ComDisposable(com);
            }
            tk.SetResult(res);
            return tk.Task;

        }


        internal void ComDisposable(IDbCommand com)
        {
            Check.NotNull(com, "com", () => Transactionale.isError = true);
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