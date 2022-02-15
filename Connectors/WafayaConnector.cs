using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Web;
using YoAppWebProxy.Logs;
using YoAppWebProxy.Models.Wafaya;

namespace YoAppWebProxy.Connectors
{
    public class WafayaConnector
    {
        public WafayaCodeResponse GetCode(WafayaCodeRequest codeRequest, string serviceProvider)
        {
            WafayaCodeResponse codeResponse = new WafayaCodeResponse();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string queryRequest =
                    "response_type=" + codeRequest.response_type +
                    "&client_id=" + codeRequest.client_id +
                    "&redirect_uri" + codeRequest.redirect_uri +
                    "&scope" + codeRequest.scope;

                string url = String.Format("https://www.wa-faya.com/oauth/authorize?" + queryRequest);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.PreAuthenticate = true;
                httpWebRequest.Timeout = 120000;
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "GET";
                httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;                

                HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (Stream stream = httpResponse.GetResponseStream())

                using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    codeResponse = JsonConvert.DeserializeObject<WafayaCodeResponse>(result);
                }
            }
            catch (Exception ex)
            {
                Log.HttpError("Wafaya-HttpError", serviceProvider, ex.Message);
            }

            return codeResponse;
        }

        public WafayaVoucherResponse GetVoucherDetails(WafayaVoucherRequest wafayaVoucherRequest, string serviceProvider)
        {
            WafayaVoucherResponse voucherResponse = new WafayaVoucherResponse();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );
                
                string queryRequest = wafayaVoucherRequest.Voucher;                   

                string url = String.Format("https://www.wa-faya.com/api/voucher/" + queryRequest);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.PreAuthenticate = true;
                httpWebRequest.Timeout = 120000;
                httpWebRequest.Headers.Add("Authorization", "Bearer " + wafayaVoucherRequest.Token);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "GET";
                httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;

                HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (Stream stream = httpResponse.GetResponseStream())

                using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    voucherResponse = JsonConvert.DeserializeObject<WafayaVoucherResponse>(result);
                }
            }
            catch (Exception ex)
            {
                Log.HttpError("Wafaya-HttpError", serviceProvider, ex.Message);
            }

            return voucherResponse;
        }

        public WafayaTokenResponse GetAuthorizationToken(WafayaTokenRequest wafayaTokenRequest, string serviceProvider)
        {
            WafayaTokenResponse tokenResponse = new WafayaTokenResponse();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );
                               
                string url = String.Format("https://www.wa-faya.com/oauth/token");
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.PreAuthenticate = true;
                httpWebRequest.Timeout = 120000;
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;

                string json = JsonConvert.SerializeObject(wafayaTokenRequest);

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        tokenResponse = JsonConvert.DeserializeObject<WafayaTokenResponse>(result);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.HttpError("Wafaya-HttpError", serviceProvider, ex.Message);
            }

            return tokenResponse;
        }

        public WafayaInitializeRedemptionResponse InitializeVoucher(WafayaInitializeRedemptionRequest wafayaInitializeRedemptionRequest, string serviceProvider)
        {
            WafayaInitializeRedemptionResponse initializeRedemptionResponse = new WafayaInitializeRedemptionResponse();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string url = String.Format("https://www.wa-faya.com/api/voucher/" + wafayaInitializeRedemptionRequest.voucher + "/initialize-redemption?disable_messaging=true");
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.PreAuthenticate = true;
                httpWebRequest.Timeout = 120000;
                httpWebRequest.Headers.Add("Authorization", "Bearer " + wafayaInitializeRedemptionRequest.token);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;

                string json = JsonConvert.SerializeObject(wafayaInitializeRedemptionRequest);

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        initializeRedemptionResponse = JsonConvert.DeserializeObject<WafayaInitializeRedemptionResponse>(result);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.HttpError("Wafaya-HttpError", serviceProvider, ex.Message);
            }

            return initializeRedemptionResponse;
        }

        public WafayaFinalizeVoucherResponse FinalizeVoucher(WafayaFinalizeVoucherRequest wafayaFinalizeVoucherRequest, string serviceProvider)
        {
            WafayaFinalizeVoucherResponse finalizeVoucherResponse = new WafayaFinalizeVoucherResponse();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string url = String.Format("https://www.wa-faya.com/api/voucher/" + wafayaFinalizeVoucherRequest.voucher + "/finalize-redemption");
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.PreAuthenticate = true;
                httpWebRequest.Timeout = 120000;
                httpWebRequest.Headers.Add("Authorization", "Bearer " + wafayaFinalizeVoucherRequest.token);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;

                string json = JsonConvert.SerializeObject(wafayaFinalizeVoucherRequest);

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        finalizeVoucherResponse = JsonConvert.DeserializeObject<WafayaFinalizeVoucherResponse>(result);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.HttpError("Wafaya-HttpError", serviceProvider, ex.Message);
            }

            return finalizeVoucherResponse;
        }

        public WafayaResourceOwnerResponse GetPasswordToken(WafayaResourceOwnerRequest wafayaResourceOwnerRequest, string serviceProvider)
        {
            WafayaResourceOwnerResponse resourceOwnerResponse = new WafayaResourceOwnerResponse();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string url = String.Format("https://www.wa-faya.com/oauth/token");
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.PreAuthenticate = true;
                httpWebRequest.Timeout = 120000;
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;

                string json = JsonConvert.SerializeObject(wafayaResourceOwnerRequest);

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        resourceOwnerResponse = JsonConvert.DeserializeObject<WafayaResourceOwnerResponse>(result);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.HttpError("Wafaya-HttpError", serviceProvider, ex.Message);
            }

            return resourceOwnerResponse;
        }

        public WafayaRefresherTokenResponse GetNewToken(WafayaRefresherTokenRequest wafayaRefresherTokenRequest, string serviceProvider)
        {
            WafayaRefresherTokenResponse refresherTokenResponse = new WafayaRefresherTokenResponse();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string url = String.Format("https://www.wa-faya.com/oauth/token");
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.PreAuthenticate = true;
                httpWebRequest.Timeout = 120000;
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;

                string json = JsonConvert.SerializeObject(wafayaRefresherTokenRequest);

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        refresherTokenResponse = JsonConvert.DeserializeObject<WafayaRefresherTokenResponse>(result);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.HttpError("Wafaya-HttpError", serviceProvider, ex.Message);
            }

            return refresherTokenResponse;
        }
    }
}