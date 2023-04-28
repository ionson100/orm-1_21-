using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace ORM_1_21_
{

    internal static partial class AttributesOfClass<T>
    {
        public static void SpotRider(IDataReader reader, ProviderName name, T d)
        {
            var list = ListBaseAttrE(name);
            for (int i = 0; i < list.Count; i++)
            {
                var resCore = Pizdaticus.MethodFreeIndex(name, list[i].PropertyType, reader, i);
                SetValueE(name, list[i].PropertyName, d, resCore);
            }

        }

        public static Lazy<Action<IDataReader, ProviderName, T>> SpotRiderLazy = new Lazy<Action<IDataReader, ProviderName, T>>(
            () =>
            {
                ParameterExpression s1 = Expression.Parameter(typeof(IDataReader), "reader");
                ParameterExpression s2 = Expression.Parameter(typeof(ProviderName), "name");
                ParameterExpression s3 = Expression.Parameter(typeof(T));
                MethodInfo miContain = typeof(AttributesOfClass<T>).GetMethod("SpotRider", new[] { typeof(IDataReader), typeof(ProviderName), typeof(T) });
                MethodCallExpression body = Expression.Call(miContain, s1, s2, s3);
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
