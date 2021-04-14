using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models
{
    public class CbzPaymentRequest
    {
        public List<CbzVoucherPaymentDetails> VoucherPaymentDetails { get; set; }
    }
}