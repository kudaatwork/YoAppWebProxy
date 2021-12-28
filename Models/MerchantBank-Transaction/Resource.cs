using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.MerchantBank_Agents
{
    public class Resource
    {
        public bool open { get; set; }
        public string file { get; set; }
        public bool readable { get; set; }
        public string url { get; set; }
        public string uri { get; set; }
        public string description { get; set; }
        public List<string> inputStream { get; set; }
    }
}