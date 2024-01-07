using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ORM_1_21_.geo
{
    /// <summary>
    /// 
    /// </summary>
    public static class FactoryGeo
    {
        #region Point

        /// <summary>
        /// Create geo object Point
        /// </summary>
        /// <param name="x">Coordinate x</param>
        /// <param name="y">Coordinate y</param>
        /// <returns>Geo Object Point</returns>
        public static IGeoShape CreatePoint(double x, double y)
        {
            return new GeoObject(x, y);
        }
        /// <summary>
        /// Create geo object Point
        /// </summary>
        /// <param name="point">Template point</param>
        /// <returns>Geo Object Point</returns>
        public static IGeoShape CreatePoint(GeoPoint point)
        {
            return new GeoObject(point);
        }
        /// <summary>
        /// Create geo object Point
        /// </summary>
        /// <param name="str">String the OGC Well-Known Text (WKT) representation of the geometry</param>
        /// <returns>Geo Object Point</returns>
        public static IGeoShape CreatePoint(string str)
        {
            return new GeoObject(str);
        }





        #endregion


        #region MultiPoint 

        /// <summary>
        /// Create geo object MultiPoint
        /// </summary>
        /// <param name="d">array double values</param>
        /// <returns>Geo Object MultiPoint</returns>
        public static IGeoShape CreateMultiPoint(params double[] d)
        {
            return new GeoObject(GeoType.MultiPoint, d);
        }

        /// <summary>
        /// Create geo object MultiPoint
        /// </summary>
        /// <param name="str">String the OGC Well-Known Text (WKT) representation of the geometry</param>
        /// <returns>Geo Object MultiPoint</returns>
        public static IGeoShape CreateMultiPoint(string str)
        {
            return new GeoObject(str);
        }

        /// <summary>
        /// Create geo object MultiPoint
        /// </summary>
        /// <param name="p">Array template points</param>
        /// <returns>Geo Object MultiPoint</returns>
        public static IGeoShape CreateMultiPoint(params GeoPoint[] p)
        {
            return new GeoObject(GeoType.MultiPoint, p);
        }
        /// <summary>
        /// Create geo object MultiPoint
        /// </summary>
        /// <param name="p">Array Geo Objects</param>
        /// <returns>Geo Object MultiPoint</returns>
        public static IGeoShape CreateMultiPoint(params IGeoShape[] p)
        {
            return new GeoObject(GeoType.MultiPoint, p);
        }

        #endregion



        #region Polygon

        /// <summary>
        /// Create geo object Polygon
        /// </summary>
        /// <param name="d">Array double values</param>
        /// <returns>Geo Object Polygon</returns>
        public static IGeoShape CreatePolygon(params double[] d)
        {
            return new GeoObject(GeoType.Polygon, d);
        }
        /// <summary>
        /// Create geo object Polygon
        /// </summary>
        /// <param name="str">String the OGC Well-Known Text (WKT) representation of the geometry</param>
        /// <returns>Geo Object Polygon</returns>
        public static IGeoShape CreatePolygon(string str)
        {
            return new GeoObject(str);
        }

        /// <summary>
        /// Create geo object Polygon
        /// </summary>
        /// <param name="p">Array template points</param>
        /// <returns>Geo Object Polygon</returns>
        public static IGeoShape CreatePolygon(params GeoPoint[] p)
        {
            return new GeoObject(GeoType.Polygon, p);
        }

        #endregion

        #region MultiPolygon


        /// <summary>
        /// Create geo object MultiPolygon
        /// </summary>
        /// <param name="str">String the OGC Well-Known Text (WKT) representation of the geometry</param>
        /// <returns>Geo object MultiPolygon</returns>
        public static IGeoShape CreateMultiPolygon(string str)
        {
            return new GeoObject(str);
        }

        /// <summary>
        /// Create geo object MultiPolygon
        /// </summary>
        /// <param name="shape">Array geo objects</param>
        /// <returns>Geo object MultiPolygon</returns>
        public static IGeoShape CreateMultiPolygon(params IGeoShape[] shape)
        {
            return new GeoObject(GeoType.MultiPolygon, shape);
        }

        #endregion


        #region LineString

        /// <summary>
        /// Create geo object LineString
        /// </summary>
        /// <param name="d">Array double values</param>
        /// <returns>Geo object LineString</returns>
        public static IGeoShape CreateLineString(params double[] d)
        {
            return new GeoObject(GeoType.LineString, d);
        }

        /// <summary>
        /// Create geo object LineString
        /// </summary>
        /// <param name="str">String the OGC Well-Known Text (WKT) representation of the geometry</param>
        /// <returns>Geo object LineString</returns>
        public static IGeoShape CreateLineString(string str)
        {
            return new GeoObject(str);
        }

        /// <summary>
        /// Create geo object LineString
        /// </summary>
        /// <param name="p">Array template points</param>
        /// <returns>Geo object LineString</returns>
        public static IGeoShape CreateLineString(params GeoPoint[] p)
        {
            return new GeoObject(GeoType.LineString,p);
        }

        #endregion

        #region MultiLineString

        /// <summary>
        /// Create geo object MultiLineString
        /// </summary>
        /// <param name="str">String the OGC Well-Known Text (WKT) representation of the geometry</param>
        /// <returns>Geo object MultiLineString</returns>
        public static IGeoShape CreateMultiLineString(string str)
        {
            return new GeoObject(str);
        }

        /// <summary>
        /// Create geo object MultiLineString
        /// </summary>
        /// <param name="shapes">Array geo objects</param>
        /// <returns>Geo object MultiLineString</returns>
        public static IGeoShape CreateMultiLineString(params IGeoShape[] shapes)
        {
            return new GeoObject(GeoType.MultiLineString,shapes);
        }

        #endregion

        #region PolygonWithHole

        /// <summary>
        /// Create geo object PolygonWithHole
        /// </summary>
        /// <param name="str">String the OGC Well-Known Text (WKT) representation of the geometry</param>
        /// <returns>Geo object PolygonWithHole</returns>
        public static IGeoShape CreatePolygonWithHole(string str)
        {
            return new GeoObject(str);
        }
        /// <summary>
        /// Create geo object PolygonWithHole
        /// </summary>
        /// <param name="shape1">Object Parent</param>
        /// <param name="shape2">Object Hole</param>
        /// <returns>Geo object PolygonWithHole</returns>
        public static IGeoShape CreatePolygonWithHole(IGeoShape shape1,IGeoShape shape2)
        {
            return GeoObject.CreateGeoPolygonWithHole(shape1, shape2);
        }

        /// <summary>
        /// Create geo object PolygonWithHole
        /// </summary>
        /// <param name="d1">Array double values paren object</param>
        /// <param name="d2">Array double values hole object</param>
        /// <returns>Geo object PolygonWithHole</returns>
        public static IGeoShape CreatePolygonWithHole(double[] d1, double[] d2)
        {
            return GeoObject.CreateGeoPolygonWithHole(d1, d2);
        }

        #endregion

        #region GeometryCollection

        /// <summary>
        /// Create geo object GeometryCollection
        /// </summary>
        /// <param name="shape">Array geo objects</param>
        /// <returns>Geo object GeometryCollection</returns>
        public static IGeoShape CreateGeometryCollection(params IGeoShape[] shape)
        {
            return new GeoObject(GeoType.GeometryCollection, shape);
        }

        /// <summary>
        /// Create geo object GeometryCollection
        /// </summary>
        /// <param name="str">String the OGC Well-Known Text (WKT) representation of the geometry</param>
        /// <returns>Geo object GeometryCollection</returns>
        public static IGeoShape CreateGeometryCollection(string str)
        {
            return new GeoObject(str);
        }

        #endregion



        /// <summary>
        /// Create geo object any type
        /// </summary>
        /// <param name="str">String the OGC Well-Known Text (WKT) representation of the geometry</param>
        /// <returns>Пeo object</returns>
        public static IGeoShape CreateGeo(string str)
        {
            ValidateString(str);
            return new GeoObject(str);
        }

        /// <summary>
        /// Create geo object 
        /// </summary>
        /// <param name="geoType">Type geo object</param>
        /// <param name="ds">List arrays double values (array size=2 as coordinate point (x,y)</param>
        /// <returns>Geo type</returns>
        public static IGeoShape CreateGeo(GeoType geoType, List<double[]> ds)
        {
            return new GeoObject(geoType, ds);
        }

        /// <summary>
        /// Return List geo objects from string GeoJson
        /// </summary>
        /// <param name="geoJson">string GeoJson</param>
        /// <returns>List geo objects</returns>
        public static List<IGeoShape> GetGeometryFromGeoJson(string geoJson)
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
                    list.AddRange(InnerPaceGeoJson(geoText, geo));
                }
                return list;

            }

            

        }

        private static List<IGeoShape> InnerPaceGeoJson(string geoText, JArray geo)
        {

            switch (geoText)
            {
                case "POINT":
                    {

                        double[] t = geo.Value<JArray>().ToObject<double[]>();
                        {

                            return new List<IGeoShape> { FactoryGeo.CreatePoint(t[0], t[1]) };

                        }
                    }
                case "MULTIPOINT":
                    {
                        var t = geo.Value<JArray>().ToObject<List<double[]>>();
                        {
                            return new List<IGeoShape> { FactoryGeo.CreateGeo(GeoType.MultiPoint, t) };

                        }
                    }
                case "LINESTRING":
                    {
                        var t = geo.Value<JArray>().ToObject<List<double[]>>();
                        {
                            return new List<IGeoShape> { FactoryGeo.CreateGeo(GeoType.LineString, t) };

                        }
                    }

                case "MULTILINESTRING":
                    {
                        var t = geo.Value<JArray>().ToObject<List<List<double[]>>>();
                        List<IGeoShape> list = new List<IGeoShape>();
                        foreach (List<double[]> d in t)
                        {
                            list.Add(FactoryGeo.CreateGeo(GeoType.LineString, d));
                        }

                        {
                            return new List<IGeoShape> { FactoryGeo.CreateMultiLineString(list.ToArray()) };

                        }
                    }

                case "POLYGON":
                    {
                        List<List<double[]>> t = geo.Value<JArray>().ToObject<List<List<double[]>>>();
                        {
                            if (t.Count == 1)
                            {
                                return new List<IGeoShape> { FactoryGeo.CreateGeo(GeoType.Polygon, t.First()) };
                            }

                            List<double> list = new List<double>();
                            foreach (double[] d in t[0])
                            {
                                list.Add(d[0]);
                                list.Add(d[1]);
                            }
                            List<double> list2 = new List<double>();
                            foreach (double[] d in t[1])
                            {
                                list2.Add(d[0]);
                                list2.Add(d[1]);
                            }
                            return new List<IGeoShape> { FactoryGeo.CreatePolygonWithHole(list.ToArray(), list2.ToArray()) };


                        }
                    }

                case "MULTIPOLYGON":
                    {
                        var t = geo.Value<JArray>().ToObject<List<List<List<double[]>>>>();
                        List<IGeoShape> list = new List<IGeoShape>();
                        foreach (List<List<double[]>> list1 in t)
                        {
                            foreach (List<double[]> d in t.First())
                            {
                                list.Add(FactoryGeo.CreateGeo(GeoType.Polygon, d));
                            }
                        }


                        {

                            return new List<IGeoShape> { FactoryGeo.CreateMultiPolygon(list.ToArray()) };

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
        public static List<string> ParseGeoCollection(string geoCollection)
        {
           
            ValidateString(geoCollection);

            int s = geoCollection.IndexOf('(') + 1;
            int f = geoCollection.LastIndexOf(')');
            int d = f - s;
            geoCollection = geoCollection.Substring(s, d);
            List<string> list = new List<string>();
            int count = 0;
            StringBuilder sb = new StringBuilder();
            foreach (char c in geoCollection)
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
