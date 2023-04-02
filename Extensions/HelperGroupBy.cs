using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ORM_1_21_.Linq;

namespace ORM_1_21_.Extensions
{
    public static partial class Helper
    {
        /// <summary>
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

            return new GroupedEnumerable<TSource, TKey, TElement>(source, keySelector.Compile(),
                elementSelector.Compile(), null);
        }



        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <param name="elementSelector"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TElement"></typeparam>
        /// <returns></returns>
        public static async Task<IEnumerable<IGrouping<TKey, TElement>>> GroupByCoreAsync<TSource, TKey, TElement>(
            this IQueryable<TSource> source,
            Expression<Func<TSource, TKey>> keySelector,
            Expression<Func<TSource, TElement>> elementSelector,
            CancellationToken  cancellationToken=default)
        {
            var tk = new TaskCompletionSource<IEnumerable<IGrouping<TKey, TElement>>>(TaskCreationOptions.RunContinuationsAsynchronously);

            
            var pTSource = new DbQueryProvider<TSource>((Sessione)((ISqlComposite)source.Provider).Sessione);
            var sources = await pTSource.ExecuteExtensionAsync<IEnumerable<TSource>>(source.Expression,null,cancellationToken);
            var res=new GroupedEnumerable<TSource, TKey, TElement>(sources, keySelector.Compile(), elementSelector.Compile(), null);
            tk.SetResult(res);
            return await tk.Task;
        }

        /// <summary>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <param name="elementSelector"></param>
        /// <param name="comparer"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TElement"></typeparam>
        /// <returns></returns>
        public static IEnumerable<IGrouping<TKey, TElement>> GroupByCore<TSource, TKey, TElement>(
            this IQueryable<TSource> source,
            Expression<Func<TSource, TKey>> keySelector,
            Expression<Func<TSource, TElement>> elementSelector,
            IEqualityComparer<TKey> comparer)
        {

            return new GroupedEnumerable<TSource, TKey, TElement>(source, keySelector.Compile(),
                elementSelector.Compile(), comparer);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <param name="elementSelector"></param>
        /// <param name="comparer"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TElement"></typeparam>
        /// <returns></returns>
        public static async Task<IEnumerable<IGrouping<TKey, TElement>>> GroupByCoreAsync<TSource, TKey, TElement>(
            this IQueryable<TSource> source,
            Expression<Func<TSource, TKey>> keySelector,
            Expression<Func<TSource, TElement>> elementSelector,
            IEqualityComparer<TKey> comparer,
            CancellationToken cancellationToken=default)
        {
            var tk = new TaskCompletionSource<IEnumerable<IGrouping<TKey, TElement>>>(TaskCreationOptions.RunContinuationsAsynchronously);
            var p = new DbQueryProvider<TSource>((Sessione)((ISqlComposite)source.Provider).Sessione);
            var sources =  await p.ExecuteExtensionAsync<IEnumerable<TSource>>(source.Expression,null,cancellationToken);
            tk.SetResult(new GroupedEnumerable<TSource, TKey, TElement>(sources, keySelector.Compile(),
                elementSelector.Compile(), comparer));
            return await  tk.Task;
        }

        /// <summary>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <param name="comparer"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <returns></returns>
        public static IEnumerable<IGrouping<TKey, TSource>> GroupByCore<TSource, TKey>(this IQueryable<TSource> source,
            Expression<Func<TSource, TKey>> keySelector, IEqualityComparer<TKey> comparer)
        {
            return new GroupedEnumerable<TSource, TKey, TSource>(source, keySelector.Compile(),
                IdentityFunction<TSource>.Instance, comparer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <param name="comparer"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <returns></returns>
        public static async Task<IEnumerable<IGrouping<TKey, TSource>>> GroupByCoreAsync<TSource, TKey>(this IQueryable<TSource> source,
            Expression<Func<TSource, TKey>> keySelector, IEqualityComparer<TKey> comparer,CancellationToken cancellationToken=default)
        {
            var tk = new TaskCompletionSource<IEnumerable<IGrouping<TKey, TSource>>>();
            var p = new DbQueryProvider<TSource>((Sessione)((ISqlComposite)source.Provider).Sessione);
            var sources =  await p.ExecuteExtensionAsync<IEnumerable<TSource>>(source.Expression,null,cancellationToken);
            tk.SetResult(new GroupedEnumerable<TSource, TKey, TSource>(sources, keySelector.Compile(),
                IdentityFunction<TSource>.Instance, comparer));
            return await tk.Task;
        }

        /// <summary>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <returns></returns>
        public static IEnumerable<IGrouping<TKey, TSource>> GroupByCore<TSource, TKey>(this IQueryable<TSource> source,
            Expression<Func<TSource, TKey>> keySelector
        )
        {

            return new GroupedEnumerable<TSource, TKey, TSource>(
                source,
                keySelector.Compile(),
                IdentityFunction<TSource>.Instance,
                null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <returns></returns>
        public static async Task<IEnumerable<IGrouping<TKey, TSource>>> GroupByCoreSync<TSource, TKey>(this IQueryable<TSource> source,
            Expression<Func<TSource, TKey>> keySelector,CancellationToken cancellationToken=default)
        {
            var tk = new TaskCompletionSource<IEnumerable<IGrouping<TKey, TSource>>>();
            var p = new DbQueryProvider<TSource>((Sessione)((ISqlComposite)source.Provider).Sessione);
            var sources =  await p.ExecuteExtensionAsync<IEnumerable<TSource>>(source.Expression,null,cancellationToken);
            tk.SetResult( new GroupedEnumerable<TSource, TKey, TSource>(
                sources,
                keySelector.Compile(),
                IdentityFunction<TSource>.Instance,
                null));
            return await tk.Task;
        }

        /// <summary>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <param name="resultSelector"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static IEnumerable<TResult> GroupByCore<TSource, TKey, TResult>(this IQueryable<TSource> source,
            Expression<Func<TSource, TKey>> keySelector,
            Expression<Func<TKey, IEnumerable<TSource>, TResult>> resultSelector)
        {
           
            return new GroupedEnumerable<TSource, TKey, TSource, TResult>(
                source,
                keySelector.Compile(),
                IdentityFunction<TSource>.Instance,
                resultSelector.Compile(),
                null);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <param name="resultSelector"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static async Task<IEnumerable<TResult>> GroupByCoreAsync<TSource, TKey, TResult>(this IQueryable<TSource> source,
            Expression<Func<TSource, TKey>> keySelector,
            Expression<Func<TKey, IEnumerable<TSource>, TResult>> resultSelector,CancellationToken cancellationToken=default)
        {
            var tk = new TaskCompletionSource<IEnumerable<TResult>>();
            var p = new DbQueryProvider<TSource>((Sessione)((ISqlComposite)source.Provider).Sessione);
            var sources = await p.ExecuteExtensionAsync<IEnumerable<TSource>>(source.Expression, null, cancellationToken);
            tk.SetResult(new GroupedEnumerable<TSource, TKey, TSource, TResult>(
                sources,
                keySelector.Compile(),
                IdentityFunction<TSource>.Instance,
                resultSelector.Compile(),
                null));
            return await tk.Task;
        }

        /// <summary>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <param name="resultSelector"></param>
        /// <param name="comparer"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static IEnumerable<TResult> GroupByCore<TSource, TKey, TResult>(this IQueryable<TSource> source,
            Expression<Func<TSource, TKey>> keySelector,
            Expression<Func<TKey, IEnumerable<TSource>, TResult>> resultSelector,
            IEqualityComparer<TKey> comparer)
        {
            return new GroupedEnumerable<TSource, TKey, TSource, TResult>(
                source,
                keySelector.Compile(),
                IdentityFunction<TSource>.Instance,
                resultSelector.Compile(),
                comparer);
        }
















        /// <summary>
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
        public static IEnumerable<TResult> GroupJoinCore<TOuter, TInner, TKey, TResult>(this IQueryable<TOuter> outer,
            IQueryable<TInner> inner,
            Expression<Func<TOuter, TKey>> outerKeySelector,
            Expression<Func<TInner, TKey>> innerKeySelector,
            Expression<Func<TOuter, IEnumerable<TInner>, TResult>> resultSelector)
        {
            var pTOuter = (QueryProvider)outer.Provider; 
            var pTInner = new DbQueryProvider<TInner>((Sessione)((ISqlComposite)outer.Provider).Sessione.SessionCloneForTask());
            var t1= pTOuter.ExecuteExtensionAsync<IEnumerable<TOuter>>(outer.Expression,null,CancellationToken.None);
            var t2= pTInner.ExecuteExtensionAsync<IEnumerable<TInner>>(inner.Expression,null,CancellationToken.None);
            Task.WaitAll(t1, t2);
            var outerS = t1.Result;
            var innerS=t2.Result;
            

            return GroupJoinIterator(outerS, innerS, outerKeySelector.Compile(), innerKeySelector.Compile(),
                resultSelector.Compile(), null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outer"></param>
        /// <param name="inner"></param>
        /// <param name="outerKeySelector"></param>
        /// <param name="innerKeySelector"></param>
        /// <param name="resultSelector"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TOuter"></typeparam>
        /// <typeparam name="TInner"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static async Task<IEnumerable<TResult>> GroupJoinCoreAsync<TOuter, TInner, TKey, TResult>(this IQueryable<TOuter> outer,
            IQueryable<TInner> inner,
            Expression<Func<TOuter, TKey>> outerKeySelector,
            Expression<Func<TInner, TKey>> innerKeySelector,
            Expression<Func<TOuter, IEnumerable<TInner>, 
                TResult>> resultSelector,
            CancellationToken cancellationToken=default)
        {
            var tk =new  TaskCompletionSource<IEnumerable<TResult>>(TaskCreationOptions.RunContinuationsAsynchronously);
            var pTOuter = (QueryProvider)outer.Provider;
            var pTInner = new DbQueryProvider<TInner>((Sessione)((ISqlComposite)outer.Provider).Sessione.SessionCloneForTask());
            var outerS = await pTOuter.ExecuteExtensionAsync<IEnumerable<TOuter>>(outer.Expression, null, cancellationToken);
            var innerS =  await pTInner.ExecuteExtensionAsync<IEnumerable<TInner>>(inner.Expression, null, cancellationToken);
           
            tk.SetResult(GroupJoinIterator(outerS, innerS, outerKeySelector.Compile(), innerKeySelector.Compile(),
                resultSelector.Compile(), null));
            return await tk.Task;
        }


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
        public static IEnumerable<TResult> GroupJoinCore<TOuter, TInner, TKey, TResult>(this IQueryable<TOuter> outer,
            IEnumerable<TInner> inner,
            Expression<Func<TOuter, TKey>> outerKeySelector,
            Expression<Func<TInner, TKey>> innerKeySelector,
            Expression<Func<TOuter, IEnumerable<TInner>, TResult>> resultSelector)
        {
            var pTOuter = (QueryProvider)outer.Provider;
            var outerS = (IEnumerable<TOuter>)pTOuter.Execute<TOuter>(outer.Expression);
            return GroupJoinIterator(outerS, inner, outerKeySelector.Compile(), innerKeySelector.Compile(),
                resultSelector.Compile(), null);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outer"></param>
        /// <param name="inner"></param>
        /// <param name="outerKeySelector"></param>
        /// <param name="innerKeySelector"></param>
        /// <param name="resultSelector"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TOuter"></typeparam>
        /// <typeparam name="TInner"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static async Task<IEnumerable<TResult>> GroupJoinCoreAsync<TOuter, TInner, TKey, TResult>(this IQueryable<TOuter> outer,
            IEnumerable<TInner> inner,
            Expression<Func<TOuter, TKey>> outerKeySelector,
            Expression<Func<TInner, TKey>> innerKeySelector,
            Expression<Func<TOuter, IEnumerable<TInner>,
                TResult>> resultSelector,
            CancellationToken cancellationToken=default)
        {
            var tk = new TaskCompletionSource<IEnumerable<TResult>>(TaskCreationOptions.RunContinuationsAsynchronously);
            var pTOuter = (QueryProvider)outer.Provider;
            var outerS = await pTOuter.ExecuteExtensionAsync<IEnumerable<TOuter>>(outer.Expression,null,cancellationToken);
            tk.SetResult( GroupJoinIterator(outerS, inner, outerKeySelector.Compile(), innerKeySelector.Compile(),
                resultSelector.Compile(), null));
            return await tk.Task;

        }



        /// <summary>
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
        public static IEnumerable<TResult> GroupJoinCore<TOuter, TInner, TKey, TResult>(this IQueryable<TOuter> outer,
            IQueryable<TInner> inner,
            Expression<Func<TOuter, TKey>> outerKeySelector,
            Expression<Func<TInner, TKey>> innerKeySelector,
            Expression<Func<TOuter, IEnumerable<TInner>, TResult>> resultSelector,
            IEqualityComparer<TKey> comparer)
        {
            var pTOuter = (QueryProvider)outer.Provider;
            var pTInner = new DbQueryProvider<TInner>((Sessione)((ISqlComposite)outer.Provider).Sessione.SessionCloneForTask());
            var t1 = pTOuter.ExecuteExtensionAsync<IEnumerable<TOuter>>(outer.Expression, null, CancellationToken.None);
            var t2 = pTInner.ExecuteExtensionAsync<IEnumerable<TInner>>(inner.Expression, null, CancellationToken.None);
            Task.WaitAll(t1, t2);
            var outerS = t1.Result;
            var innerS = t2.Result;

            return GroupJoinIterator(outerS, innerS, outerKeySelector.Compile(), innerKeySelector.Compile(),
                resultSelector.Compile(), comparer);
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
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TOuter"></typeparam>
        /// <typeparam name="TInner"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static async Task<IEnumerable<TResult>> GroupJoinCoreAsync<TOuter, TInner, TKey, TResult>(this IQueryable<TOuter> outer,
            IQueryable<TInner> inner,
            Expression<Func<TOuter, TKey>> outerKeySelector,
            Expression<Func<TInner, TKey>> innerKeySelector,
            Expression<Func<TOuter, IEnumerable<TInner>, TResult>> resultSelector,
            IEqualityComparer<TKey> comparer,
            CancellationToken cancellationToken=default)
        {
            var tk =new TaskCompletionSource<IEnumerable<TResult>>(TaskCreationOptions.RunContinuationsAsynchronously);
            var pTOuter = (QueryProvider)outer.Provider;
            var pTInner = new DbQueryProvider<TInner>((Sessione)((ISqlComposite)outer.Provider).Sessione.SessionCloneForTask());
            var outerS  = await pTOuter.ExecuteExtensionAsync<IEnumerable<TOuter>>(outer.Expression, null, cancellationToken);
            var innerS = await pTInner.ExecuteExtensionAsync<IEnumerable<TInner>>(inner.Expression, null, cancellationToken);
            tk.SetResult(GroupJoinIterator(outerS, innerS, outerKeySelector.Compile(), innerKeySelector.Compile(),
                resultSelector.Compile(), comparer));
            return await tk.Task;
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
        public static IEnumerable<TResult> GroupJoinCore<TOuter, TInner, TKey, TResult>(this IQueryable<TOuter> outer,
            IEnumerable<TInner> inner,
            Expression<Func<TOuter, TKey>> outerKeySelector,
            Expression<Func<TInner, TKey>> innerKeySelector,
            Expression<Func<TOuter, IEnumerable<TInner>, TResult>> resultSelector,
            IEqualityComparer<TKey> comparer)
        {
            var pTOuter = (QueryProvider)outer.Provider;
            var outerS =(IEnumerable<TOuter>) pTOuter.Execute<TOuter>(outer.Expression);
            return GroupJoinIterator(outerS, inner, outerKeySelector.Compile(), innerKeySelector.Compile(),
                resultSelector.Compile(), comparer);
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
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TOuter"></typeparam>
        /// <typeparam name="TInner"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static async Task<IEnumerable<TResult>> GroupJoinCoreAsync<TOuter, TInner, TKey, TResult>(this IQueryable<TOuter> outer,
            IEnumerable<TInner> inner,
            Expression<Func<TOuter, TKey>> outerKeySelector,
            Expression<Func<TInner, TKey>> innerKeySelector,
            Expression<Func<TOuter, IEnumerable<TInner>, TResult>> resultSelector,
            IEqualityComparer<TKey> comparer,
            CancellationToken cancellationToken=default)
        {
            var tk = new TaskCompletionSource<IEnumerable<TResult>>(TaskCreationOptions.RunContinuationsAsynchronously);
            var pTOuter = (QueryProvider)outer.Provider;
            var outerS =  await pTOuter.ExecuteExtensionAsync<IEnumerable<TOuter>>(outer.Expression,null,cancellationToken);
            tk.SetResult(GroupJoinIterator(outerS, inner, outerKeySelector.Compile(), innerKeySelector.Compile(),
                resultSelector.Compile(), comparer));
            return await tk.Task;
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

    internal class IdentityFunction<T>
    {
        public static Func<T, T> Instance => x => x;
    }


    internal class GroupedEnumerable<TSource, TKey, TElement> :
        IEnumerable<IGrouping<TKey, TElement>>
    {
        private readonly IEqualityComparer<TKey> _comparer;
        private readonly Func<TSource, TElement> _elementSelector;
        private readonly Func<TSource, TKey> _keySelector;
        private readonly IEnumerable<TSource> _source;

        public GroupedEnumerable(
            IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector,
            IEqualityComparer<TKey> comparer)
        {
            _source = source;
            _keySelector = keySelector;
            _elementSelector = elementSelector;
            _comparer = comparer;
        }

        public IEnumerator<IGrouping<TKey, TElement>> GetEnumerator()
        {
            return Create(_source, _keySelector, _elementSelector, _comparer).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal static IEnumerable<IGrouping<TKey, TElement>> Create(
            IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector,
            IEqualityComparer<TKey> comparer)
        {
            var rr = source.ToLookup(keySelector, elementSelector, comparer);
            return rr;
        }
    }

    internal class GroupedEnumerable<TSource, TKey, TElement, TResult> : IEnumerable<TResult>, IEnumerable
    {
        private readonly IEqualityComparer<TKey> _comparer;

        private readonly Func<TSource, TElement> _elementSelector;

        private readonly Func<TSource, TKey> _keySelector;

        private readonly Func<TKey, IEnumerable<TElement>, TResult> _resultSelector;
        private readonly IEnumerable<TSource> _source;

        public GroupedEnumerable(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource,
                TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector,
            IEqualityComparer<TKey> comparer)
        {
            _source = source;
            _keySelector = keySelector;
            _elementSelector = elementSelector;
            _comparer = comparer;
            _resultSelector = resultSelector;
        }

        public IEnumerator<TResult> GetEnumerator()
        {
            var rr = _source.ToLookup(_keySelector, _elementSelector, _comparer);
            foreach (var elements in rr) yield return _resultSelector(elements.Key, elements);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}