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
    public sealed class Query<T> : IOrderedQueryable<T>, IGetTypeQuery,IInnerList
    {

        public object GetInnerList()
        {
            return  _provider.Execute<T>(_expression);
        }
        /// <summary>
        /// Провайдер
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
            _provider = provider ?? throw new ArgumentNullException("provider");
            _expression = Expression.Constant(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="expression"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Query(QueryProvider provider, Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            if (!typeof(IQueryable<T>).IsAssignableFrom(expression.Type))
            {
                throw new ArgumentOutOfRangeException("expression");
            }

            _provider = provider ?? throw new ArgumentNullException("provider");

            _expression = expression;
        }

        internal Query(QueryProvider provider, Expression expression,int i)
        {
            //if (!typeof(IQueryable<T>).IsAssignableFrom(expression.Type))
            //{
            //    throw new ArgumentOutOfRangeException("expression");
            //}
            _provider = provider ?? throw new ArgumentNullException("provider");
            _expression = expression ?? throw new ArgumentNullException("expression");
        }

        Expression IQueryable.Expression
        {
            get { return _expression; }
        }

        Type IQueryable.ElementType => typeof(T);

        IQueryProvider IQueryable.Provider => _provider;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return ( (IEnumerable<T>)_provider.Execute<T>(_expression)).GetEnumerator();
                      
        }
       
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ( (IEnumerable)_provider.Execute(_expression)).GetEnumerator();
                      
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _provider.GetQueryText(_expression);
        }

        internal string GetJoinParam(List<OneComprosite> comprosites, Dictionary<string, object> dictionary,string parSimvol)
        {
            return _provider.GetQueryTextForJoin(_expression, comprosites, dictionary,parSimvol);
        }

    
        /// <summary>
        /// Тип 
        /// </summary>
        /// <returns></returns>
        public Type GetTypeQuery()
        {
            return typeof (T);
        }

      
    }
}
