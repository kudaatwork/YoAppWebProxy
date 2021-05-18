using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.Aqusales
{
    public class TranCurrencyInfoVM
    {
        public string TransactionCurrency { get; set; }
        public decimal TransactionCurrencyRate { get; set; }
        public decimal TransactionCurrencyTotal { get; set; }
    }
}