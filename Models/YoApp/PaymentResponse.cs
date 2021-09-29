using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.YoApp
{
    public class PaymentResponse
    {
        public string dynamicQRBase64 { set; get; }
        public string dynamicQR { set; get; }
        public string successURL { set; get; }
        public string failedURL { set; get; }
        public string responseCode { set; get; }
        public string responseMessage { set; get; }
        public string transactionAmount { set; get; }
        public string transactionDescription { set; get; }
        public string externalpaymentref { set; get; }
        public string PaymentType { set; get; }
        public string paymentCurrency { set; get; }
        public string merchantCode { set; get; }
        public string custmerEmail { set; get; }
        public string custmerPhone { set; get; }
    }
}