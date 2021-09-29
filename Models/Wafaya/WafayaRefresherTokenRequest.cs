using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.Wafaya
{
    public class WafayaRefresherTokenRequest
    {
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string refresh_token { get; set; }        
        public string scope { get; set; }
        public string grant_type { get; set; }
    }
}