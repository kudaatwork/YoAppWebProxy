using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using YoAppWebProxy.Models;
using YoAppWebProxy.Logs;

namespace YoAppWebProxy
{
    public class CbzConnector
    {
        public CbzPaymentResponse GetCBZResponse(List<CbzVoucherPaymentDetails> cbzRequest)
        {
            CbzPaymentResponse cbzResponse = new CbzPaymentResponse();

            try
            {
                string url = String.Format("http://192.168.3.150/CBZ-YoMoney-API-WCF/CBZ-YoMoney-API-WCF/v1.svc/sendFarmerRedemptions");
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                //httpWebRequest.CookieContainer = new CookieContainer();
                //Cookie cookie = new Cookie("AspxAutoDetectCookieSupport", "1");
                //cookie.Domain = "www.yomoneyservice.com";
                //httpWebRequest.CookieContainer.Add(cookie);

                string json = JsonConvert.SerializeObject(cbzRequest, Formatting.Indented);

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        cbzResponse = JsonConvert.DeserializeObject<CbzPaymentResponse>(result);
                    }
                }
            }
            catch (Exception e)
            {
                CbzLog.HttpErrorLog(e.Message);                
            }

            return cbzResponse;
        }
    }
}