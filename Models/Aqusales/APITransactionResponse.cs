using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.Aqusales
{
    public class APITransactionResponse
    {
        public string Status { get; set; }
        public string Description { get; set; }
        public List<string> ErrorList { get; set; }
    }
}