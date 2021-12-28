using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.MerchantBank_Login
{
    public class UserPermission
    {
        public int id { get; set; }
        public string authority { get; set; }
        public string description { get; set; }
    }
}