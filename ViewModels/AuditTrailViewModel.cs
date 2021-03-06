using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using YoAppWebProxy.Models.AuditLog;

namespace YoAppWebProxy.ViewModels
{
    public class AuditTrailViewModel
    {
        public string EndDate { get; set; }
        public string StartDate { get; set; }
        public string SubFolder { get; set; }
        public string ServiceProvider { get; set; }
        public List<string> ServiceProviders { get; set; }
        public List<string> SubDirectories { get; set; }
        public List<AuditTrail> Logs { get; set; }
    }
}