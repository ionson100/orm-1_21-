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
        /// 
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public static IGeoShape CreatePoint(double latitude, double longitude)
        {
            return new GeoObject(latitude, longitude);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static IGeoShape CreatePoint(GeoPoint point)
        {
            return new GeoObject(point);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static IGeoShape CreatePoint(string str)
        {
            return new GeoObject(str);
        }

        



        #endregion


        #region MultiPoint

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static IGeoShape CreateMultiPoint(params double[] d)
        {
            return new GeoObject(GeoType.MultiPoint, d);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static IGeoShape CreateMultiPoint(string str)
        {
            return new GeoObject(str);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static IGeoShape CreateMultiPoint(params GeoPoint[] p)
        {
            return new GeoObject(GeoType.MultiPoint, p);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static IGeoShape CreateMultiPoint(params IGeoShape[] p)
        {
            return new GeoObject(GeoType.MultiPoint, p);
        }

        #endregion



        #region Polygon

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static IGeoShape CreatePolygon(params double[] d)
        {
            return new GeoObject(GeoType.Polygon, d);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static IGeoShape CreatePolygon(string str)
        {
            return new GeoObject(str);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static IGeoShape CreatePolygon(params GeoPoint[] p)
        {
            return new GeoObject(GeoType.Polygon, p);
        }

        #endregion

        #region MultiPolygon

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static IGeoShape CreateMultiPolygon(string str)
        {
            return new GeoObject(str);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        /// <returns></returns>
        public static IGeoShape CreateMultiPolygon(params IGeoShape[] shape)
        {
            return new GeoObject(GeoType.MultiPolygon, shape);
        }

        #endregion


        #region LineString

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static IGeoShape CreateLineString(params double[] d)
        {
            return new GeoObject(GeoType.LineString, d);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static IGeoShape CreateLineString(string str)
        {
            return new GeoObject(str);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static IGeoShape CreateLineString(params GeoPoint[] p)
        {
            return new GeoObject(GeoType.LineString,p);
        }

        #endregion

        #region MultiLineString

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static IGeoShape CreateMultiLineString(string str)
        {
            return new GeoObject(str);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shapes"></param>
        /// <returns></returns>
        public static IGeoShape CreateMultiLineString(params IGeoShape[] shapes)
        {
            return new GeoObject(GeoType.MultiLineString,shapes);
        }

        #endregion

        #region PolygonWithHole

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static IGeoShape CreatePolygonWithHole(string str)
        {
            return new GeoObject(str);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape1"></param>
        /// <param name="shape2"></param>
        /// <returns></returns>
        public static IGeoShape CreatePolygonWithHole(IGeoShape shape1,IGeoShape shape2)
        {
            return GeoObject.CreateGeoPolygonWithHole(shape1, shape2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <returns></returns>
        public static IGeoShape CreatePolygonWithHole(double[] d1, double[] d2)
        {
            return GeoObject.CreateGeoPolygonWithHole(d1, d2);
        }

        #endregion

        #region GeometryCollection

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        /// <returns></returns>
        public static IGeoShape CreateGeometryCollection(params IGeoShape[] shape)
        {
            return new GeoObject(GeoType.GeometryCollection, shape);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static IGeoShape CreateGeometryCollection(string str)
        {
            return new GeoObject(str);
        }

        #endregion



        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static IGeoShape CreateGeo(string str)
        {
            return new GeoObject(str);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geoType"></param>
        /// <param name="ds"></param>
        /// <returns></returns>
        public static IGeoShape CreateGeo(GeoType geoType, List<double[]> ds)
        {
            return new GeoObject(geoType, ds);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geoJson"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        public static List<IGeoShape> GetGeometryFromGeoJson(string geoJson)
        {
            if (string.IsNullOrWhiteSpace(geoJson)) throw new ArgumentException("param geoJson is empty");
            if (!geoJson.ToUpper().Contains("GeometryCollection".ToUpper()))
            {
                ORM_1_21_.geo.GeoJson json = JsonConvert.DeserializeObject<GeoJson>(geoJson);
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

            throw new Exception("Не поддерживается");
        }




    }
}
