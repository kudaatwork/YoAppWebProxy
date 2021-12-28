using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.MerchantBank_Transaction
{
    public class TransactionResponse
    {
        public int transactionId { get; set; }
        public string description { get; set; }
        public string transactionReference { get; set; }
        public string amount { get; set; }
        public string fees { get; set; }
        public string status { get; set; }
        public string collectionCurrencyCode { get; set; }
    }
}