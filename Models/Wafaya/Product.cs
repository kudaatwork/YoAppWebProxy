using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.Wafaya
{
    public class Product
    {
        public string name { get; set; }
        public string image_url { get; set; }
        public string description { get; set; }
        public int value { get; set; }
        public int days_til_expiry { get; set; }
        public bool show_on_website { get; set; }
        public int sort { get; set; }
        public bool is_featured { get; set; }
        public string currency { get; set; }
        public DateTime created_at { get; set; }
    }
}