using ORM_1_21_.Transaction;
using System;
using System.Collections.Generic;
using System.Data;

namespace ORM_1_21_.Linq
{
    interface ISqlComposite
    {
        Transactionale Transactionale { get; set; }
        ISession Sessione { get; }
        IDbTransaction Transaction { get; set; }
        List<ContainerCastExpression> ListCastExpression { get; }
    }

    interface IGetTypeGetTypeGeneric
    {
        Type GetTypeGetTypeGeneric();
    }
}
