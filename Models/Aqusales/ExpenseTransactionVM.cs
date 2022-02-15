using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.Aqusales
{
    public class ExpenseTransactionVM
    {
        public string Supplier { get; set; }
        public string BankName { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Company { get; set; }
        public string ExpenseAccount { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Reference { get; set; }
        public Nullable<decimal> InputTax { get; set; }
        public Nullable<DateTime> TransactionDate { get; set; }
        public string Cashier { get; set; }
        public string CostCenter { get; set; }
    }
}