using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.MerchantBank
{
    public class RegisterAgentRequest
    {
        public string name { get; set; }
        public Address address { get; set; }
        public List<ContactPerson> contactPerson { get; set; }
        public string username { get; set; }
        public string userLastName { get; set; }
        public string userFirstName { get; set; }
        public string userEmail { get; set; }
        public string bpnNumber { get; set; }
        public string applicationForm { get; set; }
    }
}