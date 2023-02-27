

namespace ORM_1_21_
{
    /// <summary>
    /// Table field list element
    /// </summary>
    [MapTableName("notdata")]
    public class TableColumn
    {
        /// <summary>
        /// Name field
        /// </summary>
        [MapColumnName("name")]
        public string ColumnName { get; set; }
        /// <summary>
        /// Type field
        /// </summary>
        [MapColumnName("type")]
        
        public string ColumnType { get; set; }
        /// <summary>
        /// Is primary key
        /// </summary>
        [MapColumnName("ispk")]
        public bool IsPk { get; set; }
    }
}
