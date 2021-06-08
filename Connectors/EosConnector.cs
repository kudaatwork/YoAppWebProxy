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
        public EosRedemptionResponse PostRedemption(EosRedemptionRequest eosRequest, string serviceProvider)
        {
            EosRedemptionResponse eosResponse = new EosRedemptionResponse();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string url = String.Format(" http://62.138.16.229:9005/agroyieldtxn/inputAllocationService/farmerInputAllocationRequest");
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
                        eosResponse = JsonConvert.DeserializeObject<EosRedemptionResponse>(result);
                    }
                }
            }
            catch (Exception e)
            {
                Log.HttpError("Aqu-HttpError", serviceProvider, e.Message);
            }

            return eosResponse;
        }

        public EosReversalResponse PostReversal(EosReversalRequest eosRequest, string serviceProvider)
        {
            EosReversalResponse eosResponse = new EosReversalResponse();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string url = String.Format(" http://62.138.16.229:9005/agroyieldtxn/inputAllocationService/farmerInputAllocationRequest");
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
                        eosResponse = JsonConvert.DeserializeObject<EosReversalResponse>(result);
                    }
                }
            }
            catch (Exception e)
            {
                Log.HttpError("Aqu-HttpError", serviceProvider, e.Message);
            }

            return eosResponse;
        }

        public EosTopupResponse PostTopup(EosTopupRequest eosRequest, string serviceProvider)
        {
            EosTopupResponse eosResponse = new EosTopupResponse();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string url = String.Format(" http://62.138.16.229:9005/agroyieldtxn/inputAllocationService/farmerInputAllocationRequest");
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
                        eosResponse = JsonConvert.DeserializeObject<EosTopupResponse>(result);
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