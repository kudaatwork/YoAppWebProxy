using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Web;
using YoAppWebProxy.Logs;
using YoAppWebProxy.Models;

namespace YoAppWebProxy.Connectors
{
    public class TrekGetConnector
    {
        public TrekDevicesResponse GetAllTrekDevices()
        {
            TrekDevicesResponse trekDevicesResponse = new TrekDevicesResponse();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string url = String.Format("https://agroyield.trek.engineering/api/device/all");
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.PreAuthenticate = true;
                httpWebRequest.Timeout = 120000;
                httpWebRequest.Headers.Add("Authorization", "Bearer " + Token.StringToken);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "GET";
                httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;

                HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (Stream stream = httpResponse.GetResponseStream())

                using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    trekDevicesResponse = JsonConvert.DeserializeObject<TrekDevicesResponse>(result);
                }
            }
            catch (Exception e)
            {
                TrekLog.HttpErrorLog(e.Message);
            }

            return trekDevicesResponse;
        }
    }
}