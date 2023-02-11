using System;

namespace ORM_1_21_
{


    /// <summary>
    /// Отмечаются индексируемые поля
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class MapIndexAttribute : System.Attribute
    {
    }
}
