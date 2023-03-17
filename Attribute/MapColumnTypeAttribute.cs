using System;

namespace ORM_1_21_
{
    /// <summary>
    /// Field type when creating a table (set by the user)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class MapColumnTypeAttribute : System.Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        internal string TypeString { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public MapColumnTypeAttribute(string typeColumn)
        {
            TypeString = typeColumn;
        }
    }
}