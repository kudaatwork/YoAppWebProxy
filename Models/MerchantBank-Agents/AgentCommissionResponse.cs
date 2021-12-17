using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.MerchantBank_Agents
{
    public class AgentCommissionResponse
    {
        public string agentName { get; set; }
        public decimal totalReceiveMoneyCommission { get; set; }
        public decimal totalSendMoneyCommission { get; set; }
    }
}