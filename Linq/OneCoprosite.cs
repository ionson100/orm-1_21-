using System;

namespace ORM_1_21_.Linq
{
    internal class OneComprosite
    {


        internal Evolution Operand { get; set; }
        private string _body;

        public object NewConstructor { get; set; }

        public bool IsAggregate { get; set; }

        public string Body
        {
            get => _body;
            set => _body = value.TrimEnd(',');
        }
        public Delegate ExpressionDelegate { get; set; }
    }


}
