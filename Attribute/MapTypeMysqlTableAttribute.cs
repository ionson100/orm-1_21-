using System;

namespace ORM_1_21_
{
    /// <summary>
    ///Information for creating a specific table type, for MySql only. Example: ENGINE=InnoDB DEFAULT CHARSET=UTF8;
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class MapTypeMysqlTableAttribute : System.Attribute
    {
        internal string TableType { get; }

        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="tableType">user data</param>
        public MapTypeMysqlTableAttribute(string tableType)
        {
            TableType = tableType;
        }
    }
}
