using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.BulkPayments
{
    public class Response
    {
        public string TrnRefNo { get; set; }
        public string Status { get; set; }
        public string StatusDescription { get; set; }
    }
}