using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.BulkPayments
{
    public class MSTRequest
    {
        public string TransactionType { get; set; }
        public string ProcessingCode { get; set; }
        public string AccountBranch { get; set; }
        public string AccountNumber { get; set; }
        public string Range { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Email { get; set; }
    }
}