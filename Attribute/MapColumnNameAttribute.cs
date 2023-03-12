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
    
}