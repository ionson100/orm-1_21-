using System.Linq.Expressions;
using ORM_1_21_.Linq;

namespace ORM_1_21_
{
    internal class ProxyAttribute<T> : IProxy
    {
        public string GetTableName(ProviderName providerName)
        {
            return AttributesOfClass<T>.TableName(providerName);
        }

        public WrapperVisitor Translation(ProviderName providerName, Expression expression, int paramIndex)
        {
            var res = new QueryTranslator<T>(providerName, paramIndex);
            res.Translate(expression);
            WrapperVisitor visitor = new WrapperVisitor
            {
                ListOneComposite = res.GetListOne(),
                Params = res.Param,
                ParamsIndex = res.GetParamIndex(),
                FieldName = res.FieldOne()
            };
            return visitor;

        }
    }
}