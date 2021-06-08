using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.EOS
{
    public class EosReversalRequest
    {
        public string TransactionCode { set; get; }
        public string CustomerId { set; get; }
        public string ReceiversName { set; get; }
        public string ReceiversSurname { set; get; }
        public string ReceiversIdentification { set; get; }
        public string ServiceRegion { set; get; }// district
        public string ServiceProvince { set; get; }
        public string SupplierId { set; get; }
        public string SupplierName { set; get; }
        public string CustomerName { set; get; }
        public string ResponseCode { set; get; }
        public string LocationCode { set; get; }
        public List<EosReversalProducts> Products { set; get; }
    }
}