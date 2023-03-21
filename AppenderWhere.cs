namespace ORM_1_21_
{
    /// <summary>
    /// Container to add update condition: Where
    /// </summary>
    public class AppenderWhere
    {

        /// <summary>
        /// Table column Name
        /// </summary>
        public string ColumnName { get; }
        /// <summary>
        /// Column value
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="columnName">Table column Name</param>
        /// <param name="value">Column value</param>
        public AppenderWhere(string columnName, object value)
        {
            ColumnName = columnName;
            Value = value;
        }
    }
}
