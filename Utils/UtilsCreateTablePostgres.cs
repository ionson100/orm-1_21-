using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using ORM_1_21_.geo;
using ORM_1_21_.Linq;

namespace ORM_1_21_.Utils
{
    internal class UtilsCreateTablePostgres
    {
        public static string Create<T>(ProviderName providerName)
        {
            var builder = new StringBuilder();
            var tableName = AttributesOfClass<T>.TableName(providerName);
            
            builder.AppendLine($"CREATE TABLE IF NOT EXISTS {tableName} (");
            var pk = AttributesOfClass<T>.PkAttribute(providerName);

            var typePk = GetTypePgPk(pk.PropertyType, pk.Generator);
            if (pk.TypeString != null)
                typePk = pk.TypeString;
            var defValue = "PRIMARY KEY";
            if (pk.DefaultValue != null)
            {
                defValue = pk.DefaultValue;
            }

            builder.AppendLine($" {pk.GetColumnName(providerName)} {typePk}  {defValue},");

            foreach (MapColumnAttribute map in AttributesOfClass<T>.CurrentTableAttributeDal(providerName))
            {
                builder.AppendLine(
                    $" {map.GetColumnName(providerName)} {GetTypePg(map)} {FactoryCreatorTable.GetDefaultValue(map,providerName)} ,");
            }
            var str2 = builder.ToString();
            str2 = str2.Substring(0, str2.LastIndexOf(','));
            builder.Clear();
            builder.Append(str2);
            builder.AppendLine(");");
            var tableNameRaw = AttributesOfClass<T>.TableNameRaw(providerName);
            foreach (var map in AttributesOfClass<T>.CurrentTableAttributeDal(providerName))
                if (map.IsIndex)
                {
                    var colName = map.GetColumnNameRaw();
                    if (map.IsInheritIGeoShape)
                    {
                        builder.AppendLine(
                            $"CREATE INDEX IF NOT EXISTS  \"idx_{tableNameRaw}_{colName}_geom\" ON \"{tableNameRaw}\" USING gist (\"{colName}\");");
                        
                    }else if (map.IsJson)
                    {
                        builder.AppendLine(
                            $"CREATE INDEX IF NOT EXISTS \"idx_{tableNameRaw}_{colName}_json\" ON \"{tableNameRaw}\" USING GIN (\"{colName}\");");
                    }
                    else
                    {
                        builder.AppendLine(
                            $"CREATE INDEX IF NOT EXISTS  \"INDEX_{tableNameRaw}_{colName}\" ON {tableName} (\"{colName}\");");
                    }
                    
                    
                }

            return builder.ToString();
        }


        private static string GetTypePg(MapColumnAttribute map)
        {
            if (map.TypeString != null) return map.TypeString;
            if (map.IsInheritIGeoShape)
            {
                return "geometry";
            }

            if (map.IsJson)
            {
                return "jsonb";
            }

            var type = UtilsCore.GetCoreType(map.PropertyType);
            if (type == null) return null;
            if (type.GetInterfaces().Contains(typeof(IGeoShape)) )
            {
                return "geometry";
            }

            if (type == typeof(long) ||
                type == typeof(ulong))
                return "BIGINT";

            if (type == typeof(int) ||
                type.BaseType == typeof(Enum) ||
                type == typeof(Enum) ||
                type == typeof(uint) ||
                type == typeof(ushort) ||
                type == typeof(short)
                )
                return "INTEGER";

            if (type == typeof(bool))
                return "BOOLEAN";

            if (type == typeof(decimal))
                return "decimal";

            if (type == typeof(float))
                return "NUMERIC";

            if (type == typeof(double))
                return "double precision";

            if (type == typeof(DateTime))
                return "TIMESTAMP";

            if (type == typeof(Guid))
                return "UUID";

            if (type == typeof(byte) ||
                type == typeof(sbyte))
                return "smallint";

            if (type == typeof(char))
                return "character(1)";

            if (type == typeof(byte[]))
                return "BYTEA";

            return "VARCHAR(256)";
        }

        private static string GetTypePgPk(Type type, Generator generator)
        {
            type = UtilsCore.GetCoreType(type);
            if (generator == Generator.Assigned)
            {
                if (type == typeof(long)) return "BIGINT";
                if (type == typeof(int) || type.BaseType == typeof(Enum) ) return "INTEGER";
                if (type == typeof(Guid)) return "UUID";
            }

            if (generator == Generator.Native || generator == Generator.NativeNotReturningId)
            {
                if (type == typeof(long) || type == typeof(ulong)) return "BIGSERIAL";
                if (type == typeof(int) || type == typeof(uint) || type.BaseType == typeof(Enum)) return "SERIAL";
                if (type == typeof(Guid)) return "UUID";
            }

            return "SERIAL";
        }
    }
}