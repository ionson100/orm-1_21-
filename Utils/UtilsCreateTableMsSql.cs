
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
            if (pk.Generator == Generator.Native|| pk.Generator == Generator.NativeNotReturningId)
            {
                var typePk = $" {GetTypeMsSQl(pk)}IDENTITY(1,1) NOT NULL";
                if (pk.TypeString != null)
                    typePk = pk.TypeString;
                var defValue = "PRIMARY KEY";
                if (pk.DefaultValue!=null)
                {
                    defValue = pk.DefaultValue;
                }
                builder.AppendLine($"{pk.GetColumnName(providerName)} {typePk} {defValue},");
            }
            if (pk.Generator == Generator.Assigned)
            {
                var typePk = "uniqueIdentifier default (newId())";
                if (pk.TypeString != null)
                    typePk = pk.TypeString;
                var defValue = "PRIMARY KEY";
                if (pk.DefaultValue != null)
                {
                    defValue = pk.DefaultValue;
                }

                builder.AppendLine($"{pk.GetColumnName(providerName)} {typePk} {defValue},");
            }

           
            foreach (MapColumnAttribute map in AttributesOfClass<T>.CurrentTableAttributeDal(providerName))
            {
             
                builder.AppendLine($"{map.GetColumnName(providerName)} {GetTypeMsSQl(map)} {FactoryCreatorTable.GetDefaultValue(map,providerName)},");
               
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
        private static string GetTypeMsSQl(BaseAttribute map)
        {
            if (map.TypeString != null) return map.TypeString;
            var type = UtilsCore.GetCoreType(map.PropertyType);
            if (type == typeof(long))
            {
                return "[bigint]";
            }
            if (type == typeof(int) || type.BaseType == typeof(Enum))
            {
                return "[INT]";
            }

            if (type == typeof(UInt32))
            {
                return "[INT]";
            }
            if (type == typeof(Int16))
            {
                return "[SMALLINT]";
            }

            if (type == typeof(UInt16))
            {
                return "[SMALLINT]";
            }

            if (type == typeof(UInt64))
            {
                return "[bigint]";
            }

            if (type == typeof(Byte)|| type == typeof(SByte) )
            {
                return "[TINYINT]";
            }

            if (type == typeof(char))
            {
                return "[CHAR](1)";
            }
            if (type == typeof(bool))
            {
                return "[BIT]";
            }
            if (type == typeof(decimal))
            {
                return "[decimal](10,2)";
            }
            if (type == typeof(float))
            {
                return "[REAL]";
            }

            if (type == typeof(double))
            {
                return "[float]";
            }

            if (type == typeof(DateTime))
            {
                return "[DATETIME]";
            }

            if (type == typeof(Guid))
            {
                return "[uniqueidentifier]";
            }

            if ( type == typeof(byte[]))
            {
                return "varbinary(MAX)";
            }
            return "[NVARCHAR] (256)";
        }
    }
}
