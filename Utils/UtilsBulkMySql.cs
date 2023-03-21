using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace ORM_1_21_.Utils
{
    internal class UtilsBulkMySql
    {
        private readonly ProviderName _providerName;

        public UtilsBulkMySql(ProviderName providerName)
        {
            _providerName = providerName;
        }

        public string GetSql<T>(IEnumerable<T> list, string fileCsv, string fieldTerminator)
        {
            if (fileCsv != null)
            {
                return SqlFile(list, fileCsv, fieldTerminator);
            }
            return SqlSimple(list);
        }


        public string GetSql<T>(IEnumerable<T> list)
        {
            return SqlSimple(list);
        }

        private string SqlFile<T>(IEnumerable<T> list, string fileCsv, string fieldTerminator)
        {
            StringBuilder sql = new StringBuilder($"LOAD DATA INFILE '{fileCsv}'");
            sql.AppendLine($"INTO TABLE {AttributesOfClass<T>.TableName(_providerName)}");
            sql.AppendLine($"FIELDS TERMINATED BY '{fieldTerminator}'");
            sql.AppendLine("ENCLOSED BY '\"'");
            sql.AppendLine("LINES TERMINATED BY '\n'");
            sql.AppendLine("IGNORE 1 ROWS");
            StringBuilder builder = new StringBuilder();
            bool isAddPk = AttributesOfClass<T>.PkAttribute(_providerName).Generator != Generator.Native;

            StringBuilder rowHead = new StringBuilder();
            if (isAddPk)
            {
                rowHead.Append(AttributesOfClass<T>.PkAttribute(_providerName).GetColumnName(_providerName))
                    .Append(";");
            }
            foreach (var map in AttributesOfClass<T>.CurrentTableAttributeDall(_providerName))
            {
                rowHead.Append(map.GetColumnName(_providerName)).Append(";");
            }

            builder.Append(rowHead.ToString().Substring(0, rowHead.ToString().LastIndexOf(';')))
                .Append(Environment.NewLine);



            foreach (var ob in list)
            {
                StringBuilder row = new StringBuilder();
                if (isAddPk)
                {
                    var o = AttributesOfClass<T>.GetValueE(_providerName, AttributesOfClass<T>.PkAttribute(_providerName).PropertyName, ob);
                    Type type = AttributesOfClass<T>.PropertyInfoList.Value[AttributesOfClass<T>.PkAttribute(_providerName).PropertyName].PropertyType;
                    row.Append(GetValue(o, type)).Append(";");
                }
                foreach (var map in AttributesOfClass<T>.CurrentTableAttributeDall(_providerName))
                {
                    var o = AttributesOfClass<T>.GetValueE(_providerName, map.PropertyName, ob);
                    Type type = AttributesOfClass<T>.PropertyInfoList.Value[map.PropertyName].PropertyType;
                    if (type == typeof(Image) || type == typeof(byte[]))
                    {
                        continue;
                    }
                    string str = GetValue(o, type);
                    row.Append(str).Append(";");
                }

                string s = row.ToString().Substring(0, row.ToString().LastIndexOf(";", StringComparison.Ordinal)) + "\n";
                builder.Append(s);
            }
            File.WriteAllText(fileCsv, builder.ToString());


            return sql.ToString();
        }

        private string SqlSimple<T>(IEnumerable<T> list)
        {
            StringBuilder builder = new StringBuilder($"INSERT INTO {AttributesOfClass<T>.TableName(_providerName)}");
            builder.Append(" ( ");

            bool isAddPk = AttributesOfClass<T>.PkAttribute(_providerName).Generator != Generator.Native;

            StringBuilder rowHead = new StringBuilder();
            if (isAddPk)
            {
                rowHead.Append(AttributesOfClass<T>.PkAttribute(_providerName).GetColumnName(_providerName))
                    .Append(",");
            }
            foreach (var map in AttributesOfClass<T>.CurrentTableAttributeDall(_providerName))
            {

                if (map.TypeColumn == typeof(Image) || map.TypeColumn == typeof(byte[]))
                {
                    continue;
                }
                rowHead.Append(map.GetColumnName(_providerName)).Append(",");
            }

            builder.Append(rowHead.ToString().Substring(0, rowHead.ToString().LastIndexOf(",", StringComparison.Ordinal))).Append(") VALUES");
            foreach (var ob in list)
            {
                StringBuilder row = new StringBuilder("(");
                if (isAddPk)
                {
                    var o = AttributesOfClass<T>.GetValueE(_providerName, AttributesOfClass<T>.PkAttribute(_providerName).PropertyName, ob);
                    Type type = AttributesOfClass<T>.PropertyInfoList.Value[AttributesOfClass<T>.PkAttribute(_providerName).PropertyName].PropertyType;
                    //if (o is Guid)
                    //
                    //   row.Append(GetValueHex((Guid)o)).Append(",");
                    //else
                    row.Append(GetValue(o, type)).Append(",");


                }
                foreach (var map in AttributesOfClass<T>.CurrentTableAttributeDall(_providerName))
                {
                    var o = AttributesOfClass<T>.GetValueE(_providerName, map.PropertyName, ob);
                    Type type = AttributesOfClass<T>.PropertyInfoList.Value[map.PropertyName].PropertyType;
                    if (type == typeof(Image) || type == typeof(byte[]))
                    {
                        continue;
                    }
                    string str = GetValue(o, type);
                    row.Append(str).Append(",");
                }

                builder.AppendLine(row.ToString().Substring(0, row.ToString().LastIndexOf(',')) + "),");
            }
            var res = builder.ToString().Substring(0, builder.ToString().LastIndexOf(','));
            return res;
        }

        public string GetValueHex(Guid guidData)
        {
            return $" HEX('{guidData}') ";
        }

        public string GetValue(object o, Type type)
        {
            if (o == null)
            {
                return "null";
            }


            if (type == typeof(int)
                || type == typeof(decimal)
                || type == typeof(decimal)
                || type == typeof(long)
                || type == typeof(double)
                || type == typeof(float)
                || type == typeof(uint)
                || type == typeof(sbyte)
                || type == typeof(short)
                || type == typeof(int?)
                || type == typeof(long?)
                || type == typeof(double?)
                || type == typeof(float?)
                || type == typeof(uint?)
                || type == typeof(sbyte?)
                || type == typeof(short?))
            {
                return o.ToString().Replace(",", ".");
            }

            if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                return $"'{(DateTime)o:yyyy-MM-dd HH:mm:ss.fff}'";
            }

            if (type == typeof(Image))
            {
                return "null";
            }

            if (type.IsEnum)
            {
                return Convert.ToInt32(o).ToString();
            }

            if (type == typeof(bool?) || type == typeof(bool))
            {
                if (_providerName == ProviderName.PostgreSql)
                {
                    return o.ToString();
                }
                bool v = Convert.ToBoolean(o);
                return v ? 0.ToString() : 1.ToString();
            }

            var st = UtilsCore.GetSerializeType(type);
            if (st==SerializeType.Self||st==SerializeType.User)
            {
                switch (_providerName)
                {
                    case ProviderName.MsSql:
                        return $"'{UtilsCore.ObjectToJson(o).Replace("'", "''")}'";
                    case ProviderName.MySql:
                        return $"'{UtilsCore.ObjectToJson(o).Replace("\\", "\\\\").Replace("'", "''")}'";
                    case ProviderName.PostgreSql:
                        return $"'{UtilsCore.ObjectToJson(o).Replace("'", "''")}'";
                    case ProviderName.SqLite:
                        return $"'{UtilsCore.ObjectToJson(o).Replace("'", "''")}'";
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            }

            switch (_providerName)
            {
                case ProviderName.MsSql:
                    return $"'{o.ToString().Replace("'", "''")}'";
                case ProviderName.MySql:
                    return $"\"{o.ToString().Replace("\"", "\\\"")}\"";
                case ProviderName.PostgreSql:
                    return $"'{o.ToString().Replace("'", "''")}'";
                case ProviderName.SqLite:
                    return $"'{o.ToString().Replace("'", "''")}'";
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        public static string InsertFile<T>(string fileCsv, string fieldterminator, ProviderName providerName)
        {
            StringBuilder sql = new StringBuilder($"LOAD DATA INFILE '{fileCsv}'");
            sql.AppendLine($"INTO TABLE {AttributesOfClass<T>.TableName(providerName)}");
            sql.AppendLine($"FIELDS TERMINATED BY '{fieldterminator}'");
            sql.AppendLine("ENCLOSED BY '\"'");
            sql.AppendLine("LINES TERMINATED BY '\n'");
            sql.AppendLine("IGNORE 1 ROWS");
            return sql.ToString();
        }
    }
}
