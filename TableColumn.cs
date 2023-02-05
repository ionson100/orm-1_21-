using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORM_1_21_
{
    public class TableColumn
    {
        public string ColumnName { get; set; }
        public string ColumnType { get; set; }

        public object DefaultValue { get; set; }

        public bool IsPk { get; set; }
    }
}
