using System;
using System.Globalization;
using System.Linq.Expressions;

namespace ORM_1_21_.Linq
{
    class JoinAlias : ExpressionVisitor
    {
        private string _alias;
        public string GetAlias(Expression m)
        {
            Visit(m);
            return _alias;
        }
        protected override Expression VisitUnary(UnaryExpression u)
        {
            switch (u.NodeType)
            {


                case ExpressionType.Quote:
                    Visit(u.Operand);
                    break;

                default:
                    throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture,
                        "The unary operator '{0}' is not supported", u.NodeType));
            }
            return u;
        }

        protected override Expression VisitMemberAccess(MemberExpression m)
        {

            if (m.Expression != null
                && m.Expression.NodeType == ExpressionType.Parameter)
            {

                _alias = (string)m.Expression.GetType().GetProperty("Name").GetValue(m.Expression, null);

                return m;
            }
            if (m.Expression != null
                && m.Expression.NodeType == ExpressionType.New)
            {

                _alias = (string)m.Expression.GetType().GetProperty("Name").GetValue(m.Expression, null);
                return m;
            }

            if (m.Expression != null
             && m.Expression.NodeType == ExpressionType.Constant)
            {
                return m;
            }
            return m;
        }
    }
}
