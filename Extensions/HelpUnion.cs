using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ORM_1_21_.Extensions;
using ORM_1_21_.Utils;

namespace ORM_1_21_
{
    [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
    public static partial class Helper
    {
        /// <summary>
        ///     Produces the set union of two sequences by using the default equality comparer as Primary key property.
        /// </summary>
        /// <param name="first">An IQueryable&lt;T&gt; whose distinct elements form the first set for the union.</param>
        /// <param name="second">An IQueryable&lt;T&gt; whose distinct elements form the second set for the union.</param>
        /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
        /// <returns>
        ///     An IEnumerable&lt;T&gt; that contains the elements from both input sequences, excluding duplicates (Primary
        ///     key).
        /// </returns>
        public static IEnumerable<TSource> UnionCore<TSource>(this IQueryable<TSource> first,
            IQueryable<TSource> second)
        {
            Check.NotNull(first, nameof(first));
            Check.NotNull(second, nameof(second));
            var w = new Sweetmeat<TSource, TSource>(first, second);
            w.Wait();
            return UnionIterator(w.First, w.Seconds, null);
        }


        /// <summary>
        ///     Asynchronously produces the set union of two sequences by using the default equality comparer as Primary key
        ///     property.
        /// </summary>
        /// <param name="first">An IQueryable&lt;T&gt; whose distinct elements form the first set for the union.</param>
        /// <param name="second">An IQueryable&lt;T&gt; whose distinct elements form the second set for the union.</param>
        /// <param name="cancellationToken">Asynchronous operation cancel object</param>
        /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
        /// <returns>
        ///     An IEnumerable&lt;T&gt; that contains the elements from both input sequences, excluding duplicates (Primary
        ///     key).
        /// </returns>
        public static async Task<IEnumerable<TSource>> UnionCoreAsync<TSource>(this IQueryable<TSource> first,
            IQueryable<TSource> second,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(first, nameof(first));
            Check.NotNull(second, nameof(second));
            var w = new Sweetmeat<TSource, TSource>(first, second,cancellationToken);
            await w.WaitAsync();
            return UnionIterator(w.First, w.Seconds, null);
        }


        /// <summary>
        ///     Produces the set union of two sequences by using the default equality comparer as Primary key property.
        /// </summary>
        /// <param name="first">An IQueryable&lt;T&gt; whose distinct elements form the first set for the union.</param>
        /// <param name="second">An IEnumerable&lt;T&gt; whose distinct elements form the second set for the union.</param>
        /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
        /// <returns>
        ///     An IEnumerable&lt;T&gt; that contains the elements from both input sequences, excluding duplicates (Primary
        ///     key).
        /// </returns>
        public static IEnumerable<TSource> UnionCore<TSource>(this IQueryable<TSource> first,
            IEnumerable<TSource> second)
        {
            Check.NotNull(first, nameof(first));
            Check.NotNull(second, nameof(second));
            return UnionIterator(first, second, null);
        }

        /// <summary>
        ///     Asynchronously produces the set union of two sequences by using the default equality comparer as Primary key
        ///     property.
        /// </summary>
        /// <param name="first">An IQueryable&lt;T&gt; whose distinct elements form the first set for the union.</param>
        /// <param name="second">An IEnumerable&lt;T&gt; whose distinct elements form the second set for the union.</param>
        /// <param name="cancellationToken">Asynchronous operation cancel object</param>
        /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
        /// <returns>
        ///     An IEnumerable&lt;T&gt; that contains the elements from both input sequences, excluding duplicates (Primary
        ///     key).
        /// </returns>
        public static async Task<IEnumerable<TSource>> UnionCoreAsync<TSource>(this IQueryable<TSource> first,
            IEnumerable<TSource> second,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(first, nameof(first));
            Check.NotNull(second, nameof(second));
            var firstR = await QueryableToListAsync(first, cancellationToken);
            return UnionIterator(firstR, second, null);
        }


        /// <summary>
        ///     Produces the set union of two sequences by using a specified IEqualityComparer&lt;T&gt;.
        /// </summary>
        /// <param name="first">An IQueryable&lt;T&gt; whose distinct elements form the first set for the union.</param>
        /// <param name="second">An IQueryable&lt;T&gt; whose distinct elements form the second set for the union.</param>
        /// <param name="comparer">The IEqualityComparer&lt;T&gt; to compare values.</param>
        /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
        /// <returns>An IEnumerable&lt;T&gt; that contains the elements from both input sequences, excluding duplicates.</returns>
        public static IEnumerable<TSource> UnionCore<TSource>(this IQueryable<TSource> first,
            IQueryable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            Check.NotNull(first, nameof(first));
            Check.NotNull(second, nameof(second));
            var w = new Sweetmeat<TSource, TSource>(first, second);
            w.Wait();
            return UnionIterator(w.First, w.Seconds, comparer);
        }


        /// <summary>
        ///     Asynchronously Produces the set union of two sequences by using a specified IEqualityComparer&lt;T&gt;.
        /// </summary>
        /// <param name="first">An IQueryable&lt;T&gt; whose distinct elements form the first set for the union.</param>
        /// <param name="second">An IQueryable&lt;T&gt; whose distinct elements form the second set for the union.</param>
        /// <param name="comparer">The IEqualityComparer&lt;T&gt; to compare values.</param>
        /// <param name="cancellationToken">Asynchronous operation cancel object</param>
        /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
        /// <returns>An IEnumerable&lt;T&gt; that contains the elements from both input sequences, excluding duplicates.</returns>
        public static async Task<IEnumerable<TSource>> UnionCoreAsync<TSource>(this IQueryable<TSource> first,
            IQueryable<TSource> second,
            IEqualityComparer<TSource> comparer,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(first, nameof(first));
            Check.NotNull(second, nameof(second));
            Check.NotNull(comparer, nameof(comparer));
            var w = new Sweetmeat<TSource, TSource>(first, second,cancellationToken);
            await w.WaitAsync();
            return UnionIterator(w.First, w.Seconds, comparer);
        }


        /// <summary>
        ///     Produces the set union of two sequences by using a specified IEqualityComparer&lt;T&gt;.
        /// </summary>
        /// <param name="first">An IQueryable&lt;T&gt; whose distinct elements form the first set for the union.</param>
        /// <param name="second">An IEnumerable&lt;T&gt; whose distinct elements form the second set for the union.</param>
        /// <param name="comparer">The IEqualityComparer&lt;T&gt; to compare values.</param>
        /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
        /// <returns>An IEnumerable&lt;T&gt; that contains the elements from both input sequences, excluding duplicates.</returns>
        public static IEnumerable<TSource> UnionCore<TSource>(this IQueryable<TSource> first,
            IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            Check.NotNull(first, nameof(first));
            Check.NotNull(second, nameof(second));
            Check.NotNull(comparer, nameof(comparer));
            return UnionIterator(first, second, comparer);
        }

        /// <summary>
        ///     Asynchronously produces the set union of two sequences by using a specified IEqualityComparer&lt;T&gt;..
        /// </summary>
        /// <param name="first">An IQueryable&lt;T&gt; whose distinct elements form the first set for the union.</param>
        /// <param name="second">An IEnumerable&lt;T&gt; whose distinct elements form the second set for the union.</param>
        /// <param name="comparer">The IEqualityComparer&lt;T&gt; to compare values.</param>
        /// <param name="cancellationToken">Asynchronous operation cancel object</param>
        /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
        /// <returns>An IEnumerable&lt;T&gt; that contains the elements from both input sequences, excluding duplicates.</returns>
        public static async Task<IEnumerable<TSource>> UnionCoreAsync<TSource>(this IQueryable<TSource> first,
            IEnumerable<TSource> second,
            IEqualityComparer<TSource> comparer,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(first, nameof(first));
            Check.NotNull(second, nameof(second));
            Check.NotNull(comparer, nameof(comparer));
            var firstR = await QueryableToListAsync(first, cancellationToken);
            return UnionIterator(firstR, second, comparer);
        }


        private static IEnumerable<TSource> UnionIterator<TSource>(IEnumerable<TSource> first,
            IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            if (UtilsCore.IsValid<TSource>() && comparer == null)
            {
                var mySet = new MySet<TSource>();
                foreach (var item in first)
                    if (mySet.Add(item))
                        yield return item;
                foreach (var item2 in second)
                    if (mySet.Add(item2))
                        yield return item2;
            }
            else
            {
                var set = new Set<TSource>(comparer);
                foreach (var item in first)
                    if (set.Add(item))
                        yield return item;
                foreach (var item2 in second)
                    if (set.Add(item2))
                        yield return item2;
            }
        }



        /// <summary>
        /// Applies a specified function to the corresponding elements of two sequences, producing a sequence of the results.
        /// </summary>
        /// <param name="first">The first sequence to merge.</param>
        /// <param name="second">The second sequence to merge.</param>
        /// <param name="resultSelector">A function that specifies how to merge the elements from the two sequences.</param>
        /// <typeparam name="TFirst">The type of the elements of the first input sequence.</typeparam>
        /// <typeparam name="TSecond">The type of the elements of the second input sequence.</typeparam>
        /// <typeparam name="TResult">The type of the elements of the third input sequence.</typeparam>
        /// <returns>An IEnumerable&lt;T&gt; that contains merged elements of two input sequences.</returns>
        public static IEnumerable<TResult> ZipCore<TFirst, TSecond, TResult>(this IQueryable<TFirst> first, 
            IQueryable<TSecond> second,
            Func<TFirst, TSecond, 
                TResult> resultSelector)
        {
            Check.NotNull(first, nameof(first));
            Check.NotNull(second, nameof(second));
            Check.NotNull(resultSelector, nameof(resultSelector));
            var w = new Sweetmeat<TFirst, TSecond>(first, second);
            w.Wait();
            return ZipIterator(w.First, w.Seconds, resultSelector);
        }
        /// <summary>
        /// Applies a specified function to the corresponding elements of two sequences, producing a sequence of the results.
        /// </summary>
        /// <param name="first">The first sequence to merge.</param>
        /// <param name="second">The second sequence to merge.</param>
        /// <param name="resultSelector">A function that specifies how to merge the elements from the two sequences.</param>
        /// <typeparam name="TFirst">The type of the elements of the first input sequence.</typeparam>
        /// <typeparam name="TSecond">The type of the elements of the second input sequence.</typeparam>
        /// <typeparam name="TResult">The type of the elements of the third input sequence.</typeparam>
        /// <returns>An IEnumerable&lt;T&gt; that contains merged elements of two input sequences.</returns>
        public static IEnumerable<TResult> ZipCore<TFirst, TSecond, TResult>(this IQueryable<TFirst> first,
            IEnumerable<TSecond> second,
            Func<TFirst, TSecond,
                TResult> resultSelector)
        {
            Check.NotNull(first, nameof(first));
            Check.NotNull(second, nameof(second));
            Check.NotNull(resultSelector, nameof(resultSelector));
            return ZipIterator(first, second, resultSelector);
        }


        /// <summary>
        /// Applies a specified function to the corresponding elements of two sequences,asynchronous producing a sequence of the results.
        /// </summary>
        /// <param name="first">The first sequence to merge.</param>
        /// <param name="second">The second sequence to merge.</param>
        /// <param name="resultSelector">A function that specifies how to merge the elements from the two sequences.</param>
        /// <param name="cancellationToken">Asynchronous operation cancel object</param>
        /// <typeparam name="TFirst">The type of the elements of the first input sequence.</typeparam>
        /// <typeparam name="TSecond">The type of the elements of the second input sequence.</typeparam>
        /// <typeparam name="TResult">The type of the elements of the third input sequence.</typeparam>
        /// <returns>An IEnumerable&lt;T&gt; that contains merged elements of two input sequences.</returns>
        public static async Task<IEnumerable<TResult>> ZipCoreAsync<TFirst, TSecond, TResult>(this IQueryable<TFirst> first,
            IQueryable<TSecond> second,
            Func<TFirst, TSecond,
                TResult> resultSelector,CancellationToken cancellationToken=default)
        {
            Check.NotNull(first, nameof(first));
            Check.NotNull(second, nameof(second));
            Check.NotNull(resultSelector, nameof(resultSelector));
            var w = new Sweetmeat<TFirst,TSecond>(first, second, cancellationToken);
            await w.WaitAsync();
            return ZipIterator(w.First, w.Seconds, resultSelector);
        }

        /// <summary>
        ///  Applies a specified function to the corresponding elements of two sequences,asynchronous producing a sequence of the results.
        /// </summary>
        /// <param name="first">The first sequence to merge.</param>
        /// <param name="second">The second sequence to merge.</param>
        /// <param name="resultSelector">A function that specifies how to merge the elements from the two sequences.</param>
        /// <param name="cancellationToken">Asynchronous operation cancel object</param>
        /// <typeparam name="TFirst">The type of the elements of the first input sequence.</typeparam>
        /// <typeparam name="TSecond">The type of the elements of the second input sequence.</typeparam>
        /// <typeparam name="TResult">The type of the elements of the third input sequence.</typeparam>
        /// <returns>An IEnumerable&lt;T&gt; that contains merged elements of two input sequences.</returns>
        public static async Task<IEnumerable<TResult>> ZipCoreAsync<TFirst, TSecond, TResult>(this IQueryable<TFirst> first,
            IEnumerable<TSecond> second,
            Func<TFirst, TSecond,
                TResult> resultSelector, CancellationToken cancellationToken = default)
        {
            Check.NotNull(first, nameof(first));
            Check.NotNull(second, nameof(second));
            Check.NotNull(resultSelector, nameof(resultSelector));
            var firstS = await QueryableToListAsync(first, cancellationToken);
            return ZipIterator(firstS, second, resultSelector);
        }

        private static IEnumerable<TResult> ZipIterator<TFirst, TSecond, TResult>(IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
        {
            var  e1 = first.GetEnumerator();
            var e2 = second.GetEnumerator();
            while (e1.MoveNext() && e2.MoveNext())
            {
                yield return resultSelector(e1.Current, e2.Current);
            }
            
        }


    }
}