using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using YoAppWebProxy.Api_Credentials;
using YoAppWebProxy.Connectors;
using YoAppWebProxy.Logs;
using YoAppWebProxy.Models;
using YoAppWebProxy.Models.Aqusales;
using YoAppWebProxy.Models.AuditLog;
using YoAppWebProxy.Models.YoApp;
using YoAppWebProxy.ViewModels;

namespace YoAppWebProxy.Controllers
{
    public class WebServiceController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult OnlinePayments(WebPayments payments)
        {
            #region Declared Objects
            var serviceProvider = "PaymentService";
            WebPayments webPayments = new WebPayments();
            Narrative narrative = new Narrative();
            Payments yoappPayments = new Payments();
            YoAppConnector yoAppConnector = new YoAppConnector();
            AquSalesConnector aquSalesConnector = new AquSalesConnector();
            YoAppCredentials yoAppCredentials = new YoAppCredentials();
            string responseBrowseUrl = "";
            Company company = new Company();
            User aqusalesUser = new User();

            #endregion

            try
            {
                Log.RequestsAndResponses("WebHook-Push", serviceProvider, payments);

                if (payments == null)
                {
                    payments.Status = "Failed";
                    payments.Description = "Received Nothing from the request. Your request object is null";

                    Log.RequestsAndResponses("WebHook-Response", serviceProvider, payments);

                    return Json(payments, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    // Check User if Username Exists in Aqusales
                    aqusalesUser.Username = payments.username;
                    aqusalesUser.Company = payments.orgName;

                    Log.RequestsAndResponses("CheckUserNameRequest", serviceProvider, aqusalesUser);

                    var transactionResponse = aquSalesConnector.CheckUser(aqusalesUser, serviceProvider);

                    Log.RequestsAndResponses("CheckUserNameResponse", serviceProvider, transactionResponse);

                    if (transactionResponse.Status.ToUpper() == "SUCCESS")
                    {
                        #region Receipt and Ref No.

                        var date = DateTime.Now.ToString("MMddyyHHmmss");
                        //var dateToBeConverted = DateTime.ParseExact(date, "dd/M/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                        //var dateTimeConverterd = dateToBeConverted.ToString("MMddyyHHmmss");
                        //var receiptNo = "YOAPP" + dateTimeConverterd;
                        Random random = new Random();

                        //int num = random.Next(1, 2000000000);
                        var refNo = "REF" + date;

                        Random random2 = new Random();

                        int num2 = random2.Next(1, 1000000000);
                        var receiptNo = "REC" + num2.ToString();

                        #endregion

                        #region Hashing

                        var sSourceData = "MySourceData";
                        var tmpHash = GetHashString(sSourceData);

                        #endregion

                        payments.packageType = "LITE";

                        yoappPayments.merchantCode = yoAppCredentials.UserId;

                        if (payments.billingCycle.ToUpper() == "MONTHLY")
                        {
                            payments.cycleQuantity = 1;
                        }
                        else if (payments.billingCycle.ToUpper() == "QUATERLY")
                        {
                            payments.cycleQuantity = 4;
                        }
                        else if (payments.billingCycle.ToUpper() == "ANNUALLY")
                        {
                            payments.cycleQuantity = 12;
                        }

                        #region Validate Comma's within strings

                        if (payments.orgName.Contains(","))
                        {
                            payments.orgName = payments.orgName.Replace(",", "|");
                        }

                        if (payments.orgPhoneNumber.Contains(","))
                        {
                            payments.orgPhoneNumber = payments.orgPhoneNumber.Replace(",", "|");
                        }

                        if (payments.orgAddress.Contains(","))
                        {
                            payments.orgAddress = payments.orgAddress.Replace(",", "|");
                        }

                        if (payments.orgLocation.Contains(","))
                        {
                            payments.orgLocation = payments.orgLocation.Replace(",", "|");
                        }

                        if (payments.contactPersonName.Contains(","))
                        {
                            payments.contactPersonName = payments.contactPersonName.Replace(",", "|");
                        }

                        if (payments.contactPersonLastName.Contains(","))
                        {
                            payments.contactPersonLastName = payments.contactPersonLastName.Replace(",", "|");
                        }

                        if (payments.customerEmail.Contains(","))
                        {
                            payments.customerEmail = payments.customerEmail.Replace(",", "|");
                        }

                        if (payments.username.Contains(","))
                        {
                            payments.username = payments.username.Replace(",", "|");
                        }

                        #endregion

                        payments.paymentReference = refNo;
                        payments.paymentMethod = "";

                        yoappPayments.successURL = yoAppCredentials.SuccessUrl + "?orgName=" + payments.orgName.Trim() +
                                                                                 "&orgPhoneNumber=" + payments.orgPhoneNumber.Trim() +
                                                                                 "&orgAddress=" + payments.orgAddress.Trim() +
                                                                                 "&orgLocation=" + payments.orgLocation.Trim() +
                                                                                 "&packageType=" + payments.packageType.Trim() +
                                                                                 "&contactPersonName=" + payments.contactPersonName.Trim() +
                                                                                 "&contactPersonLastName=" + payments.contactPersonLastName.Trim() +
                                                                                 "&customerEmail=" + payments.customerEmail.Trim() +
                                                                                 "&username=" + payments.username.Trim() +
                                                                                 "&password=" + payments.password.Trim() +
                                                                                 "&paymentCurrency=" + payments.paymentCurrency.Trim() +
                                                                                 "&transactionAmount=" + payments.transactionAmount.Trim() +
                                                                                 "&billingCycle=" + payments.billingCycle.Trim() +
                                                                                 "&cycleQuantity=" + payments.cycleQuantity +
                                                                                 "&paymentReference=" + payments.paymentReference.Trim() + 
                                                                                 "&paymentMethod=" + payments.paymentMethod.Trim();

                        //company.CompanyName = payments.orgName;
                        //company.CompanyPhoneNumber = payments.orgPhoneNumber;
                        //company.CompanyAddress = payments.orgAddress;
                        //company.CompanyLocation = payments.orgLocation;
                        //company.PackageType = "AQUSALES LITE";
                        //company.FirstName = payments.contactPersonName;
                        //company.Surname = payments.contactPersonLastName;
                        //company.EmailAddress = payments.customerEmail;
                        //company.Username = payments.username;
                        //company.Password = payments.password;

                        //var d = DateTime.Now.ToString("yy.mm.HH.mm.ss");

                        //var paymentId = "PAY-ID" + d;

                        //Log.StoreData("PaymentIds", serviceProvider, company);

                        yoappPayments.failedURL = yoAppCredentials.FailureUrl;
                        yoappPayments.ReceiptNo = receiptNo;
                        yoappPayments.custmerEmail = payments.customerEmail;
                        yoappPayments.transactionAmount = payments.transactionAmount;
                        yoappPayments.paymentCurrency = payments.paymentCurrency;
                        yoappPayments.transactionDescription = "Aqusales Lite Payment";
                        yoappPayments.paymentref = refNo;
                        yoappPayments.status = "Success";
                        yoappPayments.hash = tmpHash.ToString();

                        Log.RequestsAndResponses("YoAppPaymentsRequest", serviceProvider, yoappPayments);

                        var responseUrls = yoAppConnector.YoAppPayment(serviceProvider, yoappPayments);

                        Log.RequestsAndResponses("YoAppPaymentsResponse", serviceProvider, responseUrls);

                        if (!string.IsNullOrEmpty(responseUrls))
                        {
                            char[] delimiter = new char[] { '-' };
                            string[] part = responseUrls.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                            responseBrowseUrl = part[0];
                            var responsePollsUrl = part[1];

                            webPayments.Status = "Success";
                            webPayments.Description = "Received the relevant payment details and redirecting to " + responseBrowseUrl;

                            ResponsePollsUrl responsePolls = new ResponsePollsUrl();

                            responsePolls.pollsUrl = responsePollsUrl;

                            Log.StoreMpin(payments.paymentReference, serviceProvider, responsePolls);

                            Log.RequestsAndResponses("WebHook-Response", serviceProvider, webPayments);

                            return Json(new { url = responseBrowseUrl }, JsonRequestBehavior.AllowGet);
                            //return Redirect(responseBrowseUrl);
                        }
                        else
                        {
                            webPayments.Status = "Failed";
                            webPayments.Description = "Received the relevant payment details but failed to redirect to Payment Gateway";

                            Log.RequestsAndResponses("WebHook-Response", serviceProvider, webPayments);

                            return Json(webPayments, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        webPayments.Status = "Failed";
                        webPayments.Description = "Username exists";

                        Log.RequestsAndResponses("WebHook-Response", serviceProvider, webPayments);

                        return Json(webPayments, JsonRequestBehavior.AllowGet);
                    }                    
                }
            }
            catch (Exception e)
            {
                Log.HttpError("Exception", serviceProvider, "Message: " + e.Message + ", InnerException: " + e.InnerException + ", StackTrace: " + e.StackTrace);
            }

            return Redirect(responseBrowseUrl);
        }

        [HttpGet]
        public ActionResult PaymentResponse(WebPayments payments)
        {
            #region Declared Objects

            var serviceProvider = "PaymentResponse";
            WebPayments webPayments = new WebPayments();
            Narrative narrative = new Narrative();
            Payments yoappPayments = new Payments();
            YoAppConnector yoAppConnector = new YoAppConnector();
            AquSalesConnector aquSalesConnector = new AquSalesConnector();
            YoAppCredentials yoAppCredentials = new YoAppCredentials();
            YoAppResponse yoAppResponse = new YoAppResponse();
            YoAppRequest yoAppRequest = new YoAppRequest();
            Company company = new Company();

            #endregion

            try
            {
                Log.RequestsAndResponses("YoAppPaymentResponse", serviceProvider, payments);

                if (payments == null)
                {
                    payments.Status = "Failed";
                    payments.Description = "Received Nothing from the request. Your request object is null";

                    Log.RequestsAndResponses("YoAppPaymentResponse", serviceProvider, payments);

                    return Json(payments, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    #region Billing Cycle Quantity

                    if (payments.billingCycle.ToUpper() == "MONTHLY")
                    {
                        payments.cycleQuantity = 1;
                    }
                    else if (payments.billingCycle.ToUpper() == "QUATERLY")
                    {
                        payments.cycleQuantity = 3;
                    }
                    else if (payments.billingCycle.ToUpper() == "ANNUALLY")
                    {
                        payments.cycleQuantity = 12;
                    }

                    #endregion                    

                    // Send API Request to YoApp to Create a new Customer and a Voucher. Send Response as a License and Company Creation Request to AquSales
                    yoAppRequest.AgentCode = "5-0001-0001185:@qus@leslic3ns3";
                    yoAppRequest.ServiceId = 6;
                    yoAppRequest.ActionId = 1;
                    yoAppRequest.MTI = "0200";
                    yoAppRequest.TransactionType = 2;
                    yoAppRequest.Currency = payments.paymentCurrency.Trim();
                    yoAppRequest.Amount = decimal.Parse(payments.transactionAmount.Trim(), System.Globalization.CultureInfo.InvariantCulture);
                    yoAppRequest.CustomerName = payments.orgName.Trim();
                    yoAppRequest.CustomerMSISDN = payments.orgPhoneNumber.Trim();
                    yoAppRequest.CustomerData = payments.contactPersonName.Trim() + "," + payments.contactPersonLastName.Trim() + "," + payments.username.Trim() +
                       "," + payments.customerEmail.Trim() + "," + payments.orgPhoneNumber.Trim() + ",na," + payments.packageType.Trim() + "," + payments.billingCycle.Trim() + "," 
                       + payments.orgLocation.Trim() + "," + payments.paymentCurrency.Trim() + "," + payments.transactionAmount.Trim() + "," + payments.cycleQuantity +
                       "," + payments.orgName.Trim() + "," + payments.orgPhoneNumber.Trim();
                    yoAppRequest.ServiceProvider = "5-0001-0000980";
                    yoAppRequest.ProcessingCode = "360000";

                    Log.RequestsAndResponses("CreateCustomerAndLicenseRequest", serviceProvider, yoAppRequest);

                    var yoApp = yoAppConnector.CreateCustomerAndLicense(yoAppRequest, serviceProvider);

                    Log.RequestsAndResponses("CreateCustomerAndLicenseResponse", serviceProvider, yoApp);

                    if (yoApp.ResponseCode == "00000")
                    {
                        var desirializedYoAppResponse = JsonConvert.DeserializeObject<Narrative>(yoApp.Narrative);

                        company.CompanyName = payments.orgName.Trim();
                        company.CompanyPhoneNumber = payments.orgPhoneNumber.Trim();
                        company.CompanyAddress = payments.orgAddress.Trim();
                        company.CompanyLocation = payments.orgLocation.Trim();
                        company.PackageType = payments.packageType.Trim();
                        company.FirstName = payments.contactPersonName.Trim();
                        company.Surname = payments.contactPersonLastName.Trim();
                        company.EmailAddress = payments.customerEmail.Trim();
                        company.Username = payments.username.Trim();
                        company.Password = payments.password.Trim();
                        company.UserToken = desirializedYoAppResponse.TransactionCode.Trim();

                        Log.RequestsAndResponses("AquSalesCompanyCreationRequest", serviceProvider, company);

                        var response = aquSalesConnector.CreateCompany(company, serviceProvider);

                        Log.RequestsAndResponses("AquSalesCompanyCreationResponse", serviceProvider, company);

                        if (response.Status.ToUpper() == "SUCCESS")
                        {
                            #region Query the Polls result

                            string urlFilePath = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/" + serviceProvider + "/Files/" + payments.paymentReference + ".json");

                            var urlFile = LoadResponseUrl(urlFilePath, serviceProvider);
                            string result = "";

                            try
                            {
                                using (var client = new HttpClient())
                                {
                                    client.DefaultRequestHeaders.Accept.Clear();

                                    //client.DefaultRequestHeaders.Accept.Add(newMediaTypeWithQualityHeaderValue("application/json"));
                                    //GET Method  

                                    HttpResponseMessage paymentDetailsResponse = client.GetAsync(urlFile.pollsUrl.Trim()).Result;

                                    if (paymentDetailsResponse.IsSuccessStatusCode)
                                    {
                                        result = paymentDetailsResponse.Content.ReadAsStringAsync().Result;

                                        Log.RequestsAndResponses("PollsRedirectResponse", serviceProvider, result);
                                    }
                                    else
                                    {
                                        Console.WriteLine("Internal server Error");
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Log.HttpError("Exception", serviceProvider, "Message: " + e.Message + ", InnerException: " + e.InnerException + ", StackTrace: " + e.StackTrace);
                            }

                            #endregion                            

                            var polresult = JsonConvert.DeserializeObject<PaymentResponse>(result);

                            payments.paymentMethod = polresult.PaymentType;

                            PaymentTransaction paymentTransaction = new PaymentTransaction();

                            paymentTransaction.CustomerName = payments.contactPersonName + " " + payments.contactPersonLastName;
                            paymentTransaction.CustomerEmail = payments.customerEmail;
                            paymentTransaction.CustomerPhoneNumber = payments.orgPhoneNumber;
                            paymentTransaction.TransactionReference = payments.paymentReference;
                            paymentTransaction.Company = payments.orgName;
                            paymentTransaction.Currency = payments.paymentCurrency;
                            paymentTransaction.PaymentMethod = payments.paymentMethod;
                            paymentTransaction.Amount = decimal.Parse(payments.transactionAmount, System.Globalization.CultureInfo.InvariantCulture);
                            paymentTransaction.TransactionDate = DateTime.Now.Date;
                            paymentTransaction.TransactionName = "";
                            paymentTransaction.Cashier = "ONLINE STORE";

                            Log.RequestsAndResponses("AquSalesPaymentRequest", serviceProvider, paymentTransaction);

                            var aqusalesServerResponse = aquSalesConnector.PostPayment(paymentTransaction, serviceProvider);

                            Log.RequestsAndResponses("AquSalesPaymentResponse", serviceProvider, paymentTransaction);

                            if (response.Status.ToUpper() == "SUCCESS")
                            {
                                #region Redirect to Aqusales Login
                                aquSalesConnector.WebsiteRedirect(serviceProvider);
                                #endregion

                                yoAppResponse.ResponseCode = "00000";
                                yoAppResponse.Description = response.Description;
                                yoAppResponse.Note = "Success";
                                yoAppResponse.Narrative = "Transaction/Payment Posted successfully";

                                Log.RequestsAndResponses("AquPayment-Response-YoApp", serviceProvider, response);

                                return Json(yoAppResponse, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                yoAppResponse.ResponseCode = "00008";
                                yoAppResponse.Description = response.Description;
                                yoAppResponse.Note = "Transaction Failed";
                                yoAppResponse.Narrative = "Transaction Failed";

                                Log.RequestsAndResponses("AquPayment-Response-YoApp", serviceProvider, response);

                                return Json(yoAppResponse, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            yoAppResponse.ResponseCode = "00008";
                            yoAppResponse.Description = response.Description;
                            yoAppResponse.Note = "Transaction Failed";
                            yoAppResponse.Narrative = "Transaction Failed";

                            Log.RequestsAndResponses("AquPayment-Response-YoApp", serviceProvider, response);

                            return Json(yoAppResponse, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        yoAppResponse.ResponseCode = "00008";
                        yoAppResponse.Description = "There has been an error on creating either the customer or the License in YoApp. Check log for Details";
                        yoAppResponse.Note = "Transaction Failed";
                        yoAppResponse.Narrative = "Transaction Failed";

                        Log.RequestsAndResponses("YoAppCustomerAndLicenseCreation-Response-YoApp", serviceProvider, yoAppResponse);

                        return Json(yoAppResponse, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception e)
            {
                Log.HttpError("Exception", serviceProvider, "Message: " + e.Message + ", InnerException: " + e.InnerException + ", StackTrace: " + e.StackTrace);
            }

            return Json(yoAppResponse, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult FailedPaymentResponse(FailedPaymentResponse failedPaymentResponse)
        {
            return Json(new { url = failedPaymentResponse.failedUrl }, JsonRequestBehavior.AllowGet);
        }

        public static byte[] GetHash(string inputString)
        {
            using (HashAlgorithm algorithm = SHA256.Create())
                return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        public static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

        [NonAction]
        public ResponsePollsUrl LoadResponseUrl(string file, string serviceProvider)
        {
            try
            {
                using (FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.ReadWrite, FileShare.None))

                using (StreamReader sr = new StreamReader(fileStream))
                {
                    string json = sr.ReadToEnd();
                    var responsePollsUrl = JsonConvert.DeserializeObject<ResponsePollsUrl>(json);

                    return responsePollsUrl;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                var exceptionMessage = "Message: " + ex.Message + ", Inner Exception: " + ex.InnerException + ", StackTrace: " + ex.StackTrace;

                Log.HttpError("File-Exception", serviceProvider, exceptionMessage);

                return null;
            }
        }
    }
}