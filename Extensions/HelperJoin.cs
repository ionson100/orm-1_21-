using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ORM_1_21_.Utils;

namespace ORM_1_21_.Extensions
{
    public static partial class Helper
    {
        /// <summary>
        ///     Correlates the elements of two sequences based on matching keys.
        ///     The default equality comparer is used to compare keys.
        /// </summary>
        /// <param name="outer">The type of the result elements.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
        /// <param name="resultSelector">A function to create a result element from two matching elements.</param>
        /// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
        /// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
        /// <typeparam name="TResult">The type of the result elements.</typeparam>
        /// <returns>
        ///     An IEnumerable&lt;T&gt; that has elements of type TResult that are obtained by performing an inner join on two
        ///     sequences.
        /// </returns>
        public static IEnumerable<TResult> JoinCore<TOuter, TInner, TKey, TResult>(
            this IQueryable<TOuter> outer,
            IQueryable<TInner> inner,
            Expression<Func<TOuter, TKey>> outerKeySelector,
            Expression<Func<TInner, TKey>> innerKeySelector,
            Expression<Func<TOuter, TInner, TResult>> resultSelector)
        {
            Check.NotNull(outer, nameof(outer));
            Check.NotNull(inner, nameof(inner));
            Check.NotNull(outerKeySelector, nameof(outerKeySelector));
            Check.NotNull(resultSelector, nameof(resultSelector));
            var w = new Sweetmeat<TOuter, TInner>(outer, inner);
            w.Wait();

            return JoinIterator(w.First, w.Seconds, outerKeySelector.Compile(), innerKeySelector.Compile(),
                resultSelector.Compile(), null);
        }

        /// <summary>
        ///     Correlates the elements of two sequences based on matching keys.
        ///     A specified IEqualityComparer&lt;T&gt; is used to compare keys.
        /// </summary>
        /// <param name="outer">The type of the result elements.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
        /// <param name="resultSelector">A function to create a result element from two matching elements.</param>
        /// <param name="comparer">An IEqualityComparer&lt;T&gt; to hash and compare keys.</param>
        /// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
        /// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
        /// <typeparam name="TResult">The type of the result elements.</typeparam>
        /// <returns>
        ///     An IEnumerable&lt;T&gt; that has elements of type TResult that are obtained by performing an inner join on two
        ///     sequences.
        /// </returns>
        public static IEnumerable<TResult> JoinCore<TOuter, TInner, TKey, TResult>(
            this IQueryable<TOuter> outer,
            IQueryable<TInner> inner,
            Expression<Func<TOuter, TKey>> outerKeySelector,
            Expression<Func<TInner, TKey>> innerKeySelector,
            Expression<Func<TOuter, TInner, TResult>> resultSelector,
            IEqualityComparer<TKey> comparer)
        {
            Check.NotNull(outer, nameof(outer));
            Check.NotNull(inner, nameof(inner));
            Check.NotNull(outerKeySelector, nameof(outerKeySelector));
            Check.NotNull(resultSelector, nameof(resultSelector));
            Check.NotNull(comparer, nameof(comparer));
            var w = new Sweetmeat<TOuter, TInner>(outer, inner);
            w.Wait();
            return JoinIterator(w.First, w.Seconds, outerKeySelector.Compile(), innerKeySelector.Compile(),
                resultSelector.Compile(), comparer);
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
            foreach (var go in lookupOuter)
            foreach (var outer1 in go)
            foreach (var grouping in lookup)
                if (go.Key.Equals(grouping.Key))
                    foreach (var inner1 in grouping)
                    {
                        var rrh = resultSelector(outer1, inner1);
                        yield return rrh;
                    }
        }
    }
}