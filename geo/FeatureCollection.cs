using System.Collections.Generic;

namespace ORM_1_21_.geo
{
    /// <summary>
    /// 
    /// </summary>
    public class FeatureCollection
    {
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; } = "FeatureCollection";
        /// <summary>
        /// 
        /// </summary>
        public List<object> features { get; set; } = new List<object>();
    }

}