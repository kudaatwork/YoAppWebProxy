using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.MerchantBank_Clients
{
    public class ClientRequest
    {
        public int id { get; set; }
        public string clientId { get; set; }
        public string phoneNumber { get; set; }
        public string nationalId { get; set; }
        public string page { get; set; }
        public string size { get; set; }        
    }
}