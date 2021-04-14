using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using YoAppWebProxy.Models;

namespace YoAppWebProxy.Helpful_Functions
{
    public class TrekMethods
    {
        TrekBearerConnector getConnector = new TrekBearerConnector();

        public string GetBearerToken(TrekBearerTokenRequest trekBearerTokenRequest)
        {
            var tokenResonse = getConnector.GetTokenResponse(trekBearerTokenRequest);
            var stringResponse = JsonConvert.SerializeObject(tokenResonse);

            return stringResponse;
        }
    }
}