using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.Wafaya
{
    public class RedemptionLimits
    {
        public string period { get; set; }
        public int amount { get; set; }
        public DateTime created_at { get; set; }
    }
}