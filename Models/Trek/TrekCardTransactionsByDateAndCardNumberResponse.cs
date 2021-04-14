using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models
{
    public class TrekCardTransactionsByDateAndCardNumberResponse
    {
        public string status { get; set; }
        public string message { get; set; }
        public List<CardTransactions> data { get; set; }
    }
}