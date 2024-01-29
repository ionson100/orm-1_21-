using System;
using System.Collections.Generic;

namespace ORM_1_21_.geo
{
    /// <summary>
    /// 
    /// </summary>
    public class GeoJson
    {
        /// <summary>
        /// 
        /// </summary>
        public GeoJson()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public GeoJson GetGeoJson(IGeoShape shape, Object properties = null)
        {
            if (shape.GeoType == GeoType.CircularString)
            {
                throw new Exception("'CircularString' geometry type not supported as  GeoJson object");
            }
            this.properties = properties;
            geometry = new Geometry(shape);
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="properties"></param>
        public GeoJson(IGeoShape shape, object properties = null)
        {
            GetGeoJson(shape, properties);
        }
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; } = "Feature";
        /// <summary>
        /// 
        /// </summary>
        public Geometry geometry { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public object properties { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class GeoJsonCollection
    {

        /// <summary>
        /// 
        /// </summary>
        public GeoJsonCollection()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public GeoJsonCollection GetGeoJsonCollection(IGeoShape shape, Object properties = null)
        {
            this.properties = properties;
            foreach (IGeoShape shapeItem in shape.MultiGeoShapes)
            {
                geometries.Add(new Geometry(shapeItem));
            }

            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="properties"></param>
        public GeoJsonCollection(IGeoShape shape, Object properties = null)
        {
            GetGeoJsonCollection(shape, properties);
        }
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; } = "GeometryCollection";
        /// <summary>
        /// 
        /// </summary>
        public List<Geometry> geometries { get; set; } = new List<Geometry>();

        /// <summary>
        /// 
        /// </summary>
        public object properties { get; set; }
    }
}