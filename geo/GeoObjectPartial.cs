using ORM_1_21_.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using ORM_1_21_.Linq;
using Newtonsoft.Json.Linq;
using static System.Collections.Specialized.BitVector32;
using System.Reflection;

namespace ORM_1_21_.geo
{
    partial class GeoObject : IGeoShape
    {

        public string StGeometryType()
        {
            ISession session = _session;
            return ExecuteNoneGeo<string>("StGeometryType", session);
        }

        public double? StArea()
        {
            ISession session = _session;
          
            return ExecuteNoneGeo<double?>("StArea", session);
        }

        public bool? StWithin(IGeoShape shape)
        {
            ISession session = _session;
            return ExecuteTwoGeo<bool?>("StWithin", shape, session);
        }

        public byte[] StAsBinary()
        {
            ISession session = _session;
            return ExecuteNoneGeo<byte[]>("StAsBinary", session);
        }

        public IGeoShape StBoundary()
        {
            ISession session = _session;
            return ExecuteGetGeoObjectNoParam<IGeoShape>("StBoundary", session);
            
        }

        public IGeoShape StBuffer(float distance)
        {
            ISession session = _session;
            return ExecuteGetGeoObjectBufferE<IGeoShape>("StBuffer",  session,distance);
        }

        public IGeoShape StCentroid()
        {
            ISession session = _session;
            return ExecuteGetGeoObjectNoParam<IGeoShape>("StCentroid", session);
        }

        public bool? StContains(IGeoShape shape)
        {
            ISession session = _session;
            return ExecuteTwoGeo<bool?>("StContains", shape, session);
        }

        public bool? StCrosses(IGeoShape shape)
        {
            ISession session = _session;
            return ExecuteTwoGeo<bool?>("StCrosses", shape, session);
        }

        public IGeoShape StDifference(IGeoShape shape)
        {
            ISession session = _session;
            return ExecuteGetGeoObjectParamGeo<IGeoShape>("StDifference", shape, session);
          
        }

        public int? StDimension()
        {
            ISession session = _session;
            return ExecuteNoneGeo<int?>("StDimension", session);
        }

        public bool? StDisjoint(IGeoShape shape)
        {
            ISession session = _session;
            return ExecuteTwoGeo<bool?>("StDisjoint", shape, session);
        }

        public double? StDistance(IGeoShape shape)
        {
            ISession session = _session;
            return ExecuteTwoGeo<double?>("StDistance", shape, session);
        }

        public IGeoShape StEndPoint()
        {
            ISession session = _session;
            return ExecuteGetGeoObjectNoParam<IGeoShape>("StEndPoint", session);
        }

        public IGeoShape StEnvelope()
        {
            ISession session = _session;
            return ExecuteGetGeoObjectNoParam<IGeoShape>("StEnvelope", session);
        }

        public bool? StEquals(IGeoShape shape)
        {
            ISession session = _session;
            return ExecuteTwoGeo<bool?>("StEquals", shape, session);
        }

        public bool? StIntersects(IGeoShape shape)
        {
            ISession session = _session;
            return ExecuteTwoGeo<bool?>("StIntersects", shape, session);
        }

        public bool? StOverlaps(IGeoShape shape)
        {
            ISession session = _session;
            return ExecuteTwoGeo<bool?>("StOverlaps", shape, session);
        }

        public int? StSrid()
        {
            return Srid;
        }

        public IGeoShape StStartPoint()
        {
            ISession session = _session;
            return ExecuteGetGeoObjectNoParam<IGeoShape>("StStartPoint", session);
        }

        public IGeoShape StSymDifference(IGeoShape shape)
        {
            ISession session = _session;
            return ExecuteGetGeoObjectParamGeo<IGeoShape>("StSymDifference", shape, session);
        }

        public bool? StTouches(IGeoShape shape)
        {
            ISession session = _session;
            return ExecuteTwoGeo<bool?>("StTouches", shape, session);
        }

        public int? StNumGeometries()
        {
            ISession session = _session;
            return ExecuteNoneGeo<int>("StNumGeometries", session);
        }

        public int? StNumInteriorRing()
        {
            ISession session = _session;
            return ExecuteNoneGeo<int>("StNumInteriorRing", session);
        }

        public bool? StIsSimple()
        {
            ISession session = _session;
            return ExecuteNoneGeo<bool?>("StIsSimple", session);
        }

        public bool? StIsValid()
        {
            ISession session = _session;
            return ExecuteNoneGeo<bool?>("StIsValid", session);
        }

        public double? StLength()
        {
            ISession session = _session;
            return ExecuteNoneGeo<double?>("StLength", session);
        }

        public bool? StIsClosed()
        {
            ISession session = _session;
            return ExecuteNoneGeo<bool?>("StIsClosed", session);
        }

        public int? StNumPoints()
        {
            ISession session = _session;
            return ExecuteNoneGeo<int?>("StNumPoints", session);
        }

        public double? StPerimeter()
        {
            ISession session = _session;
            return ExecuteNoneGeo<double?>("StPerimeter", session);
        }

        public IGeoShape StTranslate(float deltaX, float deltaY)
        {
            ISession session = _session;
            return ExecuteGetGeoObjectBufferE<IGeoShape>("StTranslate", session, deltaX, deltaY);
        }

        public IGeoShape StConvexHull()
        {
            ISession session = _session;
            return ExecuteGetGeoObjectNoParam<IGeoShape>("StConvexHull",session );
        }

        public IGeoShape StCollect(params IGeoShape[] shapes)
        {
            throw new NotImplementedException();
        }

        public IGeoShape StPointN(int n)
        {
            ISession session = _session;
           
            return ExecuteGetGeoObjectBufferE<IGeoShape>("StPointN", session, n);
        }

        public IGeoShape StPointOnSurface()
        {
            ISession session = _session;
            return ExecuteGetGeoObjectNoParam<IGeoShape>("StPointOnSurface", session);
        }

        public IGeoShape StInteriorRingN(int n)
        {
            ISession session = _session;

            return ExecuteGetGeoObjectBufferE<IGeoShape>("StInteriorRingN", session, n);
        }

        public double? StX()
        {
            if (GeoType != GeoType.Point)
            {
                throw new Exception("Input must be a point.");
            }
            ISession session = _session;
            return ExecuteNoneGeo<double?>("StX", session);
        }

        public double? StY()
        {
            if (GeoType != GeoType.Point)
            {
                throw new Exception("Input must be a point.");
            }
            ISession session = _session;
            return ExecuteNoneGeo<double?>("StY", session);
        }

        public IGeoShape StTransform(int srid)
        {
            ISession session = _session;

            return ExecuteGetGeoObjectBufferE<IGeoShape>("StTransform", session, srid);
        }

        public IGeoShape StSetSRID(int srid)
        {
            SetSrid(srid);
            return this;
        }

        public string StAsLatLonText(string format = "''")
        {
            ISession session = _session;
            if (session.ProviderName != ProviderName.PostgreSql)
            {
                throw new Exception("Only for Postgres");
            }

            if (GeoType != GeoType.Point)
            {
                throw new Exception("Only for point");
            }
            string sql =  $" select ST_AsLatLonText(ST_GeomFromText(@1, {StSrid()}), @2)";
            var res = (string)session.ExecuteScalar(sql, new SqlParam($"@1", this.StAsText()),new SqlParam("@2",format));
            return res;

        }

        public IGeoShape StReverse()
        {
            ISession session = _session;
            return ExecuteGetGeoObjectNoParam<IGeoShape>("StReverse", session);
        }


        public IGeoShape StUnion(IGeoShape shape)
        {
            ISession session = _session;
            return ExecuteGetGeoObjectParamGeo<IGeoShape>("StUnion", shape, session);
        }

        T ExecuteTwoGeo<T>(string methodName,IGeoShape shape, ISession session)
        {
            Check.NotNull(session, nameof(session));
            Check.NotNull(session, nameof(session));
            Check.NotNull(shape, nameof(shape));
            ProviderName providerName = session.ProviderName;
            string sql = null;
            methodName=QueryTranslator<object>.GetNameMethod(methodName, providerName);
            string p = session.SymbolParam;
            switch (providerName)
            {
                case ProviderName.MsSql:
                {
                    var cur = $"geometry::STGeomFromText({p}1, {p}2)";
                    var par = $"geometry::STGeomFromText({p}3, {p}4)";
                    sql = $" select ({cur}).{methodName}({par})";
                    break;
                }

                case ProviderName.MySql:
                {
                    var cur = $"ST_GeomFromText({p}1,  {p}2)";
                    var par = $"ST_GeomFromText({p}3,  {p}4)";
                    sql = $" select {methodName}({cur}, {par})";
                    break;
                }
                case ProviderName.PostgreSql:
                {
                    var cur = $"ST_GeomFromText({p}1,  {p}2)";
                    var par = $"ST_GeomFromText({p}3,  {p}4)";
                    sql = $" select {methodName}({cur}, {par})";
                    break;
                }
                case ProviderName.SqLite:
                    UtilsCore.ErrorAlert();
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Database type is not defined:{providerName}");
            }

            var res = session.ExecuteScalar(sql,
                new SqlParam($"{p}1", this.StAsText()),
                new SqlParam($"{p}2", this.StSrid()),
                new SqlParam($"{p}3", shape.StAsText()),
                new SqlParam($"{p}4", shape.StSrid()));

            return (T)UtilsCore.Convertor(res, typeof(T));

        }

        T ExecuteNoneGeo<T>(string methodName, ISession session)
        {
            Check.NotEmpty(methodName, nameof(methodName));
            Check.NotNull(session, nameof(session));
           
            ProviderName providerName = session.ProviderName;
            methodName = QueryTranslator<object>.GetNameMethod(methodName, providerName);
            string sql = null;
            string p = session.SymbolParam;
            switch (providerName)
            {
                case ProviderName.MsSql:
                    if (methodName == "STX")
                    {
                        sql = $" select (geometry::STGeomFromText({p}1, {p}2)).STX";
                    }
                    else if (methodName == "STY")
                    {
                        sql = $" select (geometry::STGeomFromText({p}1, {p}2)).STY";
                    }
                    else
                    {
                        sql = $" select (geometry::STGeomFromText({p}1, {p}2)).{methodName}()";
                    }
                       
                    break;
                case ProviderName.MySql:
                    sql = $" select {methodName}(ST_GeomFromText({p}1, {p}2))";
                    break;
                case ProviderName.PostgreSql:
                    sql = $" select {methodName}(ST_GeomFromText({p}1, {p}2))";
                    break;
                case ProviderName.SqLite:
                    UtilsCore.ErrorAlert();
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Database type is not defined:{providerName}");
            }

            var res=session.ExecuteScalar(sql,
                new SqlParam($"{p}1",this.StAsText()), 
                new SqlParam($"{p}2",this.StSrid()));
            return (T)UtilsCore.Convertor(res, typeof(T));

        }

        T ExecuteGetGeoObjectNoParam<T>(string methodName, ISession session) where T:IGeoShape
        {
            Check.NotNull(session, nameof(session));
            Check.NotNull(methodName, nameof(methodName));
            ProviderName providerName = session.ProviderName;
            string sql = null;
            string p = session.SymbolParam;
            methodName = QueryTranslator<object>.GetNameMethod(methodName, providerName);
            switch (providerName)
            {
                case ProviderName.MsSql:
                    sql = $"(geometry::STGeomFromText({p}1, {this.StSrid()})).{methodName}()";
                    sql = $" select CONCAT('SRID={this.StSrid()}',';',{sql}.STAsText())";
                    break;
                case ProviderName.MySql:
                    sql = $"{methodName}(ST_GeomFromText({p}1, {this.StSrid()}))";
                    sql = $" select CONCAT('SRID={this.StSrid()}',';',ST_AsText({sql}))";
                    break;
                case ProviderName.PostgreSql:
                    sql = $"{methodName}(ST_GeomFromText({p}1, {this.StSrid()}))";
                    sql = $" select CONCAT('SRID={this.StSrid()}',';',ST_AsText({sql}))";
                    break;
                case ProviderName.SqLite:
                    UtilsCore.ErrorAlert();
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Database type is not defined:{providerName}");
            }

            var res =(string) session.ExecuteScalar(sql, this.StAsText());
            var str= UtilsCore.Convertor(res, typeof(string));
            if (str==null)
            {
                return default(T);
            }

            try
            {
                return (T)FactoryGeo.CreateGeo(res).SetSession(session);
            }
            catch (GeoException)
            {
                return default(T);
            }
            
        }

        T ExecuteGetGeoObjectParamGeo<T>(string methodName,IGeoShape shape, ISession session) where T : IGeoShape
        {
            Check.NotNull(session, nameof(session));
            Check.NotEmpty(methodName, nameof(methodName));
            ProviderName providerName = session.ProviderName;
            string sql = null;
            string p = session.SymbolParam;
            methodName = QueryTranslator<object>.GetNameMethod(methodName, providerName);
            var srid = this.StSrid();
            switch (providerName)
            {
                case ProviderName.MsSql:
                    sql = $"geometry::STGeomFromText({p}1, {srid}).{methodName}(geometry::STGeomFromText({p}2, {srid}))";
                    sql = $" select CONCAT('SRID={srid}',';',{sql}.STAsText())";
                    break;
                case ProviderName.MySql:
                    sql = $"{methodName}(ST_GeomFromText({p}1, {srid}),ST_GeomFromText({p}2, {srid}))";
                    sql = $" select CONCAT('SRID={srid}',';',ST_AsText({sql}))";
                    break;
                case ProviderName.PostgreSql:
                    sql = $"{methodName}(ST_GeomFromText({p}1, {srid}),ST_GeomFromText({p}2, {srid}))";
                    sql = $" select CONCAT('SRID={srid}',';',ST_AsText({sql}))";
                    break;
                case ProviderName.SqLite:
                    UtilsCore.ErrorAlert();
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Database type is not defined:{providerName}");
            }

            var res = (string)session.ExecuteScalar(sql, new SqlParam($"{p}1",this.StAsText()),new SqlParam($"{p}2",shape.StAsText()));
            if (typeof(T) == typeof(IGeoShape))
            {
                var str = UtilsCore.Convertor(res, typeof(string));
                if (str == null)
                {
                    return default;
                }

                try
                {
                    return (T)FactoryGeo.CreateGeo(res).SetSession(session);
                }
                catch (GeoException)
                {
                    return default;
                }
            }
            else
            {
                var str = UtilsCore.Convertor(res, typeof(T));
                return (T)str;
            }
           

        }

        T ExecuteGetGeoObjectBuffer<T>(string methodName, object par, ISession session) where T : IGeoShape
        {
            Check.NotNull(session, nameof(session));
            Check.NotEmpty(methodName, nameof(methodName));
            ProviderName providerName = session.ProviderName;
            string sql = null;
            string p = session.SymbolParam;
            methodName = QueryTranslator<object>.GetNameMethod(methodName, providerName);
            var srid = this.StSrid();
            switch (providerName)
            {
                case ProviderName.MsSql:
                    sql = $"geometry::STGeomFromText({p}1, {srid}).{methodName}({par})";
                    sql = $" select CONCAT('SRID={srid}',';',{sql}.STAsText())";
                    break;
                case ProviderName.MySql:
                    sql = $"{methodName}(ST_GeomFromText({p}1, {srid}),{par})";
                    sql = $" select CONCAT('SRID={srid}',';',ST_AsText({sql}))";
                    break;
                case ProviderName.PostgreSql:
                    sql = $"{methodName}(ST_GeomFromText({p}1, {srid}),{par})";
                    sql = $" select CONCAT('SRID={srid}',';',ST_AsText({sql}))";
                    break;
                case ProviderName.SqLite:
                    UtilsCore.ErrorAlert();
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Database type is not defined:{providerName}");
            }

            var res = (string)session.ExecuteScalar(sql, new SqlParam($"{p}1", this.StAsText()));
            if (typeof(T) == typeof(IGeoShape))
            {
                var str = UtilsCore.Convertor(res, typeof(string));
                if (str == null)
                {
                    return default;
                }

                try
                {
                    return (T)FactoryGeo.CreateGeo(res);
                }
                catch (GeoException)
                {
                    return default;
                }
            }
            else
            {
                var str = UtilsCore.Convertor(res, typeof(T));
                return (T)str;
            }


        }

        T ExecuteGetGeoObjectBufferE<T>(string methodName, ISession session,params object[] par ) where T : IGeoShape
        {
            for (var i = 0; i < par.Length; i++)
            {
                if (par[i] == null) throw new Exception("Parameters can not be empty");
                if (par[i] is string)
                {
                    par[i] = $"'{par[i]}'";
                }
                if (par[i] is double)
                {
                    par[i] = $"'{((double)par[i]).ToString(CultureInfo.InvariantCulture)}'";
                }
                if (par[i] is float)
                {
                    par[i] = $"'{((float)par[i]).ToString(CultureInfo.InvariantCulture)}'";
                }
                else
                {
                    par[i] = par[i].ToString();
                }
            }
            string paramCore=String.Join(",",par);
           
            Check.NotNull(session, nameof(session));
            Check.NotEmpty(methodName, nameof(methodName));
            ProviderName providerName = session.ProviderName;
            string sql = null;
            string p = session.SymbolParam;
            methodName = QueryTranslator<object>.GetNameMethod(methodName, providerName);
            var srid = this.StSrid();
            switch (providerName)
            {
                case ProviderName.MsSql:
                    sql = $"geometry::STGeomFromText({p}1, {srid}).{methodName}({paramCore})";
                    sql = $" select CONCAT('SRID={srid}',';',{sql}.STAsText())";
                    break;
                case ProviderName.MySql:
                    sql = $"{methodName}(ST_GeomFromText({p}1, {srid}),{paramCore})";
                    sql = $" select CONCAT('SRID={srid}',';',ST_AsText({sql}))";
                    break;
                case ProviderName.PostgreSql:
                    sql = $"{methodName}(ST_GeomFromText({p}1, {srid}),{paramCore})";
                    sql = $" select CONCAT('SRID={srid}',';',ST_AsText({sql}))";
                    break;
                case ProviderName.SqLite:
                    UtilsCore.ErrorAlert();
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Database type is not defined:{providerName}");
            }

            var res = (string)session.ExecuteScalar(sql, new SqlParam($"{p}1", this.StAsText()));
            if (typeof(T) == typeof(IGeoShape))
            {
                var str = UtilsCore.Convertor(res, typeof(string));
                if (str == null)
                {
                    return default;
                }

                try
                {
                    return (T)FactoryGeo.CreateGeo(res).SetSession(session);
                }
                catch (GeoException)
                {
                    return default;
                }
            }
            else
            {
                var str = UtilsCore.Convertor(res, typeof(T));
                return (T)str;
            }


        }






    }
}
