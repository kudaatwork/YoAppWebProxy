using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.YoApp
{
    public class Payments
    {
        public string merchantCode { get; set; }
        public string successURL { get; set; }
        public string failedURL { get; set; }
        public string ReceiptNo { get; set; }
        public string custmerEmail { get; set; }
        public string transactionAmount { get; set; }
        public string paymentCurrency { get; set; }
        public string transactionDescription { get; set; }
        public string paymentref { get; set; }
        public string paymentType { get; set; }
        public string status { get; set; }
        public string hash { get; set; }
    }
}