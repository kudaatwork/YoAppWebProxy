using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.Wafaya
{
    public class WafayaInitializeRedemptionRequest
    {
        public decimal amount { get; set; }
        public string voucher { get; set; }
        public string token { get; set; }
    }
}