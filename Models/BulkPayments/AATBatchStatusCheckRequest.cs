using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.BulkPayments
{
    public class AATBatchStatusCheckRequest
    {
        public string InstitutionID { get; set; }
        public string ExtReferenceNo { get; set; }
        public string ProcessingCode { get; set; }
    }
}