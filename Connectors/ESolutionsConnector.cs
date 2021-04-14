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
    public class ESolutionsConnector
    {
        public ESolutionsResponse GetESolutionsResponse(ESolutionsRequest eSolutionsRequest)
        {
            ESolutionsResponse eSolutionsResponse = new ESolutionsResponse();
            ESolutionsCredentials eSolutionsCredentials = new ESolutionsCredentials();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string url = String.Format("https://41.78.78.238:8083/billpayments/vend");
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Timeout = 120000;
                SetBasicAuthHeader(httpWebRequest, eSolutionsCredentials.Username, eSolutionsCredentials.Password);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                string json = JsonConvert.SerializeObject(eSolutionsRequest);

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        eSolutionsResponse = JsonConvert.DeserializeObject<ESolutionsResponse>(result);
                    }
                }
            }
            catch (Exception e)
            {
                ESolutionsLog.HttpErrorLog(e.Message);
            }

            return eSolutionsResponse;
        }

        public void SetBasicAuthHeader(WebRequest request, String userName, String userPassword)
        {
            string authInfo = userName + ":" + userPassword;
            authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
            request.Headers["Authorization"] = "Basic " + authInfo;
        }

        //public ESolutionsResponse GetESolutionsResponse(ESolutionsRequest eSolutionsRequest)
        //{

        //    ESolutionsResponse eSolutionsResponse = new ESolutionsResponse();
        //    ESolutionsCredentials eSolutionsCredentials = new ESolutionsCredentials();

        //    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
        //    (
        //           delegate { return true; }
        //    );

        //    var authInfo = eSolutionsCredentials.Username + ":" + eSolutionsCredentials.Password;
        //    authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
        //    // var byt = Encoding.Default.GetBytes(Body);
        //    HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create("https://41.78.78.238:8083/billpayments/vend");
        //    httpWReq.ContentType = "application/json";

        //    httpWReq.Headers.Add("Authorization", "Basic " + authInfo);
        //    httpWReq.Method = WebRequestMethods.Http.Post;
        //    httpWReq.Timeout = 120000;

        //    //httpWReq.ContentLength = byt.Length;
        //    //httpWReq.ContentLength = ContentLength;

        //    string body = JsonConvert.SerializeObject(eSolutionsRequest, Formatting.Indented);
        //    using (var streamWriter = new StreamWriter(httpWReq.GetRequestStream()))
        //    {
        //        streamWriter.Write(body);
        //    }
        //    HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse();
        //    StreamReader sr = new StreamReader(response.GetResponseStream());
        //    var returnvalue = sr.ReadToEnd();

        //    var res = JsonConvert.DeserializeObject<ESolutionsResponse>(returnvalue);

        //    return res;
        //}
    }
}