using System;
using System.Collections.Generic;

using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace ORM_1_21_.geo
{
    /// <summary>
    /// 
    /// </summary>
      class GeoObject : IGeoShape
    {
        private string _innerStringGeo;
        private List<IGeoShape> _multiGeoShapes = new List<IGeoShape>();

        /// <summary>
        /// 
        /// </summary>
        public GeoObject()
        {
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        public GeoObject(string obj)
        {
            GeoData = obj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geoType"></param>
        /// <param name="points"></param>
        public GeoObject(GeoType geoType, double[] points)
        {
            if (geoType == GeoType.PolygonWithHole) throw new Exception("Запрещено создавать полигон с дырой");
            SetGeoTypePoints(geoType, points);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static GeoObject CreateGeoPolygonWithHole(double[] p1, double[] p2)
        {
            return new GeoObject(GeoType.PolygonWithHole,  new GeoObject(GeoType.Polygon, p1), new GeoObject(GeoType.Polygon, p2) );
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static GeoObject CreateGeoPolygonWithHole(IGeoShape p1, IGeoShape p2)
        {
            return new GeoObject(GeoType.PolygonWithHole,  p1, p2 );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geoType"></param>
        /// <param name="geoShapes"></param>
        public GeoObject(GeoType geoType, params IGeoShape[] geoShapes)
        {
            this.GeoType = geoType;
            string type = this.GeoType.ToString();

            if (geoType == GeoType.PolygonWithHole)
            {
                if (geoShapes.Length != 2)
                {
                    throw new ArgumentException("Полигон с дырой должен состоять из двух геометрий");
                }

                _multiGeoShapes = new List<IGeoShape>(geoShapes);
                StringBuilder builderP = new StringBuilder("POLYGON(");
                builderP.Append("(");
                foreach (GeoPoint point in geoShapes[0].ListGeoPoints)
                {
                    builderP.Append($"{point.Latitude} {point.Longitude}, ");
                }

                builderP = new StringBuilder(builderP.ToString().Trim(' ', ',')).Append("), (");
                foreach (GeoPoint point in geoShapes[1].ListGeoPoints)
                {
                    builderP.Append($"{point.Latitude} {point.Longitude}, ");
                }

                _innerStringGeo = builderP.ToString().Trim(' ', ',') + "))";



                return;
            }


            _multiGeoShapes = new List<IGeoShape>(geoShapes);
            StringBuilder builder = new StringBuilder($"{type}(");



            if (geoType == GeoType.GeometryCollection)
            {
                foreach (IGeoShape geoShape in geoShapes)
                {
                    builder.Append(geoShape.GeoData).Append(", ");
                }
                _innerStringGeo = builder.ToString().TrimEnd(',', ' ') + ")";
                return;

            }
            foreach (IGeoShape geoShape in geoShapes)
            {
                if (geoShape.GeoType != this.GeoType)
                {
                    //throw new Exception($"Не совпадение типов для {this.GeoType}");

                }



                Regex regex = new Regex(@"\((.*)\)");
                MatchCollection matches = regex.Matches(geoShape.GeoData);
                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                    {
                        var rr = match.Value.Trim('(', ')', ' ');
                        if (this.GeoType == GeoType.MultiPolygon)
                        {
                            builder.Append($"(({rr})), ");

                        }
                        else
                        {
                            builder.Append($"({rr}), ");
                        }



                    }

                }
                else
                {
                    throw new Exception("Совпадений не найдено");
                }

            }
            _innerStringGeo = builder.ToString().TrimEnd(',', ' ') + ")";

        }

        /// <summary>
        /// Only Point
        /// </summary>
        /// <param name="geoPoint"></param>
        public GeoObject(GeoPoint geoPoint)
        {
            ListGeoPoints = new List<GeoPoint> { geoPoint };
            SetGeoTypePoints(GeoType.Point, new[] { geoPoint.Latitude, geoPoint.Longitude });
        }

        /// <summary>
        /// Only Point
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        public GeoObject(double latitude, double longitude)
        {
            ListGeoPoints = new List<GeoPoint> { new GeoPoint { Latitude = latitude, Longitude = longitude } };
            SetGeoTypePoints(GeoType.Point, new[] { latitude, longitude });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geoType"></param>
        /// <param name="points"></param>
        public GeoObject(GeoType geoType, List<double[]> points)
        {
            ListGeoPoints = new List<GeoPoint>();
            double[] l = new double[points.Count * 2];
            int i = 0;
            foreach (double[] doubles in points)
            {
                ListGeoPoints.Add(new GeoPoint { Longitude = doubles[1], Latitude = doubles[0] });
                l[i] = doubles[0]; i++;
                l[i] = doubles[1]; i++;
            }

            try
            {
                SetGeoTypePoints(geoType, l);

            }
            catch (Exception)
            {
                ListGeoPoints = new List<GeoPoint>();
                throw;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geoType"></param>
        /// <param name="points"></param>
        public GeoObject(GeoType geoType, params GeoPoint[] points)
        {
            if (geoType == GeoType.PolygonWithHole) throw new Exception("Запрещено создавать полигон с дырой");
            ListGeoPoints = new List<GeoPoint>();
            double[] l = new double[points.Length * 2];
            int i = 0;
            foreach (GeoPoint doubles in points)
            {
                ListGeoPoints.Add(new GeoPoint { Longitude = doubles.Longitude, Latitude = doubles.Latitude });
                l[i] = doubles.Latitude; i++;
                l[i] = doubles.Longitude; i++;

            }

            SetGeoTypePoints(geoType, l);
        }


        /// <summary>
        /// 
        /// </summary>
        public int Srid { get; set; } = 4326;

        /// <summary>
        /// 
        /// </summary>
        public GeoType GeoType { get; set; }



        /// <summary>
        /// 
        /// </summary>
        public string GeoData
        {
            get => _innerStringGeo;
            set
            {
                _innerStringGeo = value;
                var str = value.ToUpper().Trim();
                if (str.StartsWith("POLYGON"))
                {
                    Regex regex = new Regex(@"\(([^)]+)\)");
                    MatchCollection matches = regex.Matches(str);
                    if (matches.Count == 1)
                    {
                        GeoType = GeoType.Polygon;
                        ListGeoPoints = UtilsGeo.GetListPoint(GeoType, str, _multiGeoShapes);
                        return;
                    }

                    if (matches.Count == 2)
                    {
                        GeoType = GeoType.PolygonWithHole;
                        ListGeoPoints = UtilsGeo.GetListPoint(GeoType, str, _multiGeoShapes);
                        return;
                    }

                }
                if (str.StartsWith("POINT"))
                {
                    GeoType = GeoType.Point;
                }
                if (str.StartsWith("LINESTRING"))
                {
                    GeoType = GeoType.LineString;
                }
                if (str.StartsWith("MULTIPOINT"))
                {
                    GeoType = GeoType.MultiPoint;
                }
                if (str.StartsWith("MULTILINESTRING"))
                {
                    GeoType = GeoType.MultiLineString;
                }
                if (str.StartsWith("MULTIPOLYGON"))
                {
                    GeoType = GeoType.MultiPolygon;
                }
                if (str.StartsWith("GEOMETRYCOLLECTION"))
                {
                    GeoType = GeoType.GeometryCollection;
                }
                if (str.StartsWith("CIRCULARSTRING"))
                {
                    GeoType = GeoType.CircularString;
                }

                ListGeoPoints = UtilsGeo.GetListPoint(GeoType, str, _multiGeoShapes);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="points"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void SetGeoTypePoints(GeoType type, double[] points)
        {
            if (type == GeoType.None)
                throw new ArgumentException("type not is none");
            if (points == null || points.Length == 0) throw new ArgumentException("point is empty");


            GeoType = type;
            switch (type)
            {
                case GeoType.Point when points.Length != 2:
                    throw new Exception("Точка должна определяться двумя координатами");
                case GeoType.Point:
                    _innerStringGeo = $"POINT({points[0].ToString(CultureInfo.InvariantCulture)} {points[1].ToString(CultureInfo.InvariantCulture)})";
                    ListGeoPoints.Add(new GeoPoint{Latitude = points[0],Longitude = points[1] });
                    break;
                case GeoType.LineString:
                    {
                        if (points.Length % 2 != 0)
                            throw new Exception("Количество точек определяющее линию, должно быть четным");
                        StringBuilder builder = new StringBuilder("LINESTRING(");
                        for (int i = 0; i < points.Length; i += 2)
                        {
                            builder.Append(
                                $"{points[i].ToString(CultureInfo.InvariantCulture)} {points[i + 1].ToString(CultureInfo.InvariantCulture)}, ");
                            ListGeoPoints.Add(new GeoPoint() { Latitude = points[i], Longitude = points[i + 1] });

                        }
                        _innerStringGeo = builder.ToString().Trim(' ', ',') + ")";
                        break;
                    }
                case GeoType.Polygon:
                    {

                        if (points.Length % 2 != 0)
                            throw new Exception("Количество точек определяющее линию, должно быть четным");
                        StringBuilder builder = new StringBuilder("POLYGON((");
                        for (int i = 0; i < points.Length; i += 2)
                        {

                            builder.Append(
                                $"{points[i].ToString(CultureInfo.InvariantCulture)} {points[i + 1].ToString(CultureInfo.InvariantCulture)}, ");
                            ListGeoPoints.Add(new GeoPoint() { Latitude = points[i], Longitude = points[i + 1] });

                        }
                        _innerStringGeo = builder.ToString().Trim(' ', ',') + "))";
                        break;

                    }


                case GeoType.None:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);

                case GeoType.MultiPoint:
                    {
                        if (points.Length % 2 != 0)
                            throw new Exception("Количество точек  быть четным");
                        StringBuilder builder = new StringBuilder("MULTIPOINT(");
                        for (int i = 0; i < points.Length; i += 2)
                        {
                            builder.Append(
                                $"{points[i].ToString(CultureInfo.InvariantCulture)} {points[i + 1].ToString(CultureInfo.InvariantCulture)}, ");

                        }
                        foreach (GeoPoint geoPoint in ListGeoPoints)
                        {
                            _multiGeoShapes.Add(new GeoObject(GeoType.Point,  geoPoint ));
                        }
                        _innerStringGeo = builder.ToString().Trim(' ', ',') + ")";
                        break;
                    }
                case GeoType.CircularString:
                    {
                        StringBuilder builder = new StringBuilder("CIRCULARSTRING(");
                        for (int i = 0; i < points.Length; i += 2)
                        {
                            builder.Append(
                                $"{points[i].ToString(CultureInfo.InvariantCulture)} {points[i + 1].ToString(CultureInfo.InvariantCulture)}, ");

                        }
                        foreach (GeoPoint geoPoint in ListGeoPoints)
                        {
                            _multiGeoShapes.Add(new GeoObject(GeoType.Point, geoPoint));
                        }
                        _innerStringGeo = builder.ToString().Trim(' ', ',') + ")";
                        break;
                    }

                case GeoType.MultiLineString:
                case GeoType.MultiPolygon:
                case GeoType.GeometryCollection:

                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public List<GeoPoint> ListGeoPoints { get; set; } = new List<GeoPoint>();



        /// <summary>
        /// 
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public object GetGeoJson(object properties = null)
        {
            if (GeoType == GeoType.GeometryCollection)
            {
                return new GeoJsonCollection(this, properties);
            }
            return new GeoJson(this, properties);
        }

        /// <summary>
        /// 
        /// </summary>
        public List<IGeoShape> MultiGeoShapes
        {
            get => _multiGeoShapes;
            set => _multiGeoShapes = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public object ArrayCoordinate
        {
            get
            {
                switch (GeoType)
                {
                    case GeoType.None:
                        throw new ArgumentOutOfRangeException();

                    case GeoType.Point:
                        return new[] { ListGeoPoints[0].Latitude, ListGeoPoints[0].Longitude };
                    case GeoType.LineString:
                        {
                            var s = new object[ListGeoPoints.Count];
                            for (int i = 0; i < ListGeoPoints.Count; i++)
                            {
                                s[i] = new[] { ListGeoPoints[i].Latitude, ListGeoPoints[i].Longitude };
                            }
                            return s;
                        }


                    case GeoType.MultiPoint:
                        {
                            var s = new object[_multiGeoShapes.Count];
                            for (int i = 0; i < _multiGeoShapes.Count; i++)
                            {
                                s[i] = _multiGeoShapes[i].ArrayCoordinate;
                            }

                            return s;
                        }

                    case GeoType.MultiLineString:
                        {
                            var s = new object[_multiGeoShapes.Count];
                            for (int i = 0; i < _multiGeoShapes.Count; i++)
                            {
                                s[i] = _multiGeoShapes[i].ArrayCoordinate;
                            }

                            return s;

                        }

                    case GeoType.MultiPolygon:
                        {
                            var s = new object[_multiGeoShapes.Count];
                            for (int i = 0; i < _multiGeoShapes.Count; i++)
                            {
                                s[i] = _multiGeoShapes[i].ArrayCoordinate;
                            }

                            return s;

                        }


                    case GeoType.Polygon:
                        {
                            var sp = new object[1];
                            var s = new object[ListGeoPoints.Count];
                            for (int i = 0; i < ListGeoPoints.Count; i++)
                            {
                                s[i] = new[] { ListGeoPoints[i].Latitude, ListGeoPoints[i].Longitude };
                            }

                            sp[0] = s;
                            return sp;
                        }
                    case GeoType.PolygonWithHole:
                        {
                            var sp = new object[2];
                            sp[0] = ((object[])_multiGeoShapes[0].ArrayCoordinate)[0];
                            sp[1] = ((object[])_multiGeoShapes[1].ArrayCoordinate)[0];

                            return sp;
                        }
                    case GeoType.GeometryCollection:
                        {
                            var sp = new object[1];

                            return sp;
                        }

                    default:
                        throw new ArgumentOutOfRangeException();
                }

            }
            set => throw new NotImplementedException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="point"></param>
        /// <exception cref="NotImplementedException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void SetGeoTypePoints(GeoType type, GeoPoint point)
        {
            if (type == GeoType.None) throw new ArgumentException("type not is none");
            if (point == null) throw new ArgumentException("point is empty");
            GeoType = type;
            switch (type)
            {

                case GeoType.Point:
                    _innerStringGeo = $"POINT({point.Latitude.ToString(CultureInfo.InvariantCulture)} {point.Longitude.ToString(CultureInfo.InvariantCulture)})";
                    break;


            }
        }




    }
}