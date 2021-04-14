using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models
{
    public class CbzVoucherPaymentDetails
    {
        public string referenceNumber { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string ReceiverNationalId { get; set; }
        public string CropType { get; set; }
        public List<Product> Summary { get; set; }
    }
}