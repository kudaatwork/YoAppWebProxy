using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models
{
    public class Merchants
    {
        public string Merchant { get; set; }
        public string Product { get; set; }
        public bool SupportsCustomerInfo { get; set; }
    }    
}