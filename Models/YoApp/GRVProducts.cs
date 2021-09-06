using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.YoApp
{
    public class GRVProducts
    {       
        public string ItemCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }      
        public decimal Quantity { get; set; }
        public string Unit { get; set; }
        public decimal UnitPrice { get; set; }
    }
}