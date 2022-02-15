using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Api_Credentials
{
    public class YoAppCredentials
    {
        private string userId = "5-0001-0000980";

        public string UserId
        {
            get { return userId; }
        }

        private string password = "@qus@leslic3ns3";

        public string Password
        {
            get { return password; }
        }

        private string successURL = "http://102.130.120.163:8095/WebService/PaymentResponse/";

        public string SuccessUrl
        {
            get { return successURL; }
        }

        private string failureUrl = "http://102.130.120.163:8095/WebService/PaymentResponse/";

        public string FailureUrl
        {
            get { return failureUrl; }
        }
    }
}