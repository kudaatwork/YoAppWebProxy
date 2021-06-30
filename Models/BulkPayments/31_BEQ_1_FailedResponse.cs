using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.BulkPayments
{
    public class _31_BEQ_1_FailedResponse
    {
        public string InstitutionID { get; set; }
        public string ExtReferenceNo { get; set; }
        public string TransactionType { get; set; }
        public BEQResponse Response { get; set; }
    }
}