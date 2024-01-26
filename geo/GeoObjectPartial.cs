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
using System.Threading;
using System.Threading.Tasks;

namespace ORM_1_21_.geo
{
    partial class GeoObject : IGeoShape
    {

        public string StGeometryType()
        {
            ISession session = _session;
            return ExecuteNoneGeo<string>("StGeometryType", session);
        }

        public Task<string> StGeometryTypeAsync(CancellationToken cancellationToken = default)
        {
            ISession session = _session;
            return ExecuteNoneGeoAsync<string>("StGeometryType", session,cancellationToken);
        }

        public double? StArea()
        {
            ISession session = _session;
          
            return ExecuteNoneGeo<double?>("StArea", session);
        }

        public Task<double?> StAreaAsync(CancellationToken cancellationToken = default)
        {
            ISession session = _session;

            return ExecuteNoneGeoAsync<double?>("StArea", session,cancellationToken);
        }

        public bool? StWithin(IGeoShape shape)
        {
            ISession session = _session;
            return ExecuteTwoGeo<bool?>("StWithin", shape, session);
        }

        public Task<bool?> StWithinAsync(IGeoShape shape,CancellationToken cancellationToken=default)
        {
            ISession session = _session;
            return ExecuteTwoGeoAsync<bool?>("StWithin", shape, session,cancellationToken);
        }

        public byte[] StAsBinary()
        {
            ISession session = _session;
            return ExecuteNoneGeo<byte[]>("StAsBinary", session);
        }

        public Task<byte[]> StAsBinaryAsync(CancellationToken cancellationToken = default)
        {
            ISession session = _session;
            return ExecuteNoneGeoAsync<byte[]>("StAsBinary", session,cancellationToken);
        }

        public IGeoShape StBoundary()
        {
            ISession session = _session;
            return ExecuteGetGeoObjectNoParam<IGeoShape>("StBoundary", session);
            
        }

        public Task<IGeoShape> StBoundaryAsync(CancellationToken cancellationToken = default)
        {
            ISession session = _session;
            return ExecuteGetGeoObjectNoParamAsync<IGeoShape>("StBoundary", session,cancellationToken);

        }

        public IGeoShape StBuffer(float distance)
        {
            ISession session = _session;
            return ExecuteGetGeoObjectBufferE<IGeoShape>("StBuffer",  session,distance);
        }


        public Task<IGeoShape> StBufferAsync(float distance, CancellationToken cancellationToken = default)
        {
            ISession session = _session;
            return ExecuteGetGeoObjectBufferEAsync<IGeoShape>("StBuffer", session, new object[]{distance},cancellationToken);
        }

        public IGeoShape StCentroid()
        {
            ISession session = _session;
            return ExecuteGetGeoObjectNoParam<IGeoShape>("StCentroid", session);
        }


        public Task<IGeoShape> StCentroidAsync(CancellationToken cancellationToken = default)
        {
            ISession session = _session;
            return ExecuteGetGeoObjectNoParamAsync<IGeoShape>("StCentroid", session,cancellationToken);
        }

        public bool? StContains(IGeoShape shape)
        {
            ISession session = _session;
            return ExecuteTwoGeo<bool?>("StContains", shape, session);
        }


        public Task<bool?> StContainsAsync(IGeoShape shape, CancellationToken cancellationToken)
        {
            ISession session = _session;
            return ExecuteTwoGeoAsync<bool?>("StContains", shape, session,cancellationToken);
        }

        public bool? StCrosses(IGeoShape shape)
        {
            ISession session = _session;
            return ExecuteTwoGeo<bool?>("StCrosses", shape, session);
        }


        public Task<bool?> StCrossesAsync(IGeoShape shape,CancellationToken cancellationToken=default)
        {
            ISession session = _session;
            return ExecuteTwoGeoAsync<bool?>("StCrosses", shape, session,cancellationToken);
        }

        public IGeoShape StDifference(IGeoShape shape)
        {
            ISession session = _session;
            return ExecuteGetGeoObjectParamGeo<IGeoShape>("StDifference", shape, session);
          
        }

        public Task<IGeoShape> StDifferenceAsync(IGeoShape shape, CancellationToken cancellationToken=default)
        {
            ISession session = _session;
            return ExecuteGetGeoObjectParamGeoAsync<IGeoShape>("StDifference", shape, session,cancellationToken);

        }

        public int? StDimension()
        {
            ISession session = _session;
            return ExecuteNoneGeo<int?>("StDimension", session);
        }

        public Task<int?> StDimensionAsync(CancellationToken cancellationToken=default)
        {
            ISession session = _session;
            return ExecuteNoneGeoAsync<int?>("StDimension", session,cancellationToken);
        }

        public bool? StDisjoint(IGeoShape shape)
        {
            ISession session = _session;
            return ExecuteTwoGeo<bool?>("StDisjoint", shape, session);
        }

        public Task<bool?> StDisjointAsync(IGeoShape shape,CancellationToken cancellationToken=default)
        {
            ISession session = _session;
            return ExecuteTwoGeoAsync<bool?>("StDisjoint", shape, session,cancellationToken);
        }

        public double? StDistance(IGeoShape shape)
        {
            ISession session = _session;
            return ExecuteTwoGeo<double?>("StDistance", shape, session);
        }

        public Task<double?> StDistanceAsync(IGeoShape shape, CancellationToken cancellationToken=default)
        {
            ISession session = _session;
            return ExecuteTwoGeoAsync<double?>("StDistance", shape, session,cancellationToken);
        }

        public IGeoShape StEndPoint()
        {
            ISession session = _session;
            return ExecuteGetGeoObjectNoParam<IGeoShape>("StEndPoint", session);
        }

        public Task<IGeoShape> StEndPointAsync(CancellationToken cancellationToken = default)
        {
            ISession session = _session;
            return ExecuteGetGeoObjectNoParamAsync<IGeoShape>("StEndPoint", session,cancellationToken);
        }

        public IGeoShape StEnvelope()
        {
            ISession session = _session;
            return ExecuteGetGeoObjectNoParam<IGeoShape>("StEnvelope", session);
        }

        public Task<IGeoShape> StEnvelopeAsync(CancellationToken cancellationToken = default)
        {
            ISession session = _session;
            return ExecuteGetGeoObjectNoParamAsync<IGeoShape>("StEnvelope", session,cancellationToken);
        }


        public bool? StEquals(IGeoShape shape)
        {
            ISession session = _session;
            return ExecuteTwoGeo<bool?>("StEquals", shape, session);
        }

        public Task<bool?> StEqualsAsync(IGeoShape shape,CancellationToken cancellationToken=default)
        {
            ISession session = _session;
            return ExecuteTwoGeoAsync<bool?>("StEquals", shape, session,cancellationToken);
        }

        public bool? StIntersects(IGeoShape shape)
        {
            ISession session = _session;
            return ExecuteTwoGeo<bool?>("StIntersects", shape, session);
        }

        public Task<bool?> StIntersectsAsync(IGeoShape shape,CancellationToken cancellationToken=default)
        {
            ISession session = _session;
            return ExecuteTwoGeoAsync<bool?>("StIntersects", shape, session,cancellationToken);
        }

        public bool? StOverlaps(IGeoShape shape)
        {
            ISession session = _session;
            return ExecuteTwoGeo<bool?>("StOverlaps", shape, session);
        }


        public Task<bool?> StOverlapsAsync(IGeoShape shape,CancellationToken cancellationToken=default)
        {
            ISession session = _session;
            return ExecuteTwoGeoAsync<bool?>("StOverlaps", shape, session,cancellationToken);
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

        public Task<IGeoShape> StStartPointAsync(CancellationToken cancellationToken = default)
        {
            ISession session = _session;
            return ExecuteGetGeoObjectNoParamAsync<IGeoShape>("StStartPoint", session,cancellationToken);
        }

        public IGeoShape StSymDifference(IGeoShape shape)
        {
            ISession session = _session;
            return ExecuteGetGeoObjectParamGeo<IGeoShape>("StSymDifference", shape, session);
        }

        public Task<IGeoShape> StSymDifferenceAsync(IGeoShape shape, CancellationToken cancellationToken = default)
        {
            ISession session = _session;
            return ExecuteGetGeoObjectParamGeoAsync<IGeoShape>("StSymDifference", shape, session,cancellationToken);
        }

        public bool? StTouches(IGeoShape shape)
        {
            ISession session = _session;
            return ExecuteTwoGeo<bool?>("StTouches", shape, session);
        }


        public Task<bool?> StTouchesAsync(IGeoShape shape,CancellationToken cancellationToken=default)
        {
            ISession session = _session;
            return ExecuteTwoGeoAsync<bool?>("StTouches", shape, session,cancellationToken);
        }

        public bool? StToucheAsync(IGeoShape shape)
        {
            ISession session = _session;
            return ExecuteTwoGeo<bool?>("StTouches", shape, session);
        }

        public int? StNumGeometries()
        {
            ISession session = _session;
            return ExecuteNoneGeo<int>("StNumGeometries", session);
        }

        public Task<int?> StNumGeometriesAsync(CancellationToken cancellationToken=default)
        {
            ISession session = _session;
            return ExecuteNoneGeoAsync<int?>("StNumGeometries", session,cancellationToken);
        }

        public int? StNumInteriorRing()
        {
            ISession session = _session;
            return ExecuteNoneGeo<int>("StNumInteriorRing", session);
        }

        public Task<int?> StNumInteriorRingAsync(CancellationToken cancellationToken=default)
        {
            ISession session = _session;
            return ExecuteNoneGeoAsync<int?>("StNumInteriorRing", session,cancellationToken);
        }

        public bool? StIsSimple()
        {
            ISession session = _session;
            return ExecuteNoneGeo<bool?>("StIsSimple", session);
        }

        public Task<bool?> StIsSimpleAsync(CancellationToken cancellationToken=default)
        {
            ISession session = _session;
            return ExecuteNoneGeoAsync<bool?>("StIsSimple", session,cancellationToken);
        }

        public bool? StIsValid()
        {
            ISession session = _session;
            return ExecuteNoneGeo<bool?>("StIsValid", session);
        }

        public Task<bool?> StIsValidAsync(CancellationToken cancellationToken=default)
        {
            ISession session = _session;
            return ExecuteNoneGeoAsync<bool?>("StIsValid", session,cancellationToken);
        }

        public double? StLength()
        {
            ISession session = _session;
            return ExecuteNoneGeo<double?>("StLength", session);
        }

        public Task<double?> StLengthAsync(CancellationToken cancellationToken=default)
        {
            ISession session = _session;
            return ExecuteNoneGeoAsync<double?>("StLength", session,cancellationToken);
        }

        public bool? StIsClosed()
        {
            ISession session = _session;
            return ExecuteNoneGeo<bool?>("StIsClosed", session);
        }

        public Task<bool?> StIsClosedAsync(CancellationToken cancellationToken=default)
        {
            ISession session = _session;
            return ExecuteNoneGeoAsync<bool?>("StIsClosed", session,cancellationToken);
        }

        public int? StNumPoints()
        {
            ISession session = _session;
            return ExecuteNoneGeo<int?>("StNumPoints", session);
        }

        public Task<int?> StNumPointsAsync(CancellationToken cancellationToken=default)
        {
            ISession session = _session;
            return ExecuteNoneGeoAsync<int?>("StNumPoints", session,cancellationToken);
        }

        public double? StPerimeter()
        {
            ISession session = _session;
            return ExecuteNoneGeo<double?>("StPerimeter", session);
        }

        public Task<double?> StPerimeterAsync(CancellationToken cancellationToken=default)
        {
            ISession session = _session;
            return ExecuteNoneGeoAsync<double?>("StPerimeter", session,cancellationToken);
        }

        public IGeoShape StTranslate(float deltaX, float deltaY)
        {
            ISession session = _session;
            return ExecuteGetGeoObjectBufferE<IGeoShape>("StTranslate", session, deltaX, deltaY);
        }

        public Task<IGeoShape> StTranslateAsync(float deltaX, float deltaY,CancellationToken cancellationToken=default)
        {
            ISession session = _session;
            return ExecuteGetGeoObjectBufferEAsync<IGeoShape>("StTranslate", session, new object[]{deltaX, deltaY},cancellationToken );
        }

        public IGeoShape StConvexHull()
        {
            ISession session = _session;
            return ExecuteGetGeoObjectNoParam<IGeoShape>("StConvexHull",session );
        }

        public Task<IGeoShape> StConvexHullAsync(CancellationToken cancellationToken=default)
        {
            ISession session = _session;
            return ExecuteGetGeoObjectNoParamAsync<IGeoShape>("StConvexHull", session,cancellationToken);
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

        public Task<IGeoShape> StPointNAsync(int n,CancellationToken cancellationToken=default)
        {
            ISession session = _session;

            return ExecuteGetGeoObjectBufferEAsync<IGeoShape>("StPointN", session, new object[]{n},cancellationToken);
        }

        public IGeoShape StPointOnSurface()
        {
            ISession session = _session;
            return ExecuteGetGeoObjectNoParam<IGeoShape>("StPointOnSurface", session);
        }

        public Task<IGeoShape> StPointOnSurfaceAsync(CancellationToken cancellationToken=default)
        {
            ISession session = _session;
            return ExecuteGetGeoObjectNoParamAsync<IGeoShape>("StPointOnSurface", session,cancellationToken);
        }

        public IGeoShape StInteriorRingN(int n)
        {
            ISession session = _session;

            return ExecuteGetGeoObjectBufferE<IGeoShape>("StInteriorRingN", session, n);
        }

        public Task<IGeoShape> StInteriorRingNAsync(int n,CancellationToken cancellationToken=default)
        {
            ISession session = _session;

            return ExecuteGetGeoObjectBufferEAsync<IGeoShape>("StInteriorRingN", session, new object[]{n},cancellationToken);
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

        public Task<double?> StXAsync(CancellationToken cancellationToken=default)
        {
            if (GeoType != GeoType.Point)
            {
                throw new Exception("Input must be a point.");
            }
            ISession session = _session;
            return ExecuteNoneGeoAsync<double?>("StX", session,cancellationToken);
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

        public Task<double?> StYAsync(CancellationToken cancellationToken)
        {
            if (GeoType != GeoType.Point)
            {
                throw new Exception("Input must be a point.");
            }
            ISession session = _session;
            return ExecuteNoneGeoAsync<double?>("StY", session,cancellationToken);
        }

        public IGeoShape StTransform(int srid)
        {
            ISession session = _session;

            return ExecuteGetGeoObjectBufferE<IGeoShape>("StTransform", session, srid);
        }

        public Task<IGeoShape> StTransformAsync(int srid,CancellationToken cancellationToken=default)
        {
            ISession session = _session;

            return ExecuteGetGeoObjectBufferEAsync<IGeoShape>("StTransform", session,new object[] {srid},cancellationToken);
        }

        public IGeoShape StSetSRID(int srid)
        {
            SetSrid(srid);
            return this;
            // ISession session = _session;
            // return ExecuteGetGeoObjectBufferE<IGeoShape>("StSetSRID", session, srid);

        }

        public string StAsLatLonText(string format =null)
        {
            
            CheckSession(_session, "StAsLatLonText");

            if (string.IsNullOrWhiteSpace(format))
            {
                format = "D°M''S.SSS\"C";
            }
            
            if (_session.ProviderName != ProviderName.PostgreSql)
            {
                throw new Exception("Only for Postgres");
            }

            if (GeoType != GeoType.Point)
            {
                throw new Exception("Only for point");
            }

            try
            {
                string sql = $" select ST_AsLatLonText(ST_GeomFromText(@1, {StSrid()}), @2)";
                var res = (string)_session.ExecuteScalar(sql, new SqlParam($"@1", this.StAsText()),
                    new SqlParam("@2", format));
                return res;
            }
            finally
            {
                {
                    _session = null;
                }
            }
          

        }

        public Task<object> StAsLatLonTextAsync(string format, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(format))
            {
                format = "D°M''S.SSS\"C";
            }
            CheckSession(_session, "StAsLatLonTextAsync");
            if (_session.ProviderName != ProviderName.PostgreSql)
            {
                throw new Exception("Only for Postgres");
            }

            if (GeoType != GeoType.Point)
            {
                throw new Exception("Only for point");
            }

            try
            {
                string sql = $" select ST_AsLatLonText(ST_GeomFromText(@1, {StSrid()}), @2)";
                var res = _session.ExecuteScalarAsync(sql, new object[]
                {
                    new SqlParam($"@1", this.StAsText()), new SqlParam("@2", format)

                }, cancellationToken);
                return res;
            }
            finally
            {
                _session = null;
            }

        }

        public IGeoShape StReverse()
        {
            ISession session = _session;
            return ExecuteGetGeoObjectNoParam<IGeoShape>("StReverse", session);
        }

        public Task<IGeoShape> StReverseAsync(CancellationToken cancellationToken=default)
        {
            ISession session = _session;
            return ExecuteGetGeoObjectNoParamAsync<IGeoShape>("StReverse", session,cancellationToken);
        }

        public string StIsValidReason()
        {
            ISession session = _session;
            return ExecuteNoneGeo<string>("StIsValidReason", session);
        }

        public Task<string> StIsValidReasonAsync(CancellationToken cancellationToken=default)
        {
            ISession session = _session;
            return ExecuteNoneGeoAsync<string>("StIsValidReason", session,cancellationToken);
        }

        public IGeoShape StMakeValid()
        {
            ISession session = _session;
            return ExecuteGetGeoObjectNoParam<IGeoShape>("StMakeValid", session);
        }
        public Task<IGeoShape> StMakeValidAsync(CancellationToken cancellationToken=default)
        {
            ISession session = _session;
            return ExecuteGetGeoObjectNoParamAsync<IGeoShape>("StMakeValid", session,cancellationToken);
        }

        public string StAsGeoJson()
        {
            ISession session = _session;
            return ExecuteNoneGeo<string>("StAsGeoJSON", session);
        }

        public Task<string> StAsGeoJsonAsync(CancellationToken cancellationToken=default)
        {
            ISession session = _session;
            return ExecuteNoneGeoAsync<string>("StAsGeoJSON", session,cancellationToken);
        }


        public IGeoShape StUnion(IGeoShape shape)
        {
            ISession session = _session;
            return ExecuteGetGeoObjectParamGeo<IGeoShape>("StUnion", shape, session);
        }

        public Task<IGeoShape> StUnionAsync(IGeoShape shape,CancellationToken cancellationToken=default)
        {
            ISession session = _session;
            return ExecuteGetGeoObjectParamGeoAsync<IGeoShape>("StUnion", shape, session,cancellationToken);
        }






        T ExecuteTwoGeo<T>(string methodName,IGeoShape shape, ISession session)
        { 
            Check.NotEmpty(methodName, nameof(methodName));
            CheckSession(session,methodName);
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

            try
            {
                var res = session.ExecuteScalar(sql,
                    new SqlParam($"{p}1", this.StAsText()),
                    new SqlParam($"{p}2", this.StSrid()),
                    new SqlParam($"{p}3", shape.StAsText()),
                    new SqlParam($"{p}4", shape.StSrid()));

                return (T)UtilsCore.Convertor(res, typeof(T));
            }
            finally
            {
                _session = null;
            }
            

        }

        async Task<T> ExecuteTwoGeoAsync<T>(string methodName, IGeoShape shape, ISession session, CancellationToken cancellationToken)
        {
            Check.NotEmpty(methodName, nameof(methodName));
            CheckSession(session, methodName);
            Check.NotNull(session, nameof(session));
            Check.NotNull(shape, nameof(shape));
            ProviderName providerName = session.ProviderName;
            string sql = null;
            methodName = QueryTranslator<object>.GetNameMethod(methodName, providerName);
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

            try
            {
                var res = await session.ExecuteScalarAsync(sql, new object[]
                {
                    new SqlParam($"{p}1", this.StAsText()),
                    new SqlParam($"{p}2", this.StSrid()),
                    new SqlParam($"{p}3", shape.StAsText()),
                    new SqlParam($"{p}4", shape.StSrid())
                }, cancellationToken);

                return (T)UtilsCore.Convertor(res, typeof(T));
            }
            finally{
            {
                _session = null;
            }}

           

        }

        T ExecuteNoneGeo<T>(string methodName, ISession session)
        {
            Check.NotEmpty(methodName, nameof(methodName));
            CheckSession(session, methodName);

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

            try
            {
                var res = session.ExecuteScalar(sql,
                    new SqlParam($"{p}1", this.StAsText()),
                    new SqlParam($"{p}2", this.StSrid()));
                return (T)UtilsCore.Convertor(res, typeof(T));
            }
            finally
            {
                _session = null;
            }
           

        }

        async Task<T> ExecuteNoneGeoAsync<T>(string methodName, ISession session, CancellationToken cancellationToken)
        {
            Check.NotEmpty(methodName, nameof(methodName));
            CheckSession(session, methodName);

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


            var res = await session.ExecuteScalarAsync(sql,
                new object[]{new SqlParam($"{p}1", this.StAsText()),new SqlParam($"{p}2", this.StSrid())},
                    cancellationToken
                );
            var resFinal=UtilsCore.Convertor(res, typeof(T));
            if (resFinal == null)
            {
                return default;
            }

            return (T)resFinal;

        }

        T ExecuteGetGeoObjectNoParam<T>(string methodName, ISession session) where T:IGeoShape
        {
            
            Check.NotNull(methodName, nameof(methodName));
            CheckSession(session, methodName);
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

            try
            {
                var res = (string)session.ExecuteScalar(sql, this.StAsText());
                var str = UtilsCore.Convertor(res, typeof(string));
                if (str == null)
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

            finally
            {
                this._session = null;
            }
           
            
        }


        async Task<T> ExecuteGetGeoObjectNoParamAsync<T>(string methodName, ISession session, CancellationToken cancellationToken) where T : IGeoShape
        {

            Check.NotNull(methodName, nameof(methodName));
            CheckSession(session, methodName);
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

            try
            {
                var res = await session.ExecuteScalarAsync(sql, new object[] { this.StAsText() }, cancellationToken);
                var str = UtilsCore.Convertor(res as string, typeof(string));
                if (str == null)
                {
                    return default;
                }

                try
                {
                    return (T)FactoryGeo.CreateGeo(res as string);
                }
                catch (GeoException)
                {
                    return default;
                }
            }
            finally
            {
                _session = null;
            }
          

        }

        T ExecuteGetGeoObjectParamGeo<T>(string methodName,IGeoShape shape, ISession session) where T : IGeoShape
        {
           
            Check.NotEmpty(methodName, nameof(methodName));
            CheckSession(session, methodName);
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

            try
            {
                var res = (string)session.ExecuteScalar(sql, new SqlParam($"{p}1", this.StAsText()), new SqlParam($"{p}2", shape.StAsText()));
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
            finally
            {
                _session = null;
            }

           
           

        }

        async Task<T> ExecuteGetGeoObjectParamGeoAsync<T>(string methodName, IGeoShape shape, ISession session, CancellationToken cancellationToken) where T : IGeoShape
        {

            Check.NotEmpty(methodName, nameof(methodName));
            CheckSession(session, methodName);
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

            try
            {
                var res = await session.ExecuteScalarAsync(sql, new object[]
                {
                    new SqlParam($"{p}1", this.StAsText()), new SqlParam($"{p}2", shape.StAsText())

                }, CancellationToken.None);
                if (typeof(T) == typeof(IGeoShape))
                {
                    var str = UtilsCore.Convertor(res, typeof(string));
                    if (str == null)
                    {
                        return default;
                    }

                    try
                    {
                        return (T)FactoryGeo.CreateGeo(res as string);
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
            finally
            {
                _session = null;
            }

            


        }



        T ExecuteGetGeoObjectBufferE<T>(string methodName, ISession session,params object[] par ) where T : IGeoShape
        {
            Check.NotEmpty(methodName, nameof(methodName));
            CheckSession(session, methodName);

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

            try
            {
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
            finally
            {
                _session = null;
            }


        }

        async Task<T> ExecuteGetGeoObjectBufferEAsync<T>(string methodName, ISession session, object[] par, CancellationToken cancellationToken) where T : IGeoShape
        {
            Check.NotEmpty(methodName, nameof(methodName));
            CheckSession(session, methodName);

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
            string paramCore = String.Join(",", par);

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

            try
            {
                var res = await session.ExecuteScalarAsync(sql, new object[] { new SqlParam($"{p}1", this.StAsText()) },
                    cancellationToken);
                if (typeof(T) == typeof(IGeoShape))
                {
                    var str = UtilsCore.Convertor(res, typeof(string));
                    if (str == null)
                    {
                        return default;
                    }

                    try
                    {
                        return (T)FactoryGeo.CreateGeo(res as string);
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
            finally
            {
                _session = null;
            }


        }



        static void CheckSession(ISession session, string methodName)
        {
            if (session == null)
            {
                throw new Exception($"The instance method must be called through instance initialization by the session." +
                                    $" As shape.SetSession(session).{methodName}()");
            }
        }


    }
}
