using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using YoAppWebProxy.Models;
using YoAppWebProxy.Logs;

namespace YoAppWebProxy
{
    public class SiteIdentity
    {
        public static YoAppResponse GetClientIp(string domainName)
        {
            YoAppResponse yoAppResponse = new YoAppResponse();

            IPHostEntry clientIP = Dns.GetHostEntry(domainName);
            IPAddress[] address = clientIP.AddressList;

            var clientIpAddress = address[1].ToString();

            if (clientIpAddress == "102.130.113.195")
            {
                yoAppResponse.ResponseCode = "00000";
                yoAppResponse.Note = "Success";
                yoAppResponse.Narrative = "YoApp is connected";

                IpLog.IpAddress(yoAppResponse);

                return yoAppResponse;
            }
            else
            {
                string wrongIPMessage = "Request coming from an unexpected IP. The IP Address has been logged {" + clientIpAddress + "}";
                yoAppResponse.ResponseCode = "00008";
                yoAppResponse.Note = "Failed";
                yoAppResponse.Description = wrongIPMessage;
                yoAppResponse.Narrative = wrongIPMessage;

                IpLog.IpAddress(yoAppResponse);

                return yoAppResponse;
            }
        }
    }
}