using System.Collections.Generic;

namespace ORM_1_21_.geo
{
    /// <summary>
    /// Geo Object
    /// </summary>
    public interface IGeoShape
    {

        /// <summary>
        /// Quickly set value Spatial Reference System Identifier
        /// </summary>
        /// <param name="srid"></param>
        /// <returns></returns>
        IGeoShape SetSrid(int srid);
        /// <summary>
        /// Spatial Reference System Identifier
        /// </summary>
        int Srid { get; set; }

        /// <summary>
        /// Type Geo Object
        /// </summary>
        GeoType GeoType { get; }


        /// <summary>
        /// String the OGC Well-Known Text (WKT) representation of the geometry
        /// </summary>
        string GeoText { get; set; }



        /// <summary>
        /// List template points. For only MultiPoint Point,LineString,Polygon.
        /// </summary>
        List<GeoPoint> ListGeoPoints { get; }

        /// <summary>
        /// Return Full GeoJson when property value
        /// </summary>
        object GetGeoJson(object properties = null);

        /// <summary>
        /// List geo object. For only MultiLineString, MultiPolygon, GeometryCollection.
        /// </summary>
        List<IGeoShape> MultiGeoShapes { get; }



        /// <summary>
        /// Array Coordinates for builder GeoJson
        /// </summary>
        object ArrayCoordinates { get;  }


    }
}
