using System;

namespace ORM_1_21_
{
    /// <summary>
    /// An object is created with a constructor call,
    /// without this attribute, an object from the base is created without a constructor and an initializer.
    /// For derived types only.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class MapUsageActivatorAttribute : Attribute
    {
    }
}
