using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.Wafaya
{
    public class Payload
    {
        public Voucher voucher { get; set; }
        public string created_at { get; set; }
        public decimal value { get; set; }
        public bool finalized { get; set; }
        public string batch { get; set; }
        public Product product { get; set; }
    }
}