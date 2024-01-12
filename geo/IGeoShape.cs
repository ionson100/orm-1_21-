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
        IGeoShape SetSrid(int srid);
       

        /// <summary>
        /// Type Geo Object
        /// </summary>
        GeoType GeoType { get; }


       

       



        /// <summary>
        /// List template points. For only MultiPoint Point,LineString,Polygon.
        /// </summary>
        List<GeoPoint> ListGeoPoints { get; }

        /// <summary>
        /// Return Full GeoJson when property value. For empty objects, return null
        /// </summary>
        object GetGeoJson(object properties = null);

        /// <summary>
        /// List geo object. For only MultiLineString, MultiPolygon, GeometryCollection.
        /// </summary>
        List<IGeoShape> MultiGeoShapes { get; }


        /// <summary>
        /// Returns the OGC Well-Known Text (WKT) representation of the geometry.
        /// </summary>
        /// <returns></returns>
        string StAsText();





        /// <summary>
        /// Returns the type of the geometry as a string.
        /// EG postgres: 'ST_LineString', 'ST_Polygon','ST_MultiPolygon'
        /// EG mysql ms sql: 'LineString', 'Polygon','MultiPolygon'
        /// </summary>
        /// <param name="session">Session Orm</param>
        /// <returns>String</returns>
        string StGeometryType(ISession session);


        /// <summary>
        /// Returns the area of a polygonal geometry. For geometry types a 2D Cartesian (planar) area is computed, with units specified by the SRID
        /// </summary>
        /// <param name="session">Session Orm</param>
        double? StArea(ISession session);

        /// <summary>
        /// Returns TRUE if current geometry  is within geometry B. current geometry is within B if and only if all points of current geometry lie inside (i.e. in the interior or boundary of) B
        /// (or equivalently, no points of current geometry lie in the exterior of B), and the interiors of current geometry and B have at least one point in common.
        /// </summary>
        /// <param name="shape">Geometry B</param>
        /// <param name="session">Session Orm</param>
        bool? StWithin(IGeoShape  shape,ISession session);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        byte[] StAsBinary();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IGeoShape StBoundary();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        IGeoShape StBuffer(float distance);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IGeoShape StCentroid();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        /// <returns></returns>
        bool? StContains(IGeoShape shape);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        /// <returns></returns>
        bool? StCrosses(IGeoShape shape);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        /// <returns></returns>
        IGeoShape StDifference(IGeoShape shape);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int? StDimension();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        /// <returns></returns>
        bool? StDisjoint(IGeoShape shape);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        /// <returns></returns>
        float? StDistance(IGeoShape shape);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IGeoShape StEndPoint();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IGeoShape StEnvelope();

       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        /// <returns></returns>
        bool? StEquals(IGeoShape shape);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        /// <returns></returns>
        bool? StIntersects(IGeoShape shape);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        /// <returns></returns>
        bool? StOverlaps(IGeoShape shape);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        /// <returns></returns>
        bool? StOverlapsContra(IGeoShape shape);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int? StSrid();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IGeoShape StStartPoint();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        /// <returns></returns>
        bool? StWithinContra(IGeoShape shape);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        /// <returns></returns>
        IGeoShape StSymDifference(IGeoShape shape);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        /// <returns></returns>
        bool? StTouches(IGeoShape shape);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int? StNumGeometries();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int? StNumInteriorRing();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool? StIsSimple();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool? StIsValid();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        double? StLength();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool? StIsClosed();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int? StNumPoints();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        /// <returns></returns>
        IGeoShape StUnion(IGeoShape shape);







    }
}
