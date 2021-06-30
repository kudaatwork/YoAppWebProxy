using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.BulkPayments
{
    public class BEQRequest
    {
        public string TransactionType { get; set; }
        public string ProcessingCode { get; set; }
        public string AccountNumber { get; set; }
    }
}