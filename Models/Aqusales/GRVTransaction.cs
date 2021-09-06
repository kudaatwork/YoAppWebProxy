using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.Aqusales
{
    public class GRVTransaction
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string SupplierMSISDN { get; set; }
        public string SupplierAccountType { get; set; }
        public string TransactionReference { get; set; }
        public string Company { get; set; }
        public string DebtorOrCreditor { get; set; }
        public decimal Amount { get; set; }
        public decimal Vat { get; set; }
        public TranData TranData { get; set; }
        public TranSourceDetails TranSourceDetails { get; set; }
        public TranCurrencyInfoVM TranCurrencyInfoVM { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string TransactionName { get; set; }
        public string Cashier { get; set; }
        public List<PurchaseLine> PurchaseLines { get; set; }
    }
}