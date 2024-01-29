using ORM_1_21_.Utils;
using System;
using System.Collections.Generic;
using System.Data;


namespace ORM_1_21_
{
    internal static class DbTypeConverter
    {
        private static readonly Dictionary<Type, DbType> DbTypes = new Dictionary<Type, DbType>
        {
            { typeof(Int32),DbType.Int32 },
            {typeof(String),DbType.String},
            {typeof(Char[]),DbType.AnsiStringFixedLength},
            {typeof(DateTimeOffset),DbType.DateTimeOffset},
            {typeof(Byte[]),DbType.Binary},
            {typeof(Boolean),DbType.Boolean},
            {typeof(Byte),DbType.Byte},
            {typeof(Char),DbType.String},
            {typeof(DateTime),DbType.DateTime},
            {typeof(decimal),DbType.Decimal},
            {typeof(double),DbType.Double},
            {typeof(Int16),DbType.Int16},
            {typeof(Int64),DbType.Int64},
            {typeof(SByte),DbType.SByte},
            {typeof(Single),DbType.Single},
            {typeof(UInt16),DbType.UInt16},
            {typeof(UInt32),DbType.UInt32},
            {typeof(UInt64),DbType.UInt64},
            {typeof(Guid),DbType.Guid},
            {typeof(byte[]),DbType.Binary},
            {typeof(DateTimeOffset),DbType.DateTimeOffset},

        };
        public static DbType ConvertFrom(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            type = UtilsCore.GetCoreType(type);
            if (DbTypes.ContainsKey(type)) return DbTypes[type];
            if (type.BaseType == typeof(Enum)) return DbType.Int32;
            throw new NotSupportedException(string.Format("Unable to convert {0} to a DbType enum value.", type.FullName));
        }
    }
}
