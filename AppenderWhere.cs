using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORM_1_21_
{
    /// <summary>
    /// container to add update condition: Where
    /// </summary>
    public class AppenderWhere
    {

        /// <summary>
        /// Table column
        /// </summary>
        public string ColumnName { get; }
        /// <summary>
        /// Column value
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName">Table column</param>
        /// <param name="value">Column value</param>
        public AppenderWhere(string columnName, object value)
        {
            ColumnName = columnName;
            Value = value;
        }
    }
}
