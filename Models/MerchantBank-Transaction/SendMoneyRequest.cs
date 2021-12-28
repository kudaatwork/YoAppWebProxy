using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.MerchantBank_Transaction
{
    public class SendMoneyRequest
    {
        public decimal amountSend { get; set; }
        public string sourceCountryCode { get; set; }
        public string destinationCountryCode { get; set; }
        public string currencyCodeSend { get; set; }
        public int clientId { get; set; }
        public int agentId { get; set; }
        public int tellerId { get; set; }
        public decimal collectionAmount { get; set; }
        public string collectionCurrencyCode { get; set; }
        public int recipientId { get; set; }
        public string reasonForTransfer { get; set; }
        public string preauthId { get; set; }
    }
}