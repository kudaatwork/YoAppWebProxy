using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models
{
    /// <summary>
    /// E Solutions Response Object
    /// </summary>
    public class ESolutionsResponse
    {
        public string mti { get; set; }
        public string description { get; set; }
        public string vendorReference { get; set; }
        public string processingCode { get; set; }
        public string transactionAmount { get; set; }
        public string amount { get; set; }
        public string transmissionDate { get; set; }
        public string vendorNumber { get; set; }
        public string transactionReference { get; set; }       
        public string responseCode { get; set; }
        public string vendorTerminalId { get; set; }
        public string vendorBranchId { get; set; }
        public string merchantName { get; set; }
        public string productName { get; set; }
        public string cutomerAddress { get; set; }
        public string customerData { get; set; }
        public string arrears { get; set; }
        public string vendorBalance { get; set; }
        public string currencyCode { get; set; }
        public string aggregator { get; set; }
        public string utilityAccount { get; set; }
        public string narrative { get; set; }
        public string paymentType { get; set; }
        public string token { get; set; }
        public string fixedCharges { get; set; }
        public string miscellaneousData { get; set; }
        public string receiptNumber { get; set; }
        public string sourceMobile { get; set; }
        public string targetMobile { get; set; }
        public string subProductName { get; set; }
        public string serviceId { get; set; }
        public bool requiresVoucher { get; set; }
        public Nullable<decimal> initialBalance { get; set; }
        public Nullable<decimal> finalBalance { get; set; }
        public string customerBalance { get; set; }
        public string originalReference { get; set; }
        public string accountNumber { get; set; }
        public string id { get; set; }
    }
}