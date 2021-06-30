using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.BulkPayments
{
    public class MST_6_Request
    {
        public string TransactionType { get; set; }
        public string ProcessingCode { get; set; }
        public string QRcodeRef { get; set; }
        public string SysDate { get; set; }
    }
}