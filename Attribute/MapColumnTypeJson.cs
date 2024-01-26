using System;

namespace ORM_1_21_
{
    /// <summary>
    /// Field type. This type serialize in type Json.
    /// Applicable for property type Object or custom types/
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class MapColumnTypeJson : Attribute
    {
        internal TypeReturning Returning { get; }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="returning">Return type. Default as Object</param>
        public MapColumnTypeJson(TypeReturning returning = TypeReturning.AsObject)
        {
            Returning = returning;
        }
       
    }
    /// <summary>
    /// Applicable for property type Object or custom types for an instance, only if the instance type is Object, for custom types, an object is returned.
    /// </summary>
    public enum TypeReturning
    {
        /// <summary>
        /// Return as object.
        /// </summary>
        AsObject,

        /// <summary>
        /// Return as string (json).
        /// </summary>
        AsString

    }
}