
using System;
using System.Text;

namespace ORM_1_21_.Utils
{
    internal class UtilsCreateTableSqLite
    {
        public static string Create<T>(ProviderName providerName, bool isBlob)
        {
            var builder = new StringBuilder();

            var tableName = AttributesOfClass<T>.TableName(providerName);
            builder.AppendLine($"CREATE TABLE IF NOT EXISTS {tableName} (");
            var pk = AttributesOfClass<T>.PkAttribute(providerName);
            if (pk.Generator == Generator.Native || pk.Generator == Generator.NativeNotReturningId)
            {
                var typePk = $"  INTEGER PRIMARY KEY AUTOINCREMENT";
                if (pk.TypeString != null)
                    typePk = pk.TypeString;
                var defValue = "";
                if (pk.DefaultValue != null)
                {
                    defValue = pk.DefaultValue;
                }
                builder.AppendLine($"{pk.GetColumnName(providerName)} {typePk} {defValue},");
            }
            else
            {
                var typePk = $"TEXT PRIMARY KEY";
                if (pk.PropertyType == typeof(Guid))
                {
                    if (isBlob)
                    {
                        typePk = $"BLOB PRIMARY KEY";
                    }
                }

                if (UtilsCore.NumericTypes.Contains(pk.PropertyType))
                {
                    typePk = $"INTEGER PRIMARY KEY";
                }

                if (pk.TypeString != null)
                    typePk = pk.TypeString;
                var defValue = "";
                if (pk.DefaultValue != null)
                {
                    defValue = pk.DefaultValue;
                }
                builder.AppendLine($"{pk.GetColumnName(providerName)} {typePk} {defValue},");
            }


            foreach (var map in AttributesOfClass<T>.CurrentTableAttributeDal(providerName))
            {

                builder.AppendLine(
                    $" {map.GetColumnName(providerName)} {GetTypeColumn(map, isBlob)} {FactoryCreatorTable.GetDefaultValue(map, providerName, isBlob)},");
            }

            var str2 = builder.ToString();
            str2 = str2.Substring(0, str2.LastIndexOf(','));
            builder.Clear();
            builder.Append(str2);
            builder.AppendLine(");");

            foreach (var map in AttributesOfClass<T>.CurrentTableAttributeDal(providerName))
            {
                if (map.IsIndex)
                {
                    var c = map.GetColumnNameRaw();
                    var t = UtilsCore.ClearTrim(tableName);
                    builder.AppendLine($"CREATE INDEX 'INDEX_{t}_{c}' ON '{t}' ('{c}');");
                }
            }

            return builder.ToString();
        }
        private static string GetTypeColumn(MapColumnAttribute map, bool isBlob)
        {
            if (map.TypeString != null) return map.TypeString;
            if (map.IsJson)
            {
                return "JSON";
            }

            var type = UtilsCore.GetCoreType(map.PropertyType);

            if (type == typeof(Guid))
            {
                if (isBlob)
                {
                    return "BLOB";
                }
                return "TEXT";
            }

            if (type == typeof(long))
            {
                return "NUMERIC";
            }

            if (type == typeof(int)
                || type.BaseType == typeof(Enum)
                || type == typeof(UInt16)
                || type == typeof(UInt32)
                || type == typeof(UInt64)
                || type == typeof(UInt16)
                || type == typeof(SByte)
                || type == typeof(bool)
                || type == typeof(short)
                || type == typeof(Byte))
            {
                return "INTEGER";
            }

            if (type == typeof(float)
                || type == typeof(decimal)
                || type == typeof(double))
            {
                return "REAL";
            }

            if (type == typeof(DateTime)
                || type == typeof(string)
                )
            {
                return "TEXT";
            }

            if (type == typeof(Char))
            {
                return "CHARINT";
            }

            if (type == typeof(byte[]))
            {
                return "BLOB";
            }

            return "TEXT";

        }

    }
}
