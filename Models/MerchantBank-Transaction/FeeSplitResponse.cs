using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.MerchantBank_Agents
{
    public class FeeSplitResponse
    {
        public int transactionReference { get; set; }
        public string dateTime { get; set; }
        public decimal fees { get; set; }
        public decimal sendMoneyCommission { get; set; }
        public decimal receiveMoneyCommission { get; set; }
        public decimal ownerProfits { get; set; }
    }
}