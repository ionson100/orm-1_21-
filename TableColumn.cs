

namespace ORM_1_21_
{
    /// <summary>
    /// Table field list element
    /// </summary>
    [MapTable("notdata")]
    public class TableColumn
    {
        /// <summary>
        /// Name field
        /// </summary>
        [MapColumn("name")]
        public string ColumnName { get; set; }
        /// <summary>
        /// Type field
        /// </summary>
        [MapColumn("type")]

        public string ColumnType { get; set; }
        /// <summary>
        /// Is primary key
        /// </summary>
        [MapColumn("ispk")]
        public bool IsPk { get; set; }
    }
}
