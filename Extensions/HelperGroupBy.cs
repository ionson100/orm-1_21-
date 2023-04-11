using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ORM_1_21_.Linq;
using ORM_1_21_.Utils;

namespace ORM_1_21_.Extensions
{
    public static partial class Helper
    {
        /// <summary>
        ///     Groups the elements of a sequence according to a specified key selector function
        ///     and creates a result value from each group and its key. The elements of each group are projected by using a
        ///     specified function.
        /// </summary>
        /// <param name="source">An IEnumerable&lt;TSource&gt; whose elements to group.</param>
        /// <param name="keySelector">A function to extract the key for each element.</param>
        /// <param name="elementSelector">A function to map each source element to an element in an IGrouping&lt;TKey,TElement&gt;.</param>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
        /// <typeparam name="TElement">The type of the elements in each IGrouping&lt;TKey,TElement&gt;.</typeparam>
        public  static  IEnumerable<IGrouping<TKey, TElement>> GroupByCore<TSource, TKey, TElement>(
            this IQueryable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(keySelector, nameof(keySelector));
            return new GroupedEnumerable<TSource, TKey, TElement>(source, keySelector,
                elementSelector, null);
        }


        /// <summary>
        ///     Groups asynchronous  the elements of a sequence according to a specified key selector
        ///     function and creates a result value from each group and its key. The elements of each group are projected by using
        ///     a specified function.
        /// </summary>
        /// <param name="source">An IEnumerable&lt;TSource&gt; whose elements to group.</param>
        /// <param name="keySelector">A function to extract the key for each element.</param>
        /// <param name="elementSelector">A function to map each source element to an element in an IGrouping&lt;TKey,TElement&gt;.</param>
        /// <param name="cancellationToken">Object of the cancelling to asynchronous operation</param>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
        /// <typeparam name="TElement">The type of the elements in each IGrouping&lt;TKey,TElement&gt;.</typeparam>
        public static async Task<IEnumerable<IGrouping<TKey, TElement>>> GroupByCoreAsync<TSource, TKey, TElement>(
            this IQueryable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(keySelector, nameof(keySelector));
            Check.NotNull(elementSelector, nameof(elementSelector));

            var sources = await QueryableToListAsync(source, cancellationToken);
            return new GroupedEnumerable<TSource, TKey, TElement>(sources, keySelector,
                elementSelector, null);
        }

        /// <summary>
        ///     Groups the elements of a sequence according to a key selector function.The keys are
        ///     compared by using a comparer and each group's elements are projected by using a specified function.
        /// </summary>
        /// <param name="source">An IEnumerable&lt;TSource&gt; whose elements to group.</param>
        /// <param name="keySelector">A function to extract the key for each element.</param>
        /// <param name="elementSelector">A function to map each source element to an element in an IGrouping&lt;TKey,TElement&gt;.</param>
        /// <param name="comparer">An IEqualityComparer&lt;TSource&gt; to compare keys.</param>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
        /// <typeparam name="TElement">The type of the elements in each IGrouping&lt;TKey,TElement&gt;.</typeparam>
        public static IEnumerable<IGrouping<TKey, TElement>> GroupByCore<TSource, TKey, TElement>(
            this IQueryable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector,
            IEqualityComparer<TKey> comparer)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(keySelector, nameof(keySelector));
            Check.NotNull(elementSelector, nameof(elementSelector));
            Check.NotNull(comparer, nameof(comparer));
            return new GroupedEnumerable<TSource, TKey, TElement>(source, keySelector,
                elementSelector, comparer);
        }


        /// <summary>
        ///     Groups asynchronous the elements of a sequence according to a key selector function.The keys are
        ///     compared by using a comparer and each group's elements are projected by using a specified function.
        /// </summary>
        /// <param name="source">An IEnumerable&lt;TSource&gt; whose elements to group.</param>
        /// <param name="keySelector">A function to extract the key for each element.</param>
        /// <param name="elementSelector">A function to map each source element to an element in an IGrouping&lt;TKey,TElement&gt;.</param>
        /// <param name="comparer">An IEqualityComparer&lt;TSource&gt; to compare keys.</param>
        /// <param name="cancellationToken">Object of the cancelling to asynchronous operation</param>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
        /// <typeparam name="TElement">The type of the elements in each IGrouping&lt;TKey,TElement&gt;.</typeparam>
        public static async Task<IEnumerable<IGrouping<TKey, TElement>>> GroupByCoreAsync<TSource, TKey, TElement>(
            this IQueryable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector,
            IEqualityComparer<TKey> comparer,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(keySelector, nameof(keySelector));
            Check.NotNull(elementSelector, nameof(elementSelector));
            Check.NotNull(comparer, nameof(comparer));
            var sources = await QueryableToListAsync(source, cancellationToken);
            return new GroupedEnumerable<TSource, TKey, TElement>(sources, keySelector,
                elementSelector, comparer);
        }

        /// <summary>
        ///     Groups the elements of a sequence according to a specified
        ///     key selector function and compares the keys by using a specified comparer.
        /// </summary>
        /// <param name="source">An IEnumerable&amp;lt;TSource&amp;gt; whose elements to group.</param>
        /// <param name="keySelector">A function to extract the key for each element.</param>
        /// <param name="comparer">An IEqualityComparer&lt;TSource&gt; to compare keys.</param>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
        public static IEnumerable<IGrouping<TKey, TSource>> GroupByCore<TSource, TKey>(this IQueryable<TSource> source,
            Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(keySelector, nameof(keySelector));
            Check.NotNull(comparer, nameof(comparer));
            Check.NotNull(comparer, nameof(comparer));
            return new GroupedEnumerable<TSource, TKey, TSource>(source, keySelector,
                IdentityFunction<TSource>.Instance, comparer);
        }

        /// <summary>
        ///     Groups asynchronous the elements of a sequence according to a specified
        ///     key selector function and compares the keys by using a specified comparer.
        /// </summary>
        /// <param name="source">An IEnumerable&amp;lt;TSource&amp;gt; whose elements to group.</param>
        /// <param name="keySelector">A function to extract the key for each element.</param>
        /// <param name="comparer">An IEqualityComparer&lt;TSource&gt; to compare keys.</param>
        /// <param name="cancellationToken">Object of the cancelling to asynchronous operation</param>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
        public static async Task<IEnumerable<IGrouping<TKey, TSource>>> GroupByCoreAsync<TSource, TKey>(
            this IQueryable<TSource> source,
            Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(keySelector, nameof(keySelector));
            Check.NotNull(comparer, nameof(comparer));

            var sources = await QueryableToListAsync(source, cancellationToken);
            return new GroupedEnumerable<TSource, TKey, TSource>(sources, keySelector,
                IdentityFunction<TSource>.Instance, comparer);
        }

        /// <summary>
        ///     Groups the elements of a sequence according to a specified key selector function.
        /// </summary>
        /// <param name="source">An IEnumerable&amp;lt;TSource&amp;gt; whose elements to group.</param>
        /// <param name="keySelector">A function to extract the key for each element.</param>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
        /// <returns></returns>
        public static IEnumerable<IGrouping<TKey, TSource>> GroupByCore<TSource, TKey>(this IQueryable<TSource> source,
           Func<TSource, TKey> keySelector
        )
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(keySelector, nameof(keySelector));
            return new GroupedEnumerable<TSource, TKey, TSource>(
                source,
                keySelector,
                IdentityFunction<TSource>.Instance,
                null);
        }

        /// <summary>
        ///     Groups asynchronous the elements of a sequence according to a specified key selector function.
        /// </summary>
        /// <param name="source">An IEnumerable&amp;lt;TSource&amp;gt; whose elements to group.</param>
        /// <param name="keySelector">A function to extract the key for each element.</param>
        /// <param name="cancellationToken">Object of the canceling to asynchronous operation</param>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
        /// <returns></returns>
        public static async Task<IEnumerable<IGrouping<TKey, TSource>>> GroupByCoreAsync<TSource, TKey>(
            this IQueryable<TSource> source,
            Func<TSource, TKey> keySelector, CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(keySelector, nameof(keySelector));
            var sources = await QueryableToListAsync(source, cancellationToken);
            return new GroupedEnumerable<TSource, TKey, TSource>(
                sources,
                keySelector,
                IdentityFunction<TSource>.Instance,
                null);
        }

        /// <summary>
        ///     Groups the elements of a sequence according to a specified key selector
        ///     function and creates a result value from each group and its key.
        /// </summary>
        /// <param name="source">An IEnumerable&amp;lt;TSource&amp;gt; whose elements to group.</param>
        /// <param name="keySelector">A function to extract the key for each element.</param>
        /// <param name="resultSelector">A function to create a result value from each group.</param>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
        /// <typeparam name="TResult">The type of the result value returned by resultSelector.</typeparam>
        /// <returns></returns>
        public static IEnumerable<TResult> GroupByCore<TSource, TKey, TResult>(this IQueryable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TKey, IEnumerable<TSource>, TResult> resultSelector)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(keySelector, nameof(keySelector));
            Check.NotNull(resultSelector, nameof(resultSelector));
            return new GroupedEnumerable<TSource, TKey, TSource, TResult>(
                source,
                keySelector,
                IdentityFunction<TSource>.Instance,
                resultSelector,
                null);
        }


        /// <summary>
        ///     Groups asynchronous the elements of a sequence according to a specified key selector
        ///     function and creates a result value from each group and its key.
        /// </summary>
        /// <param name="source">An IEnumerable&amp;lt;TSource&amp;gt; whose elements to group.</param>
        /// <param name="keySelector">A function to extract the key for each element.</param>
        /// <param name="resultSelector">A function to create a result value from each group.</param>
        /// <param name="cancellationToken">Object of the canceling to asynchronous operation</param>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
        /// <typeparam name="TResult">The type of the result value returned by resultSelector.</typeparam>
        /// <returns></returns>
        public static async Task<IEnumerable<TResult>> GroupByCoreAsync<TSource, TKey, TResult>(
            this IQueryable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TKey, IEnumerable<TSource>, TResult> resultSelector,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(keySelector, nameof(keySelector));
            Check.NotNull(resultSelector, nameof(resultSelector));
            var sources = await QueryableToListAsync(source, cancellationToken);
            return new GroupedEnumerable<TSource, TKey, TSource, TResult>(
                sources,
                keySelector,
                IdentityFunction<TSource>.Instance,
                resultSelector,
                null);
        }

        /// <summary>
        ///     Groups the elements of a sequence according to a specified key selector
        ///     function and creates a result value from each group and its key. The keys are compared by using a specified
        ///     comparer.
        /// </summary>
        /// <param name="source">An IEnumerable&lt;T&gt; whose elements to group.</param>
        /// <param name="keySelector">A function to extract the key for each element.</param>
        /// <param name="resultSelector">A function to create a result value from each group.</param>
        /// <param name="comparer">An IEqualityComparer&lt;T&gt; to compare keys with.</param>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
        /// <typeparam name="TResult">The type of the result value returned by resultSelector.</typeparam>
        /// <returns></returns>
        public static IEnumerable<TResult> GroupByCore<TSource, TKey, TResult>(this IQueryable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TKey, IEnumerable<TSource>, TResult> resultSelector,
            IEqualityComparer<TKey> comparer)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(keySelector, nameof(keySelector));
            Check.NotNull(resultSelector, nameof(resultSelector));
            Check.NotNull(comparer, nameof(comparer));
            return new GroupedEnumerable<TSource, TKey, TSource, TResult>(
                source,
                keySelector,
                IdentityFunction<TSource>.Instance,
                resultSelector,
                comparer);
        }


        /// <summary>
        ///     Correlates the elements of two sequences based on equality of keys and groups the results.
        ///     The default equality comparer is used to compare keys.
        /// </summary>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
        /// <param name="resultSelector">
        ///     A function to create a result element from an element
        ///     from the first sequence and a collection of matching elements from the second sequence.
        /// </param>
        /// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
        /// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
        /// <typeparam name="TResult">The type of the result elements.</typeparam>
        /// <returns>
        ///     An IEnumerable&lt;T&gt; that contains elements of type TResult that
        ///     are obtained by performing a grouped join on two sequences.
        /// </returns>
        public static IEnumerable<TResult> GroupJoinCore<TOuter, TInner, TKey, TResult>(this IQueryable<TOuter> outer,
            IQueryable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
        {
            Check.NotNull(outer, nameof(outer));
            Check.NotNull(inner, nameof(inner));
            Check.NotNull(outerKeySelector, nameof(outerKeySelector));
            Check.NotNull(innerKeySelector, nameof(innerKeySelector));

            var w = new Sweetmeat<TOuter, TInner>(outer, inner);
            w.Wait();

            return GroupJoinIterator(w.First, w.Seconds, outerKeySelector, innerKeySelector,
                resultSelector, null);
        }

        /// <summary>
        ///     Asynchronous correlates the elements of two sequences based on equality of keys and groups the results.
        ///     The default equality comparer is used to compare keys.
        /// </summary>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
        /// <param name="resultSelector">
        ///     A function to create a result element from an element
        ///     from the first sequence and a collection of matching elements from the second sequence.
        /// </param>
        /// <param name="cancellationToken">Object of the canceling to asynchronous operation</param>
        /// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
        /// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
        /// <typeparam name="TResult">The type of the result elements.</typeparam>
        /// <returns>
        ///     An IEnumerable&lt;T&gt; that contains elements of type TResult that
        ///     are obtained by performing a grouped join on two sequences.
        /// </returns>
        public static async Task<IEnumerable<TResult>> GroupJoinCoreAsync<TOuter, TInner, TKey, TResult>(
            this IQueryable<TOuter> outer,
            IQueryable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<TOuter, IEnumerable<TInner>,
                TResult> resultSelector,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(outer, nameof(outer));
            Check.NotNull(inner, nameof(inner));
            Check.NotNull(outerKeySelector, nameof(outerKeySelector));
            Check.NotNull(innerKeySelector, nameof(innerKeySelector));
            Check.NotNull(resultSelector, nameof(resultSelector));
            var w = new Sweetmeat<TOuter, TInner>(outer, inner, cancellationToken);
            await w.WaitAsync();
            return GroupJoinIterator(w.First, w.Seconds, outerKeySelector, innerKeySelector,
                resultSelector, null);
        }


        /// <summary>
        ///     Correlates the elements of two sequences based on equality of keys and groups the results.
        ///     The default equality comparer is used to compare keys.
        /// </summary>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
        /// <param name="resultSelector">
        ///     A function to create a result element from an element
        ///     from the first sequence and a collection of matching elements from the second sequence.
        /// </param>
        /// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
        /// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
        /// <typeparam name="TResult">The type of the result elements.</typeparam>
        /// <returns>
        ///     An IEnumerable&lt;T&gt; that contains elements of type TResult that
        ///     are obtained by performing a grouped join on two sequences.
        /// </returns>
        public static IEnumerable<TResult> GroupJoinCore<TOuter, TInner, TKey, TResult>(this IQueryable<TOuter> outer,
            IEnumerable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
        {
            Check.NotNull(outer, nameof(outer));
            Check.NotNull(inner, nameof(inner));
            Check.NotNull(outerKeySelector, nameof(outerKeySelector));
            Check.NotNull(innerKeySelector, nameof(innerKeySelector));
            Check.NotNull(resultSelector, nameof(resultSelector));
            var pTOuter = (QueryProvider)outer.Provider;
            var outerS = (IEnumerable<TOuter>)pTOuter.Execute<TOuter>(outer.Expression);
            return GroupJoinIterator(outerS, inner, outerKeySelector, innerKeySelector,
                resultSelector, null);
        }

        /// <summary>
        ///     Asynchronous correlates the elements of two sequences based on equality of keys and groups the results.
        ///     The default equality comparer is used to compare keys.
        /// </summary>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
        /// <param name="resultSelector">
        ///     A function to create a result element from an element
        ///     from the first sequence and a collection of matching elements from the second sequence.
        /// </param>
        /// <param name="cancellationToken">Object of the canceling to asynchronous operation</param>
        /// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
        /// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
        /// <typeparam name="TResult">The type of the result elements.</typeparam>
        /// <returns>
        ///     An IEnumerable&lt;T&gt; that contains elements of type TResult that
        ///     are obtained by performing a grouped join on two sequences.
        /// </returns>
        public static async Task<IEnumerable<TResult>> GroupJoinCoreAsync<TOuter, TInner, TKey, TResult>(
            this IQueryable<TOuter> outer,
            IEnumerable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<TOuter, IEnumerable<TInner>,
                TResult> resultSelector,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(outer, nameof(outer));
            Check.NotNull(inner, nameof(inner));
            Check.NotNull(outerKeySelector, nameof(outerKeySelector));
            Check.NotNull(innerKeySelector, nameof(innerKeySelector));
            Check.NotNull(resultSelector, nameof(resultSelector));
            var pTOuter = (QueryProvider)outer.Provider;
            var outerS = await QueryableToListAsync(outer, cancellationToken);
            return GroupJoinIterator(outerS, inner, outerKeySelector, innerKeySelector,
                resultSelector, null);
        }


        /// <summary>
        ///     Correlates the elements of two sequences based on key equality and groups the results.
        ///     A specified IEqualityComparer&lt;T&gt; is used to compare keys.
        /// </summary>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
        /// <param name="resultSelector">
        ///     A function to create a result element from an element
        ///     from the first sequence and a collection of matching elements from the second sequence.
        /// </param>
        /// <param name="comparer">An IEqualityComparer&lt;T&gt; to hash and compare keys.</param>
        /// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
        /// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
        /// <typeparam name="TResult">The type of the result elements.</typeparam>
        /// <returns>
        ///     An IEnumerable&lt;T&gt; that contains elements of type TResult that
        ///     are obtained by performing a grouped join on two sequences.
        /// </returns>
        public static IEnumerable<TResult> GroupJoinCore<TOuter, TInner, TKey, TResult>(this IQueryable<TOuter> outer,
            IQueryable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<TOuter, IEnumerable<TInner>, TResult> resultSelector,
            IEqualityComparer<TKey> comparer)
        {
            var w = new Sweetmeat<TOuter, TInner>(outer, inner);
            w.Wait();
            return GroupJoinIterator(w.First, w.Seconds, outerKeySelector, innerKeySelector,
                resultSelector, comparer);
        }


        /// <summary>
        ///     Asynchronous correlates the elements of two sequences based on key equality and groups the results.
        ///     A specified IEqualityComparer&lt;T&gt; is used to compare keys.
        /// </summary>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
        /// <param name="resultSelector">
        ///     A function to create a result element from an element
        ///     from the first sequence and a collection of matching elements from the second sequence.
        /// </param>
        /// <param name="comparer">An IEqualityComparer&lt;T&gt; to hash and compare keys.</param>
        /// <param name="cancellationToken">Object of the canceling to asynchronous operation</param>
        /// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
        /// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
        /// <typeparam name="TResult">The type of the result elements.</typeparam>
        /// <returns>
        ///     An IEnumerable&lt;T&gt; that contains elements of type TResult that
        ///     are obtained by performing a grouped join on two sequences.
        /// </returns>
        public static async Task<IEnumerable<TResult>> GroupJoinCoreAsync<TOuter, TInner, TKey, TResult>(
            this IQueryable<TOuter> outer,
            IQueryable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<TOuter, IEnumerable<TInner>, TResult> resultSelector,
            IEqualityComparer<TKey> comparer,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(outer, nameof(outer));
            Check.NotNull(inner, nameof(inner));
            Check.NotNull(outerKeySelector, nameof(outerKeySelector));
            Check.NotNull(innerKeySelector, nameof(innerKeySelector));
            Check.NotNull(resultSelector, nameof(resultSelector));
            Check.NotNull(comparer, nameof(comparer));
            var w = new Sweetmeat<TOuter, TInner>(outer, inner, cancellationToken);
            await w.WaitAsync();
            return GroupJoinIterator(w.First, w.Seconds, outerKeySelector, innerKeySelector,
                resultSelector, comparer);
        }

        /// <summary>
        ///     Correlates the elements of two sequences based on key equality and groups the results.
        ///     A specified IEqualityComparer&lt;T&gt; is used to compare keys.
        /// </summary>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
        /// <param name="resultSelector">
        ///     A function to create a result element from an element
        ///     from the first sequence and a collection of matching elements from the second sequence.
        /// </param>
        /// <param name="comparer">An IEqualityComparer&lt;T&gt; to hash and compare keys.</param>
        /// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
        /// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
        /// <typeparam name="TResult">The type of the result elements.</typeparam>
        /// <returns>
        ///     An IEnumerable&lt;T&gt; that contains elements of type TResult that
        ///     are obtained by performing a grouped join on two sequences.
        /// </returns>
        public static IEnumerable<TResult> GroupJoinCore<TOuter, TInner, TKey, TResult>(this IQueryable<TOuter> outer,
            IEnumerable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<TOuter, IEnumerable<TInner>, TResult> resultSelector,
            IEqualityComparer<TKey> comparer)
        {
            Check.NotNull(outer, nameof(outer));
            Check.NotNull(inner, nameof(inner));
            Check.NotNull(outerKeySelector, nameof(outerKeySelector));
            Check.NotNull(innerKeySelector, nameof(innerKeySelector));
            Check.NotNull(resultSelector, nameof(resultSelector));
            Check.NotNull(comparer, nameof(comparer));
            var pTOuter = (QueryProvider)outer.Provider;
            var outerS = (IEnumerable<TOuter>)pTOuter.Execute<TOuter>(outer.Expression);
            return GroupJoinIterator(outerS, inner, outerKeySelector, innerKeySelector,
                resultSelector, comparer);
        }


        /// <summary>
        ///     Asynchronous correlates the elements of two sequences based on key equality and groups the results.
        ///     A specified IEqualityComparer&lt;T&gt; is used to compare keys.
        /// </summary>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
        /// <param name="resultSelector">
        ///     A function to create a result element from an element
        ///     from the first sequence and a collection of matching elements from the second sequence.
        /// </param>
        /// <param name="comparer">An IEqualityComparer&lt;T&gt; to hash and compare keys.</param>
        /// <param name="cancellationToken">Object of the canceling to asynchronous operation</param>
        /// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
        /// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
        /// <typeparam name="TResult">The type of the result elements.</typeparam>
        /// <returns>
        ///     An IEnumerable&lt;T&gt; that contains elements of type TResult that
        ///     are obtained by performing a grouped join on two sequences.
        /// </returns>
        public static async Task<IEnumerable<TResult>> GroupJoinCoreAsync<TOuter, TInner, TKey, TResult>(
            this IQueryable<TOuter> outer,
            IEnumerable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<TOuter, IEnumerable<TInner>, TResult> resultSelector,
            IEqualityComparer<TKey> comparer,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(outer, nameof(outer));
            Check.NotNull(inner, nameof(inner));
            Check.NotNull(outerKeySelector, nameof(outerKeySelector));
            Check.NotNull(innerKeySelector, nameof(innerKeySelector));
            Check.NotNull(resultSelector, nameof(resultSelector));
            Check.NotNull(comparer, nameof(comparer));
            var pTOuter = (QueryProvider)outer.Provider;
            var outerS = await QueryableToListAsync(outer, cancellationToken);
            return GroupJoinIterator(outerS, inner, outerKeySelector, innerKeySelector,
                resultSelector, comparer);
        }

        private static IEnumerable<TResult> GroupJoinIterator<TOuter, TInner, TKey, TResult>(
            IEnumerable<TOuter> outer,
            IEnumerable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<TOuter, IEnumerable<TInner>,
                TResult> resultSelector,
            IEqualityComparer<TKey> comparer)
        {
            var rr = inner.ToLookup(innerKeySelector, IdentityFunction<TInner>.Instance, comparer);
            foreach (var item in outer) yield return resultSelector(item, rr[outerKeySelector(item)]);
        }
    }
}