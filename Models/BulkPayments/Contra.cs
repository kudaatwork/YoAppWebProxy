using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.BulkPayments
{
    public class Contra
    {
        public string ClientID { get; set; }
        public string BatchNo { get; set; }
        public string BenAcNo { get; set; }
        public string BenName { get; set; }
        public string CnAmt { get; set; }
        public string BenBank { get; set; }
        public string BankSWIFTCode { get; set; }
        public string BenBranch { get; set; }
        public string RecordNo { get; set; }
        public string Narrative { get; set; }
        public Response Response { get; set; }
        public string PayDet1 { get; set; }
        public string PayDet2 { get; set; }
        public string PayDet3 { get; set; }
        public string PayDet4 { get; set; }
    }
}