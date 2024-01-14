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
        /// <param name="session">Open session Orm</param>
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
        /// <param name="session">Open session Orm</param>
        bool? StWithin(IGeoShape  shape,ISession session);

        /// <summary>
        /// Returns the OGC/ISO Well-Known Binary (WKB) representation of the geometry.
        /// </summary>
        /// <param name="session">Open session Orm</param>
        byte[] StAsBinary(ISession session);











        /// <summary>
        /// Returns the closure of the combinatorial boundary of this Geometry.
        /// The combinatorial boundary is defined as described in section 3.12.3.2 of the OGC SPEC.
        /// Because the result of this function is a closure, and hence topologically closed, the resulting boundary
        /// can be represented using representational geometry primitives as discussed in the OGC SPEC, section 3.12.2.
        /// </summary>
        /// <param name="session">Open session Orm</param>
        IGeoShape StBoundary(ISession session);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        IGeoShape StBuffer(float distance, ISession session);

        /// <summary>
        /// Computes a point which is the geometric center of mass of a geometry.
        /// For [MULTI]POINTs, the centroid is the arithmetic mean of the input coordinates.
        /// For [MULTI]LINESTRINGs, the centroid is computed using the weighted length of each line segment.
        /// For [MULTI]POLYGONs, the centroid is computed in terms of area.
        /// If an empty geometry is supplied, an empty GEOMETRYCOLLECTION is returned. If NULL is supplied, NULL is returned.
        /// If CIRCULARSTRING or COMPOUNDCURVE are supplied, they are converted to linestring with CurveToLine first, then same than for LINESTRING
        /// </summary>
        /// <param name="session">Open session orm</param>
        IGeoShape StCentroid(ISession session);

        /// <summary>
        /// Returns the last point of a LINESTRING or CIRCULARLINESTRING geometry as a POINT. Returns NULL if the input is not a LINESTRING or CIRCULARLINESTRING.
        /// </summary>
        /// <param name="session">Open session orm</param>
        IGeoShape StEndPoint(ISession session);

        /// <summary>
        /// Returns the minimum axis-aligned bounding rectangle of the instance.
        /// </summary>
        /// <param name="session">Open session orm</param>
        IGeoShape StEnvelope(ISession session);

        /// <summary>
        /// Returns the first point of a LINESTRING or CIRCULARLINESTRING geometry as a POINT. Returns NULL if the input is not a LINESTRING or CIRCULARLINESTRING.
        /// </summary>
        /// <param name="session">Open session orm</param>
        IGeoShape StStartPoint(ISession session);

        /// <summary>
        /// Returns an object that represents all points that are either in one geometry instance or another geometry instance, but not those points that lie in both instances.
        /// </summary>
        /// <param name="shape">Other geo object</param>
        /// <param name="session">Open session orm</param>
        IGeoShape StSymDifference(IGeoShape shape, ISession session);

        /// <summary>
        /// Returns an object that represents the union of a geometry instance with another geometry instance.
        /// </summary>
        /// <param name="shape">Other geo object</param>
        /// <param name="session">Open session orm</param>
        IGeoShape StUnion(IGeoShape shape, ISession session);

        /// <summary>
        /// Returns TRUE if current geometry  contains geometry B. 
        /// </summary>
        /// <param name="shape">Geometry B</param>
        /// <param name="session">Open session orm</param>
        bool? StContains(IGeoShape shape, ISession session);

        /// <summary>
        /// Compares two geometry objects and returns true if their intersection "spatially crosses"; that is, the geometries have some, but not all interior points in common.
        /// The intersection of the interiors of the geometries must be non-empty and must have dimension less than the maximum dimension of the two input geometries,
        /// and the intersection of the two geometries must not equal either geometry. Otherwise, it returns false.
        /// The crosses relation is symmetric and irreflexive.
        /// </summary>
        /// <param name="shape">Other geo object</param>
        /// <param name="session">Open session Orm</param>
        bool? StCrosses(IGeoShape shape, ISession session);

        /// <summary>
        /// Returns a geometry representing the part of geometry A that does not intersect geometry B.
        /// This is equivalent to A - ST_Intersection(A,B).
        /// If A is completely contained in B then an empty atomic geometry of appropriate type is returned.
        /// Where geometry A - current geo object, geometry B other geo object.
        /// </summary>
        /// <param name="shape">Other geo object</param>
        /// <param name="session">Open session Orm</param>
        /// <returns></returns>
        IGeoShape StDifference(IGeoShape shape,ISession  session);

        /// <summary>
        /// Return the topological dimension of this Geometry object, which must be less than or equal to the coordinate dimension
        /// </summary>
        /// <param name="session">Open session orm</param>
        int? StDimension(ISession session);

        /// <summary>
        /// Returns true if two geometries are disjoint. Geometries are disjoint if they have no point in common.
        /// </summary>
        /// <param name="shape">Other geo object</param>
        /// <param name="session">Open session orm</param>
        bool? StDisjoint(IGeoShape shape, ISession session);

        /// <summary>
        /// For geometry types returns the minimum 2D Cartesian (planar) distance between two geometries, in projected units (spatial ref units).
        /// </summary>
        /// <param name="shape">Other geo object</param>
        /// <param name="session">Open session Orm</param>
        /// <returns></returns>
        double? StDistance(IGeoShape shape, ISession session);

      


        /// <summary>
        /// Returns true if the given geometries are "topologically equal".
        /// Use this for a 'better' answer than '='.
        /// Topological equality means that the geometries have the same dimension, and their point-sets occupy the same space.
        /// This means that the order of vertices may be different in topologically equal geometries.
        /// To verify the order of points is consistent use ST_OrderingEquals (it must be noted ST_OrderingEquals is a little more stringent than simply verifying order of points are the same).
        /// </summary>
        /// <param name="shape">Other geo object</param>
        /// <param name="session">Open session orm</param>
        bool? StEquals(IGeoShape shape, ISession session);


        /// <summary>
        /// Returns true if two geometries intersect. Geometries intersect if they have any point in common.
        ///For geography, a distance tolerance of 0.00001 meters is used (so points that are very close are considered to intersect).
        /// </summary>
        /// <param name="shape">Other geo object</param>
        /// <param name="session">Open session Orm</param>
        bool? StIntersects(IGeoShape shape, ISession session);

        /// <summary>
        /// Returns TRUE if geometry A and B "spatially overlap".
        /// Two geometries overlap if they have the same dimension, their interiors intersect in that dimension.
        /// and each has at least one point inside the other (or equivalently, neither one covers the other).
        /// The overlaps relation is symmetric and irreflexive.
        /// Where geometry A - current geo object, geometry B - other geo object
        /// </summary>
        /// <param name="shape">Other geo object</param>
        /// <param name="session">Open session Orm</param>
        bool? StOverlaps(IGeoShape shape, ISession session);

        

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int? StSrid();

       

        /// <summary>
        /// Returns TRUE if A and B intersect, but their interiors do not intersect.
        /// Equivalently, A and B have at least one point in common, and the common points lie in at least one boundary.
        /// For Point/Point inputs the relationship is always FALSE, since points do not have a boundary.
        /// Where A - current geo object, B - other geo object
        /// </summary>
        /// <param name="shape">Other geo object</param>
        /// <param name="session">Open session orm</param>
        /// <returns></returns>
        bool? StTouches(IGeoShape shape, ISession session);


        /// <summary>
        /// Returns the number of elements in a geometry collection (GEOMETRYCOLLECTION or MULTI*). For non-empty atomic geometries returns 1. For empty geometries returns 0.
        /// </summary>
        /// <param name="session">Open session orm</param>
        int? StNumGeometries( ISession session);

        /// <summary>
        /// Return the number of interior rings of a polygon geometry. Return NULL if the geometry is not a polygon.
        /// </summary>
        /// <param name="session">Open session orm</param>
        int? StNumInteriorRing(ISession session);


        /// <summary>
        /// Returns true if this Geometry has no anomalous geometric points, such as self-intersection or self-tangency.
        /// </summary>
        /// <param name="session">Open session orm</param>
        bool? StIsSimple(ISession session);


        /// <summary>
        /// Tests if an current geometry  is well-formed and valid in 2D according to the OGC rules.
        /// </summary>
        /// <param name="session">Open session orm</param>
        /// <returns></returns>
        bool? StIsValid(ISession session);


        /// <summary>
        /// For geometry types: returns the 2D Cartesian length of the geometry if it is a LineString, MultiLineString, ST_Curve, ST_MultiCurve.
        /// For areal geometries 0 is returned; use ST_Perimeter instead.
        /// The units of length is determined by the spatial reference system of the geometry.
        /// </summary>
        /// <param name="session">Open session orm</param>
        double? StLength(ISession  session);

        /// <summary>
        /// Returns TRUE if the LINESTRING's start and end points are coincident. For Polyhedral Surfaces, reports if the surface is areal (open) or volumetric (closed).
        /// </summary>
        /// <param name="session">Open session orm</param>
        bool? StIsClosed(ISession  session);

        /// <summary>
        /// Return the number of points in an LineString or CircularString value.
        /// </summary>
        /// <param name="session">Open session orm</param>
        int? StNumPoints(ISession session);

        /// <summary>
        /// Returns the 2D perimeter of the geometry/geography if it is a ST_Surface, ST_MultiSurface (Polygon, MultiPolygon). 0 is returned for non-areal geometries.
        /// For linear geometries use ST_Length. For geometry types, units for perimeter measures are specified by the spatial reference system of the geometry.
        /// </summary>
        /// <param name="session">Open session orm</param>
        double? StPerimeter(ISession session);

        /// <summary>
        /// Returns a new geometry whose coordinates are translated delta x,delta y.
        /// Units are based on the units defined in spatial reference (SRID) for this geometry.
        /// </summary>
        /// <param name="deltaX">Coordinate x</param>
        /// <param name="deltaY">Coordinate y</param>
        /// <param name="session"></param>
        /// <returns></returns>
        IGeoShape StTranslate(float deltaX, float deltaY,ISession session);






















    }
}
