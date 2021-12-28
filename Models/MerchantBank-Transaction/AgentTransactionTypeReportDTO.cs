using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.MerchantBank_Agents
{
    public class AgentTransactionTypeReportDTO
    {
        public string currencyCode { get; set; }
        public decimal totalAmount { get; set; }
    }
}