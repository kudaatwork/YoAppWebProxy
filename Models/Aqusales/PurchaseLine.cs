using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.Aqusales
{
    public class PurchaseLine
    {
        public long ID { get; set; }
        public string Reciept { get; set; }
        public string item { get; set; }
        public Nullable<decimal> quantity { get; set; }
        public Nullable<decimal> price { get; set; }
        public Nullable<decimal> tax { get; set; }
        public Nullable<decimal> priceinc { get; set; }
        public string Category { get; set; }
        public Nullable<System.DateTime> Dated { get; set; }
        public string ItemCode { get; set; }
        public string Barcode { get; set; }
        public string Status { get; set; }
        public Nullable<decimal> Sold { get; set; }
        public Nullable<System.DateTime> DateClosed { get; set; }
        public string Company { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<System.DateTime> DeshelvingDate { get; set; }
        public string BatchNumber { get; set; }
        public Nullable<decimal> ogquantity { get; set; }
        public string ProductType { get; set; }
        public string Notes { get; set; }
        public Nullable<decimal> ExpPercentCost { get; set; }
        public Nullable<decimal> ActPercentCost { get; set; }
        public Nullable<decimal> ExpQuantity { get; set; }
        public Nullable<decimal> CurrentCost { get; set; }
    }
}