using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.YoApp
{
    public class YoAppGrvResponse
    {
        public long Id { get; set; }
        public Nullable<long> ActionId { get; set; }
        public Nullable<long> ServiceId { get; set; }
        public string OrderNumber { get; set; }
        public string ProviderCode { get; set; }
        public string SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string ServiceName { get; set; }
        public string OrderType { get; set; }
        public string BranchType { get; set; }
        public string BranchName { get; set; }
        public string ParentId { get; set; }
        public string ParentName { get; set; }
        public string VendorId { get; set; }
        public string VendorName { get; set; }
        public string VendorDetails { get; set; }
        public string Note { get; set; }
        public string Cashier { get; set; }
        public string Supervisor { get; set; }
        public string Status { get; set; }
        public string Currency { get; set; }
        public Nullable<decimal> Total { get; set; }
        public Nullable<decimal> StockedValue { get; set; }
        public Nullable<decimal> PreviousStock { get; set; }
        public Nullable<decimal> CurrentStock { get; set; }
        public Nullable<decimal> NewStock { get; set; }
        public Nullable<System.DateTime> RequestDate { get; set; }
        public Nullable<System.DateTime> AuthorisationDate { get; set; }
        public Nullable<System.DateTime> DateLastAccess { get; set; }
        public string Files { get; set; }
        public string BranchId { get; set; }
        public Nullable<long> DestinationId { get; set; }
        public Nullable<long> PackageId { get; set; }
        public Nullable<bool> CreateTask { get; set; }
        public Nullable<long> RouteId { get; set; }
    }
}