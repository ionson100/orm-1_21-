using System;
using System.Text;

namespace ORM_1_21_.Utils
{
    internal class UtilsCreateTablePostgres
    {
        public static string Create<T>(ProviderName providerName)
        {
            var builder = new StringBuilder();
            var tableName = AttributesOfClass<T>.TableNameRaw(providerName);
            builder.AppendLine($"CREATE TABLE IF NOT EXISTS \"{tableName}\" (");
            var pk = AttributesOfClass<T>.PkAttribute(providerName);

            var typePk = GetTypePgPk(pk.TypeColumn, pk.Generator);
            if (!string.IsNullOrWhiteSpace(pk.TypeString))
                typePk = pk.TypeString;
            var defValue = "PRIMARY KEY";
            if (!string.IsNullOrWhiteSpace(pk.DefaultValue))
            {
                defValue = pk.DefaultValue;
            }

            builder.AppendLine($" \"{UtilsCore.ClearTrim(pk.ColumnNameForRider(providerName))}\" {typePk}  {defValue},");


            foreach (var map in AttributesOfClass<T>.CurrentTableAttributeDal(providerName))
            {
                if (map.TypeString == null)
                {
                    builder.AppendLine(
                        $" \"{UtilsCore.ClearTrim(map.ColumnNameForReader(providerName))}\" {GetTypePg(map.TypeColumn)} {FactoryCreatorTable.GetDefaultValue(map.DefaultValue, map.TypeColumn)} ,");
                }
                else
                {
                    builder.AppendLine(
                        $" \"{UtilsCore.ClearTrim(map.ColumnNameForReader(providerName))}\" {map.TypeString} ,");
                }
            }

            var str2 = builder.ToString();
            str2 = str2.Substring(0, str2.LastIndexOf(','));
            builder.Clear();
            builder.Append(str2);
            builder.AppendLine(");");

            foreach (var map in AttributesOfClass<T>.CurrentTableAttributeDal(providerName))
                if (map.IsIndex)
                {
                    var colName = map.GetColumnNameRaw();

                    builder.AppendLine(
                        $"CREATE INDEX IF NOT EXISTS \"INDEX_{tableName}_{colName}\" ON \"{tableName}\" (\"{colName}\");");
                }

            return builder.ToString();
        }


        private static string GetTypePg(Type type)
        {
            if (type == typeof(long) ||
                type == typeof(long?)|| 
                type == typeof(ulong?)||
                type == typeof(ulong)) 
                return "BIGINT";
            if (type == typeof(int) ||
                type.BaseType == typeof(Enum) ||
                type == typeof(Enum) ||
                type == typeof(int?)||
                type == typeof(uint?)||
                type == typeof(uint)||
                type == typeof(ushort?)|| 
                type == typeof(ushort)||
                type == typeof(short)||
                type == typeof(short?)
                ) return "INTEGER";
            if (type == typeof(bool) || 
                type == typeof(bool?)) //real
                return "BOOLEAN";
            if (type == typeof(decimal)|| 
                type == typeof(decimal?))
                return "decimal";
            if (type == typeof(float)||
                type == typeof(float?))
                return "NUMERIC";

            if (type == typeof(double)||
                type == typeof(double?))
                return "double precision";

            if (type == typeof(DateTime)||
                type == typeof(DateTime?)) 
                return "TIMESTAMP";

            if (type == typeof(Guid)|| type == typeof(Guid?)) 
                return "UUID";
            if (type == typeof(byte) || 
                type == typeof(byte?)||
                type == typeof(sbyte)|| 
                type == typeof(sbyte?))
                return "smallint";
            if (type == typeof(char) || type == typeof(char?))
                return "character(1)";
            var st = UtilsCore.GetSerializeType(type);
            if (st==SerializeType.User) 
                return "TEXT";

            if (type == typeof(byte[])) 
                return "BYTEA";


            return "VARCHAR(256)";
        }

        private static string GetTypePgPk(Type type, Generator generator)
        {
            if (generator == Generator.Assigned)
            {
                if (type == typeof(long) || type == typeof(long?)) return "BIGINT";
                if (type == typeof(int) || type.BaseType == typeof(Enum) || type == typeof(int?)) return "INTEGER";
                if (type == typeof(Guid)) return "UUID";
            }

            if (generator == Generator.Native||generator == Generator.NativeNotLastInsert)
            {
                if (type == typeof(long) || type == typeof(ulong) || type == typeof(long?)) return "BIGSERIAL";
                if (type == typeof(int) || type == typeof(uint) || type.BaseType == typeof(Enum) || type == typeof(int?)) return "SERIAL";
                if (type == typeof(Guid)) return "UUID";
            }

            return "SERIAL";
        }
    }
}