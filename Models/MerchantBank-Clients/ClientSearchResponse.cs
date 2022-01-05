using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using YoAppWebProxy.Models.MerchantBank;

namespace YoAppWebProxy.Models.MerchantBank_Clients
{
    public class ClientSearchResponse
    {
        public bool clientRegistered { get; set; }
        public bool clientHasUserAccount { get; set; }
        public Client client { get; set; }
    }
}