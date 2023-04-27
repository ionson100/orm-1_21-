
using System;
using System.Text;

namespace ORM_1_21_.Utils
{
    internal class UtilsCreateTableMySql
    {
        public static string Create<T>(ProviderName providerName,bool isBlob)
        {
            StringBuilder builder = new StringBuilder();

            var tableName = AttributesOfClass<T>.TableName(providerName);
            builder.AppendLine($"CREATE TABLE IF NOT EXISTS {tableName} (");
            MapPrimaryKeyAttribute pk = AttributesOfClass<T>.PkAttribute(providerName);
            if (pk.Generator == Generator.Native|| pk.Generator == Generator.NativeNotReturningId)
            {
                var typePk = $" {GetTypeMySql(pk,false)}  PRIMARY KEY AUTO_INCREMENT";
                if (pk.TypeString != null)
                    typePk = pk.TypeString;
                var defValue = "";
                if (pk.DefaultValue != null)
                {
                    defValue = pk.DefaultValue;
                }
                builder.AppendLine($" {pk.GetColumnName(providerName)} {typePk}  {defValue},");
            }
            else
            {
                var typePk = $" {GetTypeMySql(pk,isBlob)}  PRIMARY KEY";
                if (pk.TypeString != null)
                    typePk = pk.TypeString;
                var defValue = "";
                if (pk.DefaultValue != null)
                {
                    defValue = pk.DefaultValue;
                }
                builder.AppendLine($" {pk.GetColumnName(providerName)} {typePk} {defValue}  ,");
            }


            foreach (MapColumnAttribute map in AttributesOfClass<T>.CurrentTableAttributeDal(providerName))
            {
                builder.AppendLine($" {map.GetColumnName(providerName)} {GetTypeMySql(map,isBlob)} {FactoryCreatorTable.GetDefaultValue(map,providerName,isBlob)} ,");
            }

            string str2 = builder.ToString();
            str2 = str2.Substring(0, str2.LastIndexOf(','));
            builder.Clear();
            builder.Append(str2);
            builder.AppendLine(");").Append(AttributesOfClass<T>.GetTypeTable(providerName));

            var tableNameRaw= AttributesOfClass<T>.TableNameRaw(providerName);
            foreach (MapColumnAttribute map in AttributesOfClass<T>.CurrentTableAttributeDal(providerName))
            {
                if (map.IsIndex)
                {
                    builder.AppendLine(
                        $"ALTER TABLE {tableName} ADD INDEX `INDEX_{tableNameRaw}_{map.GetColumnNameRaw()}` ({map.GetColumnNameRaw()});");
                }
            }

            var res = builder.ToString();
            return res;
        }

        private static string GetTypeMySql(BaseAttribute map,bool isBlob)
        {
            if (map.TypeString != null) return map.TypeString;

            var type = UtilsCore.GetCoreType(map.PropertyType);

            if (type == typeof(Guid))
            {
                return isBlob ? "BINARY(16)" : "VARCHAR(36)";
            }

            if (type == typeof(long))//)
            {
                return "BIGINT";
            }

            if (type == typeof(UInt64))
            {
                return "BIGINT UNSIGNED";
            }

            if (type == typeof(int) || type.BaseType == typeof(Enum))
            {
                return "INT";
            }
          
            if (type == typeof(uint))
            {
                return "MEDIUMINT UNSIGNED";
            }
            if (type == typeof(short))
            {
                return "SMALLINT";
            }

            if (type == typeof(UInt16))
            {
                return "SMALLINT UNSIGNED";
            }
            if (type == typeof(bool))
            {
                return "TINYINT(1)";
            }

            if (type==typeof(Byte)|| type == typeof(SByte))
            {
                return "TINYINT(1)";
            }
          
            if (type == typeof(decimal))
            {
                return "DECIMAL(10,2)";
            }
            if (type == typeof(float))
            {
                return "FLOAT";
            }

            if (type == typeof(char))
            {
                return "CHAR(1)";
            }

            if (type == typeof(double))
            {
                return "DOUBLE";
            }

            if (type == typeof(DateTime))
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
