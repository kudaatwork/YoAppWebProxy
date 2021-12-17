using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.MerchantBank
{
    public class Pageable
    {
        public int offset { get; set; }
        public Sort sort { get; set; }
        public int pageNumber { get; set; }
        public int pageSize { get; set; }
        public bool paged { get; set; }
        public bool unpaged { get; set; }
    }
}