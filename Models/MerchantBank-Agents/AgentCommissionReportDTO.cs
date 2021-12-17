using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.MerchantBank_Agents
{
    public class AgentCommissionReportDTO
    {
        public string currencyCode { get; set; }
        public decimal totalCommission { get; set; }
    }
}