using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.MerchantBank
{
    public class ClientResponse
    {
        public bool clientFound { get; set; }
        public Client client { get; set; }
    }
}