using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using YoAppWebProxy.Models;

namespace YoAppWebProxy
{
    public class CbzMethods
    {
        public string GetCropType(string productDetails)
        {
            if (productDetails.ToUpper() == "DRYLAND" || productDetails.ToUpper() == "DRYLAND2")
            {
                productDetails = "Maize";
            }
            else if (productDetails.ToUpper() == "SOYA" || productDetails.ToUpper() == "SOYA2")
            {
                productDetails = "Soya";
            }

            return productDetails;
        }

        public List<Product> GetProductsList(List<Products> productsList)
        {
            List<Product> products = new List<Product>();

            foreach (var item in productsList)
            {
                products.Add(new Product { ProductRedeemed = item.Name, QuantityRedeemed = item.Collected, PricePerUnit = item.Price });
            }

            return products;
        }
    }
}