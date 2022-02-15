using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.YoApp
{
    public class WebPayments
    {        
        public string customerEmail { get; set; }
        public string transactionAmount { get; set; }
        public string paymentCurrency { get; set; }
        public string orgName { get; set; }
        public string orgPhoneNumber { get; set; }
        public string orgAddress { get; set; }
        public string packageType { get; set; }
        public string contactPersonName { get; set; }
        public string contactPersonLastName { get; set; }
        public string orgLocation { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }        
        public string paymentPeriod { get; set; }
        public string billingCycle { get; set; }
        public int cycleQuantity { get; set; }
    }
}