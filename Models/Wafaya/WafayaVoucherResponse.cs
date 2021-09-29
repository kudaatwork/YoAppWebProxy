using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.Wafaya
{
    public class WafayaVoucherResponse
    {
        public string created_at { get; set; }
        public string voucher_code { get; set; }
        public string description { get; set; }
        public string redeemer_phone { get; set; }
        public string redeemer_name { get; set; }
        public bool active { get; set; }
        public string expiry_date { get; set; }
        public bool is_expired { get; set; }
        public decimal voucher_value { get; set; }
        public string voucher_currency { get; set; }
        public decimal voucher_balance { get; set; }
        public List<RedemptionLimits> redemption_limits { get; set; }
        public ProductRedemptionLimit product_redemption_limit { get; set; }
    }
}