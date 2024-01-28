using ORM_1_21_.Extensions;
using ORM_1_21_.Linq;
using ORM_1_21_.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ORM_1_21_
{
    /// <summary>
    ///     Extension ORM
    /// </summary>
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
    public static partial class Helper
    {
        /// <summary>
        /// The IN operator allows you to specify multiple values in a WHERE clause.
        /// The IN operator is a shorthand for multiple OR conditions.
        /// </summary>
        public static IQueryable<T> WhereIn<T,TS>(this IQueryable<T> query, Expression<Func<T, TS>> selector, params TS[] o)
        {
            Check.NotNull(query, nameof(query));
            Check.NotNull(selector, nameof(selector));
            Check.NotNull(o, nameof(o));
            var selectE = selector.Body;
            var mi = typeof(V).GetMethod("WhereIn");
            var miConstructed = mi.MakeGenericMethod(typeof(TS));

            ReadOnlyCollection<ParameterExpression> selectorParameters = selector.Parameters;
            var paramsE = Expression.Constant(o);
            Expression check = Expression.Call(null, miConstructed, selectE, paramsE);
            var lambada = Expression.Lambda<Func<T, bool>>(check, selectorParameters);
            return query.Where(lambada);
        }


        /// <summary>
        /// Custom  Select clause
        /// </summary>
        /// <param name="query">Current IQueryable</param>
        /// <param name="sql">Raw query string</param>
        /// <typeparam name="TR">Return Queryable type</typeparam>
        /// <returns></returns>
        public static IQueryable<TR> SelectSql<TR>(this IQueryable query, string sql)
        {
            Check.NotNull(query, nameof(query));
            Check.NotNull(sql, nameof(sql));
            var sqlE = Expression.Constant(sql);
            var mi = typeof(V).GetMethod("SelectSql");
            var miConstructed = mi.MakeGenericMethod(typeof(TR));
            Expression check = Expression.Call(null, miConstructed, sqlE);
            return query.Provider.CreateQuery<TR>(check);
        }

        /// <summary>
        /// The raw query Sql, allows you not to remember the names of the table fields, it returns an IQueryable&lt;object&gt;
        /// </summary>
        /// <param name="query">Current IQueryable</param>
        /// <param name="selector">Body string</param>
        /// <param name="sqlParams">Array of request parameters</param>
        public static IQueryable<object> SelectSqlE<TSource>(this IQueryable<TSource> query, Expression<Func<TSource, string>> selector, params SqlParam[] sqlParams)
        {
            Check.NotNull(query, nameof(query));
            Check.NotNull(selector, nameof(selector));
            var sqlParamsE = Expression.Constant(sqlParams);
            var mi = typeof(V).GetMethod("SelectSqlE");
            var miConstructed = mi.MakeGenericMethod(typeof(object));
            Expression check = Expression.Call(null, miConstructed, query.Expression,selector, sqlParamsE);
            return query.Provider.CreateQuery<object>(check);
        }







        /// <summary>
        /// A raw request sql to update a table allows you not to remember the names of the table fields
        /// </summary>
        /// <param name="query">Current IQueryable</param>
        /// <param name="rawSql">Body string</param>
        /// <param name="sqlParams">Array of request parameters</param>
        public static int UpdateSql<T>(this IQueryable<T> query, Expression<Func<T, string>> rawSql, params SqlParam[] sqlParams)
        {
            Check.NotNull(query, nameof(query));
            Check.NotNull(rawSql, nameof(rawSql));
            Check.NotNull(sqlParams, nameof(sqlParams));
            var sqlParamsE = Expression.Constant(sqlParams);

            var mi = typeof(V).GetMethod("UpdateSqlE");
            Expression check = Expression.Call(null, mi, query.Expression, rawSql, sqlParamsE);
            var expressionNodeType = query.Expression.NodeType;
            return query.Provider.Execute<int>(check);

           
        }

       


        /// <summary>
        /// Custom  Select clause
        /// </summary>
        /// <param name="query">Current IQueryable</param>
        /// <param name="sql">Raw query string</param>
        /// <param name="sqlParams">Params query string</param>
        /// <typeparam name="TR">Return Queryable type</typeparam>
        /// <returns></returns>
        public static IQueryable<TR> SelectSql<TR>(this IQueryable query, string sql, params SqlParam[] sqlParams)
        {
            Check.NotNull(query, nameof(query));
            Check.NotNull(sql, nameof(sql));
            Check.NotNull(sqlParams, nameof(sqlParams));
            var t = Expression.Parameter(query.GetType());
            var sqlE = Expression.Constant(sql);
            var sqlParamsE = Expression.Constant(sqlParams);
            var mi = typeof(V).GetMethod("SelectSqlP");
            var miConstructed = mi.MakeGenericMethod(typeof(TR));
            Expression check = Expression.Call(null, miConstructed, sqlE, sqlParamsE);
            return query.Provider.CreateQuery<TR>(check);
        }


        /// <summary>
        /// A raw query sql for selecting by condition allows you not to remember the names of table fields
        /// </summary>
        /// <param name="query">Current IQueryable</param>
        /// <param name="rawSql">Body string</param>
        /// <param name="sqlParams">Array of request parameters</param>
        public static IQueryable<T> WhereSql<T>(this IQueryable<T> query, Expression<Func<T, string>> rawSql, params SqlParam[] sqlParams)
        {
            Check.NotNull(query, nameof(query));
            Check.NotNull(rawSql, nameof(rawSql));
            Check.NotNull(sqlParams, nameof(sqlParams));
            var sqlParamsE = Expression.Constant(sqlParams);
            var t = Expression.Parameter(typeof(T));
            var mi = typeof(V).GetMethod("WhereSqlE");
            Expression check = Expression.Call(null, mi, query.Expression, rawSql, sqlParamsE);
            var lambada = Expression.Lambda<Func<T, bool>>(check,t);
            return query.Where(lambada);
        }





        /// <summary>
        /// Custom FROM  clause (Doesn't work with MsSql database)
        /// </summary>
        public static IQueryable<T> FromSql<T>(this IQueryable<T> query, string sql)
        {
            Check.NotNull(query, nameof(query));
            Check.NotNull(sql, nameof(sql));
            
            var provider = (DbQueryProvider<T>)query.Provider; 
            ProviderName providerName = provider.Sessione.ProviderName;
            if (providerName == ProviderName.MsSql)
            {
                throw new Exception("Method:FromSql doesn't work with MsSql database");
            }

            var sqlE = Expression.Constant(sql);
            var queryE = Expression.Constant(query);
            var mi = typeof(V).GetMethod("FromString");
            var miConstructed = mi.MakeGenericMethod(typeof(T));
            Expression check = Expression.Call(null, miConstructed, sqlE,queryE);
            return query.Provider.CreateQuery<T>(check);
        }

        /// <summary>
        /// Custom FROM  clause with parameters sql
        /// </summary>
        /// <param name="query"></param>
        /// <param name="sql"></param>
        /// <param name="sqlParams"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IQueryable<T> FromSql<T>(this IQueryable<T> query, string sql,params SqlParam[] sqlParams)
        {
            Check.NotNull(query, nameof(query));
            Check.NotNull(sql, nameof(sql));
            Check.NotNull(sqlParams, nameof(sqlParams));

            var sqlE = Expression.Constant(sql);
            var sqlParamsE = Expression.Constant(sqlParams);
            var queryE = Expression.Constant(query);
            var mi = typeof(V).GetMethod("FromStringP");
            var miConstructed = mi.MakeGenericMethod(typeof(T));
            Expression check = Expression.Call(null, miConstructed, sqlE,sqlParamsE ,queryE);
            return query.Provider.CreateQuery<T>(check);
        }



       


        /// <summary>
        /// NOT IN operator replaces a set of arguments with the &lt; &gt; or != operator that is combined with the AND operator.
        /// </summary>

        public static IQueryable<T> WhereNotIn<T, TS>(this IQueryable<T> query, Expression<Func<T, TS>> selector, params TS[] o)
        {
            Check.NotNull(selector, nameof(selector));
            Check.NotNull(o, nameof(o));
            var selectE = selector.Body;
            var mi = typeof(V).GetMethod("WhereNotIn");
            var miConstructed = mi.MakeGenericMethod(typeof(TS));

            var selectorParameters = selector.Parameters;
            var paramsE = Expression.Constant(o);
            Expression check = Expression.Call(null, miConstructed, selectE, paramsE);
            var lambada = Expression.Lambda<Func<T, bool>>(check, selectorParameters);
            return query.Where(lambada);
        }


       




        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="expression"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <returns></returns>
        public static Task<TSource> ExecuteAsync<TSource>(this IQueryProvider provider, Expression expression, CancellationToken cancellationToken = default)
        {
            var p = (QueryProvider)provider;
            return p.ExecuteExtensionAsync<TSource>(expression, null, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>   
        /// <typeparam name="TSource"></typeparam>
        /// <returns></returns>
        public static IEnumerable<TSource> AsEnumerable<TSource>(
            this IQueryable<TSource> source)
        {
            return source.Provider.Execute<IEnumerable<TSource>>(source.Expression);
        }


        /// <summary>
        ///     Iterating over a collection
        /// </summary>
        public static void ForEach<TSource>(this IQueryable<TSource> source, Action<TSource> action)
        {
            var res = (Query<TSource>)source;
            res.ForEach(action);
        }

        /// <summary>
        ///     Convert date to SQL format
        /// </summary>
        public static string DataToString(this ISession ses, DateTime dateTime)
        {
            return Configure.Utils.DateToString(dateTime);
        }

        /// <summary>
        ///     Array of non-recurring values, by selected field
        /// </summary>
        public static IEnumerable<TResult> Distinct<TSource, TResult>(this IQueryable<TSource> source,
            Expression<Func<TSource, TResult>> exp) where TSource : class
        {
            ((ISqlComposite)source.Provider).ListCastExpression.Add(new ContainerCastExpression
            {
                CustomExpression = exp,
                TypeEvolution = Evolution.DistinctCore,
                TypeReturn = typeof(TResult),
                ListDistinct = new List<TResult>()
            });
            return source.Provider.Execute<IEnumerable<TResult>>(source.Expression);
        }


        /// <summary>
        ///     Execution to delete a record from a table
        /// </summary>
        public static int Delete<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> exp = null)
            where TSource : class
        {
            ((ISqlComposite)source.Provider).ListCastExpression.Add(new ContainerCastExpression
            { CustomExpression = exp, TypeEvolution = Evolution.Delete });
            return source.Provider.Execute<int>(source.Expression);
        }

        /// <summary>
        ///     Execution to delete a record from a table
        /// </summary>
        public static Task<int> DeleteAsync<TSource>(this IQueryable<TSource> source,
            Expression<Func<TSource, bool>> exp = null) where TSource : class
        {
            ((ISqlComposite)source.Provider).ListCastExpression.Add(new ContainerCastExpression
            { CustomExpression = exp, TypeEvolution = Evolution.Delete });
            return ((QueryProvider)source.Provider).ExecuteExtensionAsync<int>(source.Expression, null,
                CancellationToken.None);
        }

        /// <summary>
        /// Getting an object by key field
        /// </summary>
        /// <param name="session">Current session</param>
        /// <param name="key">Primary key value</param>
        /// <typeparam name="T"></typeparam>
        public static T Get<T>(this ISession session, object key) where T : class
        {
            Check.NotNull(key, nameof(key));
            var s = AttributesOfClass<T>.PkAttribute(session.ProviderName).GetColumnName(session.ProviderName);
            string sql = $"select * from {session.TableName<T>()} where {s} = {session.SymbolParam}1;";
            var res = session.FreeSql<T>(sql, key);
            switch (res.Count())
            {
                case 0:
                    {
                        return null;
                    }
                case 1:
                    {
                        return res.First();
                    }
                default:
                    {
                        throw new Exception($"When querying by key field, more than one value was received: {res.Count()}");
                    }
            }
        }
        /// <summary>
        /// Getting an object by key field
        /// </summary>
        /// <param name="session">Current session</param>
        /// <param name="key">Primary key value</param>
        /// <typeparam name="T"></typeparam>
        public static async Task<T> GetAsync<T>(this ISession session, object key) where T : class
        {
            Check.NotNull(key, nameof(key));
            var s = AttributesOfClass<T>.PkAttribute(session.ProviderName).GetColumnName(session.ProviderName);
            string sql = $"select * from {session.TableName<T>()} where {s} = {session.SymbolParam}1;";
            var res = await session.FreeSqlAsync<T>(sql, key);
            switch (res.Count())
            {
                case 0:
                    {
                        return null;
                    }
                case 1:
                    {
                        return res.First();
                    }
                default:
                    {
                        throw new Exception($"When querying by key field, more than one value was received: {res.Count()}");
                    }
            }
        }

        /// <summary>
        /// Modification of an object in the database (insert, update).
        /// The object is marked with the attribute: MapUsagePersistentAttribute.
        /// </summary>
        /// <param name="session">Current session.</param>
        /// <param name="ob">Modification object.</param>
        /// <typeparam name="T">Type modification object.</typeparam>
        /// <returns>Number of rows modified in the database.</returns>
        public static int Save<T>(this ISession session,T ob) where T : class
        {
            Check.NotNull(ob, nameof(ob));
            if (UtilsCore.IsPersistent(ob))
            {
                return session.Update(ob);
            }

            return session.Insert(ob);
        }

        /// <summary>
        /// Asynchronous operation.
        /// Modification of an object in the database (insert, update).
        /// The object is marked with the attribute: MapUsagePersistentAttribute.
        /// </summary>
        /// <param name="session">Current session.</param>
        /// <param name="ob">Modification object</param>
        /// <param name="cancellationToken">Cancel token.</param>
        /// <typeparam name="T">Type modification object.</typeparam>
        /// <returns>Number of rows modified in the database.</returns>
        public static async Task<int> SaveAsync<T>(this ISession session, T ob,CancellationToken cancellationToken= default) where T : class
        {
            Check.NotNull(ob, nameof(ob));
            if (UtilsCore.IsPersistent(ob))
            {
                return await session.UpdateAsync(ob,cancellationToken);
            }

            return await session.InsertAsync(ob, cancellationToken);
        }


        /// <summary>
        /// Mark an object as being received from a database.
        /// </summary>
        /// <param name="session">Current session</param>
        /// <param name="obj">The object type must have an attribute: MapUsagePersistentAttribute</param>
        /// <typeparam name="T">Object type</typeparam>
        public static void ToPersistent<T>(this ISession session,T obj) where T : class
        {
            Check.NotNull(session, nameof(session));
            Check.NotNull(obj, nameof(obj));
            if (AttributesOfClass<T>.IsUsagePersistent.Value == false)
            {
                throw new Exception("The object type  does not have an attribute: MapUsagePersistentAttribute");
            }
            UtilsCore.SetPersistent(obj);
        }

        /// <summary>
        /// Checking where an object came from.
        /// True - object from database. False - object not from database.
        /// </summary>
        /// <returns>True - object from database. False - object not from database.</returns>
        public static bool IsPersistent<T>(this ISession session, T obj) where T : class
        {
            return UtilsCore.IsPersistent(obj);
        }


        /// <summary>
            ///     LIMIT is always placed at the end of the sentence
            ///     (the beginning of the position, taking into account zero, the number in the sample)
            /// </summary>
            /// <param name="source"></param>
            /// <param name="start">Position start</param>
            /// <param name="length">Record length</param>
            public static IQueryable<TSource> Limit<TSource>(this IQueryable<TSource> source, int start, int length)
        {
            ((ISqlComposite)source.Provider).ListCastExpression.Add(new ContainerCastExpression
            { TypeEvolution = Evolution.Limit, ParamList = new List<object> { start, length } });
            return source;
        }

        /// <summary>
        ///     LIMIT is always placed at the end of the sentence
        ///     (amount from beginning)
        /// </summary>
        /// <param name="source"></param>
        /// <param name="length">Record length</param>
        public static IQueryable<TSource> Limit<TSource>(this IQueryable<TSource> source, int length)
        {
            ((ISqlComposite)source.Provider).ListCastExpression.Add(new ContainerCastExpression
            { TypeEvolution = Evolution.Limit, ParamList = new List<object> { 0, length } });
            return source;
        }

        /// <summary>
        ///     Partitioning a sequence IQueryable
        /// </summary>
        /// <param name="source">An System.Linq.IQueryable`1 to return the first element of</param>
        /// <param name="chunkSize">Quantity per piece</param>
        public static IEnumerable<IEnumerable<TSource>> Split<TSource>(this IQueryable<TSource> source, int chunkSize)
        {
            return source.ToList()
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value));
        }

        /// <summary>
        ///     Partitioning a sequence IEnumerable
        /// </summary>
        /// <param name="source">An IEnumerable to return the first element of</param>
        /// <param name="chunkSize">Quantity per piece</param>
        public static IEnumerable<IEnumerable<TSource>> Split<TSource>(this IEnumerable<TSource> source, int chunkSize)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value));
        }

        /// <summary>
        ///   Asynchronously  partitioning a sequence
        /// </summary>
        /// <param name="source">An System.Linq.IQueryable`1 to return the first element of</param>
        /// <param name="chunkSize">Quantity per piece</param>
        /// <param name="cancellationToken"></param>
        public static async Task<IEnumerable<IEnumerable<TSource>>> SplitAsync<TSource>(this IQueryable<TSource> source,
            int chunkSize, CancellationToken cancellationToken = default)
        {
            var res = await QueryableToListAsync(source, cancellationToken);
            return res.Split(chunkSize);
        }

        


        /// <summary>
        ///     Asynchronously enumerates the query results and performs the specified action
        ///     on each element.
        /// </summary>
        public static async Task ForEachAsync<TSource>(this IQueryable<TSource> source, Action<TSource> action,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, "source");
            Check.NotNull(action, "action");
            var res = await source.ToListAsync(cancellationToken);
            res.ForEach(action);
        }

        /// <summary>
        ///     Query to update table
        /// </summary>
        /// <param name="source">IQueryable</param>
        /// <param name="param">field-value dictionary</param>
        public static int Update<TSource>(this IQueryable<TSource> source,
            Expression<Func<TSource, Dictionary<object, object>>> param) where TSource : class
        {
            Check.NotNull(source, "source");
            ((ISqlComposite)source.Provider).ListCastExpression.Add(new ContainerCastExpression
            { CustomExpression = param, TypeEvolution = Evolution.Update });
            return source.Provider.Execute<int>(source.Expression);
        }

       



        /// <summary>
        ///     Query to update table
        /// </summary>
        /// <param name="source">IQueryable</param>
        /// <param name="param">field-value dictionary</param>
        public static Task<int> UpdateAsync<TSource>(this IQueryable<TSource> source,
            Expression<Func<TSource, Dictionary<object, object>>> param) where TSource : class
        {
            ((ISqlComposite)source.Provider).ListCastExpression.Add(new ContainerCastExpression
            { CustomExpression = param, TypeEvolution = Evolution.Update });
            return ((QueryProvider)source.Provider).ExecuteExtensionAsync<int>(source.Expression, null,
                CancellationToken.None);
        }

        /// <summary>
        ///     Executing an arbitrary query with parameters
        /// </summary>
        /// <param name="ses">Extension session</param>
        /// <param name="sql">Request string</param>
        /// <param name="param">Request parameters</param>
        /// <returns>IEnumerableTResult</returns>
        public static IEnumerable<TResult> FreeSql<TResult>(this ISession ses, string sql, params object[] param)
        {
            if(string.IsNullOrWhiteSpace(sql))  throw new ArgumentNullException(nameof(sql));
            var p = new V(sql);
            Expression callExpr = Expression.Call(
                Expression.Constant(p), p.GetType().GetMethod("FreeSql"));
            var db = new DbQueryProvider<TResult>((Session)ses);
            if (param != null && param.Length > 0) db.GetParamFree().AddRange(param);

            return (IEnumerable<TResult>)db.Execute<TResult>(callExpr);
        }
        /// <summary>
        ///   Executing an arbitrary query with parameters.
        ///   Suitable for anonymous types.
        /// </summary>
        /// <param name="ses">Extension session</param>
        /// <param name="temp">Type object, to get the type IEnumerable</param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static IEnumerable<TResult> FreeSqlAsTemplate<TResult>(this ISession ses, TResult temp, string sql, params object[] param)
        {
            var p = new V(sql);
            Expression callExpr = Expression.Call(
                Expression.Constant(p), p.GetType().GetMethod("FreeSql"));
            var db = new DbQueryProvider<TResult>((Session)ses);
            if (param != null && param.Length > 0) db.GetParamFree().AddRange(param);

            return (IEnumerable<TResult>)db.Execute<TResult>(callExpr);
        }

        /// <summary>
        ///     Executing an asynchronously  request with parameters
        ///     Suitable for anonymous types.
        /// </summary>
        /// <param name="ses">ISession</param>
        /// <param name="temp">Type object, to get the type IEnumerable</param>
        /// <param name="sql">Request string</param>
        /// <param name="param">Request parameters</param>
        public static async Task<IEnumerable<TResult>> FreeSqlAsTemplateAsync<TResult>(this ISession ses, TResult temp, string sql,
            params object[] param)
        {

            var p = new V(sql);
            Expression callExpr = Expression.Call(Expression.Constant(p), p.GetType().GetMethod("FreeSql"));
            var db = new DbQueryProvider<TResult>((Session)ses);
            if (param != null && param.Length > 0) db.GetParamFree().AddRange(param);
            var res = (IEnumerable<TResult>)await db.ExecuteAsync<TResult>(callExpr, null, CancellationToken.None);
            return res;
        }

        /// <summary>
        ///     Executing an asynchronously  request with parameters
        /// </summary>
        /// <param name="ses">ISession</param>
        /// <param name="sql">Request string</param>
        /// <param name="param">Request parameters</param>
        public static async Task<IEnumerable<TResult>> FreeSqlAsync<TResult>(this ISession ses, string sql,
            params object[] param)
        {

            var p = new V(sql);
            Expression callExpr = Expression.Call(Expression.Constant(p), p.GetType().GetMethod("FreeSql"));
            var db = new DbQueryProvider<TResult>((Session)ses);
            if (param != null && param.Length > 0) db.GetParamFree().AddRange(param);
            var res = (IEnumerable<TResult>)await db.ExecuteAsync<TResult>(callExpr, null, CancellationToken.None);
            return res;
        }


        /// <summary>
        ///     Calling a stored procedure
        /// </summary>
        /// <param name="ses">ISession</param>
        /// <param name="sql">Name procedure</param>   /// <typeparam name="TResult">Return type enumerable</typeparam>
        public static object ProcedureCall<TResult>(this ISession ses, string sql)
        {
            var p = new V(sql);
            Expression callExpr = Expression.Call(
                Expression.Constant(p), p.GetType().GetMethod("FreeSql"));
            return new DbQueryProvider<TResult>((Session)ses).ExecuteCall<TResult>(callExpr);
        }

        /// <summary>
        ///    Asynchronous calling a stored procedure
        /// </summary>
        /// <param name="ses">ISession</param>
        /// <param name="sql">Procedure name</param>
        /// <param name="cancellationToken">Object of the canceling to asynchronous operation</param>
        public static async Task<object> ProcedureCallAsync<TResult>(this ISession ses, string sql, CancellationToken cancellationToken = default)
        {
            var p = new V(sql);
            Expression callExpr = Expression.Call(Expression.Constant(p), p.GetType().GetMethod("FreeSql"));
            return await new DbQueryProvider<TResult>((Session)ses).ExecuteCallAsync<TResult>(callExpr, cancellationToken);
        }

        /// <summary>
        ///     Calling a stored procedure with parameters
        /// </summary>
        /// <param name="ses">ISession</param>
        /// <param name="sql">Procedure name</param>
        /// <param name="param">Request parameters</param>
        public static object ProcedureCallParam<TResult>(this ISession ses, string sql,
            params ParameterStoredPr[] param)
        {
            var p = new V(sql);
            Expression callExpr = Expression.Call(Expression.Constant(p), p.GetType().GetMethod("FreeSql"));
            return new DbQueryProvider<TResult>((Session)ses).ExecuteCallParam<TResult>(callExpr, param);
        }

        /// <summary>
        ///     Asynchronous calling a stored procedure
        /// </summary>
        /// <param name="ses">ISession</param>
        /// <param name="sql">Procedure name</param>
        /// <param name="param">Request parameters</param>
        /// <param name="cancellationToken">Object of the canceling to asynchronous operation</param>
        public static async Task<object> ProcedureCallParamAsync<TResult>(this ISession ses, string sql,
             ParameterStoredPr[] param, CancellationToken cancellationToken = default)
        {
            var p = new V(sql);
            Expression callExpr = Expression.Call(Expression.Constant(p), p.GetType().GetMethod("FreeSql"));
            return await new DbQueryProvider<TResult>((Session)ses).ExecuteCallParamAsync<TResult>(callExpr, param, cancellationToken);
        }

        /// <summary>
        ///     Getting List Asynchronous
        /// </summary>
        public static Task<List<TResult>> ToListAsync<TResult>(this IQueryable<TResult> source,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var res = ((QueryProvider)source.Provider).ExecuteToListAsync<TResult>(source.Expression,
                    cancellationToken);
                return res;
            }
            catch (Exception ex)
            {
                MySqlLogger.Info($" {Environment.NewLine}{source}{Environment.NewLine}{ex}");
                throw;
            }
        }

        /// <summary>
        ///     Creates System.Collections.Generic.List from IQueryable.(Executes a database query)
        /// </summary>
        public static List<TResult> ToList<TResult>(this IQueryable<TResult> source)
        {
            return (List<TResult>)((QueryProvider)source.Provider).Execute<TResult>(source.Expression);
        }

        /// <summary>
        ///     Getting Array Asynchronous
        /// </summary>
        public static async Task<TResult[]> ToArrayAsync<TResult>(this IQueryable<TResult> source,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var res = await ((QueryProvider)source.Provider).ExecuteToArray<TResult>(source.Expression,
                    cancellationToken);
                return res.ToArray();
            }
            catch (Exception ex)
            {
                MySqlLogger.Info($" {Environment.NewLine}{source}{Environment.NewLine}{ex}");
                throw;
            }
        }


        /// <summary>
        ///     Set command timeout for one request
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="value">>=0</param>
        /// <returns></returns>
        public static IQueryable<TSource> SetTimeOut<TSource>(this IQueryable<TSource> source, int value)
        {
            ((ISqlComposite)source.Provider).ListCastExpression.Add(new ContainerCastExpression
            { Timeout = value, TypeEvolution = Evolution.Timeout });
            return source;
        }










        /// <summary>
        ///     The BETWEEN operator selects values within a given range.
        /// </summary>
        /// <param name="source">An IEnumerable&lt;TSource&gt; whose elements to predicate between</param>
        /// <param name="func">property for predicate</param>
        /// <param name="left">Left value</param>
        /// <param name="right">Right value</param>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="T">Property type</typeparam>
        /// <returns></returns>
        public static IQueryable<TSource> Between<TSource, T>(this IQueryable<TSource> source,
            Expression<Func<TSource, T>> func, T left, T right)
        {
            ((ISqlComposite)source.Provider).ListCastExpression.Add(new ContainerCastExpression
            {
                TypeEvolution = Evolution.Between,
                CustomExpression = func,
                ParamList = new List<object>
                {
                    Expression.Constant(left, typeof(object)),
                    Expression.Constant(right, typeof(object))
                }
            });
            return source;
        }


        //   /// <summary>
        //   /// 
        //   /// </summary>
        //   public static IQueryable<TResult> Join<TOuter, TInner, TKey, TResult>(
        //       this IQueryable<TOuter> outer,
        //       IQueryable<TInner> inner,
        //       Expression<Func<TOuter, TKey>> outerKeySelector,
        //       Expression<Func<TInner, TKey>> innerKeySelector,
        //       Expression<Func<TOuter, TInner, TResult>> resultSelector)
        //   {
        //
        //       AttributesOfClass<TInner>.Init();
        //       return outer.Provider.CreateQuery<TResult>(Expression.Call(null,
        //           GetMethodInfo<IQueryable<TOuter>,
        //                   IQueryable<TInner>,
        //                   Expression<Func<TOuter, TKey>>,
        //                   Expression<Func<TInner, TKey>>,
        //                   Expression<Func<TOuter, TInner, TResult>>, IQueryable<TResult>>
        //               (Queryable.Join), outer.Expression, inner.Expression,
        //           Expression.Quote(outerKeySelector), Expression.Quote(innerKeySelector),
        //           Expression.Quote(resultSelector)));
        //   }
        //
        //   private static Expression GetSourceExpression<TSource>(IEnumerable<TSource> source) =>
        //       source is IQueryable<TSource> queryable
        //           ? queryable.Expression
        //           : (Expression)Expression.Constant((object)source, typeof(IEnumerable<TSource>));
        //


        /// <summary>
        ///     Projects each element of a sequence into a new form.
        /// </summary>
        /// <param name="source">A sequence of values to invoke a transform function on.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <returns>
        ///     An IEnumerable&lt;T&gt; whose elements are the result of invoking the transform function on each element of
        ///     source.
        /// </returns>
        public static IEnumerable<TResult> SelectCore<TSource, TResult>(
            this IQueryable<TSource> source,
            Func<TSource, TResult> selector)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(selector, nameof(selector));
            var d = ((Query<TSource>)source).ToList();
            foreach (var source1 in d) yield return selector(source1);
        }



        private static async Task<IEnumerable<TResult>> QueryableToListAsync<TSource, TResult>(
            Func<TSource, TResult> selector, IQueryable proxySource, CancellationToken cancellationToken)
        {
            var list = new List<TResult>();
            var ss = await ((Query<TSource>)proxySource).ToListAsync(cancellationToken);
            ss.ForEach(a => { list.Add(selector(a)); });
            return list;
        }

        private static async Task<IEnumerable<TResult>> QueryableSelectorToListIntAsync<TSource, TResult>(Func<TSource, int,
            TResult> selector, IQueryable proxySource, CancellationToken cancellationToken)
        {
            var list = new List<TResult>();
            var ss = await ((Query<TSource>)proxySource).ToListAsync(cancellationToken);
            var index = -1;
            ss.ForEach(a =>
            {
                checked
                {
                    ++index;
                }

                list.Add(selector(a, index));
            });
            return list;
        }

        static async Task<List<T>> QueryableToListAsync<T>(this IQueryable<T> source, CancellationToken cancellationToken = default)
        {
            return await source.Provider.ExecuteAsync<List<T>>(source.Expression, cancellationToken);
        }

        /// <summary>
        ///     Asynchronous  returns the input typed as IEnumerable&lt;T&gt;.
        /// </summary>
        /// <param name="source">The sequence to type as IQueryable&lt;T&gt;.</param>
        /// <param name="cancellationToken">Object of the canceling to asynchronous operation</param>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <returns>IEnumerable&lt;TSource&gt;</returns>
        public static async Task<IEnumerable<TSource>> AsEnumerableAsync<TSource>(
            this IQueryable<TSource> source, CancellationToken cancellationToken = default)
        {
            var res = await QueryableToListAsync(source, cancellationToken);
            return res;
        }

        #region Select

        /// <summary>
        ///     Asynchronous projects each element of a sequence into a new form.
        /// </summary>
        /// <param name="source">A sequence of values to invoke a transform function on.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <param name="cancellationToken">Object of the canceling to asynchronous operation</param>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <returns>
        ///     An IEnumerable&lt;T&gt; whose elements are the result of invoking the transform function on each element of
        ///     source.
        /// </returns>
        public static async Task<IEnumerable<TResult>> SelectCoreAsync<TSource, TResult>(
            this IQueryable<TSource> source,
            Func<TSource, TResult> selector,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(selector, nameof(selector));
            return await QueryableToListAsync(selector, source, cancellationToken);
        }

        /// <summary>
        ///     Projects each element of a sequence into a new form by incorporating the element's index.
        /// </summary>
        /// <param name="source">A sequence of values to invoke a transform function on.</param>
        /// <param name="selector">
        ///     A transform function to apply to each source element;
        ///     the second parameter of the function represents the index of the source element.
        /// </param>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static IEnumerable<TResult> SelectCore<TSource, TResult>(
            this IQueryable<TSource> source,
            Func<TSource, int, TResult> selector)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(selector, nameof(selector));
            var d = ((Query<TSource>)source).ToList();
            var index = -1;
            foreach (var source1 in d)
            {
                checked
                {
                    ++index;
                }

                yield return selector(source1, index);
            }
        }

        /// <summary>
        ///     Asynchronous  projects of a sequence into a new form by incorporating the element's index.
        /// </summary>
        /// <param name="source">A sequence of values to invoke a transform function on.</param>
        /// <param name="selector">
        ///     A transform function to apply to each source element;
        ///     the second parameter of the function represents the index of the source element.
        /// </param>
        /// <param name="cancellationToken">Object of the canceling to asynchronous operation</param>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <returns>
        ///     An IEnumerable&lt;T&gt; whose elements are the result of invoking the transform function on each element of
        ///     source.
        /// </returns>
        public static async Task<IEnumerable<TResult>> SelectCoreAsync<TSource, TResult>(
            this IQueryable source,
            Func<TSource, int, TResult> selector,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(selector, nameof(selector));
            return await QueryableSelectorToListIntAsync(selector, source, cancellationToken);
        }

        #endregion


        #region AggregateCore

        /// <summary>
        ///     Applies an accumulator function over a sequence.
        /// </summary>
        /// <param name="source">An IQueryable&lt;T&gt; to aggregate over.</param>
        /// <param name="func">An accumulator function to be invoked on each element.</param>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <returns>The final accumulator value.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static TSource AggregateCore<TSource>(
            this IQueryable<TSource> source,
            Func<TSource, TSource, TSource> func)
        {
            Check.NotNull(source, nameof(source));
            using (var enumerator = source.GetEnumerator())
            {
                var source1 = enumerator.MoveNext()
                    ? enumerator.Current
                    : throw new InvalidOperationException(" Element Empty");
                while (enumerator.MoveNext())
                    source1 = func(source1, enumerator.Current);
                return source1;
            }
        }

        /// <summary>
        ///     Asynchronous applies an accumulator function over a sequence.
        /// </summary>
        /// <param name="source">An IQueryable&lt;T&gt; to aggregate over.</param>
        /// <param name="func">An accumulator function to be invoked on each element.</param>
        /// <param name="cancellationToken">Object of the canceling to asynchronous operation</param>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <returns>The final accumulator value.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static async Task<TSource> AggregateCoreAsync<TSource>(
            this IQueryable<TSource> source,
            Func<TSource, TSource, TSource> func, CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(func, nameof(func));
            var list = await QueryableToListAsync(source, cancellationToken);
            using (IEnumerator<TSource> enumerator = list.GetEnumerator())
            {
                var source1 = enumerator.MoveNext()
                    ? enumerator.Current
                    : throw new InvalidOperationException(" Element Empty");
                while (enumerator.MoveNext())
                    source1 = func(source1, enumerator.Current);
                return source1;
            }
        }


        /// <summary>
        ///     Applies an accumulator function over a sequence. The specified seed value is used as the initial accumulator value.
        /// </summary>
        /// <param name="source">An IQueryable&lt;T&gt; to aggregate over.</param>
        /// <param name="seed">The initial accumulator value.</param>
        /// <param name="func">An accumulator function to be invoked on each element.</param>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TAccumulate"></typeparam>
        /// <returns>The final accumulator value.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static TAccumulate AggregateCore<TSource, TAccumulate>(
            this IQueryable<TSource> source,
            TAccumulate seed,
            Func<TAccumulate, TSource, TAccumulate> func)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(func, nameof(func));
            var accumulate = seed;
            foreach (var source1 in source)
                accumulate = func(accumulate, source1);
            return accumulate;
        }


        /// <summary>
        ///     Asynchronous applies an accumulator function over a sequence. The specified seed value is used as the initial
        ///     accumulator value.
        /// </summary>
        /// <param name="source">An IQueryable&lt;T&gt; to aggregate over.</param>
        /// <param name="seed">The initial accumulator value.</param>
        /// <param name="func">An accumulator function to be invoked on each element.</param>
        /// <param name="cancellationToken">Object of the canceling to asynchronous operation</param>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TAccumulate"></typeparam>
        /// <returns>The final accumulator value.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static async Task<TAccumulate> AggregateCoreAsync<TSource, TAccumulate>(
            this IQueryable<TSource> source,
            TAccumulate seed,
            Func<TAccumulate,
                TSource, TAccumulate> func,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(func, nameof(func));
            var accumulate = seed;
            var res = await QueryableToListAsync(source, cancellationToken);

            foreach (var source1 in res)
                accumulate = func(accumulate, source1);
            return accumulate;
        }

        /// <summary>
        ///     Applies an accumulator function over a sequence.
        ///     The specified seed value is used as the initial accumulator value,
        ///     and the specified function is used to select the result value.
        /// </summary>
        /// <param name="source">An IQueryable&lt;T&gt; to aggregate over.</param>
        /// <param name="seed">The initial accumulator value.</param>
        /// <param name="func">An accumulator function to be invoked on each element.</param>
        /// <param name="resultSelector">A function to transform the final accumulator value into the result value.</param>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TAccumulate">The type of the accumulator value.</typeparam>
        /// <typeparam name="TResult">The type of the resulting value.</typeparam>
        /// <returns>The transformed final accumulator value.</returns>
        public static TResult AggregateCore<TSource, TAccumulate, TResult>(
            this IQueryable<TSource> source,
            TAccumulate seed,
            Func<TAccumulate, TSource, TAccumulate> func,
            Func<TAccumulate, TResult> resultSelector)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(func, nameof(func));
            Check.NotNull(resultSelector, nameof(resultSelector));
            var accumulate = seed;
            foreach (var source1 in source)
                accumulate = func(accumulate, source1);
            return resultSelector(accumulate);
        }


        /// <summary>
        ///     Asynchronous applies an accumulator function over a sequence.
        ///     The specified seed value is used as the initial accumulator value,
        ///     and the specified function is used to select the result value.
        /// </summary>
        /// <param name="source">An IQueryable&lt;T&gt; to aggregate over.</param>
        /// <param name="seed">The initial accumulator value.</param>
        /// <param name="func">An accumulator function to be invoked on each element.</param>
        /// <param name="resultSelector">A function to transform the final accumulator value into the result value.</param>
        /// <param name="cancellationToken">Object of the canceling to asynchronous operation</param>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TAccumulate">The type of the accumulator value.</typeparam>
        /// <typeparam name="TResult">The type of the resulting value.</typeparam>
        /// <returns>The transformed final accumulator value.</returns>
        public static async Task<TResult> AggregateCoreAsync<TSource, TAccumulate, TResult>(
            this IQueryable<TSource> source,
            TAccumulate seed,
            Func<TAccumulate, TSource, TAccumulate> func,
            Func<TAccumulate, TResult> resultSelector,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(func, nameof(func));
            Check.NotNull(resultSelector, nameof(resultSelector));
            var res = await QueryableToListAsync(source, cancellationToken);
            var accumulate = seed;
            foreach (var source1 in res)
                accumulate = func(accumulate, source1);
            return resultSelector(accumulate);
        }

        #endregion


        #region CastCore

        /// <summary>
        ///     Casts the elements of an IQueryable to the specified type.
        /// </summary>
        /// <param name="source">The IQueryable that contains the elements to be cast to type TResult.</param>
        /// <typeparam name="TResult">The type to cast the elements of source to.</typeparam>
        /// <returns>IEnumerable&lt;TResult&gt;</returns>
        public static IEnumerable<TResult> CastCore<TResult>(this IQueryable source)
        {
            Check.NotNull(source, nameof(source));
            var provider = (QueryProvider)source.Provider;
            IEnumerable<object> t1 = (IEnumerable<object>)provider.Execute<IEnumerable<object>>(source.Expression);
            return t1.Cast<TResult>();
        }


        /// <summary>
        ///     Asynchronous casts the elements of an IQueryable to the specified type.
        /// </summary>
        /// <param name="source">The IQueryable that contains the elements to be cast to type TResult.</param>
        /// <param name="cancellationToken">Object of the canceling to asynchronous operation</param>
        /// <typeparam name="TResult">The type to cast the elements of source to.</typeparam>
        /// <returns>IEnumerable&lt;TResult&gt;</returns>
        public static async Task<IEnumerable<TResult>> CastCoreAsync<TResult>(this IQueryable source,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));
            var provider = (QueryProvider)source.Provider;
            IEnumerable<object> t1 = await provider.ExecuteAsync<IEnumerable<object>>(source.Expression,cancellationToken);
            return t1.Cast<TResult>();
        }

        #endregion

        #region ExceptCore

        /// <summary>
        ///     Produces the set difference of two sequences by using the default equality comparer to compare values of primary
        ///     key.
        /// </summary>
        /// <param name="first">An IQueryable&lt;T&gt; whose elements that are not also in second will be returned.</param>
        /// <param name="second">
        ///     An IQueryable&lt;T&gt; whose elements that also occur in the first sequence will
        ///     cause those elements to be removed from the returned sequence.
        /// </param>
        /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
        /// <returns>
        ///     IEnumerable&lt;TSource&gt;
        ///     A sequence that contains the set difference of the elements of two sequences.
        /// </returns>
        public static IEnumerable<TSource> ExceptCore<TSource>(
            this IQueryable<TSource> first,
            IQueryable<TSource> second)
        {
            Check.NotNull(first, nameof(first));
            Check.NotNull(second, nameof(second));
            var w = new Sweetmeat<TSource, TSource>(first, second);
            w.Wait();
            return ExceptIterator(w.First, w.Seconds, null);
        }


        /// <summary>
        ///     Produces the set difference of two sequences by using the specified IEqualityComparer&lt;T&gt; to compare values.
        /// </summary>
        /// <param name="first">An IQueryable&lt;T&gt; whose elements that are not also in second will be returned.</param>
        /// <param name="second">
        ///     An IQueryable&lt;T&gt; whose elements that also occur in the first sequence will
        ///     cause those elements to be removed from the returned sequence.
        /// </param>
        /// <param name="comparer">An IEqualityComparer&lt;T&gt; to compare values.</param>
        /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
        /// <returns>
        ///     IEnumerable&lt;TSource&gt;
        ///     A sequence that contains the set difference of the elements of two sequences.
        /// </returns>
        public static IEnumerable<TSource> ExceptCore<TSource>(
            this IQueryable<TSource> first,
            IQueryable<TSource> second,
            IEqualityComparer<TSource> comparer) where TSource : class
        {
            Check.NotNull(first, nameof(first));
            Check.NotNull(second, nameof(second));
            var w = new Sweetmeat<TSource, TSource>(first, second);
            w.Wait();
            return ExceptIterator(w.First, w.Seconds, comparer);
        }

        /// <summary>
        ///     Produces the set difference of two sequences by using the default equality comparer to compare values of primary
        ///     key.
        /// </summary>
        /// <param name="first">An IQueryable&lt;T&gt; whose elements that are not also in second will be returned.</param>
        /// <param name="second">
        ///     An IEnumerable&lt;T&gt; whose elements that also occur in the first sequence will
        ///     cause those elements to be removed from the returned sequence.
        /// </param>
        /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
        /// <returns>
        ///     IEnumerable&lt;TSource&gt;
        ///     A sequence that contains the set difference of the elements of two sequences.
        /// </returns>
        public static IEnumerable<TSource> ExceptCore<TSource>(
            this IQueryable<TSource> first,
            IEnumerable<TSource> second) where TSource : class
        {
            Check.NotNull(first, nameof(first));
            Check.NotNull(second, nameof(second));
            return ExceptIterator(first, second, null);
        }


        /// <summary>
        ///     Produces the set difference of two sequences by using the specified IEqualityComparer&lt;T&gt; to compare values.
        /// </summary>
        /// <param name="first">An IQueryable&lt;T&gt; whose elements that are not also in second will be returned.</param>
        /// <param name="second">
        ///     An IEnumerable&lt;T&gt; whose elements that also occur in the first sequence will
        ///     cause those elements to be removed from the returned sequence.
        /// </param>
        /// <param name="comparer">An IEqualityComparer&lt;T&gt; to compare values.</param>
        /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
        /// <returns>
        ///     IEnumerable&lt;TSource&gt;
        ///     A sequence that contains the set difference of the elements of two sequences.
        /// </returns>
        public static IEnumerable<TSource> ExceptCore<TSource>(
            this IQueryable<TSource> first,
            IEnumerable<TSource> second,
            IEqualityComparer<TSource> comparer) where TSource : class
        {
            Check.NotNull(first, nameof(first));
            Check.NotNull(second, nameof(second));
            Check.NotNull(comparer, nameof(comparer));
            return ExceptIterator(first, second, comparer);
        }

        private static IEnumerable<TSource> ExceptIterator<TSource>(
            IEnumerable<TSource> first,
            IEnumerable<TSource> second,
            IEqualityComparer<TSource> comparer)
        {
            Check.NotNull(first, nameof(first));
            Check.NotNull(second, nameof(second));
            Check.NotNull(comparer, nameof(comparer));
            if (UtilsCore.IsValid<TSource>() && comparer == null)
            {
                var set = new MySet<TSource>();
                foreach (var source in second)
                    set.Add(source);
                foreach (var source in first)
                    if (set.Add(source))
                        yield return source;
            }
            else
            {
                var set = new Set<TSource>(comparer);
                foreach (var source in second)
                    set.Add(source);
                foreach (var source in first)
                    if (set.Add(source))
                        yield return source;
            }
        }


        /// <summary>
        ///     Asynchronous produces the set difference of two sequences by using the default equality comparer to compare values
        ///     of primary key.
        /// </summary>
        /// <param name="first">An IQueryable&lt;T&gt; whose elements that are not also in second will be returned.</param>
        /// <param name="second">
        ///     An IQueryable&lt;T&gt; whose elements that also occur in the first sequence will
        ///     cause those elements to be removed from the returned sequence.
        /// </param>
        /// <param name="cancellationToken">Object of the canceling to asynchronous operation</param>
        /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
        /// <returns>
        ///     IEnumerable&lt;TSource&gt;
        ///     A sequence that contains the set difference of the elements of two sequences.
        /// </returns>
        public static async Task<IEnumerable<TSource>> ExceptCoreAsync<TSource>(
            this IQueryable<TSource> first,
            IQueryable<TSource> second,
            CancellationToken cancellationToken = default) where TSource : class
        {
            Check.NotNull(first, nameof(first));
            Check.NotNull(second, nameof(second));
            var w = new Sweetmeat<TSource, TSource>(first, second, cancellationToken);
            await w.WaitAsync();
            return ExceptIterator(w.First, w.Seconds, null);
        }

        /// <summary>
        ///     Asynchronous produces the set difference of two sequences by using the specified IEqualityComparer&lt;T&gt; to
        ///     compare values.
        /// </summary>
        /// <param name="first">An IQueryable&lt;T&gt; whose elements that are not also in second will be returned.</param>
        /// <param name="second">
        ///     An IQueryable&lt;T&gt; whose elements that also occur in the first sequence will
        ///     cause those elements to be removed from the returned sequence.
        /// </param>
        /// <param name="comparer">An IEqualityComparer&lt;T&gt; to compare values.</param>
        /// <param name="cancellationToken">Object of the canceling to asynchronous operation</param>
        /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
        /// <returns>
        ///     IEnumerable&lt;TSource&gt;
        ///     A sequence that contains the set difference of the elements of two sequences.
        /// </returns>
        public static async Task<IEnumerable<TSource>> ExceptCoreAsync<TSource>(
            this IQueryable<TSource> first,
            IQueryable<TSource> second,
            IEqualityComparer<TSource> comparer,
            CancellationToken cancellationToken = default) where TSource : class
        {
            Check.NotNull(first, nameof(first));
            Check.NotNull(second, nameof(second));
            Check.NotNull(comparer, nameof(comparer));
            var w = new Sweetmeat<TSource, TSource>(first, second, cancellationToken);
            await w.WaitAsync();

            return ExceptIterator(w.First, w.Seconds, comparer);
        }


        /// <summary>
        ///     Asynchronous produces the set difference of two sequences by using the default equality comparer to compare values
        ///     of primary key.
        /// </summary>
        /// <param name="first">An IQueryable&lt;T&gt; whose elements that are not also in second will be returned.</param>
        /// <param name="second">
        ///     An IEnumerable&lt;T&gt; whose elements that also occur in the first sequence will
        ///     cause those elements to be removed from the returned sequence.
        /// </param>
        /// <param name="cancellationToken">Object of the canceling to asynchronous operation</param>
        /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
        /// <returns>
        ///     IEnumerable&lt;TSource&gt;
        ///     A sequence that contains the set difference of the elements of two sequences.
        /// </returns>
        public static async Task<IEnumerable<TSource>> ExceptCoreAsync<TSource>(
            this IQueryable<TSource> first,
            IEnumerable<TSource> second,
            CancellationToken cancellationToken = default) where TSource : class
        {
            Check.NotNull(first, nameof(first));
            Check.NotNull(second, nameof(second));
            var res = await QueryableToListAsync(first, cancellationToken);
            return ExceptIterator(res, second, null);
        }


        /// <summary>
        ///     Asynchronous produces the set difference of two sequences by using the specified IEqualityComparer&lt;T&gt; to
        ///     compare values.
        /// </summary>
        /// <param name="first">An IQueryable&lt;T&gt; whose elements that are not also in second will be returned.</param>
        /// <param name="second">
        ///     An IEnumerable&lt;T&gt; whose elements that also occur in the first sequence will
        ///     cause those elements to be removed from the returned sequence.
        /// </param>
        /// <param name="comparer">An IEqualityComparer&lt;T&gt; to compare values.</param>
        /// <param name="cancellationToken">Object of the canceling to asynchronous operation</param>
        /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
        /// <returns>
        ///     IEnumerable&lt;TSource&gt;
        ///     A sequence that contains the set difference of the elements of two sequences.
        /// </returns>
        public static async Task<IEnumerable<TSource>> ExceptCoreAsync<TSource>(
            this IQueryable<TSource> first,
            IEnumerable<TSource> second,
            IEqualityComparer<TSource> comparer,
            CancellationToken cancellationToken = default) where TSource : class
        {
            Check.NotNull(first, nameof(first));
            Check.NotNull(second, nameof(second));
            Check.NotNull(comparer, nameof(comparer));
            var res = await QueryableToListAsync(first, cancellationToken);
            return ExceptIterator(res, second, comparer);
        }

        #endregion

        #region ConcatCore

        /// <summary>
        ///     Concatenates two sequences.
        /// </summary>
        /// <param name="first">The first sequence to concatenate.</param>
        /// <param name="second">The sequence to concatenate to the first sequence.</param>
        /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
        /// <returns>An IEnumerable&lt;T&gt; that contains the concatenated elements of the two input sequences.</returns>
        public static IEnumerable<TSource> ConcatCore<TSource>(
            this IQueryable<TSource> first,
            IQueryable<TSource> second) where TSource : class
        {
            Check.NotNull(first, nameof(first));
            Check.NotNull(second, nameof(second));
            var w = new Sweetmeat<TSource, TSource>(first, second);
            w.Wait();
            return ConcatIterator(w.First, w.Seconds);
        }


        /// <summary>
        ///     Asynchronous concatenates two sequences.
        /// </summary>
        /// <param name="first">The first sequence to concatenate.</param>
        /// <param name="second">The sequence to concatenate to the first sequence.</param>
        /// <param name="cancellationToken">Object of the canceling to asynchronous operation</param>
        /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
        /// <returns>An IEnumerable&lt;T&gt; that contains the concatenated elements of the two input sequences.</returns>
        public static async Task<IEnumerable<TSource>> ConcatCoreAsync<TSource>(
            this IQueryable<TSource> first,
            IQueryable<TSource> second,
            CancellationToken cancellationToken = default) where TSource : class
        {
            Check.NotNull(first, nameof(first));
            Check.NotNull(second, nameof(second));
            var w = new Sweetmeat<TSource, TSource>(first, second, cancellationToken);
            await w.WaitAsync();
            return ConcatIterator(w.First, w.Seconds);
        }


        /// <summary>
        ///     Concatenates two sequences.
        /// </summary>
        /// <param name="first">The first sequence to concatenate.</param>
        /// <param name="second">The sequence to concatenate to the first sequence.</param>
        /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
        /// <returns>An IEnumerable&lt;T&gt; that contains the concatenated elements of the two input sequences.</returns>
        public static IEnumerable<TSource> ConcatCore<TSource>(
            this IQueryable<TSource> first,
            IEnumerable<TSource> second) where TSource : class
        {
            Check.NotNull(first, nameof(first));
            Check.NotNull(second, nameof(second));
            return ConcatIterator(first, second);
        }

        /// <summary>
        ///     Asynchronous concatenates two sequences.
        /// </summary>
        /// <param name="first">The first sequence to concatenate.</param>
        /// <param name="second">The sequence to concatenate to the first sequence.</param>
        /// <param name="cancellationToken">Object of the canceling to asynchronous operation</param>
        /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
        /// <returns>An IEnumerable&lt;T&gt; that contains the concatenated elements of the two input sequences.</returns>
        public static async Task<IEnumerable<TSource>> ConcatCoreAsync<TSource>(
            this IQueryable<TSource> first,
            IEnumerable<TSource> second, CancellationToken cancellationToken = default) where TSource : class
        {
            Check.NotNull(first, nameof(first));
            Check.NotNull(second, nameof(second));
            var res = await QueryableToListAsync(first, cancellationToken);
            return ConcatIterator(res, second);
        }

        private static IEnumerable<TSource> ConcatIterator<TSource>(
            IEnumerable<TSource> first,
            IEnumerable<TSource> second)
        {
            foreach (var source in first)
                yield return source;
            foreach (var source in second)
                yield return source;
        }

        #endregion

        #region SelectMany

        /// <summary>
        ///     Projects each element of a sequence to an IEnumerable&lt;T&gt;
        ///     and flattens the resulting sequences into one sequence.
        /// </summary>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TResult">The type of the elements of the sequence returned by selector.</typeparam>
        /// <returns>
        ///     An IEnumerable&lt;T&gt; whose elements are the result
        ///     of invoking the one-to-many transform function on each element of the input sequence.
        /// </returns>
        public static IEnumerable<TResult> SelectManyCore<TSource, TResult>(this IQueryable<TSource> source,
            Func<TSource, IEnumerable<TResult>> selector)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(selector, nameof(selector));
            return SelectManyIterator(source, selector);
        }

        /// <summary>
        ///     Asynchronous projects each element of a sequence to an IEnumerable&lt;T&gt;
        ///     and flattens the resulting sequences into one sequence.
        /// </summary>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <param name="cancellationToken">Object of the canceling to asynchronous operation</param>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TResult">The type of the elements of the sequence returned by selector.</typeparam>
        /// <returns>
        ///     An IEnumerable&lt;T&gt; whose elements are the result
        ///     of invoking the one-to-many transform function on each element of the input sequence.
        /// </returns>
        public static async Task<IEnumerable<TResult>> SelectManyCoreAsync<TSource, TResult>(
            this IQueryable<TSource> source,
            Func<TSource, IEnumerable<TResult>> selector,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(selector, nameof(selector));
            var res = await QueryableToListAsync(source, cancellationToken);
            return SelectManyIterator(res, selector);
        }

        private static IEnumerable<TResult> SelectManyIterator<TSource, TResult>(IEnumerable<TSource> source,
            Func<TSource, IEnumerable<TResult>> selector)
        {
            foreach (var item in source)
                foreach (var item2 in selector(item))
                    yield return item2;
        }

        private static IEnumerable<TResult> SelectManyIterator<TSource, TResult>(IEnumerable<TSource> source,
            Func<TSource, int, IEnumerable<TResult>> selector)
        {
            var index = -1;
            foreach (var item in source)
            {
                index = checked(index + 1);
                foreach (var item2 in selector(item, index)) yield return item2;
            }
        }

        private static IEnumerable<TResult> SelectManyIterator<TSource, TCollection, TResult>(
            IEnumerable<TSource> source, Func<TSource, int, IEnumerable<TCollection>> collectionSelector,
            Func<TSource, TCollection, TResult> resultSelector)
        {
            var index = -1;
            foreach (var element in source)
            {
                index = checked(index + 1);
                foreach (var item in collectionSelector(element, index)) yield return resultSelector(element, item);
            }
        }

        private static IEnumerable<TResult> SelectManyIterator<TSource, TCollection, TResult>(
            IEnumerable<TSource> source, Func<TSource, IEnumerable<TCollection>> collectionSelector,
            Func<TSource, TCollection, TResult> resultSelector)
        {
            foreach (var element in source)
                foreach (var item in collectionSelector(element))
                    yield return resultSelector(element, item);
        }


        /// <summary>
        ///     Projects each element of a sequence to an IEnumerable&lt;T&gt;, and flattens the resulting sequences into one
        ///     sequence.
        ///     The index of each source element is used in the projected form of that element.
        /// </summary>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="selector">
        ///     A transform function to apply to each source element;
        ///     the second parameter of the function represents the index of the source element.
        /// </param>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TResult">The type of the elements of the sequence returned by selector.</typeparam>
        /// <returns>
        ///     An IEnumerable&lt;T&gt; whose elements are the result of invoking the
        ///     one-to-many transform function on each element of an input sequence.
        /// </returns>
        public static IEnumerable<TResult> SelectManyCore<TSource, TResult>(
            this IQueryable<TSource> source,
            Func<TSource, int, IEnumerable<TResult>> selector)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(selector, nameof(selector));
            return SelectManyIterator(source, selector);
        }

        /// <summary>
        ///     Asynchronous projects each element of a sequence to an IEnumerable&lt;T&gt;, and flattens the resulting sequences
        ///     into one sequence.
        ///     The index of each source element is used in the projected form of that element.
        /// </summary>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="selector">
        ///     A transform function to apply to each source element;
        ///     the second parameter of the function represents the index of the source element.
        /// </param>
        /// <param name="cancellationToken">Object of the canceling to asynchronous operation</param>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TResult">The type of the elements of the sequence returned by selector.</typeparam>
        /// <returns>
        ///     An IEnumerable&lt;T&gt; whose elements are the result of invoking the
        ///     one-to-many transform function on each element of an input sequence.
        /// </returns>
        public static async Task<IEnumerable<TResult>> SelectManyCoreAsync<TSource, TResult>(
            this IQueryable<TSource> source,
            Func<TSource, int, IEnumerable<TResult>> selector,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(selector, nameof(selector));
            var res = await QueryableToListAsync(source, cancellationToken);
            return SelectManyIterator(res, selector);
        }


        /// <summary>
        ///     Projects each element of a sequence to an IEnumerable&lt;T&gt;,
        ///     flattens the resulting sequences into one sequence, and invokes a result selector function on each element therein.
        /// </summary>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="collectionSelector">A transform function to apply to each element of the input sequence.</param>
        /// <param name="resultSelector">A transform function to apply to each element of the intermediate sequence.</param>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TCollection">The type of the intermediate elements collected by collectionSelector.</typeparam>
        /// <typeparam name="TResult">The type of the elements of the resulting sequence.</typeparam>
        /// <returns>
        ///     An IEnumerable&lt;T&gt; whose elements are the result of invoking the one-to-many transform function
        ///     collectionSelector on each element of
        ///     source and then mapping each of those sequence elements and
        ///     their corresponding source element to a result element.
        /// </returns>
        public static IEnumerable<TResult> SelectManyCore<TSource, TCollection, TResult>(
            this IQueryable<TSource> source,
            Func<TSource, int, IEnumerable<TCollection>> collectionSelector,
            Func<TSource, TCollection, TResult> resultSelector)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(collectionSelector, nameof(collectionSelector));
            Check.NotNull(resultSelector, nameof(resultSelector));
            return SelectManyIterator(source, collectionSelector, resultSelector);
        }

        /// <summary>
        ///     Asynchronous projects each element of a sequence to an IEnumerable&lt;T&gt;,
        ///     flattens the resulting sequences into one sequence, and invokes a result selector function on each element therein.
        /// </summary>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="collectionSelector">A transform function to apply to each element of the input sequence.</param>
        /// <param name="resultSelector">A transform function to apply to each element of the intermediate sequence.</param>
        /// <param name="cancellationToken">Object of the canceling to asynchronous operation</param>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TCollection">The type of the intermediate elements collected by collectionSelector.</typeparam>
        /// <typeparam name="TResult">The type of the elements of the resulting sequence.</typeparam>
        /// <returns>
        ///     An IEnumerable&lt;T&gt; whose elements are the result of invoking the one-to-many transform function
        ///     collectionSelector on each element of
        ///     source and then mapping each of those sequence elements and
        ///     their corresponding source element to a result element.
        /// </returns>
        public static async Task<IEnumerable<TResult>> SelectManyCoreAsync<TSource, TCollection, TResult>(
            this IQueryable<TSource> source,
            Func<TSource, int, IEnumerable<TCollection>> collectionSelector,
            Func<TSource, TCollection, TResult> resultSelector, CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(collectionSelector, nameof(collectionSelector));
            Check.NotNull(resultSelector, nameof(resultSelector));
            var res = await QueryableToListAsync(source, cancellationToken);
            return SelectManyIterator(res, collectionSelector, resultSelector);
        }


        /// <summary>
        ///     Projects each element of a sequence to an IEnumerable&lt;T&gt;, flattens the resulting sequences into one sequence,
        ///     and invokes a result selector function on each element therein.
        ///     The index of each source element is used in the intermediate projected form of that element.
        /// </summary>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="collectionSelector">
        ///     A transform function to apply to each source element;
        ///     the second parameter of the function represents the index of the source element.
        /// </param>
        /// <param name="resultSelector">A transform function to apply to each element of the intermediate sequence.</param>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TCollection">The type of the intermediate elements collected by collectionSelector.</typeparam>
        /// <typeparam name="TResult">The type of the elements of the resulting sequence.</typeparam>
        /// <returns>
        ///     An IEnumerable&lt;T&gt; whose elements are the result of invoking the one-to-many
        ///     transform function collectionSelector on each element of source and then mapping each of
        ///     those sequence elements and their corresponding source element to a result element.
        /// </returns>
        public static IEnumerable<TResult> SelectManyCore<TSource, TCollection, TResult>(
            this IQueryable<TSource> source, Func<TSource, IEnumerable<TCollection>> collectionSelector,
            Func<TSource, TCollection, TResult> resultSelector)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(collectionSelector, nameof(collectionSelector));
            Check.NotNull(resultSelector, nameof(resultSelector));
            return SelectManyIterator(source, collectionSelector, resultSelector);
        }

        /// <summary>
        ///     Asynchronous projects each element of a sequence to an IEnumerable&lt;T&gt;, flattens the resulting sequences into
        ///     one sequence,
        ///     and invokes a result selector function on each element therein.
        ///     The index of each source element is used in the intermediate projected form of that element.
        /// </summary>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="collectionSelector">
        ///     A transform function to apply to each source element;
        ///     the second parameter of the function represents the index of the source element.
        /// </param>
        /// <param name="resultSelector">A transform function to apply to each element of the intermediate sequence.</param>
        /// <param name="cancellationToken">Object of the canceling to asynchronous operation</param>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TCollection">The type of the intermediate elements collected by collectionSelector.</typeparam>
        /// <typeparam name="TResult">The type of the elements of the resulting sequence.</typeparam>
        /// <returns>
        ///     An IEnumerable&lt;T&gt; whose elements are the result of invoking the one-to-many
        ///     transform function collectionSelector on each element of source and then mapping each of
        ///     those sequence elements and their corresponding source element to a result element.
        /// </returns>
        public static async Task<IEnumerable<TResult>> SelectManyCoreAsync<TSource,
            TCollection, TResult>(this IQueryable<TSource> source,
            Func<TSource, IEnumerable<TCollection>> collectionSelector,
            Func<TSource, TCollection, TResult> resultSelector,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(collectionSelector, nameof(collectionSelector));
            Check.NotNull(resultSelector, nameof(resultSelector));
            var res = await QueryableToListAsync(source, cancellationToken);
            return SelectManyIterator(res, collectionSelector, resultSelector);
        }

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
 
    public class PairUpdate
    {
        /// <summary>
        /// 
        /// </summary>
        public object Left { get; }
        /// <summary>
        /// 
        /// </summary>
        public object Richt { get; }

       
         /// <summary>
         /// 
         /// </summary>
         /// <param name="left"></param>
         /// <param name="richt"></param>
         public PairUpdate(object left, object  richt)
         {
             Left = left;
             Richt = richt;
         }
       
    }
}