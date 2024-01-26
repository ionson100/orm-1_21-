using System;

namespace ORM_1_21_
{
    /// <summary>
    /// Field type when creating a table (set by the user)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class MapColumnTypeAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        internal string TypeString { get; }

        /// <summary>
        /// 
        /// </summary>
        public MapColumnTypeAttribute(string typeColumn)
        {
            TypeString = typeColumn;
        }
    }
}