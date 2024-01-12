using ORM_1_21_.Utils;

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
        /// ctor.
        /// </summary>
        /// <param name="d"></param>
        public GeoPoint(double[] d)
        {
            Check.AsTwoValue(d,nameof(d));
            X = d[0]; Y = d[1];
        }
        /// <summary>
        /// coordinate x
        /// </summary>
        public double X { get; set; }
        /// <summary>
        /// coordinate y
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Get doubles Array as new[] { X, Y };
        /// </summary>
        public double[] GeDoubles()
        {
            return new[] { X, Y };
        }
    }
}