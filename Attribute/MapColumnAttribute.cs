using ORM_1_21_.Utils;
using System;
using System.Data;

namespace ORM_1_21_
{
    /// <summary>
    /// Field name in the table
    /// </summary>

    public sealed class MapColumnAttribute : BaseAttribute
    {
       
        internal bool IsIndex { get; set; }

        

        /// <summary>
        /// This property is involved in creating the table,
        /// the name of the table field is specified by the user
        /// </summary>
        /// <param name="nameColumnTable">field table name</param>
        public MapColumnAttribute(string nameColumnTable) : base(nameColumnTable)
        {
        }

        /// <summary>
        /// This property is involved in creating the table,
        /// field table name equals property name
        /// </summary>
        public MapColumnAttribute() : base()
        {
        }

       

        internal string ColumnNameForReader(ProviderName providerName)
        {
            
            return UtilsCore.ClearTrim(GetColumnName(providerName));
        }

        internal DbType DbType()
        {
            return DbTypeConverter.ConvertFrom(PropertyType);
        }
    }
    
}