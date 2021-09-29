using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.Wafaya
{
    public class WafayaFinalizeVoucherResponse
    {
        public string success { get; set; }
        public List<Payload> payload { get; set; }
        public string status { get; set; }
    }
}