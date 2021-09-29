using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Api_Credentials
{
    public class WafayaCredentials
    {
        private string authorisationClientId = "94730c2e-99de-4f4b-8f67-bcaebd9e05bb";

        public string AuthorisationClientId
        {
            get { return authorisationClientId; }
        }

        private string passwordClientId = "94730d4c-b6bc-4a1a-9ac9-07b5b9c139fd";

        public string PasswordClientId
        {
            get { return passwordClientId; }
        }

        private string authorizationSecret = "4loyYHSe5XsS3DB5xBjpSkdU1Q9Q38sZnVaqKNxe";

        public string AuthorizationSecret
        {
            get { return authorizationSecret; }
        }

        private string passwordSecret = "RfPGn92XOcKgEi4zo08pNinb6M6T4VtdyAijlpZI";

        public string PasswordSecret
        {
            get { return passwordSecret; }
        }

        private string clientUrl = "https://www.yomoneyservice.com";

        public string ClientUrl
        {
            get { return clientUrl; }
        }

        private string username = "yoapp@wa-faya.com";

        public string Username
        {
            get { return username; }
        }

        private string password = "12345678";

        public string Password
        {
            get { return password; }
        }
    }
}