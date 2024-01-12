
using ORM_1_21_.Utils;
using System;
using System.Text;

namespace ORM_1_21_
{
    class CommandNativeInsert
    {
        private readonly ProviderName _providerName;

        public CommandNativeInsert(ProviderName providerName)
        {
            _providerName = providerName;
        }

        public string GetInsertSql<T>(T t)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($"INSERT INTO {AttributesOfClass<T>.TableName(_providerName)} (");
            MapPrimaryKeyAttribute pk = AttributesOfClass<T>.PkAttribute(_providerName);
            if (pk.Generator == Generator.Assigned)
            {
                builder.Append($" {pk.GetColumnName(_providerName)},");
            }
            foreach (var map in AttributesOfClass<T>.CurrentTableAttributeDal(_providerName))
            {

                builder.Append($" {map.GetColumnName(_providerName)},");
            }

            string s = builder.ToString().Trim(',');
            builder.Length = 0;
            builder.Append(s).Append(") ");
            builder.Append(" VALUES (");
            if (pk.Generator == Generator.Assigned)
            {
                var o = AttributesOfClass<T>.GetValueE(_providerName, AttributesOfClass<T>.PkAttribute(_providerName).PropertyName, t);
                Type type = AttributesOfClass<T>.PropertyInfoList.Value[AttributesOfClass<T>.PkAttribute(_providerName).PropertyName].PropertyType;
                builder.Append(GetValue(o, type)).Append(',');

            }

            foreach (var map in AttributesOfClass<T>.CurrentTableAttributeDal(_providerName))
            {
                var o = AttributesOfClass<T>.GetValueE(_providerName, map.PropertyName, t);
                var type = AttributesOfClass<T>.PropertyInfoList.Value[map.PropertyName].PropertyType;
                if ( type == typeof(byte[])) continue;
                var str = GetValue(o, type);
                builder.Append(str).Append(',');
            }
            return builder.ToString().Trim(',') + ");";


        }

        private string GetValue(object o, Type type)
        {
            if (o == null) return "null";
            type = UtilsCore.GetCoreType(type);

            if (type == typeof(int)
                || type == typeof(decimal)
                || type == typeof(decimal)
                || type == typeof(long)
                || type == typeof(double)
                || type == typeof(float)
                || type == typeof(uint)
                || type == typeof(sbyte)
                || type == typeof(short))
                return o.ToString().Replace(",", ".");
            if (type == typeof(Guid) || type == typeof(string))
            {
                return $"'{o.ToString().Replace("'", "''")}'";
            }
            if (type == typeof(DateTime))
                return $"'{((DateTime)o):yyyy-MM-dd HH:mm:ss.fff}'";


            if (type.IsEnum) return Convert.ToInt32(o).ToString();

            if (type == typeof(bool))
            {
                if (_providerName == ProviderName.PostgreSql) return o.ToString();
                var v = Convert.ToBoolean(o);
                return v ? 0.ToString() : 1.ToString();
            }

            return $"{o.ToString().Replace("'", "''")}";
        }

        public string GetDeleteSql<T>(T t)
        {
            var o = AttributesOfClass<T>.GetValueE(_providerName, AttributesOfClass<T>.PkAttribute(_providerName).PropertyName, t);
            Type type = AttributesOfClass<T>.PropertyInfoList.Value[AttributesOfClass<T>.PkAttribute(_providerName).PropertyName].PropertyType;
            return
                $"Delete from {AttributesOfClass<T>.TableName(_providerName)} where " +
                $"{AttributesOfClass<T>.PkAttribute(_providerName).GetColumnName(_providerName)} = {GetValue(o, type)}";
        }
    }

    
}
