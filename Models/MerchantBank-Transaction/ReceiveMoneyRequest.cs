using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.MerchantBank_Transaction
{
    public class ReceiveMoneyRequest
    {
        public string voucherNumber { get; set; }
        public int tellerId { get; set; }
        public int agentId { get; set; }
        public string preauthId { get; set; }
    }
}