using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models
{
    public class AgribankPaymentRequest
    {
        public string batch_reference { get; set; }
        public string batch_currency { get; set; }
        public string destination_acc { get; set; }
    }
}