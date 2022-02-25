using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using YoAppWebProxy.Logs;
using YoAppWebProxy.Models;
using YoAppWebProxy.Models.YoApp;

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

        public string YoAppPayment(string serviceProvider, Payments payments)
        {
            string responseUrls = "";

            try
            {
                /*string data = "";
                data = data + "merchantCode=" + payments.merchantCode;
                data = data + "&successURL=" + payments.successURL;
                data = data + "&failedURL=" + payments.failedURL;// Id;
                data = data + "&custmerEmail=" + payments.custmerEmail;
                data = data + "&transactionAmount=" + payments.transactionAmount.ToString();
                data = data + "&paymentCurrency=" + payments.paymentCurrency.Trim();
                data = data + "&transactionDescription=" + payments.transactionDescription;
                data = data + "&paymentref=" + payments.paymentref;
                data = data + "&status=" + payments.status;
                data = data + "&paymentType=" + payments.paymentType;
                data = data + "&hash=" + payments.hash;*/

                string url = String.Format("https://www.yomoneyservice.com/Payment/ApiRequest");
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.Timeout = 120000;
                httpWebRequest.CookieContainer = new CookieContainer();
                Cookie cookie = new Cookie("AspxAutoDetectCookieSupport", "1");
                cookie.Domain = "www.yomoneyservice.com";
                httpWebRequest.CookieContainer.Add(cookie);

                string json = JsonConvert.SerializeObject(payments, Formatting.Indented);

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        responseUrls = JsonConvert.DeserializeObject<string>(result);                        
                    }
                }
            }
            catch (Exception e)
            {
                Log.HttpError("{Exception", serviceProvider, "Message: " + e.Message + ", InnerException: " + e.InnerException + ", StackTrace: " + e.StackTrace + "}");
            }

            return responseUrls;
        }

        public YoAppResponse CreateCustomerAndLicense(YoAppRequest yoappRequest, string serviceProvider)
        {
            YoAppResponse yoappResponse = new YoAppResponse();

            try
            {
                string url = String.Format("https://www.yomoneyservice.com/yoclient/transaction");
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
                Log.HttpError("Exception", serviceProvider, "Message: " + e.Message + ", InnerException: " + e.InnerException + ", StackTrace: " + e.StackTrace);
            }

            return yoappResponse;
        }
    }
}