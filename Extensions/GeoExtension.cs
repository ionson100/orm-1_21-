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
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="providerName"></param>
        /// <returns></returns>
        public static string GetTypeName(this GeoType type, ProviderName providerName)
        {
            switch (providerName)
            {
                case ProviderName.MsSql:
                    return type.ToString();
                case ProviderName.MySql:
                    if (type == GeoType.GeometryCollection)
                    {
                        return "GEOMCOLLECTION";
                    }
                    return type.ToString().ToUpper();
                case ProviderName.PostgreSql:
                    return $"ST_{type.ToString()}";
               
            }
          

            return null;
        }
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
                sql = $"SELECT ST_AsText(ST_Buffer(ST_GeomFromText('{shape.StAsText()}',{((GeoObject)shape).Srid}),{radius} {adding}));";
            }
            else if (providerName == ProviderName.MySql)
            {
                
                sql = $"SELECT ST_AsText(ST_Buffer(ST_GeomFromText('{shape.StAsText()}',{((GeoObject)shape).Srid}),{radius}));";
            }
            else if (providerName == ProviderName.MsSql)
            {
                sql = $"SELECT (geometry::STGeomFromText('{shape.StAsText()}',{((GeoObject)shape).Srid}).STBuffer({radius})).STAsText();";
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
                sql = $" SELECT ST_Area('{o.StAsText()}');";
            if (useSpheroid == false)
            {
                sql = $" SELECT ST_Area('{o.StAsText()}', false);";
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
                sql = $" SELECT ST_Area('{o.StAsText()}')*POWER(0.3048,2);";
            if (useSpheroid == false)
            {
                sql = $" SELECT ST_Area('{o.StAsText()}', false)*POWER(0.3048,2);";
            }

            var p = session.ExecuteScalar(sql);
            return (double)p;
        }
        /// <summary>
        /// Returns the area of a polygonal geometry.
        /// For geometry types a 2D Cartesian (planar) area is computed, with units specified by the SRID.
        /// </summary>
        /// <param name="session">Current session</param>
        /// <param name="shape">Geo object</param>
        /// <returns></returns>
        public static double GeoST_Area(this ISession session, IGeoShape shape)
        {
            
            Check.NotNull(shape, nameof(shape));
            Check.NotNull(session, nameof(session));
            ProviderName providerName = session.ProviderName;
            string sql = $" select ST_Area(ST_GeomFromText('{shape.StAsText()}',{((GeoObject)shape).Srid}))";
            if (providerName == ProviderName.MsSql)
            {
                sql = $" select (geometry::STGeomFromText('{shape.StAsText()}',{((GeoObject)shape).Srid})).STArea()";
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
            var p = session.ExecuteScalar($"SELECT ST_Distance( ST_Transform ('SRID=4326;{o1.StAsText()}'::geometry, 3857), " +
                                          $"ST_Transform('SRID=4326;{o2.StAsText()}'::geometry, 3857)) * cosd(42.3521) ;");
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
            var p = session.ExecuteScalar($"SELECT ST_Within(ST_GeomFromText('{o1.StAsText()}'),ST_GeomFromText('{o2.StAsText()}'));");
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
            var p = session.ExecuteScalar($"SELECT ST_Contains(ST_GeomFromText('{o1.StAsText()}'),ST_GeomFromText('{o2.StAsText()}'));");
            return (bool)p;
        }

        /// <summary>
        /// Test if an ST_Geometry value is well formed.
        /// </summary>
        /// <param name="session">Current Session</param>
        /// <param name="shape">Object IGeoShape</param>
        /// <returns>bool result</returns>
        public static bool GeoST_IsValid(this ISession session, IGeoShape shape)
        {
            Check.NotNull(session, nameof(session));
            Check.NotNull(shape, nameof(shape));
            ProviderName providerName = session.ProviderName;
            string sql = $"SELECT ST_IsValid(ST_GeomFromText('{shape.StAsText()}',{((GeoObject)shape).Srid}));";
            if (providerName == ProviderName.MsSql)
            {
                 sql = $"SELECT (geometry::STGeomFromText('{shape.StAsText()}',{((GeoObject)shape).Srid})).STIsValid();";
            }
            var p = session.ExecuteScalar(sql);
            if (providerName == ProviderName.MySql)
            {
                return p.ToString()=="1";
            }
            return (bool)p;
        }

        

       
       //public static IQueryable<T> GeoWhereST_Within<T>(this IQueryable<T> query, Expression<Func<T, IGeoShape>> selector, IGeoShape geoObj,bool actionResult=true)
       //{
       //    Check.NotNull(query, nameof(query));
       //    Check.NotNull(selector, nameof(selector));
       //    Check.NotNull(geoObj, nameof(geoObj));
       //    var provider = (DbQueryProvider<T>)query.Provider;
       //    ProviderName providerName = provider.Sessione.ProviderName;
       //    var geo = Expression.Constant(geoObj);
       //    var nameColumnE = GetNameColumnCore(selector,providerName);
       //    var isNotE = Expression.Constant(actionResult);
       //    var selectorParameters = selector.Parameters;
       //    Expression check = Expression.Call(null,typeof(V).GetMethod("GeoST_Within"), nameColumnE,  geo,isNotE);
       //    var lambada = Expression.Lambda<Func<T, bool>>(check, selectorParameters);
       //    return query.Where(lambada);
       //}

       
      

        

      

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
           
            return  (string)session.ExecuteScalar($"SELECT ST_AsGeoJSON(ST_GeomFromText({session.SymbolParam}1,{((GeoObject)shape).Srid}));", shape.StAsText());

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
            return (double)session.ExecuteScalar($"SELECT ST_Perimeter(ST_GeomFromText({session.SymbolParam}1,{((GeoObject)shape).Srid}));", shape.StAsText());
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
                    return session.ExecuteScalar($"select geometry::STGeomFromText({session.SymbolParam}1,{((GeoObject)shape).Srid}).STAsBinary()", shape.StAsText()) as byte[];
                  
                case ProviderName.MySql:
                    return session.ExecuteScalar($"SELECT HEX(ST_GeomFromText({session.SymbolParam}1,{((GeoObject)shape).Srid})) ", shape.StAsText()) as byte[];
                case ProviderName.PostgreSql:
                    return session.ExecuteScalar($"SELECT ST_GeomFromText(@1,{((GeoObject)shape).Srid})::bytea ", shape.StAsText()) as byte[];
                case ProviderName.SqLite:
                    throw new Exception("Not support geo object");
                default:
                    throw new ArgumentOutOfRangeException($"Database type is not defined:{providerName}");
            }
            
           
            
         
        }
       


















         static ConstantExpression GetNameColumnCore<T>(Expression<Func<T, IGeoShape>> selector,
             ProviderName providerName)
        {
            var m = (selector.Body as MemberExpression);
            var nameColumn = GetColumnNameSimple<T>(m.Member.Name, providerName);
            var nameTable = GetTableName<T>(providerName);
            return Expression.Constant($"{nameTable}.{nameColumn}");
        }

        
        private static string GetColumnNameSimple<T>(string member,  ProviderName providerName)
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
