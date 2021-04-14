using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Web;
using YoAppWebProxy.Models;

namespace YoAppWebProxy
{
    public class AgribankPostConnector
    {
        public AgribankPaymentResponse PostPayment(AgribankPaymentRequest agribankPaymentRequest)
        {
            AgribankPaymentResponse paymentResponse = new AgribankPaymentResponse();
            
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );
                                
                string url = String.Format("http://196.2.66.6:8010/api/upload/companyName");
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.PreAuthenticate = true; 
                httpWebRequest.Timeout = 120000;
                httpWebRequest.Headers.Add("Authorization", "Bearer " + Token.StringToken);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "GET";
                httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;

                string json = JsonConvert.SerializeObject(agribankPaymentRequest);

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        paymentResponse = JsonConvert.DeserializeObject<AgribankPaymentResponse>(result);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return paymentResponse;
        }        
    }
}