using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using YoAppWebProxy.Models;

namespace YoAppWebProxy
{
    public class YoAppConnector
    {
        public YoAppResponse GetYoAppResponse(YoAppRequest yoappRequest)
        {
            YoAppResponse yoappResponse = new YoAppResponse();

            try
            {
                string url = String.Format("http://192.168.100.11:5000/yoclient/transaction");
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.Timeout = 120000;
                httpWebRequest.CookieContainer = new CookieContainer();
                Cookie cookie = new Cookie("AspxAutoDetectCookieSupport", "1");
                cookie.Domain = "www.yomoneyservice.com";
                httpWebRequest.CookieContainer.Add(cookie);

                string json = JsonConvert.SerializeObject(yoappRequest, Formatting.Indented);

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        yoappResponse = JsonConvert.DeserializeObject<YoAppResponse>(result);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return yoappResponse;
        }
    }
}