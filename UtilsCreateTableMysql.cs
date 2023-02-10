using ORM_1_21_.Attribute;
using System;
using System.Text;

namespace ORM_1_21_
{
    internal class UtilsCreateTableMySql
    {
        public static string Create<T>(ProviderName providerName)
        {
            StringBuilder builder = new StringBuilder();

            var tableName = AttributesOfClass<T>.TableNameRaw(providerName);
            builder.AppendLine($"CREATE TABLE IF NOT EXISTS `{tableName}` (");
            var pk = AttributesOfClass<T>.PkAttribute(providerName);

            builder.AppendLine($" `{pk.ColumnNameForRider(providerName)}` {GetTypeMySql(pk.TypeColumn)}  " +
                               $"PRIMARY KEY {(pk.Generator == Generator.Native ? "AUTO_INCREMENT" : "")},");
            foreach (MapColumnNameAttribute map in AttributesOfClass<T>.CurrentTableAttributeDall(providerName))
            {
                string typeColumn = map.TypeString;
                if (typeColumn == null)
                {
                    typeColumn = GetTypeMySql(map.TypeColumn);
                }

                builder.AppendLine(
                    $" `{map.ColumnNameForReader(providerName)}` {typeColumn} {FactoryCreatorTable.GetDefaultValue(map.DefaultValue, map.TypeColumn)} ,");
            }

            string str2 = builder.ToString();
            str2 = str2.Substring(0, str2.LastIndexOf(','));
            builder.Clear();
            builder.Append(str2);
            builder.AppendLine(");").Append(AttributesOfClass<T>.GetTypeTable(providerName));

            foreach (MapColumnNameAttribute map in AttributesOfClass<T>.CurrentTableAttributeDall(providerName))
            {
                if (map.IsIndex)
                {
                    builder.AppendLine(
                        $"ALTER TABLE `{tableName}` ADD INDEX `INDEX_{tableName}_{map.GetColumnNameRaw()}` ({map.GetColumnNameRaw()});");
                }
            }

            return builder.ToString();
        }

        private static string GetTypeMySql(Type type)
        {
            if (type == typeof(Guid) || type == typeof(Guid?))
            {
                return "VARCHAR(36)";
            }
            if (type == typeof(long) || type == typeof(long?))
            {
                return "BIGINT";
            }

            if (type == typeof(int) || type.BaseType == typeof(Enum) || type == typeof(int?) || type == typeof(uint) || type.BaseType == typeof(uint?))
            {
                return "INT(11)";
            }
            if (type == typeof(short) || type.BaseType == typeof(short?))
            {
                return "SMALLINT";
            }
            if (type == typeof(bool) || type == typeof(bool?))
            {
                return "TINYINT(1)";
            }
            if (type == typeof(decimal) || type == typeof(decimal?))
            {
                return "DECIMAL(10,2)";
            }
            if (type == typeof(float) || type == typeof(float?))
            {
                return "FLOAT";
            }

            if (type == typeof(double) || type == typeof(double?))
            {
                return "DOUBLE";
            }

            if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                return "DATETIME";
            }

            if (type.BaseType == typeof(Enum))
            {
                return "ENUM";
            }

            if (type == typeof(string))
            {
                return "VARCHAR(256)";
            }

            if (type == typeof(System.Drawing.Image) || type == typeof(byte[]))
            {
                return "BLOB";
            }

            return "TEXT";

        }
    }
}
