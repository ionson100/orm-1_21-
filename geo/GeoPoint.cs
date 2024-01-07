namespace ORM_1_21_.geo
{
    /// <summary>
    /// Template point
    /// </summary>
    public class GeoPoint
    {
        /// <summary>
        /// ctor.
        /// </summary>
        public GeoPoint(){}

        /// <summary>
        /// ctor.
        /// </summary>
        /// <param name="x">coordinate x</param>
        /// <param name="y">coordinate y</param>
        public GeoPoint(double x, double y)
        {
            X=x;Y=y;
        }
        /// <summary>
        /// coordinate x
        /// </summary>
        public double X { get; set; }
        /// <summary>
        /// coordinate y
        /// </summary>
        public double Y { get; set; }
    }
}