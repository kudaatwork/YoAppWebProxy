using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using YoAppWebProxy.Models;
using Newtonsoft.Json;

namespace YoAppWebProxy
{
    public class AgribankMethods
    {
        AgribankBearerConnector getConnector = new AgribankBearerConnector();

        public string GetBearerToken(AgribankTokenRequest agribankTokenRequest)
        {
            var tokenResonse = getConnector.GetTokenResponse(agribankTokenRequest);
            var stringResponse = JsonConvert.SerializeObject(tokenResonse);
            
            return stringResponse;
        }
    }
}