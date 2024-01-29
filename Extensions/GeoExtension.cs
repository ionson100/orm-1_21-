using ORM_1_21_.geo;
using System;
using System.Linq.Expressions;

namespace ORM_1_21_.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class GeoExtension
    {


        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="providerName"></param>
        /// <returns></returns>
        public static string GetTypeName(this GeoType type, ProviderName providerName)
        {
            switch (providerName)
            {
                case ProviderName.MsSql:
                    return type.ToString();
                case ProviderName.MySql:
                    if (type == GeoType.GeometryCollection)
                    {
                        return "GEOMCOLLECTION";
                    }
                    return type.ToString().ToUpper();
                case ProviderName.PostgreSql:
                    return $"ST_{type}";

            }


            return null;
        }

        static ConstantExpression GetNameColumnCore<T>(Expression<Func<T, IGeoShape>> selector,
            ProviderName providerName)
        {
            var m = (selector.Body as MemberExpression);
            var nameColumn = GetColumnNameSimple<T>(m.Member.Name, providerName);
            var nameTable = GetTableName<T>(providerName);
            return Expression.Constant($"{nameTable}.{nameColumn}");
        }


        private static string GetColumnNameSimple<T>(string member, ProviderName providerName)
        {
            var ss = AttributesOfClass<T>.GetNameSimpleColumnForQuery(member, providerName);
            return ss;
        }
        private static string GetTableName<T>(ProviderName providerName)
        {
            var ss = AttributesOfClass<T>.TableName(providerName);
            return ss;
        }

    }
}
