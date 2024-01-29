using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ORM_1_21_.Linq
{
    class FactoryExpression
    {
        public class ParametersExtractorVisitor : ExpressionVisitor
        {
            public IList<ParameterExpression> ExtractedParameters { get; } = new List<ParameterExpression>();
            protected override Expression VisitParameter(ParameterExpression node)
            {
                ExtractedParameters.Add(node);
                return base.VisitParameter(node);
            }
        }
        private static object GetValue(MethodCallExpression expression)
        {
            var objectMember = Expression.Convert(expression, typeof(object));

            var getterLambda = Expression.Lambda<Func<object>>(objectMember);

            var getter = getterLambda.Compile();

            return getter();
        }

        public static dynamic GetData<T>(IEnumerable<T> o, List<PostExpression> postExpressionList)
        {
            object res = null;
            int i = 0;
            RecursionAction(o, postExpressionList, i, ref res, o.First());
            return null;

        }

        private static void RecursionAction<T>(IEnumerable o, List<PostExpression> postExpressionList, int i, ref object res, T t)
        {

            //  if (i+1>postExpressionList.Count) return;
            //  var p = postExpressionList[i];
            //  if (p.Evolution == Evolution.GroupBy)
            //  {
            //      var type = typeof(T);
            //      var ress = LambdaFactory.GetResult<T>(o, p.ExpressionList[0]);
            //      
            //      RecursionAction((IEnumerable) ress,postExpressionList, ++i,ref res,First((IEnumerable)ress));
            //  }


        }

        public static object First(IEnumerable items)
        {
            IEnumerator iter = items.GetEnumerator();
            iter.MoveNext();
            return iter.Current;
        }


        private static IEnumerable<object> GroupBy<TIn>(IEnumerable<TIn> source, List<Expression> expressionList, out Object res)
        {

            if (expressionList.Count == 1)
            {
                res = LambdaFactory.GetResult<TIn>(source, expressionList[0]);


                return (IEnumerable<object>)res;
            }
            var lambdaSelect = (LambdaExpression)StripQuotes(expressionList[0]);
            _ = (Func<TIn, object>)lambdaSelect.Compile();

            res = null;
            return (IEnumerable<IGrouping<object, TIn>>)res;



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
