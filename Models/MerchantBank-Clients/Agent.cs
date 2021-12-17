using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.MerchantBank
{
    public class Agent
    {
        public string dateCreated { get; set; }
        public string dateModified { get; set; }
        public string version { get; set; }
        public bool deleted { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public Address address { get; set; }
        public List<ContactPerson> contactPerson { get; set; }
        public bool active { get; set; }
        public string bpnNumber { get; set; }
        public string applicationForm { get; set; }
        public string accountNumber { get; set; }
    }
}