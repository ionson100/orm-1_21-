using System.Linq.Expressions;

namespace ORM_1_21_
{
    internal interface IProxy
    {
        string GetTableName(ProviderName providerName);
        WrapperVisitor Translation(ProviderName providerName, Expression expression, int paramIndex);
    }
}