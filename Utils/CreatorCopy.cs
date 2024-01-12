using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ORM_1_21_.Utils
{
    internal class CreatorCopy
    {
        private readonly ProviderName _providerName;
        private readonly bool _isBlob;

        public CreatorCopy(ProviderName provider, bool isBlob = false)
        {
            _providerName = provider;
            _isBlob = isBlob;
        }
        internal void CreateCsvFile<T>(IEnumerable<T> list, string fileCsv, string fieldterminator)
        {
            var builder = new StringBuilder();



            StringBuilder rowHead = new StringBuilder();

            rowHead.Append(AttributesOfClass<T>.PkAttribute(_providerName).GetColumnNameRaw())
                .Append(fieldterminator);

            foreach (var map in AttributesOfClass<T>.CurrentTableAttributeDal(_providerName))
            {
                rowHead.Append(map.GetColumnNameRaw()).Append(fieldterminator);
            }


            builder.Append(rowHead.ToString().Substring(0, rowHead.ToString().LastIndexOf(fieldterminator, StringComparison.Ordinal)) + "\n");

            foreach (var ob in list)
            {
                var row = new StringBuilder();
                {
                    var o = AttributesOfClass<T>.GetValueE(_providerName, AttributesOfClass<T>.PkAttribute(_providerName).PropertyName, ob);
                    var type = AttributesOfClass<T>.PropertyInfoList
                        .Value[AttributesOfClass<T>.PkAttribute(_providerName).PropertyName].PropertyType;
                    row.Append(GetValueCopy(o, type)).Append($"{fieldterminator}");

                }

                foreach (var map in AttributesOfClass<T>.CurrentTableAttributeDal(_providerName))
                {
                    var o = AttributesOfClass<T>.GetValueE(_providerName, map.PropertyName, ob);
                    var type = AttributesOfClass<T>.PropertyInfoList.Value[map.PropertyName].PropertyType;
                    var str = GetValueCopy(o, type);
                    row.Append(str).Append($"{fieldterminator}");

                }

                var s = row.ToString().Substring(0, row.ToString().LastIndexOf(fieldterminator, StringComparison.Ordinal))
                             + "\n";
                builder.Append(s);
            }

            File.WriteAllText(fileCsv, builder.ToString());
        }
        public string GetValueCopy(object o, Type type)
        {

            if (_providerName == ProviderName.MySql)
            {
                if (o == null) return "\\N";
            }
            else
            {
                if (o == null) return "";
            }
            

            type = UtilsCore.GetCoreType(type);

           

            if (type == typeof(Guid) && _isBlob)
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

            if (type == typeof(Guid))
            {
                return $"{o}";
            }

            if (type == typeof(char))
            {
                return $"{o}";
            }

            if (type == typeof(byte))
            {

                string result = ((byte)o).ToString();
                return result;
            }

            if (type == typeof(byte[]))
            {
                string result = Encoding.Default.GetString((byte[])o);
                
                return result;
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
            if (_providerName == ProviderName.MsSql && type == typeof(DateTime))
            {
                string ass= ((DateTime)o).ToString("G").Replace(".","/");
                
                return ass;
            }
            if (type == typeof(DateTime))
                return $"'{(DateTime)o:yyyy-MM-dd HH:mm:ss.fff}'";

            if (type.IsEnum) return Convert.ToInt32(o).ToString();

            if (type == typeof(bool))
            {
                if (_providerName == ProviderName.PostgreSql) return o.ToString();
                var v = Convert.ToBoolean(o);
                return v == false ? "0" : "1";
            }
            if (_providerName == ProviderName.SqLite)
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
                    throw new ArgumentOutOfRangeException($"Database type is not defined:{_providerName}");
            }
        }
    }
}
