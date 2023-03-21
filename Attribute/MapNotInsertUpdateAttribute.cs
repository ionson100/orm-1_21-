using System;

namespace ORM_1_21_
{
    /// <summary>
    /// Fields are not included in the query Insert Update
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class MapNotInsertUpdateAttribute : Attribute
    {
    }
}