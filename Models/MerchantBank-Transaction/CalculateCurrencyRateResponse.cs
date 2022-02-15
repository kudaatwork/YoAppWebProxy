using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.MerchantBank_Agents
{
    public class CalculateCurrencyRateResponse
    {
        public decimal senderAmount { get; set; }
        public decimal receiverAmount { get; set; }
        public decimal fee { get; set; }
        public decimal rate { get; set; }
        public string convertToCurrencyCode { get; set; }
        public string convertFromCurrencyCode { get; set; }        
    }
}