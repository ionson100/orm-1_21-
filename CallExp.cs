using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ORM_1_21_
{
    internal static class CallExp<TRes, TElement>
    {
        public static IEnumerable<TRes> GetTrechForGroupBy(IEnumerable<TElement> list, Delegate @delegate, Type type)
        {
            if (type == typeof(Guid)) return (IEnumerable<TRes>)list.GroupBy((Func<TElement, Guid>)@delegate);
            if (type == typeof(uint)) return (IEnumerable<TRes>)list.GroupBy((Func<TElement, uint>)@delegate);
            if (type == typeof(ulong)) return (IEnumerable<TRes>)list.GroupBy((Func<TElement, ulong>)@delegate);
            if (type == typeof(ushort)) return (IEnumerable<TRes>)list.GroupBy((Func<TElement, ushort>)@delegate);
            if (type == typeof(bool)) return (IEnumerable<TRes>)list.GroupBy((Func<TElement, bool>)@delegate);
            if (type == typeof(byte)) return (IEnumerable<TRes>)list.GroupBy((Func<TElement, byte>)@delegate);
            if (type == typeof(char)) return (IEnumerable<TRes>)list.GroupBy((Func<TElement, char>)@delegate);
            if (type == typeof(DateTime)) return (IEnumerable<TRes>)list.GroupBy((Func<TElement, DateTime>)@delegate);
            if (type == typeof(decimal)) return (IEnumerable<TRes>)list.GroupBy((Func<TElement, decimal>)@delegate);
            if (type == typeof(double)) return (IEnumerable<TRes>)list.GroupBy((Func<TElement, double>)@delegate);
            if (type == typeof(short)) return (IEnumerable<TRes>)list.GroupBy((Func<TElement, short>)@delegate);
            if (type == typeof(int)) return (IEnumerable<TRes>)list.GroupBy((Func<TElement, int>)@delegate);
            if (type == typeof(long)) return (IEnumerable<TRes>)list.GroupBy((Func<TElement, long>)@delegate);
            if (type == typeof(sbyte)) return (IEnumerable<TRes>)list.GroupBy((Func<TElement, sbyte>)@delegate);
            if (type == typeof(float)) return (IEnumerable<TRes>)list.GroupBy((Func<TElement, float>)@delegate);
            if (type == typeof(string)) return (IEnumerable<TRes>)list.GroupBy((Func<TElement, string>)@delegate);
            if (type == typeof(uint?)) return (IEnumerable<TRes>)list.GroupBy((Func<TElement, uint?>)@delegate);
            if (type == typeof(ulong?)) return (IEnumerable<TRes>)list.GroupBy((Func<TElement, ulong?>)@delegate);
            if (type == typeof(ushort?)) return (IEnumerable<TRes>)list.GroupBy((Func<TElement, ushort?>)@delegate);
            if (type == typeof(bool?)) return (IEnumerable<TRes>)list.GroupBy((Func<TElement, bool?>)@delegate);
            if (type == typeof(byte?)) return (IEnumerable<TRes>)list.GroupBy((Func<TElement, byte?>)@delegate);
            if (type == typeof(char?)) return (IEnumerable<TRes>)list.GroupBy((Func<TElement, char?>)@delegate);
            if (type == typeof(DateTime?))
                return (IEnumerable<TRes>)list.GroupBy((Func<TElement, DateTime?>)@delegate);
            if (type == typeof(decimal?)) return (IEnumerable<TRes>)list.GroupBy((Func<TElement, decimal?>)@delegate);
            if (type == typeof(double?)) return (IEnumerable<TRes>)list.GroupBy((Func<TElement, double?>)@delegate);
            if (type == typeof(short?)) return (IEnumerable<TRes>)list.GroupBy((Func<TElement, short?>)@delegate);
            if (type == typeof(int?)) return (IEnumerable<TRes>)list.GroupBy((Func<TElement, int?>)@delegate);
            if (type == typeof(long?)) return (IEnumerable<TRes>)list.GroupBy((Func<TElement, long?>)@delegate);
            if (type == typeof(sbyte?)) return (IEnumerable<TRes>)list.GroupBy((Func<TElement, sbyte?>)@delegate);
            if (type == typeof(float?)) return (IEnumerable<TRes>)list.GroupBy((Func<TElement, float?>)@delegate);
            if (type == typeof(byte[])) return (IEnumerable<TRes>)list.GroupBy((Func<TElement, byte?>)@delegate);
            if (type == typeof(object)) return (IEnumerable<TRes>)list.GroupBy((Func<TElement, object>)@delegate);
            throw new Exception(string.Format(CultureInfo.CurrentCulture, "не могу конвертировать тип {0} as {1}",
                type.FullName));
        }
    }
}