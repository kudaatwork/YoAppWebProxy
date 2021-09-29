using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.Wafaya
{
    public class ProductRedemptionLimit
    {
        public Product product { get; set; }
        public string period { get; set; }
        public string created_at { get; set; }
    }
}