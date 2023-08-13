using System;
using System.Collections.Generic;
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
            if (fileCsv != null) return SqlFile(list, fileCsv, fieldTerminator);
            return SqlSimple(list);
        }

        public string GetSql<T>(IEnumerable<T> list, bool isBlob)
        {
            return SqlSimple(list, isBlob);
        }

        private string SqlFile<T>(IEnumerable<T> list, string fileCsv, string fieldTerminator)
        {
            var sql = new StringBuilder($"LOAD DATA INFILE '{fileCsv}'");
            sql.AppendLine($" INTO TABLE {AttributesOfClass<T>.TableName(_providerName)}");
            sql.AppendLine($" FIELDS TERMINATED BY '{fieldTerminator}'");
            //sql.AppendLine("ENCLOSED BY '\"'");
            //sql.AppendLine("LINES TERMINATED BY '\n'");
            sql.AppendLine("IGNORE 1 ROWS");
            var builder = new StringBuilder();
            var isAddPk = AttributesOfClass<T>.PkAttribute(_providerName).Generator == Generator.Assigned;

            var rowHead = new StringBuilder();
            if (isAddPk)
                rowHead.Append(AttributesOfClass<T>.PkAttribute(_providerName).GetColumnName(_providerName))
                    .Append(";");
            foreach (var map in AttributesOfClass<T>.CurrentTableAttributeDal(_providerName))
                rowHead.Append(map.GetColumnName(_providerName)).Append(";");

            builder.Append(rowHead.ToString().Substring(0, rowHead.ToString().LastIndexOf(';')))
                .Append(Environment.NewLine);

            foreach (var ob in list)
            {
                var row = new StringBuilder();
                if (isAddPk)
                {
                    var o = AttributesOfClass<T>.GetValueE(_providerName,
                        AttributesOfClass<T>.PkAttribute(_providerName).PropertyName, ob);
                    var type = AttributesOfClass<T>.PropertyInfoList
                        .Value[AttributesOfClass<T>.PkAttribute(_providerName).PropertyName].PropertyType;
                    row.Append(GetValue(o, type)).Append(";");
                }

                foreach (var map in AttributesOfClass<T>.CurrentTableAttributeDal(_providerName))
                {
                    var o = AttributesOfClass<T>.GetValueE(_providerName, map.PropertyName, ob);
                    var type = AttributesOfClass<T>.PropertyInfoList.Value[map.PropertyName].PropertyType;
                    var str = GetValue(o, type);
                    row.Append(str).Append(";");
                }

                var s = row.ToString().Substring(0, row.ToString().LastIndexOf(";", StringComparison.Ordinal)) + "\n";
                builder.Append(s);
            }

            File.WriteAllText(fileCsv, builder.ToString());
            return sql.ToString();
        }

        private string SqlSimple<T>(IEnumerable<T> list, bool isBlob = false)
        {
            var builder = new StringBuilder($"INSERT INTO {AttributesOfClass<T>.TableName(_providerName)}");
            builder.Append(" ( ");
            var isAddPk = AttributesOfClass<T>.PkAttribute(_providerName).Generator == Generator.Assigned;
            var rowHead = new StringBuilder();
            if (isAddPk)
                rowHead.Append(AttributesOfClass<T>.PkAttribute(_providerName).GetColumnName(_providerName))
                    .Append(",");
            foreach (var map in AttributesOfClass<T>.CurrentTableAttributeDal(_providerName))
                rowHead.Append(map.GetColumnName(_providerName)).Append(",");

            builder.Append(rowHead.ToString()
                .Substring(0, rowHead.ToString().LastIndexOf(",", StringComparison.Ordinal))).Append(") VALUES");
            foreach (var ob in list)
            {
                var row = new StringBuilder("(");
                if (isAddPk)
                {
                    var o = AttributesOfClass<T>.GetValueE(_providerName,
                        AttributesOfClass<T>.PkAttribute(_providerName).PropertyName, ob);
                    var type = AttributesOfClass<T>.PropertyInfoList
                        .Value[AttributesOfClass<T>.PkAttribute(_providerName).PropertyName].PropertyType;
                    row.Append(GetValue(o, type, isBlob)).Append(",");
                }

                foreach (var map in AttributesOfClass<T>.CurrentTableAttributeDal(_providerName))
                {
                    var o = AttributesOfClass<T>.GetValueE(_providerName, map.PropertyName, ob);
                    var type = AttributesOfClass<T>.PropertyInfoList.Value[map.PropertyName].PropertyType;
                    var str = GetValue(o, type, isBlob);
                    row.Append(str).Append(",");
                }

                builder.AppendLine(row.ToString().Substring(0, row.ToString().LastIndexOf(',')) + "),");
            }

            var res = builder.ToString().Substring(0, builder.ToString().LastIndexOf(','));
            return res;
        }

        public string GetValue(object o, Type type, bool isBlob = false)
        {
            if (o == null) return "null";

            type = UtilsCore.GetCoreType(type);

            if (type == typeof(byte[]))
                switch (_providerName)
                {
                    case ProviderName.MsSql:
                    case ProviderName.MySql:
                        return "0x" + BitConverter.ToString((byte[])o).Replace("-", "");
                    case ProviderName.PostgreSql:
                        return "decode('" + BitConverter.ToString((byte[])o).Replace("-", "") + "', 'hex')";
                    case ProviderName.SqLite:
                        return "x'" + BitConverter.ToString((byte[])o).Replace("-", "") + "'";
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            if (type == typeof(Guid) && isBlob)
            {
                Guid guid = (Guid)o;
                if (_providerName == ProviderName.SqLite)
                {
                    return "x'" + BitConverter.ToString(guid.ToByteArray()).Replace("-", "") + "'";
                }

                if (_providerName == ProviderName.MySql)
                {
                    return "0x" + BitConverter.ToString(guid.ToByteArray()).Replace("-", "");
                }
            }


            if (type == typeof(int)
                || type == typeof(uint)
                || type == typeof(decimal)
                || type == typeof(long)
                || type == typeof(ulong)
                || type == typeof(double)
                || type == typeof(float)
                || type == typeof(ushort)
                || type == typeof(sbyte)
                || type == typeof(short))
                return o.ToString().Replace(",", ".");

            if (type == typeof(DateTime))
                return $"'{(DateTime)o:yyyy-MM-dd HH:mm:ss.fff}'";

            if (type.IsEnum) return Convert.ToInt32(o).ToString();

            if (type == typeof(bool))
            {
                if (_providerName == ProviderName.PostgreSql) return o.ToString();
                var v = Convert.ToBoolean(o);
                return v == false ? "0" : "1";
            }
            if(_providerName==ProviderName.SqLite)
                if (type == typeof(char))
                {
                    return $"{Convert.ToByte(o)}";
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
            var sql = new StringBuilder($".import '{fileCsv}' {AttributesOfClass<T>.TableName(providerName)}");
           //sql.AppendLine($"INTO TABLE ");
           //sql.AppendLine($"FIELDS TERMINATED BY '{fieldterminator}'");
           //sql.AppendLine("ENCLOSED BY '\"'");
           //sql.AppendLine("LINES TERMINATED BY '\n'");
           //sql.AppendLine("IGNORE 1 ROWS");
            return sql.ToString();
        }
    }
}