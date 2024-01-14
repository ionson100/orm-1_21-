using ORM_1_21_.geo;
using ORM_1_21_.Linq;
using ORM_1_21_.Utils;
using System;
using System.Linq;
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
       
       //public static IQueryable<T> GeoWhereST_Within<T>(this IQueryable<T> query, Expression<Func<T, IGeoShape>> selector, IGeoShape geoObj,bool actionResult=true)
       //{
       //    Check.NotNull(query, nameof(query));
       //    Check.NotNull(selector, nameof(selector));
       //    Check.NotNull(geoObj, nameof(geoObj));
       //    var provider = (DbQueryProvider<T>)query.Provider;
       //    ProviderName providerName = provider.Sessione.ProviderName;
       //    var geo = Expression.Constant(geoObj);
       //    var nameColumnE = GetNameColumnCore(selector,providerName);
       //    var isNotE = Expression.Constant(actionResult);
       //    var selectorParameters = selector.Parameters;
       //    Expression check = Expression.Call(null,typeof(V).GetMethod("GeoST_Within"), nameColumnE,  geo,isNotE);
       //    var lambada = Expression.Lambda<Func<T, bool>>(check, selectorParameters);
       //    return query.Where(lambada);
       //}


         static ConstantExpression GetNameColumnCore<T>(Expression<Func<T, IGeoShape>> selector,
             ProviderName providerName)
        {
            var m = (selector.Body as MemberExpression);
            var nameColumn = GetColumnNameSimple<T>(m.Member.Name, providerName);
            var nameTable = GetTableName<T>(providerName);
            return Expression.Constant($"{nameTable}.{nameColumn}");
        }

        
        private static string GetColumnNameSimple<T>(string member,  ProviderName providerName)
        {
            var ss = AttributesOfClass<T>.GetNameSimpleColumnForQuery(member,providerName);
            return ss;
        }
        private static string GetTableName<T>(ProviderName providerName)
        {
            var ss = AttributesOfClass<T>.TableName(providerName);
            return ss;
        }

    }
}
