using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ORM_1_21_.Linq
{
    internal class ContainerCastExpression
    {
        public Evolution TypeEvolution { get; set; }
        public Expression CustomExpression { get; set; }
        public List<object> ParamList { get; set; }
        public int Timeout { get; set; } = -1;
        public Type TypeReturn { get; set; }
        public IList ListDistinct { get; set; }
        public string Body { get; set; }
        public List<Expression> ListJoinExpressions { get; set; }
    
    }

    
}