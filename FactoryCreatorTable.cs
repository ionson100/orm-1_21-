using ORM_1_21_.Utils;
using System;
using ORM_1_21_;

namespace ORM_1_21_
{
   
    internal class FactoryCreatorTable
    {
        public string SqlCreate<T>(ProviderName providerName,bool isBlob)
        {
            switch (providerName)
            {
                case ProviderName.MySql:
                    return UtilsCreateTableMySql.Create<T>(providerName,isBlob);
                case ProviderName.MsSql:
                    return UtilsCreateTableMsSql.Create<T>(providerName);
                case ProviderName.PostgreSql:
                    return UtilsCreateTablePostgres.Create<T>(providerName);
                case ProviderName.SqLite:
                    return UtilsCreateTableSqLite.Create<T>(providerName,isBlob);
                default:
                    return null;
            }
        }

        public static string GetDefaultValue(MapColumnAttribute map, ProviderName providerName, bool isBlob=false)
        {
            if (map.DefaultValue != null)
            {
                return map.DefaultValue;
            }

            var type = map.PropertyType;
            if (type.IsGenericType && UtilsCore.IsNullableType(type))
            {
                return "NULL";
            }

            type = UtilsCore.GetCoreType(type);
            if (type == typeof(long))
            {
                return "NOT NULL DEFAULT '0'";
            }

           
            if (type == typeof(int)
                || type.BaseType == typeof(Enum)
                || type == typeof(long)
                || type == typeof(uint)
                || type == typeof(ulong)
                || type == typeof(short)
                || type == typeof(ushort)
                || type == typeof(bool)
                || type == typeof(decimal)
                || type == typeof(float)
                || type == typeof(double))
            {
                return "NOT NULL DEFAULT '0'";
            }

            if (type == typeof(DateTime))
            {
                if (providerName == ProviderName.MsSql)
                {
                    return $"NOT NULL DEFAULT '{Configure.Utils.DateToString(Configure.Utils.DefaultSqlDateTime())}'";
                }
                return $"NOT NULL DEFAULT '{Configure.Utils.DateToString(DateTime.MinValue)}'";
            }
            if (type == typeof(char))
            {
                return $"NOT NULL DEFAULT  ' '";
            }

            if (type == typeof(Guid))
            {
               

                if (isBlob)
                {
                    if (providerName == ProviderName.SqLite)
                    {
                        var s= "x'" + BitConverter.ToString(Guid.Empty.ToByteArray()).Replace("-", "") + "'";
                        return $"NOT NULL DEFAULT {s}";
                    }

                    if (providerName == ProviderName.MySql)
                    {
                        return $"NOT NULL DEFAULT (uuid_to_bin('{Guid.Empty}',1))";
                    }
                }
                return $"NOT NULL DEFAULT '{Guid.Empty}'";



            }
            return "NULL";
        }
    }
}

