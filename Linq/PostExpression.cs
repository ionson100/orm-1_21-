using System.Collections.Generic;
using System.Linq.Expressions;

namespace ORM_1_21_.Linq
{
    class PostExpression
    {
        public PostExpression(Evolution evolution, MethodCallExpression expression)
        {
            CallExpression = expression;
            Evolution = evolution;
            for (var i = 0; i < expression.Arguments.Count; i++)
            {
                if (i == 0) continue;
                ExpressionList.Add(expression.Arguments[i]);
            }
        }
        public MethodCallExpression CallExpression { get; set; }
        public Evolution Evolution { get; set; }
        public List<Expression> ExpressionList { get; set; } = new List<Expression>();
    }
}
