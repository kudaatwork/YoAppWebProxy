using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models
{
    public class YoAppRequest
    {
        public Nullable<long> ActionId { get; set; }
        public string AgentCode { get; set; }
        public string Currency { get; set; }
        public string Action { get; set; } 
        public string Mpin { get; set; }
        public decimal Amount { get; set; }
        public string CustomerMSISDN { get; set; }
        public long ServiceId { get; set; }
        public string MTI { get; set; }
        public string TerminalId { get; set; }
        public string TransactionRef { get; set; }
        public string CustomerAccount { get; set; }
        public string CustomerData { get; set; }
        public string CustomerName { get; set; }
        public string Product { get; set; }
        public string ServiceProvider { get; set; }
        public string Source { get; set; }
        public string PaymentMethod { get; set; }
        public string ProcessingCode { get; set; }
        public string Quantity { get; set; }
        public string OrderLine { get; set; }
        public string Narrative { get; set; }
        public string Note { get; set; }
        public decimal MaxSale { get; set; }
        public decimal MinSale { get; set; }
        public Nullable<long> TransactionType { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public Narrative NarrativeRequest { get; set; }    
    }
}