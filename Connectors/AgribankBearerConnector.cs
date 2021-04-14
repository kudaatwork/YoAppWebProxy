using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Web;
using YoAppWebProxy.Models;

namespace YoAppWebProxy
{
    public class AgribankBearerConnector
    {
        public AgribankTokenResponse GetTokenResponse(AgribankTokenRequest agribankTokenRequest)
        {
            AgribankTokenResponse agribankTokenResponse = new AgribankTokenResponse();
            AgribankTokenRequest tokenRequest = new AgribankTokenRequest();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string url = String.Format("http://196.2.66.6:8010/api/fetchToken");
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Timeout = 120000;                
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.PreAuthenticate = true;
                httpWebRequest.Method = "POST";
                httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;

                string json = JsonConvert.SerializeObject(agribankTokenRequest);

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        agribankTokenResponse = JsonConvert.DeserializeObject<AgribankTokenResponse>(result);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return agribankTokenResponse;
        }        
    }
}