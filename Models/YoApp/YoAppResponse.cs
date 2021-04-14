using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models
{
    /// <summary>
    /// Respresents Transaction Response from YoApp
    /// </summary>
    public class YoAppResponse
    {
        /// <summary>
        /// ServiceActionId on YoApp
        /// </summary>
        public long ActionId { get; set; }

        /// <summary>
        /// ResponseCode from the Initiated Transaction on YoApp
        /// </summary>
        public string ResponseCode { get; set; }

        /// <summary>
        /// Response Description from the Initiated Transaction on YoApp
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Supplier Account's Balance on YoApp
        /// </summary>
        public string Balance { get; set; }
        public List<Vouchers> vouchers { get; set; }
        public string AgentCode { get; set; }
        public string Mpin { get; set; }
        public decimal Amount { get; set; }
        public string CustomerMSISDN { get; set; }
        public long ServiceId { get; set; }
        public string MTI { get; set; }
        public string TerminalId { get; set; }
        public string TransactionRef { get; set; }
        public string CustomerAccount { get; set; }
        public string CustomerData { get; set; }
        public string Product { get; set; }
        public string ServiceProvider { get; set; }
        public string ProcessingCode { get; set; }
        public string Quantity { get; set; }
        public string Narrative { get; set; }
        public string Note { get; set; }
        public decimal MaxSale { get; set; }
        public decimal MinSale { get; set; }
        public string TransactionCode { get; set; }
        public Narrative NarrativeResponse { get; set; }
    }
}