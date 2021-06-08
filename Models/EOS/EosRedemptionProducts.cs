using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.EOS
{
    public class EosRedemptionProducts
    {
        public Nullable<long> Id { get; set; }
        public Nullable<long> ActionId { get; set; }
        public Nullable<long> ServiceId { get; set; }        
        public string Name { get; set; }        
        public string Description { get; set; }        
        public decimal CollectionAmount { get; set; }
        public string Currency { get; set; }
        public decimal Collected { get; set; }        
    }
}