using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ORM_1_21_.Utils;


namespace ORM_1_21_.geo
{
    /// <summary>
    /// 
    /// </summary>
    public static class FactoryGeo
    {

        /// <summary>
        /// 
        /// </summary>
        public static int DefaultSrid { get; set; } = 4326;
        #region Point

        /// <summary>
        /// Create geo object Point
        /// </summary>
        /// <param name="x">Coordinate x</param>
        /// <param name="y">Coordinate y</param>
        /// <returns>Geo Object Point</returns>
        public static IGeoShape Point(double x, double y)
        {
            return new GeoObject(x, y).SetSrid(DefaultSrid);
        }

        /// <summary>
        /// Create geo object Point
        /// </summary>
        /// <param name="point">Template point</param>
        /// <returns>Geo Object Point</returns>
        public static IGeoShape Point(GeoPoint point)
        {
            Check.NotNull(point, nameof(point));
            return new GeoObject(point).SetSrid(DefaultSrid);
        }
        /// <summary>
        /// Create geo object Point
        /// </summary>
        /// <param name="str">String the OGC Well-Known Text (WKT) representation of the geometry</param>
        /// <returns>Geo Object Point</returns>
        public static IGeoShape Point(string str)
        {
            ValidateString(str);
            return new GeoObject(str).SetSrid(DefaultSrid);
        }





        #endregion

        #region MultiPoint 

        /// <summary>
        /// Create geo object MultiPoint
        /// </summary>
        /// <param name="d">array double values</param>
        /// <returns>Geo Object MultiPoint</returns>
        public static IGeoShape MultiPoint(params double[] d)
        {
            return new GeoObject(GeoType.MultiPoint, d).SetSrid(DefaultSrid);
        }

        /// <summary>
        /// Create geo object MultiPoint
        /// </summary>
        /// <param name="str">String the OGC Well-Known Text (WKT) representation of the geometry</param>
        /// <returns>Geo Object MultiPoint</returns>
        public static IGeoShape MultiPoint(string str)
        {
            ValidateString(str);
            return new GeoObject(str).SetSrid(DefaultSrid);
        }

        /// <summary>
        /// Create geo object MultiPoint
        /// </summary>
        /// <param name="p">Array template points</param>
        /// <returns>Geo Object MultiPoint</returns>
        public static IGeoShape MultiPoint(params GeoPoint[] p)
        {
            return new GeoObject(GeoType.MultiPoint, p).SetSrid(DefaultSrid);
        }
        /// <summary>
        /// Create geo object MultiPoint
        /// </summary>
        /// <param name="shapes">Array Geo Objects</param>
        /// <returns>Geo Object MultiPoint</returns>
        public static IGeoShape MultiPoint(params IGeoShape[] shapes)
        {
            ValidateArrayIGeoShape(shapes, nameof(shapes),GeoType.Point);
            return new GeoObject(GeoType.MultiPoint, shapes).SetSrid(DefaultSrid);
        }

        #endregion

        #region Polygon

        /// <summary>
        /// Create geo object Polygon
        /// </summary>
        /// <param name="d">Array double values</param>
        /// <returns>Geo Object Polygon</returns>
        public static IGeoShape Polygon(params double[] d)
        {
            return new GeoObject(GeoType.Polygon, d).SetSrid(DefaultSrid);
        }
        /// <summary>
        /// Create geo object Polygon
        /// </summary>
        /// <param name="str">String the OGC Well-Known Text (WKT) representation of the geometry</param>
        /// <returns>Geo Object Polygon</returns>
        public static IGeoShape Polygon(string str)
        {
            ValidateString(str);
            return new GeoObject(str).SetSrid(DefaultSrid);
        }

        /// <summary>
        /// Create geo object Polygon
        /// </summary>
        /// <param name="p">Array template points</param>
        /// <returns>Geo Object Polygon</returns>
        public static IGeoShape Polygon(params GeoPoint[] p)
        {
            return new GeoObject(GeoType.Polygon, p).SetSrid(DefaultSrid);
        }

        #endregion

        #region MultiPolygon


        /// <summary>
        /// Create geo object MultiPolygon
        /// </summary>
        /// <param name="str">String the OGC Well-Known Text (WKT) representation of the geometry</param>
        /// <returns>Geo object MultiPolygon</returns>
        public static IGeoShape MultiPolygon(string str)
        {
            ValidateString(str);
            return new GeoObject(str).SetSrid(DefaultSrid);
        }

        /// <summary>
        /// Create geo object MultiPolygon
        /// </summary>
        /// <param name="shapes">Array geo objects</param>
        /// <returns>Geo object MultiPolygon</returns>
        public static IGeoShape MultiPolygon(params IGeoShape[] shapes)
        {
            ValidateArrayIGeoShape(shapes, nameof(shapes), GeoType.Polygon);
            return new GeoObject(GeoType.MultiPolygon, shapes).SetSrid(DefaultSrid);
        }

        #endregion

        #region LineString

        /// <summary>
        /// Create geo object LineString
        /// </summary>
        /// <param name="d">Array double values</param>
        /// <returns>Geo object LineString</returns>
        public static IGeoShape LineString(params double[] d)
        {
            return new GeoObject(GeoType.LineString, d).SetSrid(DefaultSrid);
        }

        /// <summary>
        /// Create geo object LineString
        /// </summary>
        /// <param name="str">String the OGC Well-Known Text (WKT) representation of the geometry</param>
        /// <returns>Geo object LineString</returns>
        public static IGeoShape LineString(string str)
        {
            ValidateString(str);
            return new GeoObject(str).SetSrid(DefaultSrid);
        }

        /// <summary>
        /// Create geo object LineString
        /// </summary>
        /// <param name="p">Array template points</param>
        /// <returns>Geo object LineString</returns>
        public static IGeoShape LineString(params GeoPoint[] p)
        {
            return new GeoObject(GeoType.LineString,p).SetSrid(DefaultSrid);
        }

        #endregion

        #region MultiLineString

        /// <summary>
        /// Create geo object MultiLineString
        /// </summary>
        /// <param name="str">String the OGC Well-Known Text (WKT) representation of the geometry</param>
        /// <returns>Geo object MultiLineString</returns>
        public static IGeoShape MultiLineString(string str)
        {
            ValidateString(str);
            return new GeoObject(str).SetSrid(DefaultSrid);
        }

        /// <summary>
        /// Create geo object MultiLineString
        /// </summary>
        /// <param name="shapes">Array geo objects</param>
        /// <returns>Geo object MultiLineString</returns>
        public static IGeoShape MultiLineString(params IGeoShape[] shapes)
        {
            ValidateArrayIGeoShape(shapes, nameof(shapes), GeoType.LineString);
            return new GeoObject(GeoType.MultiLineString,shapes).SetSrid(DefaultSrid);
        }

        #endregion

        #region PolygonWithHole

        /// <summary>
        /// Create geo object PolygonWithHole
        /// </summary>
        /// <param name="str">String the OGC Well-Known Text (WKT) representation of the geometry</param>
        /// <returns>Geo object PolygonWithHole</returns>
        public static IGeoShape PolygonWithHole(string str)
        {
            ValidateString(str);
            return new GeoObject(str).SetSrid(DefaultSrid);
        }
       


        /// <summary>
        /// Create geo object PolygonWithHole
        /// </summary>
        /// <param name="p">Object Parent</param>
        /// <param name="holes">Objects Hole</param>
        /// <returns></returns>
        public static IGeoShape PolygonWithHole(IGeoShape p, params IGeoShape[] holes)
        {
            if (p.GeoType != GeoType.Polygon) throw new ArgumentException("Params p is not type Polygon");
            ValidateArrayIGeoShape(holes, nameof(holes), GeoType.Polygon);
            return GeoObject.CreateGeoPolygonWithHole(p, holes).SetSrid(DefaultSrid);
        }


        /// <summary>
        /// Create geo object PolygonWithHole
        /// </summary>
        /// <param name="p">Array double values paren object</param>
        /// <param name="holes">Array double values hole objects</param>
        /// <returns></returns>
        public static IGeoShape PolygonWithHole(double[] p, params double[][] holes)
        {
            List<double[]> list = new List<double[]>(p.Length+1) { p };
            list.AddRange(holes);

            return GeoObject.CreateGeoPolygonWithHole(list.ToArray()).SetSrid(FactoryGeo.DefaultSrid);
        }

        private static void ValidateArrayIGeoShape(IGeoShape[] par,string paramName, GeoType type)
        {
            foreach (IGeoShape geoShape in par)
            {
                if (geoShape.GeoType != type)
                {
                    throw new ArgumentException($"Invalid object type for parameter{paramName}");
                }
            }
        }

        #endregion

        #region GeometryCollection

        /// <summary>
        /// Create geo object GeometryCollection
        /// </summary>
        /// <param name="shape">Array geo objects</param>
        /// <returns>Geo object GeometryCollection</returns>
        public static IGeoShape GeometryCollection(params IGeoShape[] shape)
        {
            return new GeoObject(GeoType.GeometryCollection, shape).SetSrid(DefaultSrid);
        }

        /// <summary>
        /// Create geo object GeometryCollection
        /// </summary>
        /// <param name="str">String the OGC Well-Known Text (WKT) representation of the geometry</param>
        /// <returns>Geo object GeometryCollection</returns>
        public static IGeoShape GeometryCollection(string str)
        {
            ValidateString(str);
            return new GeoObject(str).SetSrid(DefaultSrid);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IGeoShape Empty(GeoType type)
        {
            if (type == GeoType.Empty) throw new ArgumentException($"Type:{type} cannot be used");
            return new GeoObject($"{type} empty".ToUpper()).SetSrid(DefaultSrid);
        }

        #endregion


        /// <summary>
        /// Create geo object any type from text
        /// </summary>
        /// <param name="str">String the OGC Well-Known Text (WKT) representation of the geometry</param>
        /// <returns>Пeo object</returns>
        public static IGeoShape CreateGeo(string str)
        {
           
            ValidateString(str);
            return new GeoObject(str).SetSrid(DefaultSrid);
        }

        /// <summary>
        /// Create geo object
        /// </summary>
        /// <param name="geoType">Type geo object</param>
        /// <param name="ds">List arrays double values (array size=2 as coordinate point (x,y)</param>
        /// <returns>Geo type</returns>
        public static IGeoShape CreateGeo(GeoType geoType, List<double[]> ds)
        {
            return new GeoObject(geoType, ds).SetSrid(DefaultSrid);
        }

        /// <summary>
        /// Return List geo objects from string GeoJson
        /// </summary>
        /// <param name="geoJson">string GeoJson</param>
        /// <returns>List geo objects</returns>
        public static IGeoShape GetGeometryFromGeoJson(string geoJson)
        {
            if (string.IsNullOrWhiteSpace(geoJson)) throw new ArgumentException("param geoJson is empty");
            if (!geoJson.ToUpper().Contains("GeometryCollection".ToUpper()))
            {
                GeoJson json = JsonConvert.DeserializeObject<GeoJson>(geoJson);
                if (json == null) throw new ArgumentException("geoJson parse error");
                JArray geo = (JArray)json.geometry.coordinates;
                string geoText = json.geometry.type.ToUpper();
                return InnerPaceGeoJson(geoText, geo);
            }
            else
            {
                GeoJsonCollection json = JsonConvert.DeserializeObject<GeoJsonCollection>(geoJson);
                if (json == null) throw new ArgumentException("geoJson parse error");
                List<Geometry> geoList = json.geometries;
                List<IGeoShape> list = new List<IGeoShape>();
                foreach (var geometry in geoList)
                {
                    string geoText = geometry.type.ToUpper();
                    JArray geo = (JArray)geometry.coordinates;
                    list.Add(InnerPaceGeoJson(geoText, geo));
                }

                var res = GeometryCollection(list.ToArray());
                return res;

            }

            

        }

        private static IGeoShape InnerPaceGeoJson(string geoText, JArray geo)
        {

            switch (geoText)
            {
                case "POINT":
                    {
                        double[] t = geo.Value<JArray>()?.ToObject<double[]>();
                        {
                            return Point(t[0], t[1]);
                        }
                    }
                case "MULTIPOINT":
                    {
                        List<double[]> t = geo.Value<JArray>()?.ToObject<List<double[]>>();
                        {
                           
                            return CreateGeo(GeoType.MultiPoint, t);
                        }
                    }
                case "LINESTRING":
                    {
                        var t = geo.Value<JArray>()?.ToObject<List<double[]>>();
                        {
                            return CreateGeo(GeoType.LineString, t);
                        }
                    }

                case "MULTILINESTRING":
                    {
                        var t = geo.Value<JArray>()?.ToObject<List<List<double[]>>>();
                        List<IGeoShape> list = new List<IGeoShape>();
                        foreach (List<double[]> d in t)
                        {
                            list.Add(CreateGeo(GeoType.LineString, d));
                        }

                        {
                            return MultiLineString(list.ToArray());

                        }
                    }

                case "POLYGON":
                    {
                        List<List<double[]>> t = geo.Value<JArray>()?.ToObject<List<List<double[]>>>();
                        {
                            if (t.Count == 1)
                            {
                                return CreateGeo(GeoType.Polygon, t.First());
                            }

                            List<IGeoShape> listE = new List<IGeoShape>();
                            foreach (var t1 in t)
                            {
                                listE.Add(CreateGeo(GeoType.Polygon, t1));
                            }

                            var holeP = PolygonWithHole(listE[0], listE.Skip(1).ToArray());
                            return holeP;
                        }
                    }

                case "MULTIPOLYGON":
                    {
                        var t = geo.Value<JArray>()?.ToObject<List<List<List<double[]>>>>();
                        List<IGeoShape> list = new List<IGeoShape>();
                        foreach (List<List<double[]>> list1 in t)
                        {
                            foreach (List<double[]> d in t.First())
                            {
                                list.Add(CreateGeo(GeoType.Polygon, d));
                            }
                        }
                        {
                            return MultiPolygon(list.ToArray());
                        }
                    }
            }

            throw new Exception($"Not supported : {geoText}");
        }

        /// <summary>
        /// Return list String the OGC Well-Known Text (WKT) representation of the geometry
        /// </summary>
        /// <param name="geoCollection"> String GeometryCollection as String the OGC Well-Known Text (WKT) representation of the geometry</param>
        /// <returns>list String</returns>
        internal static List<string> ParseGeoCollection(string geoCollection)
        {
           
            ValidateString(geoCollection);

            var s = geoCollection.IndexOf('(') + 1;
            var f = geoCollection.LastIndexOf(')');
            var d = f - s;
            geoCollection = geoCollection.Substring(s, d);
            var list = new List<string>();
            var count = 0;
            var sb = new StringBuilder();
            foreach (var c in geoCollection)
            {
                if (c == '\n') continue;
                sb.Append(c);
                if (c == '(') count++;
                if (c == ')') count--;
                if (count == 0 && c == ',')
                {
                    list.Add(sb.ToString().Trim(','));
                    sb.Clear();
                }



            }
            list.Add(sb.ToString().Trim(','));
            return list;
        }

        internal static void ValidateString(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                throw new ArgumentException("Argument string geo object is empty");
            }
            int col = 0;
            foreach (var c in str.ToCharArray())
            {
                if (c == '(') col++;
                if (c == ')') col--;
            }

            if (col != 0) throw new Exception($"String not correct: {str}");
        }

        

    }
}
