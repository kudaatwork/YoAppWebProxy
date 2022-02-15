using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.MerchantBank_Agents
{
    public class CalculateCurrencyRateRequest
    {
        public string convertToCurrencyCode { get; set; }
        public string convertFromCurrencyCode { get; set; }
        public decimal fromAmount { get; set; }
        public decimal toAmount { get; set; }
        public int countryId { get; set; }
        public int toCountryCode { get; set; }
        public string activeInput { get; set; }
    }
}