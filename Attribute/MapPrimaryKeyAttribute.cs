using ORM_1_21_.Utils;
using System;
using System.Data;

namespace ORM_1_21_
{
    /// <summary>
    /// Table primary key attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class MapPrimaryKeyAttribute : BaseAttribute
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="columnName">Field name</param>
        /// <param name="generator">Primary key type, native-autoincrement ,assigned-user assigned</param>
        public MapPrimaryKeyAttribute(string columnName, Generator generator) : base(columnName)
        {
            if (string.IsNullOrWhiteSpace(columnName)) throw new ArgumentException("column name zero");
            Generator = generator;
        }

        internal string ColumnNameForRider(ProviderName providerName)
        {
          return UtilsCore.ClearTrim(GetColumnName(providerName));

        }

        internal Type TypeColumn { get; set; }

        internal Generator Generator { get; }

        internal DbType DbType()
        {
            return DbTypeConverter.ConvertFrom(TypeColumn);
        }
    }
}