using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.Aqusales
{
    public class TranSourceDetails
    {
        public string tranTable { get; set; }
        public string Column { get; set; }
        public string ColumnType { get; set; }
        public string ColumnValue { get; set; }
    }
}