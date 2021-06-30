using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.BulkPayments
{
    public class AATBatchReceiptAcknowledgemet
    {
        public string InstitutionID { get; set; }
        public string ExtReferenceNo { get; set; }
        public string TransactionType { get; set; }
        public string BatchNo { get; set; }
        public string TransactionCode { get; set; }
        public string ResponseCode { get; set; }
        public string Status { get; set; }
        public Response Response { get; set; }       
    }
}