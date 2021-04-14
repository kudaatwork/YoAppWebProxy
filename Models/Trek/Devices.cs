using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models
{
    public class Devices
    {
        public long id { get; set; }
        public string imei { get; set; }
        public string status { get; set; }
        public string name { get; set; }
        public string last_use { get; set; }
        public string pos_type { get; set; }
    }
}