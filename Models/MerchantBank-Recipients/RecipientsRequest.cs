using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.MerchantBank_Recipients
{
    public class RecipientsRequest
    {
        public int clientId { get; set; }
        public string nationalId { get; set; }
        public string phoneNumber { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string gender { get; set; }
        public string relationship { get; set; }
        public string countryId { get; set; }
        public string address { get; set; }
    }
}