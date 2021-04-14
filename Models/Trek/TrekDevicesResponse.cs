using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models
{
    public class TrekDevicesResponse
    {
        public string status { get; set; }
        public string message { get; set; }
        public List<Devices> data { get; set; }
    }
}