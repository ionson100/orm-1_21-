using System.Data;

namespace ORM_1_21_
{
    /// <summary>
    /// Parameter Container
    /// </summary>
    public sealed class Parameter
    {
        /// <summary>
        /// User parameter
        /// </summary>
        public IDataParameter UserParameter { get; }
        /// <summary>
        /// Parameter database type
        /// </summary>
        public DbType? DbType { get;  }
        /// <summary>
        /// Parameter is can  Nullable
        /// </summary>
     
        public string Name { get; private set; }
        /// <summary>
        /// Parameter value
        /// </summary>
        public object Value { get; private set; }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <param name="value">Parameter value</param>
        public Parameter(string name, object value)
        {
            Name = name;//.Replace("?", string.Empty).Replace("@", string.Empty);
            Value = value;
        }
        
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <param name="value">Parameter value</param>
        /// <param name="type">DataBase type parameter</param>
        public Parameter(string name, object value, DbType type)
        {
            Name = name;//.Replace("?", string.Empty).Replace("@", string.Empty);
            Value = value;
            DbType = type;
        }

        /// <summary>
        /// Constructor User parameter 
        /// </summary>
        /// <param name="userParameter"></param>
        public Parameter(IDataParameter userParameter)
        {
            UserParameter = userParameter;
        }
    }

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