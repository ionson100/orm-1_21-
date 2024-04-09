using ORM_1_21_.Linq;
using ORM_1_21_.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace ORM_1_21_.geo
{
    partial class GeoObject : IGeoShape
    {
        object IGeoShape.ArrayCoordinates()
        {
            return ArrayCoordinates;
        }

        public string StGeometryType()
        {
            return ExecuteNoneGeo<string>("StGeometryType");
        }

        public Task<string> StGeometryTypeAsync(CancellationToken cancellationToken = default)
        {
            return ExecuteNoneGeoAsync<string>("StGeometryType", cancellationToken);
        }

        public double? StArea()
        {
            return ExecuteNoneGeo<double?>("StArea");
        }

        public Task<double?> StAreaAsync(CancellationToken cancellationToken = default)
        {
            return ExecuteNoneGeoAsync<double?>("StArea", cancellationToken);
        }

        public bool? StWithin(IGeoShape shape)
        {
            return ExecuteTwoGeo<bool?>("StWithin", shape);
        }

        public Task<bool?> StWithinAsync(IGeoShape shape, CancellationToken cancellationToken = default)
        {
            return ExecuteTwoGeoAsync<bool?>("StWithin", shape, cancellationToken);
        }

        public byte[] StAsBinary()
        {
            return ExecuteNoneGeo<byte[]>("StAsBinary");
        }

        public Task<byte[]> StAsBinaryAsync(CancellationToken cancellationToken = default)
        {
            return ExecuteNoneGeoAsync<byte[]>("StAsBinary", cancellationToken);
        }

        public IGeoShape StBoundary()
        {
            return ExecuteGetGeoObjectNoParam<IGeoShape>("StBoundary");
        }

        public Task<IGeoShape> StBoundaryAsync(CancellationToken cancellationToken = default)
        {
            return ExecuteGetGeoObjectNoParamAsync<IGeoShape>("StBoundary", cancellationToken);

        }

        public IGeoShape StBuffer(float distance)
        {
            ISession session = _session;
            return ExecuteGetGeoObjectBufferE<IGeoShape>("StBuffer", session, distance);
        }


        public Task<IGeoShape> StBufferAsync(float distance, CancellationToken cancellationToken = default)
        {
            ISession session = _session;
            return ExecuteGetGeoObjectBufferEAsync<IGeoShape>("StBuffer", session, new object[] { distance }, cancellationToken);
        }

        public IGeoShape StCentroid()
        {
            return ExecuteGetGeoObjectNoParam<IGeoShape>("StCentroid");
        }


        public Task<IGeoShape> StCentroidAsync(CancellationToken cancellationToken = default)
        {
            return ExecuteGetGeoObjectNoParamAsync<IGeoShape>("StCentroid", cancellationToken);
        }

        public bool? StContains(IGeoShape shape)
        {
            return ExecuteTwoGeo<bool?>("StContains", shape);
        }


        public Task<bool?> StContainsAsync(IGeoShape shape, CancellationToken cancellationToken)
        {
            return ExecuteTwoGeoAsync<bool?>("StContains", shape, cancellationToken);
        }

        public bool? StCrosses(IGeoShape shape)
        {
            return ExecuteTwoGeo<bool?>("StCrosses", shape);
        }


        public Task<bool?> StCrossesAsync(IGeoShape shape, CancellationToken cancellationToken = default)
        {
            return ExecuteTwoGeoAsync<bool?>("StCrosses", shape, cancellationToken);
        }

        public IGeoShape StDifference(IGeoShape shape)
        {
            return ExecuteGetGeoObjectParamGeo<IGeoShape>("StDifference", shape);
        }

        public Task<IGeoShape> StDifferenceAsync(IGeoShape shape, CancellationToken cancellationToken = default)
        {
            return ExecuteGetGeoObjectParamGeoAsync<IGeoShape>("StDifference", shape, cancellationToken);

        }

        public int? StDimension()
        {
            return ExecuteNoneGeo<int?>("StDimension");
        }

        public Task<int?> StDimensionAsync(CancellationToken cancellationToken = default)
        {
            return ExecuteNoneGeoAsync<int?>("StDimension", cancellationToken);
        }

        public bool? StDisjoint(IGeoShape shape)
        {
            return ExecuteTwoGeo<bool?>("StDisjoint", shape);
        }

        public Task<bool?> StDisjointAsync(IGeoShape shape, CancellationToken cancellationToken = default)
        {
            return ExecuteTwoGeoAsync<bool?>("StDisjoint", shape, cancellationToken);
        }

        public double? StDistance(IGeoShape shape)
        {
            return ExecuteTwoGeo<double?>("StDistance", shape);
        }

        public Task<double?> StDistanceAsync(IGeoShape shape, CancellationToken cancellationToken = default)
        {
            return ExecuteTwoGeoAsync<double?>("StDistance", shape, cancellationToken);
        }

        public IGeoShape StEndPoint()
        {
            return ExecuteGetGeoObjectNoParam<IGeoShape>("StEndPoint");
        }

        public Task<IGeoShape> StEndPointAsync(CancellationToken cancellationToken = default)
        {
            return ExecuteGetGeoObjectNoParamAsync<IGeoShape>("StEndPoint", cancellationToken);
        }

        public IGeoShape StEnvelope()
        {
            return ExecuteGetGeoObjectNoParam<IGeoShape>("StEnvelope");
        }

        public Task<IGeoShape> StEnvelopeAsync(CancellationToken cancellationToken = default)
        {

            return ExecuteGetGeoObjectNoParamAsync<IGeoShape>("StEnvelope", cancellationToken);
        }


        public bool? StEquals(IGeoShape shape)
        {
            return ExecuteTwoGeo<bool?>("StEquals", shape);
        }

        public Task<bool?> StEqualsAsync(IGeoShape shape, CancellationToken cancellationToken = default)
        {
            return ExecuteTwoGeoAsync<bool?>("StEquals", shape, cancellationToken);
        }

        public bool? StIntersects(IGeoShape shape)
        {
            return ExecuteTwoGeo<bool?>("StIntersects", shape);
        }

        public Task<bool?> StIntersectsAsync(IGeoShape shape, CancellationToken cancellationToken = default)
        {
            return ExecuteTwoGeoAsync<bool?>("StIntersects", shape, cancellationToken);
        }

        public bool? StOverlaps(IGeoShape shape)
        {
            return ExecuteTwoGeo<bool?>("StOverlaps", shape);
        }


        public Task<bool?> StOverlapsAsync(IGeoShape shape, CancellationToken cancellationToken = default)
        {
            return ExecuteTwoGeoAsync<bool?>("StOverlaps", shape, cancellationToken);
        }

        public int? StSrid()
        {
            return Srid;
        }

        public IGeoShape StStartPoint()
        {
            return ExecuteGetGeoObjectNoParam<IGeoShape>("StStartPoint");
        }

        public Task<IGeoShape> StStartPointAsync(CancellationToken cancellationToken = default)
        {
            return ExecuteGetGeoObjectNoParamAsync<IGeoShape>("StStartPoint", cancellationToken);
        }

        public IGeoShape StSymDifference(IGeoShape shape)
        {
            return ExecuteGetGeoObjectParamGeo<IGeoShape>("StSymDifference", shape);
        }

        public Task<IGeoShape> StSymDifferenceAsync(IGeoShape shape, CancellationToken cancellationToken = default)
        {
            return ExecuteGetGeoObjectParamGeoAsync<IGeoShape>("StSymDifference", shape, cancellationToken);
        }

        public bool? StTouches(IGeoShape shape)
        {
            return ExecuteTwoGeo<bool?>("StTouches", shape);
        }


        public Task<bool?> StTouchesAsync(IGeoShape shape, CancellationToken cancellationToken = default)
        {
            return ExecuteTwoGeoAsync<bool?>("StTouches", shape, cancellationToken);
        }

        public bool? StToucheAsync(IGeoShape shape)
        {
            return ExecuteTwoGeo<bool?>("StTouches", shape);
        }

        public int? StNumGeometries()
        {
            return ExecuteNoneGeo<int>("StNumGeometries");
        }

        public Task<int?> StNumGeometriesAsync(CancellationToken cancellationToken = default)
        {
            return ExecuteNoneGeoAsync<int?>("StNumGeometries", cancellationToken);
        }

        public int? StNumInteriorRing()
        {
            return ExecuteNoneGeo<int>("StNumInteriorRing");
        }

        public Task<int?> StNumInteriorRingAsync(CancellationToken cancellationToken = default)
        {
            return ExecuteNoneGeoAsync<int?>("StNumInteriorRing", cancellationToken);
        }

        public bool? StIsSimple()
        {
            return ExecuteNoneGeo<bool?>("StIsSimple");
        }

        public Task<bool?> StIsSimpleAsync(CancellationToken cancellationToken = default)
        {
            return ExecuteNoneGeoAsync<bool?>("StIsSimple", cancellationToken);
        }

        public bool? StIsValid()
        {
            return ExecuteNoneGeo<bool?>("StIsValid");
        }

        public Task<bool?> StIsValidAsync(CancellationToken cancellationToken = default)
        {
            return ExecuteNoneGeoAsync<bool?>("StIsValid", cancellationToken);
        }

        public double? StLength()
        {
            return ExecuteNoneGeo<double?>("StLength");
        }

        public Task<double?> StLengthAsync(CancellationToken cancellationToken = default)
        {
            return ExecuteNoneGeoAsync<double?>("StLength", cancellationToken);
        }

        public bool? StIsClosed()
        {
            return ExecuteNoneGeo<bool?>("StIsClosed");
        }

        public Task<bool?> StIsClosedAsync(CancellationToken cancellationToken = default)
        {
            return ExecuteNoneGeoAsync<bool?>("StIsClosed", cancellationToken);
        }

        public int? StNumPoints()
        {
            return ExecuteNoneGeo<int?>("StNumPoints");
        }

        public Task<int?> StNumPointsAsync(CancellationToken cancellationToken = default)
        {
            return ExecuteNoneGeoAsync<int?>("StNumPoints", cancellationToken);
        }

        public double? StPerimeter()
        {
            return ExecuteNoneGeo<double?>("StPerimeter");
        }

        public Task<double?> StPerimeterAsync(CancellationToken cancellationToken = default)
        {
            return ExecuteNoneGeoAsync<double?>("StPerimeter", cancellationToken);
        }

        public IGeoShape StTranslate(float deltaX, float deltaY)
        {
            ISession session = _session;
            return ExecuteGetGeoObjectBufferE<IGeoShape>("StTranslate", session, deltaX, deltaY);
        }

        public Task<IGeoShape> StTranslateAsync(float deltaX, float deltaY, CancellationToken cancellationToken = default)
        {
            ISession session = _session;
            return ExecuteGetGeoObjectBufferEAsync<IGeoShape>("StTranslate", session, new object[] { deltaX, deltaY }, cancellationToken);
        }

        public IGeoShape StConvexHull()
        {
            return ExecuteGetGeoObjectNoParam<IGeoShape>("StConvexHull");
        }

        public Task<IGeoShape> StConvexHullAsync(CancellationToken cancellationToken = default)
        {
            return ExecuteGetGeoObjectNoParamAsync<IGeoShape>("StConvexHull", cancellationToken);
        }

        public IGeoShape StCollect(params IGeoShape[] shapes)
        {
            if (shapes.Length == 0) throw new Exception("shapes empty");
            List<IGeoShape> list=new List<IGeoShape>(shapes);
            list.Insert(0,this);
            return FactoryGeo.GeometryCollection(list.ToArray());
        }




        public IGeoShape StPointN(int n)
        {
            ISession session = _session;

            return ExecuteGetGeoObjectBufferE<IGeoShape>("StPointN", session, n);
        }

        public Task<IGeoShape> StPointNAsync(int n, CancellationToken cancellationToken = default)
        {
            ISession session = _session;

            return ExecuteGetGeoObjectBufferEAsync<IGeoShape>("StPointN", session, new object[] { n }, cancellationToken);
        }

        public IGeoShape StPointOnSurface()
        {
            return ExecuteGetGeoObjectNoParam<IGeoShape>("StPointOnSurface");
        }

        public Task<IGeoShape> StPointOnSurfaceAsync(CancellationToken cancellationToken = default)
        {
            return ExecuteGetGeoObjectNoParamAsync<IGeoShape>("StPointOnSurface", cancellationToken);
        }

        public IGeoShape StInteriorRingN(int n)
        {
            ISession session = _session;

            return ExecuteGetGeoObjectBufferE<IGeoShape>("StInteriorRingN", session, n);
        }

        public Task<IGeoShape> StInteriorRingNAsync(int n, CancellationToken cancellationToken = default)
        {
            ISession session = _session;

            return ExecuteGetGeoObjectBufferEAsync<IGeoShape>("StInteriorRingN", session, new object[] { n }, cancellationToken);
        }

        public double? StX()
        {
            if (GeoType != GeoType.Point)
            {
                throw new Exception("Input must be a point.");
            }

            return ExecuteNoneGeo<double?>("StX");
        }

        public Task<double?> StXAsync(CancellationToken cancellationToken = default)
        {
            if (GeoType != GeoType.Point)
            {
                throw new Exception("Input must be a point.");
            }
            return ExecuteNoneGeoAsync<double?>("StX", cancellationToken);
        }

        public double? StY()
        {
            if (GeoType != GeoType.Point)
            {
                throw new Exception("Input must be a point.");
            }

            return ExecuteNoneGeo<double?>("StY");
        }

        public Task<double?> StYAsync(CancellationToken cancellationToken)
        {
            if (GeoType != GeoType.Point)
            {
                throw new Exception("Input must be a point.");
            }
            return ExecuteNoneGeoAsync<double?>("StY", cancellationToken);
        }

        public IGeoShape StTransform(int srid)
        {
            ISession session = _session;

            return ExecuteGetGeoObjectBufferE<IGeoShape>("StTransform", session, srid);
        }

        public Task<IGeoShape> StTransformAsync(int srid, CancellationToken cancellationToken = default)
        {
            ISession session = _session;

            return ExecuteGetGeoObjectBufferEAsync<IGeoShape>("StTransform", session, new object[] { srid }, cancellationToken);
        }

        public IGeoShape StSetSRID(int srid)
        {
            SetSrid(srid);
            return this;
          
        }

        public string StAsLatLonText(string format = null)
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
            return ExecuteGetGeoObjectNoParam<IGeoShape>("StReverse");
        }

        public Task<IGeoShape> StReverseAsync(CancellationToken cancellationToken = default)
        {
            return ExecuteGetGeoObjectNoParamAsync<IGeoShape>("StReverse", cancellationToken);
        }

        public string StIsValidReason()
        {
            return ExecuteNoneGeo<string>("StIsValidReason");
        }

        public Task<string> StIsValidReasonAsync(CancellationToken cancellationToken = default)
        {
            return ExecuteNoneGeoAsync<string>("StIsValidReason", cancellationToken);
        }

        public IGeoShape StMakeValid()
        {
            return ExecuteGetGeoObjectNoParam<IGeoShape>("StMakeValid");
        }


        public Task<IGeoShape> StMakeValidAsync(CancellationToken cancellationToken = default)
        {
            return ExecuteGetGeoObjectNoParamAsync<IGeoShape>("StMakeValid", cancellationToken);
        }

        public string StAsGeoJson()
        {
            return ExecuteNoneGeo<string>("StAsGeoJSON");
        }

        public Task<string> StAsGeoJsonAsync(CancellationToken cancellationToken = default)
        {

            return ExecuteNoneGeoAsync<string>("StAsGeoJSON", cancellationToken);
        }


        public IGeoShape StUnion(IGeoShape shape)
        {
            return ExecuteGetGeoObjectParamGeo<IGeoShape>("StUnion", shape);
        }

        public Task<IGeoShape> StUnionAsync(IGeoShape shape, CancellationToken cancellationToken = default)
        {
            return ExecuteGetGeoObjectParamGeoAsync<IGeoShape>("StUnion", shape, cancellationToken);
        }

        public IGeoShape StIntersection( IGeoShape b)
        {
            return ExecuteGetGeoObjectParamGeo<IGeoShape>("StIntersection", b);
        }

        public async Task<IGeoShape> StIntersectionAsync(IGeoShape b, CancellationToken cancellationToken = default)
        {
            return await ExecuteGetGeoObjectParamGeoAsync<IGeoShape>("StIntersection", b,cancellationToken);
        }

        public double StLineLocatePoint(IGeoShape point)
        {
            return ExecuteGetGeoObjectParamGeoDouble<double>("StLineLocatePoint", point);
        }

        public async Task<double> StLineLocatePointAsync(IGeoShape point, CancellationToken cancellationToken = default)
        {
            return await ExecuteGetGeoObjectParamGeoDoubleAsync<double>("StLineLocatePoint", point,cancellationToken);
        }

        public IGeoShape StLineInterpolatePoint(float f)
        {
            return ExecuteGetGeoObjectBufferE<IGeoShape>("StLineInterpolatePoint", _session, f);
        }

        public async Task<IGeoShape> StLineInterpolatePointAsync(float f, CancellationToken cancellationToken = default)
        {
            return await ExecuteGetGeoObjectBufferEAsync<IGeoShape>("StLineInterpolatePoint", _session, new object[]{f},cancellationToken);
        }

        public IGeoShape StLineSubstring(float startfraction, float endfraction)
        {
           
            return ExecuteGetGeoObjectBufferE<IGeoShape>("StLineSubstring", _session, startfraction, endfraction);
        }

        public Task<IGeoShape> StLineSubstringAsync(float startfraction, float endfraction, CancellationToken cancellationToken = default)
        {
            return ExecuteGetGeoObjectBufferEAsync<IGeoShape>("StLineSubstring", _session,new object[]{startfraction,endfraction}  ,cancellationToken);
        }


        T ExecuteTwoGeo<T>(string methodName, IGeoShape shape)
        {
            ISession session = _session;
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

        async Task<T> ExecuteTwoGeoAsync<T>(string methodName, IGeoShape shape, CancellationToken cancellationToken)
        {
            ISession session = _session;
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
            finally
            {
                {
                    _session = null;
                }
            }



        }

        T ExecuteNoneGeo<T>(string methodName)
        {
            ISession session = _session;
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

        async Task<T> ExecuteNoneGeoAsync<T>(string methodName, CancellationToken cancellationToken)
        {
            ISession session = _session;
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
                var res = await session.ExecuteScalarAsync(sql,
                    new object[] { new SqlParam($"{p}1", this.StAsText()), new SqlParam($"{p}2", this.StSrid()) },
                    cancellationToken
                );
                var resFinal = UtilsCore.Convertor(res, typeof(T));
                if (resFinal == null)
                {
                    return default;
                }

                return (T)resFinal;
            }
            finally
            {
                _session = null;
            }


        }

        T ExecuteGetGeoObjectNoParam<T>(string methodName) where T : IGeoShape
        {
            ISession session = _session;
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

        async Task<T> ExecuteGetGeoObjectNoParamAsync<T>(string methodName, CancellationToken cancellationToken) where T : IGeoShape
        {
            ISession session = _session;
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

        T ExecuteGetGeoObjectParamGeo<T>(string methodName, IGeoShape shape) where T : IGeoShape
        {

            ISession session = _session;
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
        T ExecuteGetGeoObjectParamGeoDouble<T>(string methodName, IGeoShape shape) 
        {

            ISession session = _session;
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
                    sql = $" select geometry::STGeomFromText({p}1, {srid}).{methodName}(geometry::STGeomFromText({p}2, {srid}))";
                    break;
                case ProviderName.MySql:
                    sql = $" select {methodName}(ST_GeomFromText({p}1, {srid}),ST_GeomFromText({p}2, {srid}))";
                    break;
                case ProviderName.PostgreSql:
                    sql = $"select {methodName}(ST_GeomFromText({p}1, {srid}),ST_GeomFromText({p}2, {srid}))";
                    break;
                case ProviderName.SqLite:
                    UtilsCore.ErrorAlert();
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Database type is not defined:{providerName}");
            }

            try
            {
                var res = session.ExecuteScalar(sql, new SqlParam($"{p}1", this.StAsText()),
                    new SqlParam($"{p}2", shape.StAsText()));


                return (T)res;
            }
            finally
            {
                _session = null;
            }




        }

        async Task<T> ExecuteGetGeoObjectParamGeoDoubleAsync<T>(string methodName, IGeoShape shape, CancellationToken cancellationToken)
        {

            ISession session = _session;
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
                    sql = $" select geometry::STGeomFromText({p}1, {srid}).{methodName}(geometry::STGeomFromText({p}2, {srid}))";
                    break;
                case ProviderName.MySql:
                    sql = $" select {methodName}(ST_GeomFromText({p}1, {srid}),ST_GeomFromText({p}2, {srid}))";
                    break;
                case ProviderName.PostgreSql:
                    sql = $"select {methodName}(ST_GeomFromText({p}1, {srid}),ST_GeomFromText({p}2, {srid}))";
                    break;
                case ProviderName.SqLite:
                    UtilsCore.ErrorAlert();
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Database type is not defined:{providerName}");
            }

            try
            {
                var o = new object[] {new SqlParam($"{p}1", this.StAsText()),new SqlParam($"{p}2", shape.StAsText()) };
                var res = await session.ExecuteScalarAsync(sql, o,cancellationToken);


                return (T)res;
            }
            finally
            {
                _session = null;
            }




        }

        async Task<T> ExecuteGetGeoObjectParamGeoAsync<T>(string methodName, IGeoShape shape, CancellationToken cancellationToken) where T : IGeoShape
        {
            ISession session = _session;
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

                }, cancellationToken);
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

        T ExecuteGetGeoObjectBufferE<T>(string methodName, ISession session, params object[] par) where T : IGeoShape
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




        public object Clone()
        {
           return FactoryGeo.CreateGeo(this.StAsText()).SetSrid(this.Srid);
        }
    }
}
