using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.MerchantBank
{
    public class PageAgent
    {
        public int totalPages { get; set; }
        public int totalElements { get; set; }
        public int size { get; set; }
        public List<Agent> content { get; set; }
        public int number { get; set; }
        public Sort sort { get; set; }
        public Pageable pageable { get; set; }
        public bool first { get; set; }
        public int numberOfElements { get; set; }
        public bool last { get; set; }
        public bool empty { get; set; }
    }
}