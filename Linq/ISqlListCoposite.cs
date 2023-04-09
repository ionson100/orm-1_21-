using System;
using System.Collections.Generic;
using System.Data;

namespace ORM_1_21_.Linq
{
    internal interface ISqlComposite
    {
       
        ISession Sessione { get; }
        ISessionInner SessioneInner { get; }
        IDbTransaction Transaction { get; set; }
        List<ContainerCastExpression> ListCastExpression { get; }
        Type GetSourceType();
        
    }

    interface IGetTypeGetTypeGeneric
    {
        Type GetTypeGetTypeGeneric();
    }
}
