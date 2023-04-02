using System;
using System.Linq.Expressions;

namespace ORM_1_21_.Linq
{
    internal class OneComposite
    {


        internal Evolution Operand { get; set; }
        private string _body;

        public Expression NewConstructor { get; set; }

        public bool IsAggregate { get; set; }

        public string Body
        {
            get => _body;
            set => _body = value.TrimEnd(',');
        }
        public Delegate ExpressionDelegate { get; set; }
    }


}
