using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.MerchantBank_Login
{
    public class RefresherTokenRequest
    {
        public string token { get; set; }
        public string username { get; set; }
    }
}