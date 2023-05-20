using System;

namespace ORM_1_21_
{
    /// <summary>
    /// Object attribute, lets you know if the object is retrieved from the database.
    /// Allows you to insert or modify an object in the database using the save method.
    /// Attention. Lengthens the selection of an object from the database
    /// For derived types only
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class MapUsagePersistentAttribute : Attribute
    {

    }
}