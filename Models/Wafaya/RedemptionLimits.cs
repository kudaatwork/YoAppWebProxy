using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.Wafaya
{
    public class RedemptionLimits
    {
        public string period { get; set; }
        public decimal amount { get; set; }
        public string created_at { get; set; }
    }
}