using System;
// ReSharper disable All

namespace ORM_1_21_.geo
{
    /// <summary>
    /// 
    /// </summary>
    public class Geometry
    {
        /// <summary>
        /// 
        /// </summary>
        public Geometry()
        {

        }
        /// <summary>
        /// ctr.
        /// </summary>
        /// <param name="shape"></param>
        public Geometry(IGeoShape shape)
        {
            switch (shape.GeoType)
            {
                case GeoType.None:
                    throw new ArgumentException("GeoType is None");
                case GeoType.Point:
                    type = "Point";
                    break;
                case GeoType.LineString:
                    type = "LineString";
                    break;
                case GeoType.Polygon:
                    type = "Polygon";
                    break;
                case GeoType.MultiPoint:
                    type = "MultiPoint";
                    break;
                case GeoType.MultiLineString:
                    type = "MultiLineString";
                    break;
                case GeoType.MultiPolygon:
                    type = "MultiPolygon";
                    break;
                case GeoType.GeometryCollection:
                    type = "GeometryCollection";
                    break;
                case GeoType.PolygonWithHole:
                    type = "Polygon";
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Geometry type not used:{shape.GeoType}");
            }

            coordinates =((GeoObject) shape).ArrayCoordinates;
        }
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public object coordinates { get; set; }


    }
}