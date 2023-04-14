using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ORM_1_21_.Linq
{

    class LambdaFactory
    {
        public static object GetResult<TIn>(IEnumerable  enumerable, Expression key)
        {

            var tt = typeof(TIn);
           
            LambdaExpression lambda = (LambdaExpression)StripQuotes(key);
            
            if (lambda.ReturnType == typeof(Int32))
            {
                //var res=new GroupedEnumerable<TIn, Int32, TIn>(enumerable, (Func<TIn, Int32>)lambda.Compile(), Funcs, null).ToList();
               
            

               
                var sss = (Func<object, int>)lambda.Compile();
                return ((IEnumerable<object>) enumerable).GroupBy(sss);
            }
          
            return null;
        }
        private static Expression StripQuotes(Expression e)
        {
            while (e.NodeType == ExpressionType.Quote)
            {
                e = ((UnaryExpression)e).Operand;
            }
            return e;
        }
    }
  
}
