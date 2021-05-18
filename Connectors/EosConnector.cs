using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Web;
using YoAppWebProxy.Logs;
using YoAppWebProxy.Models.EOS;

namespace YoAppWebProxy.Connectors
{
    public class EosConnector
    {
        public EosResponse PostRedemption(EosRequest eosRequest, string serviceProvider)
        {
            EosResponse eosResponse = new EosResponse();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string url = String.Format("http://192.168.100.75:5000/Yomoney/SaleTransaction");
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.PreAuthenticate = true;
                httpWebRequest.Timeout = 120000;
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;
               
                string json = JsonConvert.SerializeObject(eosRequest);

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        eosResponse = JsonConvert.DeserializeObject<EosResponse>(result);
                    }
                }
            }
            catch (Exception e)
            {
                Log.HttpError("Aqu-HttpError", serviceProvider, e.Message);
            }

            return eosResponse;
        }
    }
}