using ORM_1_21_.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ORM_1_21_.Extensions
{
    public static partial class Helper
    {
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
        public static IEnumerable<TResult> JoinCore<TOuter, TInner, TKey, TResult>(
            this IQueryable<TOuter> outer,
            IQueryable<TInner> inner,
            Expression<Func<TOuter, TKey>> outerKeySelector,
            Expression<Func<TInner, TKey>> innerKeySelector,
            Expression<Func<TOuter, TInner, TResult>> resultSelector)
        {
            var p = new DbQueryProvider<TOuter>((Sessione)((ISqlComposite)outer.Provider).Sessione);
            var db = new DbQueryProvider<TInner>((Sessione)Configure.Session);
            var t1 = db.ExecuteAsync<TInner>(inner.Expression, null, CancellationToken.None);
            var t2 = p.ExecuteAsync<TOuter>(outer.Expression, null, CancellationToken.None);
            Task.WaitAll(t1, t2);
            var innerS = (IEnumerable<TInner>)t1.Result;
            var outerS = (IEnumerable<TOuter>)t2.Result;

            return JoinIterator(outerS, innerS, outerKeySelector.Compile(), innerKeySelector.Compile(), resultSelector.Compile(), null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outer"></param>
        /// <param name="inner"></param>
        /// <param name="outerKeySelector"></param>
        /// <param name="innerKeySelector"></param>
        /// <param name="resultSelector"></param>
        /// <param name="comparer"></param>
        /// <typeparam name="TOuter"></typeparam>
        /// <typeparam name="TInner"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static IEnumerable<TResult> JoinCore<TOuter, TInner, TKey, TResult>(
            this IQueryable<TOuter> outer,
            IQueryable<TInner> inner,
            Expression<Func<TOuter, TKey>> outerKeySelector,
            Expression<Func<TInner, TKey>> innerKeySelector,
            Expression<Func<TOuter, TInner, TResult>> resultSelector,
            IEqualityComparer<TKey> comparer)
        {
            var p = new DbQueryProvider<TOuter>((Sessione)((ISqlComposite)outer.Provider).Sessione);
            var db = new DbQueryProvider<TInner>((Sessione)((ISqlComposite)outer.Provider).Sessione);
            var innerS = (IEnumerable<TInner>)db.Execute<TInner>(inner.Expression);
            var outerS = (IEnumerable<TOuter>)p.Execute<TOuter>(outer.Expression);

            return JoinIterator(outerS, innerS, outerKeySelector.Compile(), innerKeySelector.Compile(), resultSelector.Compile(), comparer);
        }
        private static IEnumerable<TResult> JoinIterator<TOuter, TInner, TKey, TResult>(
            IEnumerable<TOuter> outer,
            IEnumerable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<TOuter, TInner, TResult> resultSelector,
            IEqualityComparer<TKey> comparer)
        {
            var lookupOuter = outer.ToLookup(outerKeySelector, IdentityFunction<TOuter>.Instance, comparer);
            var lookup = inner.ToLookup(innerKeySelector, IdentityFunction<TInner>.Instance, comparer);
            foreach (IGrouping<TKey, TOuter> go in lookupOuter)
            {
                foreach (TOuter outer1 in go)
                {
                    foreach (IGrouping<TKey, TInner> grouping in lookup)
                    {
                        if (go.Key.Equals(grouping.Key))
                            foreach (TInner inner1 in grouping)
                            {
                                var rrh = resultSelector(outer1, inner1);
                                yield return rrh;
                            }
                    }
                }
            }
        }

    }


}
