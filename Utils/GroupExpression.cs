using System;
using System.Linq.Expressions;

namespace ORM_1_21_.Utils
{
    class GroupExpression<T>
    {

        public static Delegate Delegate(LambdaExpression lexp)
        {
            return Action(lexp, lexp.ReturnType);
        }
        private static Delegate Action(LambdaExpression lexp, Type type)
        {

            if (type == typeof(Guid)) { return (Func<T, Guid>)lexp.Compile(); }
            if (type == typeof(DateTime)) { return (Func<T, DateTime>)lexp.Compile(); }
            if (type == typeof(uint)) { return (Func<T, uint>)lexp.Compile(); }
            if (type == typeof(ulong)) { return (Func<T, ulong>)lexp.Compile(); }
            if (type == typeof(ushort)) { return (Func<T, ushort>)lexp.Compile(); }
            if (type == typeof(bool)) { return (Func<T, bool>)lexp.Compile(); }
            if (type == typeof(byte)) { return (Func<T, byte>)lexp.Compile(); }
            if (type == typeof(char)) { return (Func<T, char>)lexp.Compile(); }
            if (type == typeof(decimal)) { return (Func<T, decimal>)lexp.Compile(); }
            if (type == typeof(double)) { return (Func<T, double>)lexp.Compile(); }
            if (type == typeof(short)) { return (Func<T, short>)lexp.Compile(); }
            if (type == typeof(int)) { return (Func<T, int>)lexp.Compile(); }
            if (type == typeof(long)) { return (Func<T, long>)lexp.Compile(); }
            if (type == typeof(sbyte)) { return (Func<T, sbyte>)lexp.Compile(); }
            if (type == typeof(float)) { return (Func<T, float>)lexp.Compile(); }

            if (type == typeof(string))
            {
                return (Func<T, string>)lexp.Compile();
            }
            if (type == typeof(object)) { return (Func<T, object>)lexp.Compile(); }


            if (type == typeof(Guid?)) { return (Func<T, Guid?>)lexp.Compile(); }
            if (type == typeof(DateTime?)) { return (Func<T, DateTime?>)lexp.Compile(); }
            if (type == typeof(uint?)) { return (Func<T, uint?>)lexp.Compile(); }
            if (type == typeof(ulong?)) { return (Func<T, ulong?>)lexp.Compile(); }
            if (type == typeof(ushort?)) { return (Func<T, ushort?>)lexp.Compile(); }
            if (type == typeof(bool?)) { return (Func<T, bool?>)lexp.Compile(); }
            if (type == typeof(byte?)) { return (Func<T, byte?>)lexp.Compile(); }
            if (type == typeof(char?)) { return (Func<T, char?>)lexp.Compile(); }
            if (type == typeof(decimal?)) { return (Func<T, decimal?>)lexp.Compile(); }
            if (type == typeof(double?)) { return (Func<T, double?>)lexp.Compile(); }
            if (type == typeof(short?)) { return (Func<T, short?>)lexp.Compile(); }
            if (type == typeof(int?)) { return (Func<T, int?>)lexp.Compile(); }
            if (type == typeof(long?)) { return (Func<T, long?>)lexp.Compile(); }
            if (type == typeof(sbyte?)) { return (Func<T, sbyte?>)lexp.Compile(); }
            if (type == typeof(float?)) { return (Func<T, float?>)lexp.Compile(); }



            return (Func<T, object>)lexp.Compile();


        }
    }
}