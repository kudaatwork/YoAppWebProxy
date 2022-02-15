using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
            YoAppCredentials yoAppCredentials = new YoAppCredentials();
            string responseBrowseUrl = "";
            Company company = new Company();
            #endregion

            try
            {
                Log.RequestsAndResponses("WebHook-Push", serviceProvider, payments);

                if (payments == null)
                {
                    payments.Status = "Failed";
                    payments.Description = "Received Nothing from the request. Your request object is null";

                    Log.RequestsAndResponses("WebHook-Response", serviceProvider, payments);

                    return Json(payments);
                }
                else
                {
                    #region Receipt and Ref No.

                    var date = DateTime.Now.ToString();
                    //var dateToBeConverted = DateTime.ParseExact(date, "dd/M/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                    //var dateTimeConverterd = date.ToString("MMddyyHHmmss");
                    var receiptNo = "YOAPP" + date;
                    var refNo = "REF" + date;

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
                        payments.cycleQuantity = 3;
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

                    yoappPayments.successURL = yoAppCredentials.SuccessUrl + "?orgName=" + payments.orgName +
                                                                             "&orgPhoneNumber=" + payments.orgPhoneNumber +
                                                                             "&orgAddress=" + payments.orgAddress +
                                                                             "&orgLocation=" + payments.orgLocation +
                                                                             "&packageType=" + payments.packageType +
                                                                             "&contactPersonName=" + payments.contactPersonName +
                                                                             "&contactPersonLastName=" + payments.contactPersonLastName +
                                                                             "&customerEmail=" + payments.customerEmail +
                                                                             "&username=" + payments.username +
                                                                             "&password=" + payments.password +
                                                                             "&paymentCurrency=" + payments.paymentCurrency + 
                                                                             "&transactionAmount=" + payments.transactionAmount +                                                                            
                                                                             "&billingCycle=" + payments.billingCycle +
                                                                             "&cycleQuantity=" + payments.cycleQuantity;

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

                        Log.RequestsAndResponses("WebHook-Response", serviceProvider, webPayments);
                        return Json(new { url = responseBrowseUrl }, JsonRequestBehavior.AllowGet);
                        //return Redirect(responseBrowseUrl);
                    }
                    else
                    {
                        webPayments.Status = "Success";
                        webPayments.Description = "Received the relevant payment details but failed to redirect to Payment Gateway";

                        Log.RequestsAndResponses("WebHook-Response", serviceProvider, webPayments);

                        return Json(webPayments);
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

                    return Json(payments);
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
                    yoAppRequest.AgentCode = "5-0001-0001175:@qus@leslic3ns3";
                    yoAppRequest.ServiceId = 2;
                    yoAppRequest.ActionId = 1;
                    yoAppRequest.MTI = "0200";
                    yoAppRequest.TransactionType = 2;
                    yoAppRequest.Currency = payments.paymentCurrency;
                    yoAppRequest.Amount = Convert.ToDecimal(payments.transactionAmount);
                    yoAppRequest.CustomerName = payments.orgName;
                    yoAppRequest.CustomerMSISDN = payments.orgPhoneNumber;
                    yoAppRequest.CustomerData = payments.contactPersonName + "," + payments.contactPersonLastName + "," + payments.username +
                       "," + payments.customerEmail + "," + payments.orgPhoneNumber + ",na," + payments.packageType + "," + payments.billingCycle + "," 
                       + payments.orgLocation + "," + payments.paymentCurrency + "," + payments.transactionAmount + "," + payments.cycleQuantity;
                    yoAppRequest.ServiceProvider = "5-0001-0001174";
                    yoAppRequest.ProcessingCode = "360000";

                    Log.RequestsAndResponses("CreateCustomerAndLicenseRequest", serviceProvider, yoAppRequest);

                    var yoApp = yoAppConnector.CreateCustomerAndLicense(yoAppRequest, serviceProvider);

                    Log.RequestsAndResponses("CreateCustomerAndLicenseResponse", serviceProvider, yoApp);

                    if (yoApp.ResponseCode == "00000")
                    {
                        var desirializedYoAppResponse = JsonConvert.DeserializeObject<Narrative>(yoApp.Narrative);

                        company.CompanyName = payments.orgName;
                        company.CompanyPhoneNumber = payments.orgPhoneNumber;
                        company.CompanyAddress = payments.orgAddress;
                        company.CompanyLocation = payments.orgLocation;
                        company.PackageType = payments.packageType;
                        company.FirstName = payments.contactPersonName;
                        company.Surname = payments.contactPersonLastName;
                        company.EmailAddress = payments.customerEmail;
                        company.Username = payments.username;
                        company.Password = payments.password;
                        company.UserToken = desirializedYoAppResponse.TransactionCode;

                        Log.RequestsAndResponses("AquSalesCompanyCreationRequest", serviceProvider, company);

                        var response = aquSalesConnector.CreateCompany(company, serviceProvider);

                        Log.RequestsAndResponses("AquSalesCompanyCreationResponse", serviceProvider, company);

                        if (response.Status.ToUpper() == "SUCCESS")
                        {
                            yoAppResponse.ResponseCode = "00000";
                            yoAppResponse.Description = response.Description;
                            yoAppResponse.Note = "Success";
                            yoAppResponse.Narrative = "Transaction/Payment Posted successfully";

                            Log.RequestsAndResponses("AquPayment-Response-YoApp", serviceProvider, response);

                            return Json(yoAppResponse);
                        }
                        else
                        {
                            yoAppResponse.ResponseCode = "00008";
                            yoAppResponse.Description = response.Description;
                            yoAppResponse.Note = "Transaction Failed";
                            yoAppResponse.Narrative = "Transaction Failed";

                            Log.RequestsAndResponses("AquPayment-Response-YoApp", serviceProvider, response);

                            return Json(yoAppResponse);
                        }
                    }
                    else
                    {
                        yoAppResponse.ResponseCode = "00008";
                        yoAppResponse.Description = "There has been an error on creating either the customer or the License in YoApp. Check log for Details";
                        yoAppResponse.Note = "Transaction Failed";
                        yoAppResponse.Narrative = "Transaction Failed";

                        Log.RequestsAndResponses("YoAppCustomerAndLicenseCreation-Response-YoApp", serviceProvider, yoAppResponse);

                        return Json(yoAppResponse);
                    }
                }
            }
            catch (Exception e)
            {
                Log.HttpError("Exception", serviceProvider, "Message: " + e.Message + ", InnerException: " + e.InnerException + ", StackTrace: " + e.StackTrace);
            }

            return Json(yoAppResponse);
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
    }
}