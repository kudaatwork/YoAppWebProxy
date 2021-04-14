using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models
{
    public class TrekCardTransactionsByDateRequest
    {
        public string start_date { get; set; }
        public string end_date { get; set; }
    }
}