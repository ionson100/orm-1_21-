using System;
using System.Collections.Generic;
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
    public static class Helper
    {

        /// <summary>
        ///  Iterating over a collection
        /// </summary>
        public static void  ForEach<T>(this IQueryable<T> coll, Action<T> action)
        {
            Query<T> res = (Query<T>)coll;
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
        public static IEnumerable<TR> Distinct<T, TR>(this IQueryable<T> coll, Expression<Func<T, TR>> exp) where T : class
        {
            ((ISqlComposite)coll.Provider).ListCastExpression.Add(new ContainerCastExpression
            { CastomExpression = exp, TypeRevalytion = Evolution.DistinctCore, TypeRetyrn = typeof(TR), ListDistict = new List<TR>() });
            return coll.Provider.Execute<IEnumerable<TR>>(coll.Expression);
        }

        


        /// <summary>
        /// Execution to delete a record from a table
        /// </summary>
        public static int Delete<T>(this IQueryable<T> coll, Expression<Func<T, bool>> exp = null) where T : class
        {
            ((ISqlComposite)coll.Provider).ListCastExpression.Add(new ContainerCastExpression
            { CastomExpression = exp, TypeRevalytion = Evolution.Delete });
            return coll.Provider.Execute<int>(coll.Expression);
        }


        /// <summary>
        /// LIMIT is always placed at the end of the sentence
        /// (the beginning of the position, taking into account zero, the number in the sample)
        /// </summary>
        /// <param name="coll"></param>
        /// <param name="start">Position start</param>
        /// <param name="length">Record length</param>
        public static IQueryable<T> Limit<T>(this IQueryable<T> coll, int start, int length)
        {
            ((ISqlComposite)coll.Provider).ListCastExpression.Add(new ContainerCastExpression
            { TypeRevalytion = Evolution.Limit, ParamList = new List<object> { start, length } });
            return coll;
        }


       
        internal static Image GetImage(this byte[] coll)
        {
            return UtilsCore.ImageFromByte(coll);
        }

      
        internal static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> source, int chunkLength)
        {
            var enumerable = source as IList<T> ?? source.ToList();
            using (var enumerator = enumerable.GetEnumerator())
            {
                while (enumerator.MoveNext()) yield return InnerSplit(enumerator, chunkLength);
            }
        }

        
        internal static IEnumerable<IEnumerable<T>> SplitQueryable<T>(this IQueryable<T> coll, int splitSize)
        {
            var enumerable = coll;
            using (var enumerator = enumerable.GetEnumerator())
            {
                while (enumerator.MoveNext()) yield return InnerSplit(enumerator, splitSize);
            }
        }

        private static IEnumerable<T> InnerSplit<T>(IEnumerator<T> enumerator, int splitSize)
        {
            var count = 0;
            do
            {
                count++;
                yield return enumerator.Current;
            } while (count % splitSize != 0
                     && enumerator.MoveNext());
        }


        ///  <summary>
        /// Query to update table
        ///  </summary>
        ///  <param name="coll">IQueryable</param>
        ///  <param name="param">field-value dictionary</param>
        public static int Update<T>(this IQueryable<T> coll, Expression<Func<T, Dictionary<object, object>>> param) where T : class
        {
           
            ((ISqlComposite)coll.Provider).ListCastExpression.Add(new ContainerCastExpression
            { CastomExpression = param, TypeRevalytion = Evolution.Update });
            return coll.Provider.Execute<int>(coll.Expression);
        }

        /// <summary>
        ///     Executing an arbitrary query with parameters
        /// </summary>
        /// <param name="ses">ISession</param>
        /// <param name="sql">Request string</param>
        /// <param name="par">Request parameters</param>
        /// <returns>IEnumerableTResult</returns>
        public static IEnumerable<TResult> FreeSql<TResult>(this ISession ses, string sql, params Parameter[] par)
        {
            var p = new V(sql);
            Expression callExpr = Expression.Call(
                Expression.Constant(p), p.GetType().GetMethod("FreeSql"));
            if (par != null&&par.Length>0)
            {
                return (IEnumerable<TResult>)new DbQueryProvider<TResult>((Sessione)ses).ExecuteParam<TResult>(callExpr, par);
            }
            return (IEnumerable<TResult>)new DbQueryProvider<TResult>((Sessione)ses).Execute<TResult>(callExpr);
        }

        /// <summary>
        ///  Executing an asynchronously  request with parameters
        /// </summary>
        /// <param name="ses">ISession</param>
        /// <param name="sql">Request string</param>
        /// <param name="par">Request parameters</param>
        public static Task<IEnumerable<TResult>> FreeSqlAsync<TResult>(this ISession ses, string sql, params Parameter[] par)
        {
            var p = new V(sql);
            Expression callExpr = Expression.Call(Expression.Constant(p), p.GetType().GetMethod("FreeSql"));
            if (par != null&&par.Length>0)
            {
                return Task.FromResult((IEnumerable<TResult>)new DbQueryProvider<TResult>((Sessione)ses).ExecuteParam<TResult>(callExpr, par));
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
        /// <param name="par">Request parameters</param>
        /// <typeparam name="TResult">Return type enumerable</typeparam>
        public static IEnumerable<TResult> ProcedureCallParam<TResult>(this ISession ses, string sql, params ParameterStoredPr[] par)
        {
            var p = new V(sql);
            Expression callExpr = Expression.Call(
                Expression.Constant(p), p.GetType().GetMethod("FreeSql"));
            return new DbQueryProvider<TResult>((Sessione)ses).ExecuteCallParam<TResult>(callExpr, par);
        }

       // /// <summary>
       // ///Asynchronous request execution for groupBy
       // /// </summary>
        // public static Task<List<IGrouping<string, TResult>>> ToListAsync<TResult>(this IQueryable<IGrouping<string, TResult>> coll) 
        // {
        //
        //     try
        //     {
        //         return Task.FromResult(ActionCoreGroupBy());
        //     }
        //     catch (Exception ex)
        //     {
        //         MySqlLogger.Info($" {Environment.NewLine}{coll}{Environment.NewLine}{ex}");
        //         throw;
        //     }
        //     List<IGrouping<string, TResult>> ActionCoreGroupBy()
        //     {
        //         List<IGrouping<string, TResult>> resList = new List<IGrouping<string, TResult>>();
        //         ISession ses = ((ISqlComposite)coll.Provider).Sessione;
        //         var provider = new DbQueryProvider<TResult>((Sessione)ses);
        //         foreach (var o in (IEnumerable<object>)provider.Execute<IGrouping<string, TResult>>(coll.Expression))
        //         {
        //             resList.Add((IGrouping<string, TResult>)o);
        //         }
        //         return resList;
        //     }
        //
        // }
        /// <summary>
        ///Asynchronous request execution for groupBy
        /// </summary>
        public static Task<List<IGrouping<string, TResult>>> ToLisAsync<TResult>(this IQueryable<IGrouping<string, TResult>> coll,
            CancellationToken cancellationToken=default)
        {

            try
            {
                ISession ses = ((ISqlComposite)coll.Provider).Sessione;
                return new DbQueryProvider<TResult>((Sessione)ses).ToListGroupByAsync<TResult>(coll.Expression,  default);
            }
            catch (Exception ex)
            {
                MySqlLogger.Info($" {Environment.NewLine}{coll}{Environment.NewLine}{ex}");
                throw;
            }

        }



        /// <summary>
        /// Asynchronous request execution
        /// </summary>
        public static Task<List<TResult>> ToListAsync<TResult>(this IQueryable<TResult> coll, CancellationToken cancellationToken = default)
        {

            try
            {
                ISession ses = ((ISqlComposite)coll.Provider).Sessione;
               return new DbQueryProvider<TResult>((Sessione)ses).ToListAsync<TResult>(coll.Expression,cancellationToken); 
            }
            catch (Exception ex)
            {
                MySqlLogger.Info($" {Environment.NewLine}{coll}{Environment.NewLine}{ex}");
                throw;
            }
       
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="coll"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static List<TResult> ToList<TResult>(this IQueryable<TResult> coll) where TResult: IEnumerable<IGrouping<string, object>>
        {
            var sas = typeof(TResult);
            var d = (IInnerList)coll;
            var list = d.GetInnerList();
            var s = list.GetType();
            try
            {
             
                return (List<TResult>)list;
            }
            catch (Exception ex)
            {
                MySqlLogger.Info($" {Environment.NewLine}{coll}{Environment.NewLine}{ex}");
                throw;

            }

        }

        /// <summary>
        /// Set command timeout for one request 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="coll"></param>
        /// <param name="value">>=0</param>
        /// <returns></returns>
        public static IQueryable<T> SetTimeOut<T>(this IQueryable<T> coll, int value)
        {
            ((ISqlComposite)coll.Provider).ListCastExpression.Add(new ContainerCastExpression
            { Timeout = value, TypeRevalytion = Evolution.Timeout });
            return coll;
        }

        /// <summary>
        /// Request using cache, if there is no cache, it will be created
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="coll"></param>
        /// <returns></returns>
        public static IQueryable<T> CacheUsage<T>(this IQueryable<T> coll)
        {
            ((ISqlComposite)coll.Provider).ListCastExpression.Add(new ContainerCastExpression
                { TypeRevalytion = Evolution.CacheUsage });
            return coll;
        }
        /// <summary>
        /// Request with cache rewrite, if there is no cache, the cache will be created,
        /// the old cache will be overwritten
        /// </summary>
        public static IQueryable<T> CacheOver<T>(this IQueryable<T> coll)
        {
            ((ISqlComposite)coll.Provider).ListCastExpression.Add(new ContainerCastExpression
                { TypeRevalytion = Evolution.CacheOver });
            return coll;
        }
        /// <summary>
        /// Clears the cache for a type :T
        /// </summary>
        public static void CacheClear<T>(this ISession session)
        {
            MyCache<T>.Clear();
        }

        /// <summary>
        /// Get key to get cache value
        /// </summary>

        public static int CacheGetKey<T>(this IQueryable<T> coll)
        {
            ISession ses = ((ISqlComposite)coll.Provider).Sessione;
            var provider = new DbQueryProvider<T>((Sessione)ses);
           provider.ListCastExpression.Add(new ContainerCastExpression
                { TypeRevalytion = Evolution.CacheKey });
            return (int)provider.Execute<int>(coll.Expression);

        }
        /// <summary>
        /// Get value from cache by key
        /// </summary>
        public static object CacheGetValue<T>(this ISession session,int key)
        {
            return MyCache<T>.GetValue(key);
        }

    }
}