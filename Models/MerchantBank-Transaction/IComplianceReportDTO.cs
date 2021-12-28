using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.MerchantBank_Transaction
{
    public class IComplianceReportDTO
    {
        public decimal usdAmount { get; set; }
        public string dateTime { get; set; }
        public decimal amountReceived { get; set; }
        public decimal senderAmount { get; set; }
        public string receiverCurrency { get; set; }
        public string senderCurrency { get; set; }
        public string senderName { get; set; }
        public string senderNationalId { get; set; }
        public string receiverName { get; set; }
        public string receiverNationalId { get; set; }
        public string senderFirstName { get; set; }
        public string senderLastName { get; set; }
        public string senderAddress { get; set; }
        public string senderPhoneNumber { get; set; }
        public string senderCountryOfResidence { get; set; }
        public string senderStreet { get; set; }
        public string senderCity { get; set; }
        public string senderSuburb { get; set; }
        public string dateCollected { get; set; }
        public string receiverAddress { get; set; }
        public string receiverMobileNumber { get; set; }
        public string receiverCountryOfResidence { get; set; }
        public string receiverFirstName { get; set; }
        public string receiverLastName { get; set; }
    }
}