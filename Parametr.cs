using System.Data;

namespace ORM_1_21_
{
   

    /// <summary>
    /// Parameter Container for store procedure
    /// </summary>
    public sealed class ParameterStoredPr
    {
       
        /// <summary>
        /// Parameter name
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Parameter value
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Parameter type
        /// </summary>
        public ParameterDirection Direction { get; set; }

        /// <summary>
        /// Stored Procedure Options
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <param name="value">Parameter value</param>
        /// <param name="direction">ParameterDirection </param>

        public ParameterStoredPr(string name, object value, ParameterDirection direction)
        {

            Name = name.Replace("?", string.Empty).Replace("@", string.Empty);
            Value = value;
            Direction = direction;
        }
    }
}