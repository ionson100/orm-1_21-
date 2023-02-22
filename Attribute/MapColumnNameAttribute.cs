using ORM_1_21_.Utils;
using System;
using System.Data;

namespace ORM_1_21_
{
    /// <summary>
    /// Field name in the table
    /// </summary>

    public sealed class MapColumnNameAttribute : BaseAttribute
    {
       
        internal bool IsIndex { get; set; }

        internal string TypeString { get; set; }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="nameColumnTable">field name</param>
        public MapColumnNameAttribute(string nameColumnTable) : base(nameColumnTable)
        {
        }

        internal Type TypeColumn { get; set; }

        internal string ColumnNameForReader(ProviderName providerName)
        {
            
            return UtilsCore.ClearTrim(GetColumnName(providerName));
        }

        internal DbType DbType()
        {
            return DbTypeConverter.ConvertFrom(TypeColumn);
        }
    }

    /// <summary>
    /// Table field default value (set by user)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class MapDefaultValueAttribute : System.Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        internal string Value { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">value string</param>
        public MapDefaultValueAttribute(string value)
        {
            Value = value;
        }
    }

    /// <summary>
    /// Field type when creating a table (set by the user)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class MapColumnTypeAttribute : System.Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        internal string TypeString { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public MapColumnTypeAttribute(string typeColumn)
        {
            TypeString = typeColumn;
        }
    }
}