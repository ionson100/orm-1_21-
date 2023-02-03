using ORM_1_21_.Attribute;
using System;
using System.Drawing;
using System.Text;

namespace ORM_1_21_
{
    class CommandNativePostgres
    {
        private readonly ProviderName _providerName;

        public CommandNativePostgres(ProviderName providerName)
        {
            _providerName = providerName;
        }

        public  string GetInsertSql<T>(T t)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($"INSERT INTO {AttributesOfClass<T>.TableName} (");
            MapPrimaryKeyAttribute pk = AttributesOfClass<T>.PkAttribute;
            if (pk.Generator == Generator.Assigned)
            {
                builder.Append($" {pk.GetColumnName(_providerName)},");
            }
            foreach (var map in AttributesOfClass<T>.CurrentTableAttributeDall)
            {

                builder.Append($" {map.GetColumnName(_providerName)},");
            }

            string s = builder.ToString().Trim(',');
            builder.Length = 0;
            builder.Append(s).Append(") ");
            builder.Append(" VALUES (");
            if (pk.Generator == Generator.Assigned)
            {
                var o = AttributesOfClass<T>.GetValue.Value[AttributesOfClass<T>.PkAttribute.PropertyName](t);
                Type type = AttributesOfClass<T>.PropertyInfoList.Value[AttributesOfClass<T>.PkAttribute.PropertyName].PropertyType;
                builder.Append(GetValue(o, type)).Append(",");

            }

            foreach (var map in AttributesOfClass<T>.CurrentTableAttributeDall)
            {
                var o = AttributesOfClass<T>.GetValue.Value[map.PropertyName](t);
                var type = AttributesOfClass<T>.PropertyInfoList.Value[map.PropertyName].PropertyType;
                if (type == typeof(Image) || type == typeof(byte[])) continue;
                var str = GetValue(o, type);
                builder.Append(str).Append(",");
            }
            return builder.ToString().Trim(',') + ");";


        }

        private  string GetValue(object o, Type type)
        {
            if (o == null) return "null";


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
                return o.ToString().Replace(",", ".");
            if (type == typeof(Guid) || type == typeof(string))
            {
                return $"'{o.ToString().Replace("'", "''")}'";
            }
            if (type == typeof(DateTime) || type == typeof(DateTime?))
                return $"'{((DateTime)o).ToString("yyyy-MM-dd HH:mm:ss.fff")}'";

            if (type == typeof(Image)) return "null";

            if (type.IsEnum) return Convert.ToInt32(o).ToString();

            if (type == typeof(bool?) || type == typeof(bool))
            {
                if (_providerName == ProviderName.Postgresql) return o.ToString();
                var v = Convert.ToBoolean(o);
                return v ? 0.ToString() : 1.ToString();
            }

            if (Utils.IsJsonType(type)) return $"'{Utils.ObjectToJson(o)}'";
            return $"{o.ToString().Replace("'", "''")}";
        }

        public  string GetDeleteSql<T>(T t)
        {
            var o = AttributesOfClass<T>.GetValue.Value[AttributesOfClass<T>.PkAttribute.PropertyName](t);
            Type type = AttributesOfClass<T>.PropertyInfoList.Value[AttributesOfClass<T>.PkAttribute.PropertyName].PropertyType;
            return
                $"Delete from {AttributesOfClass<T>.TableName} where " +
                $"{AttributesOfClass<T>.PkAttribute.GetColumnName(_providerName)} = {GetValue(o, type)}";
        }
    }
}
