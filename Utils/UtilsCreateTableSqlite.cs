
using System;
using System.Text;

namespace ORM_1_21_.Utils
{
    internal class UtilsCreateTableSqLite
    {
        public static string Create<T>(ProviderName providerName)
        {
            StringBuilder builder = new StringBuilder();

            var tableName = AttributesOfClass<T>.TableNameRaw(providerName);
            builder.AppendLine($"CREATE TABLE IF NOT EXISTS '{tableName}' (");
            var pk = AttributesOfClass<T>.PkAttribute(providerName);
            if (pk.Generator == Generator.Native)
            {
                builder.AppendLine($" '{pk.GetColumnName(providerName)}' INTEGER PRIMARY KEY AUTOINCREMENT,");
            }
            else
            {
                builder.AppendLine($" '{pk.GetColumnName(providerName)}'  BLOB PRIMARY KEY,");
            }

            foreach (MapColumnNameAttribute map in AttributesOfClass<T>.CurrentTableAttributeDall(providerName))
            {
                builder.AppendLine(
                    $" '{map.GetColumnName(providerName)}' {GetTypeColumn(map.TypeColumn)} {FactoryCreatorTable.GetDefaultValue(map.DefaultValue, map.TypeColumn)},");
            }

            string str2 = builder.ToString();
            str2 = str2.Substring(0, str2.LastIndexOf(','));
            builder.Clear();
            builder.Append(str2);
            builder.AppendLine(");");

            foreach (MapColumnNameAttribute map in AttributesOfClass<T>.CurrentTableAttributeDall(providerName))
            {
                if (map.IsIndex)
                {
                    var c = map.GetColumnNameRaw();
                    builder.AppendLine($"CREATE INDEX 'INDEX_{tableName}_{c}' ON '{tableName}' ('{c}');");
                }
            }

            return builder.ToString();
        }
        private static string GetTypeColumn(Type type)
        {
            if (type == typeof(Guid) || type == typeof(Guid?))
            {
                return "TEXT";
            }
            if (type == typeof(long) || type == typeof(long?))
            {
                return "NUMERIC";
            }

            if (type == typeof(int) || type.BaseType == typeof(Enum) || type == typeof(int?))
            {
                return "INTEGER";
            }

            if (type == typeof(UInt16) || type == typeof(UInt16?) || type == typeof(UInt32) ||
                type == typeof(UInt32?) ||
                type == typeof(UInt64) || type == typeof(UInt64?)||type==typeof(UInt16)||type==typeof(Int16?))
            {
                return "INTEGER";
            }

            if (type == typeof(SByte) || type == typeof(SByte?) || type == typeof(Byte) || type == typeof(Byte?))
            {
                return "INTEGER";
            }

            if (type == typeof(char) || type == typeof(char?))
            {
                return "TEXT";
            }
            if (type == typeof(short) || type.BaseType == typeof(short?))
            {
                return "INTEGER";
            }
            if (type == typeof(bool) || type == typeof(bool?))
            {
                return "INTEGER";
            }
            if (type == typeof(decimal) || type == typeof(decimal?))
            {
                return "REAL";
            }
            if (type == typeof(float) || type == typeof(float?))
            {
                return "REAL";
            }

            if (type == typeof(double) || type == typeof(double?))
            {
                return "REAL";
            }

            if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                return "TEXT";
            }

            if (type.BaseType == typeof(Enum))
            {
                return "INT";
            }

            if (type == typeof(string))
            {
                return "TEXT";
            }

            if (type == typeof(byte[]))
            {
                return "BLOB";
            }

            return "TEXT";

        }

    }
}
