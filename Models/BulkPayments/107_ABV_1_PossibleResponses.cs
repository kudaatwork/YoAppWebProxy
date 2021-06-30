using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.BulkPayments
{
    public class _107_ABV_1_PossibleResponses
    {
        public string InstitutionID { get; set; }
        public string ExtReferenceNo { get; set; }
        public string TransactionType { get; set; }
        public string accountBranch { get; set; }
        public string accountNumber { get; set; }
        public string accountName { get; set; }
        public string customerNumber { get; set; }
        public string currency { get; set; }
        public ABVResponse Response { get; set; }
    }
}