using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.YoApp
{
    public class InitialProducts
    {
        public Nullable<long> Id { get; set; }
        public Nullable<long> ActionId { get; set; }
        public Nullable<long> ServiceId { get; set; }
        public string SupplierId { get; set; }
        public string ServiceName { get; set; }
        public string Branch { get; set; }
        public string ActionName { get; set; }
        public string Name { get; set; }
        public string ItemCode { get; set; }
        public string Description { get; set; }
        public string Unit { get; set; }
        public Nullable<long> Count { get; set; }
        public decimal Amount { get; set; }
        public decimal CollectionAmount { get; set; }
        public decimal Quantity { get; set; }
        public decimal Collected { get; set; }
        public Nullable<long> Reorder { get; set; }
        public decimal Price { get; set; }
        public decimal TaxPercentage { get; set; }
        public decimal TaxPrice { get; set; }
        public string Currency { get; set; }
        public string ServiceType { get; set; }
        public string ServiceCategory { get; set; }
        public string Image { get; set; }
        public string AmountRequiredIn { get; set; }
        public decimal MaxSale { get; set; }
        public decimal MinSale { get; set; }
        public string Status { get; set; }
        public bool IsExternal { get; set; }
        public Nullable<long> ExternalServiceId { get; set; }
        public string ExternalURL { get; set; }
        public string Base { get; set; }
        public string ProportionProducts { get; set; }
        public bool HasRecipe { get; set; }
        public bool HasFiles { get; set; }
        public string Files { get; set; }
        public bool DontDisplayRecipe { get; set; }
        public bool NotForSale { get; set; }
    }
}