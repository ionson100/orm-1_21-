using System;
using System.Reflection;
using System.Text;

namespace ORM_1_21_.Linq
{
    internal static class UnaryConverter
    {
        public static Guid ToGuidByte(object o)
        {
            if (o.GetType() == typeof(byte[]))
            {
                var bytes = (byte[])o;
                return bytes.Length == 16 ? new Guid(bytes) : new Guid(Encoding.ASCII.GetString(bytes));
            }

            return Guid.Parse(o.ToString());
        }

        public static DateTime ToDateTime(object o)
        {
            return DateTime.Parse(o.ToString());
        }


        public static MethodInfo GetMethodInfo(ProviderName provider, Type propertyType)
        {
            if (propertyType == typeof(Guid))
                return typeof(UnaryConverter).GetRuntimeMethod("ToGuidByte", new[] { typeof(object) });

            if (propertyType == typeof(int))
                return typeof(Convert).GetRuntimeMethod("ToInt32", new[] { typeof(object) });
            if (propertyType == typeof(decimal))
                return typeof(Convert).GetRuntimeMethod("ToDecimal", new[] { typeof(object) });
            if (propertyType == typeof(float))
                return typeof(Convert).GetRuntimeMethod("ToSingle", new[] { typeof(object) });
            if (propertyType == typeof(uint))
                return typeof(Convert).GetRuntimeMethod("ToUInt32", new[] { typeof(object) });
            if (propertyType == typeof(ulong))
                return typeof(Convert).GetRuntimeMethod("ToUInt64", new[] { typeof(object) });
            if (propertyType == typeof(ushort))
                return typeof(Convert).GetRuntimeMethod("ToUInt16", new[] { typeof(object) });
            if (propertyType == typeof(bool))
                return typeof(Convert).GetRuntimeMethod("ToBoolean", new[] { typeof(object) });
            if (propertyType == typeof(byte))
                return typeof(Convert).GetRuntimeMethod("ToByte", new[] { typeof(object) });
            if (propertyType == typeof(char))
                return typeof(Convert).GetRuntimeMethod("ToChar", new[] { typeof(object) });
            if (propertyType == typeof(DateTime))
                return typeof(Convert).GetRuntimeMethod("ToDateTime", new[] { typeof(object) });
            if (propertyType == typeof(double))
                return typeof(Convert).GetRuntimeMethod("ToDouble", new[] { typeof(object) });
            if (propertyType == typeof(short))
                return typeof(Convert).GetRuntimeMethod("ToInt16", new[] { typeof(object) });
            if (propertyType == typeof(sbyte))
                return typeof(Convert).GetRuntimeMethod("ToSByte", new[] { typeof(object) });
            if (propertyType == typeof(byte[]))
                return typeof(Convert).GetRuntimeMethod("ToSByte", new[] { typeof(object) });

            return null;
        }
    }
}