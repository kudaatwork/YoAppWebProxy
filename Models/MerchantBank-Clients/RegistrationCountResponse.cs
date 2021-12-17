using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.MerchantBank
{
    public class RegistrationCountResponse
    {
        public bool ClientSearchResponse { get; set; }
        public bool clientHasUserAccount { get; set; }
        public Client client { get; set; }
    }
}