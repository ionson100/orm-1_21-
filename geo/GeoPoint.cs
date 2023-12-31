namespace ORM_1_21_.geo
{
    /// <summary>
    /// 
    /// </summary>
    public class GeoPoint
    {
        /// <summary>
        /// 
        /// </summary>
        public GeoPoint(){}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        public GeoPoint(double latitude, double longitude)
        {
            Latitude=latitude;Longitude=longitude;
        }
        /// <summary>
        /// 
        /// </summary>
        public double Latitude { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double Longitude { get; set; }
    }
}