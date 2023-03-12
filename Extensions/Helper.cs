using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ORM_1_21_.Linq;
using ORM_1_21_.Utils;

namespace ORM_1_21_.Extensions
{

    /// <summary>
    /// Extension ORM
    /// </summary>
    public static partial class Helper
    {

        /// <summary>
        ///  Iterating over a collection
        /// </summary>
        public static void  ForEach<TSource>(this IQueryable<TSource> source, Action<TSource> action)
        {
            Query<TSource> res = (Query<TSource>)source;
            res.ForEach(action);
        }

        /// <summary>
        ///Convert date to SQL format
        /// </summary>
        public static string DataToString(this ISession ses, DateTime dateTime)
        {
            return Configure.Utils.DateToString(dateTime);
        }

        /// <summary>
        /// Array of non-recurring values, by selected field
        /// </summary>
        public static IEnumerable<TResult> Distinct<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TResult>> exp) where TSource : class
        {
            ((ISqlComposite)source.Provider).ListCastExpression.Add(new ContainerCastExpression
            { CastomExpression = exp, TypeRevalytion = Evolution.DistinctCore, TypeRetyrn = typeof(TResult), ListDistict = new List<TResult>() });
            return source.Provider.Execute<IEnumerable<TResult>>(source.Expression);
        }

        


        /// <summary>
        /// Execution to delete a record from a table
        /// </summary>
        public static int Delete<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> exp = null) where TSource : class
        {
            ((ISqlComposite)source.Provider).ListCastExpression.Add(new ContainerCastExpression
            { CastomExpression = exp, TypeRevalytion = Evolution.Delete });
            return source.Provider.Execute<int>(source.Expression);
        }
        /// <summary>
        /// Execution to delete a record from a table
        /// </summary>
        public static Task<int> DeleteAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> exp = null) where TSource : class
        {
            ((ISqlComposite)source.Provider).ListCastExpression.Add(new ContainerCastExpression
                { CastomExpression = exp, TypeRevalytion = Evolution.Delete });
            return Task.FromResult(source.Provider.Execute<int>(source.Expression));
        }


        /// <summary>
        /// LIMIT is always placed at the end of the sentence
        /// (the beginning of the position, taking into account zero, the number in the sample)
        /// </summary>
        /// <param name="source"></param>
        /// <param name="start">Position start</param>
        /// <param name="length">Record length</param>
        public static IQueryable<TSource> Limit<TSource>(this IQueryable<TSource> source, int start, int length)
        {
            ((ISqlComposite)source.Provider).ListCastExpression.Add(new ContainerCastExpression
            { TypeRevalytion = Evolution.Limit, ParamList = new List<object> { start, length } });
            return source;
        }

       



        internal static Image GetImage(this byte[] coll)
        {
            return UtilsCore.ImageFromByte(coll);
        }


        /// <summary>
        /// Partitioning a sequence
        /// </summary>
        /// <param name="source">An System.Linq.IQueryable`1 to return the first element of</param>
        /// <param name="chunkSize">Quantity per piece</param>
        public static List<List<TSource>> SplitQueryable<TSource>(this IQueryable<TSource> source, int chunkSize)
        {
            return InnerSplitQueryable(source, chunkSize);
        }

        /// <summary>
        /// Partitioning a sequence
        /// </summary>
        /// <param name="source">An System.Linq.IQueryable`1 to return the first element of</param>
        /// <param name="chunkSize">Quantity per piece</param>
        public static Task<List<List<TSource>>> SplitQueryableAsync<TSource>(this IQueryable<TSource> source, int chunkSize)
        {
            return Task.FromResult(InnerSplitQueryable(source, chunkSize));
        }

        private static List<List<T>> InnerSplitQueryable<T>(this IQueryable<T> coll, int chunkSize)
        {
            var enumerable = coll.ToList();
            return enumerable
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="action"></param>
        /// <typeparam name="TSource"></typeparam>
        public static async Task ForEachAsync<TSource>(this IQueryable<TSource> source, Action<TSource> action)
        {
            Check.NotNull(source, "source");
            Check.NotNull(action, "action");
            var res = await source.ToListAsync();
            res.ForEach(action);
        }

        ///  <summary>
        /// Query to update table
        ///  </summary>
        ///  <param name="source">IQueryable</param>
        ///  <param name="param">field-value dictionary</param>
        public static int Update<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, Dictionary<object, object>>> param) where TSource : class
        {
           
            ((ISqlComposite)source.Provider).ListCastExpression.Add(new ContainerCastExpression
            { CastomExpression = param, TypeRevalytion = Evolution.Update });
            return source.Provider.Execute<int>(source.Expression);
        }

      

        ///  <summary>
        /// Query to update table
        ///  </summary>
        ///  <param name="source">IQueryable</param>
        ///  <param name="param">field-value dictionary</param>
        public static Task<int> UpdateAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, Dictionary<object, object>>> param) where TSource : class
        {

            ((ISqlComposite)source.Provider).ListCastExpression.Add(new ContainerCastExpression
                { CastomExpression = param, TypeRevalytion = Evolution.Update });
            return Task.FromResult(source.Provider.Execute<int>(source.Expression));
        }

        /// <summary>
        ///     Executing an arbitrary query with parameters
        /// </summary>
        /// <param name="ses">ISession</param>
        /// <param name="sql">Request string</param>
        /// <param name="param">Request parameters</param>
        /// <returns>IEnumerableTResult</returns>
        public static IEnumerable<TResult> FreeSql<TResult>(this ISession ses, string sql, params object[] param)
        {
            var p = new V(sql);
            Expression callExpr = Expression.Call(
                Expression.Constant(p), p.GetType().GetMethod("FreeSql"));
            if (param != null&&param.Length>0)
            {
                return (IEnumerable<TResult>)new DbQueryProvider<TResult>((Sessione)ses).ExecuteParam<TResult>(callExpr, param);
            }
            return (IEnumerable<TResult>)new DbQueryProvider<TResult>((Sessione)ses).Execute<TResult>(callExpr);
        }

        /// <summary>
        ///  Executing an asynchronously  request with parameters
        /// </summary>
        /// <param name="ses">ISession</param>
        /// <param name="sql">Request string</param>
        /// <param name="param">Request parameters</param>
        public static Task<IEnumerable<TResult>> FreeSqlAsync<TResult>(this ISession ses, string sql, params object[] param)
        {
            var p = new V(sql);
            Expression callExpr = Expression.Call(Expression.Constant(p), p.GetType().GetMethod("FreeSql"));
            if (param != null&&param.Length>0)
            {
                return Task.FromResult((IEnumerable<TResult>)new DbQueryProvider<TResult>((Sessione)ses).ExecuteParam<TResult>(callExpr, param));
            }
            return Task.FromResult((IEnumerable<TResult>)new DbQueryProvider<TResult>((Sessione)ses).Execute<TResult>(callExpr));
        }


        /// <summary>
        /// Calling a stored procedure
        /// </summary>
        /// <param name="ses">ISession</param>
        /// <param name="sql">Request string</param>
        /// <typeparam name="TResult">Return type enumerable</typeparam>
        public static IEnumerable<TResult> ProcedureCall<TResult>(this ISession ses, string sql)
        {
            var p = new V(sql);
            Expression callExpr = Expression.Call(
                Expression.Constant(p), p.GetType().GetMethod("FreeSql"));
            return new DbQueryProvider<TResult>((Sessione)ses).ExecuteCall<TResult>(callExpr);
        }

        /// <summary>
        /// Calling a stored procedure with parameters
        /// </summary>
        /// <param name="ses">ISession</param>
        /// <param name="sql">Request string</param>
        /// <param name="param">Request parameters</param>
        /// <typeparam name="TResult">Return type enumerable</typeparam>
        public static IEnumerable<TResult> ProcedureCallParam<TResult>(this ISession ses, string sql, params ParameterStoredPr[] param)
        {
            var p = new V(sql);
            Expression callExpr = Expression.Call(
                Expression.Constant(p), p.GetType().GetMethod("FreeSql"));
            return new DbQueryProvider<TResult>((Sessione)ses).ExecuteCallParam<TResult>(callExpr, param);
        }

        /// <summary>
        /// Asynchronous request execution
        /// </summary>
        public static Task<List<TResult>> ToListAsync<TResult>(this IQueryable<TResult> source, CancellationToken cancellationToken = default)
        {
            try
            {
                var res = ((QueryProvider)source.Provider).ExecuteAsync<TResult>(source.Expression,cancellationToken);
                return res;
                
            }
            catch (Exception ex)
            {
                MySqlLogger.Info($" {Environment.NewLine}{source}{Environment.NewLine}{ex}");
                throw;
            }
       
        }

     

        /// <summary>
        /// Set command timeout for one request 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="value">>=0</param>
        /// <returns></returns>
        public static IQueryable<TSource> SetTimeOut<TSource>(this IQueryable<TSource> source, int value)
        {
            ((ISqlComposite)source.Provider).ListCastExpression.Add(new ContainerCastExpression
            { Timeout = value, TypeRevalytion = Evolution.Timeout });
            return source;
        }

        /// <summary>
        /// Request using cache, if there is no cache, it will be created
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IQueryable<TSource> CacheUsage<TSource>(this IQueryable<TSource> source)
        {
            ((ISqlComposite)source.Provider).ListCastExpression.Add(new ContainerCastExpression
                { TypeRevalytion = Evolution.CacheUsage });
            return source;
        }
        /// <summary>
        /// Request with cache rewrite, if there is no cache, the cache will be created,
        /// the old cache will be overwritten
        /// </summary>
        public static IQueryable<TSource> CacheOver<TSource>(this IQueryable<TSource> source)
        {
            ((ISqlComposite)source.Provider).ListCastExpression.Add(new ContainerCastExpression
                { TypeRevalytion = Evolution.CacheOver });
            return source;
        }
        /// <summary>
        /// Clears the cache for a type :TSource
        /// </summary>
        public static void CacheClear<TSource>(this ISession session)
        {
            MyCache<TSource>.Clear();
        }

        /// <summary>
        /// Get key to get cache value
        /// </summary>

        public static int CacheGetKey<TSource>(this IQueryable<TSource> source)
        {
            ISession ses = ((ISqlComposite)source.Provider).Sessione;
            var provider = new DbQueryProvider<TSource>((Sessione)ses);
           provider.ListCastExpression.Add(new ContainerCastExpression
                { TypeRevalytion = Evolution.CacheKey });
            return (int)provider.Execute<int>(source.Expression);

        }
        /// <summary>
        /// Get value from cache by key
        /// </summary>
        public static object CacheGetValue<TSource>(this ISession session,int key)
        {
            return MyCache<TSource>.GetValue(key);
        }

    }

  
}