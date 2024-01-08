using System;
using System.Collections.Generic;

using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace ORM_1_21_.geo
{
    
      class GeoObject : IGeoShape
    {
        private string _innerStringGeo;
        private List<IGeoShape> _multiGeoShapes = new List<IGeoShape>();
        public GeoObject(string obj)
        {
            var t = obj.IndexOf(';');
            if (obj.Trim().ToUpper().StartsWith("SRID")&&t>0)
            {
                var s = obj.Substring(0,t).Trim(" SRID=".ToCharArray());
                Srid = int.Parse(s);
                obj=obj.Substring(t+1);
            }
           
            GeoText = obj;
        }
        public GeoObject(GeoType geoType, double[] points)
        {
            if (geoType == GeoType.PolygonWithHole) throw new Exception("It is forbidden to create a polygon with a hole");
            SetGeoTypePoints(geoType, points);
        }
        
        public static GeoObject CreateGeoPolygonWithHole(params double[][] p2)
        {
            List<GeoObject> list = new List<GeoObject>();
             foreach (double[] doubles in p2)
            {
                list.Add(new GeoObject(GeoType.Polygon, doubles));
            }
            return new GeoObject(GeoType.PolygonWithHole, list.ToArray());
        }
        
        public static GeoObject CreateGeoPolygonWithHole(IGeoShape p1,params IGeoShape[] p2)
        {
            List<IGeoShape> list=new List<IGeoShape> { p1 };
            list.AddRange(p2);
            return new GeoObject(GeoType.PolygonWithHole, list.ToArray());
        }
        
        public GeoObject(GeoType geoType, params IGeoShape[] geoShapes)
        {
            this.GeoType = geoType;
            string type = this.GeoType.ToString();

            if (geoType == GeoType.PolygonWithHole)
            {
                if (geoShapes.Length == 0|| geoShapes.Length == 1)
                {
                    throw new ArgumentException("A polygon with a hole must consist min of two geometries");
                }
                _multiGeoShapes = new List<IGeoShape>(geoShapes);
                StringBuilder builderP = new StringBuilder("POLYGON(");
                builderP.Append("(");
                foreach (var geoShape in geoShapes)
                {
                    foreach (GeoPoint point in geoShape.ListGeoPoints)
                    {
                        builderP.Append($"{point.X.ToString("G")} {point.Y:G}, ");
                    }
                    builderP = new StringBuilder(builderP.ToString().Trim(' ', ',')).Append("), (");
                }
                _innerStringGeo = builderP.ToString().Trim('(',' ', ',') + ")";
                return;
            }


            _multiGeoShapes = new List<IGeoShape>(geoShapes);
            StringBuilder builder = new StringBuilder($"{type}(");
            
            if (geoType == GeoType.GeometryCollection)
            {
                foreach (IGeoShape geoShape in geoShapes)
                {
                    builder.Append(geoShape.GeoText).Append(", ");
                }
                _innerStringGeo = builder.ToString().TrimEnd(',', ' ') + ")";
                return;

            }
            foreach (IGeoShape geoShape in geoShapes)
            {
                Regex regex = new Regex(@"\((.*)\)");
                MatchCollection matches = regex.Matches(geoShape.GeoText);
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
                    throw new Exception(" orm factory regex: No matches found");
                }

            }
            _innerStringGeo = builder.ToString().TrimEnd(',', ' ') + ")";

        }

        public GeoObject(GeoPoint geoPoint)
        {
            ListGeoPoints = new List<GeoPoint> { geoPoint };
            SetGeoTypePoints(GeoType.Point, new[] { geoPoint.X, geoPoint.Y });
        }
        
        public GeoObject(double latitude, double longitude)
        {
            ListGeoPoints = new List<GeoPoint> { new GeoPoint { X = latitude, Y = longitude } };
            SetGeoTypePoints(GeoType.Point, new[] { latitude, longitude });
        }
       
        public GeoObject(GeoType geoType, List<double[]> points)
        {
            ListGeoPoints = new List<GeoPoint>();
            double[] l = new double[points.Count * 2];
            int i = 0;
            foreach (double[] doubles in points)
            {
                ListGeoPoints.Add(new GeoPoint { Y = doubles[1], X = doubles[0] });
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
      
        public GeoObject(GeoType geoType, params GeoPoint[] points)
        {
            if (geoType == GeoType.PolygonWithHole) throw new Exception("It is forbidden to create a polygon with a hole");
            ListGeoPoints = new List<GeoPoint>();
            double[] l = new double[points.Length * 2];
            int i = 0;
            foreach (GeoPoint doubles in points)
            {
                ListGeoPoints.Add(new GeoPoint { Y = doubles.Y, X = doubles.X });
                l[i] = doubles.X; i++;
                l[i] = doubles.Y; i++;

            }

            SetGeoTypePoints(geoType, l);
        }
      
        public IGeoShape SetSrid(int srid)
        {
            Srid = srid;
            return this;
        }

        public int Srid { get; set; } = 4326;

        public GeoType GeoType { get; set; }

        public string GeoText
        {
            get => _innerStringGeo;
            set
            {
                _innerStringGeo = value;
                var str = value.ToUpper().Trim();
                if (str.EndsWith("EMPTY"))
                {
                    GeoType = GeoType.Empty;
                    ListGeoPoints = UtilsGeo.GetListPoint(GeoType, str, _multiGeoShapes);
                    return;
                }
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

                    if (matches.Count >1)
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

                if (GeoType == GeoType.GeometryCollection)
                {

                }

                ListGeoPoints = UtilsGeo.GetListPoint(GeoType, str, _multiGeoShapes);
            }
        }

        public void SetGeoTypePoints(GeoType type, double[] points)
        {
            if (type == GeoType.None)
                throw new ArgumentException("type not is none");
            if (points == null || points.Length == 0) throw new ArgumentException("point is empty");


            GeoType = type;
            switch (type)
            {
                case GeoType.Point when points.Length != 2:
                    throw new Exception("A point must be defined by two coordinates");
                case GeoType.Point:
                    _innerStringGeo = $"POINT({points[0].ToString(CultureInfo.InvariantCulture)} {points[1].ToString(CultureInfo.InvariantCulture)})";
                    ListGeoPoints.Add(new GeoPoint{X = points[0],Y = points[1] });
                    break;
                case GeoType.LineString:
                    {
                        if (points.Length % 2 != 0)
                            throw new Exception("The number of points defining a line must be even");
                        StringBuilder builder = new StringBuilder("LINESTRING(");
                        for (int i = 0; i < points.Length; i += 2)
                        {
                            builder.Append(
                                $"{points[i].ToString(CultureInfo.InvariantCulture)} {points[i + 1].ToString(CultureInfo.InvariantCulture)}, ");
                            ListGeoPoints.Add(new GeoPoint() { X = points[i], Y = points[i + 1] });

                        }
                        _innerStringGeo = builder.ToString().Trim(' ', ',') + ")";
                        break;
                    }
                case GeoType.Polygon:
                    {

                        if (points.Length % 2 != 0)
                            throw new Exception("The number of points defining a line must be even");
                        StringBuilder builder = new StringBuilder("POLYGON((");
                        for (int i = 0; i < points.Length; i += 2)
                        {

                            builder.Append(
                                $"{points[i].ToString(CultureInfo.InvariantCulture)} {points[i + 1].ToString(CultureInfo.InvariantCulture)}, ");
                            ListGeoPoints.Add(new GeoPoint() { X = points[i], Y = points[i + 1] });

                        }
                        _innerStringGeo = builder.ToString().Trim(' ', ',') + "))";
                        break;

                    }


                case GeoType.None:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);

                case GeoType.MultiPoint:
                    {
                        if (points.Length % 2 != 0)
                            throw new Exception("Number of points to be even");
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

        public List<GeoPoint> ListGeoPoints { get; set; } = new List<GeoPoint>();

        public object GetGeoJson(object properties = null)
        {
            if(GeoType==GeoType.None||GeoType==GeoType.Empty) return null;
            if (GeoType == GeoType.GeometryCollection)
            {
                return new GeoJsonCollection(this, properties);
            }
            return new GeoJson(this, properties);
        }

        public List<IGeoShape> MultiGeoShapes
        {
            get => _multiGeoShapes;
            set => _multiGeoShapes = value;
        }

        public object ArrayCoordinates
        {
            get
            {
                switch (GeoType)
                {
                    case GeoType.None:
                        throw new ArgumentOutOfRangeException();
                    case GeoType.Empty:
                        return Array.Empty<double>();
                    case GeoType.Point:
                        return new[] { ListGeoPoints[0].X, ListGeoPoints[0].Y };
                    case GeoType.LineString:
                        {
                            var s = new object[ListGeoPoints.Count];
                            for (int i = 0; i < ListGeoPoints.Count; i++)
                            {
                                s[i] = new[] { ListGeoPoints[i].X, ListGeoPoints[i].Y };
                            }
                            return s;
                        }


                    case GeoType.MultiPoint:
                        {
                            var s = new object[_multiGeoShapes.Count];
                            for (int i = 0; i < _multiGeoShapes.Count; i++)
                            {
                                s[i] = _multiGeoShapes[i].ArrayCoordinates;
                            }

                            return s;
                        }

                    case GeoType.MultiLineString:
                        {
                            var s = new object[_multiGeoShapes.Count];
                            for (int i = 0; i < _multiGeoShapes.Count; i++)
                            {
                                s[i] = _multiGeoShapes[i].ArrayCoordinates;
                            }

                            return s;

                        }

                    case GeoType.MultiPolygon:
                        {
                            var s = new object[_multiGeoShapes.Count];
                            for (int i = 0; i < _multiGeoShapes.Count; i++)
                            {
                                s[i] = _multiGeoShapes[i].ArrayCoordinates;
                            }

                            return s;

                        }


                    case GeoType.Polygon:
                        {
                            var sp = new object[1];
                            var s = new object[ListGeoPoints.Count];
                            for (int i = 0; i < ListGeoPoints.Count; i++)
                            {
                                s[i] = new[] { ListGeoPoints[i].X, ListGeoPoints[i].Y };
                            }

                            sp[0] = s;
                            return sp;
                        }
                    case GeoType.PolygonWithHole:
                        {
                            var sp = new object[_multiGeoShapes.Count];
                            for (var i = 0; i < _multiGeoShapes.Count; i++)
                            {
                                sp[i] = ((object[])_multiGeoShapes[i].ArrayCoordinates)[0];
                            }
                           
                            return sp;
                        }
                    case GeoType.GeometryCollection:
                    {
                        throw new Exception("geometry GeometryCollection not supported");
                    }

                    default:
                        throw new Exception($"geometry {GeoType} not supported");
                }

            }
            set => throw new NotImplementedException();
        }

       
    }
}