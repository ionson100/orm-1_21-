
using System;
using System.Text;

namespace ORM_1_21_.Utils
{
    internal class UtilsCreateTableMsSql
    {
        public static string Create<T>(ProviderName providerName)
        {
            var tableName = AttributesOfClass<T>.TableNameRaw(providerName);
            StringBuilder builder = new StringBuilder($"IF not exists (select 1 from information_schema.tables where table_name = '{tableName}')");
            builder.AppendLine($"CREATE TABLE [dbo].[{tableName}](");
            var pk = AttributesOfClass<T>.PkAttribute(providerName);
            if (pk.Generator == Generator.Native|| pk.Generator == Generator.NativeNotLastInsert)
            {
                var typePk = $" {GetTypeMsSQl(pk.TypeColumn)}IDENTITY(1,1) NOT NULL";
                if (!string.IsNullOrWhiteSpace(pk.TypeString))
                    typePk = pk.TypeString;
                var defValue = "PRIMARY KEY";
                if (!string.IsNullOrWhiteSpace(pk.DefaultValue))
                {
                    defValue = pk.DefaultValue;
                }
                builder.AppendLine($"[{pk.ColumnNameForRider(providerName)}]  {typePk} {defValue},");
            }
            if (pk.Generator == Generator.Assigned)
            {
                var typePk = "uniqueIdentifier default (newId())";
                if (!string.IsNullOrWhiteSpace(pk.TypeString))
                    typePk = pk.TypeString;
                var defValue = "PRIMARY KEY";
                if (!string.IsNullOrWhiteSpace(pk.DefaultValue))
                {
                    defValue = pk.DefaultValue;
                }

                builder.AppendLine($"[{pk.ColumnNameForRider(providerName)}]  {typePk} {defValue},");
            }


            foreach (MapColumnAttribute map in AttributesOfClass<T>.CurrentTableAttributeDal(providerName))
            {
                var typeUser = map.TypeString;
                if (typeUser == null)
                {
                    builder.AppendLine($" [{map.ColumnNameForReader(providerName)}] {GetTypeMsSQl(map.TypeColumn)} {FactoryCreatorTable.GetDefaultValue(map.DefaultValue, map.TypeColumn)},");
                }
                else
                {
                    builder.AppendLine(
                        $" [{map.ColumnNameForReader(providerName)}] {typeUser} {FactoryCreatorTable.GetDefaultValue(map.DefaultValue, map.TypeColumn)},");
                }
            }

            string res = builder.ToString().Trim(' ', ',') + ");";
            builder.Clear().Append(res);
            foreach (MapColumnAttribute map in AttributesOfClass<T>.CurrentTableAttributeDal(providerName))
            {
                if (map.IsIndex)
                {
                    builder.AppendLine(
                        $"CREATE INDEX [INDEX_{tableName}_{map.GetColumnNameRaw()}] ON [{tableName}] ({map.GetColumnNameRaw()});");
                }
            }

            return builder.ToString();
        }
        private static string GetTypeMsSQl(Type type)
        {
            if (type == typeof(long) || type == typeof(long?))
            {
                return "[bigint]";
            }
            if (type == typeof(int) || type.BaseType == typeof(Enum) || type == typeof(int?))
            {
                return "[INT]";
            }

            if (type == typeof(UInt32) || type == typeof(UInt32?))
            {
                return "[INT]";
            }
            if (type == typeof(Int16) || type == typeof(Int16?))
            {
                return "[SMALLINT]";
            }

            if (type == typeof(UInt16) || type == typeof(UInt16?))
            {
                return "[SMALLINT]";
            }

            if (type == typeof(UInt64) || type == typeof(UInt64?))
            {
                return "[bigint]";
            }

            if (type == typeof(Byte) || type == typeof(Byte?) || type == typeof(SByte) || type == typeof(SByte?))
            {
                return "[TINYINT]";
            }

            if (type == typeof(char) || type == typeof(char?))
            {
                return "[char](1)";
            }
            if (type == typeof(bool) || type == typeof(bool?))//real
            {
                return "[BIT]";
            }
            if (type == typeof(decimal) || type == typeof(decimal?))
            {
                return "[decimal]";
            }
            if (type == typeof(float) || type == typeof(float?))
            {
                return "[float]";
            }

            if (type == typeof(double) || type == typeof(double?))
            {
                return "[float]";
            }

            if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                return "[DATETIME]";
            }


            if (type == typeof(Guid))
            {
                return "[uniqueidentifier]";
            }
            var st = UtilsCore.GetSerializeType(type);
            if (st==SerializeType.User)
            {
                return "[nvarchar] (max)";
            }

            if ( type == typeof(byte[]))
            {
                return "varbinary(MAX)";
            }
            return "[NVARCHAR] (256)";
        }
    }
}
