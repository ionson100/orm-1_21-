using ORM_1_21_.Attribute;

namespace ORM_1_21_
{
    /// <summary>
    /// Элемент списка полей таблицы
    /// </summary>
    [MapTableName("notdata")]
    public class TableColumn
    {
        /// <summary>
        /// Название  поля
        /// </summary>
        [MapColumnName("name")]
        public string ColumnName { get; set; }
        /// <summary>
        /// Тип поля
        /// </summary>
        [MapColumnName("type")]
        
        public string ColumnType { get; set; }
        /// <summary>
        /// Принадлежность к первичному ключу
        /// </summary>
        [MapColumnName("ispk")]
        public bool IsPk { get; set; }
    }
}
