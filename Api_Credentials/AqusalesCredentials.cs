using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Api_Credentials
{
    public class AqusalesCredentials
    {
        private static string username = "Salesaqu";

        public static string Username
        {
            get
            {
                return username;
            }
        }

        private static string password = "admin";

        public static string Password
        {
            get
            {
                return password;
            }
        }
    }
}