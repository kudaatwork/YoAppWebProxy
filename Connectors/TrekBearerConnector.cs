using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
using YoAppWebProxy.Logs;

namespace YoAppWebProxy
{
    public class TrekBearerConnector
    {
        public TrekBearerTokenResponse GetTokenResponse(TrekBearerTokenRequest trekBearerTokenRequest)
        {
            TrekBearerTokenResponse trekBearerTokenResponse = new TrekBearerTokenResponse();
            
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string url = String.Format("https://agroyield.trek.engineering/api/login");
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Timeout = 120000;
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.PreAuthenticate = true;
                httpWebRequest.Method = "POST";
                httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;

                string json = JsonConvert.SerializeObject(trekBearerTokenRequest);

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        trekBearerTokenResponse = JsonConvert.DeserializeObject<TrekBearerTokenResponse>(result);
                    }
                }
            }
            catch (Exception e)
            {
                TrekLog.HttpErrorLog(e.Message);
            }

            return trekBearerTokenResponse;
        }
    }
}