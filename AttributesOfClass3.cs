using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ORM_1_21_.geo;

namespace ORM_1_21_
{

    internal static partial class AttributesOfClass<T>
    {
        internal static Lazy<string> StarSql = new Lazy<string>(() =>
        {
            var list = ListBaseAttrE(Provider);
            StringBuilder sb = new StringBuilder();
            foreach (BaseAttribute baseAttribute in list)
            {
                sb.Append($"{baseAttribute.GetColumnName(Provider)}, ");
            }

            return sb.ToString().Trim(' ', ',');
        });
        public static string GetListFieldFreeSqlStar(ProviderName name)
        {
            Provider = name;
            return StarSql.Value;
        }

        public static void SpotRider(IDataReader reader, ProviderName name, T d)
        {
            var list = ListBaseAttrE(name);

            for (int i = 0; i < list.Count; i++)
            {
                var valCore = Pizdaticus.MethodFreeIndex(name, list[i].PropertyType, reader, i);
                if (list[i].IsJson)
                {
                    if (valCore == null)
                    {
                        SetValueE(name, list[i].PropertyName, d, null);
                    }else if ( valCore is string)
                    {
                        var dd = JsonConvert.DeserializeObject(valCore.ToString(), list[i].PropertyType);
                        SetValueE(name, list[i].PropertyName, d, dd);
                    }
                    else 
                    {
                        SetValueE(name, list[i].PropertyName, d, valCore);
                    }
                    

                }
                else if (list[i].IsInheritIGeoShape)
                {
                   
                     //var o= Activator.CreateInstance(list[i].PropertyType);
                    // ((IGeoShape)o).GeoData=resGeoCore.ToString();
                    SetValueE(name, list[i].PropertyName, d,FactoryGeo.CreateGeo(valCore.ToString()));
                }
                else
                {
                    
                    SetValueE(name, list[i].PropertyName, d, valCore);
                }

            }

        }

        public static Lazy<Action<IDataReader, ProviderName, T>> SpotRiderLazy = new Lazy<Action<IDataReader, ProviderName, T>>(
            () =>
            {
                ParameterExpression s1 = Expression.Parameter(typeof(IDataReader), "reader");
                ParameterExpression s2 = Expression.Parameter(typeof(ProviderName), "name");
                ParameterExpression s3 = Expression.Parameter(typeof(T));
                MethodInfo m = typeof(AttributesOfClass<T>).GetMethod("SpotRider");
                MethodCallExpression body = Expression.Call(m, s1, s2, s3);
                Expression<Action<IDataReader, ProviderName, T>> ss = Expression.Lambda<Action<IDataReader, ProviderName, T>>(body, s1, s2, s3);
                return ss.Compile();
            }, LazyThreadSafetyMode.PublicationOnly);


        public static void SpotRiderFree(IDataReader reader, ProviderName name, T d)
        {
            var list = ListBaseAttrE(name);
            for (var i = 0; i < reader.FieldCount; i++)
            {
                var n = reader.GetName(i);
                var s = list.FirstOrDefault(a => a.ColumnNameAlias == n || a.GetColumnNameRaw() == n);
                if (s == null) continue;
                var resCore = Pizdaticus.MethodFreeIndex(name, s.PropertyType, reader, i);
                SetValueE(name, s.PropertyName, d, resCore);
            }

        }




    }
}
