using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Api_Credentials
{
    public class MetBankCredentials
    {
        private string username = "cap10";

        public string Username
        {
            get { return username; }
        }

        private string password = "Venon?1986";

        public string Password
        {
            get { return password; }
        }

        private string accessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE2NDEwODg2NTYsInVzZXJfbmFtZSI6ImNhcDEwIiwiYXV0aG9yaXRpZXMiOlsiREVMRVRFX0VYQ0hBTkdFX1JBVEUiLCJVUERBVEVfRVhDSEFOR0VfUkFURSIsIlVQREFURV9GRUVTIiwiVVBEQVRFX0NMSUVOVCIsIkFQUFJPVkVfQ09VTlRSWSIsIkFQUFJPVkVfQ0xJRU5UIiwiQVBQUk9WRV9DVVJSRU5DWSIsIlVQREFURV9UUkFOU0FDVElPTl9MSU1JVFMiLCJVUERBVEVfQ09NTUlTU0lPTiIsIkFQUFJPVkVfVFJBTlNBQ1RJT05fTElNSVRTIiwiQVBQUk9WRV9DT01NSVNTSU9OIiwiQVBQUk9WRV9FWENIQU5HRV9SQVRFIiwiVVBEQVRFX0NPVU5UUlkiLCJVUERBVEVfQ1VSUkVOQ1kiLCJBUFBST1ZFX0FHRU5UIiwiUk9MRV9TWVNURU1fQURNSU4iLCJBUFBST1ZFX0ZFRVMiXSwianRpIjoiZTA0Yzc0M2MtNWZlOS00NzgyLTlhOTQtNGIxYzIwOGFkYmQ5IiwiY2xpZW50X2lkIjoiQURNSU4iLCJzY29wZSI6WyJyZWFkIiwid3JpdGUiXX0.epK7begDY6yTNM3ZX2y2Bv6aN-9kaHXbJLUWVZrnwAQ";

        public string AccessToken
        {
            get { return accessToken; }
        }
    }
}