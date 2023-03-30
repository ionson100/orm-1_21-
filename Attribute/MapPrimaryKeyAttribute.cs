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
        ///  Purpose of the primary key field.
        /// </summary>
        /// <param name="columnName">Field table name</param>
        /// <param name="generator">Primary key type, native-autoincrement ,assigned-user assigned</param>
        public MapPrimaryKeyAttribute(string columnName, Generator generator) : base(columnName)
        {
            if (string.IsNullOrWhiteSpace(columnName)) throw new ArgumentException("column name zero");
            Generator = generator;
        }

        /// <summary>
        /// Purpose of the primary key field.
        /// The name of the primary key field, corresponds to the name of the property.
        /// </summary>
        /// <param name="generator"></param>
        public MapPrimaryKeyAttribute( Generator generator) : base()
        {
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