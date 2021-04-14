using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models
{
    public class TrekCardTransactionsByDateResponse
    {
        public string status { get; set; }
        public string message { get; set; }
        public List<CardTransactionsByDate> data { get; set; }
    }
}