using AutoMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using YoAppWebProxy.Models;
using YoAppWebProxy.Logs;
using YoAppWebProxy.Api_Credentials;
using YoAppWebProxy.Helpful_Functions;
using YoAppWebProxy.Connectors;
using YoAppWebProxy.ServiceProviders;
using System.IO;

namespace YoAppWebProxy.Controllers
{
    /// <summary>
    /// YoApp WebProxy Documentation 
    /// </summary>
    public class WebProxyController : ApiController
    {
        Narrative deserializedYoAppNarrative = new Narrative();

        /// <summary>
        /// Receives YoApp Response, 
        /// Structures response to match client's Request, 
        /// Sends client's Request to the Client's URL endpoint,
        /// Receives client's Response
        /// Structures client's Response to match YoApp's Request
        /// Sends Request to YoApp
        /// </summary>
        /// <param name="yoAppRequest">Request from YoApp</param>
        [Route("api/proxy/cbz/real-time-voucher-payments")]
        public YoAppResponse RealTimeVoucherPaymentsProxy(YoAppRequest yoAppRequest)
        {
            #region Declared Objects
            YoAppResponse yoAppResponse = new YoAppResponse();
            #endregion

            #region Checking Ip Address
            String hostName = String.Empty;
            hostName = Dns.GetHostName();
            yoAppResponse = SiteIdentity.GetClientIp(hostName);
            #endregion

            if (yoAppResponse.ResponseCode != "00000")
            {
                return yoAppResponse; // yoAppResponse already defined in SiteIdentity.GetClientIp()
            }
            else
            {
                if (yoAppRequest == null)
                {
                    string message = "Received Nothing. Your request object is null";

                    Request.CreateResponse(HttpStatusCode.OK, message);

                    yoAppResponse.ResponseCode = "00008";
                    yoAppResponse.Note = "Failed";
                    yoAppResponse.Description = message;

                    var serializedRequest = JsonConvert.SerializeObject(yoAppRequest);

                    yoAppResponse.Narrative = serializedRequest;

                    return yoAppResponse;
                }
                else
                {
                    switch (yoAppRequest.ServiceId)
                    {

                        case 2: // CBZ Service (Real-Time Voucher Payments for Loan Creation)

                            #region Declared Objects
                            CbzPaymentRequest cbzRequest = new CbzPaymentRequest();
                            CbzVoucherPaymentDetails voucherPaymentDetails = new CbzVoucherPaymentDetails();
                            CbzConnector cbzAPIConnector = new CbzConnector();
                            CbzMethods cbzMethods = new CbzMethods();
                            #endregion

                            deserializedYoAppNarrative = JsonConvert.DeserializeObject<Narrative>(yoAppRequest.Narrative);

                            //cbzRequest.VoucherPaymentDetails = new List<VoucherPaymentDetails>();

                            //cbzRequest.VoucherPaymentDetails.Add(new VoucherPaymentDetails
                            //{
                            //    referenceNumber = yoAppResponse.TransactionRef,
                            //    username = WebServiceObjects.Username,
                            //    password = WebServiceObjects.Password,
                            //    ReceiverNationalId = deserializedApiResponse.ReceiversIdentification,
                            //    CropType = GetCropType(deserializedApiResponse.ProductDetails),
                            //    Summary = GetProductsList(deserializedApiResponse.Products)
                            //});

                            voucherPaymentDetails.referenceNumber = yoAppRequest.TransactionRef;
                            voucherPaymentDetails.username = CbzCredentials.Username;
                            voucherPaymentDetails.password = CbzCredentials.Password;
                            voucherPaymentDetails.ReceiverNationalId = deserializedYoAppNarrative.ReceiversIdentification;

                            voucherPaymentDetails.CropType = cbzMethods.GetCropType(deserializedYoAppNarrative.ProductDetails);

                            voucherPaymentDetails.Summary = new List<Product>();

                            foreach (var item in deserializedYoAppNarrative.Products)
                            {
                                voucherPaymentDetails.Summary.Add(new Product { ProductRedeemed = item.Name, QuantityRedeemed = item.Collected, PricePerUnit = item.Price });
                            }

                            CbzLog.Request(voucherPaymentDetails);

                            var vPaymentDetails = new List<CbzVoucherPaymentDetails>();
                            vPaymentDetails.Add(voucherPaymentDetails);

                            var apiResponse = cbzAPIConnector.GetCBZResponse(vPaymentDetails);

                            if (apiResponse.ResponseCode == "00")
                            {
                                var serilizedApiResponse = JsonConvert.SerializeObject(apiResponse);

                                yoAppResponse.ResponseCode = "00000";
                                yoAppResponse.Note = "Success";
                                yoAppResponse.Description = apiResponse.RespnseMessage;
                                yoAppResponse.Narrative = serilizedApiResponse;

                                CbzLog.Response(voucherPaymentDetails);

                                return yoAppResponse;
                            }

                            if (apiResponse.ResponseCode == "05")
                            {
                                var serilizedApiResponse = JsonConvert.SerializeObject(apiResponse);

                                yoAppResponse.ResponseCode = "00008";
                                yoAppResponse.Note = "Failed";
                                yoAppResponse.Description = apiResponse.RespnseMessage;
                                yoAppResponse.Narrative = serilizedApiResponse;

                                CbzLog.Response(voucherPaymentDetails);

                                return yoAppResponse;
                            }

                            if (apiResponse.ResponseCode == "95")
                            {
                                var serilizedApiResponse = JsonConvert.SerializeObject(apiResponse);

                                yoAppResponse.ResponseCode = "00008";
                                yoAppResponse.Note = "Failed";
                                yoAppResponse.Description = apiResponse.RespnseMessage;
                                yoAppResponse.Narrative = serilizedApiResponse;

                                CbzLog.Response(voucherPaymentDetails);

                                return yoAppResponse;
                            }

                            break;
                    }
                }
            }

            return yoAppResponse;
        }

        /// <summary>
        /// CBZ API Requests and Responses
        /// </summary>
        /// <param name="yoAppRequest"></param>
        /// <returns></returns>
        [Route("api/cbz/trek-fuel-redemptions")]
        [HttpPost]
        public YoAppResponse CbzTrekRedemptionsProxy(YoAppRequest yoAppRequest)
        {
            #region Declared Objects
            YoAppResponse yoAppResponse = new YoAppResponse();
            #endregion

            #region Checking Ip Address
            String hostName = String.Empty;
            hostName = Dns.GetHostName();
            yoAppResponse = SiteIdentity.GetClientIp(hostName);
            #endregion

            if (yoAppResponse.ResponseCode != "00000")
            {
                return yoAppResponse; // yoAppResponse already defined in SiteIdentity.GetClientIp()
            }
            else
            {
                if (yoAppRequest == null)
                {
                    string message = "Received Nothing. Your request object is null";

                    Request.CreateResponse(HttpStatusCode.OK, message);

                    yoAppResponse.ResponseCode = "00008";
                    yoAppResponse.Note = "Failed";
                    yoAppResponse.Description = message;

                    var serializedRequest = JsonConvert.SerializeObject(yoAppRequest);

                    yoAppResponse.Narrative = serializedRequest;

                    return yoAppResponse;
                }
                else
                {
                    switch (yoAppRequest.ServiceId)
                    {
                        case 1: // Trek Service

                            #region Declared Objects
                            TrekBearerTokenRequest trekBearerTokenRequest = new TrekBearerTokenRequest();
                            TrekBearerTokenResponse trekBearerTokenResponse = new TrekBearerTokenResponse();
                            TrekCredentials trekCredentials = new TrekCredentials();
                            TrekMethods trekMethods = new TrekMethods();
                            TrekCardTransactionsByDateAndCardNumberRequest trekCardTransactionsByDateAndCardNumberRequest = new TrekCardTransactionsByDateAndCardNumberRequest();
                            TrekCardTransactionsByDateAndCardNumberResponse trekCardTransactionsByDateAndCardNumberResponse = new TrekCardTransactionsByDateAndCardNumberResponse();
                            TrekCardTransactionsByDateRequest trekCardTransactionsByDateRequest = new TrekCardTransactionsByDateRequest();
                            TrekCardTransactionsByDateResponse trekCardTransactionsByDateResponse = new TrekCardTransactionsByDateResponse();
                            TrekPostConnector trekPostConnector = new TrekPostConnector();
                            TrekCardBalanceRequest trekCardBalanceRequest = new TrekCardBalanceRequest();
                            TrekCardBalanceResponse trekCardBalanceResponse = new TrekCardBalanceResponse();
                            #endregion

                            // Authorization --- Get Token
                            trekBearerTokenRequest.email = trekCredentials.Username;
                            trekBearerTokenRequest.password = trekCredentials.Password;

                            var token = trekMethods.GetBearerToken(trekBearerTokenRequest);

                            var deserializeTokenResponse = JsonConvert.DeserializeObject<TrekBearerTokenResponse>(token);
                            Token.StringToken = deserializeTokenResponse.data.access_token;

                            switch (yoAppRequest.ActionId)
                            {
                                case 1: // Devices

                                    TrekGetConnector trekGetConnector = new TrekGetConnector();
                                    TrekDevicesResponse trekDevicesResponse = new TrekDevicesResponse();

                                    trekDevicesResponse = trekGetConnector.GetAllTrekDevices();

                                    if (trekDevicesResponse != null)
                                    {
                                        yoAppResponse.ResponseCode = "00000";
                                        yoAppResponse.Note = "Success";
                                        yoAppResponse.Description = "All Trek Devices have been fetched for you";

                                        var serializedDevicesResponse = JsonConvert.SerializeObject(trekDevicesResponse);

                                        yoAppResponse.Narrative = serializedDevicesResponse;

                                        return yoAppResponse;
                                    }
                                    else
                                    {
                                        yoAppResponse.ResponseCode = "00008";
                                        yoAppResponse.Note = "Failed";
                                        yoAppResponse.Description = "Failed to fetch trek devices";

                                        return yoAppResponse;
                                    }

                                case 2: // Card Balance
                                    trekCardBalanceRequest.card_number = yoAppRequest.CustomerAccount;

                                    TrekLog.GetTrekCardNumberBalanceRequest(trekCardBalanceRequest);

                                    break;

                                case 3: // Get Card Transactions by Card Number and dates

                                    trekCardTransactionsByDateAndCardNumberRequest.card_number = yoAppRequest.CustomerAccount;
                                    trekCardTransactionsByDateAndCardNumberRequest.start_date = yoAppRequest.StartDate;
                                    trekCardTransactionsByDateAndCardNumberRequest.end_date = yoAppRequest.EndDate;

                                    TrekLog.GetTrekTransactionsByDatesAndCardNumberRequest(trekCardTransactionsByDateAndCardNumberRequest);

                                    trekCardTransactionsByDateAndCardNumberResponse = trekPostConnector.GetTrekCardTransactionsByDateAndCardNumber(trekCardTransactionsByDateAndCardNumberRequest);

                                    TrekLog.GetTrekTransactionsByDatesAndCardNumberResponse(trekCardTransactionsByDateAndCardNumberResponse);

                                    var serializedTransactionsByDatesAndCardNumberResponse = JsonConvert.SerializeObject(trekCardTransactionsByDateAndCardNumberResponse);

                                    yoAppResponse.ResponseCode = "00000";
                                    yoAppResponse.Description = "Success";
                                    yoAppResponse.Note = "Success";
                                    yoAppResponse.Narrative = serializedTransactionsByDatesAndCardNumberResponse;

                                    break;

                                case 4: // Get Card Transactions by dates
                                    trekCardTransactionsByDateRequest.start_date = yoAppRequest.StartDate;
                                    trekCardTransactionsByDateRequest.end_date = yoAppRequest.EndDate;

                                    TrekLog.GetTrekTransactionsByDatesRequest(trekCardTransactionsByDateRequest);

                                    trekCardTransactionsByDateResponse = trekPostConnector.GetTrekCardTransactionsByDates(trekCardTransactionsByDateRequest);

                                    TrekLog.GetTrekTransactionsByDatesResponse(trekCardTransactionsByDateResponse);

                                    var serializedTransactionsByDatesResponse = JsonConvert.SerializeObject(trekCardTransactionsByDateResponse);

                                    yoAppResponse.ResponseCode = "00000";
                                    yoAppResponse.Description = "Success";
                                    yoAppResponse.Note = "Success";
                                    yoAppResponse.Narrative = serializedTransactionsByDatesResponse;

                                    return yoAppResponse;
                            }

                            break;
                    }
                }

                return yoAppResponse;
            }
        }        

        /// <summary>
        /// CBZ API Requests and Responses
        /// </summary>
        /// <param name="yoAppRequest"></param>
        /// <returns></returns>
        [Route("api/yoapp/new-field")]
        [HttpPost]
        public YoAppResponse NewFieldProxy(YoAppRequest yoAppRequest)
        {
            #region Declared Objects
            YoAppResponse yoAppResponse = new YoAppResponse();
            #endregion

            #region Checking Ip Address
            String hostName = String.Empty;
            hostName = Dns.GetHostName();
            yoAppResponse = SiteIdentity.GetClientIp(hostName);
            #endregion

            if (yoAppResponse.ResponseCode != "00000")
            {
                return yoAppResponse; // yoAppResponse already defined in SiteIdentity.GetClientIp()
            }
            else
            {
                if (yoAppRequest == null)
                {
                    string message = "Received Nothing. Your request object is null";

                    Request.CreateResponse(HttpStatusCode.OK, message);

                    yoAppResponse.ResponseCode = "00008";
                    yoAppResponse.Note = "Failed";
                    yoAppResponse.Description = message;

                    var serializedRequest = JsonConvert.SerializeObject(yoAppRequest);

                    yoAppResponse.Narrative = serializedRequest;

                    return yoAppResponse;
                }
                else
                {
                    switch (yoAppRequest.ServiceId)
                    {
                        case 1: // CBZ Service (External Column Service)

                            var fuelCardNumber = "";

                            var dateTimeNow = DateTime.Now.ToString();
                            var dateTimeNowToBeLogged = DateTime.Parse(dateTimeNow, CultureInfo.InvariantCulture).ToString("MM/dd/yyyy/ HH:mm:ss");
                            var dateTimeNowFileName = DateTime.Parse(dateTimeNow, CultureInfo.InvariantCulture).ToString("MM.dd.yyyy");

                            var serializedRequest = JsonConvert.SerializeObject(yoAppRequest);

                            var fileName = "trek_cards";
                            var logFileName = "cards_log";

                            string filePath = HttpContext.Current.Server.MapPath("~/App_Data/CBZ/Fuel_Cards/" + fileName + ".txt");

                            string logFilePath = HttpContext.Current.Server.MapPath("~/App_Data/CBZ/Fuel_Cards_Log/" + logFileName + "_" + dateTimeNowFileName + ".txt");

                            var logString = "DateTimeCardNumberLogged: " + dateTimeNowToBeLogged + "," + "ServiceProvider: " + yoAppRequest.ServiceProvider + ","
                                + "ServiceId: " + yoAppRequest.ServiceId + "," + "ActionId: " + yoAppRequest.ActionId + "Fuel Card Number Allocated: " + fuelCardNumber
                                + "RequestObject: " + serializedRequest;

                            string[] firstlines = File.ReadAllLines(filePath);

                            try
                            {
                                if (File.Exists(filePath))
                                {
                                    if (firstlines == null)
                                    {
                                        yoAppResponse.CustomerAccount = fuelCardNumber;

                                        yoAppResponse.ResponseCode = "00008";
                                        yoAppResponse.Note = "Failed";
                                        yoAppResponse.Description = "All card numbers have been allocated, we have run out of card numbers";

                                        var serializedResponse = JsonConvert.SerializeObject(yoAppRequest);

                                        yoAppResponse.Narrative = serializedResponse;

                                        return yoAppResponse;
                                    }
                                    else
                                    {
                                        List<string> lines = new List<string>(firstlines);

                                        fuelCardNumber = lines[0];

                                        lines.Remove(lines[0]);

                                        using (StreamWriter streamWriter = File.AppendText(logFilePath))
                                        {
                                            streamWriter.WriteLine(logString);
                                            streamWriter.WriteLine("=============================================");
                                        }

                                        string[] arrayLines = lines.ToArray();

                                        File.WriteAllLines(filePath, arrayLines);
                                    }
                                }
                                else
                                {
                                    if (firstlines == null)
                                    {
                                        yoAppResponse.CustomerAccount = fuelCardNumber;

                                        yoAppResponse.ResponseCode = "00008";
                                        yoAppResponse.Note = "Failed";
                                        yoAppResponse.Description = "All card numbers have been allocated, we have run out of card numbers";

                                        var serializedResponse = JsonConvert.SerializeObject(yoAppRequest);

                                        yoAppResponse.Narrative = serializedResponse;

                                        return yoAppResponse;
                                    }
                                    else
                                    {
                                        List<string> lines = new List<string>(firstlines);

                                        fuelCardNumber = lines[0];

                                        lines.Remove(lines[0]);

                                        using (StreamWriter streamWriter = File.CreateText(logFilePath))
                                        {
                                            streamWriter.WriteLine(logString);
                                            streamWriter.WriteLine("=============================================");
                                        }

                                        string[] arrayLines = lines.ToArray();

                                        File.WriteAllLines(filePath, arrayLines);
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }

                            if (String.IsNullOrEmpty(fuelCardNumber))
                            {
                                yoAppResponse.ResponseCode = "00008";
                                yoAppResponse.Note = "Failed";
                                yoAppResponse.Description = "Failed to Allocate Fuel Card Number";

                                return yoAppResponse;
                            }
                            else
                            {
                                yoAppResponse.CustomerAccount = fuelCardNumber;

                                yoAppResponse.ResponseCode = "00000";
                                yoAppResponse.Note = "Success";
                                yoAppResponse.Description = "Card Number " + fuelCardNumber + " has been successfully allocated to farmer";

                                var serializedResponse = JsonConvert.SerializeObject(yoAppRequest);

                                yoAppResponse.Narrative = serializedResponse;

                                return yoAppResponse;
                            }
                    }
                }
            }

            return yoAppResponse;
        }

        /// <summary>
        /// CBZ API Requests and Responses
        /// </summary>
        /// <param name="yoAppRequest"></param>
        /// <returns></returns>
        [Route("api/yoapp/new-field")]
        [HttpPost]
        public YoAppResponse AgriBankBulkPaymentsProxy(YoAppRequest yoAppRequest)
        {
            #region Declared Objects
            YoAppResponse yoAppResponse = new YoAppResponse();
            #endregion

            #region Checking Ip Address
            String hostName = String.Empty;
            hostName = Dns.GetHostName();
            yoAppResponse = SiteIdentity.GetClientIp(hostName);
            #endregion

            if (yoAppResponse.ResponseCode != "00000")
            {
                return yoAppResponse; // yoAppResponse already defined in SiteIdentity.GetClientIp()
            }
            else
            {
                if (yoAppRequest == null)
                {
                    string message = "Received Nothing. Your request object is null";

                    Request.CreateResponse(HttpStatusCode.OK, message);

                    yoAppResponse.ResponseCode = "00008";
                    yoAppResponse.Note = "Failed";
                    yoAppResponse.Description = message;

                    var serializedRequest = JsonConvert.SerializeObject(yoAppRequest);

                    yoAppResponse.Narrative = serializedRequest;

                    return yoAppResponse;
                }
                else
                {
                    switch (yoAppRequest.ServiceId)
                    {
                        case 1:
                            #region Objects to use for the Application
                            AgribankMethods agribankMethods = new AgribankMethods();
                            AgribankCredentials agribankCredentials = new AgribankCredentials();
                            AgribankPostConnector postConnector = new AgribankPostConnector();
                            AgribankTokenRequest agribankTokenRequest = new AgribankTokenRequest();
                            AgribankPaymentRequest agribankPaymentRequest = new AgribankPaymentRequest();
                            AgribankPaymentResponse agribankPaymentResponse = new AgribankPaymentResponse();
                            #endregion

                            agribankTokenRequest.Username = agribankCredentials.Username;
                            agribankTokenRequest.Password = agribankCredentials.Password;

                            // Get Token
                            var token = agribankMethods.GetBearerToken(agribankTokenRequest);
                            Token.StringToken = token;

                            agribankPaymentRequest.batch_currency = yoAppRequest.Currency;
                            agribankPaymentRequest.batch_reference = yoAppRequest.TransactionRef;
                            agribankPaymentRequest.destination_acc = agribankCredentials.ClientName;

                            AgriBankLog.Request(agribankPaymentRequest);

                            var paymentResponse = postConnector.PostPayment(agribankPaymentRequest);

                            AgriBankLog.Response(agribankPaymentResponse);

                            var serializedResponse = JsonConvert.SerializeObject(paymentResponse);

                            yoAppResponse.Narrative = serializedResponse;

                            break;

                    }
                }
            }

            return yoAppResponse;
        }

        /// <summary>
        /// CBZ API Requests and Responses
        /// </summary>
        /// <param name="yoAppRequest"></param>
        /// <returns></returns>
        [Route("api/esolutions/services")]
        [HttpPost]
        public YoAppResponse ESolutionsServicesProxy(YoAppRequest yoAppRequest)
        {
            #region Declared Objects
            YoAppResponse yoAppResponse = new YoAppResponse();
            #endregion

            #region Checking Ip Address
            String hostName = String.Empty;
            hostName = Dns.GetHostName();
            yoAppResponse = SiteIdentity.GetClientIp(hostName);
            #endregion

            if (yoAppResponse.ResponseCode != "00000")
            {
                return yoAppResponse; // yoAppResponse already defined in SiteIdentity.GetClientIp()
            }
            else
            {
                if (yoAppRequest == null)
                {
                    string message = "Received Nothing. Your request object is null";

                    Request.CreateResponse(HttpStatusCode.OK, message);

                    yoAppResponse.ResponseCode = "00008";
                    yoAppResponse.Note = "Failed";
                    yoAppResponse.Description = message;

                    var serializedRequest = JsonConvert.SerializeObject(yoAppRequest);

                    yoAppResponse.Narrative = serializedRequest;

                    return yoAppResponse;
                }
                else
                {
                    switch (yoAppRequest.ServiceId)
                    {

                        case 1:

                            switch (yoAppRequest.ActionId)
                            {
                                case 1: // Get Merchants

                                    #region Declared List
                                    List<Merchants> merchantsList = new List<Merchants>
                                                {
                                                    new Merchants{ Merchant = "ZETDC", Product = "ZETDC_PREPAID", SupportsCustomerInfo = true },
                                                    new Merchants{ Merchant = "NETONE", Product = "NETONE_AIRTIME", SupportsCustomerInfo = false },
                                                    new Merchants{ Merchant = "ECONET", Product = "ECONET_AIRTIME", SupportsCustomerInfo = false },
                                                    new Merchants{ Merchant = "TELECEL", Product = "TELECEL", SupportsCustomerInfo = false },
                                                    new Merchants{ Merchant = "EDGARS", Product = "EDGARS", SupportsCustomerInfo = false },
                                                    new Merchants{ Merchant = "EDGARS", Product = "JET", SupportsCustomerInfo = false },
                                                    new Merchants{ Merchant = "ZOL", Product = "ZOL", SupportsCustomerInfo = true },
                                                    new Merchants{ Merchant = "TELONE", Product = "", SupportsCustomerInfo = true },
                                                    new Merchants{ Merchant = "DSTV", Product = "DSTV", SupportsCustomerInfo = true },
                                                    new Merchants{ Merchant = "BYOBILL", Product = "BYOBILL", SupportsCustomerInfo = true },
                                                    new Merchants{ Merchant = "HARARE", Product = "HARARE", SupportsCustomerInfo = true },
                                                    new Merchants{ Merchant = "GWERU", Product = "GWERU", SupportsCustomerInfo = true }

                                                };
                                    #endregion

                                    var serializedMerchantsList = JsonConvert.SerializeObject(merchantsList);
                                    yoAppResponse.ResponseCode = "00000";
                                    yoAppResponse.Description = "Merchant List Returned Successfully! Check in the Narrative Object";
                                    yoAppResponse.Narrative = serializedMerchantsList;

                                    return yoAppResponse;

                                case 2: // API Calls

                                    #region Declared Objects
                                    ESolutionsRequest eSolutionsRequest = new ESolutionsRequest();
                                    ESolutionsApiObjects eSolutionsApiObjects = new ESolutionsApiObjects();
                                    ESolutionsMethods eSolutionsMethods = new ESolutionsMethods();
                                    #endregion

                                    if (String.IsNullOrEmpty(yoAppRequest.MTI)) // without mti and processingCode
                                    {
                                        string message = "Your request does not have an mti e.g. 0200. " +
                                            "Please put the correct mti and resend your request";

                                        Request.CreateResponse(HttpStatusCode.OK, message);

                                        yoAppResponse.ResponseCode = "00008";
                                        yoAppResponse.Note = "Failed";
                                        yoAppResponse.Description = message;

                                        var serializedRequest = JsonConvert.SerializeObject(eSolutionsRequest);

                                        yoAppResponse.Narrative = serializedRequest;

                                        return yoAppResponse;
                                    }
                                    else // With mti and processingCode
                                    {
                                        eSolutionsRequest.mti = yoAppRequest.MTI;
                                        eSolutionsRequest.processingCode = yoAppRequest.ProcessingCode;
                                        eSolutionsRequest.vendorReference = yoAppRequest.TransactionRef;
                                        eSolutionsRequest.transactionAmount = (long)yoAppRequest.Amount;

                                        switch (eSolutionsRequest.mti)
                                        {
                                            case "0200": // Transaction Request

                                                switch (eSolutionsRequest.processingCode)
                                                {
                                                    case "300000": // Vendor Balance Enquiry
                                                        yoAppResponse = eSolutionsMethods.VendorBalanceEnquiry(eSolutionsRequest);
                                                        break;

                                                    case "310000": // Customer Information
                                                        yoAppResponse = eSolutionsMethods.CustomerInformation(eSolutionsRequest);
                                                        break;

                                                    case "320000": // Last Customer Token
                                                        yoAppResponse = eSolutionsMethods.CustomerInformation(eSolutionsRequest);
                                                        break;

                                                    case "U50000": // Purchase Token
                                                        yoAppResponse = eSolutionsMethods.ServicePurchase(eSolutionsRequest);
                                                        break;

                                                    case "520000": // Direct Payment for Service e.g. Airtime
                                                        yoAppResponse = eSolutionsMethods.DirectServicePurchase(eSolutionsRequest);
                                                        break;

                                                    case "":
                                                        string message = "Your request does not have an processingCode e.g. 300000. " +
                                                            "Please put the correct processingCode and resend your request";

                                                        Request.CreateResponse(HttpStatusCode.OK, message);

                                                        yoAppResponse.ResponseCode = "00008";
                                                        yoAppResponse.Note = "Failed";
                                                        yoAppResponse.Description = message;

                                                        var serializedRequest = JsonConvert.SerializeObject(eSolutionsRequest);

                                                        yoAppResponse.Narrative = serializedRequest;

                                                        return yoAppResponse;
                                                }

                                                break;

                                            default:

                                                yoAppResponse.ResponseCode = "00008";
                                                yoAppResponse.Note = "Failed";
                                                yoAppResponse.Description = "Request did not follow proper channels";

                                                return yoAppResponse;
                                        }
                                    }

                                    break;

                                default:

                                    yoAppResponse.ResponseCode = "00008";
                                    yoAppResponse.Note = "Failed";
                                    yoAppResponse.Description = "Request did not follow proper channels";

                                    return yoAppResponse;
                            }

                            break;
                    }
                }
            }

            return yoAppResponse;
        }
    }
}
