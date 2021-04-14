using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy
{
    public class AgribankCredentials
    {
        private string username = "rkanyepi@agribank.co.zw";

        public string Username
        {
            get { return username; }
        }

        private string password = "Password@1";

        public string Password
        {
            get { return password; }
        }

        private string clientName = "agribank";

        public string ClientName
        {
            get { return clientName; }
        }
    }
}