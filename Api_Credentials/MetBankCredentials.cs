using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Api_Credentials
{
    public class MetBankCredentials
    {
        private string username = "CMbizo";

        public string Username
        {
            get { return username; }
        }

        private string password = "Venon?1986";

        public string Password
        {
            get { return password; }
        }

        private string accessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE2NDUzMjc3NTgsInVzZXJfbmFtZSI6IkNNYml6byIsImF1dGhvcml0aWVzIjpbIkFQUFJPVkVfQ0xJRU5UIiwiQ1JFQVRFX0NMSUVOVCIsIlJPTEVfQUdFTlRfQURNSU4iLCJVUERBVEVfQ0xJRU5UIl0sImp0aSI6IjkyNjE2ODk0LTAxMjItNDgyNi04Njg3LTQ3NTBjZTZjZWI3MyIsImNsaWVudF9pZCI6IkFHRU5UIiwic2NvcGUiOlsicmVhZCIsIndyaXRlIl19.4guDSM38Itw6OS938xJBdvLt-GPa8zIggsMkmkXnVXc";

        public string AccessToken
        {
            get { return accessToken; }
        }
    }
}