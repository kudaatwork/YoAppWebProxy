using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models
{
    public class Product
    {
        public string ProductRedeemed { get; set; }
        public decimal QuantityRedeemed { get; set; }
        public decimal PricePerUnit { get; set; }
    }
}