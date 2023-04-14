
using System;
using System.Text;

namespace ORM_1_21_.Utils
{
    internal class UtilsCreateTableMySql
    {
        public static string Create<T>(ProviderName providerName)
        {
            StringBuilder builder = new StringBuilder();

            var tableName = AttributesOfClass<T>.TableNameRaw(providerName);
            builder.AppendLine($"CREATE TABLE IF NOT EXISTS `{tableName}` (");
            var pk = AttributesOfClass<T>.PkAttribute(providerName);
            if (pk.Generator == Generator.Native|| pk.Generator == Generator.NativeNotLastInsert)
            {
                var typePk = $" {GetTypeMySql(pk.TypeColumn)}  PRIMARY KEY AUTO_INCREMENT";
                if (!string.IsNullOrWhiteSpace(pk.TypeString))
                    typePk = pk.TypeString;
                var defValue = "";
                if (!string.IsNullOrWhiteSpace(pk.DefaultValue))
                {
                    defValue = pk.DefaultValue;
                }
                builder.AppendLine($" `{pk.ColumnNameForRider(providerName)}` {typePk}  {defValue},");
            }
            else
            {
                var typePk = $" {GetTypeMySql(pk.TypeColumn)}  PRIMARY KEY";
                if (!string.IsNullOrWhiteSpace(pk.TypeString))
                    typePk = pk.TypeString;
                var defValue = "";
                if (!string.IsNullOrWhiteSpace(pk.DefaultValue))
                {
                    defValue = pk.DefaultValue;
                }
                builder.AppendLine($" `{pk.ColumnNameForRider(providerName)}` {typePk} {defValue}  ,");
            }


            //builder.AppendLine($" `{pk.ColumnNameForRider(providerName)}` {GetTypeMySql(pk.TypeColumn)}  PRIMARY KEY {(pk.Generator == Generator.Native ? "AUTO_INCREMENT" : "")},");
            foreach (MapColumnAttribute map in AttributesOfClass<T>.CurrentTableAttributeDal(providerName))
            {
                string typeColumn = map.TypeString ?? GetTypeMySql(map.TypeColumn);
                string sd =
                    $" `{map.ColumnNameForReader(providerName)}` {typeColumn} {FactoryCreatorTable.GetDefaultValue(map.DefaultValue, map.TypeColumn)} ,";
                builder.AppendLine(sd);
            }

            string str2 = builder.ToString();
            str2 = str2.Substring(0, str2.LastIndexOf(','));
            builder.Clear();
            builder.Append(str2);
            builder.AppendLine(");").Append(AttributesOfClass<T>.GetTypeTable(providerName));

            foreach (MapColumnAttribute map in AttributesOfClass<T>.CurrentTableAttributeDal(providerName))
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
            if (type == typeof(long) || type == typeof(long?))//)
            {
                return "BIGINT";
            }

            if (type == typeof(UInt64) || type == typeof(UInt64?))
            {
                return "BIGINT UNSIGNED";
            }

            if (type == typeof(int) || type.BaseType == typeof(Enum))
            {
                return "MEDIUMINT";
            }

            if (type == typeof(uint) || type.BaseType == typeof(uint?))
            {
                return "MEDIUMINT UNSIGNED";
            }
            if (type == typeof(short) || type.BaseType == typeof(short?))
            {
                return "SMALLINT";
            }

            if (type == typeof(UInt16) || type == typeof(UInt16?))
            {
                return "SMALLINT UNSIGNED";
            }
            if (type == typeof(bool) || type == typeof(bool?))
            {
                return "TINYINT(1)";
            }

            if (type==typeof(Byte)|| type == typeof(Byte?)|| type == typeof(SByte) || type == typeof(SByte?))
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

            if (type == typeof(char) || type == typeof(char?))
            {
                return "CHAR(1)";
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

            if ( type == typeof(byte[]))
            {
                return "BLOB";
            }

            return "VARCHAR(256)";

        }
    }
}
