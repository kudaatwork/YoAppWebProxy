using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models
{
    public class TrekCardBalanceResponse
    {
        public string status { get; set; }
        public string message { get; set; }
        public CardBalance data { get; set; }
    }    
}