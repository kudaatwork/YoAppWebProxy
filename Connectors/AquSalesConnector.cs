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
        public APITransactionResponse PostCBZRedemption(SaleTransaction saleTransaction, string serviceProvider)
        {
            APITransactionResponse transactionResponse = new APITransactionResponse();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string url = String.Format("http://102.130.120.163:8060/Yomoney/SaleTransaction");
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

        public APITransactionResponse PostExpense(ExpenseTransactionVM expenseTransaction, string serviceProvider)
        {
            APITransactionResponse transactionResponse = new APITransactionResponse();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string url = String.Format("http://192.168.100.75:5000/Yomoney/ExpenseTransaction");
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.PreAuthenticate = true;
                httpWebRequest.Timeout = 120000;
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;
                httpWebRequest.CookieContainer = new CookieContainer();
                Cookie cookie = new Cookie("AspxAutoDetectCookieSupport", "1");
                cookie.Domain = "192.168.100.75";
                httpWebRequest.CookieContainer.Add(cookie);

                string json = JsonConvert.SerializeObject(expenseTransaction);

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
                Log.HttpError("Aqu-HttpError", serviceProvider, e.Message + e.InnerException + e.StackTrace);
            }

            return transactionResponse;
        }

        public APITransactionResponse CheckUser(User user, string serviceProvider)
        {
            APITransactionResponse transactionResponse = new APITransactionResponse();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string url = String.Format("http://102.130.120.163:8094/Yomoney/CheckUserName");
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

                string json = JsonConvert.SerializeObject(user);

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
                Log.HttpError("Aqu-HttpError", serviceProvider, e.Message + e.InnerException + e.StackTrace);
            }

            return transactionResponse;
        }

        public APITransactionResponse PostCBZRedemption2(SaleTransaction saleTransaction, string serviceProvider)
        {
            APITransactionResponse transactionResponse = new APITransactionResponse();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string url = String.Format("http://102.130.120.163:8087/Yomoney/SaleTransaction");
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

        public APITransactionResponse PostCBZReversal(SaleTransaction saleTransaction, string serviceProvider)
        {
            APITransactionResponse transactionResponse = new APITransactionResponse();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string url = String.Format("http://102.130.120.163:8060/Yomoney/SaleTransaction");
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

        public APITransactionResponse PostCBZReversal2(SaleTransaction saleTransaction, string serviceProvider)
        {
            APITransactionResponse transactionResponse = new APITransactionResponse();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string url = String.Format("http://102.130.120.163:8087/Yomoney/SaleTransaction");
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

        public APITransactionResponse PostOhlangaRedemption(SaleTransaction saleTransaction, string serviceProvider)
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

        public APITransactionResponse PostCBZGRV(GRVTransaction grvTransaction, string serviceProvider)
        {
            APITransactionResponse transactionResponse = new APITransactionResponse();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string url = String.Format("http://102.130.120.163:8060/Yomoney/GRVTransaction");
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

        public APITransactionResponse PostCBZGRV2(GRVTransaction grvTransaction, string serviceProvider)
        {
            APITransactionResponse transactionResponse = new APITransactionResponse();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string url = String.Format("http://102.130.120.163:8087/Yomoney/GRVTransaction");
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

        public APITransactionResponse PostOhlangaGRV(GRVTransaction grvTransaction, string serviceProvider)
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

        public APITransactionResponse PostOhlangaPayment(GRVTransaction grvTransaction, string serviceProvider)
        {
            APITransactionResponse transactionResponse = new APITransactionResponse();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string url = String.Format("http://102.130.120.163:8091/Yomoney/InvoicePayment");
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

        public APITransactionResponse PostOhlangaSupplierCreation(GRVTransaction grvTransaction, string serviceProvider)
        {
            APITransactionResponse transactionResponse = new APITransactionResponse();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string url = String.Format("http://102.130.120.163:8091/Yomoney/SupplierCreation");
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

        public APITransactionResponse CreateCompany(Company company, string serviceProvider)
        {
            APITransactionResponse transactionResponse = new APITransactionResponse();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string url = String.Format("http://102.130.120.163:8094/Yomoney/CreateCompany");
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

                string json = JsonConvert.SerializeObject(company);

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
                Log.HttpError("Aqu-HttpError", serviceProvider, e.Message + e.InnerException + e.StackTrace);
            }

            return transactionResponse;
        }

        public APITransactionResponse PostPayment(PaymentTransaction paymentTransaction, string serviceProvider)
        {
            APITransactionResponse transactionResponse = new APITransactionResponse();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string url = String.Format("http://102.130.120.163:8094/Yomoney/AqusaleslitePayment");
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

                string json = JsonConvert.SerializeObject(paymentTransaction);

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
                Log.HttpError("Aqu-HttpError", serviceProvider, "Message: " + e.Message + ", InnerException: " + e.InnerException + ", StackStrace: " + e.StackTrace);
            }

            return transactionResponse;
        }

        public void WebsiteRedirect(string serviceProvider)
        {
            // APITransactionResponse transactionResponse = new APITransactionResponse();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );               

                string url = String.Format("https://www.aqusales.com/wp-json/wcra/v1/AqusalesRedirect/?redirectUrl=http://102.130.120.163:8094/login");
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Timeout = 120000;
                //SetBasicAuthHeader(httpWebRequest, eSolutionsCredentials.Username, eSolutionsCredentials.Password);
                //httpWebRequest.Headers.Add("Authorization", "Bearer " + token);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "GET";
                httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;

                HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (Stream stream = httpResponse.GetResponseStream())

                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();                           
                        }
                    }
            }
            catch (Exception e)
            {
                Log.HttpError("Exception", serviceProvider, "Message: " + e.Message + ", InnerException: " + e.InnerException + ", StackTrace: " + e.StackTrace);
            }
        }
    }
}