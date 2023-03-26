using System;
using System.Collections.Generic;
using System.Data;

namespace ORM_1_21_.Linq
{
    interface ISqlComposite
    {
       
        ISession Sessione { get; }
        IDbTransaction Transaction { get; set; }
        List<ContainerCastExpression> ListCastExpression { get; }
        Type GetSourceType();
        
    }

    interface IGetTypeGetTypeGeneric
    {
        Type GetTypeGetTypeGeneric();
    }
}
