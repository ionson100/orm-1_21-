using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ORM_1_21_.Extensions
{
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
}