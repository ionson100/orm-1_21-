using System.Data;

namespace ORM_1_21_
{
    /// <summary>
    /// Единица параметра, для запроса с параметрами
    /// </summary>
    public sealed class Parameter
    {
        /// <summary>
        /// Имя параметра
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Значение параметра
        /// </summary>
        public object Value { get; private set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="name">Имя параметра</param>
        /// <param name="value">Имя параметра</param>
        public Parameter(string name, object value)
        {
            Name = name.Replace("?", string.Empty).Replace("@", string.Empty);
            Value = value;
        }
    }

    /// <summary>
    /// Единица параметра, для запроса с параметрами
    /// </summary>
    public sealed class ParameterStoredPr
    {
        //private readonly string _sourceColumn;

        /// <summary>
        /// Название колонки таблицы
        /// </summary>
       // public string SourceColumn { get; private set; }

        /// <summary>
        /// Имя параметра
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Значение параметра
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Перечисление типа параметра
        /// </summary>
        public ParameterDirection Direction { get; set; }

        /// <summary>
        /// Параметры хранимой процедуры
        /// </summary>
        /// <param name="name">Имя параметра</param>
        /// <param name="value">Имя параметра</param>
        /// <param name="direction">ParameterDirection </param>

        public ParameterStoredPr(string name, object value, ParameterDirection direction)
        {

            Name = name.Replace("?", string.Empty).Replace("@", string.Empty);
            Value = value;
            Direction = direction;
        }
    }
}