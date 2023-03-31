using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using ORM_1_21_.Linq;
using ORM_1_21_.Linq.MySql;
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
        public static void ForEach<TSource>(this IQueryable<TSource> source, Action<TSource> action)
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
        public static IEnumerable<TResult> Distinct<TSource, TResult>(this IQueryable<TSource> source,
            Expression<Func<TSource, TResult>> exp) where TSource : class
        {
            ((ISqlComposite)source.Provider).ListCastExpression.Add(new ContainerCastExpression
            {
                CustomExpression = exp, TypeEvolution = Evolution.DistinctCore, TypeReturn = typeof(TResult),
                ListDistinct = new List<TResult>()
            });
            return source.Provider.Execute<IEnumerable<TResult>>(source.Expression);
        }




        /// <summary>
        /// Execution to delete a record from a table
        /// </summary>
        public static int Delete<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> exp = null)
            where TSource : class
        {
            ((ISqlComposite)source.Provider).ListCastExpression.Add(new ContainerCastExpression
                { CustomExpression = exp, TypeEvolution = Evolution.Delete });
            return source.Provider.Execute<int>(source.Expression);
        }

        /// <summary>
        /// Execution to delete a record from a table
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
        /// LIMIT is always placed at the end of the sentence
        /// (the beginning of the position, taking into account zero, the number in the sample)
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
        /// Partitioning a sequence
        /// </summary>
        /// <param name="source">An System.Linq.IQueryable`1 to return the first element of</param>
        /// <param name="chunkSize">Quantity per piece</param>
        public static List<List<TSource>> SplitQueryable<TSource>(this IQueryable<TSource> source, int chunkSize)
        {
            var enumerable = source.ToList();
            return enumerable
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }

        /// <summary>
        /// Partitioning a sequence
        /// </summary>
        /// <param name="source">An System.Linq.IQueryable`1 to return the first element of</param>
        /// <param name="chunkSize">Quantity per piece</param>
        public static Task<List<List<TSource>>> SplitQueryableAsync<TSource>(this IQueryable<TSource> source,
            int chunkSize)
        {
            return InnerSplitQueryable(source, chunkSize);
        }

        private static async Task<List<List<T>>> InnerSplitQueryable<T>(this IQueryable<T> coll, int chunkSize)
        {
            return await Task.Run(() =>
            {
                var enumerable = coll.ToList();
                return enumerable
                    .Select((x, i) => new { Index = i, Value = x })
                    .GroupBy(x => x.Index / chunkSize)
                    .Select(x => x.Select(v => v.Value).ToList())
                    .ToList();
            }).ConfigureAwait(false);
        }


        /// <summary>
        /// Asynchronously enumerates the query results and performs the specified action
        /// on each element.
        /// </summary>
        public static async Task ForEachAsync<TSource>(this IQueryable<TSource> source, Action<TSource> action,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, "source");
            Check.NotNull(action, "action");
            var res = await source.ToListAsync(cancellationToken: cancellationToken);
            res.ForEach(action);
        }

        ///  <summary>
        /// Query to update table
        ///  </summary>
        ///  <param name="source">IQueryable</param>
        ///  <param name="param">field-value dictionary</param>
        public static int Update<TSource>(this IQueryable<TSource> source,
            Expression<Func<TSource, Dictionary<object, object>>> param) where TSource : class
        {

            ((ISqlComposite)source.Provider).ListCastExpression.Add(new ContainerCastExpression
                { CustomExpression = param, TypeEvolution = Evolution.Update });
            return source.Provider.Execute<int>(source.Expression);
        }



        ///  <summary>
        /// Query to update table
        ///  </summary>
        ///  <param name="source">IQueryable</param>
        ///  <param name="param">field-value dictionary</param>
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
        /// <param name="ses">ISession</param>
        /// <param name="sql">Request string</param>
        /// <param name="param">Request parameters</param>
        /// <returns>IEnumerableTResult</returns>
        public static IEnumerable<TResult> FreeSql<TResult>(this ISession ses, string sql, params object[] param)
        {
            var p = new V(sql);
            Expression callExpr = Expression.Call(
                Expression.Constant(p), p.GetType().GetMethod("FreeSql"));
            var db = new DbQueryProvider<TResult>((Sessione)ses);
            if (param != null && param.Length > 0)
            {
                db.GetParamFree().AddRange(param);
            }

            return (IEnumerable<TResult>)db.Execute<TResult>(callExpr);
        }

        /// <summary>
        ///  Executing an asynchronously  request with parameters
        /// </summary>
        /// <param name="ses">ISession</param>
        /// <param name="sql">Request string</param>
        /// <param name="param">Request parameters</param>
        public static async Task<IEnumerable<TResult>> FreeSqlAsync<TResult>(this ISession ses, string sql,
            params object[] param)
        {
            var tk = new TaskCompletionSource<IEnumerable<TResult>>(TaskCreationOptions.RunContinuationsAsynchronously);
            var p = new V(sql);
            Expression callExpr = Expression.Call(
                Expression.Constant(p), p.GetType().GetMethod("FreeSql"));
            var db = new DbQueryProvider<TResult>((Sessione)ses);
            if (param != null && param.Length > 0)
            {
                db.GetParamFree().AddRange(param);
            }

            var res = (IEnumerable<TResult>)await db.ExecuteAsync<TResult>(callExpr, null, CancellationToken.None);
            tk.SetResult(res);
            return await tk.Task;
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
        public static IEnumerable<TResult> ProcedureCallParam<TResult>(this ISession ses, string sql,
            params ParameterStoredPr[] param)
        {
            var p = new V(sql);
            Expression callExpr = Expression.Call(
                Expression.Constant(p), p.GetType().GetMethod("FreeSql"));
            return new DbQueryProvider<TResult>((Sessione)ses).ExecuteCallParam<TResult>(callExpr, param);
        }

        /// <summary>
        /// Getting List Asynchronous
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
        /// Getting Array Asynchronous
        /// </summary>
        public static Task<Array> ToArrayAsync<TResult>(this IQueryable<TResult> source,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var res = ((QueryProvider)source.Provider).ExecuteToArray<TResult>(source.Expression,
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
        /// Set command timeout for one request 
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
        /// Request using cache, if there is no cache, it will be created
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IQueryable<TSource> CacheUsage<TSource>(this IQueryable<TSource> source)
        {
            ((ISqlComposite)source.Provider).ListCastExpression.Add(new ContainerCastExpression
                { TypeEvolution = Evolution.CacheUsage });
            return source;
        }

        /// <summary>
        /// Request with cache rewrite, if there is no cache, the cache will be created,
        /// the old cache will be overwritten
        /// </summary>
        public static IQueryable<TSource> CacheOver<TSource>(this IQueryable<TSource> source)
        {
            ((ISqlComposite)source.Provider).ListCastExpression.Add(new ContainerCastExpression
                { TypeEvolution = Evolution.CacheOver });
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
                { TypeEvolution = Evolution.CacheKey });
            return (int)provider.Execute<int>(source.Expression);

        }

        /// <summary>
        /// Get value from cache by key
        /// </summary>
        public static object CacheGetValue<TSource>(this ISession session, int key)
        {
            return MyCache<TSource>.GetValue(key);
        }

        /// <summary>
        /// The BETWEEN operator selects values within a given range. 
        /// </summary>
        public static IQueryable<TSource> Between<TSource, T>(this IQueryable<TSource> source,
            Expression<Func<TSource, T>> func, T left, T right)
        {
            ((ISqlComposite)source.Provider).ListCastExpression.Add(new ContainerCastExpression
            {
                TypeEvolution = Evolution.Between, CustomExpression = func, ParamList = new List<object>
                {
                    Expression.Constant(left, typeof(object)),
                    Expression.Constant(right, typeof(object))
                }
            });
            return source;
        }



        /// <summary>
        /// 
        /// </summary>
        public static IQueryable<TResult> Join<TOuter, TInner, TKey, TResult>(
            this IQueryable<TOuter> outer,
            IQueryable<TInner> inner,
            Expression<Func<TOuter, TKey>> outerKeySelector,
            Expression<Func<TInner, TKey>> innerKeySelector,
            Expression<Func<TOuter, TInner, TResult>> resultSelector)
        {

            AttributesOfClass<TInner>.Init();
            return outer.Provider.CreateQuery<TResult>(Expression.Call(null, 
                GetMethodInfo<IQueryable<TOuter>, 
                    IQueryable<TInner>,
                    Expression<Func<TOuter, TKey>>,
                    Expression<Func<TInner, TKey>>,
                    Expression<Func<TOuter, TInner, TResult>>, IQueryable<TResult>>
                    (Queryable.Join), outer.Expression, inner.Expression,
                Expression.Quote(outerKeySelector), Expression.Quote(innerKeySelector), Expression.Quote(resultSelector)));
        }

        private static Expression GetSourceExpression<TSource>(IEnumerable<TSource> source) =>
            source is IQueryable<TSource> queryable
                ? queryable.Expression
                : (Expression)Expression.Constant((object)source, typeof(IEnumerable<TSource>));

       



        /// <summary>
        /// 
        /// </summary>
        /// <param name="outer"></param>
        /// <param name="inner"></param>
        /// <param name="outerKeySelector"></param>
        /// <param name="innerKeySelector"></param>
        /// <param name="resultSelector"></param>
        /// <typeparam name="TOuter"></typeparam>
        /// <typeparam name="TInner"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static IQueryable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(
            this IQueryable<TOuter> outer,
            IQueryable<TInner> inner,
            Expression<Func<TOuter, TKey>> outerKeySelector,
            Expression<Func<TInner, TKey>> innerKeySelector,
            Expression<Func<TOuter, IEnumerable<TInner>, TResult>> resultSelector)
        {
            AttributesOfClass<TInner>.Init();
            return outer.Provider.CreateQuery<TResult>(Expression.Call(null,
                GetMethodInfo<IQueryable<TOuter>, IQueryable<TInner>, Expression<Func<TOuter, TKey>>, Expression<Func<TInner, TKey>>, Expression<Func<TOuter, IEnumerable<TInner>, TResult>>, IQueryable<TResult>>(Queryable.GroupJoin), outer.Expression, GetSourceExpression(inner), Expression.Quote(outerKeySelector),
                Expression.Quote(innerKeySelector), Expression.Quote(resultSelector)));
        }

      

    

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <param name="elementSelector"></param>
        /// <param name="resultSelector"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TElement"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static IQueryable<TResult> GroupByCore<TSource, TKey, TElement, TResult>(
            this IQueryable<TSource> source,
            Expression<Func<TSource, TKey>> keySelector,
            Expression<Func<TSource, TElement>> elementSelector,
            Expression<Func<TKey, IEnumerable<TElement>, TResult>> resultSelector) 
        {
         
            return source.Provider.CreateQuery<TResult>(Expression.Call(null, GetMethodInfo<IQueryable<TSource>, Expression<Func<TSource, TKey>>, Expression<Func<TSource, TElement>>, Expression<Func<TKey, IEnumerable<TElement>, TResult>>, IQueryable<TResult>>(GroupByCore), source.Expression, Expression.Quote(keySelector), Expression.Quote(elementSelector), Expression.Quote(resultSelector)));
        }
     

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <param name="elementSelector"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TElement"></typeparam>
        /// <returns></returns>
        public static IEnumerable<IGrouping<TKey, TElement>> GroupByCore<TSource, TKey, TElement>(
            this IQueryable<TSource> source,
            Expression<Func<TSource, TKey>> keySelector,
            Expression<Func<TSource, TElement>> elementSelector)
        {
           

            ISession ses = ((ISqlComposite)source.Provider).Sessione;
            var p=new DbQueryProvider<TSource>((Sessione)ses);
            IEnumerable<TSource> sources = (IEnumerable<TSource>)p.Execute<TSource>(source.Expression);

            return new GroupedEnumerable<TSource, TKey, TElement>(sources, keySelector.Compile(), elementSelector.Compile(), (IEqualityComparer<TKey>)null);
            //return source.Provider.CreateQuery<IGrouping<TKey, TElement>>(Expression.Call(null, GetMethodInfo<IQueryable<TSource>, Expression<Func<TSource, TKey>>, Expression<Func<TSource, TElement>>, IQueryable<IGrouping<TKey, TElement>>>(GroupByCore), new[]
            //{
            //    source.Expression,
            //    Expression.Quote(keySelector),
            //    Expression.Quote(elementSelector)
            //}));
        }
    }

    internal class GroupedEnumerable<TSource, TKey, TElement> :
        IEnumerable<IGrouping<TKey, TElement>>,
        IEnumerable
    {
        private IEnumerable<TSource> source;
        private Func<TSource, TKey> keySelector;
        private Func<TSource, TElement> elementSelector;
        private IEqualityComparer<TKey> comparer;

        public GroupedEnumerable(
            IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector,
            IEqualityComparer<TKey> comparer)
        {
         
            this.source = source;
            this.keySelector = keySelector;
            this.elementSelector = elementSelector;
            this.comparer = comparer;
        }

        public IEnumerator<IGrouping<TKey, TElement>> GetEnumerator() => Create(this.source, this.keySelector, this.elementSelector, this.comparer).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        internal static List<IGrouping<TKey, TElement>> Create(
            IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector,
            IEqualityComparer<TKey> comparer)
        {
            

            var l = new List<TSource>();
            var rr = source.ToLookup(keySelector,elementSelector).ToList();

            return rr;
        }
    }




}