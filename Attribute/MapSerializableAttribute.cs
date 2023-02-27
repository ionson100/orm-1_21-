using System;

namespace ORM_1_21_
{
    /// <summary>
    /// The object is stored in the database in Json format, as a string
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class MapSerializableAttribute : System.Attribute
    {
    }
}