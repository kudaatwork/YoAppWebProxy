using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Web;
using YoAppWebProxy.Api_Credentials;
using YoAppWebProxy.Logs;
using YoAppWebProxy.Models.MerchantBank;
using YoAppWebProxy.Models.MerchantBank_Agents;
using YoAppWebProxy.Models.MerchantBank_Clients;
using YoAppWebProxy.Models.MerchantBank_Login;
using YoAppWebProxy.Models.MerchantBank_Recipients;
using YoAppWebProxy.Models.MerchantBank_Transaction;

namespace YoAppWebProxy.Connectors
{
    public class MerchantBankConnector
    {
        public Client GetClientByClientId(string serviceProvider, ClientRequest clientRequest, string token)
        {
            Client client = new Client();
            MetBankCredentials metBankCredentials = new MetBankCredentials();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string queryRequest = clientRequest.clientId;

                string url = String.Format("http://62.171.136.41:8203/clients/" + queryRequest);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Timeout = 120000;
                //SetBasicAuthHeader(httpWebRequest, eSolutionsCredentials.Username, eSolutionsCredentials.Password);
                httpWebRequest.Headers.Add("Authorization", "Bearer " + token);
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
                            client = JsonConvert.DeserializeObject<Client>(result);
                        }
                    }                
            }
            catch (Exception e)
            {
                Log.HttpError("Exception", serviceProvider, "Message: " + e.Message + ", InnerException: " + e.InnerException + ", StackTrace: " + e.StackTrace);
            }

            return client;
        }

        public ClientResponse GetClientByPhoneNumber(string serviceProvider, ClientRequest clientRequest, string token)
        {
            ClientResponse clientResponse = new ClientResponse();
            MetBankCredentials metBankCredentials = new MetBankCredentials();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string queryRequest = clientRequest.phoneNumber.Trim();

                string url = String.Format("https://api-remit263.metbank.co.zw/api/v1/registration/clients/phone-number/" + queryRequest);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Timeout = 120000;
                //SetBasicAuthHeader(httpWebRequest, eSolutionsCredentials.Username, eSolutionsCredentials.Password);
                httpWebRequest.Headers.Add("Authorization", "Bearer " + token);
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
                            clientResponse = JsonConvert.DeserializeObject<ClientResponse>(result);
                        }
                    }
            }
            catch (Exception e)
            {
                Log.HttpError("Exception", serviceProvider, "Message: " + e.Message + ", InnerException: " + e.InnerException + ", StackTrace: " + e.StackTrace);
            }

            return clientResponse;
        }

        public ClientResponse GetClientByNationalId(string serviceProvider, ClientRequest clientRequest, string token)
        {
            ClientResponse clientResponse = new ClientResponse();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string queryRequest = clientRequest.nationalId;

                string url = String.Format("https://api-remit263.metbank.co.zw/api/v1/registration/clients/national-id/" + queryRequest);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Timeout = 120000;
                //SetBasicAuthHeader(httpWebRequest, eSolutionsCredentials.Username, eSolutionsCredentials.Password);
                httpWebRequest.Headers.Add("Authorization", "Bearer " + token);
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
                            clientResponse = JsonConvert.DeserializeObject<ClientResponse>(result);
                        }
                    }
            }
            catch (Exception e)
            {
                Log.HttpError("Exception", serviceProvider, "Message: " + e.Message + ", InnerException: " + e.InnerException + ", StackTrace: " + e.StackTrace);
            }

            return clientResponse;
        }

        public Recipient GetRecipientById(string serviceProvider, ClientRequest clientRequest, string token)
        {
            Recipient recipient = new Recipient();
            MetBankCredentials metBankCredentials = new MetBankCredentials();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string queryRequest = clientRequest.clientId;

                string url = String.Format("https://api-remit263.metbank.co.zw/api/v1/transactions-service/recipients/" + queryRequest);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Timeout = 120000;
                //SetBasicAuthHeader(httpWebRequest, eSolutionsCredentials.Username, eSolutionsCredentials.Password);
                httpWebRequest.Headers.Add("Authorization", "Bearer " + token);
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
                            recipient = JsonConvert.DeserializeObject<Recipient>(result);
                        }
                    }
            }
            catch (Exception e)
            {
                Log.HttpError("Exception", serviceProvider, "Message: " + e.Message + ", InnerException: " + e.InnerException + ", StackTrace: " + e.StackTrace);
            }

            return recipient;
        }

        public List<Recipient> GetRecipientByClientId(string serviceProvider, ClientRequest clientRequest, string token)
        {
            List<Recipient> recipients = new List<Recipient>();
            MetBankCredentials metBankCredentials = new MetBankCredentials();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string queryRequest = clientRequest.clientId;

                string url = String.Format("https://api-remit263.metbank.co.zw/api/v1/transactions-service/recipients/client/" + queryRequest);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Timeout = 120000;
                //SetBasicAuthHeader(httpWebRequest, eSolutionsCredentials.Username, eSolutionsCredentials.Password);
                httpWebRequest.Headers.Add("Authorization", "Bearer " + token);
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
                            recipients = JsonConvert.DeserializeObject<List<Recipient>>(result);
                        }
                    }
            }
            catch (Exception e)
            {
                Log.HttpError("Exception", serviceProvider, "Message: " + e.Message + ", InnerException: " + e.InnerException + ", StackTrace: " + e.StackTrace);
            }

            return recipients;
        }

        public Recipient GetRecipientByPhoneNumber(string serviceProvider, ClientRequest clientRequest, string token)
        {
            Recipient recipient = new Recipient();
            MetBankCredentials metBankCredentials = new MetBankCredentials();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string queryRequest = clientRequest.phoneNumber;

                string url = String.Format("https://api-remit263.metbank.co.zw/api/v1/transactions-service/recipients/phone-number/" + queryRequest);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Timeout = 120000;
                //SetBasicAuthHeader(httpWebRequest, eSolutionsCredentials.Username, eSolutionsCredentials.Password);
                httpWebRequest.Headers.Add("Authorization", "Bearer " + token);
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
                            recipient = JsonConvert.DeserializeObject<Recipient>(result);
                        }
                    }
            }
            catch (Exception e)
            {
                Log.HttpError("Exception", serviceProvider, "Message: " + e.Message + ", InnerException: " + e.InnerException + ", StackTrace: " + e.StackTrace);
            }

            return recipient;
        }

        public Client ApproveClient(string serviceProvider, ClientRequest clientRequest, string token)
        {
            Client client = new Client();
            MetBankCredentials metBankCredentials = new MetBankCredentials();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string queryRequest = clientRequest.clientId;

                string url = String.Format("http://62.171.136.41:8203/clients/approve-client/" + queryRequest);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Timeout = 120000;
                //SetBasicAuthHeader(httpWebRequest, eSolutionsCredentials.Username, eSolutionsCredentials.Password);
                httpWebRequest.Headers.Add("Authorization", "Bearer " + token);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "PUT";
                httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;

                string json = JsonConvert.SerializeObject(clientRequest);

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();
                            client = JsonConvert.DeserializeObject<Client>(result);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.HttpError("Exception", serviceProvider, "Message: " + e.Message + ", InnerException: " + e.InnerException + ", StackTrace: " + e.StackTrace);
            }

            return client;
        }

        public Client DisApproveClient(string serviceProvider, ClientRequest clientRequest, string token)
        {
            Client client = new Client();
            MetBankCredentials metBankCredentials = new MetBankCredentials();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string queryRequest = clientRequest.clientId;

                string url = String.Format("http://62.171.136.41:8203/clients/disapprove-client/" + queryRequest);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Timeout = 120000;
                //SetBasicAuthHeader(httpWebRequest, eSolutionsCredentials.Username, eSolutionsCredentials.Password);
                httpWebRequest.Headers.Add("Authorization", "Bearer " + token);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "PUT";
                httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;

                string json = JsonConvert.SerializeObject(clientRequest);

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();
                            client = JsonConvert.DeserializeObject<Client>(result);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.HttpError("Exception", serviceProvider, "Message: " + e.Message + ", InnerException: " + e.InnerException + ", StackTrace: " + e.StackTrace);
            }

            return client;
        }

        public RegistrationCountResponse GetNumberOfRegisteredClients(string serviceProvider, ClientRequest clientRequest, string token)
        {
            RegistrationCountResponse registrationCountResponse = new RegistrationCountResponse();
            MetBankCredentials metBankCredentials = new MetBankCredentials();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                //string queryRequest = clientRequest.clientId;

                string url = String.Format("http://62.171.136.41:8203/agents/number-of-registered-agents");
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Timeout = 120000;
                //SetBasicAuthHeader(httpWebRequest, eSolutionsCredentials.Username, eSolutionsCredentials.Password);
                httpWebRequest.Headers.Add("Authorization", "Bearer " + token);
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
                            registrationCountResponse = JsonConvert.DeserializeObject<RegistrationCountResponse>(result);
                        }
                    }
            }
            catch (Exception e)
            {
                Log.HttpError("Exception", serviceProvider, "Message: " + e.Message + ", InnerException: " + e.InnerException + ", StackTrace: " + e.StackTrace);
            }

            return registrationCountResponse;
        }

        public ClientSearchResponse ClientLogin(string serviceProvider, ClientRequest clientRequest, string token)
        {
            ClientSearchResponse clientSearchResponse = new ClientSearchResponse();
            MetBankCredentials metBankCredentials = new MetBankCredentials();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string queryRequest = clientRequest.phoneNumber;

                string url = String.Format("http://62.171.136.41:8203/clients/phone-number-login/" + queryRequest);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Timeout = 120000;
                //SetBasicAuthHeader(httpWebRequest, eSolutionsCredentials.Username, eSolutionsCredentials.Password);
                httpWebRequest.Headers.Add("Authorization", "Bearer " + token);
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
                            clientSearchResponse = JsonConvert.DeserializeObject<ClientSearchResponse>(result);
                        }
                    }
            }
            catch (Exception e)
            {
                Log.HttpError("Exception", serviceProvider, "Message: " + e.Message + ", InnerException: " + e.InnerException + ", StackTrace: " + e.StackTrace);
            }

            return clientSearchResponse;
        }
        
        public PageClient GetClientsByPagesAndSize(string serviceProvider, ClientRequest clientRequest, string token)
        {
            PageClient pageClient = new PageClient();
            MetBankCredentials metBankCredentials = new MetBankCredentials();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string page = clientRequest.page;
                string size = clientRequest.size;

                string url = String.Format("http://62.171.136.41:8203/clients/" + page + "/" + size);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Timeout = 120000;
                //SetBasicAuthHeader(httpWebRequest, eSolutionsCredentials.Username, eSolutionsCredentials.Password);
                httpWebRequest.Headers.Add("Authorization", "Bearer " + token);
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
                            pageClient = JsonConvert.DeserializeObject<PageClient>(result);
                        }
                    }
            }
            catch (Exception e)
            {
                Log.HttpError("Exception", serviceProvider, "Message: " + e.Message + ", InnerException: " + e.InnerException + ", StackTrace: " + e.StackTrace);
            }

            return pageClient;
        }

        public Client UpdateRecipientsByPhoneNumber(string serviceProvider, ClientRequest clientRequest, string token)
        {
            Client client = new Client();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string queryRequest = clientRequest.clientId;

                string url = String.Format("http://62.171.136.41:8203/recipients/" + queryRequest);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Timeout = 120000;
                //SetBasicAuthHeader(httpWebRequest, eSolutionsCredentials.Username, eSolutionsCredentials.Password);
                httpWebRequest.Headers.Add("Authorization", "Bearer " + token);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "PUT";

                string json = JsonConvert.SerializeObject(clientRequest);

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();
                            client = JsonConvert.DeserializeObject<Client>(result);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.HttpError("Exception", serviceProvider, "Message: " + e.Message + ", InnerException: " + e.InnerException + ", StackTrace: " + e.StackTrace);
            }

            return client;
        }

        public Client PutClient(string serviceProvider, ClientRequest clientRequest, string token)
        {
            Client client = new Client();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string queryRequest = clientRequest.clientId;

                string url = String.Format("http://62.171.136.41:8203/clients/" + queryRequest);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Timeout = 120000;
                //SetBasicAuthHeader(httpWebRequest, eSolutionsCredentials.Username, eSolutionsCredentials.Password);
                httpWebRequest.Headers.Add("Authorization", "Bearer " + token);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "PUT";

                string json = JsonConvert.SerializeObject(clientRequest);

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();
                            client = JsonConvert.DeserializeObject<Client>(result);
                        }
                    }                   
                }
            }
            catch (Exception e)
            {
                Log.HttpError("Exception", serviceProvider, "Message: " + e.Message + ", InnerException: " + e.InnerException + ", StackTrace: " + e.StackTrace);
            }

            return client;
        }

        public Client DeleteClient(ClientRequest clientRequest)
        {
            Client client = new Client();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string queryRequest = clientRequest.clientId;

                string url = String.Format("http://62.171.136.41:8203/clients/" + queryRequest);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Timeout = 120000;
                //SetBasicAuthHeader(httpWebRequest, eSolutionsCredentials.Username, eSolutionsCredentials.Password);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "DELETE";

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                    }
                }               
            }
            catch (Exception e)
            {
                ESolutionsLog.HttpErrorLog(e.Message);
            }

            return client;
        }

        public Client RegisterClient(string serviceProvider, RegisterClientRequest registerClientRequest, string token)
        {
            Client client = new Client();
            MetBankCredentials metBankCredentials = new MetBankCredentials();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );
                
                string url = String.Format("https://api-remit263.metbank.co.zw/api/v1/registration/clients/agent-register");
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Timeout = 120000;            
                httpWebRequest.Headers.Add("Authorization", "Bearer " + token);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                string json = JsonConvert.SerializeObject(registerClientRequest);

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    if (httpResponse.StatusCode == HttpStatusCode.OK || httpResponse.StatusCode ==HttpStatusCode.Created)
                    {
                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();
                            client = JsonConvert.DeserializeObject<Client>(result);
                        }
                    }                    
                }
            }
            catch (Exception e)
            {
                Log.HttpError("Exception", serviceProvider, "Message: " + e.Message + ", InnerException: " + e.InnerException + ", StackTrace: " + e.StackTrace);
            }

            return client;
        }

        public ResultDTO GetNewToken(string serviceProvider, RefresherTokenRequest refresherTokenRequest)
        {
            ResultDTO resultDTO = new ResultDTO();
            MetBankCredentials metBankCredentials = new MetBankCredentials();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string url = String.Format("http://62.171.136.41:8203/refresh-token/" + refresherTokenRequest.token + "/" + refresherTokenRequest.username);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Timeout = 120000;
                //httpWebRequest.Headers.Add("Authorization", "Bearer " + metBankCredentials.AccessToken);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                string json = JsonConvert.SerializeObject(refresherTokenRequest);

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();
                            resultDTO = JsonConvert.DeserializeObject<ResultDTO>(result);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.HttpError("Exception", serviceProvider, "Message: " + e.Message + ", InnerException: " + e.InnerException + ", StackTrace: " + e.StackTrace);
            }

            return resultDTO;
        }

        public ResultDTO GetToken(string serviceProvider, UserLogin userLogin)
        {
            ResultDTO resultDTO = new ResultDTO();
            MetBankCredentials metBankCredentials = new MetBankCredentials();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string url = String.Format("https://api-remit263.metbank.co.zw/api/v1/oauth-server/client-login");
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Timeout = 120000;
               // httpWebRequest.Headers.Add("Authorization", "Bearer " + metBankCredentials.AccessToken);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                string json = JsonConvert.SerializeObject(userLogin);

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();
                            resultDTO = JsonConvert.DeserializeObject<ResultDTO>(result);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.HttpError("Exception", serviceProvider, "Message: " + e.Message + ", InnerException: " + e.InnerException + ", StackTrace: " + e.StackTrace);
            }

            return resultDTO;
        }

        public Agent RegisterAgent(string serviceProvider, RegisterAgentRequest registerAgentRequest, string token)
        {
            Agent agent = new Agent();
            MetBankCredentials metBankCredentials = new MetBankCredentials();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string url = String.Format("http://62.171.136.41:8203/agents/register");
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Timeout = 120000;
                httpWebRequest.Headers.Add("Authorization", "Bearer " + token);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                string json = JsonConvert.SerializeObject(registerAgentRequest);

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();
                            agent = JsonConvert.DeserializeObject<Agent>(result);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.HttpError("Exception", serviceProvider, "Message: " + e.Message + ", InnerException: " + e.InnerException + ", StackTrace: " + e.StackTrace);
            }

            return agent;
        }

        public Recipient RegisterRecipient(string serviceProvider, RecipientsRequest recipientsRequest, string token)
        {
            Recipient recipient = new Recipient();
            MetBankCredentials metBankCredentials = new MetBankCredentials();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string url = String.Format("https://api-remit263.metbank.co.zw/api/v1/transactions-service/recipients");
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Timeout = 120000;
                httpWebRequest.Headers.Add("Authorization", "Bearer " + token);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                string json = JsonConvert.SerializeObject(recipientsRequest);

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    if (httpResponse.StatusCode == HttpStatusCode.OK || httpResponse.StatusCode == HttpStatusCode.Created)
                    {
                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();
                            recipient = JsonConvert.DeserializeObject<Recipient>(result);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.HttpError("Exception", serviceProvider, "Message: " + e.Message + ", InnerException: " + e.InnerException + ", StackTrace: " + e.StackTrace);
            }

            return recipient;
        }

        public TransactionResponse ApproveSendMoney(string serviceProvider, ApproveSendMoneyRequest approveSendMoneyRequest, string token)
        {
            TransactionResponse transactionResponse = new TransactionResponse();
            MetBankCredentials metBankCredentials = new MetBankCredentials();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string url = String.Format("http://62.171.136.41:8205/transactions/send-money/approve/transaction");
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Timeout = 120000;
                httpWebRequest.Headers.Add("Authorization", "Bearer " + token);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                string json = JsonConvert.SerializeObject(approveSendMoneyRequest);

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();
                            transactionResponse = JsonConvert.DeserializeObject<TransactionResponse>(result);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.HttpError("Exception", serviceProvider, "Message: " + e.Message + ", InnerException: " + e.InnerException + ", StackTrace: " + e.StackTrace);
            }

            return transactionResponse;
        }

        public TransactionResponse SendMoney(string serviceProvider, SendMoneyRequest sendMoneyRequest, string token)
        {
            TransactionResponse transactionResponse = new TransactionResponse();
            MetBankCredentials metBankCredentials = new MetBankCredentials();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string url = String.Format("https://api-remit263.metbank.co.zw/api/v1/transactions-service/transactions/send-money/transaction");
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Timeout = 120000;
                httpWebRequest.Headers.Add("Authorization", "Bearer " + token);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                string json = JsonConvert.SerializeObject(sendMoneyRequest);

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    if (httpResponse.StatusCode == HttpStatusCode.OK || httpResponse.StatusCode == HttpStatusCode.Created)
                    {
                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();
                            transactionResponse = JsonConvert.DeserializeObject<TransactionResponse>(result);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.HttpError("Exception", serviceProvider, "Message: " + e.Message + ", InnerException: " + e.InnerException + ", StackTrace: " + e.StackTrace);
            }

            return transactionResponse;
        }

        public TransactionResponse ReceiveMoney(string serviceProvider, ReceiveMoneyRequest receiveMoneyRequest, string token)
        {
            TransactionResponse transactionResponse = new TransactionResponse();
            MetBankCredentials metBankCredentials = new MetBankCredentials();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string url = String.Format("https://api-remit263.metbank.co.zw/api/v1/transactions-service/transactions/receive-money/transaction");
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Timeout = 120000;
                httpWebRequest.Headers.Add("Authorization", "Bearer " + token);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                string json = JsonConvert.SerializeObject(receiveMoneyRequest);

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    if (httpResponse.StatusCode == HttpStatusCode.OK || httpResponse.StatusCode == HttpStatusCode.Created)
                    {
                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();
                            transactionResponse = JsonConvert.DeserializeObject<TransactionResponse>(result);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.HttpError("Exception", serviceProvider, "Message: " + e.Message + ", InnerException: " + e.InnerException + ", StackTrace: " + e.StackTrace);
            }

            return transactionResponse;
        }

        public ReceiveMoneyPreAuthResponse PreAuthReceiveMoney(string serviceProvider, ReceiveMoneyPreAuthRequest receiveMoneyPreAuthRequest, string token)
        {
            ReceiveMoneyPreAuthResponse receiveMoneyPreAuth = new ReceiveMoneyPreAuthResponse();
            MetBankCredentials metBankCredentials = new MetBankCredentials();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string url = String.Format("https://api-remit263.metbank.co.zw/api/v1/transactions-service/transactions/receive-money/preauth");
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Timeout = 120000;
                httpWebRequest.Headers.Add("Authorization", "Bearer " + token);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                string json = JsonConvert.SerializeObject(receiveMoneyPreAuthRequest);

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    if (httpResponse.StatusCode == HttpStatusCode.OK || httpResponse.StatusCode == HttpStatusCode.Created)
                    {
                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();
                            receiveMoneyPreAuth = JsonConvert.DeserializeObject<ReceiveMoneyPreAuthResponse>(result);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.HttpError("Exception", serviceProvider, "Message: " + e.Message + ", InnerException: " + e.InnerException + ", StackTrace: " + e.StackTrace);
            }

            return receiveMoneyPreAuth;
        }

        public SendMoneyPreAuthResponse PreAuthSendMoney(string serviceProvider, SendMoneyPreAuthRequest sendMoneyPreAuthRequest, string token)
        {
            SendMoneyPreAuthResponse sendMoneyPreAuthResponse = new SendMoneyPreAuthResponse();
            MetBankCredentials metBankCredentials = new MetBankCredentials();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string url = String.Format("https://api-remit263.metbank.co.zw/api/v1/transactions-service/transactions/send-money/preauth");
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Timeout = 120000;
                httpWebRequest.Headers.Add("Authorization", "Bearer " + token);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                string json = JsonConvert.SerializeObject(sendMoneyPreAuthRequest);

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    if (httpResponse.StatusCode == HttpStatusCode.OK || httpResponse.StatusCode == HttpStatusCode.Created)
                    {
                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();
                            sendMoneyPreAuthResponse = JsonConvert.DeserializeObject<SendMoneyPreAuthResponse>(result);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.HttpError("Exception", serviceProvider, "Message: " + e.Message + ", InnerException: " + e.InnerException + ", StackTrace: " + e.StackTrace);
            }

            return sendMoneyPreAuthResponse;
        }

        public ClientSendMoneyResponse ClientPreAuthSendMoney(string serviceProvider, ClientSendMoneyPreAuthRequest clientSendMoneyPreAuthRequest, string token)
        {
            ClientSendMoneyResponse clientSendMoneyResponse = new ClientSendMoneyResponse();
            MetBankCredentials metBankCredentials = new MetBankCredentials();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string url = String.Format("http://62.171.136.41:8205/transactions/client-send-money/transaction");
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Timeout = 120000;
                httpWebRequest.Headers.Add("Authorization", "Bearer " + token);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                string json = JsonConvert.SerializeObject(clientSendMoneyPreAuthRequest);

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();
                            clientSendMoneyResponse = JsonConvert.DeserializeObject<ClientSendMoneyResponse>(result);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.HttpError("Exception", serviceProvider, "Message: " + e.Message + ", InnerException: " + e.InnerException + ", StackTrace: " + e.StackTrace);
            }

            return clientSendMoneyResponse;
        }

        public TransactionResponse ClientSendMoney(string serviceProvider, ClientSendMoneyRequest clientSendMoneyRequest, string token)
        {
            TransactionResponse transactionResponse = new TransactionResponse();
            MetBankCredentials metBankCredentials = new MetBankCredentials();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string url = String.Format("http://62.171.136.41:8205/transactions/client-send-money/transaction");
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Timeout = 120000;
                httpWebRequest.Headers.Add("Authorization", "Bearer " + token);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                string json = JsonConvert.SerializeObject(clientSendMoneyRequest);

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();
                            transactionResponse = JsonConvert.DeserializeObject<TransactionResponse>(result);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.HttpError("Exception", serviceProvider, "Message: " + e.Message + ", InnerException: " + e.InnerException + ", StackTrace: " + e.StackTrace);
            }

            return transactionResponse;
        }

        public CalculateCurrencyRateResponse CalculateCurrencyRate(string serviceProvider, CalculateCurrencyRateRequest calculateCurrencyRateRequest, string token)
        {
            CalculateCurrencyRateResponse currencyRateResponse = new CalculateCurrencyRateResponse();
            MetBankCredentials metBankCredentials = new MetBankCredentials();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string url = String.Format("http://62.171.136.41:8205/transactions/client-send-money/transaction");
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Timeout = 120000;
                httpWebRequest.Headers.Add("Authorization", "Bearer " + token);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                string json = JsonConvert.SerializeObject(calculateCurrencyRateRequest);

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();
                            currencyRateResponse = JsonConvert.DeserializeObject<CalculateCurrencyRateResponse>(result);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.HttpError("Exception", serviceProvider, "Message: " + e.Message + ", InnerException: " + e.InnerException + ", StackTrace: " + e.StackTrace);
            }

            return currencyRateResponse;
        } 

        public void SetBasicAuthHeader(WebRequest request, String userName, String userPassword)
        {
            string authInfo = userName + ":" + userPassword;
            authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
            request.Headers["Authorization"] = "Basic " + authInfo;
        }
    }
}