using System;

namespace ORM_1_21_
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class MapGeoSridAttribute : Attribute
    {
        /// <summary>
        /// Setting the value SRID from column table (default 4326)
        /// </summary>
        public readonly int _srid;

        /// <summary>
        /// ctor.
        /// </summary>
        /// <param name="srid">SRID value</param>
        public MapGeoSridAttribute(int srid)
        {
            _srid = srid;
        }
    }
}