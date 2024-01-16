using System;

namespace ORM_1_21_.geo
{
    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum GeoType
    {
        /// <summary>
        /// 
        /// </summary>
        None,
        /// <summary>
        /// 
        /// </summary>
        Point,
        /// <summary>
        /// 
        /// </summary>
        LineString,
        /// <summary>
        /// 
        /// </summary>
        Polygon,

        /// <summary>
        /// 
        /// </summary>
        MultiPoint,
        /// <summary>
        /// 
        /// </summary>
        MultiLineString,

        /// <summary>
        /// 
        /// </summary>
        MultiPolygon,

        /// <summary>
        /// 
        /// </summary>
        GeometryCollection,

        /// <summary>
        /// 
        /// </summary>
        CircularString,

        /// <summary>
        /// 
        /// </summary>
        PolygonWithHole,

        /// <summary>
        /// 
        /// </summary>
        Empty



    }
}