using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Web;
using System.Web.Http;
using YoAppWebProxy.Api_Credentials;
using YoAppWebProxy.Helpful_Functions;
using YoAppWebProxy.Logs;
using YoAppWebProxy.Models.BulkPayments;

namespace YoAppWebProxy.Connectors
{
    public class BulkPaymentsConnector
    {
        BulkPaymentsMethods bulkPayments = new BulkPaymentsMethods();

        public AATBatchReceiptAcknowledgemet PostAATBatch(AATBatchRequest batchRequest, string serviceProvider)
        {
            AATBatchReceiptAcknowledgemet batchReceiptAcknowledgemet = new AATBatchReceiptAcknowledgemet();            
            AATBatchRequestToServer batchRequestToServer = new AATBatchRequestToServer();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
               (
                   delegate { return true; }
               );

                string url = String.Format("");
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Accept = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.Timeout = 120000;
                SetBasicAuthHeader(httpWebRequest, BulkPaymentsCredentials.Username, BulkPaymentsCredentials.Password);
                                
                var serializedRequestObject = JsonConvert.SerializeObject(batchRequest.Request);
                var enryptedObject = bulkPayments.Encrypt(serializedRequestObject, "KEY"); // Key to be added (Encryption)

                batchRequestToServer.InstitutionID = batchRequest.InstitutionID;
                batchRequestToServer.ExtReferenceNo = batchRequest.ExtReferenceNo;
                batchRequestToServer.Request = enryptedObject;

                string json = JsonConvert.SerializeObject(batchRequestToServer, Formatting.Indented);
                httpWebRequest.ContentLength = json.Length;

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

                        // Decryption
                        var serializedResponseObject = JsonConvert.SerializeObject(batchReceiptAcknowledgemet);
                        var decryptedObject = bulkPayments.Decrypt(serializedResponseObject, "KEY");
                        batchReceiptAcknowledgemet = JsonConvert.DeserializeObject<AATBatchReceiptAcknowledgemet>(decryptedObject);
                    }
                }
            }
            catch (Exception e)
            {
                Log.HttpError("AATPayment-HttpError", serviceProvider, e.Message);
            }

            return batchReceiptAcknowledgemet;
        }

        [NonAction]
        public void SetBasicAuthHeader(WebRequest request, String userName, String userPassword)
        {
            string authInfo = userName + ":" + userPassword;
            authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
            request.Headers["Authorization"] = "Basic " + authInfo;
        }

        public AATBatchProgressResponse BatchStatusCheck(AATBatchStatusCheckRequest batchStatusCheckRequest, string serviceProvider)
        {
            AATBatchProgressResponse batchProgressResponse = new AATBatchProgressResponse();
            
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
               (
                   delegate { return true; }
               );

                string url = String.Format("");
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Accept = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.Timeout = 120000;
                SetBasicAuthHeader(httpWebRequest, BulkPaymentsCredentials.Username, BulkPaymentsCredentials.Password);
                               
                string json = JsonConvert.SerializeObject(batchStatusCheckRequest, Formatting.Indented);
                httpWebRequest.ContentLength = json.Length;

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        batchProgressResponse = JsonConvert.DeserializeObject<AATBatchProgressResponse>(result);
                    }
                }
            }
            catch (Exception e)
            {
                Log.HttpError("AATProgress-HttpError", serviceProvider, e.Message);
            }

            return batchProgressResponse;
        }

        public IBTResponse IBTBulkPayment(IBTRequest iBTRequest, string serviceProvider)
        {
            IBTResponse iBTResponse = new IBTResponse();
            IBTRequestToServer iBTRequestToServer = new IBTRequestToServer();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
               (
                   delegate { return true; }
               );

                string url = String.Format("");
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Accept = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.Timeout = 120000;
                SetBasicAuthHeader(httpWebRequest, BulkPaymentsCredentials.Username, BulkPaymentsCredentials.Password);

                var serializedRequestObject = JsonConvert.SerializeObject(iBTRequest.Request);
                var enryptedObject = bulkPayments.Encrypt(serializedRequestObject, "KEY"); // Key to be added (Encryption)

                iBTRequestToServer.InstitutionID = iBTRequest.InstitutionID;
                iBTRequestToServer.ExtReferenceNo = iBTRequest.ExtReferenceNo;
                iBTRequestToServer.Request = enryptedObject;

                string json = JsonConvert.SerializeObject(iBTRequest, Formatting.Indented);
                httpWebRequest.ContentLength = json.Length;

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        iBTResponse = JsonConvert.DeserializeObject<IBTResponse>(result);

                        // Decryption
                        var serializedResponseObject = JsonConvert.SerializeObject(iBTResponse);
                        var decryptedObject = bulkPayments.Decrypt(serializedResponseObject, "KEY");
                        iBTResponse = JsonConvert.DeserializeObject<IBTResponse>(decryptedObject);
                    }
                }
            }
            catch (Exception e)
            {
                Log.HttpError("IBT-HttpError", serviceProvider, e.Message);
            }

            return iBTResponse;
        }

        public _31_BEQ_1_Response BEQPayment(_31_BEQ_1_Request _31_BEQ_1_Request, string serviceProvider)
        {
            _31_BEQ_1_Response _BEQ_1_Response = new _31_BEQ_1_Response();
            _31_BEQ_1_RequestToServer _31_BEQ_1_RequestToServer = new _31_BEQ_1_RequestToServer();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
               (
                   delegate { return true; }
               );

                string url = String.Format("");
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Accept = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.Timeout = 120000;
                SetBasicAuthHeader(httpWebRequest, BulkPaymentsCredentials.Username, BulkPaymentsCredentials.Password);

                var serializedRequestObject = JsonConvert.SerializeObject(_31_BEQ_1_Request.Request);
                var enryptedObject = bulkPayments.Encrypt(serializedRequestObject, "KEY"); // Key to be added (Encryption)

                _31_BEQ_1_RequestToServer.InstitutionID = _31_BEQ_1_Request.InstitutionID;
                _31_BEQ_1_RequestToServer.ExtReferenceNo = _31_BEQ_1_Request.ExtReferenceNo;
                _31_BEQ_1_RequestToServer.Request = enryptedObject;

                string json = JsonConvert.SerializeObject(_31_BEQ_1_Request, Formatting.Indented);
                httpWebRequest.ContentLength = json.Length;

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        _BEQ_1_Response = JsonConvert.DeserializeObject<_31_BEQ_1_Response>(result);

                        // Decryption
                        var serializedResponseObject = JsonConvert.SerializeObject(_BEQ_1_Response);
                        var decryptedObject = bulkPayments.Decrypt(serializedResponseObject, "KEY");
                        _BEQ_1_Response = JsonConvert.DeserializeObject<_31_BEQ_1_Response>(decryptedObject);
                    }
                }
            }
            catch (Exception e)
            {
                Log.HttpError("BEQ-HttpError", serviceProvider, e.Message);
            }

            return _BEQ_1_Response;
        }

        public void MSTPayment(_38_MST_1_Request _Request, string serviceProvider)
        {
            _38_MST_1_RequestToServer _RequestToServer = new _38_MST_1_RequestToServer();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
               (
                   delegate { return true; }
               );

                string url = String.Format("");
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Accept = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.Timeout = 120000;
                SetBasicAuthHeader(httpWebRequest, BulkPaymentsCredentials.Username, BulkPaymentsCredentials.Password);

                var serializedRequestObject = JsonConvert.SerializeObject(_Request.Request);
                var enryptedObject = bulkPayments.Encrypt(serializedRequestObject, "KEY"); // Key to be added (Encryption)

                _RequestToServer.InstitutionID = _Request.InstitutionID;
                _RequestToServer.ExtReferenceNo = _Request.ExtReferenceNo;
                _RequestToServer.Request = enryptedObject;

                string json = JsonConvert.SerializeObject(_Request, Formatting.Indented);
                httpWebRequest.ContentLength = json.Length;

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        var _Response = JsonConvert.DeserializeObject<_31_BEQ_1_Response>(result);

                        // Decryption
                        var serializedResponseObject = JsonConvert.SerializeObject(_Response);
                        var decryptedObject = bulkPayments.Decrypt(serializedResponseObject, "KEY");
                        _Response = JsonConvert.DeserializeObject<_31_BEQ_1_Response>(decryptedObject);
                    }
                }
            }
            catch (Exception e)
            {
                Log.HttpError("MST-HttpError", serviceProvider, e.Message);
            }            
        }

        public _107_ABV_1_PossibleResponses ABVPayment(_107_ABV_1_Request _Request, string serviceProvider)
        {
            _107_ABV_1_PossibleResponses _PossibleResponses = new _107_ABV_1_PossibleResponses();
            _107_ABV_1_RequestToServer _RequestToServer = new _107_ABV_1_RequestToServer();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
               (
                   delegate { return true; }
               );

                string url = String.Format("");
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Accept = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.Timeout = 120000;
                SetBasicAuthHeader(httpWebRequest, BulkPaymentsCredentials.Username, BulkPaymentsCredentials.Password);

                var serializedRequestObject = JsonConvert.SerializeObject(_Request.Request);
                var enryptedObject = bulkPayments.Encrypt(serializedRequestObject, "KEY"); // Key to be added (Encryption)

                _RequestToServer.InstitutionID = _Request.InstitutionID;
                _RequestToServer.ExtReferenceNo = _Request.ExtReferenceNo;
                _RequestToServer.Request = enryptedObject;

                string json = JsonConvert.SerializeObject(_Request, Formatting.Indented);
                httpWebRequest.ContentLength = json.Length;

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        _PossibleResponses = JsonConvert.DeserializeObject<_107_ABV_1_PossibleResponses>(result);

                        // Decryption
                        var serializedResponseObject = JsonConvert.SerializeObject(_PossibleResponses);
                        var decryptedObject = bulkPayments.Decrypt(serializedResponseObject, "KEY");
                        _PossibleResponses = JsonConvert.DeserializeObject<_107_ABV_1_PossibleResponses>(decryptedObject);
                    }
                }
            }
            catch (Exception e)
            {
                Log.HttpError("MST-HttpError", serviceProvider, e.Message);
            }

            return _PossibleResponses;
        }

        public _38_MST_6_SuccessResponse MST6Payment(_38_MST_6_Request _Request, string serviceProvider)
        {
            _38_MST_6_SuccessResponse _Response = new _38_MST_6_SuccessResponse();
            _38_MST_6_RequestToServer _RequestToServer = new _38_MST_6_RequestToServer();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
               (
                   delegate { return true; }
               );

                string url = String.Format("");
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Accept = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.Timeout = 120000;
                SetBasicAuthHeader(httpWebRequest, BulkPaymentsCredentials.Username, BulkPaymentsCredentials.Password);

                var serializedRequestObject = JsonConvert.SerializeObject(_Request.Request);
                var enryptedObject = bulkPayments.Encrypt(serializedRequestObject, "KEY"); // Key to be added (Encryption)

                _RequestToServer.InstitutionID = _Request.InstitutionID;
                _RequestToServer.ExtReferenceNo = _Request.ExtReferenceNo;
                _RequestToServer.Request = enryptedObject;

                string json = JsonConvert.SerializeObject(_Request, Formatting.Indented);
                httpWebRequest.ContentLength = json.Length;

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        _Response = JsonConvert.DeserializeObject<_38_MST_6_SuccessResponse>(result);

                        // Decryption
                        var serializedResponseObject = JsonConvert.SerializeObject(_Response);
                        var decryptedObject = bulkPayments.Decrypt(serializedResponseObject, "KEY");
                        _Response = JsonConvert.DeserializeObject<_38_MST_6_SuccessResponse>(decryptedObject);
                    }
                }
            }
            catch (Exception e)
            {
                Log.HttpError("MST-HttpError", serviceProvider, e.Message);
            }

            return _Response;
        }
    }
}