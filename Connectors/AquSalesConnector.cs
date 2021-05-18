using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Web;
using YoAppWebProxy.Models.Aqusales;
using YoAppWebProxy.Logs;

namespace YoAppWebProxy.Connectors
{
    public class AquSalesConnector
    {
        public APITransactionResponse PostRedemption(SaleTransaction saleTransaction, string serviceProvider)
        {
            APITransactionResponse transactionResponse = new APITransactionResponse();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string url = String.Format("http://102.130.120.163:8091/Yomoney/SaleTransaction");
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.PreAuthenticate = true;
                httpWebRequest.Timeout = 120000;               
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;
                httpWebRequest.CookieContainer = new CookieContainer();
                Cookie cookie = new Cookie("AspxAutoDetectCookieSupport", "1");
                cookie.Domain = "102.130.120.163";
                httpWebRequest.CookieContainer.Add(cookie);

                string json = JsonConvert.SerializeObject(saleTransaction);

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        transactionResponse = JsonConvert.DeserializeObject<APITransactionResponse>(result);
                    }
                }
            }
            catch (Exception e)
            {
                Log.HttpError("Aqu-HttpError", serviceProvider, e.Message);
            }

            return transactionResponse;
        }

        public APITransactionResponse PostGRV(GRVTransaction grvTransaction, string serviceProvider)
        {
            APITransactionResponse transactionResponse = new APITransactionResponse();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string url = String.Format("http://102.130.120.163:8091/Yomoney/GRVTransaction");
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.PreAuthenticate = true;
                httpWebRequest.Timeout = 120000;
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;
                httpWebRequest.CookieContainer = new CookieContainer();
                Cookie cookie = new Cookie("AspxAutoDetectCookieSupport", "1");
                cookie.Domain = "102.130.120.163";
                httpWebRequest.CookieContainer.Add(cookie);

                string json = JsonConvert.SerializeObject(grvTransaction);

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        transactionResponse = JsonConvert.DeserializeObject<APITransactionResponse>(result);
                    }
                }
            }
            catch (Exception e)
            {
                Log.HttpError("Aqu-HttpError", serviceProvider, e.Message);
            }

            return transactionResponse;
        }
    }
}