using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models
{
    public class Narrative
    {
        public Nullable<long> Id { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerMobileNumber { get; set; }
        public string CustomerCardNumber { get; set; }
        public Nullable<long> ServiceId { get; set; }
        public Nullable<long> TransactionType { get; set; }
        public string ServiceName { get; set; }
        public string ServiceType { get; set; }
        public decimal Quantity { get; set; }
        public bool HasProduct { get; set; }
        public bool IsActive { get; set; }
        public bool HasRecords { get; set; }
        public bool AllowPartPayment { get; set; }
        public bool DeactivateOnAuthorisation { get; set; }
        public string ProductName { get; set; }
        public string ProductDetails { get; set; }
        public string ServiceProvider { get; set; }
        public string SupplierId { get; set; }
        public string ServiceAgentId { get; set; }
        public string SupplierName { get; set; }
        public string Description { get; set; }
        public decimal Balance { get; set; }
        public decimal SuspenseBalance { get; set; }
        public string TransactionCode { get; set; }
        public string DateCreated { get; set; }
        public string DateLastAccess { get; set; }
        public string SubDue { get; set; }
        public string BillingCode { get; set; }
        public string ReceiverMobile { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseDescription { get; set; }
        public string logo { get; set; }
        public string ReceiversName { get; set; }
        public string ReceiversSurname { get; set; }
        public string ReceiversIdentification { get; set; }
        public string ReceiversGender { get; set; }
        public string ServiceRegion { get; set; }
        public string ServiceProvince { get; set; }
        public string ServiceCountry { get; set; }
        public string Currency { get; set; }
        public string Information1 { get; set; }
        public string Information2 { get; set; }
        public string Cashier { get; set; }
        public string Authoriser { get; set; }
        public string LocationCode { get; set; }
        public string JsonProducts { get; set; }
        public string InitialProducts { get; set; }
        public List<Products> Products { get; set; }
    }
}