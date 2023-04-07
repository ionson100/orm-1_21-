using ORM_1_21_.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ORM_1_21_.Extensions
{
    public static partial class Helper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <returns></returns>
        public static IEnumerable<TSource> UnionCore<TSource>(this IQueryable<TSource> first, IQueryable<TSource> second)
        {
           
            var p2 = new DbQueryProvider<TSource>((Sessione)((ISqlComposite)first.Provider).Sessione.SessionCloneForTask());
            var t1 = ((QueryProvider)first.Provider).ExecuteExtensionAsync<IEnumerable<TSource>>(first.Expression, null, CancellationToken.None);
            var t2 = p2.ExecuteExtensionAsync<IEnumerable<TSource>>(second.Expression, null, CancellationToken.None);
            Task.WaitAll(t1, t2);
            return UnionIterator(t1.Result, t2.Result, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <returns></returns>
        public static Task<IEnumerable<TSource>> UnionCoreAsync<TSource>(this IQueryable<TSource> first, IQueryable<TSource> second,
            CancellationToken cancellationToken=default)
        {
            var tk = new TaskCompletionSource<IEnumerable<TSource>>();
            var p2 = new DbQueryProvider<TSource>((Sessione)((ISqlComposite)first.Provider).Sessione.SessionCloneForTask());
            var t1 = ((QueryProvider)first.Provider).ExecuteExtensionAsync<IEnumerable<TSource>>(first.Expression, null, cancellationToken);
            var t2 = p2.ExecuteExtensionAsync<IEnumerable<TSource>>(second.Expression, null, cancellationToken);
            Task.WaitAll(t1, t2);
            tk.SetResult(UnionIterator(t1.Result, t2.Result, null));
            return tk.Task;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <returns></returns>
        public static IEnumerable<TSource> UnionCore<TSource>(this IQueryable<TSource> first, IEnumerable<TSource> second)
        {
            var firstR = (IEnumerable < TSource >)((QueryProvider)first.Provider).Execute<TSource>(first.Expression);
            return UnionIterator(firstR, second, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <returns></returns>
        public static async Task<IEnumerable<TSource>> UnionCoreAsync<TSource>(this IQueryable<TSource> first, IEnumerable<TSource> second,
            CancellationToken cancellationToken=default)
        {
            var firstR = (IEnumerable<TSource>)await ((QueryProvider)first.Provider).ExecuteExtensionAsync<TSource>(first.Expression,null,cancellationToken);
            return UnionIterator(firstR, second, null);
        }

        private static IEnumerable<TSource> UnionIterator<TSource>(IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            Set<TSource> set = new Set<TSource>(comparer);
            foreach (TSource item in first)
            {
                if (set.Add(item))
                {
                    yield return item;
                }
            }
            foreach (TSource item2 in second)
            {
                if (set.Add(item2))
                {
                    yield return item2;
                }
            }
        }
    }
}
