using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models
{
    public class CardTransactionsByDate
    {
        public long id { get; set; }        
        public string status { get; set; }
        public string card_number { get; set; }
        public string amount { get; set; }
        public string channel { get; set; }
        public string description { get; set; }
        public string balance { get; set; }
        public string terminal_id { get; set; }
        public string agent_name { get; set; }
    }
}