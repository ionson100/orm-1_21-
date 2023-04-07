using ORM_1_21_.Linq;
using ORM_1_21_.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

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
                CustomExpression = exp,
                TypeEvolution = Evolution.DistinctCore,
                TypeReturn = typeof(TResult),
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
        /// Creates System.Collections.Generic.List from IQueryable.(Executes a database query)
        /// </summary>
        public static List<TResult> ToList<TResult>(this IQueryable<TResult> source)
        {

            return (List<TResult>)((QueryProvider)source.Provider).Execute<TResult>(source.Expression);
        }

        /// <summary>
        /// Getting Array Asynchronous
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
        /// Projects each element of a sequence into a new form.
        /// </summary>
        /// <param name="source">A sequence of values to invoke a transform function on.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <returns>An IEnumerable&lt;T&gt; whose elements are the result of invoking the transform function on each element of source.</returns>
        public static IEnumerable<TResult> SelectCore<TSource, TResult>(
            this IQueryable<TSource> source,
            Func<TSource, TResult> selector)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(selector, nameof(selector));
            var d = ((Query<TSource>)source).ToList();
            foreach (var source1 in d)
            {
                yield return selector(source1);
            }

        }



        private static async Task<IEnumerable<TResult>> QueryableToListAsync<TSource, TResult>(
            Func<TSource, TResult> selector, IQueryable proxySource, CancellationToken cancellationToken)
        {
            List<TResult> list = new List<TResult>();
            var ss = await ((Query<TSource>)proxySource).ToListAsync(cancellationToken: cancellationToken);
            ss.ForEach(a =>
              {
                  list.Add(selector(a));
              });
            return list;

        }

        private static async Task<IEnumerable<TResult>> QueryableToListIntAsync<TSource, TResult>(Func<TSource, int,
            TResult> selector, IQueryable proxySource, CancellationToken cancellationToken)
        {

            List<TResult> list = new List<TResult>();
            var ss = await ((Query<TSource>)proxySource).ToListAsync(cancellationToken: cancellationToken);
            int index = -1;
            ss.ForEach(a =>
            {
                checked { ++index; }
                list.Add(selector(a, index));
            });
            return list;

        }

        #region Select





        /// <summary>
        /// Asynchronous projects each element of a sequence into a new form.
        /// </summary>
        /// <param name="source">A sequence of values to invoke a transform function on.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <param name="cancellationToken">Object of the cancelling to asynchronous operation</param>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <returns>An IEnumerable&lt;T&gt; whose elements are the result of invoking the transform function on each element of source.</returns>
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
        ///  Projects each element of a sequence into a new form by incorporating the element's index.
        /// </summary>
        /// <param name="source">A sequence of values to invoke a transform function on.</param>
        /// <param name="selector">A transform function to apply to each source element;
        /// the second parameter of the function represents the index of the source element.</param>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static IEnumerable<TResult> SelectCore<TSource, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, int, TResult> selector)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(selector, nameof(selector));
            var d = ((Query<TSource>)source).ToList();
            int index = -1;
            foreach (var source1 in d)
            {
                checked { ++index; }
                yield return selector(source1, index);
            }
        }

        /// <summary>
        /// Asynchronous  projects of a sequence into a new form by incorporating the element's index.
        /// </summary>
        /// <param name="source">A sequence of values to invoke a transform function on.</param>
        /// <param name="selector">A transform function to apply to each source element;
        /// the second parameter of the function represents the index of the source element.</param>
        /// <param name="cancellationToken">Object of the cancelling to asynchronous operation</param>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <returns>An IEnumerable&lt;T&gt; whose elements are the result of invoking the transform function on each element of source.</returns>
        public static async Task<IEnumerable<TResult>> SelectCoreAsync<TSource, TResult>(
            this IQueryable source,
            Func<TSource, int, TResult> selector,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(selector, nameof(selector));
            return await QueryableToListIntAsync(selector, source, cancellationToken);
        }
        #endregion


        #region AggregateCore






        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="func"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static TSource AggregateCore<TSource>(
            this IQueryable<TSource> source,
            Func<TSource, TSource, TSource> func)
        {
            using (IEnumerator<TSource> enumerator = source.GetEnumerator())
            {
                TSource source1 = enumerator.MoveNext() ? enumerator.Current : throw new InvalidOperationException(" Element Empty"); ;
                while (enumerator.MoveNext())
                    source1 = func(source1, enumerator.Current);
                return source1;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="func"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static async Task<TSource> AggregateCoreAsync<TSource>(
            this IQueryable<TSource> source,
            Func<TSource, TSource, TSource> func, CancellationToken cancellationToken = default)
        {
            var list =
                await ((QueryProvider)source.Provider).ExecuteExtensionAsync<List<TSource>>(source.Expression,
                    null, cancellationToken);
            using (IEnumerator<TSource> enumerator = list.GetEnumerator())
            {
                var source1 = enumerator.MoveNext() ? enumerator.Current : throw new InvalidOperationException(" Element Empty"); ;
                while (enumerator.MoveNext())
                    source1 = func(source1, enumerator.Current);
                return source1;
            }
        }





        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="seed"></param>
        /// <param name="func"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TAccumulate"></typeparam>
        /// <returns></returns>
        public static TAccumulate AggregateCore<TSource, TAccumulate>(
            this IQueryable<TSource> source,
            TAccumulate seed,
            Func<TAccumulate, TSource, TAccumulate> func)
        {
            TAccumulate accumulate = seed;
            foreach (TSource source1 in source)
                accumulate = func(accumulate, source1);
            return accumulate;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="seed"></param>
        /// <param name="func"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TAccumulate"></typeparam>
        /// <returns></returns>
        public static async Task<TAccumulate> AggregateCoreAsync<TSource, TAccumulate>(
            this IQueryable<TSource> source,
            TAccumulate seed,
            Func<TAccumulate, TSource, TAccumulate> func, CancellationToken cancellationToken = default)
        {
            TAccumulate accumulate = seed;
            List<TSource> res = await ((QueryProvider)source.Provider).ExecuteExtensionAsync<List<TSource>>(source.Expression, null,
                cancellationToken);

            foreach (TSource source1 in res)
                accumulate = func(accumulate, source1);
            return accumulate;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="seed"></param>
        /// <param name="func"></param>
        /// <param name="resultSelector"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TAccumulate"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static TResult AggregateCore<TSource, TAccumulate, TResult>(
            this IQueryable<TSource> source,
            TAccumulate seed,
            Func<TAccumulate, TSource, TAccumulate> func,
            Func<TAccumulate, TResult> resultSelector)
        {

            TAccumulate accumulate = seed;
            foreach (TSource source1 in source)
                accumulate = func(accumulate, source1);
            return resultSelector(accumulate);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="seed"></param>
        /// <param name="func"></param>
        /// <param name="resultSelector"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TAccumulate"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static async Task<TResult> AggregateCoreAsync<TSource, TAccumulate, TResult>(
            this IQueryable<TSource> source,
            TAccumulate seed,
            Func<TAccumulate, TSource, TAccumulate> func,
            Func<TAccumulate, TResult> resultSelector, CancellationToken cancellationToken = default)
        {
            List<TSource> res = await ((QueryProvider)source.Provider).ExecuteExtensionAsync<List<TSource>>(source.Expression, null,
                cancellationToken);
            TAccumulate accumulate = seed;
            foreach (TSource source1 in res)
                accumulate = func(accumulate, source1);
            return resultSelector(accumulate);
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <returns></returns>
        ///
        ///
        public static async Task<IEnumerable<TSource>> AsEnumerableAsync<TSource>(
            this IQueryable<TSource> source, CancellationToken cancellationToken = default)
        {
            var res = await ((QueryProvider)source.Provider).ExecuteExtensionAsync<List<TSource>>(source.Expression, null,
                cancellationToken);
            return res.AsEnumerable();
        }


        #region CastCore

        

        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static IEnumerable<TResult> CastCore<TResult>(this IQueryable source)
        {
            return source.Cast<TResult>();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static Task<IEnumerable<TResult>> CastCoreAsync<TResult>(this IQueryable source, CancellationToken cancellationToken = default)
        {
            var tk = new TaskCompletionSource<IEnumerable<TResult>>(TaskCreationOptions.RunContinuationsAsynchronously);
            tk.SetResult(source.Cast<TResult>());
            return tk.Task;
        }

        #endregion

        #region ExceptCore






        /// <summary>
        /// 
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <returns></returns>
        public static IEnumerable<TSource> ExceptCore<TSource>(
            this IQueryable<TSource> first,
            IQueryable<TSource> second)
        {
            var pFirst = (QueryProvider)first.Provider;
            var pSecond = new DbQueryProvider<TSource>((Sessione)((ISqlComposite)second.Provider).Sessione.SessionCloneForTask());
            var t1 = pFirst.ExecuteExtensionAsync<IEnumerable<TSource>>(first.Expression, null, CancellationToken.None);
            var t2 = pSecond.ExecuteExtensionAsync<IEnumerable<TSource>>(second.Expression, null, CancellationToken.None);
            Task.WaitAll(t1, t2);
            var first1 = t1.Result;
            var second1 = t2.Result;
            return ExceptIterator(first1, second1, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="comparer"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <returns></returns>
        public static IEnumerable<TSource> ExceptCore<TSource>(
            this IQueryable<TSource> first,
            IQueryable<TSource> second,
            IEqualityComparer<TSource> comparer) where TSource : class
        {
            var pFirst = (QueryProvider)first.Provider;
            var pSecond = new DbQueryProvider<TSource>((Sessione)((ISqlComposite)second.Provider).Sessione.SessionCloneForTask());
            var t1 = pFirst.ExecuteExtensionAsync<IEnumerable<TSource>>(first.Expression, null, CancellationToken.None);
            var t2 = pSecond.ExecuteExtensionAsync<IEnumerable<TSource>>(second.Expression, null, CancellationToken.None);
            Task.WaitAll(t1, t2);
            var first1 = t1.Result;
            var second1 = t2.Result;
            return ExceptIterator(first1, second1, comparer);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <returns></returns>
        public static IEnumerable<TSource> ExceptCore<TSource>(
            this IQueryable<TSource> first,
            IEnumerable<TSource> second) where TSource : class
        {
            return ExceptIterator(first, second, null);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="comparer"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <returns></returns>
        public static IEnumerable<TSource> ExceptCore<TSource>(
            this IQueryable<TSource> first,
            IEnumerable<TSource> second,
            IEqualityComparer<TSource> comparer) where TSource : class
        {
            return ExceptIterator(first, second, comparer);
        }

        private static IEnumerable<TSource> ExceptIterator<TSource>(
            IEnumerable<TSource> first,
            IEnumerable<TSource> second,
            IEqualityComparer<TSource> comparer)
        {
            Set<TSource> set = new Set<TSource>(comparer);
            foreach (TSource source in second)
                set.Add(source);
            foreach (TSource source in first)
            {
                if (set.Add(source))
                    yield return source;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <returns></returns>
        public static Task<IEnumerable<TSource>> ExceptCoreAsync<TSource>(
            this IQueryable<TSource> first,
            IQueryable<TSource> second,
            CancellationToken cancellationToken=default) where TSource : class
        {
            var tk = new TaskCompletionSource<IEnumerable<TSource>>(TaskCreationOptions.RunContinuationsAsynchronously);
            var pFirst = (QueryProvider)first.Provider;
            var pSecond = new DbQueryProvider<TSource>((Sessione)((ISqlComposite)second.Provider).Sessione.SessionCloneForTask());
            var t1 = pFirst.ExecuteExtensionAsync<IEnumerable<TSource>>(first.Expression, null, cancellationToken);
            var t2 = pSecond.ExecuteExtensionAsync<IEnumerable<TSource>>(second.Expression, null, cancellationToken);
            Task.WaitAll(t1, t2);
            var first1 = t1.Result;
            var second1 = t2.Result;
            tk.SetResult(ExceptIterator(first1, second1, null));
            return tk.Task;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="comparer"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <returns></returns>
        public static Task<IEnumerable<TSource>> ExceptCoreAsync<TSource>(
            this IQueryable<TSource> first,
            IQueryable<TSource> second,
            IEqualityComparer<TSource> comparer,
            CancellationToken cancellationToken = default) where TSource : class
        {
            var tk = new TaskCompletionSource<IEnumerable<TSource>>(TaskCreationOptions.RunContinuationsAsynchronously);
            var pFirst = (QueryProvider)first.Provider;
            var pSecond = new DbQueryProvider<TSource>((Sessione)((ISqlComposite)second.Provider).Sessione.SessionCloneForTask());
            var t1 = pFirst.ExecuteExtensionAsync<IEnumerable<TSource>>(first.Expression, null, cancellationToken);
            var t2 = pSecond.ExecuteExtensionAsync<IEnumerable<TSource>>(second.Expression, null, cancellationToken);
            Task.WaitAll(t1, t2);
            var first1 = t1.Result;
            var second1 = t2.Result;
            tk.SetResult(ExceptIterator(first1, second1, comparer));
            return tk.Task;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <returns></returns>
        public static async Task<IEnumerable<TSource>> ExceptCoreAsync<TSource>(
            this IQueryable<TSource> first,
            IEnumerable<TSource> second,
            CancellationToken cancellationToken=default) where TSource : class
        {
            var res = await ((QueryProvider)first.Provider).ExecuteExtensionAsync<IEnumerable<TSource>>(first.Expression,
                null, cancellationToken);
            return ExceptIterator(res, second, null);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="comparer"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <returns></returns>
        public static async Task<IEnumerable<TSource>> ExceptCoreAsync<TSource>(
            this IQueryable<TSource> first,
            IEnumerable<TSource> second,
            IEqualityComparer<TSource> comparer,
            CancellationToken cancellationToken = default) where TSource : class
        {
            var res = await ((QueryProvider)first.Provider).ExecuteExtensionAsync<IEnumerable<TSource>>(first.Expression,
                null, cancellationToken);
            return ExceptIterator(res, second, comparer);
        }

        #endregion

        #region ConcatCore



       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <returns></returns>
        public static IEnumerable<TSource> ConcatCore<TSource>(
            this IQueryable<TSource> first,
            IQueryable<TSource> second) where TSource : class
        {

           // var tk = new TaskCompletionSource<IEnumerable<TSource>>(TaskCreationOptions.RunContinuationsAsynchronously);
            var pFirst = (QueryProvider)first.Provider;
            var pSecond = new DbQueryProvider<TSource>((Sessione)((ISqlComposite)second.Provider).Sessione.SessionCloneForTask());
            var t1 = pFirst.ExecuteExtensionAsync<IEnumerable<TSource>>(first.Expression, null, CancellationToken.None);
            var t2 = pSecond.ExecuteExtensionAsync<IEnumerable<TSource>>(second.Expression, null, CancellationToken.None);
            Task.WaitAll(t1, t2);
            return ConcatIterator<TSource>(t1.Result, t2.Result);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <returns></returns>
        public static Task<IEnumerable<TSource>> ConcatCoreAsync<TSource>(
            this IQueryable<TSource> first,
            IQueryable<TSource> second,CancellationToken cancellationToken = default) where TSource : class
        {

            var tk = new TaskCompletionSource<IEnumerable<TSource>>(TaskCreationOptions.RunContinuationsAsynchronously);
            var pFirst = (QueryProvider)first.Provider;
            var pSecond = new DbQueryProvider<TSource>((Sessione)((ISqlComposite)second.Provider).Sessione.SessionCloneForTask());
            var t1 = pFirst.ExecuteExtensionAsync<IEnumerable<TSource>>(first.Expression, null, cancellationToken);
            var t2 = pSecond.ExecuteExtensionAsync<IEnumerable<TSource>>(second.Expression, null, cancellationToken);
            Task.WaitAll(t1, t2);
            tk.SetResult(ConcatIterator(t1.Result, t2.Result));
            return tk.Task;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <returns></returns>
        public static IEnumerable<TSource> ConcatCore<TSource>(
            this IQueryable<TSource> first,
            IEnumerable<TSource> second) where TSource : class
        {
            return ConcatIterator<TSource>(first, second);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <returns></returns>
        public static async Task<IEnumerable<TSource>> ConcatCoreCoreAsync<TSource>(
            this IQueryable<TSource> first,
            IEnumerable<TSource> second,CancellationToken cancellationToken=default) where TSource : class
        {
            var res = await ((QueryProvider)first.Provider).ExecuteExtensionAsync<IEnumerable<TSource>>(first.Expression,
                null, cancellationToken);
            return ConcatIterator<TSource>(res, second);
        }

        private static IEnumerable<TSource> ConcatIterator<TSource>(
            IEnumerable<TSource> first,
            IEnumerable<TSource> second)
        {
            foreach (TSource source in first)
                yield return source;
            foreach (TSource source in second)
                yield return source;
        }
        #endregion




    }














}