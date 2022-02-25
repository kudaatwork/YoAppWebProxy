using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.Aqusales
{
    public class PaymentTransaction
    {
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string TransactionReference { get; set; }
        public string Company { get; set; }
        public string Currency { get; set; }
        public string PaymentMethod { get; set; }
        public decimal Amount { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string TransactionName { get; set; }
        public string Cashier { get; set; }
    }
}