using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ORM_1_21_.Linq
{
    internal class ContainerCastExpression
    {
        public Evolution TypeRevalytion { get; set; }
        public Expression CastomExpression { get; set; }
        public List<object> ParamList { get; set; }
        public int Timeout { get; set; } = -1;
        public Type TypeRetyrn { get; set; }
        public IList ListDistict { get; set; }
    }
}