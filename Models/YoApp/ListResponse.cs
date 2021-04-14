using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models
{
    public class ListResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ServiceId { get; set; }
        public long TransactionType { get; set; }
        public string SupplierId { get; set; }
        public long Count { get; set; }
        public bool HasProducts { get; set; }
        public string Amount { get; set; }
        public string Image { get; set; }
    }
}