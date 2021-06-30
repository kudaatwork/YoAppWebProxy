using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.BulkPayments
{
    public class _31_BEQ_1_SuccessResponse
    {
        public string InstitutionID { get; set; }
        public string ExtReferenceNo { get; set; }
        public string TransactionType { get; set; }
        public string accountBranch { get; set; }
        public string accountNumber { get; set; }
        public string accountName { get; set; }
        public string balance { get; set; }
        public BEQResponse Response { get; set; }
    }
}