using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using YoAppWebProxy.Api_Credentials;
using YoAppWebProxy.Connectors;
using YoAppWebProxy.Logs;
using YoAppWebProxy.Models.Wafaya;

namespace YoAppWebProxy.Helpful_Functions
{
    public class WafayaMethods
    {
        //WafayaCredentials wafayaCredentials = new WafayaCredentials();
        //WafayaCodeRequest codeRequest = new WafayaCodeRequest();
        //WafayaTokenRequest tokenRequest = new WafayaTokenRequest();
        //WafayaResourceOwnerRequest resourceOwnerRequest = new WafayaResourceOwnerRequest();

        //WafayaConnector connector = new WafayaConnector();
        //bool isTokenValid = false;
        //var fileName = "tokens";

        //public void GenerateToken()
        //{
        //    resourceOwnerRequest.grant_type = "password";
        //    resourceOwnerRequest.client_id = wafayaCredentials.PasswordClientId;
        //    resourceOwnerRequest.client_secret = wafayaCredentials.PasswordSecret;
        //    resourceOwnerRequest.scope = "";
        //    resourceOwnerRequest.username = wafayaCredentials.Username;
        //    resourceOwnerRequest.password = wafayaCredentials.Password;

        //    Log.RequestsAndResponses("Wafaya-TokenRequest", serviceProvider, resourceOwnerRequest);

        //    var resourceOwnerResponse = connector.GetPasswordToken(resourceOwnerRequest, serviceProvider);

        //    if (resourceOwnerResponse.token_type.ToLower() == "bearer")
        //    {
        //        yoAppResponse.ResponseCode = "00000";
        //        yoAppResponse.Description = "Token generated successfully";
        //        yoAppResponse.Note = "Transaction Successful";

        //        var expDate = DateTime.Now.AddSeconds(Convert.ToDouble(resourceOwnerResponse.expires_in));

        //        resourceOwnerResponse.expires_in = expDate.ToString("dd/MM/yyyy-HH:mm:ss");

        //        var token = JsonConvert.SerializeObject(resourceOwnerResponse);

        //        yoAppResponse.Narrative = token;

        //        Log.RequestsAndResponses("Wafaya-TokenResponse-YoApp", serviceProvider, resourceOwnerResponse);
        //        Log.StoreData("tokens", serviceProvider, resourceOwnerResponse);

        //        return yoAppResponse;
        //    }
        //    else
        //    {
        //        yoAppResponse.ResponseCode = "00008";
        //        yoAppResponse.Description = "Code could not be generated";
        //        yoAppResponse.Note = "Transaction Failed";

        //        Log.RequestsAndResponses("Wafaya-TokenResponse-YoApp", serviceProvider, resourceOwnerResponse);

        //        return yoAppResponse;
        //    }
        //}
    }
}