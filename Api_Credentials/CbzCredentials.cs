﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy
{
    public class CbzCredentials
    {
        // Login Credentials
        private static string username = "YoMoney";

        public static string Username
        {
            get
            {
                return username;
            }
        }

        private static string password = "Password123";

        public static string Password
        {
            get
            {
                return password;
            }
        }
    }
}