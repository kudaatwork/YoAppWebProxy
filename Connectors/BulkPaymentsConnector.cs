using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using YoAppWebProxy.Logs;
using YoAppWebProxy.Models.BulkPayments;

namespace YoAppWebProxy.Connectors
{
    public class BulkPaymentsConnector
    {
        public AATBatchReceiptAcknowledgemet PostAATBatch(AATBatchRequest batchRequest)
        {
            AATBatchReceiptAcknowledgemet batchReceiptAcknowledgemet = new AATBatchReceiptAcknowledgemet();

            try
            {
                string url = String.Format("");
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                //httpWebRequest.CookieContainer = new CookieContainer();
                //Cookie cookie = new Cookie("AspxAutoDetectCookieSupport", "1");
                //cookie.Domain = "www.yomoneyservice.com";
                //httpWebRequest.CookieContainer.Add(cookie);

                string json = JsonConvert.SerializeObject(batchRequest, Formatting.Indented);

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        batchReceiptAcknowledgemet = JsonConvert.DeserializeObject<AATBatchReceiptAcknowledgemet>(result);
                    }
                }
            }
            catch (Exception e)
            {
                CbzLog.HttpErrorLog(e.Message);
            }

            return batchReceiptAcknowledgemet;
        }
    }
}