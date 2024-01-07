using ORM_1_21_.geo;
using ORM_1_21_.Linq;
using ORM_1_21_.Utils;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace ORM_1_21_.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class GeoExtension
    {
        /// <summary>
        /// Computes a POLYGON or MULTIPOLYGON that represents all points whose distance from a geometry/geography is less than or equal to a given distance.
        /// A negative distance shrinks the geometry rather than expanding it.
        /// A negative distance may shrink a polygon completely, in which case POLYGON EMPTY is returned.
        /// For points and lines negative distances always return empty results.
        /// </summary>
        /// <param name="session">Current session</param>
        /// <param name="shape">Center geo object shape</param>
        /// <param name="radius">
        /// A negative distance may shrink a polygon completely, in which case POLYGON EMPTY is returned.
        /// For points and lines negative distances always return empty results.
        /// </param>
        /// <param name="bufferStyleParameters">
        /// The optional third parameter controls the buffer accuracy and style.
        /// The accuracy of circular arcs in the buffer is specified as the number of line segments used to approximate a quarter circle (default is 8).
        /// The buffer style can be specifed by providing a list of blank-separated key=value pairs as follows:
        ///'quad_segs=#' : number of line segments used to approximate a quarter circle(default is 8).
        ///'endcap=round|flat|square' : endcap style(defaults to "round"). 'butt' is accepted as a synonym for 'flat'.
        ///'join=round|mitre|bevel' : join style(defaults to "round"). 'miter' is accepted as a synonym for 'mitre'.
        ///'mitre_limit=#.#' : mitre ratio limit(only affects mitered join style). 'miter_limit' is accepted as a synonym for 'mitre_limit'.
        ///'side=both|left|right' : 'left' or 'right' performs a single-sided buffer on the geometry,
        /// with the buffered side relative to the direction of the line.This is only applicable to LINESTRING geometry and does not affect POINT or POLYGON geometries.
        /// By default end caps are square.
        /// </param>
        /// <returns></returns>
        public static string GeoST_Buffer(this ISession session, IGeoShape shape, int radius, string bufferStyleParameters=null)
        {
           
            Check.NotNull(shape, nameof(shape));
            Check.NotNull(session, nameof(session));
            ProviderName providerName = session.ProviderName;
            string sql = string.Empty;
            if (providerName == ProviderName.PostgreSql)
            {
                string adding = string.Empty;
                if (!string.IsNullOrEmpty(bufferStyleParameters))
                {
                    adding = $", '{bufferStyleParameters}'";
                } 
                sql = $"SELECT ST_AsText(ST_Buffer(ST_GeomFromText('{shape.GeoText}',{shape.Srid}),{radius} {adding}));";
            }
            else if (providerName == ProviderName.MySql)
            {
                
                sql = $"SELECT ST_AsText(ST_Buffer(ST_GeomFromText('{shape.GeoText}',{shape.Srid}),{radius}));";
            }
            else if (providerName == ProviderName.MsSql)
            {
                sql = $"SELECT (geometry::STGeomFromText('{shape.GeoText}',{shape.Srid}).STBuffer({radius})).STAsText();";
            }


            var p = session.ExecuteScalar(sql);
            return (string)p;
        }





       

        /// <summary>
        /// Returns the area of the geometry if it is a Polygon or MultiPolygon.
        /// Return the area measurement of an ST_Surface or ST_MultiSurface value. (sqft)
        /// For geometry, a 2D Cartesian area is determined with units specified by the SRID.
        /// For geography, by default area is determined on a spheroid with units in square meters.
        /// To measure around the faster but less accurate sphere, use ST_Area(geog,false).
        /// </summary>
        /// <param name="session">Current session</param>
        /// <param name="o"></param>
        /// <param name="useSpheroid">Object IGeoShape</param>
        /// <returns> double sqft</returns>
        public static double GeoST_AreaAsSqFt(this ISession session, IGeoShape o,bool useSpheroid = true)
        {
            ProviderName providerName = session.ProviderName;
            if (providerName != ProviderName.PostgreSql)
            {
                throw new Exception("Only for PostgreSql");
            }
            Check.NotNull(o, nameof(o));
            Check.NotNull(session, nameof(session));
            string
                sql = $" SELECT ST_Area('{o.GeoText}');";
            if (useSpheroid == false)
            {
                sql = $" SELECT ST_Area('{o.GeoText}', false);";
            }

            var p = session.ExecuteScalar(sql);
            return (double)p;
        }

        /// <summary>
        /// Returns the area of the geometry if it is a Polygon or MultiPolygon.
        /// Return the area measurement of an ST_Surface or ST_MultiSurface value. (sqm)
        /// For geometry, a 2D Cartesian area is determined with units specified by the SRID.
        /// For geography, by default area is determined on a spheroid with units in square meters.
        /// To measure around the faster but less accurate sphere, use ST_Area(geog,false).
        /// </summary>
        /// <param name="session">Current session</param>
        /// <param name="o"></param>
        /// <param name="useSpheroid">Object IGeoShape</param>
        /// <returns> double sqm</returns>
        public static double GeoST_AreaAsSqM(this ISession session, IGeoShape o, bool useSpheroid = true)
        {
            ProviderName providerName = session.ProviderName;
            if (providerName != ProviderName.PostgreSql)
            {
                throw new Exception("Only for PostgreSql");
            }
            Check.NotNull(o, nameof(o));
            Check.NotNull(session, nameof(session));
            string
                sql = $" SELECT ST_Area('{o.GeoText}')*POWER(0.3048,2);";
            if (useSpheroid == false)
            {
                sql = $" SELECT ST_Area('{o.GeoText}', false)*POWER(0.3048,2);";
            }

            var p = session.ExecuteScalar(sql);
            return (double)p;
        }
        /// <summary>
        /// Returns the area of a polygonal geometry.
        /// For geometry types a 2D Cartesian (planar) area is computed, with units specified by the SRID.
        /// </summary>
        /// <param name="session">Current session</param>
        /// <param name="o">Geo object</param>
        /// <returns></returns>
        public static double GeoST_Area(this ISession session, IGeoShape o)
        {
            
            Check.NotNull(o, nameof(o));
            Check.NotNull(session, nameof(session));
            ProviderName providerName = session.ProviderName;
            string sql = $" select ST_Area(ST_GeomFromText('{o.GeoText}',{o.Srid}))";
            if (providerName == ProviderName.MsSql)
            {
                sql = $" select (geometry::STGeomFromText('{o.GeoText}',{o.Srid})).STArea()";
            }
            var p = session.ExecuteScalar(sql);
            return (double)p;
        }



        /// <summary>
        /// For geometry types returns the minimum 2D Cartesian (planar) distance between two geometries, in projected units (spatial ref units).
        /// </summary>
        /// <param name="session">Current session</param>
        /// <param name="o1">Object IGeoShape</param>
        /// <param name="o2">Object IGeoShape</param>
        /// <returns>Result as double (meters)</returns>
        public static double GeoST_Distance(this ISession session, IGeoShape o1, IGeoShape o2)
        {
            ProviderName providerName = session.ProviderName;
            if (providerName != ProviderName.PostgreSql)
            {
                throw new Exception("Only for PostgreSql");
            }
            Check.NotNull(o1, nameof(o1));
            Check.NotNull(o2, nameof(o2));
            Check.NotNull(session, nameof(session));
            var p = session.ExecuteScalar($"SELECT ST_Distance( ST_Transform ('SRID=4326;{o1.GeoText}'::geometry, 3857), " +
                                          $"ST_Transform('SRID=4326;{o2.GeoText}'::geometry, 3857)) * cosd(42.3521) ;");
            return (double)p;
        }

        

        /// <summary>
        /// Returns TRUE if geometry A is within geometry B. A is within B if and only if all points of A lie inside (i.e. in the interior or boundary of) B (or equivalently, no points of A lie in the exterior of B), and the interiors of A and B have at least one point in common.
        ///For this function to make sense, the source geometries must both be of the same coordinate projection, having the same SRID.
        /// </summary>
        /// <param name="session">Current session</param>
        /// <param name="o1">Object IGeoShape</param>
        /// <param name="o2">Object IGeoShape</param>
        /// <returns>Boll result</returns>
        public static bool ST_Within(this ISession session, IGeoShape o1, IGeoShape o2)
        {
            Check.NotNull(session, nameof(session));
            Check.NotNull(o1, nameof(o1));
            Check.NotNull(o2, nameof(o2));
            var p = session.ExecuteScalar($"SELECT ST_Within(ST_GeomFromText('{o1.GeoText}'),ST_GeomFromText('{o2.GeoText}'));");
            return (bool)p;
        }

        /// <summary>
        /// Returns TRUE if geometry A contains geometry B. A contains B if and only if all points of B lie inside (i.e. in the interior or boundary of)
        /// A (or equivalently, no points of B lie in the exterior of A), and the interiors of A and B have at least one point in common.
        /// </summary>
        /// <param name="session">Current session</param>
        /// <param name="o1">Object IGeoShape (A)</param>
        /// <param name="o2">Object IGeoShape (B)</param>
        /// <returns>Bool result</returns>
        public static bool ST_Contains(this ISession session, IGeoShape o1, IGeoShape o2)
        {
            Check.NotNull(session, nameof(session));
            Check.NotNull(o1, nameof(o1));
            Check.NotNull(o2, nameof(o2));
            var p = session.ExecuteScalar($"SELECT ST_Contains(ST_GeomFromText('{o1.GeoText}'),ST_GeomFromText('{o2.GeoText}'));");
            return (bool)p;
        }

        /// <summary>
        /// Test if an ST_Geometry value is well formed.
        /// </summary>
        /// <param name="session">Current Session</param>
        /// <param name="o">Object IGeoShape</param>
        /// <returns>bool result</returns>
        public static bool GeoST_IsValid(this ISession session, IGeoShape o)
        {
            Check.NotNull(session, nameof(session));
            Check.NotNull(o, nameof(o));
            ProviderName providerName = session.ProviderName;
            string sql = $"SELECT ST_IsValid(ST_GeomFromText('{o.GeoText}',{o.Srid}));";
            if (providerName == ProviderName.MsSql)
            {
                 sql = $"SELECT (geometry::STGeomFromText('{o.GeoText}',{o.Srid})).STIsValid();";
            }
            var p = session.ExecuteScalar(sql);
            if (providerName == ProviderName.MySql)
            {
                return p.ToString()=="1";
            }
            return (bool)p;
        }

        

        /// <summary>
        /// Returns TRUE if geometry A is within geometry B. A is within B if and only if all points of A lie inside (i.e. in the interior or boundary of) B (or equivalently, no points of A lie in the exterior of B), and the interiors of A and B have at least one point in common.
        ///For this function to make sense, the source geometries must both be of the same coordinate projection, having the same SRID.
        /// </summary>
        /// <param name="query">Query provider</param>
        /// <param name="selector">geometry A</param>
        /// <param name="geoObj">geometry B</param>
        /// <param name="actionResult">Comparison result as sql where</param>
        /// <typeparam name="T">The type of the elements of the input sequences</typeparam>
        public static IQueryable<T> GeoWhereST_Within<T>(this IQueryable<T> query, Expression<Func<T, IGeoShape>> selector, IGeoShape geoObj,bool actionResult=true)
        {
            Check.NotNull(query, nameof(query));
            Check.NotNull(selector, nameof(selector));
            Check.NotNull(geoObj, nameof(geoObj));
            var provider = (DbQueryProvider<T>)query.Provider;
            ProviderName providerName = provider.Sessione.ProviderName;
            var geo = Expression.Constant(geoObj);
            var nameColumnE = GetNameColumnCore(selector,providerName);
            var isNotE = Expression.Constant(actionResult);
            var selectorParameters = selector.Parameters;
            Expression check = Expression.Call(null,typeof(V).GetMethod("GeoST_Within"), nameColumnE,  geo,isNotE);
            var lambada = Expression.Lambda<Func<T, bool>>(check, selectorParameters);
            return query.Where(lambada);
        }

        /// <summary>
        /// Returns TRUE if geometry A contains geometry B. A contains B if and only if all points of B lie inside (i.e. in the interior or boundary of)
        /// A (or equivalently, no points of B lie in the exterior of A), and the interiors of A and B have at least one point in common.
        /// </summary>
        /// <param name="query">Query provider</param>
        /// <param name="selector">geometry A</param>
        /// <param name="geoObj">geometry B</param>
        /// <param name="actionResult">Comparison result as sql where</param>
        /// <typeparam name="T">The type of the elements of the input sequences</typeparam>
        public static IQueryable<T> GeoWhereST_Contains<T>(this IQueryable<T> query, Expression<Func<T, IGeoShape>> selector, IGeoShape geoObj, bool actionResult = true)
        {
            Check.NotNull(query, nameof(query));
            Check.NotNull(selector, nameof(selector));
            Check.NotNull(geoObj, nameof(geoObj));
            var provider = (DbQueryProvider<T>)query.Provider;
            ProviderName providerName = provider.Sessione.ProviderName;
            var geo = Expression.Constant(geoObj);
            var nameColumnE = GetNameColumnCore(selector, providerName);
            var isNotE = Expression.Constant(actionResult);
            var selectorParameters = selector.Parameters;
            Expression check = Expression.Call(null, typeof(V).GetMethod("GeoST_Contains"), nameColumnE, geo, isNotE);
            var lambada = Expression.Lambda<Func<T, bool>>(check, selectorParameters);
            return query.Where(lambada);
        }
        /// <summary>
        /// Returns true if two geometries intersect. Geometries intersect if they have any point in common.
        ///For geography, a distance tolerance of 0.00001 meters is used(so points that are very close are considered to intersect).
        /// </summary>
        /// <param name="query"></param>
        /// <param name="selector"></param>
        /// <param name="geoObj"></param>
        /// <param name="actionResult"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IQueryable<T> GeoWhereST_Intersects<T>(this IQueryable<T> query, Expression<Func<T, IGeoShape>> selector, IGeoShape geoObj, bool actionResult = true)
        {
            Check.NotNull(query, nameof(query));
            Check.NotNull(selector, nameof(selector));
            Check.NotNull(geoObj, nameof(geoObj));
            var provider = (DbQueryProvider<T>)query.Provider;
            ProviderName providerName = provider.Sessione.ProviderName;
            var geo = Expression.Constant(geoObj);
            var nameColumnE = GetNameColumnCore(selector, providerName);
            var isNotE = Expression.Constant(actionResult);
            var selectorParameters = selector.Parameters;
            Expression check = Expression.Call(null, typeof(V).GetMethod("GeoST_Intersects"), nameColumnE, geo, isNotE);
            var lambada = Expression.Lambda<Func<T, bool>>(check, selectorParameters);
            return query.Where(lambada);
        }


        /// <summary>
        /// Returns true if two geometries are disjoint. Geometries are disjoint if they have no point in common.
        ///If any other spatial relationship is true for a pair of geometries, they are not disjoint.Disjoint implies that ST_Intersects is false.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="selector"></param>
        /// <param name="geoObj"></param>
        /// <param name="actionResult"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IQueryable<T> GeoWhereST_Disjoint<T>(this IQueryable<T> query, Expression<Func<T, IGeoShape>> selector, IGeoShape geoObj, bool actionResult = true)
        {
            Check.NotNull(query, nameof(query));
            Check.NotNull(selector, nameof(selector));
            Check.NotNull(geoObj, nameof(geoObj));
            var provider = (DbQueryProvider<T>)query.Provider;
            ProviderName providerName = provider.Sessione.ProviderName;
            var geo = Expression.Constant(geoObj);
            var nameColumnE = GetNameColumnCore(selector, providerName);
            var isNotE = Expression.Constant(actionResult);
            var selectorParameters = selector.Parameters;
            Expression check = Expression.Call(null, typeof(V).GetMethod("GeoST_Disjoint"), nameColumnE, geo, isNotE);
            var lambada = Expression.Lambda<Func<T, bool>>(check, selectorParameters);
            return query.Where(lambada);
        }

        /// <summary>
        /// Compares two geometry objects and returns true if their intersection
        /// "spatially crosses"; that is, the geometries have some, but not all interior points in common.
        /// The intersection of the interiors of the geometries must be non-empty and must have dimension less than the maximum dimension of the two input geometries,
        /// and the intersection of the two geometries must not equal either geometry.
        /// Otherwise, it returns false. The crosses relation is symmetric and irreflexive.
        /// </summary>
        /// <param name="query">Current IQueryable</param>
        /// <param name="selector">Property object as IGeoShape</param>
        /// <param name="geoObj">Object for comparison</param>
        /// <param name="actionResult">Result of the comparison (default true)</param>
        /// <typeparam name="T">Table Entity Type</typeparam>
        /// <returns>bool</returns>
        public static IQueryable<T> GeoWhereST_Crosses<T>(this IQueryable<T> query, Expression<Func<T, IGeoShape>> selector, IGeoShape geoObj, bool actionResult = true)
        {
            Check.NotNull(query, nameof(query));
            Check.NotNull(selector, nameof(selector));
            Check.NotNull(geoObj, nameof(geoObj));
            var provider = (DbQueryProvider<T>)query.Provider;
            ProviderName providerName = provider.Sessione.ProviderName;
            var geo = Expression.Constant(geoObj);
            var nameColumnE = GetNameColumnCore(selector, providerName);
            var isNotE = Expression.Constant(actionResult);
            var selectorParameters = selector.Parameters;
            Expression check = Expression.Call(null, typeof(V).GetMethod("GeoST_Crosses"), nameColumnE, geo, isNotE);
            var lambada = Expression.Lambda<Func<T, bool>>(check, selectorParameters);
            return query.Where(lambada);
        }

        /// <summary>
        /// Returns true if the given geometries are "topologically equal".
        /// Use this for a 'better' answer than '='. Topological equality means that the geometries have the same dimension,
        /// and their point-sets occupy the same space.
        /// This means that the order of vertices may be different in topologically equal geometries.
        /// To verify the order of points is consistent use ST_OrderingEquals
        /// (it must be noted ST_OrderingEquals is a little more stringent than simply verifying order of points are the same).
        /// </summary>
        /// <param name="query">Current IQueryable</param>
        /// <param name="selector">Property object as IGeoShape</param>
        /// <param name="geoObj">Object for comparison</param>
        /// <param name="actionResult">Result of the comparison (default true)</param>
        /// <typeparam name="T">Table Entity Type</typeparam>
        /// <returns>bool</returns>
        public static IQueryable<T> GeoWhereST_Equals<T>(this IQueryable<T> query, Expression<Func<T, IGeoShape>> selector, IGeoShape geoObj, bool actionResult = true)
        {
            Check.NotNull(query, nameof(query));
            Check.NotNull(selector, nameof(selector));
            Check.NotNull(geoObj, nameof(geoObj));
            var provider = (DbQueryProvider<T>)query.Provider;
            ProviderName providerName = provider.Sessione.ProviderName;
            var geo = Expression.Constant(geoObj);
            var nameColumnE = GetNameColumnCore(selector, providerName);
            var isNotE = Expression.Constant(actionResult);
            var selectorParameters = selector.Parameters;
            Expression check = Expression.Call(null, typeof(V).GetMethod("GeoST_Equals"), nameColumnE, geo, isNotE);
            var lambada = Expression.Lambda<Func<T, bool>>(check, selectorParameters);
            return query.Where(lambada);
        }

        /// <summary>
        /// Returns TRUE if geometry A and B "spatially overlap". Two geometries overlap if they have the same dimension, their interiors intersect in that dimension.
        /// and each has at least one point inside the other (or equivalently, neither one covers the other).
        /// The overlaps relation is symmetric and irreflexive.
        /// </summary>
        /// <param name="query">Current IQueryable</param>
        /// <param name="selector">Property object as IGeoShape</param>
        /// <param name="geoObj">Object for comparison</param>
        /// <param name="actionResult">Result of the comparison (default true)</param>
        /// <typeparam name="T">Table Entity Type</typeparam>
        /// <returns>bool</returns>
        public static IQueryable<T> GeoWhereST_Overlaps<T>(this IQueryable<T> query, Expression<Func<T, IGeoShape>> selector, IGeoShape geoObj, bool actionResult = true)
        {
            Check.NotNull(query, nameof(query));
            Check.NotNull(selector, nameof(selector));
            Check.NotNull(geoObj, nameof(geoObj));
            var provider = (DbQueryProvider<T>)query.Provider;
            ProviderName providerName = provider.Sessione.ProviderName;
            var geo = Expression.Constant(geoObj);
            var nameColumnE = GetNameColumnCore(selector, providerName);
            var isNotE = Expression.Constant(actionResult);
            var selectorParameters = selector.Parameters;
            Expression check = Expression.Call(null, typeof(V).GetMethod("GeoST_Overlaps"), nameColumnE, geo, isNotE);
            var lambada = Expression.Lambda<Func<T, bool>>(check, selectorParameters);
            return query.Where(lambada);
        }

        /// <summary>
        /// Returns TRUE if A and B intersect, but their interiors do not intersect.
        /// Equivalently, A and B have at least one point in common, and the common points lie in at least one boundary.
        /// For Point/Point inputs the relationship is always FALSE, since points do not have a boundary.
        /// </summary>
        /// <param name="query">Current IQueryable</param>
        /// <param name="selector">Property object as IGeoShape</param>
        /// <param name="geoObj">Object for comparison</param>
        /// <param name="actionResult">Result of the comparison (default true)</param>
        /// <typeparam name="T">Table Entity Type</typeparam>
        /// <returns>bool</returns>
        public static IQueryable<T> GeoWhereST_Touches<T>(this IQueryable<T> query, Expression<Func<T, IGeoShape>> selector, IGeoShape geoObj, bool actionResult = true)
        {
            Check.NotNull(query, nameof(query));
            Check.NotNull(selector, nameof(selector));
            Check.NotNull(geoObj, nameof(geoObj));
            var provider = (DbQueryProvider<T>)query.Provider;
            ProviderName providerName = provider.Sessione.ProviderName;
            var geo = Expression.Constant(geoObj);
            var nameColumnE = GetNameColumnCore(selector, providerName);
            var isNotE = Expression.Constant(actionResult);
            var selectorParameters = selector.Parameters;
            Expression check = Expression.Call(null, typeof(V).GetMethod("GeoST_Touches"), nameColumnE, geo, isNotE);
            var lambada = Expression.Lambda<Func<T, bool>>(check, selectorParameters);
            return query.Where(lambada);
        }


        /// <summary>
        /// Test if an ST_Geometry value is well formed.
        /// </summary>
        /// <param name="query">Current IQueryable</param>
        /// <param name="selector">Property object as IGeoShape</param>
        /// <param name="actionResult">Result of the comparison (default true)</param>
        /// <typeparam name="T">Table Entity Type</typeparam>
        /// <returns>bool</returns>
        public static IQueryable<T> GeoWhereST_IsValid<T>(this IQueryable<T> query, Expression<Func<T, IGeoShape>> selector,  bool actionResult = true)
        {
            Check.NotNull(query, nameof(query));
            Check.NotNull(selector, nameof(selector));
          
            var provider = (DbQueryProvider<T>)query.Provider;
            ProviderName providerName = provider.Sessione.ProviderName;
            var nameColumnE = GetNameColumnCore(selector, providerName);
            var isNotE = Expression.Constant(actionResult);
            var selectorParameters = selector.Parameters;
            Expression check = Expression.Call(null, typeof(V).GetMethod("GeoST_IsValid"),nameColumnE,   isNotE);
            var lambada = Expression.Lambda<Func<T, bool>>(check, selectorParameters);
            return query.Where(lambada);
        }




        /// <summary>
        /// Returns givenned type to geometries
        /// </summary>
        /// <param name="query">Current IQueryable</param>
        /// <param name="selector">Property object as IGeoShape</param>
        /// <param name="type">Enum GeoType</param>
        /// <typeparam name="T">Table Entity Type</typeparam>
        /// <returns>bool</returns>
        public static IQueryable<T> GeoWhereST_GeometryType<T>(this IQueryable<T> query, Expression<Func<T, IGeoShape>> selector,GeoType type)
        {
            Check.NotNull(query, nameof(query));
            Check.NotNull(selector, nameof(selector));
           
          
            var provider = (DbQueryProvider<T>)query.Provider;
            ProviderName providerName = provider.Sessione.ProviderName;
            
            
            
            var nameColumnE = GetNameColumnCore(selector, providerName);
          
            var selectorParameters = selector.Parameters;
            var typeE = Expression.Constant((int)type);
            Expression check = Expression.Call(null, typeof(V).GetMethod("GeoST_GeometryType"), nameColumnE,typeE);
            var lambada = Expression.Lambda<Func<T, bool>>(check, selectorParameters);
            return query.Where(lambada);
        }

        /// <summary>
        /// Returns true if the geometries are within a given distance
        /// For geometry: The distance is specified in units defined by the spatial reference system of the geometries.
        /// For this function to make sense, the source geometries must be in the same coordinate system (have the same SRID). (result value as meter)
        /// </summary>
        /// <param name="query"></param>
        /// <param name="selector"></param>
        /// <param name="shape">Object base</param>
        /// <param name="distance"> Value Distance</param>
        /// <param name="actionResult"> Default true</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IQueryable<T> GeoWhereST_DWithin<T>(this IQueryable<T> query, Expression<Func<T, IGeoShape>> selector, IGeoShape shape, int distance, bool actionResult = true)
        {
            Check.NotNull(query, nameof(query));
            Check.NotNull(selector, nameof(selector));
            Check.NotNull(shape, nameof(shape));
            var provider = (DbQueryProvider<T>)query.Provider;
            ProviderName providerName = provider.Sessione.ProviderName;
            var nameColumnE = GetNameColumnCore(selector, providerName);

            var selectorParameters = selector.Parameters;
            var shapeE = Expression.Constant(shape);
            var distanceE = Expression.Constant(distance);
            var isNotE = Expression.Constant(actionResult);
            Expression check = Expression.Call(null, typeof(V).GetMethod("GeoST_DWithin"), nameColumnE, shapeE, distanceE,isNotE);
            var lambada = Expression.Lambda<Func<T, bool>>(check, selectorParameters);
            return query.Where(lambada);
        }

        /// <summary>
        /// For geometry types: returns the 2D Cartesian length of the geometry if it is a LineString, MultiLineString, ST_Curve, ST_MultiCurve.
        /// For areal geometries 0 is returned; use ST_Perimeter instead.
        /// The units of length is determined by the spatial reference system of the geometry.
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public static double ST_Length(this IGeoShape shape ,ISession session)
        {
            Check.NotNull(session, nameof(session));
            Check.NotNull(shape, nameof(shape));
            ProviderName providerName = session.ProviderName;
            if (providerName == ProviderName.MsSql)
            {
                var res0 = session.ExecuteScalar($" SELECT (geometry::STGeomFromText({session.SymbolParam}1,{shape.Srid})).STLength();", shape.GeoText);
                return (double)res0;
            }
            var res=session.ExecuteScalar($" SELECT ST_Length(ST_GeomFromText({session.SymbolParam}1,{shape.Srid}));",shape.GeoText);
            return (double)res;
        }

        /// <summary>
        /// Returns the OGC Well-Known Text (WKT) representation of the geography.
        /// </summary>
        /// <param name="shape">Geo Object</param>
        /// <returns>string</returns>
        public static string ST_AsText(this IGeoShape shape)
        {
            Check.NotNull(shape, nameof(shape));
           
            return shape.GeoText;
        }

        /// <summary>
        /// Returns a geometry as a GeoJSON "geometry". Only for Postgres or Mysql
        /// </summary>
        /// <param name="shape">Geo object</param>
        /// <param name="session">ISession orm</param>
        /// <returns>GeoJson as string</returns>
        public static string ST_AsGeoJSON(this IGeoShape shape,ISession session)
        {
            Check.NotNull(shape, nameof(shape));
            Check.NotNull(session, nameof(session));
            ProviderName providerName = session.ProviderName;
            if (providerName == ProviderName.MsSql)
            {
                throw new Exception("Only for Postgres or Mysql");
            }
           
            return  (string)session.ExecuteScalar($"SELECT ST_AsGeoJSON(ST_GeomFromText({session.SymbolParam}1,{shape.Srid}));", shape.GeoText);

        }

        /// <summary>
        /// Returns the 2D perimeter of the geometry/geography if it is a ST_Surface, ST_MultiSurface (Polygon, MultiPolygon).
        /// 0 is returned for non-areal geometries. For linear geometries use ST_Length. 
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public static double ST_Perimeter(this IGeoShape shape, ISession session)
        {
            Check.NotNull(shape, nameof(shape));
            Check.NotNull(session, nameof(session));
            ProviderName providerName = session.ProviderName;
            if (providerName != ProviderName.PostgreSql)
            {
                throw new Exception("Only for Postgres");
            }
            return (double)session.ExecuteScalar($"SELECT ST_Perimeter(ST_GeomFromText({session.SymbolParam}1,{shape.Srid}));", shape.GeoText);
        }
       

        /// <summary>
        /// Returns a geography object as byte[], from the well-known text representation. SRID 4326 is assumed if unspecified.
        /// </summary>
        /// <returns></returns>
        public static byte[] ST_GeoToByteArray(this IGeoShape shape, ISession session)
        {
            Check.NotNull(shape, nameof(shape));
            Check.NotNull(session, nameof(session));
            ProviderName providerName = session.ProviderName;
            switch (providerName)
            {
                case ProviderName.MsSql:
                    return session.ExecuteScalar($"select geometry::STGeomFromText({session.SymbolParam}1,{shape.Srid}).STAsBinary()", shape.GeoText) as byte[];
                  
                case ProviderName.MySql:
                    return session.ExecuteScalar($"SELECT HEX(ST_GeomFromText({session.SymbolParam}1,{shape.Srid})) ", shape.GeoText) as byte[];
                case ProviderName.PostgreSql:
                    return session.ExecuteScalar($"SELECT ST_GeomFromText(@1,{shape.Srid})::bytea ", shape.GeoText) as byte[];
                case ProviderName.SqLite:
                    throw new Exception("Not support geo object");
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
           
            
         
        }
       


















         static ConstantExpression GetNameColumnCore<T>(Expression<Func<T, IGeoShape>> selector,
             ProviderName providerName)
        {
            var m = (selector.Body as MemberExpression);
            var nameColumn = GetColumnNameSimple<T>(m.Member.Name, m.Expression.Type,providerName);
            var nameTable = GetTableName<T>(providerName);
            return Expression.Constant($"{nameTable}.{nameColumn}");
        }

        
        private static string GetColumnNameSimple<T>(string member, Type type, ProviderName providerName)
        {
            var ss = AttributesOfClass<T>.GetNameSimpleColumnForQuery(member,providerName);
            return ss;
        }
        private static string GetTableName<T>(ProviderName providerName)
        {
            var ss = AttributesOfClass<T>.TableName(providerName);
            return ss;
        }

        







    }
}
