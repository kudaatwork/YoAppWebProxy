using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.MerchantBank_Agents
{
    public class FeeSplitsReportDTO
    {
        public decimal totalFees { get; set; }
        public decimal totalOwnerAmount { get; set; }
        public decimal totalAgentCommission { get; set; }
        public string currencyCode { get; set; }
        public List<FeeSplitResponse> feeSplitResponses { get; set; }
        public JRBeanCollectionDataSource dataSource { get; set; }
    }
}