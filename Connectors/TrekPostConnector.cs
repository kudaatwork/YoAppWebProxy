using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using YoAppWebProxy.Models;
using YoAppWebProxy.Logs;
using System.Net.Security;

namespace YoAppWebProxy.Connectors
{
    public class TrekPostConnector
    {
        public TrekCardTransactionsByDateAndCardNumberResponse GetTrekCardTransactionsByDateAndCardNumber(TrekCardTransactionsByDateAndCardNumberRequest trekCardTransactionsByDateAndCardNumberRequest)
        {
            TrekCardTransactionsByDateAndCardNumberResponse trekCardTransactionsByDateAndCardNumberResponse = new TrekCardTransactionsByDateAndCardNumberResponse();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string url = String.Format("https://agroyield.trek.engineering/api/card/transactions");
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.PreAuthenticate = true;
                httpWebRequest.Timeout = 120000;
                httpWebRequest.Headers.Add("Authorization", "Bearer " + Token.StringToken);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;

                string json = JsonConvert.SerializeObject(trekCardTransactionsByDateAndCardNumberRequest);

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        trekCardTransactionsByDateAndCardNumberResponse = JsonConvert.DeserializeObject<TrekCardTransactionsByDateAndCardNumberResponse>(result);
                    }
                }
            }
            catch (Exception e)
            {
                TrekLog.HttpErrorLog(e.Message);
            }

            return trekCardTransactionsByDateAndCardNumberResponse;
        }

        public TrekCardTransactionsByDateResponse GetTrekCardTransactionsByDates(TrekCardTransactionsByDateRequest trekCardTransactionsByDateRequest)
        {
            TrekCardTransactionsByDateResponse trekCardTransactionsByDateResponse = new TrekCardTransactionsByDateResponse();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string url = String.Format("https://agroyield.trek.engineering/api/transaction/date");
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.PreAuthenticate = true;
                httpWebRequest.Timeout = 120000;
                httpWebRequest.Headers.Add("Authorization", "Bearer " + Token.StringToken);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;

                string json = JsonConvert.SerializeObject(trekCardTransactionsByDateRequest);

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        trekCardTransactionsByDateResponse = JsonConvert.DeserializeObject<TrekCardTransactionsByDateResponse>(result);
                    }
                }
            }
            catch (Exception e)
            {
                TrekLog.HttpErrorLog(e.Message);
            }

            return trekCardTransactionsByDateResponse;
        }
    }
}