using System;

namespace ORM_1_21_
{
    /// <summary>
    /// Table field default value (set by user)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class MapDefaultValueAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        internal string Value { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">value string</param>
        public MapDefaultValueAttribute(string value)
        {
            Value = value;
        }
    }
}