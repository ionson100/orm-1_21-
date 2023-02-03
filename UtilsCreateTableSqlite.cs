using ORM_1_21_.Attribute;
using System;
using System.Text;

namespace ORM_1_21_
{
    internal class UtilsCreateTableSqlite
    {
        public static string Create<T>(ProviderName providerName)
        {
            StringBuilder builder = new StringBuilder();

            var tableName = AttributesOfClass<T>.TableName;
            tableName = tableName.Replace("[", "").Replace("]", "").Replace("`", "");
            builder.AppendLine($"CREATE TABLE  {tableName} (");
            var pk = AttributesOfClass<T>.PkAttribute;
            if (pk.Generator == Generator.Native)
            {
                //
                builder.AppendLine($" {pk.ColumnNameForRider(providerName)}  INTEGER PRIMARY KEY AUTOINCREMENT,");
            }
            else
            {
                builder.AppendLine($" {pk.ColumnNameForRider(providerName)}  {GetTypeColumn(pk.TypeColumn)}  PRIMARY KEY,");
            }



            foreach (MapColumnNameAttribute map in AttributesOfClass<T>.CurrentTableAttributeDall)
            {
                builder.AppendLine($" {map.ColumnNameForReader(providerName)} {GetTypeColumn(map.TypeColumn)} {FactoryCreatorTable.GetDefaultValue(map.DefaultValue, map.TypeColumn)} ,");
            }


            string str2 = builder.ToString();
            str2 = str2.Substring(0, str2.LastIndexOf(','));
            builder.Clear();
            builder.Append(str2);
            builder.AppendLine(");");

            //CREATE INDEX index_name ON table_name (column_name);

            StringBuilder indexBuilder = new StringBuilder($"CREATE INDEX INDEX_{tableName} ON {tableName} (");

            bool add = false;
            foreach (MapColumnNameAttribute map in AttributesOfClass<T>.CurrentTableAttributeDall)
            {
                if (map.IsIndex)
                {
                    add = true;
                    indexBuilder.AppendLine(map.GetColumnName(providerName)).Append(",");
                }
            }

            if (add == true)
            {
                string index = indexBuilder.ToString().Substring(0, indexBuilder.ToString().LastIndexOf(',')).Trim() + ");";
                builder.AppendLine(index);
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
                return "NUM";
            }

            if (type == typeof(int) || type.BaseType == typeof(Enum) || type == typeof(int?) || type == typeof(uint) || type.BaseType == typeof(uint?))
            {
                return "INT";
            }
            if (type == typeof(short) || type.BaseType == typeof(short?))
            {
                return "INT";
            }
            if (type == typeof(bool) || type == typeof(bool?))
            {
                return "INT";
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

            if (type == typeof(System.Drawing.Image) || type == typeof(byte[]))
            {
                return "BLOB";
            }

            return "TEXT";

        }

    }
}
