using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.AuditLog
{
    public class AuditTrail
    {
        public string DateTimeLogged { get; set; }
        public string ServiceProvider { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
    }
}