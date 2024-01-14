using ORM_1_21_.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using ORM_1_21_.Linq;
using Newtonsoft.Json.Linq;

namespace ORM_1_21_.geo
{
    partial class GeoObject : IGeoShape
    {

        public string StGeometryType(ISession session)
        {
            Check.NotNull(session, nameof(session));
            ProviderName providerName = session.ProviderName;
            string sql = null;
            switch (providerName)
            {
                case ProviderName.MsSql:
                    sql = $" select (geometry::STGeomFromText({session.SymbolParam}1, {session.SymbolParam}2)).STGeometryType()";
                    break;
                case ProviderName.MySql:
                    sql = $" select ST_GeometryType(ST_GeomFromText({session.SymbolParam}1, {session.SymbolParam}2))";
                    break;
                case ProviderName.PostgreSql:
                    sql = $" select ST_GeometryType(ST_GeomFromText({session.SymbolParam}1, {session.SymbolParam}2))";
                    break;
                case ProviderName.SqLite:
                    UtilsCore.ErrorAlert();
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Database type is not defined:{providerName}");
            }

            return (string)session.ExecuteScalar(sql, this.StAsText(), this.StSrid());
        }

        public double? StArea(ISession session)
        {
            Check.NotNull(session, nameof(session));
            ProviderName providerName = session.ProviderName;
            string sql = null;
            switch (providerName)
            {
                case ProviderName.MsSql:
                    sql = $" select (geometry::STGeomFromText({session.SymbolParam}1, {session.SymbolParam}2)).STArea()";
                    break;
                case ProviderName.MySql:
                    sql = $" select ST_Area(ST_GeomFromText({session.SymbolParam}1, {session.SymbolParam}2))";
                    break;
                case ProviderName.PostgreSql:
                    sql = $" select ST_Area(ST_GeomFromText({session.SymbolParam}1, {session.SymbolParam}2))";
                    break;
                case ProviderName.SqLite:
                    UtilsCore.ErrorAlert();
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Database type is not defined:{providerName}");
            }

            return (double?)session.ExecuteScalar(sql, this.StAsText(), this.StSrid());
        }

        public bool? StWithin(IGeoShape shape, ISession session)
        {
            return ExecuteTwoGeo<bool?>("StWithin", shape, session);
        }

        public byte[] StAsBinary(ISession session)
        {
            return ExecuteNoneGeo<byte[]>("StAsBinary", session);
        }

        public IGeoShape StBoundary(ISession session)
        {
            return ExecuteGetGeoObjectNoParam<IGeoShape>("StBoundary", session);
            
        }

        public IGeoShape StBuffer(float distance, ISession session)
        {
            return ExecuteGetGeoObjectBuffer<IGeoShape>("StBuffer", distance, session);
        }

        public IGeoShape StCentroid(ISession session)
        {
            return ExecuteGetGeoObjectNoParam<IGeoShape>("StCentroid", session);
        }

        public bool? StContains(IGeoShape shape ,ISession session)
        {
            return ExecuteTwoGeo<bool?>("StContains", shape, session);
        }

        public bool? StCrosses(IGeoShape shape, ISession session)
        {
            return ExecuteTwoGeo<bool?>("StCrosses", shape, session);
        }

        public IGeoShape StDifference(IGeoShape shape,ISession session)
        {
            return ExecuteGetGeoObjectParamGeo<IGeoShape>("StDifference", shape, session);
          
        }

        public int? StDimension(ISession session)
        {
            return ExecuteNoneGeo<int?>("StDimension", session);
        }

        public bool? StDisjoint(IGeoShape shape, ISession session)
        {
            return ExecuteTwoGeo<bool?>("StDisjoint", shape, session);
        }

        public double? StDistance(IGeoShape shape,ISession session)
        {
            return ExecuteTwoGeo<double?>("StDistance", shape, session);
        }

        public IGeoShape StEndPoint(ISession session)
        {
            return ExecuteGetGeoObjectNoParam<IGeoShape>("StEndPoint", session);
        }

        public IGeoShape StEnvelope(ISession session)
        {
            return ExecuteGetGeoObjectNoParam<IGeoShape>("StEnvelope", session);
        }

        public bool? StEquals(IGeoShape shape, ISession session)
        {
           return ExecuteTwoGeo<bool?>("StEquals", shape, session);
        }

        public bool? StIntersects(IGeoShape shape ,ISession  session)
        {
            return ExecuteTwoGeo<bool?>("StIntersects", shape, session);
        }

        public bool? StOverlaps(IGeoShape shape, ISession  session)
        {
            return ExecuteTwoGeo<bool?>("StOverlaps", shape, session);
        }

       

        public int? StSrid()
        {
            return Srid;
        }

        public IGeoShape StStartPoint(ISession session)
        {
            return ExecuteGetGeoObjectNoParam<IGeoShape>("StStartPoint", session);
        }

        public IGeoShape StSymDifference(IGeoShape shape, ISession session)
        {
            return ExecuteGetGeoObjectParamGeo<IGeoShape>("StSymDifference", shape, session);
        }

        public bool? StTouches(IGeoShape shape, ISession session)
        {
            return ExecuteTwoGeo<bool?>("StTouches", shape, session);
        }

        public int? StNumGeometries(ISession  session)
        {
            return ExecuteNoneGeo<int>("StNumGeometries", session);
        }

        public int? StNumInteriorRing(ISession session)
        {
            return ExecuteNoneGeo<int>("StNumInteriorRing", session);
        }

        public bool? StIsSimple(ISession session)
        {
            return ExecuteNoneGeo<bool?>("StIsSimple", session);
        }

        public bool? StIsValid(ISession session)
        {
            return ExecuteNoneGeo<bool?>("StIsValid", session);
        }

        public double? StLength(ISession session)
        {
            return ExecuteNoneGeo<double?>("StLength", session);
        }

        public bool? StIsClosed( ISession session)
        {
            return ExecuteNoneGeo<bool?>("StIsClosed", session);
        }

        public int? StNumPoints(ISession session)
        {
            return ExecuteNoneGeo<int?>("StNumPoints", session);
        }

        public IGeoShape StUnion(IGeoShape shape, ISession session)
        {
            return ExecuteGetGeoObjectParamGeo<IGeoShape>("StUnion", shape, session);
        }

        T ExecuteTwoGeo<T>(string name,IGeoShape shape, ISession session)
        {
            Check.NotNull(session, nameof(session));
            Check.NotNull(session, nameof(session));
            Check.NotNull(shape, nameof(shape));
            ProviderName providerName = session.ProviderName;
            string sql = null;
            name=QueryTranslator<object>.GetNameMethod(name, providerName);
            string p = session.SymbolParam;
            switch (providerName)
            {
                case ProviderName.MsSql:
                {
                    var cur = $"geometry::STGeomFromText({p}1, {p}2)";
                    var par = $"geometry::STGeomFromText({p}3, {p}4)";
                    sql = $" select ({cur}).{name}({par})";
                    break;
                }

                case ProviderName.MySql:
                {
                    var cur = $"ST_GeomFromText({p}1,  {p}2)";
                    var par = $"ST_GeomFromText({p}3,  {p}4)";
                    sql = $" select {name}({cur}, {par})";
                    break;
                }
                case ProviderName.PostgreSql:
                {
                    var cur = $"ST_GeomFromText({p}1,  {p}2)";
                    var par = $"ST_GeomFromText({p}3,  {p}4)";
                    sql = $" select {name}({cur}, {par})";
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

        T ExecuteNoneGeo<T>(string name, ISession session)
        {
            Check.NotEmpty(name, nameof(name));
            Check.NotNull(session, nameof(session));
           
            ProviderName providerName = session.ProviderName;
            name = QueryTranslator<object>.GetNameMethod(name, providerName);
            string sql = null;
            string p = session.SymbolParam;
            switch (providerName)
            {
                case ProviderName.MsSql:
                    sql = $" select (geometry::STGeomFromText({p}1, {p}2)).{name}()";
                    break;
                case ProviderName.MySql:
                    sql = $" select {name}(ST_GeomFromText({p}1, {p}2))";
                    break;
                case ProviderName.PostgreSql:
                    sql = $" select {name}(ST_GeomFromText({p}1, {p}2))";
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

        T ExecuteGetGeoObjectNoParam<T>(string name, ISession session) where T:IGeoShape
        {
            Check.NotNull(session, nameof(session));
            Check.NotNull(name, nameof(name));
            ProviderName providerName = session.ProviderName;
            string sql = null;
            string p = session.SymbolParam;
            name = QueryTranslator<object>.GetNameMethod(name, providerName);
            switch (providerName)
            {
                case ProviderName.MsSql:
                    sql = $"(geometry::STGeomFromText({p}1, {this.StSrid()})).{name}()";
                    sql = $" select CONCAT('SRID={this.StSrid()}',';',{sql}.STAsText())";
                    break;
                case ProviderName.MySql:
                    sql = $"{name}(ST_GeomFromText({p}1, {this.StSrid()}))";
                    sql = $" select CONCAT('SRID={this.StSrid()}',';',ST_AsText({sql}))";
                    break;
                case ProviderName.PostgreSql:
                    sql = $"{name}(ST_GeomFromText({p}1, {this.StSrid()}))";
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
                return (T)FactoryGeo.CreateGeo(res);
            }
            catch (GeoException)
            {
                return default(T);
            }
            
        }

        T ExecuteGetGeoObjectParamGeo<T>(string name,IGeoShape shape, ISession session) where T : IGeoShape
        {
            Check.NotNull(session, nameof(session));
            Check.NotEmpty(name, nameof(name));
            ProviderName providerName = session.ProviderName;
            string sql = null;
            string p = session.SymbolParam;
            name = QueryTranslator<object>.GetNameMethod(name, providerName);
            var srid = this.StSrid();
            switch (providerName)
            {
                case ProviderName.MsSql:
                    sql = $"geometry::STGeomFromText({p}1, {srid}).{name}(geometry::STGeomFromText({p}2, {srid}))";
                    sql = $" select CONCAT('SRID={srid}',';',{sql}.STAsText())";
                    break;
                case ProviderName.MySql:
                    sql = $"{name}(ST_GeomFromText({p}1, {srid}),ST_GeomFromText({p}2, {srid}))";
                    sql = $" select CONCAT('SRID={srid}',';',ST_AsText({sql}))";
                    break;
                case ProviderName.PostgreSql:
                    sql = $"{name}(ST_GeomFromText({p}1, {srid}),ST_GeomFromText({p}2, {srid}))";
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

        T ExecuteGetGeoObjectBuffer<T>(string name, object par, ISession session) where T : IGeoShape
        {
            Check.NotNull(session, nameof(session));
            Check.NotEmpty(name, nameof(name));
            ProviderName providerName = session.ProviderName;
            string sql = null;
            string p = session.SymbolParam;
            name = QueryTranslator<object>.GetNameMethod(name, providerName);
            var srid = this.StSrid();
            switch (providerName)
            {
                case ProviderName.MsSql:
                    sql = $"geometry::STGeomFromText({p}1, {srid}).{name}({par})";
                    sql = $" select CONCAT('SRID={srid}',';',{sql}.STAsText())";
                    break;
                case ProviderName.MySql:
                    sql = $"{name}(ST_GeomFromText({p}1, {srid}),{par})";
                    sql = $" select CONCAT('SRID={srid}',';',ST_AsText({sql}))";
                    break;
                case ProviderName.PostgreSql:
                    sql = $"{name}(ST_GeomFromText({p}1, {srid}),{par})";
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






    }
}
