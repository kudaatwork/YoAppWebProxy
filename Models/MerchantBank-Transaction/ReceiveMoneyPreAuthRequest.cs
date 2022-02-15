using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.MerchantBank_Transaction
{
    public class ReceiveMoneyPreAuthRequest
    {
        public string voucherNumber { get; set; }
        public string tellerId { get; set; }
        public long agentId { get; set; }
    }
}