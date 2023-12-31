using System.Collections.Generic;

namespace ORM_1_21_.geo
{
    /// <summary>
    /// 
    /// </summary>
    public interface IGeoShape
    {

        /// <summary>
        /// 
        /// </summary>
        int Srid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        GeoType GeoType { get; }


        /// <summary>
        /// 
        /// </summary>
        string GeoData { get; set; }

       // /// <summary>
       // /// 
       // /// </summary>
       // /// <param name="type"></param>
       // /// <param name="points"></param>
       // void SetGeoTypePoints(GeoType type, double[] points);

        /// <summary>
        /// 
        /// </summary>
        List<GeoPoint> ListGeoPoints { get; }

        /// <summary>
        /// 
        /// </summary>
        object GetGeoJson(object properties=null);

        /// <summary>
        /// 
        /// </summary>
        List<IGeoShape> MultiGeoShapes { get; }



        /// <summary>
        /// 
        /// </summary>
        object ArrayCoordinate { get;  }


    }
}
