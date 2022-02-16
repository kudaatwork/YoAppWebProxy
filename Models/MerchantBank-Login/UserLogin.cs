using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.MerchantBank_Login
{
    public class UserLogin
    {
        public string username { get; set; }
        public string password { get; set; }
        public string clientSecret { get; set; }
        public string clientId { get; set; }
    }
}