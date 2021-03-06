using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.BulkPayments
{
    public class AATBatchProgressResponse
    {
        public string InstitutionID { get; set; }
        public string ExtReferenceNo { get; set; }
        public string ProcessingCode { get; set; }
        public string ClientID { get; set; }
        public string BatchNo { get; set; }
        public string TransactionType { get; set; }
        public string TransactionCode { get; set; }
        public string SourceAcNo { get; set; }
        public string Amt { get; set; }
        public string CCY { get; set; }
        public string Narrative { get; set; }
        public string DrCr { get; set; }
        public string Status { get; set; }
        public Response Response { get; set; }
        public Contra Contra { get; set; }
    }
}