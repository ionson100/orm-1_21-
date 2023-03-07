using System.Collections.Generic;
using System.Linq.Expressions;

namespace ORM_1_21_.Linq.MsSql
{
    internal interface ITranslate
    {
        Dictionary<string, object> Param { get; set; }
        List<OneComposite> ListOne { get; }
        string Translate(Expression expression, out Evolution ev1);
        string Translate(Expression expression, out Evolution ev1, string par);
        void Translate(Expression expression, Evolution ev, List<object> paramList);
        List<OneComposite> GetListOne();
    }
}