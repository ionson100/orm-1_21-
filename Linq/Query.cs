using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ORM_1_21_.Linq
{


    interface IInnerList
    {
        object GetInnerList();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class Query<T> : IOrderedQueryable<T>, IGetTypeQuery, IInnerList
    {
        /// <summary>
        /// Collection iteration function
        /// </summary>
        public void ForEach(Action<T> action)
        {
            foreach (var item in this)
            {
                action(item);
            }
        }
        
        object IInnerList.GetInnerList()
        {
            return _provider.Execute<T>(_expression);
        }
      
        /// <summary>
        /// Provider
        /// </summary>
        public readonly QueryProvider _provider;
        private readonly Expression _expression;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <exception cref="ArgumentNullException"></exception>
        internal Query(QueryProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _expression = Expression.Constant(this);
        }

        
        internal Query(QueryProvider provider, Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }
            if (!typeof(IQueryable<T>).IsAssignableFrom(expression.Type))
            {
                throw new ArgumentOutOfRangeException(nameof(expression));
            }

            _provider = provider ?? throw new ArgumentNullException(nameof(provider));

            _expression = expression;
        }

        

        Expression IQueryable.Expression => _expression;

        Type IQueryable.ElementType => typeof(T);

        IQueryProvider IQueryable.Provider => _provider;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)_provider.Execute<T>(_expression)).GetEnumerator();

        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_provider.Execute(_expression)).GetEnumerator();

        }
        /// <summary>
        /// Textual interpretation of a database query
        /// </summary>
        public override string ToString()
        {
            return _provider.GetQueryText(_expression);
        }

        Type IGetTypeQuery.GetTypeQuery()
        {
            return typeof(T);
        }
    }
}
