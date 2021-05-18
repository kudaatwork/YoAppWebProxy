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
        //public static YoAppResponse GetClientIp(string domainName, string serviceProvider)
        //{
        //    YoAppResponse yoAppResponse = new YoAppResponse();

        //    IPHostEntry clientIP = Dns.GetHostEntry(domainName);
        //    IPAddress[] address = clientIP.AddressList;

        //    var clientIpAddress = address[1].ToString();

        //    if (clientIpAddress == "102.130.113.195")
        //    {
        //        yoAppResponse.ResponseCode = "00000";
        //        yoAppResponse.Note = "Success";
        //        yoAppResponse.Narrative = "YoApp is connected";

        //        Log.IpAddress("IpLog", serviceProvider, yoAppResponse);

        //        return yoAppResponse;
        //    }
        //    else
        //    {
        //        string wrongIPMessage = "Request coming from an unexpected IP. The IP Address has been logged {" + clientIpAddress + "}";
        //        yoAppResponse.ResponseCode = "00008";
        //        yoAppResponse.Note = "Failed";
        //        yoAppResponse.Description = wrongIPMessage;
        //        yoAppResponse.Narrative = wrongIPMessage;

        //        Log.IpAddress("IpLog", serviceProvider, yoAppResponse);

        //        return yoAppResponse;
        //    }
        // }

        public static YoAppResponse GetIp(string serviceProvider)
        {
            YoAppResponse yoAppResponse = new YoAppResponse();

            HttpContext context = HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    yoAppResponse.ResponseCode = "00000";
                    yoAppResponse.Note = "Success";
                    yoAppResponse.Narrative = "YoApp is connected";

                    Log.IpAddress("IpLog", serviceProvider, yoAppResponse);
                }
            }

            var clientIpAddress = context.Request.ServerVariables["REMOTE_ADDR"];

            if (clientIpAddress == "102.130.113.195")
            {
                yoAppResponse.ResponseCode = "00000";
                yoAppResponse.Note = "Success";
                yoAppResponse.Narrative = "YoApp is connected";

                Log.IpAddress("IpLog", serviceProvider, yoAppResponse);

                return yoAppResponse;
            }
            else
            {
                string wrongIPMessage = "Request coming from an unexpected IP. The IP Address has been logged {" + clientIpAddress + "}";
                yoAppResponse.ResponseCode = "00008";
                yoAppResponse.Note = "Failed";
                yoAppResponse.Description = wrongIPMessage;
                yoAppResponse.Narrative = wrongIPMessage;

                Log.IpAddress("IpLog", serviceProvider, yoAppResponse);

                return yoAppResponse;
            }
        }
    }
}