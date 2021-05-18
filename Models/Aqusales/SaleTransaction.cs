using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.Aqusales
{
    public class SaleTransaction
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsCreditSale { get; set; }
        public string TransactionReference { get; set; }
        public string Company { get; set; }
        public string ProductCategory { get; set; }
        public string ProductSubCategory { get; set; }
        public string Customer { get; set; }
        public string CustomerMSISDN { get; set; }
        public decimal Amount { get; set; }
        public decimal CostOfSales { get; set; }
        public decimal Vat { get; set; }
        public TranData TranData { get; set; }
        public TranSourceDetails TranSourceDetails { get; set; }
        public TranCurrencyInfoVM TranCurrencyInfoVM { get; set; }
        public decimal Discount { get; set; }
        public string TransactionDate { get; set; }
        public string TransactionName { get; set; }
        public string Cashier { get; set; }
        public string CustomerAccountType { get; set; }
    }
}