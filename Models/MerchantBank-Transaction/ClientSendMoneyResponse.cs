using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.MerchantBank_Transaction
{
    public class ClientSendMoneyResponse
    {
        public string description { get; set; }
        public decimal amount { get; set; }
        public decimal fees { get; set; }
        public string currencyCodeSend { get; set; }
        public string collectionCurrencyCode { get; set; }
        public decimal collectionAmount { get; set; }
        public string preauthId { get; set; }
        public decimal totalAmount { get; set; }
    }
}