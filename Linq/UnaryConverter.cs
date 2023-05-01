using ORM_1_21_.Utils;
using System;
using System.Collections.Generic;
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

        
        public static float? ToFloatNullable(object o)
        {
            if (o != null)
            {
                float?  res =  Convert.ToSingle(o) ; ;

                return res;
            }
            return null;
        }
        public static decimal? ToDecimalNullable(object o)
        {
            if (o != null)
            {
                decimal? res = Convert.ToDecimal(o); ;

                return res;
            }
            return null;
        }
        public static int? ToInt32Nullable(object o)
        {
            if (o != null)
            {
                int? res = Convert.ToInt32(o); 
                return res;
            }
            return null;
        }
        public static short? ToInt16Nullable(object o)
        {
            if (o != null)
            {
                short? res = Convert.ToInt16(o); ;

                return res;
            }
            return null;
        }
        public static long? ToInt64Nullable(object o)
        {
            if (o != null)
            {
                long? res = Convert.ToInt64(o); ;

                return res;
            }
            return null;
        }


        public static uint? ToUInt32Nullable(object o)
        {
            if (o != null)
            {
                uint? res = Convert.ToUInt32(o); ;

                return res;
            }
            return null;
        }
        public static ushort? ToUInt16Nullable(object o)
        {
            if (o != null)
            {
                ushort? res = Convert.ToUInt16(o); ;

                return res;
            }
            return null;
        }
        public static ulong? ToUInt64Nullable(object o)
        {
            if (o != null)
            {
                ushort? res = Convert.ToUInt16(o); ;

                return res;
            }
            return null;
        }

        public static bool? ToBoolNullable(object o)
        {
            if (o != null)
            {
                bool? res = Convert.ToBoolean(o); ;

                return res;
            }
            return null;
        }

        public static Guid? ToGuidNullable(object o)
        {
            if (o != null)
            {
                Guid? res = ToGuidByte(o);

                return res;
            }
            return null;
        }

        public static byte? ToByteNullable(object o)
        {
            if (o != null)
            {
                byte? res = Convert.ToByte(o);

                return res;
            }
            return null;
        }

        public static char? ToCharNullable(object o)
        {
            if (o != null)
            {
                char? res = Convert.ToChar(o);

                return res;
            }
            return null;
        }

        public static DateTime? ToDateTimeNullable(object o)
        {
            if (o != null)
            {
                DateTime? res = Convert.ToDateTime(o);

                return res;
            }
            return null;
        }
        public static double? ToDoubleNullable(object o)
        {
            if (o != null)
            {
                double? res = Convert.ToDouble(o);

                return res;
            }
            return null;
        }

        public static sbyte? ToSByteNullable(object o)
        {
            if (o != null)
            {
                sbyte? res = Convert.ToSByte(o);

                return res;
            }
            return null;
        }

        public static byte[] ToBytesNullable(object o)
        {
            if (o != null)
            {
                return (byte[])o; 
            }
            return null;
        }



        private static readonly Dictionary<Type, MethodInfo> MethodInfoDictionary =
            new Dictionary<Type, MethodInfo>
            {
               
                {typeof(Guid),typeof(UnaryConverter).GetRuntimeMethod("ToGuidByte", new[] { typeof(object) })},
                {typeof(int),typeof(Convert).GetRuntimeMethod("ToInt32", new[] { typeof(object) })},
                {typeof(decimal),typeof(Convert).GetRuntimeMethod("ToDecimal", new[] { typeof(object) })},
                {typeof(float),typeof(Convert).GetRuntimeMethod("ToSingle", new[] { typeof(object) })},
                {typeof(uint),typeof(Convert).GetRuntimeMethod("ToUInt32", new[] { typeof(object) })},
                {typeof(ulong),typeof(Convert).GetRuntimeMethod("ToUInt64", new[] { typeof(object) })},
                {typeof(ushort),typeof(Convert).GetRuntimeMethod("ToUInt16", new[] { typeof(object) })},
                {typeof(bool),typeof(Convert).GetRuntimeMethod("ToBoolean", new[] { typeof(object) })},
                {typeof(byte),typeof(Convert).GetRuntimeMethod("ToByte", new[] { typeof(object) })},
                {typeof(char),typeof(Convert).GetRuntimeMethod("ToChar", new[] { typeof(object) })},
                {typeof(DateTime),typeof(Convert).GetRuntimeMethod("ToDateTime", new[] { typeof(object) })},
                {typeof(double),typeof(Convert).GetRuntimeMethod("ToDouble", new[] { typeof(object) })},
                {typeof(short),typeof(Convert).GetRuntimeMethod("ToInt16", new[] { typeof(object) })},
                {typeof(sbyte),typeof(Convert).GetRuntimeMethod("ToSByte", new[] { typeof(object) })},
                {typeof(byte[]),typeof(UnaryConverter).GetRuntimeMethod("ToObject", new[] { typeof(object) })},

                {typeof(Guid?),typeof(UnaryConverter).GetRuntimeMethod("ToGuidNullable", new[] { typeof(object) })},
                {typeof(int?),typeof(UnaryConverter).GetRuntimeMethod("ToInt32Nullable", new[] { typeof(object) })},
                {typeof(decimal?),typeof(UnaryConverter).GetRuntimeMethod("ToDecimalNullable", new[] { typeof(object) })},
                {typeof(float?),typeof(UnaryConverter).GetRuntimeMethod("ToFloatNullable", new[] { typeof(object) })},
                {typeof(uint?),typeof(UnaryConverter).GetRuntimeMethod("ToUInt32Nullable", new[] { typeof(object) })},
                {typeof(ulong?),typeof(UnaryConverter).GetRuntimeMethod("ToInt64Nullable", new[] { typeof(object) })},
                {typeof(ushort ?),typeof(UnaryConverter).GetRuntimeMethod("ToInt16Nullable", new[] { typeof(object) })},
                {typeof(bool ?),typeof(UnaryConverter).GetRuntimeMethod("ToBoolNullable", new[] { typeof(object) })},
                {typeof(byte ?),typeof(UnaryConverter).GetRuntimeMethod("ToByteNullable", new[] { typeof(object) })},
                {typeof(char ?),typeof(UnaryConverter).GetRuntimeMethod("ToCharNullable", new[] { typeof(object) })},
                {typeof(DateTime?),typeof(UnaryConverter).GetRuntimeMethod("ToDateTimeNullable", new[] { typeof(object) })},
                {typeof(double ?),typeof(UnaryConverter).GetRuntimeMethod("ToDoubleNullable", new[] { typeof(object) })},
                {typeof(short ?),typeof(UnaryConverter).GetRuntimeMethod("ToInt16Nullable", new[] { typeof(object) })},
                {typeof(sbyte ?),typeof(UnaryConverter).GetRuntimeMethod("ToSByteNullable", new[] { typeof(object) })},
                



            };
        public static MethodInfo GetMethodInfo(ProviderName provider, Type type)
        {
           //if (type == typeof(float?))
           //    return typeof(UnaryConverter).GetRuntimeMethod("ToFloatNullable", new[] { typeof(object) });
           //
           //
           //type = UtilsCore.GetCoreType(type);
            if (MethodInfoDictionary.ContainsKey(type))
                return MethodInfoDictionary[type];
            if (type.BaseType == typeof(Enum))
            {
                throw new Exception($"{type}: Not to support");
            }

            return null;
        }
    }
}