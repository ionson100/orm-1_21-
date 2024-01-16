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
        string StGeometryType();

        /// <summary>
        /// Returns the area of a polygonal geometry. For geometry types a 2D Cartesian (planar) area is computed, with units specified by the SRID
        /// </summary>
        double? StArea();

        /// <summary>
        /// Returns TRUE if current geometry  is within geometry B. current geometry is within B if and only if all points of current geometry lie inside (i.e. in the interior or boundary of) B
        /// (or equivalently, no points of current geometry lie in the exterior of B), and the interiors of current geometry and B have at least one point in common.
        /// </summary>
        /// <param name="shape">Geometry B</param>
        bool? StWithin(IGeoShape  shape);

        /// <summary>
        /// Returns the OGC/ISO Well-Known Binary (WKB) representation of the geometry.
        /// </summary>
        byte[] StAsBinary();











        /// <summary>
        /// Returns the closure of the combinatorial boundary of this Geometry.
        /// The combinatorial boundary is defined as described in section 3.12.3.2 of the OGC SPEC.
        /// Because the result of this function is a closure, and hence topologically closed, the resulting boundary
        /// can be represented using representational geometry primitives as discussed in the OGC SPEC, section 3.12.2.
        /// </summary>
        IGeoShape StBoundary();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        IGeoShape StBuffer(float distance);

        /// <summary>
        /// Computes a point which is the geometric center of mass of a geometry.
        /// For [MULTI]POINTs, the centroid is the arithmetic mean of the input coordinates.
        /// For [MULTI]LINESTRINGs, the centroid is computed using the weighted length of each line segment.
        /// For [MULTI]POLYGONs, the centroid is computed in terms of area.
        /// If an empty geometry is supplied, an empty GEOMETRYCOLLECTION is returned. If NULL is supplied, NULL is returned.
        /// If CIRCULARSTRING or COMPOUNDCURVE are supplied, they are converted to linestring with CurveToLine first, then same than for LINESTRING
        /// </summary>
        IGeoShape StCentroid();

        /// <summary>
        /// Returns the last point of a LINESTRING or CIRCULARLINESTRING geometry as a POINT. Returns NULL if the input is not a LINESTRING or CIRCULARLINESTRING.
        /// </summary>
        IGeoShape StEndPoint();

        /// <summary>
        /// Returns the minimum axis-aligned bounding rectangle of the instance.
        /// </summary>
        IGeoShape StEnvelope();

        /// <summary>
        /// Returns the first point of a LINESTRING or CIRCULARLINESTRING geometry as a POINT. Returns NULL if the input is not a LINESTRING or CIRCULARLINESTRING.
        /// </summary>
        IGeoShape StStartPoint();

        /// <summary>
        /// Returns an object that represents all points that are either in one geometry instance or another geometry instance, but not those points that lie in both instances.
        /// </summary>
        /// <param name="shape">Other geo object</param>
        IGeoShape StSymDifference(IGeoShape shape);

        /// <summary>
        /// Returns an object that represents the union of a geometry instance with another geometry instance.
        /// </summary>
        /// <param name="shape">Other geo object</param>
        IGeoShape StUnion(IGeoShape shape);

        /// <summary>
        /// Returns TRUE if current geometry  contains geometry B. 
        /// </summary>
        /// <param name="shape">Geometry B</param>
        bool? StContains(IGeoShape shape);

        /// <summary>
        /// Compares two geometry objects and returns true if their intersection "spatially crosses"; that is, the geometries have some, but not all interior points in common.
        /// The intersection of the interiors of the geometries must be non-empty and must have dimension less than the maximum dimension of the two input geometries,
        /// and the intersection of the two geometries must not equal either geometry. Otherwise, it returns false.
        /// The crosses relation is symmetric and irreflexive.
        /// </summary>
        /// <param name="shape">Other geo object</param>
        bool? StCrosses(IGeoShape shape);

        /// <summary>
        /// Returns a geometry representing the part of geometry A that does not intersect geometry B.
        /// This is equivalent to A - ST_Intersection(A,B).
        /// If A is completely contained in B then an empty atomic geometry of appropriate type is returned.
        /// Where geometry A - current geo object, geometry B other geo object.
        /// </summary>
        /// <param name="shape">Other geo object</param>
        IGeoShape StDifference(IGeoShape shape);

        /// <summary>
        /// Return the topological dimension of this Geometry object, which must be less than or equal to the coordinate dimension
        /// </summary>
        int? StDimension();

        /// <summary>
        /// Returns true if two geometries are disjoint. Geometries are disjoint if they have no point in common.
        /// </summary>
        /// <param name="shape">Other geo object</param>
        bool? StDisjoint(IGeoShape shape);

        /// <summary>
        /// For geometry types returns the minimum 2D Cartesian (planar) distance between two geometries, in projected units (spatial ref units).
        /// </summary>
        /// <param name="shape">Other geo object</param>
        double? StDistance(IGeoShape shape);

      


        /// <summary>
        /// Returns true if the given geometries are "topologically equal".
        /// Use this for a 'better' answer than '='.
        /// Topological equality means that the geometries have the same dimension, and their point-sets occupy the same space.
        /// This means that the order of vertices may be different in topologically equal geometries.
        /// To verify the order of points is consistent use ST_OrderingEquals (it must be noted ST_OrderingEquals is a little more stringent than simply verifying order of points are the same).
        /// </summary>
        /// <param name="shape">Other geo object</param>
        bool? StEquals(IGeoShape shape);


        /// <summary>
        /// Returns true if two geometries intersect. Geometries intersect if they have any point in common.
        ///For geography, a distance tolerance of 0.00001 meters is used (so points that are very close are considered to intersect).
        /// </summary>
        /// <param name="shape">Other geo object</param>
        bool? StIntersects(IGeoShape shape);

        /// <summary>
        /// Returns TRUE if geometry A and B "spatially overlap".
        /// Two geometries overlap if they have the same dimension, their interiors intersect in that dimension.
        /// and each has at least one point inside the other (or equivalently, neither one covers the other).
        /// The overlaps relation is symmetric and irreflexive.
        /// Where geometry A - current geo object, geometry B - other geo object
        /// </summary>
        /// <param name="shape">Other geo object</param>
        bool? StOverlaps(IGeoShape shape);

        

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
        bool? StTouches(IGeoShape shape);


        /// <summary>
        /// Returns the number of elements in a geometry collection (GEOMETRYCOLLECTION or MULTI*). For non-empty atomic geometries returns 1. For empty geometries returns 0.
        /// </summary>
        int? StNumGeometries();

        /// <summary>
        /// Return the number of interior rings of a polygon geometry. Return NULL if the geometry is not a polygon.
        /// </summary>
        int? StNumInteriorRing();


        /// <summary>
        /// Returns true if this Geometry has no anomalous geometric points, such as self-intersection or self-tangency.
        /// </summary>
      
        bool? StIsSimple();


        /// <summary>
        /// Tests if an current geometry  is well-formed and valid in 2D according to the OGC rules.
        /// </summary>
       
        bool? StIsValid();


        /// <summary>
        /// For geometry types: returns the 2D Cartesian length of the geometry if it is a LineString, MultiLineString, ST_Curve, ST_MultiCurve.
        /// For areal geometries 0 is returned; use ST_Perimeter instead.
        /// The units of length is determined by the spatial reference system of the geometry.
        /// </summary>
      
        double? StLength();

        /// <summary>
        /// Returns TRUE if the LINESTRING's start and end points are coincident. For Polyhedral Surfaces, reports if the surface is areal (open) or volumetric (closed).
        /// </summary>
        bool? StIsClosed();

        /// <summary>
        /// Return the number of points in an LineString or CircularString value.
        /// </summary>
        int? StNumPoints();

        /// <summary>
        /// Returns the 2D perimeter of the geometry/geography if it is a ST_Surface, ST_MultiSurface (Polygon, MultiPolygon). 0 is returned for non-areal geometries.
        /// For linear geometries use ST_Length. For geometry types, units for perimeter measures are specified by the spatial reference system of the geometry.
        /// </summary>
        double? StPerimeter();

        /// <summary>
        /// Returns a new geometry whose coordinates are translated delta x,delta y.
        /// Units are based on the units defined in spatial reference (SRID) for this geometry.
        /// </summary>
        /// <param name="deltaX">Coordinate x</param>
        /// <param name="deltaY">Coordinate y</param>
        IGeoShape StTranslate(float deltaX, float deltaY);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        IGeoShape SetSession(ISession session);


        /// <summary>
        /// Computes the convex hull of a geometry. The convex hull is the smallest convex geometry that encloses all geometries in the input.
        /// </summary>
        IGeoShape StConvexHull();

        /// <summary>
        /// Collects geometries into a geometry collection. The result is either a Multi* or a GeometryCollection,
        /// depending on whether the input geometries have the same or different types (homogeneous or heterogeneous).
        /// The input geometries are left unchanged within the collection.
        /// </summary>
        /// <param name="shapes"></param>
        /// <returns></returns>
        IGeoShape StCollect(params IGeoShape[] shapes);



        /// <summary>
        /// Return the Nth point in a single linestring or circular linestring in the geometry.
        /// Negative values are counted backwards from the end of the LineString, so that -1 is the last point. Returns NULL if there is no linestring in the geometry.
        /// </summary>
        /// <param name="n">Number point</param>
        IGeoShape StPointN(int n);

        /// <summary>
        /// Returns a POINT which is guaranteed to lie in the interior of a surface (POLYGON, MULTIPOLYGON, and CURVED POLYGON).
        /// In PostGIS this function also works on line and point geometries.
        /// </summary>
        /// <returns></returns>
        IGeoShape StPointOnSurface();

        /// <summary>
        /// Returns the Nth interior ring (hole) of a POLYGON geometry as a LINESTRING. The index starts at 1.
        /// Returns NULL if the geometry is not a polygon or the index is out of range.
        /// </summary>
        /// <returns></returns>
        IGeoShape StInteriorRingN(int n);

        /// <summary>
        /// Return the X coordinate of the point, or NULL if not available. Input must be a point.
        /// </summary>
        /// <returns></returns>
        double? StX();

        /// <summary>
        /// Return the Y coordinate of the point, or NULL if not available. Input must be a point.
        /// </summary>
        /// <returns></returns>
        double? StY();

        /// <summary>
        /// Returns a new geometry with its coordinates transformed to a different spatial reference system.
        /// </summary>
        /// <param name="srid">Spatial reference system</param>
        IGeoShape StTransform(int srid);

        /// <summary>
        /// Sets the SRID on a geometry to a particular integer value.
        /// </summary>
        /// <param name="srid">Spatial reference system</param>
        // ReSharper disable once InconsistentNaming
        IGeoShape StSetSRID(int srid);

        /// <summary>
        /// Returns the Degrees, Minutes, Seconds representation of the point.
        /// </summary>
        /// <param name="format">The text parameter is a format string containing the format for the resulting text, similar to a date format string
        /// as 'D°M''S.SSS"C'</param>
        /// <returns></returns>
        string StAsLatLonText(string format="");

        /// <summary>
        /// Reverse points current geometry
        /// </summary>
        /// <returns></returns>
        IGeoShape StReverse();






















    }
}
