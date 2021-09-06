using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.BulkPayments
{
    public class IBTRequestToServer
    {
        public string InstitutionID { get; set; }
        public string ExtReferenceNo { get; set; }
        public string Request { get; set; }

    }
}