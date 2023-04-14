using System.Collections.Generic;
using ORM_1_21_.Linq;

namespace ORM_1_21_
{
    class WrapperVisitor
    {
        public List<OneComposite> ListOneComposite { get; set; } = new List<OneComposite>();
        public Dictionary<string, object> Params { get; set; } = new Dictionary<string, object>();
        public int ParamsIndex { get; set; }
        public string FieldName { get; set; }
    }
}