namespace ORM_1_21_
{
    /// <summary>
    /// 
    /// </summary>
    public class SqlParam
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// 
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public SqlParam(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }
}