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
using YoAppWebProxy.Models.Aqusales;
using YoAppWebProxy.Helpful_Objects;
using YoAppWebProxy.Models.EOS;
using YoAppWebProxy.Models.YoApp;
using YoAppWebProxy.Models.BulkPayments;
using System.Threading.Tasks;
using YoAppWebProxy.Models.Wafaya;
using System.Reflection;
using YoAppWebProxy.Models.MerchantBank_Clients;
using YoAppWebProxy.Models.MerchantBank;
using YoAppWebProxy.Models.MerchantBank_Transaction;
using YoAppWebProxy.Models.MerchantBank_Login;
using YoAppWebProxy.Models.MerchantBank_Recipients;
using System.Security.Cryptography;

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
            CbzPaymentRequest cbzRequest = new CbzPaymentRequest();
            CbzVoucherPaymentDetails voucherPaymentDetails = new CbzVoucherPaymentDetails();
            CbzConnector cbzAPIConnector = new CbzConnector();
            CbzMethods cbzMethods = new CbzMethods();

            var serviceProvider = "CBZ-IT";
            #endregion

            if (yoAppRequest == null)
            {
                string message = "Received Nothing. Your request object is null";

                yoAppResponse.ResponseCode = "00008";
                yoAppResponse.Note = "Failed";
                yoAppResponse.Description = message;

                return yoAppResponse;
            }
            else
            {
                switch (yoAppRequest.ServiceId)
                {

                    case 1: // CBZ Service (Real-Time Voucher Payments for Loan Creation)                            

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

                        voucherPaymentDetails.Summary = new List<Models.Product>();

                        foreach (var item in deserializedYoAppNarrative.Products)
                        {
                            voucherPaymentDetails.Summary.Add(new Models.Product { ProductRedeemed = item.Name, QuantityRedeemed = item.Collected, PricePerUnit = item.Price });
                        }

                        Log.RequestsAndResponses("VPayments-Request", serviceProvider, voucherPaymentDetails);

                        var vPaymentDetails = new List<CbzVoucherPaymentDetails>();
                        vPaymentDetails.Add(voucherPaymentDetails);

                        var apiResponse = cbzAPIConnector.GetCBZResponse(vPaymentDetails);

                        Log.RequestsAndResponses("VPayments-Response", serviceProvider, apiResponse);

                        if (apiResponse.ResponseCode == "00")
                        {
                            yoAppResponse.ResponseCode = "00000";
                            yoAppResponse.Note = "Success";
                            yoAppResponse.Description = apiResponse.RespnseMessage;

                            return yoAppResponse;
                        }

                        if (apiResponse.ResponseCode == "05")
                        {
                            yoAppResponse.ResponseCode = "00008";
                            yoAppResponse.Note = "Failed";
                            yoAppResponse.Description = apiResponse.RespnseMessage;

                            return yoAppResponse;
                        }

                        if (apiResponse.ResponseCode == "95")
                        {
                            yoAppResponse.ResponseCode = "00008";
                            yoAppResponse.Note = "Failed";
                            yoAppResponse.Description = apiResponse.RespnseMessage;

                            return yoAppResponse;
                        }

                        break;
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
        public YoAppResponse TrekServicesProxy(YoAppRequest yoAppRequest)

        {
            #region Declared Objects
            var serviceProvider = "Trek";

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

            YoAppResponse yoAppResponse = new YoAppResponse();
            #endregion

            #region Checking Ip Address
            //String ip = String.Empty;
            //hostName = Dns.GetHostName();
            //yoAppResponse = SiteIdentity.GetIp(serviceProvider);

            #endregion

            if (yoAppRequest == null)
            {
                string message = "Received Nothing. Your request object is null";

                Request.CreateResponse(HttpStatusCode.OK, message);

                yoAppResponse.ResponseCode = "00008";
                yoAppResponse.Note = "Failed";
                yoAppResponse.Description = message;

                return yoAppResponse;
            }
            else
            {
                // Authorization --- Get Token
                #region Authorization
                trekBearerTokenRequest.email = trekCredentials.Username;
                trekBearerTokenRequest.password = trekCredentials.Password;

                Log.RequestsAndResponses("API-Authorization_Request", serviceProvider, trekBearerTokenRequest);

                var token = trekMethods.GetBearerToken(trekBearerTokenRequest);

                Log.RequestsAndResponses("API-Authorization_Response", serviceProvider, token);

                var deserializeTokenResponse = JsonConvert.DeserializeObject<TrekBearerTokenResponse>(token);
                Token.StringToken = deserializeTokenResponse.data.access_token;
                #endregion

                switch (yoAppRequest.ServiceId)
                {
                    case 1: // Devices

                        TrekGetConnector trekGetConnector = new TrekGetConnector();
                        TrekDevicesResponse trekDevicesResponse = new TrekDevicesResponse();

                        Log.RequestsAndResponses("Get-Devices_Request", serviceProvider, "Empty Body");

                        trekDevicesResponse = trekGetConnector.GetAllTrekDevices();

                        Log.RequestsAndResponses("Get-Devices_Response", serviceProvider, trekDevicesResponse);

                        if (trekDevicesResponse != null)
                        {
                            yoAppResponse.ResponseCode = "00000";
                            yoAppResponse.Note = "Success";
                            yoAppResponse.Description = "All Trek Devices have been fetched for you";

                            var serializedDevicesResponse = JsonConvert.SerializeObject(trekDevicesResponse.data);

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

                        deserializedYoAppNarrative = JsonConvert.DeserializeObject<Narrative>(yoAppRequest.Narrative);

                        trekCardBalanceRequest.card_number = deserializedYoAppNarrative.CustomerCardNumber;

                        Log.RequestsAndResponses("CardBalance-Request", serviceProvider, trekCardBalanceRequest);

                        Log.RequestsAndResponses("TrekCardBalance", "CBZ", trekCardBalanceRequest);

                        trekCardBalanceResponse = trekPostConnector.GetCardBalance(trekCardBalanceRequest);

                        Log.RequestsAndResponses("CardBalance-Response", serviceProvider, trekCardBalanceResponse);

                        if (trekCardBalanceResponse.status == "Success")
                        {
                            yoAppResponse.ResponseCode = "00000";
                            yoAppResponse.Note = "Success";
                            yoAppResponse.Description = "Card Balance retrived successfully";

                            var serializedCardNumberRequest = JsonConvert.SerializeObject(trekCardBalanceResponse.data);

                            yoAppResponse.Narrative = serializedCardNumberRequest;

                            return yoAppResponse;
                        }
                        else
                        {
                            yoAppResponse.ResponseCode = "00008";
                            yoAppResponse.Note = "Failed";
                            yoAppResponse.Description = "Failed to retrive Card Balance";

                            return yoAppResponse;
                        }

                    case 3: // Get Card Transactions by Card Number and Dates

                        deserializedYoAppNarrative = JsonConvert.DeserializeObject<Narrative>(yoAppRequest.Narrative);

                        trekCardTransactionsByDateAndCardNumberRequest.card_number = deserializedYoAppNarrative.CustomerCardNumber;
                        trekCardTransactionsByDateAndCardNumberRequest.start_date = deserializedYoAppNarrative.DateCreated.ToString();
                        trekCardTransactionsByDateAndCardNumberRequest.end_date = deserializedYoAppNarrative.DateCreated.ToString();

                        Log.RequestsAndResponses("TransByDateAndCard-Request", serviceProvider, trekCardTransactionsByDateAndCardNumberRequest);

                        trekCardTransactionsByDateAndCardNumberResponse = trekPostConnector.GetTrekCardTransactionsByDateAndCardNumber(trekCardTransactionsByDateAndCardNumberRequest);

                        Log.RequestsAndResponses("TransByDateAndCard-Response", serviceProvider, trekCardTransactionsByDateAndCardNumberResponse);

                        if (trekCardTransactionsByDateAndCardNumberResponse.status == "success")
                        {
                            var serializedTransactionsByDatesAndCardNumberResponse = JsonConvert.SerializeObject(trekCardTransactionsByDateAndCardNumberResponse.data);

                            yoAppResponse.ResponseCode = "00000";
                            yoAppResponse.Description = "Successfully retrived fuel card transactions by Card Number and Dates";
                            yoAppResponse.Note = "Success";
                            yoAppResponse.Narrative = serializedTransactionsByDatesAndCardNumberResponse;

                            return yoAppResponse;
                        }
                        else
                        {
                            yoAppResponse.ResponseCode = "00008";
                            yoAppResponse.Note = "Failed";
                            yoAppResponse.Description = "Failed to retrive Card Transactions";

                            return yoAppResponse;
                        }

                    case 4: // Get Card Transactions by Dates

                        deserializedYoAppNarrative = JsonConvert.DeserializeObject<Narrative>(yoAppRequest.Narrative);

                        trekCardTransactionsByDateRequest.start_date = deserializedYoAppNarrative.DateCreated.ToString();
                        trekCardTransactionsByDateRequest.end_date = deserializedYoAppNarrative.DatelastAccess.ToString();

                        Log.RequestsAndResponses("TransByDates-Request", serviceProvider, trekCardTransactionsByDateRequest);

                        trekCardTransactionsByDateResponse = trekPostConnector.GetTrekCardTransactionsByDates(trekCardTransactionsByDateRequest);

                        Log.RequestsAndResponses("TransByDates-Response", serviceProvider, trekCardTransactionsByDateResponse);

                        if (trekCardTransactionsByDateResponse.status == "success")
                        {
                            var serializedTransactionsByDatesResponse = JsonConvert.SerializeObject(trekCardTransactionsByDateResponse.data);

                            yoAppResponse.ResponseCode = "00000";
                            yoAppResponse.Description = "Success";
                            yoAppResponse.Note = "Success";
                            yoAppResponse.Narrative = serializedTransactionsByDatesResponse;

                            return yoAppResponse;
                        }
                        else
                        {
                            yoAppResponse.ResponseCode = "00008";
                            yoAppResponse.Note = "Failed";
                            yoAppResponse.Description = "Failed to retrive Card Transactions";

                            return yoAppResponse;
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
        [Route("api/yoapp/external-field")]
        [HttpPost]
        public YoAppResponse PopulateFieldProxy(YoAppRequest yoAppRequest)
        {
            #region Declared Objects
            var serviceProvider = "CBZ-FuelCards";
            YoAppResponse yoAppResponse = new YoAppResponse();
            #endregion

            if (yoAppRequest == null)
            {
                string message = "Received Nothing. Your request object is null";

                yoAppResponse.ResponseCode = "00008";
                yoAppResponse.Note = "Failed";
                yoAppResponse.Description = message;

                return yoAppResponse;
            }
            else
            {
                switch (yoAppRequest.ServiceId)
                {
                    case 1: // LIVE: CBZ Service (External Column Service)

                        ExternalFieldMethods externalFields = new ExternalFieldMethods();

                        var fuelCardNumber = "";

                        var fileName = "fuel_cards";

                        string filePath = HttpContext.Current.Server.MapPath("~/App_Data/" + serviceProvider + "/" + fileName + ".txt");

                        var fileResponse = externalFields.ReadAllFileLines(filePath, fuelCardNumber, serviceProvider);

                        if (fileResponse.ResponseCode == "00000") // Fuel Card Number Allocated
                        {
                            yoAppResponse.ResponseCode = "00000";
                            yoAppResponse.Note = "Success";
                            yoAppResponse.Description = "Card Number " + fileResponse.Narrative + " has been successfully allocated to farmer";

                            yoAppResponse.Narrative = fileResponse.Narrative;

                            Log.RequestsAndResponses("FCards-Response-YoApp", serviceProvider, yoAppResponse);

                            return yoAppResponse;
                        }
                        else if (fileResponse.ResponseCode == "00055") // Fuel Card Numbers have been depleted
                        {
                            yoAppResponse.ResponseCode = "00055";
                            yoAppResponse.Note = "Failed";
                            yoAppResponse.Description = "All card numbers have been allocated, we have run out of card numbers";
                            yoAppResponse.Narrative = fileResponse.Narrative;

                            Log.RequestsAndResponses("FCards-Response-YoApp", serviceProvider, yoAppResponse);

                            return yoAppResponse;
                        }
                        else if (fileResponse.ResponseCode == "00008") // Failed to allocate fuel card number
                        {
                            yoAppResponse.ResponseCode = "00008";
                            yoAppResponse.Note = "Failed";
                            yoAppResponse.Description = "Failed to Allocate Fuel Card Number";
                            yoAppResponse.Narrative = null;

                            Log.RequestsAndResponses("FCards-Response-YoApp", serviceProvider, yoAppResponse);

                            return yoAppResponse;
                        }
                        else // Error in generating fuel card number
                        {
                            yoAppResponse.ResponseCode = "00008";
                            yoAppResponse.Note = "Failed";
                            yoAppResponse.Description = "Error in allocating Fuel Card Number";
                            yoAppResponse.Narrative = null;

                            Log.RequestsAndResponses("FCards-Response-YoApp", serviceProvider, yoAppResponse);

                            return yoAppResponse;
                        }

                    case 2: // TEST: CBZ Service (External Column Service)

                        ExternalFieldMethods externalFieldMethods = new ExternalFieldMethods();

                        var testFuelCardNumber = "";

                        var testFileName = "test_fuel_cards";

                        string testFilePath = HttpContext.Current.Server.MapPath("~/App_Data/" + serviceProvider + "/" + testFileName + ".txt");

                        Log.RequestsAndResponses("TFCards-Request", serviceProvider, yoAppRequest);

                        var fResponse = externalFieldMethods.ReadAllTestFileLines(testFilePath, testFuelCardNumber, serviceProvider);

                        if (fResponse.ResponseCode == "00000") // Fuel Card Number Allocated
                        {
                            yoAppResponse.ResponseCode = "00000";
                            yoAppResponse.Note = "Success";
                            yoAppResponse.Description = "Card Number " + fResponse.Narrative + " has been successfully allocated to farmer";

                            yoAppResponse.Narrative = fResponse.Narrative;

                            Log.RequestsAndResponses("TFCards-Response-YoApp", serviceProvider, yoAppResponse);

                            return yoAppResponse;
                        }
                        else if (fResponse.ResponseCode == "00055") // Fuel Card Numbers have been depleted
                        {
                            yoAppResponse.ResponseCode = "00055";
                            yoAppResponse.Note = "Failed";
                            yoAppResponse.Description = "All card numbers have been allocated, we have run out of card numbers";
                            yoAppResponse.Narrative = fResponse.Narrative;

                            Log.RequestsAndResponses("TFCards-Response-YoApp", serviceProvider, yoAppResponse);

                            return yoAppResponse;
                        }
                        else if (fResponse.ResponseCode == "00008") // Failed to allocate fuel card number
                        {
                            yoAppResponse.ResponseCode = "00008";
                            yoAppResponse.Note = "Failed";
                            yoAppResponse.Description = "Failed to Allocate Fuel Card Number";
                            yoAppResponse.Narrative = null;

                            Log.RequestsAndResponses("TFCards-Response-YoApp", serviceProvider, yoAppResponse);

                            return yoAppResponse;
                        }
                        else // Error in generating fuel card number
                        {
                            yoAppResponse.ResponseCode = "00008";
                            yoAppResponse.Note = "Failed";
                            yoAppResponse.Description = "Error in allocating Fuel Card Number";
                            yoAppResponse.Narrative = null;

                            Log.RequestsAndResponses("TFCards-Response-YoApp", serviceProvider, yoAppResponse);

                            return yoAppResponse;
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
        [Route("api/agribank/bulk-payments")]
        [HttpPost]
        public YoAppResponse AgriBankBulkPaymentsProxy(YoAppRequest yoAppRequest)
        {
            #region Declared Objects
            YoAppResponse yoAppResponse = new YoAppResponse();
            #endregion

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

                        Log.RequestsAndResponses("Request", yoAppRequest.ServiceProvider, agribankPaymentRequest);

                        var paymentResponse = postConnector.PostPayment(agribankPaymentRequest);

                        AgriBankLog.Response(agribankPaymentResponse);

                        var serializedResponse = JsonConvert.SerializeObject(paymentResponse);

                        yoAppResponse.Narrative = serializedResponse;

                        break;
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
        public YoAppResponse ESolutionsServicesProxy(YoAppResponse yoAppResponse)
        {
            #region Declared Objects
            var serviceProvider = "ESolutions";

            ESolutionsRequest eSolutionsRequest = new ESolutionsRequest();
            ESolutionsApiObjects eSolutionsApiObjects = new ESolutionsApiObjects();
            ESolutionsMethods eSolutionsMethods = new ESolutionsMethods();
            #endregion          

            if (yoAppResponse == null)
            {
                string message = "Received Nothing. Your request object is null";

                yoAppResponse.ResponseCode = "00008";
                yoAppResponse.Note = "Failed";
                yoAppResponse.Description = message;

                return yoAppResponse;
            }
            else
            {
                switch (yoAppResponse.ServiceId)
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

                    case 2: // API Calls - Econet Airtime

                        eSolutionsRequest.merchantName = "ECONET";
                        eSolutionsRequest.productName = "ECONET_AIRTIME";
                        eSolutionsRequest.processingCode = "U50000";
                        eSolutionsRequest.mti = "0200";
                        eSolutionsRequest.sourceMobile = yoAppResponse.CustomerAccount;
                        eSolutionsRequest.targetMobile = yoAppResponse.CustomerMSISDN;
                        eSolutionsRequest.utilityAccount = yoAppResponse.CustomerAccount;
                        // eSolutionsRequest.serviceId = yoAppResponse.ServiceId;
                        eSolutionsRequest.transactionAmount = yoAppResponse.Amount.ToString();
                        eSolutionsRequest.vendorReference = yoAppResponse.TransactionRef;
                        eSolutionsRequest.currencyCode = yoAppResponse.Currency;

                        yoAppResponse = eSolutionsMethods.DirectServicePurchase(eSolutionsRequest, serviceProvider);

                        return yoAppResponse;

                    #region Commented Out Code
                    //if (string.IsNullOrEmpty(yoAppRequest.MTI)) // without mti and processingCode
                    //{
                    //    string message = "Your request does not have an mti e.g. 0200. " +
                    //        "Please put the correct mti and resend your request";

                    //    yoAppResponse.ResponseCode = "00008";
                    //    yoAppResponse.Note = "Failed";
                    //    yoAppResponse.Description = message;

                    //    return yoAppResponse;
                    //}
                    //else // With mti and processingCode
                    //{
                    //    eSolutionsRequest.mti = yoAppRequest.MTI;
                    //    eSolutionsRequest.processingCode = yoAppRequest.ProcessingCode;
                    //    eSolutionsRequest.vendorReference = yoAppRequest.TransactionRef;
                    //    eSolutionsRequest.transactionAmount = (long)yoAppRequest.Amount;

                    //    switch (eSolutionsRequest.mti)
                    //    {
                    //        case "0200": // Transaction Request

                    //            switch (eSolutionsRequest.processingCode)
                    //            {
                    //                case "300000": // Vendor Balance Enquiry
                    //                    yoAppResponse = eSolutionsMethods.VendorBalanceEnquiry(eSolutionsRequest, serviceProvider);
                    //                    break;

                    //                case "310000": // Customer Information
                    //                    yoAppResponse = eSolutionsMethods.CustomerInformation(eSolutionsRequest, serviceProvider);
                    //                    break;

                    //                case "320000": // Last Customer Token
                    //                    yoAppResponse = eSolutionsMethods.CustomerInformation(eSolutionsRequest, serviceProvider);
                    //                    break;

                    //                case "U50000": // Purchase Token e.g. ZETDC
                    //                    yoAppResponse = eSolutionsMethods.ServicePurchase(eSolutionsRequest, serviceProvider);
                    //                    break;

                    //                case "520000": // Direct Payment for Service e.g. Airtime

                    //                    eSolutionsRequest.merchantName = "ECONET";
                    //                    eSolutionsRequest.productName = "ECONET_AIRTIME";

                    //                    yoAppResponse = eSolutionsMethods.DirectServicePurchase(eSolutionsRequest, serviceProvider);
                    //                    break;

                    //                case "":
                    //                    string message = "Your request does not have an processingCode e.g. 300000. " +
                    //                        "Please put the correct processingCode and resend your request";

                    //                    Request.CreateResponse(HttpStatusCode.OK, message);

                    //                    yoAppResponse.ResponseCode = "00008";
                    //                    yoAppResponse.Note = "Failed";
                    //                    yoAppResponse.Description = message;

                    //                    var serializedRequest = JsonConvert.SerializeObject(eSolutionsRequest);

                    //                    yoAppResponse.Narrative = serializedRequest;

                    //                    return yoAppResponse;
                    //            }

                    //            break;

                    //        default:

                    //            yoAppResponse.ResponseCode = "00008";
                    //            yoAppResponse.Note = "Failed";
                    //            yoAppResponse.Description = "Request did not follow proper channels";

                    //            return yoAppResponse;
                    //    }
                    //}

                    //break;
                    #endregion

                    case 3: // Netone Airtime

                        eSolutionsRequest.merchantName = "NETONE";
                        eSolutionsRequest.productName = "NETONE_AIRTIME";
                        eSolutionsRequest.processingCode = "U50000";
                        eSolutionsRequest.mti = "0200";
                        eSolutionsRequest.sourceMobile = yoAppResponse.CustomerAccount;
                        eSolutionsRequest.targetMobile = yoAppResponse.CustomerMSISDN;
                        eSolutionsRequest.utilityAccount = yoAppResponse.CustomerAccount;
                        //eSolutionsRequest.serviceId = 2;
                        eSolutionsRequest.transactionAmount = yoAppResponse.Amount.ToString();
                        eSolutionsRequest.vendorReference = yoAppResponse.TransactionRef;
                        eSolutionsRequest.currencyCode = yoAppResponse.Currency;

                        yoAppResponse = eSolutionsMethods.DirectServicePurchase(eSolutionsRequest, serviceProvider);

                        return yoAppResponse;

                    #region Commented Out Code
                    //if (string.IsNullOrEmpty(yoAppRequest.MTI)) // without mti and processingCode
                    //{
                    //    string message = "Your request does not have an mti e.g. 0200. " +
                    //        "Please put the correct mti and resend your request";

                    //    yoAppResponse.ResponseCode = "00008";
                    //    yoAppResponse.Note = "Failed";
                    //    yoAppResponse.Description = message;

                    //    return yoAppResponse;
                    //}
                    //else // With mti and processingCode
                    //{
                    //    eSolutionsRequest.mti = yoAppRequest.MTI;
                    //    eSolutionsRequest.processingCode = yoAppRequest.ProcessingCode;
                    //    eSolutionsRequest.vendorReference = yoAppRequest.TransactionRef;
                    //    eSolutionsRequest.transactionAmount = (long)yoAppRequest.Amount;

                    //    switch (eSolutionsRequest.mti)
                    //    {
                    //        case "0200": // Transaction Request

                    //            switch (eSolutionsRequest.processingCode)
                    //            {
                    //                case "520000": // Direct Payment for Service e.g. Airtime

                    //                    eSolutionsRequest.merchantName = "NETONE";
                    //                    eSolutionsRequest.productName = "NETONE_AIRTIME";

                    //                    yoAppResponse = eSolutionsMethods.DirectServicePurchase(eSolutionsRequest, serviceProvider);
                    //                    break;

                    //                case "":
                    //                    string message = "Your request does not have an processingCode e.g. 300000. " +
                    //                        "Please put the correct processingCode and resend your request";

                    //                    Request.CreateResponse(HttpStatusCode.OK, message);

                    //                    yoAppResponse.ResponseCode = "00008";
                    //                    yoAppResponse.Note = "Failed";
                    //                    yoAppResponse.Description = message;

                    //                    var serializedRequest = JsonConvert.SerializeObject(eSolutionsRequest);

                    //                    yoAppResponse.Narrative = serializedRequest;

                    //                    return yoAppResponse;
                    //            }

                    //            break;

                    //        default:

                    //            yoAppResponse.ResponseCode = "00008";
                    //            yoAppResponse.Note = "Failed";
                    //            yoAppResponse.Description = "Request did not follow proper channels";

                    //            return yoAppResponse;
                    //    }
                    //}

                    //break;
                    #endregion

                    case 4: // Telecel Airtime

                        eSolutionsRequest.merchantName = "TELECEL";
                        eSolutionsRequest.productName = "TELECEL_AIRTIME";
                        eSolutionsRequest.processingCode = "U50000";
                        eSolutionsRequest.mti = "0200";
                        eSolutionsRequest.sourceMobile = yoAppResponse.CustomerAccount;
                        eSolutionsRequest.targetMobile = yoAppResponse.CustomerMSISDN;
                        eSolutionsRequest.utilityAccount = yoAppResponse.CustomerAccount;
                        // eSolutionsRequest.serviceId = yoAppResponse.ServiceId;
                        eSolutionsRequest.transactionAmount = yoAppResponse.Amount.ToString();
                        eSolutionsRequest.vendorReference = yoAppResponse.TransactionRef;

                        yoAppResponse = eSolutionsMethods.DirectServicePurchase(eSolutionsRequest, serviceProvider);

                        return yoAppResponse;

                    #region Commented Out Code
                    //if (string.IsNullOrEmpty(yoAppRequest.MTI)) // without mti and processingCode
                    //{
                    //    string message = "Your request does not have an mti e.g. 0200. " +
                    //        "Please put the correct mti and resend your request";

                    //    yoAppResponse.ResponseCode = "00008";
                    //    yoAppResponse.Note = "Failed";
                    //    yoAppResponse.Description = message;

                    //    return yoAppResponse;
                    //}
                    //else // With mti and processingCode
                    //{
                    //    eSolutionsRequest.mti = yoAppRequest.MTI;
                    //    eSolutionsRequest.processingCode = yoAppRequest.ProcessingCode;
                    //    eSolutionsRequest.vendorReference = yoAppRequest.TransactionRef;
                    //    eSolutionsRequest.transactionAmount = (long)yoAppRequest.Amount;

                    //    switch (eSolutionsRequest.mti)
                    //    {
                    //        case "0200": // Transaction Request

                    //            switch (eSolutionsRequest.processingCode)
                    //            {
                    //                case "520000": // Direct Payment for Service e.g. Airtime

                    //                    eSolutionsRequest.merchantName = "TELECEL";
                    //                    eSolutionsRequest.productName = "TELECEL_AIRTIME";

                    //                    yoAppResponse = eSolutionsMethods.DirectServicePurchase(eSolutionsRequest, serviceProvider);

                    //                    return yoAppResponse;

                    //                case "":

                    //                    string message = "Your request does not have an processingCode e.g. 300000. " +
                    //                        "Please put the correct processingCode and resend your request";

                    //                    yoAppResponse.ResponseCode = "00008";
                    //                    yoAppResponse.Note = "Failed";
                    //                    yoAppResponse.Description = message;

                    //                    return yoAppResponse;
                    //            }

                    //            return yoAppResponse;

                    //        default:

                    //            yoAppResponse.ResponseCode = "00008";
                    //            yoAppResponse.Note = "Failed";
                    //            yoAppResponse.Description = "Request did not follow proper channels";

                    //            return yoAppResponse;
                    //    }
                    //}
                    #endregion

                    default:

                        yoAppResponse.ResponseCode = "00008";
                        yoAppResponse.Note = "Failed";
                        yoAppResponse.Description = "Request did not follow proper channels";

                        return yoAppResponse;
                }
            }
        }

        [Route("api/aqusales-yoapp/cbz-services")]
        [HttpPost]
        public YoAppResponse AqusalesYoAppCbzProxy(YoAppResponse redemptionResponse)
        {
            #region Declared Objects
            var serviceProvider = "CBZ-AquSales";
            YoAppResponse yoAppResponse = new YoAppResponse();
            #endregion

            Log.RequestsAndResponses("YoApp Response", serviceProvider, redemptionResponse);

            if (redemptionResponse == null)
            {
                string message = "Received Nothing. Your request object is null";

                yoAppResponse.ResponseCode = "00008";
                yoAppResponse.Note = "Failed";
                yoAppResponse.Description = message;

                return yoAppResponse;
            }
            else
            {
                switch (redemptionResponse.ServiceId)
                {
                    case 1: // Post Redemptions

                        try
                        {
                            SaleTransaction saleTransaction = new SaleTransaction();
                            AqusalesCredentials aqusalesCredentials = new AqusalesCredentials();
                            AquSalesConnector aquSalesConnector = new AquSalesConnector();

                            var deserializedResponse = JsonConvert.DeserializeObject<Narrative>(redemptionResponse.Narrative);

                            var currency = deserializedResponse.Products.Where(x => x.Currency.Trim() != null || x.Currency.Trim() != "").Select(x => x.Currency).FirstOrDefault();

                            saleTransaction.Username = AqusalesCredentials.Username;
                            saleTransaction.Password = AqusalesCredentials.Password;
                            saleTransaction.IsCreditSale = true;
                            saleTransaction.TransactionReference = redemptionResponse.TransactionRef;
                            saleTransaction.Company = "AFRICAN WIRELESS TECH";//deserializedResponse.CustomerName;
                            saleTransaction.Customer = deserializedResponse.ReceiversName.Trim() + " " + deserializedResponse.ReceiversSurname.Trim() + "~" +
                                deserializedResponse.ReceiversIdentification.Trim() + "~(" + currency.Trim() + ")";
                            saleTransaction.CustomerMSISDN = deserializedResponse.ReceiverMobile.Trim();

                            var totalAmout = 0.0m;

                            foreach (var item in deserializedResponse.Products)
                            {
                                if (item.Collected > 0)
                                {
                                    totalAmout = item.Price * item.Collected;
                                }
                            }

                            saleTransaction.Amount = totalAmout;

                            var costPrice = 0.0m;

                            foreach (var item in deserializedResponse.Products)
                            {
                                if (item.Collected > 0)
                                {
                                    costPrice = item.CostPrice * item.Collected;
                                }
                            }

                            saleTransaction.CostOfSales = costPrice;
                            saleTransaction.Vat = 0.0m;
                            TranCurrencyInfoVM tranCurrencyInfoVM = new TranCurrencyInfoVM();

                            tranCurrencyInfoVM.TransactionCurrency = currency.Trim();
                            tranCurrencyInfoVM.TransactionCurrencyRate = 1;
                            saleTransaction.TranCurrencyInfoVM = tranCurrencyInfoVM;
                            saleTransaction.TransactionDate = deserializedResponse.DatelastAccess.ToLocalTime().ToString();
                            saleTransaction.TransactionName = AqusalesObjects.TransactionName.Trim();
                            saleTransaction.Cashier = deserializedResponse.Cashier.Trim();
                            saleTransaction.CustomerAccountType = AqusalesObjects.CustomerAccountType.Trim();

                            Log.RequestsAndResponses("Redemption-Request", serviceProvider, saleTransaction);

                            var aqusalesResponse = aquSalesConnector.PostCBZRedemption(saleTransaction, serviceProvider);

                            Log.RequestsAndResponses("Redemption-Response", serviceProvider, aqusalesResponse);

                            if (aqusalesResponse.Status.ToUpper() == "SUCCESS")
                            {
                                yoAppResponse.ResponseCode = "00000";
                                yoAppResponse.Description = aqusalesResponse.Description;
                                yoAppResponse.Note = "Success";
                                yoAppResponse.Narrative = "Transaction/Redemption Posted successfully";

                                Log.RequestsAndResponses("Redemption-Response-YoApp", serviceProvider, yoAppResponse);

                                return yoAppResponse;
                            }
                            else
                            {
                                yoAppResponse.ResponseCode = "00008";
                                yoAppResponse.Description = aqusalesResponse.Description;
                                yoAppResponse.Note = "Transaction Failed";
                                yoAppResponse.Narrative = "Transaction Failed";

                                Log.RequestsAndResponses("Redemption-Response-YoApp", serviceProvider, yoAppResponse);

                                return yoAppResponse;
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.RequestsAndResponses("Exception", serviceProvider, "{Message: " + ex.Message + ",InnerException: " + ex.InnerException + ",StackTrace: " + ex.StackTrace + "}");
                            break;
                        }

                    case 2: // Post GRV

                        try
                        {
                            GRVTransaction grvTransaction = new GRVTransaction();
                            AquSalesConnector connector = new AquSalesConnector();
                            List<PurchaseLine> purchaseLines = new List<PurchaseLine>();

                            var aquResponse = JsonConvert.DeserializeObject<YoAppGrvResponse>(redemptionResponse.Narrative);

                            var desirilizedFiles = JsonConvert.DeserializeObject<List<GRVProducts>>(aquResponse.Files);

                            foreach (var item in desirilizedFiles)
                            {
                                PurchaseLine purchaseLine = new PurchaseLine();

                                purchaseLine.item = item.Name;
                                purchaseLine.ItemCode = item.ItemCode;
                                purchaseLine.quantity = item.Quantity;
                                purchaseLine.price = item.UnitPrice;
                                purchaseLine.Reciept = item.Unit;

                                purchaseLines.Add(purchaseLine);
                            }

                            grvTransaction.Username = AqusalesCredentials.Username;
                            grvTransaction.Password = AqusalesCredentials.Password;
                            grvTransaction.SupplierAccountType = AqusalesObjects.SupplierAccountType;
                            grvTransaction.TransactionName = AqusalesObjects.TransactionName;
                            grvTransaction.TransactionReference = aquResponse.OrderNumber.Trim();
                            grvTransaction.Company = "AFRICAN WIRELESS TECH"; //aquResponse.BranchName;
                            grvTransaction.DebtorOrCreditor = aquResponse.BranchName?.Trim() + "(" + aquResponse.BranchId?.Trim() + ")";
                            grvTransaction.Amount = (decimal)aquResponse.StockedValue;
                            grvTransaction.Vat = 0.0m;
                            grvTransaction.PurchaseLines = purchaseLines;

                            TranCurrencyInfoVM tranCurrencyInfo = new TranCurrencyInfoVM();
                            tranCurrencyInfo.TransactionCurrency = "ZAR"; //aquResponse.Currency;
                            tranCurrencyInfo.TransactionCurrencyRate = 1;
                            grvTransaction.TranCurrencyInfoVM = tranCurrencyInfo;
                            grvTransaction.TransactionDate = aquResponse.AuthorisationDate;
                            grvTransaction.TransactionName = aquResponse.OrderType?.Trim();
                            grvTransaction.Cashier = aquResponse.Cashier?.Trim();

                            Log.RequestsAndResponses("AquGrv-Request", serviceProvider, grvTransaction);

                            var response = connector.PostCBZGRV(grvTransaction, serviceProvider);

                            Log.RequestsAndResponses("AquGrv-Response", serviceProvider, response);

                            if (response.Status.ToUpper() == "SUCCESS")
                            {
                                yoAppResponse.ResponseCode = "00000";
                                yoAppResponse.Description = response.Description;
                                yoAppResponse.Note = "Success";
                                yoAppResponse.Narrative = "Transaction/GRV Posted successfully";

                                Log.RequestsAndResponses("AquGrv-Response-YoApp", serviceProvider, yoAppResponse);

                                return yoAppResponse;
                            }
                            else
                            {
                                yoAppResponse.ResponseCode = "00008";
                                yoAppResponse.Description = response.Description;
                                yoAppResponse.Note = "Transaction Failed";
                                yoAppResponse.Narrative = "Transaction Failed";

                                Log.RequestsAndResponses("AquGrv-Response-YoApp", serviceProvider, yoAppResponse);

                                return yoAppResponse;
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.RequestsAndResponses("Exception", serviceProvider, "{Message: " + ex.Message + ",InnerException: " + ex.InnerException + ",StackTrace: " + ex.StackTrace + "}");
                            break;
                        }

                    case 3: // Post Reversals

                        try
                        {
                            SaleTransaction sale = new SaleTransaction();
                            AqusalesCredentials credentials = new AqusalesCredentials();
                            AquSalesConnector connector = new AquSalesConnector();

                            var deserializedResponse = JsonConvert.DeserializeObject<Narrative>(redemptionResponse.Narrative);

                            var currency = deserializedResponse.Products.Where(x => x.Currency.Trim() != null || x.Currency.Trim() != "").Select(x => x.Currency).FirstOrDefault();

                            sale.Username = AqusalesCredentials.Username;
                            sale.Password = AqusalesCredentials.Password;
                            sale.IsCreditSale = true;
                            sale.TransactionReference = redemptionResponse.TransactionRef;
                            sale.Company = "AFRICAN WIRELESS TECH";//deserializedResponse.CustomerName;
                            sale.Customer = deserializedResponse.ReceiversName.Trim() + " " + deserializedResponse.ReceiversSurname.Trim() + "~" +
                                deserializedResponse.ReceiversIdentification.Trim() + "~(" + currency.Trim() + ")";
                            sale.CustomerMSISDN = deserializedResponse.ReceiverMobile.Trim();

                            var totalAmout = 0.0m;

                            foreach (var item in deserializedResponse.Products)
                            {
                                if (item.Collected > 0)
                                {
                                    totalAmout = item.Price * item.Collected * -1;
                                }
                            }

                            sale.Amount = totalAmout;

                            var costPrice = 0.0m;

                            foreach (var item in deserializedResponse.Products)
                            {
                                if (item.Collected > 0)
                                {
                                    costPrice = item.CostPrice * item.Collected;
                                }
                            }

                            sale.CostOfSales = costPrice;
                            sale.Vat = 0.0m;
                            TranCurrencyInfoVM tranCurrencyInfoVM = new TranCurrencyInfoVM();

                            tranCurrencyInfoVM.TransactionCurrency = currency.Trim();
                            tranCurrencyInfoVM.TransactionCurrencyRate = 1;
                            sale.TranCurrencyInfoVM = tranCurrencyInfoVM;
                            sale.TransactionDate = deserializedResponse.DatelastAccess.ToLocalTime().ToString();
                            sale.TransactionName = AqusalesObjects.TransactionName.Trim();
                            sale.Cashier = deserializedResponse.Cashier.Trim();
                            sale.CustomerAccountType = AqusalesObjects.CustomerAccountType.Trim();

                            Log.RequestsAndResponses("Reversal-Request", serviceProvider, sale);

                            var response = connector.PostCBZReversal(sale, serviceProvider);

                            Log.RequestsAndResponses("Reversal-Response", serviceProvider, response);

                            if (response.Status.ToUpper() == "SUCCESS")
                            {
                                yoAppResponse.ResponseCode = "00000";
                                yoAppResponse.Description = response.Description;
                                yoAppResponse.Note = "Success";
                                yoAppResponse.Narrative = "Transaction/Reversal Posted successfully";

                                Log.RequestsAndResponses("Reversal-Response-YoApp", serviceProvider, yoAppResponse);

                                return yoAppResponse;
                            }
                            else
                            {
                                yoAppResponse.ResponseCode = "00008";
                                yoAppResponse.Description = response.Description;
                                yoAppResponse.Note = "Transaction Failed";
                                yoAppResponse.Narrative = "Transaction Failed";

                                Log.RequestsAndResponses("Reversal-Response-YoApp", serviceProvider, yoAppResponse);

                                return yoAppResponse;
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.RequestsAndResponses("Exception", serviceProvider, "{Message: " + ex.Message + ",InnerException: " + ex.InnerException + ",StackTrace: " + ex.StackTrace + "}");
                            break;
                        }

                    case 4: // Post Soya Redemptions

                        try
                        {
                            SaleTransaction saleTransaction = new SaleTransaction();
                            AqusalesCredentials aqusalesCredentials = new AqusalesCredentials();
                            AquSalesConnector aquSalesConnector = new AquSalesConnector();

                            var deserializedResponse = JsonConvert.DeserializeObject<Narrative>(redemptionResponse.Narrative);

                            var currency = deserializedResponse.Products.Where(x => x.Currency.Trim() != null || x.Currency.Trim() != "").Select(x => x.Currency).FirstOrDefault();

                            saleTransaction.Username = AqusalesCredentials.Username;
                            saleTransaction.Password = AqusalesCredentials.Password;
                            saleTransaction.IsCreditSale = true;
                            saleTransaction.TransactionReference = redemptionResponse.TransactionRef;
                            saleTransaction.Company = "CBZ AGROYIELD";//deserializedResponse.CustomerName;
                            saleTransaction.Customer = deserializedResponse.ReceiversName.Trim() + " " + deserializedResponse.ReceiversSurname.Trim() + "~" +
                                deserializedResponse.ReceiversIdentification.Trim() + "~(" + currency.Trim() + ")";
                            saleTransaction.CustomerMSISDN = deserializedResponse.ReceiverMobile.Trim();

                            var totalAmout = 0.0m;

                            foreach (var item in deserializedResponse.Products)
                            {
                                if (item.Collected > 0)
                                {
                                    totalAmout = item.Price * item.Collected;
                                }
                            }

                            saleTransaction.Amount = totalAmout;

                            var costPrice = 0.0m;

                            foreach (var item in deserializedResponse.Products)
                            {
                                if (item.Collected > 0)
                                {
                                    costPrice = item.CostPrice * item.Collected;
                                }
                            }

                            saleTransaction.CostOfSales = costPrice;
                            saleTransaction.Vat = 0.0m;
                            TranCurrencyInfoVM tranCurrencyInfoVM = new TranCurrencyInfoVM();

                            tranCurrencyInfoVM.TransactionCurrency = currency.Trim();
                            tranCurrencyInfoVM.TransactionCurrencyRate = 1;
                            saleTransaction.TranCurrencyInfoVM = tranCurrencyInfoVM;
                            saleTransaction.TransactionDate = deserializedResponse.DatelastAccess.ToLocalTime().ToString();
                            saleTransaction.TransactionName = AqusalesObjects.TransactionName.Trim();
                            saleTransaction.Cashier = deserializedResponse.Cashier.Trim();
                            saleTransaction.CustomerAccountType = AqusalesObjects.CustomerAccountType.Trim();

                            Log.RequestsAndResponses("SoyaRedemption-Request", serviceProvider, saleTransaction);

                            var aqusalesResponse = aquSalesConnector.PostCBZRedemption2(saleTransaction, serviceProvider);

                            Log.RequestsAndResponses("SoyaRedemption-Response", serviceProvider, aqusalesResponse);

                            if (aqusalesResponse.Status.ToUpper() == "SUCCESS")
                            {
                                yoAppResponse.ResponseCode = "00000";
                                yoAppResponse.Description = aqusalesResponse.Description;
                                yoAppResponse.Note = "Success";
                                yoAppResponse.Narrative = "Transaction/Redemption Posted successfully";

                                Log.RequestsAndResponses("SoyaRedemption-Response-YoApp", serviceProvider, yoAppResponse);

                                return yoAppResponse;
                            }
                            else
                            {
                                yoAppResponse.ResponseCode = "00008";
                                yoAppResponse.Description = aqusalesResponse.Description;
                                yoAppResponse.Note = "Transaction Failed";
                                yoAppResponse.Narrative = "Transaction Failed";

                                Log.RequestsAndResponses("SoyaRedemption-Response-YoApp", serviceProvider, yoAppResponse);

                                return yoAppResponse;
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.RequestsAndResponses("Exception", serviceProvider, "{Message: " + ex.Message + ",InnerException: " + ex.InnerException + ",StackTrace: " + ex.StackTrace + "}");
                            break;
                        }

                    case 5: // Post Soya GRV

                        try
                        {
                            GRVTransaction grvTransaction = new GRVTransaction();
                            AquSalesConnector connector = new AquSalesConnector();
                            List<PurchaseLine> purchaseLines = new List<PurchaseLine>();

                            var aquResponse = JsonConvert.DeserializeObject<YoAppGrvResponse>(redemptionResponse.Narrative);

                            var desirilizedFiles = JsonConvert.DeserializeObject<List<GRVProducts>>(aquResponse.Files);

                            foreach (var item in desirilizedFiles)
                            {
                                PurchaseLine purchaseLine = new PurchaseLine();

                                purchaseLine.item = item.Name;
                                purchaseLine.ItemCode = item.ItemCode;
                                purchaseLine.quantity = item.Quantity;
                                purchaseLine.price = item.UnitPrice;
                                purchaseLine.Reciept = item.Unit;

                                purchaseLines.Add(purchaseLine);
                            }

                            grvTransaction.Username = AqusalesCredentials.Username;
                            grvTransaction.Password = AqusalesCredentials.Password;
                            grvTransaction.SupplierAccountType = AqusalesObjects.SupplierAccountType;
                            grvTransaction.TransactionName = AqusalesObjects.TransactionName;
                            grvTransaction.TransactionReference = aquResponse.OrderNumber.Trim();
                            grvTransaction.Company = "CBZ AGROYIELD"; //aquResponse.BranchName;
                            grvTransaction.DebtorOrCreditor = aquResponse.BranchName?.Trim() + "(" + aquResponse.BranchId?.Trim() + ")";
                            grvTransaction.Amount = (decimal)aquResponse.StockedValue;
                            grvTransaction.Vat = 0.0m;
                            grvTransaction.PurchaseLines = purchaseLines;

                            TranCurrencyInfoVM tranCurrencyInfo = new TranCurrencyInfoVM();
                            tranCurrencyInfo.TransactionCurrency = "ZWL"; //aquResponse.Currency;
                            tranCurrencyInfo.TransactionCurrencyRate = 1;
                            grvTransaction.TranCurrencyInfoVM = tranCurrencyInfo;
                            grvTransaction.TransactionDate = aquResponse.AuthorisationDate;
                            grvTransaction.TransactionName = aquResponse.OrderType?.Trim();
                            grvTransaction.Cashier = aquResponse.Cashier?.Trim();

                            Log.RequestsAndResponses("SoyaAquGrv-Request", serviceProvider, grvTransaction);

                            var response = connector.PostCBZGRV2(grvTransaction, serviceProvider);

                            Log.RequestsAndResponses("SoyaAquGrv-Response", serviceProvider, response);

                            if (response.Status.ToUpper() == "SUCCESS")
                            {
                                yoAppResponse.ResponseCode = "00000";
                                yoAppResponse.Description = response.Description;
                                yoAppResponse.Note = "Success";
                                yoAppResponse.Narrative = "Transaction/GRV Posted successfully";

                                Log.RequestsAndResponses("SoyaAquGrv-Response-YoApp", serviceProvider, yoAppResponse);

                                return yoAppResponse;
                            }
                            else
                            {
                                yoAppResponse.ResponseCode = "00008";
                                yoAppResponse.Description = response.Description;
                                yoAppResponse.Note = "Transaction Failed";
                                yoAppResponse.Narrative = "Transaction Failed";

                                Log.RequestsAndResponses("SoyaAquGrv-Response-YoApp", serviceProvider, yoAppResponse);

                                return yoAppResponse;
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.RequestsAndResponses("Exception", serviceProvider, "{Message: " + ex.Message + ",InnerException: " + ex.InnerException + ",StackTrace: " + ex.StackTrace + "}");
                            break;
                        }

                    case 6: // Post Soya Reversals

                        try
                        {
                            SaleTransaction sale = new SaleTransaction();
                            AqusalesCredentials credentials = new AqusalesCredentials();
                            AquSalesConnector connector = new AquSalesConnector();

                            var deserializedResponse = JsonConvert.DeserializeObject<Narrative>(redemptionResponse.Narrative);

                            var currency = deserializedResponse.Products.Where(x => x.Currency.Trim() != null || x.Currency.Trim() != "").Select(x => x.Currency).FirstOrDefault();

                            sale.Username = AqusalesCredentials.Username;
                            sale.Password = AqusalesCredentials.Password;
                            sale.IsCreditSale = true;
                            sale.TransactionReference = redemptionResponse.TransactionRef;
                            sale.Company = "CBZ AGROYIELD";//deserializedResponse.CustomerName;
                            sale.Customer = deserializedResponse.ReceiversName.Trim() + " " + deserializedResponse.ReceiversSurname.Trim() + "~" +
                                deserializedResponse.ReceiversIdentification.Trim() + "~(" + currency.Trim() + ")";
                            sale.CustomerMSISDN = deserializedResponse.ReceiverMobile.Trim();

                            var totalAmout = 0.0m;

                            foreach (var item in deserializedResponse.Products)
                            {
                                if (item.Collected > 0)
                                {
                                    totalAmout = item.Price * item.Collected * -1;
                                }
                            }

                            sale.Amount = totalAmout;

                            var costPrice = 0.0m;

                            foreach (var item in deserializedResponse.Products)
                            {
                                if (item.Collected > 0)
                                {
                                    costPrice = item.CostPrice * item.Collected;
                                }
                            }

                            sale.CostOfSales = costPrice;
                            sale.Vat = 0.0m;
                            TranCurrencyInfoVM tranCurrencyInfoVM = new TranCurrencyInfoVM();

                            tranCurrencyInfoVM.TransactionCurrency = currency.Trim();
                            tranCurrencyInfoVM.TransactionCurrencyRate = 1;
                            sale.TranCurrencyInfoVM = tranCurrencyInfoVM;
                            sale.TransactionDate = deserializedResponse.DatelastAccess.ToLocalTime().ToString();
                            sale.TransactionName = AqusalesObjects.TransactionName.Trim();
                            sale.Cashier = deserializedResponse.Cashier.Trim();
                            sale.CustomerAccountType = AqusalesObjects.CustomerAccountType.Trim();

                            Log.RequestsAndResponses("SoyaReversal-Request", serviceProvider, sale);

                            var response = connector.PostCBZReversal2(sale, serviceProvider);

                            Log.RequestsAndResponses("SoyaReversal-Response", serviceProvider, response);

                            if (response.Status.ToUpper() == "SUCCESS")
                            {
                                yoAppResponse.ResponseCode = "00000";
                                yoAppResponse.Description = response.Description;
                                yoAppResponse.Note = "Success";
                                yoAppResponse.Narrative = "Transaction/Reversal Posted successfully";

                                Log.RequestsAndResponses("SoyaReversal-Response-YoApp", serviceProvider, yoAppResponse);

                                return yoAppResponse;
                            }
                            else
                            {
                                yoAppResponse.ResponseCode = "00008";
                                yoAppResponse.Description = response.Description;
                                yoAppResponse.Note = "Transaction Failed";
                                yoAppResponse.Narrative = "Transaction Failed";

                                Log.RequestsAndResponses("SoyaReversal-Response-YoApp", serviceProvider, yoAppResponse);

                                return yoAppResponse;
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.RequestsAndResponses("Exception", serviceProvider, "{Message: " + ex.Message + ",InnerException: " + ex.InnerException + ",StackTrace: " + ex.StackTrace + "}");
                            break;
                        }
                }
            }

            return yoAppResponse;
        }

        [Route("api/aqusales-yoapp/ohlanga")]
        [HttpPost]
        public YoAppResponse AqusalesYoAppOhlangaProxy(YoAppResponse redemptionResponse)
        {
            #region Declared Objects
            var serviceProvider = "Ohlanga-Aqusales";
            YoAppResponse yoAppResponse = new YoAppResponse();
            #endregion

            Log.RequestsAndResponses("Check1", serviceProvider, redemptionResponse);

            if (redemptionResponse == null)
            {
                string message = "Received Nothing. Your request object is null";

                yoAppResponse.ResponseCode = "00008";
                yoAppResponse.Note = "Failed";
                yoAppResponse.Description = message;

                return yoAppResponse;
            }
            else
            {
                switch (redemptionResponse.ServiceId)
                {
                    case 1: // Post Redemptions

                        try
                        {
                            SaleTransaction saleTransaction = new SaleTransaction();
                            AqusalesCredentials aqusalesCredentials = new AqusalesCredentials();
                            AquSalesConnector aquSalesConnector = new AquSalesConnector();

                            var deserializedResponse = JsonConvert.DeserializeObject<Narrative>(redemptionResponse.Narrative);

                            var currency = deserializedResponse.Products.Where(x => x.Currency.Trim() != null || x.Currency.Trim() != "").Select(x => x.Currency).FirstOrDefault();

                            saleTransaction.Username = AqusalesCredentials.Username;
                            saleTransaction.Password = AqusalesCredentials.Password;
                            saleTransaction.IsCreditSale = true;
                            saleTransaction.TransactionReference = redemptionResponse.TransactionRef;
                            saleTransaction.Company = "OHLANGA JV";//deserializedResponse.CustomerName;
                            saleTransaction.Customer = deserializedResponse.ReceiversName.Trim() + " " + deserializedResponse.ReceiversSurname.Trim() + "~" +
                                deserializedResponse.ReceiversIdentification.Trim() + "~(" + currency.Trim() + ")";
                            saleTransaction.CustomerMSISDN = deserializedResponse.ReceiverMobile.Trim();

                            var totalAmout = 0.0m;

                            foreach (var item in deserializedResponse.Products)
                            {
                                if (item.Collected > 0)
                                {
                                    totalAmout = item.Price * item.Collected;
                                }
                            }

                            saleTransaction.Amount = totalAmout;

                            var costPrice = 0.0m;

                            foreach (var item in deserializedResponse.Products)
                            {
                                if (item.Collected > 0)
                                {
                                    costPrice = item.CostPrice * item.Collected;
                                }
                            }

                            saleTransaction.CostOfSales = costPrice;
                            saleTransaction.Vat = 0.0m;
                            TranCurrencyInfoVM tranCurrencyInfoVM = new TranCurrencyInfoVM();

                            tranCurrencyInfoVM.TransactionCurrency = currency.Trim();
                            tranCurrencyInfoVM.TransactionCurrencyRate = 1;
                            saleTransaction.TranCurrencyInfoVM = tranCurrencyInfoVM;
                            saleTransaction.TransactionDate = deserializedResponse.DatelastAccess.ToLocalTime().ToString();
                            saleTransaction.TransactionName = AqusalesObjects.TransactionName.Trim();
                            saleTransaction.Cashier = deserializedResponse.Cashier.Trim();
                            saleTransaction.CustomerAccountType = AqusalesObjects.CustomerAccountType.Trim();

                            Log.RequestsAndResponses("Aqu-Request", serviceProvider, saleTransaction);

                            var aqusalesResponse = aquSalesConnector.PostOhlangaRedemption(saleTransaction, serviceProvider);

                            Log.RequestsAndResponses("Aqu-Response", serviceProvider, aqusalesResponse);

                            if (aqusalesResponse.Status.ToUpper() == "SUCCESS")
                            {
                                yoAppResponse.ResponseCode = "00000";
                                yoAppResponse.Description = aqusalesResponse.Description;
                                yoAppResponse.Note = "Success";
                                yoAppResponse.Narrative = "Transaction/Redemption Posted successfully";

                                Log.RequestsAndResponses("Aqu-Response-YoApp", serviceProvider, aqusalesResponse);

                                return yoAppResponse;
                            }
                            else
                            {
                                yoAppResponse.ResponseCode = "00008";
                                yoAppResponse.Description = aqusalesResponse.Description;
                                yoAppResponse.Note = "Transaction Failed";
                                yoAppResponse.Narrative = "Transaction Failed";

                                Log.RequestsAndResponses("Aqu-Response-YoApp", serviceProvider, aqusalesResponse);

                                return yoAppResponse;
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.RequestsAndResponses("Exception", serviceProvider, ex.Message);
                            break;
                        }

                    case 2: // Post GRV

                        // Log.RequestsAndResponses("Check2", serviceProvider, "2nd Entry Point");

                        try
                        {
                            GRVTransaction grvTransaction = new GRVTransaction();
                            AquSalesConnector connector = new AquSalesConnector();
                            List<PurchaseLine> purchaseLines = new List<PurchaseLine>();

                            var aquResponse = JsonConvert.DeserializeObject<YoAppGrvResponse>(redemptionResponse.Narrative);

                            var desirilizedFiles = JsonConvert.DeserializeObject<List<GRVProducts>>(aquResponse.Files);

                            foreach (var item in desirilizedFiles)
                            {
                                PurchaseLine purchaseLine = new PurchaseLine();

                                purchaseLine.item = item.Name;
                                purchaseLine.ItemCode = item.ItemCode;
                                purchaseLine.quantity = item.Quantity;
                                purchaseLine.price = item.UnitPrice;
                                purchaseLine.Reciept = item.Unit;

                                purchaseLines.Add(purchaseLine);
                            }

                            grvTransaction.Username = AqusalesCredentials.Username;
                            grvTransaction.Password = AqusalesCredentials.Password;
                            grvTransaction.SupplierAccountType = AqusalesObjects.SupplierAccountType;
                            grvTransaction.TransactionName = AqusalesObjects.TransactionName;
                            grvTransaction.TransactionReference = aquResponse.OrderNumber.Trim();
                            grvTransaction.Company = "OHLANGA JV"; //aquResponse.BranchName;
                            grvTransaction.DebtorOrCreditor = aquResponse.BranchName?.Trim() + "(" + aquResponse.BranchId?.Trim() + ")";
                            grvTransaction.Amount = (decimal)aquResponse.StockedValue;
                            grvTransaction.Vat = 0.0m;
                            grvTransaction.PurchaseLines = purchaseLines;


                            TranCurrencyInfoVM tranCurrencyInfo = new TranCurrencyInfoVM();
                            tranCurrencyInfo.TransactionCurrency = "ZAR";//aquResponse.Currency;
                            tranCurrencyInfo.TransactionCurrencyRate = 1;
                            grvTransaction.TranCurrencyInfoVM = tranCurrencyInfo;
                            grvTransaction.TransactionDate = aquResponse.AuthorisationDate;
                            grvTransaction.TransactionName = aquResponse.OrderType?.Trim();
                            grvTransaction.Cashier = aquResponse.Cashier?.Trim();

                            Log.RequestsAndResponses("AquGrv-Request", serviceProvider, grvTransaction);

                            var response = connector.PostOhlangaGRV(grvTransaction, serviceProvider);

                            Log.RequestsAndResponses("AquGrv-Response", serviceProvider, response);

                            if (response.Status.ToUpper() == "SUCCESS")
                            {
                                yoAppResponse.ResponseCode = "00000";
                                yoAppResponse.Description = response.Description;
                                yoAppResponse.Note = "Success";
                                yoAppResponse.Narrative = "Transaction/Redemption Posted successfully";

                                Log.RequestsAndResponses("Aqu-Response-YoApp", serviceProvider, response);

                                return yoAppResponse;
                            }
                            else
                            {
                                yoAppResponse.ResponseCode = "00008";
                                yoAppResponse.Description = response.Description;
                                yoAppResponse.Note = "Transaction Failed";
                                yoAppResponse.Narrative = "Transaction Failed";

                                Log.RequestsAndResponses("Aqu-Response-YoApp", serviceProvider, response);

                                return yoAppResponse;
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.RequestsAndResponses("Exception", serviceProvider, ex.Message);
                            break;
                        }

                    case 3: // Supplier Payment
                            // Log.RequestsAndResponses("Check2", serviceProvider, "2nd Entry Point");
                        try
                        {
                            GRVTransaction grvTransaction = new GRVTransaction();
                            AquSalesConnector connector = new AquSalesConnector();

                            var aquResponse = JsonConvert.DeserializeObject<PaymentResponse>(redemptionResponse.Narrative);

                            grvTransaction.Username = AqusalesCredentials.Username;
                            grvTransaction.Password = AqusalesCredentials.Password;
                            grvTransaction.SupplierAccountType = AqusalesObjects.OhlangaSupplierAccountType;
                            grvTransaction.TransactionReference = aquResponse.PaymentType.Trim();
                            grvTransaction.Company = "OHLANGA JV"; //aquResponse.BranchName;
                            grvTransaction.DebtorOrCreditor = aquResponse.merchantCode?.Trim();
                            grvTransaction.Amount = Convert.ToDecimal(aquResponse.transactionAmount);
                            grvTransaction.Vat = 0.0m;

                            TranCurrencyInfoVM tranCurrencyInfo = new TranCurrencyInfoVM();
                            tranCurrencyInfo.TransactionCurrency = "ZAR";//aquResponse.Currency;
                            tranCurrencyInfo.TransactionCurrencyRate = 1;
                            grvTransaction.TranCurrencyInfoVM = tranCurrencyInfo;
                            grvTransaction.TransactionDate = DateTime.Now;
                            grvTransaction.Cashier = aquResponse.custmerEmail.Trim() + aquResponse.custmerPhone.Trim();

                            Log.RequestsAndResponses("AquPayment-Request", serviceProvider, grvTransaction);

                            var response = connector.PostOhlangaPayment(grvTransaction, serviceProvider);

                            Log.RequestsAndResponses("AquPayment-Response", serviceProvider, response);

                            if (response.Status.ToUpper() == "SUCCESS")
                            {
                                yoAppResponse.ResponseCode = "00000";
                                yoAppResponse.Description = response.Description;
                                yoAppResponse.Note = "Success";
                                yoAppResponse.Narrative = "Transaction/Payment Posted successfully";

                                Log.RequestsAndResponses("AquPayment-Response-YoApp", serviceProvider, response);

                                return yoAppResponse;
                            }
                            else
                            {
                                yoAppResponse.ResponseCode = "00008";
                                yoAppResponse.Description = response.Description;
                                yoAppResponse.Note = "Transaction Failed";
                                yoAppResponse.Narrative = "Transaction Failed";

                                Log.RequestsAndResponses("AquPayment-Response-YoApp", serviceProvider, response);

                                return yoAppResponse;
                            }

                        }
                        catch (Exception ex)
                        {
                            Log.RequestsAndResponses("Exception", serviceProvider, ex.Message);
                            break;
                        }
                    case 4: // Supplier Creation
                        try
                        {

                        }
                        catch (Exception ex)
                        {
                            Log.RequestsAndResponses("Exception", serviceProvider, ex.Message);
                            break;
                        }
                        break;
                }
            }

            return yoAppResponse;
        }

        [Route("api/eos-yoapp/cbz-services")]
        [HttpPost]
        public YoAppResponse EosYoAppCbzProxy(YoAppResponse apiYoAppResponse)
        {
            #region Declared Objects
            var serviceProvider = "CBZ-EOS";
            YoAppResponse yoAppResponse = new YoAppResponse();
            #endregion

            if (apiYoAppResponse == null)
            {
                string message = "Received Nothing. Your request object is null";

                yoAppResponse.ResponseCode = "00008";
                yoAppResponse.Note = "Failed";
                yoAppResponse.Description = message;

                return yoAppResponse;
            }
            else
            {
                EosConnector eosConnector = new EosConnector();

                switch (apiYoAppResponse.ServiceId)
                {
                    case 1: // Redemption in Test
                        EosRedemptionRequest eosRedemptionRequest = new EosRedemptionRequest();

                        var deserializedNarrative = JsonConvert.DeserializeObject<Narrative>(apiYoAppResponse.Narrative);

                        eosRedemptionRequest.TransactionCode = deserializedNarrative.TransactionCode;
                        eosRedemptionRequest.CustomerId = deserializedNarrative.CustomerId;
                        eosRedemptionRequest.ReceiversName = deserializedNarrative.ReceiversName;
                        eosRedemptionRequest.ReceiversSurname = deserializedNarrative.ReceiversSurname;
                        eosRedemptionRequest.ReceiversIdentification = deserializedNarrative.ReceiversIdentification;
                        eosRedemptionRequest.ServiceRegion = deserializedNarrative.ServiceRegion;
                        eosRedemptionRequest.ServiceProvince = deserializedNarrative.ServiceProvince;
                        eosRedemptionRequest.SupplierId = deserializedNarrative.SupplierId;
                        eosRedemptionRequest.SupplierName = deserializedNarrative.SupplierName;
                        eosRedemptionRequest.CustomerName = deserializedNarrative.CustomerName;
                        eosRedemptionRequest.ResponseCode = "00555";
                        eosRedemptionRequest.LocationCode = deserializedNarrative.LocationCode;

                        List<EosRedemptionProducts> redemptionProducts = new List<EosRedemptionProducts>();

                        foreach (var item in deserializedNarrative.Products)
                        {
                            if (item.Collected > 0)
                            {
                                redemptionProducts.Add(new EosRedemptionProducts
                                {
                                    Id = item.Id,
                                    ActionId = item.ActionId,
                                    ServiceId = item.ServiceId,
                                    Name = item.Name,
                                    Description = item.Description,
                                    Collected = item.Collected,
                                    CollectionAmount = item.CollectionAmount,
                                    Currency = item.Currency
                                });
                            }
                        }

                        eosRedemptionRequest.Products = redemptionProducts;

                        #region OldCode
                        /*
                        eosRequest.Id = deserializedNarrative.Id;
                        eosRequest.ServiceId = deserializedNarrative.ServiceId;
                        eosRequest.TransactionType = deserializedNarrative.TransactionType;
                        eosRequest.TransactionCode = deserializedNarrative.TransactionCode;
                        eosRequest.CustomerId = deserializedNarrative.CustomerId;
                        eosRequest.ReceiversName = deserializedNarrative.ReceiversName;
                        eosRequest.ReceiversSurname = deserializedNarrative.ReceiversSurname;
                        eosRequest.ReceiversIdentification = deserializedNarrative.ReceiversIdentification;
                        eosRequest.ReceiversGender = deserializedNarrative.ReceiversGender;
                        eosRequest.ServiceRegion = deserializedNarrative.ServiceRegion;
                        eosRequest.ServiceProvince = deserializedNarrative.ServiceProvince;
                        eosRequest.ServiceCountry = deserializedNarrative.ServiceCountry;
                        eosRequest.Status = deserializedNarrative.Status;
                        eosRequest.Currency = deserializedNarrative.Currency;
                        eosRequest.Balance = deserializedNarrative.Balance;
                        eosRequest.ServiceName = deserializedNarrative.ServiceName;
                        eosRequest.ServiceType = deserializedNarrative.ServiceType;
                        eosRequest.Quantity = deserializedNarrative.Quantity;
                        eosRequest.ProductDetails = deserializedNarrative.ProductDetails;
                        eosRequest.ServiceProvider = deserializedNarrative.ServiceProvider;
                        eosRequest.ProviderAccountNumber = deserializedNarrative.ProviderAccountNumber;
                        eosRequest.SupplierId = deserializedNarrative.SupplierId;
                        eosRequest.ServiceAgentId = deserializedNarrative.ServiceAgentId;
                        eosRequest.SupplierName = deserializedNarrative.SupplierName;
                        eosRequest.Description = deserializedNarrative.Description;
                        eosRequest.CustomerName = deserializedNarrative.CustomerName;
                        eosRequest.CustomerMobileNumber = deserializedNarrative.CustomerMobileNumber;
                        eosRequest.CustomerCardNumber = deserializedNarrative.CustomerCardNumber;
                        eosRequest.Information1 = deserializedNarrative.Information1;
                        eosRequest.Information2 = deserializedNarrative.Information2;
                        eosRequest.DateCreated = deserializedNarrative.DateCreated;
                        eosRequest.DatelastAccess = deserializedNarrative.DatelastAccess;
                        eosRequest.SubDue = deserializedNarrative.SubDue;
                        eosRequest.BillingCycle = deserializedNarrative.BillingCycle;
                        eosRequest.ReceiverMobile = deserializedNarrative.ReceiverMobile;
                        eosRequest.ResponseCode = "00555";
                        eosRequest.AllowPartPayment = deserializedNarrative.AllowPartPayment;
                        eosRequest.DeactivateOnAuthorisation = deserializedNarrative.DeactivateOnAuthorisation;
                        eosRequest.Cashier = deserializedNarrative.Cashier;
                        eosRequest.Authoriser = deserializedNarrative.Authoriser;
                        eosRequest.LocationCode = deserializedNarrative.LocationCode;
                        eosRequest.JsonProducts = deserializedNarrative.JsonProducts;
                        eosRequest.InitialProducts = deserializedNarrative.InitialProducts;

                        List<Products> products = new List<Products>();

                        foreach (var item in deserializedNarrative.Products)
                        {
                            products.Add(item);
                        }

                        eosRequest.Products = products;*/
                        #endregion                        

                        Log.RequestsAndResponses("EOS-Test-Redemption-Request", serviceProvider, eosRedemptionRequest);

                        var eosResponse = eosConnector.PostRedemption(eosRedemptionRequest, serviceProvider);

                        Log.RequestsAndResponses("EOS-Test-Redemption-Response", serviceProvider, eosResponse);

                        var stringResponse = JsonConvert.SerializeObject(eosResponse);

                        if (eosResponse.code == "200")
                        {
                            yoAppResponse.ResponseCode = "00000";
                            yoAppResponse.Description = eosResponse.msg;
                            yoAppResponse.Note = "Success";
                            yoAppResponse.Narrative = "Transaction/Redemption Posted successfully. Response Object:" + stringResponse;

                            Log.RequestsAndResponses("EOS-Test-Redemption-Response-YoApp", serviceProvider, eosResponse);

                            return yoAppResponse;
                        }
                        else
                        {
                            yoAppResponse.ResponseCode = "00008";
                            yoAppResponse.Description = eosResponse.msg;
                            yoAppResponse.Note = "Failed";
                            yoAppResponse.Narrative = "Failed to process transaction: Response Object: " + stringResponse;

                            Log.RequestsAndResponses("EOS-Test-Redemption-Response-YoApp", serviceProvider, eosResponse);

                            return yoAppResponse;
                        }

                    case 2: // Reversal in Test
                        EosReversalRequest eosReversalRequest = new EosReversalRequest();

                        var narrative = JsonConvert.DeserializeObject<Narrative>(apiYoAppResponse.Narrative);

                        eosReversalRequest.TransactionCode = narrative.TransactionCode;
                        eosReversalRequest.CustomerId = narrative.CustomerId;
                        eosReversalRequest.ReceiversName = narrative.ReceiversName;
                        eosReversalRequest.ReceiversSurname = narrative.ReceiversSurname;
                        eosReversalRequest.ReceiversIdentification = narrative.ReceiversIdentification;
                        eosReversalRequest.ServiceRegion = narrative.ServiceRegion;
                        eosReversalRequest.ServiceProvince = narrative.ServiceProvince;
                        eosReversalRequest.SupplierId = narrative.SupplierId;
                        eosReversalRequest.SupplierName = narrative.SupplierName;
                        eosReversalRequest.CustomerName = narrative.CustomerName;
                        eosReversalRequest.ResponseCode = "00777";
                        eosReversalRequest.LocationCode = narrative.LocationCode;

                        List<EosReversalProducts> eosProducts = new List<EosReversalProducts>();

                        foreach (var item in narrative.Products)
                        {
                            if (item.Collected > 0)
                            {
                                eosProducts.Add(new EosReversalProducts
                                {
                                    Id = item.Id,
                                    ActionId = item.ActionId,
                                    ServiceId = item.ServiceId,
                                    Name = item.Name,
                                    Description = item.Description,
                                    Collected = item.Collected,
                                    CollectionAmount = item.CollectionAmount,
                                    Currency = item.Currency
                                });
                            }

                        }

                        eosReversalRequest.Products = eosProducts;

                        #region OldCode
                        /*
                        eosRequest.Id = deserializedNarrative.Id;
                        eosRequest.ServiceId = deserializedNarrative.ServiceId;
                        eosRequest.TransactionType = deserializedNarrative.TransactionType;
                        eosRequest.TransactionCode = deserializedNarrative.TransactionCode;
                        eosRequest.CustomerId = deserializedNarrative.CustomerId;
                        eosRequest.ReceiversName = deserializedNarrative.ReceiversName;
                        eosRequest.ReceiversSurname = deserializedNarrative.ReceiversSurname;
                        eosRequest.ReceiversIdentification = deserializedNarrative.ReceiversIdentification;
                        eosRequest.ReceiversGender = deserializedNarrative.ReceiversGender;
                        eosRequest.ServiceRegion = deserializedNarrative.ServiceRegion;
                        eosRequest.ServiceProvince = deserializedNarrative.ServiceProvince;
                        eosRequest.ServiceCountry = deserializedNarrative.ServiceCountry;
                        eosRequest.Status = deserializedNarrative.Status;
                        eosRequest.Currency = deserializedNarrative.Currency;
                        eosRequest.Balance = deserializedNarrative.Balance;
                        eosRequest.ServiceName = deserializedNarrative.ServiceName;
                        eosRequest.ServiceType = deserializedNarrative.ServiceType;
                        eosRequest.Quantity = deserializedNarrative.Quantity;
                        eosRequest.ProductDetails = deserializedNarrative.ProductDetails;
                        eosRequest.ServiceProvider = deserializedNarrative.ServiceProvider;
                        eosRequest.ProviderAccountNumber = deserializedNarrative.ProviderAccountNumber;
                        eosRequest.SupplierId = deserializedNarrative.SupplierId;
                        eosRequest.ServiceAgentId = deserializedNarrative.ServiceAgentId;
                        eosRequest.SupplierName = deserializedNarrative.SupplierName;
                        eosRequest.Description = deserializedNarrative.Description;
                        eosRequest.CustomerName = deserializedNarrative.CustomerName;
                        eosRequest.CustomerMobileNumber = deserializedNarrative.CustomerMobileNumber;
                        eosRequest.CustomerCardNumber = deserializedNarrative.CustomerCardNumber;
                        eosRequest.Information1 = deserializedNarrative.Information1;
                        eosRequest.Information2 = deserializedNarrative.Information2;
                        eosRequest.DateCreated = deserializedNarrative.DateCreated;
                        eosRequest.DatelastAccess = deserializedNarrative.DatelastAccess;
                        eosRequest.SubDue = deserializedNarrative.SubDue;
                        eosRequest.BillingCycle = deserializedNarrative.BillingCycle;
                        eosRequest.ReceiverMobile = deserializedNarrative.ReceiverMobile;
                        eosRequest.ResponseCode = "00555";
                        eosRequest.AllowPartPayment = deserializedNarrative.AllowPartPayment;
                        eosRequest.DeactivateOnAuthorisation = deserializedNarrative.DeactivateOnAuthorisation;
                        eosRequest.Cashier = deserializedNarrative.Cashier;
                        eosRequest.Authoriser = deserializedNarrative.Authoriser;
                        eosRequest.LocationCode = deserializedNarrative.LocationCode;
                        eosRequest.JsonProducts = deserializedNarrative.JsonProducts;
                        eosRequest.InitialProducts = deserializedNarrative.InitialProducts;

                        List<Products> products = new List<Products>();

                        foreach (var item in deserializedNarrative.Products)
                        {
                            products.Add(item);
                        }

                        eosRequest.Products = products;*/
                        #endregion                        

                        Log.RequestsAndResponses("EOS-Test-Reversal-Request", serviceProvider, eosReversalRequest);

                        var response = eosConnector.PostReversal(eosReversalRequest, serviceProvider);

                        Log.RequestsAndResponses("EOS-Test-Reversal-Response", serviceProvider, response);

                        var strResponse = JsonConvert.SerializeObject(response);

                        if (response.code == "200")
                        {
                            yoAppResponse.ResponseCode = "00000";
                            yoAppResponse.Description = response.msg;
                            yoAppResponse.Note = "Success";
                            yoAppResponse.Narrative = "Transaction/Reversal Posted successfully. Response Object: " + strResponse;

                            Log.RequestsAndResponses("EOS-Test-Reversal-Response-YoApp", serviceProvider, response);

                            return yoAppResponse;
                        }
                        else
                        {
                            yoAppResponse.ResponseCode = "00008";
                            yoAppResponse.Description = response.msg;
                            yoAppResponse.Note = "Failed";
                            yoAppResponse.Narrative = "Failed to post reversal. Response Object: " + strResponse;

                            Log.RequestsAndResponses("EOS-Test-Reversal-Response-YoApp", serviceProvider, response);

                            return yoAppResponse;
                        }

                    case 3: // Topup in Test
                        EosTopupRequest eosTopupRequest = new EosTopupRequest();

                        var testString = JsonConvert.SerializeObject(apiYoAppResponse.Narrative);

                        var testTopupNarrative = JsonConvert.DeserializeObject<Narrative>(apiYoAppResponse.Narrative);

                        eosTopupRequest.TransactionCode = testTopupNarrative.TransactionCode;
                        eosTopupRequest.CustomerId = testTopupNarrative.CustomerId;
                        eosTopupRequest.ReceiversName = testTopupNarrative.ReceiversName;
                        eosTopupRequest.ReceiversSurname = testTopupNarrative.ReceiversSurname;
                        eosTopupRequest.ReceiversIdentification = testTopupNarrative.ReceiversIdentification;
                        eosTopupRequest.ServiceRegion = testTopupNarrative.ServiceRegion;
                        eosTopupRequest.ServiceProvince = testTopupNarrative.ServiceProvince;
                        eosTopupRequest.SupplierId = testTopupNarrative.SupplierId;
                        eosTopupRequest.SupplierName = testTopupNarrative.SupplierName;
                        eosTopupRequest.CustomerName = testTopupNarrative.CustomerName;
                        eosTopupRequest.ResponseCode = "00888";
                        eosTopupRequest.LocationCode = testTopupNarrative.LocationCode;

                        List<EosTopupProducts> testTopupProducts = new List<EosTopupProducts>();

                        foreach (var item in testTopupNarrative.Products)
                        {
                            if (item.Collected > 0)
                            {
                                testTopupProducts.Add(new EosTopupProducts
                                {
                                    Id = item.Id,
                                    ActionId = item.ActionId,
                                    ServiceId = item.ServiceId,
                                    Name = item.Name,
                                    Description = item.Description,
                                    Collected = item.Collected,
                                    CollectionAmount = item.CollectionAmount,
                                    Currency = item.Currency,
                                    Quantity = item.Quantity
                                });
                            }
                        }

                        eosTopupRequest.Products = testTopupProducts;

                        #region OldCode
                        /*
                        eosRequest.Id = deserializedNarrative.Id;
                        eosRequest.ServiceId = deserializedNarrative.ServiceId;
                        eosRequest.TransactionType = deserializedNarrative.TransactionType;
                        eosRequest.TransactionCode = deserializedNarrative.TransactionCode;
                        eosRequest.CustomerId = deserializedNarrative.CustomerId;
                        eosRequest.ReceiversName = deserializedNarrative.ReceiversName;
                        eosRequest.ReceiversSurname = deserializedNarrative.ReceiversSurname;
                        eosRequest.ReceiversIdentification = deserializedNarrative.ReceiversIdentification;
                        eosRequest.ReceiversGender = deserializedNarrative.ReceiversGender;
                        eosRequest.ServiceRegion = deserializedNarrative.ServiceRegion;
                        eosRequest.ServiceProvince = deserializedNarrative.ServiceProvince;
                        eosRequest.ServiceCountry = deserializedNarrative.ServiceCountry;
                        eosRequest.Status = deserializedNarrative.Status;
                        eosRequest.Currency = deserializedNarrative.Currency;
                        eosRequest.Balance = deserializedNarrative.Balance;
                        eosRequest.ServiceName = deserializedNarrative.ServiceName;
                        eosRequest.ServiceType = deserializedNarrative.ServiceType;
                        eosRequest.Quantity = deserializedNarrative.Quantity;
                        eosRequest.ProductDetails = deserializedNarrative.ProductDetails;
                        eosRequest.ServiceProvider = deserializedNarrative.ServiceProvider;
                        eosRequest.ProviderAccountNumber = deserializedNarrative.ProviderAccountNumber;
                        eosRequest.SupplierId = deserializedNarrative.SupplierId;
                        eosRequest.ServiceAgentId = deserializedNarrative.ServiceAgentId;
                        eosRequest.SupplierName = deserializedNarrative.SupplierName;
                        eosRequest.Description = deserializedNarrative.Description;
                        eosRequest.CustomerName = deserializedNarrative.CustomerName;
                        eosRequest.CustomerMobileNumber = deserializedNarrative.CustomerMobileNumber;
                        eosRequest.CustomerCardNumber = deserializedNarrative.CustomerCardNumber;
                        eosRequest.Information1 = deserializedNarrative.Information1;
                        eosRequest.Information2 = deserializedNarrative.Information2;
                        eosRequest.DateCreated = deserializedNarrative.DateCreated;
                        eosRequest.DatelastAccess = deserializedNarrative.DatelastAccess;
                        eosRequest.SubDue = deserializedNarrative.SubDue;
                        eosRequest.BillingCycle = deserializedNarrative.BillingCycle;
                        eosRequest.ReceiverMobile = deserializedNarrative.ReceiverMobile;
                        eosRequest.ResponseCode = "00555";
                        eosRequest.AllowPartPayment = deserializedNarrative.AllowPartPayment;
                        eosRequest.DeactivateOnAuthorisation = deserializedNarrative.DeactivateOnAuthorisation;
                        eosRequest.Cashier = deserializedNarrative.Cashier;
                        eosRequest.Authoriser = deserializedNarrative.Authoriser;
                        eosRequest.LocationCode = deserializedNarrative.LocationCode;
                        eosRequest.JsonProducts = deserializedNarrative.JsonProducts;
                        eosRequest.InitialProducts = deserializedNarrative.InitialProducts;

                        List<Products> products = new List<Products>();

                        foreach (var item in deserializedNarrative.Products)
                        {
                            products.Add(item);
                        }

                        eosRequest.Products = products;*/
                        #endregion                        

                        Log.RequestsAndResponses("EOS-Test-Topup-Request", serviceProvider, eosTopupRequest);

                        var testTopupResponse = eosConnector.PostTopup(eosTopupRequest, serviceProvider);

                        Log.RequestsAndResponses("EOS-Test-Topup-Response", serviceProvider, testTopupResponse);

                        var strTopupTestResponse = JsonConvert.SerializeObject(testTopupResponse);

                        if (testTopupResponse.code == "200")
                        {
                            yoAppResponse.ResponseCode = "00000";
                            yoAppResponse.Description = testTopupResponse.msg;
                            yoAppResponse.Note = "Success";
                            yoAppResponse.Narrative = "Transaction/Reversal Posted successfully. Response Object: " + testTopupResponse;

                            Log.RequestsAndResponses("EOS-Test-Topup-Response-YoApp", serviceProvider, testTopupResponse);

                            return yoAppResponse;
                        }
                        else
                        {
                            yoAppResponse.ResponseCode = "00008";
                            yoAppResponse.Description = testTopupResponse.msg;
                            yoAppResponse.Note = "Failed";
                            yoAppResponse.Narrative = "Failed to post reversal. Response Object: " + testTopupResponse;

                            Log.RequestsAndResponses("EOS-Test-Topup-Response-YoApp", serviceProvider, testTopupResponse);

                            return yoAppResponse;
                        }

                    case 4: // Actual Redemption
                        EosRedemptionRequest eosRedemption = new EosRedemptionRequest();

                        var deserializedRedemptionNarrative = JsonConvert.DeserializeObject<Narrative>(apiYoAppResponse.Narrative);

                        eosRedemption.TransactionCode = deserializedRedemptionNarrative.TransactionCode;
                        eosRedemption.CustomerId = deserializedRedemptionNarrative.CustomerId;
                        eosRedemption.ReceiversName = deserializedRedemptionNarrative.ReceiversName;
                        eosRedemption.ReceiversSurname = deserializedRedemptionNarrative.ReceiversSurname;
                        eosRedemption.ReceiversIdentification = deserializedRedemptionNarrative.ReceiversIdentification;
                        eosRedemption.ServiceRegion = deserializedRedemptionNarrative.ServiceRegion;
                        eosRedemption.ServiceProvince = deserializedRedemptionNarrative.ServiceProvince;
                        eosRedemption.SupplierId = deserializedRedemptionNarrative.SupplierId;
                        eosRedemption.SupplierName = deserializedRedemptionNarrative.SupplierName;
                        eosRedemption.CustomerName = deserializedRedemptionNarrative.CustomerName;
                        eosRedemption.ResponseCode = "00555";
                        eosRedemption.LocationCode = deserializedRedemptionNarrative.LocationCode;

                        List<EosRedemptionProducts> eosRedemptions = new List<EosRedemptionProducts>();

                        foreach (var item in deserializedRedemptionNarrative.Products)
                        {
                            if (item.Collected > 0)
                            {
                                eosRedemptions.Add(new EosRedemptionProducts
                                {
                                    Id = item.Id,
                                    ActionId = item.ActionId,
                                    ServiceId = item.ServiceId,
                                    Name = item.Name,
                                    Description = item.Description,
                                    Collected = item.Collected,
                                    CollectionAmount = item.CollectionAmount,
                                    Currency = item.Currency
                                });
                            }
                        }

                        eosRedemption.Products = eosRedemptions;

                        #region OldCode
                        /*
                        eosRequest.Id = deserializedNarrative.Id;
                        eosRequest.ServiceId = deserializedNarrative.ServiceId;
                        eosRequest.TransactionType = deserializedNarrative.TransactionType;
                        eosRequest.TransactionCode = deserializedNarrative.TransactionCode;
                        eosRequest.CustomerId = deserializedNarrative.CustomerId;
                        eosRequest.ReceiversName = deserializedNarrative.ReceiversName;
                        eosRequest.ReceiversSurname = deserializedNarrative.ReceiversSurname;
                        eosRequest.ReceiversIdentification = deserializedNarrative.ReceiversIdentification;
                        eosRequest.ReceiversGender = deserializedNarrative.ReceiversGender;
                        eosRequest.ServiceRegion = deserializedNarrative.ServiceRegion;
                        eosRequest.ServiceProvince = deserializedNarrative.ServiceProvince;
                        eosRequest.ServiceCountry = deserializedNarrative.ServiceCountry;
                        eosRequest.Status = deserializedNarrative.Status;
                        eosRequest.Currency = deserializedNarrative.Currency;
                        eosRequest.Balance = deserializedNarrative.Balance;
                        eosRequest.ServiceName = deserializedNarrative.ServiceName;
                        eosRequest.ServiceType = deserializedNarrative.ServiceType;
                        eosRequest.Quantity = deserializedNarrative.Quantity;
                        eosRequest.ProductDetails = deserializedNarrative.ProductDetails;
                        eosRequest.ServiceProvider = deserializedNarrative.ServiceProvider;
                        eosRequest.ProviderAccountNumber = deserializedNarrative.ProviderAccountNumber;
                        eosRequest.SupplierId = deserializedNarrative.SupplierId;
                        eosRequest.ServiceAgentId = deserializedNarrative.ServiceAgentId;
                        eosRequest.SupplierName = deserializedNarrative.SupplierName;
                        eosRequest.Description = deserializedNarrative.Description;
                        eosRequest.CustomerName = deserializedNarrative.CustomerName;
                        eosRequest.CustomerMobileNumber = deserializedNarrative.CustomerMobileNumber;
                        eosRequest.CustomerCardNumber = deserializedNarrative.CustomerCardNumber;
                        eosRequest.Information1 = deserializedNarrative.Information1;
                        eosRequest.Information2 = deserializedNarrative.Information2;
                        eosRequest.DateCreated = deserializedNarrative.DateCreated;
                        eosRequest.DatelastAccess = deserializedNarrative.DatelastAccess;
                        eosRequest.SubDue = deserializedNarrative.SubDue;
                        eosRequest.BillingCycle = deserializedNarrative.BillingCycle;
                        eosRequest.ReceiverMobile = deserializedNarrative.ReceiverMobile;
                        eosRequest.ResponseCode = "00555";
                        eosRequest.AllowPartPayment = deserializedNarrative.AllowPartPayment;
                        eosRequest.DeactivateOnAuthorisation = deserializedNarrative.DeactivateOnAuthorisation;
                        eosRequest.Cashier = deserializedNarrative.Cashier;
                        eosRequest.Authoriser = deserializedNarrative.Authoriser;
                        eosRequest.LocationCode = deserializedNarrative.LocationCode;
                        eosRequest.JsonProducts = deserializedNarrative.JsonProducts;
                        eosRequest.InitialProducts = deserializedNarrative.InitialProducts;

                        List<Products> products = new List<Products>();

                        foreach (var item in deserializedNarrative.Products)
                        {
                            products.Add(item);
                        }

                        eosRequest.Products = products;*/
                        #endregion                        

                        Log.RequestsAndResponses("EOS-Redemption-Request", serviceProvider, eosRedemption);

                        var eosActualResponse = eosConnector.PostRedemption(eosRedemption, serviceProvider);

                        Log.RequestsAndResponses("EOS-Redemption-Response", serviceProvider, eosActualResponse);

                        var stringActualResponse = JsonConvert.SerializeObject(eosActualResponse);

                        if (eosActualResponse.code == "200")
                        {
                            yoAppResponse.ResponseCode = "00000";
                            yoAppResponse.Description = eosActualResponse.msg;
                            yoAppResponse.Note = "Success";
                            yoAppResponse.Narrative = "Transaction/Redemption Posted successfully. Response Object:" + stringActualResponse;

                            Log.RequestsAndResponses("EOS-Redemption-Response-YoApp", serviceProvider, eosActualResponse);

                            return yoAppResponse;
                        }
                        else
                        {
                            yoAppResponse.ResponseCode = "00008";
                            yoAppResponse.Description = eosActualResponse.msg;
                            yoAppResponse.Note = "Failed";
                            yoAppResponse.Narrative = "Failed to process transaction: Response Object: " + stringActualResponse;

                            Log.RequestsAndResponses("EOS-Redemption-Response-YoApp", serviceProvider, eosActualResponse);

                            return yoAppResponse;
                        }

                    case 5: // Actual Reversal
                        EosReversalRequest eosReversal = new EosReversalRequest();

                        var narrative1 = JsonConvert.DeserializeObject<Narrative>(apiYoAppResponse.Narrative);

                        eosReversal.TransactionCode = narrative1.TransactionCode;
                        eosReversal.CustomerId = narrative1.CustomerId;
                        eosReversal.ReceiversName = narrative1.ReceiversName;
                        eosReversal.ReceiversSurname = narrative1.ReceiversSurname;
                        eosReversal.ReceiversIdentification = narrative1.ReceiversIdentification;
                        eosReversal.ServiceRegion = narrative1.ServiceRegion;
                        eosReversal.ServiceProvince = narrative1.ServiceProvince;
                        eosReversal.SupplierId = narrative1.SupplierId;
                        eosReversal.SupplierName = narrative1.SupplierName;
                        eosReversal.CustomerName = narrative1.CustomerName;
                        eosReversal.ResponseCode = "00777";
                        eosReversal.LocationCode = narrative1.LocationCode;

                        List<EosReversalProducts> eosReversals = new List<EosReversalProducts>();

                        foreach (var item in narrative1.Products)
                        {
                            if (item.Collected > 0)
                            {
                                eosReversals.Add(new EosReversalProducts
                                {
                                    Id = item.Id,
                                    ActionId = item.ActionId,
                                    ServiceId = item.ServiceId,
                                    Name = item.Name,
                                    Description = item.Description,
                                    Collected = item.Collected,
                                    CollectionAmount = item.CollectionAmount,
                                    Currency = item.Currency
                                });
                            }

                        }

                        eosReversal.Products = eosReversals;

                        #region OldCode
                        /*
                        eosRequest.Id = deserializedNarrative.Id;
                        eosRequest.ServiceId = deserializedNarrative.ServiceId;
                        eosRequest.TransactionType = deserializedNarrative.TransactionType;
                        eosRequest.TransactionCode = deserializedNarrative.TransactionCode;
                        eosRequest.CustomerId = deserializedNarrative.CustomerId;
                        eosRequest.ReceiversName = deserializedNarrative.ReceiversName;
                        eosRequest.ReceiversSurname = deserializedNarrative.ReceiversSurname;
                        eosRequest.ReceiversIdentification = deserializedNarrative.ReceiversIdentification;
                        eosRequest.ReceiversGender = deserializedNarrative.ReceiversGender;
                        eosRequest.ServiceRegion = deserializedNarrative.ServiceRegion;
                        eosRequest.ServiceProvince = deserializedNarrative.ServiceProvince;
                        eosRequest.ServiceCountry = deserializedNarrative.ServiceCountry;
                        eosRequest.Status = deserializedNarrative.Status;
                        eosRequest.Currency = deserializedNarrative.Currency;
                        eosRequest.Balance = deserializedNarrative.Balance;
                        eosRequest.ServiceName = deserializedNarrative.ServiceName;
                        eosRequest.ServiceType = deserializedNarrative.ServiceType;
                        eosRequest.Quantity = deserializedNarrative.Quantity;
                        eosRequest.ProductDetails = deserializedNarrative.ProductDetails;
                        eosRequest.ServiceProvider = deserializedNarrative.ServiceProvider;
                        eosRequest.ProviderAccountNumber = deserializedNarrative.ProviderAccountNumber;
                        eosRequest.SupplierId = deserializedNarrative.SupplierId;
                        eosRequest.ServiceAgentId = deserializedNarrative.ServiceAgentId;
                        eosRequest.SupplierName = deserializedNarrative.SupplierName;
                        eosRequest.Description = deserializedNarrative.Description;
                        eosRequest.CustomerName = deserializedNarrative.CustomerName;
                        eosRequest.CustomerMobileNumber = deserializedNarrative.CustomerMobileNumber;
                        eosRequest.CustomerCardNumber = deserializedNarrative.CustomerCardNumber;
                        eosRequest.Information1 = deserializedNarrative.Information1;
                        eosRequest.Information2 = deserializedNarrative.Information2;
                        eosRequest.DateCreated = deserializedNarrative.DateCreated;
                        eosRequest.DatelastAccess = deserializedNarrative.DatelastAccess;
                        eosRequest.SubDue = deserializedNarrative.SubDue;
                        eosRequest.BillingCycle = deserializedNarrative.BillingCycle;
                        eosRequest.ReceiverMobile = deserializedNarrative.ReceiverMobile;
                        eosRequest.ResponseCode = "00555";
                        eosRequest.AllowPartPayment = deserializedNarrative.AllowPartPayment;
                        eosRequest.DeactivateOnAuthorisation = deserializedNarrative.DeactivateOnAuthorisation;
                        eosRequest.Cashier = deserializedNarrative.Cashier;
                        eosRequest.Authoriser = deserializedNarrative.Authoriser;
                        eosRequest.LocationCode = deserializedNarrative.LocationCode;
                        eosRequest.JsonProducts = deserializedNarrative.JsonProducts;
                        eosRequest.InitialProducts = deserializedNarrative.InitialProducts;

                        List<Products> products = new List<Products>();

                        foreach (var item in deserializedNarrative.Products)
                        {
                            products.Add(item);
                        }

                        eosRequest.Products = products;*/
                        #endregion                        

                        Log.RequestsAndResponses("EOS-Reversal-Request", serviceProvider, eosReversal);

                        var eosReversalResponse = eosConnector.PostReversal(eosReversal, serviceProvider);

                        Log.RequestsAndResponses("EOS-Reversal-Response", serviceProvider, eosReversalResponse);

                        var _strResponse = JsonConvert.SerializeObject(eosReversalResponse);

                        if (eosReversalResponse.code == "200")
                        {
                            yoAppResponse.ResponseCode = "00000";
                            yoAppResponse.Description = eosReversalResponse.msg;
                            yoAppResponse.Note = "Success";
                            yoAppResponse.Narrative = "Transaction/Reversal Posted successfully. Response Object: " + _strResponse;

                            Log.RequestsAndResponses("EOS-Reversal-Response-YoApp", serviceProvider, eosReversalResponse);

                            return yoAppResponse;
                        }
                        else
                        {
                            yoAppResponse.ResponseCode = "00008";
                            yoAppResponse.Description = eosReversalResponse.msg;
                            yoAppResponse.Note = "Failed";
                            yoAppResponse.Narrative = "Failed to post reversal. Response Object: " + _strResponse;

                            Log.RequestsAndResponses("EOS-Reversal-Response-YoApp", serviceProvider, eosReversalResponse);

                            return yoAppResponse;
                        }

                    case 6: // Actual Topup
                        EosTopupRequest _eosTopupRequest = new EosTopupRequest();

                        var topupNarrative = JsonConvert.DeserializeObject<Narrative>(apiYoAppResponse.Narrative);

                        _eosTopupRequest.TransactionCode = topupNarrative.TransactionCode;
                        _eosTopupRequest.CustomerId = topupNarrative.CustomerId;
                        _eosTopupRequest.ReceiversName = topupNarrative.ReceiversName;
                        _eosTopupRequest.ReceiversSurname = topupNarrative.ReceiversSurname;
                        _eosTopupRequest.ReceiversIdentification = topupNarrative.ReceiversIdentification;
                        _eosTopupRequest.ServiceRegion = topupNarrative.ServiceRegion;
                        _eosTopupRequest.ServiceProvince = topupNarrative.ServiceProvince;
                        _eosTopupRequest.SupplierId = topupNarrative.SupplierId;
                        _eosTopupRequest.SupplierName = topupNarrative.SupplierName;
                        _eosTopupRequest.CustomerName = topupNarrative.CustomerName;
                        _eosTopupRequest.ResponseCode = "00888";
                        _eosTopupRequest.LocationCode = topupNarrative.LocationCode;

                        List<EosTopupProducts> topupProducts = new List<EosTopupProducts>();

                        foreach (var item in topupNarrative.Products)
                        {
                            if (item.Collected > 0)
                            {
                                topupProducts.Add(new EosTopupProducts
                                {
                                    Id = item.Id,
                                    ActionId = item.ActionId,
                                    ServiceId = item.ServiceId,
                                    Name = item.Name,
                                    Description = item.Description,
                                    Quantity = item.Quantity,
                                    Price = item.Price,
                                    Currency = item.Currency
                                });
                            }

                        }

                        _eosTopupRequest.Products = topupProducts;

                        #region OldCode
                        /*
                        eosRequest.Id = deserializedNarrative.Id;
                        eosRequest.ServiceId = deserializedNarrative.ServiceId;
                        eosRequest.TransactionType = deserializedNarrative.TransactionType;
                        eosRequest.TransactionCode = deserializedNarrative.TransactionCode;
                        eosRequest.CustomerId = deserializedNarrative.CustomerId;
                        eosRequest.ReceiversName = deserializedNarrative.ReceiversName;
                        eosRequest.ReceiversSurname = deserializedNarrative.ReceiversSurname;
                        eosRequest.ReceiversIdentification = deserializedNarrative.ReceiversIdentification;
                        eosRequest.ReceiversGender = deserializedNarrative.ReceiversGender;
                        eosRequest.ServiceRegion = deserializedNarrative.ServiceRegion;
                        eosRequest.ServiceProvince = deserializedNarrative.ServiceProvince;
                        eosRequest.ServiceCountry = deserializedNarrative.ServiceCountry;
                        eosRequest.Status = deserializedNarrative.Status;
                        eosRequest.Currency = deserializedNarrative.Currency;
                        eosRequest.Balance = deserializedNarrative.Balance;
                        eosRequest.ServiceName = deserializedNarrative.ServiceName;
                        eosRequest.ServiceType = deserializedNarrative.ServiceType;
                        eosRequest.Quantity = deserializedNarrative.Quantity;
                        eosRequest.ProductDetails = deserializedNarrative.ProductDetails;
                        eosRequest.ServiceProvider = deserializedNarrative.ServiceProvider;
                        eosRequest.ProviderAccountNumber = deserializedNarrative.ProviderAccountNumber;
                        eosRequest.SupplierId = deserializedNarrative.SupplierId;
                        eosRequest.ServiceAgentId = deserializedNarrative.ServiceAgentId;
                        eosRequest.SupplierName = deserializedNarrative.SupplierName;
                        eosRequest.Description = deserializedNarrative.Description;
                        eosRequest.CustomerName = deserializedNarrative.CustomerName;
                        eosRequest.CustomerMobileNumber = deserializedNarrative.CustomerMobileNumber;
                        eosRequest.CustomerCardNumber = deserializedNarrative.CustomerCardNumber;
                        eosRequest.Information1 = deserializedNarrative.Information1;
                        eosRequest.Information2 = deserializedNarrative.Information2;
                        eosRequest.DateCreated = deserializedNarrative.DateCreated;
                        eosRequest.DatelastAccess = deserializedNarrative.DatelastAccess;
                        eosRequest.SubDue = deserializedNarrative.SubDue;
                        eosRequest.BillingCycle = deserializedNarrative.BillingCycle;
                        eosRequest.ReceiverMobile = deserializedNarrative.ReceiverMobile;
                        eosRequest.ResponseCode = "00555";
                        eosRequest.AllowPartPayment = deserializedNarrative.AllowPartPayment;
                        eosRequest.DeactivateOnAuthorisation = deserializedNarrative.DeactivateOnAuthorisation;
                        eosRequest.Cashier = deserializedNarrative.Cashier;
                        eosRequest.Authoriser = deserializedNarrative.Authoriser;
                        eosRequest.LocationCode = deserializedNarrative.LocationCode;
                        eosRequest.JsonProducts = deserializedNarrative.JsonProducts;
                        eosRequest.InitialProducts = deserializedNarrative.InitialProducts;

                        List<Products> products = new List<Products>();

                        foreach (var item in deserializedNarrative.Products)
                        {
                            products.Add(item);
                        }

                        eosRequest.Products = products;*/
                        #endregion                        

                        Log.RequestsAndResponses("EOS-Topup-Request", serviceProvider, _eosTopupRequest);

                        var topupResponse = eosConnector.PostTopup(_eosTopupRequest, serviceProvider);

                        Log.RequestsAndResponses("EOS-Topup-Response", serviceProvider, topupResponse);

                        var _strTopupTestResponse = JsonConvert.SerializeObject(topupResponse);

                        if (topupResponse.code == "200")
                        {
                            yoAppResponse.ResponseCode = "00000";
                            yoAppResponse.Description = topupResponse.msg;
                            yoAppResponse.Note = "Success";
                            yoAppResponse.Narrative = "Transaction/Reversal Posted successfully. Response Object: " + _strTopupTestResponse;

                            Log.RequestsAndResponses("EOS-Topup-Response-YoApp", serviceProvider, topupResponse);

                            return yoAppResponse;
                        }
                        else
                        {
                            yoAppResponse.ResponseCode = "00008";
                            yoAppResponse.Description = topupResponse.msg;
                            yoAppResponse.Note = "Failed";
                            yoAppResponse.Narrative = "Failed to post reversal. Response Object: " + _strTopupTestResponse;

                            Log.RequestsAndResponses("EOS-Topup-Response-YoApp", serviceProvider, topupResponse);

                            return yoAppResponse;
                        }
                }
            }

            return yoAppResponse;
        }

        [Route("api/files/upload")]
        [HttpPost]
        public async Task<string> Post()
        {
            YoAppResponse response = new YoAppResponse();

            try
            {
                var httpRequest = System.Web.HttpContext.Current.Request;

                if (httpRequest.Files.Count > 0)
                {
                    foreach (string file in httpRequest.Files)
                    {
                        var postedFile = httpRequest.Files[file];

                        var fileName = postedFile.FileName.Split('\\').LastOrDefault().Split('/').LastOrDefault();

                        var filePath = System.Web.HttpContext.Current.Server.MapPath("~/Uploads/" + fileName);

                        postedFile.SaveAs(filePath);
                    }

                    response.ResponseCode = "00000";
                    response.Description = "Image Saved Successfully!";
                    return JsonConvert.SerializeObject(response);
                }
                else
                {
                    response.ResponseCode = "Error";
                    response.Description = "Image not saved!";
                    return JsonConvert.SerializeObject(response);
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [Route("api/bulk-payments/cbz-services")]
        [HttpPost]
        public YoAppResponse BulkPayments(YoAppResponse apiYoAppResponse)
        {
            #region Declared Objects
            var serviceProvider = "CBZ-BulkPayments";
            YoAppResponse yoAppResponse = new YoAppResponse();
            #endregion

            if (apiYoAppResponse == null)
            {
                string message = "Received Nothing. Your request object is null";

                yoAppResponse.ResponseCode = "00008";
                yoAppResponse.Note = "Failed";
                yoAppResponse.Description = message;

                return yoAppResponse;
            }
            else
            {
                BulkPaymentsConnector bulkPaymentsConnector = new BulkPaymentsConnector();
                BulkPaymentsMethods bulkPayments = new BulkPaymentsMethods();

                var referenceNumber = bulkPayments.ReferenceNumber(serviceProvider);
                var yoAppDesiarilizedNarrative = JsonConvert.DeserializeObject<Narrative>(apiYoAppResponse.Narrative);

                switch (apiYoAppResponse.ServiceId)
                {
                    case 1: // AAT Batch Request                       
                        AATBatchRequest batchRequest = new AATBatchRequest();
                        batchRequest.InstitutionID = yoAppDesiarilizedNarrative.CustomerId;
                        batchRequest.ExtReferenceNo = referenceNumber;

                        // Get actual YoApp request to construct 'Request' object.

                        Log.RequestsAndResponses("AAT-Batch-Request", serviceProvider, batchRequest);

                        var aatBulkPaymentsResponse = bulkPaymentsConnector.PostAATBatch(batchRequest, serviceProvider);

                        Log.RequestsAndResponses("AAT-Batch-Response", serviceProvider, aatBulkPaymentsResponse);

                        if (aatBulkPaymentsResponse.Response.Status.ToUpper() == "SUCCESS")
                        {
                            yoAppResponse.ResponseCode = "00000";
                            yoAppResponse.Description = aatBulkPaymentsResponse.Status;
                            yoAppResponse.Note = "Success";
                            yoAppResponse.Narrative = "Batch Posted successfully. Response Object: " + aatBulkPaymentsResponse;

                            Log.RequestsAndResponses("AATBatch-Response-YoApp", serviceProvider, aatBulkPaymentsResponse);

                            return yoAppResponse;
                        }
                        else if (aatBulkPaymentsResponse.Response.Status.ToUpper() == "FAILURE")
                        {
                            yoAppResponse.ResponseCode = "00008";
                            yoAppResponse.Description = aatBulkPaymentsResponse.Status;
                            yoAppResponse.Note = "Failed";
                            yoAppResponse.Narrative = "Batch Post Failed. Response Object: " + aatBulkPaymentsResponse;

                            Log.RequestsAndResponses("AATBatch-Response-YoApp", serviceProvider, aatBulkPaymentsResponse);

                            return yoAppResponse;
                        }
                        else
                        {
                            yoAppResponse.ResponseCode = "00008";
                            yoAppResponse.Description = aatBulkPaymentsResponse.Status;
                            yoAppResponse.Note = "Failed";
                            yoAppResponse.Narrative = "Batch Post Failed. Response Object: " + aatBulkPaymentsResponse;

                            Log.RequestsAndResponses("AATBatch-Response-YoApp", serviceProvider, aatBulkPaymentsResponse);

                            return yoAppResponse;
                        }

                    case 2: // AAT Batch Status Request                       

                        AATBatchStatusCheckRequest batchStatusCheckRequest = new AATBatchStatusCheckRequest();

                        batchStatusCheckRequest.InstitutionID = yoAppDesiarilizedNarrative.CustomerId;
                        batchStatusCheckRequest.ExtReferenceNo = referenceNumber;

                        // Get actual YoApp request to construct the 'Request' object.

                        Log.RequestsAndResponses("AAT-Status-Request", serviceProvider, batchStatusCheckRequest);

                        var aatBulkPaymentsStatusResponse = bulkPaymentsConnector.BatchStatusCheck(batchStatusCheckRequest, serviceProvider);

                        Log.RequestsAndResponses("AAT-Status-Response", serviceProvider, aatBulkPaymentsStatusResponse);

                        if (aatBulkPaymentsStatusResponse.Status.ToUpper() == "INCOMING")
                        {
                            yoAppResponse.ResponseCode = "00000";
                            yoAppResponse.Description = aatBulkPaymentsStatusResponse.Status;
                            yoAppResponse.Note = "INCOMING";
                            yoAppResponse.Narrative = "Batch Status is 'INCOMING'. Response Object: " + aatBulkPaymentsStatusResponse;

                            Log.RequestsAndResponses("AATStatus-Response-YoApp", serviceProvider, aatBulkPaymentsStatusResponse);

                            return yoAppResponse;
                        }
                        else if (aatBulkPaymentsStatusResponse.Status.ToUpper() == "PENDING")
                        {
                            yoAppResponse.ResponseCode = "00000";
                            yoAppResponse.Description = aatBulkPaymentsStatusResponse.Status;
                            yoAppResponse.Note = "PENDING";
                            yoAppResponse.Narrative = "Batch Status is 'PENDING'. Response Object: " + aatBulkPaymentsStatusResponse;

                            Log.RequestsAndResponses("AATStatus-Response-YoApp", serviceProvider, aatBulkPaymentsStatusResponse);

                            return yoAppResponse;
                        }
                        else if (aatBulkPaymentsStatusResponse.Status.ToUpper() == "PROCESSING_FLEX")
                        {
                            yoAppResponse.ResponseCode = "00000";
                            yoAppResponse.Description = aatBulkPaymentsStatusResponse.Status;
                            yoAppResponse.Note = "PROCESSING_FLEX";
                            yoAppResponse.Narrative = "Batch Status is 'PROCESSING_FLEX'. Response Object: " + aatBulkPaymentsStatusResponse;

                            Log.RequestsAndResponses("AATStatus-Response-YoApp", serviceProvider, aatBulkPaymentsStatusResponse);

                            return yoAppResponse;
                        }
                        else if (aatBulkPaymentsStatusResponse.Status.ToUpper() == "PROCESSING_MFS")
                        {
                            yoAppResponse.ResponseCode = "00000";
                            yoAppResponse.Description = aatBulkPaymentsStatusResponse.Status;
                            yoAppResponse.Note = "PROCESSING_MFS";
                            yoAppResponse.Narrative = "Batch Status is 'PROCESSING_MFS'. Response Object: " + aatBulkPaymentsStatusResponse;

                            Log.RequestsAndResponses("AATStatus-Response-YoApp", serviceProvider, aatBulkPaymentsStatusResponse);

                            return yoAppResponse;
                        }
                        else
                        {
                            yoAppResponse.ResponseCode = "00008";
                            yoAppResponse.Description = aatBulkPaymentsStatusResponse.Status;
                            yoAppResponse.Note = "Invalid Reference Number";
                            yoAppResponse.Narrative = "Invalid reference number. Response Object: " + aatBulkPaymentsStatusResponse;

                            Log.RequestsAndResponses("AATStatus-Response-YoApp", serviceProvider, aatBulkPaymentsStatusResponse);

                            return yoAppResponse;
                        }

                    case 3: // IBT Request

                        IBTRequest iBTRequest = new IBTRequest();

                        iBTRequest.InstitutionID = yoAppDesiarilizedNarrative.CustomerId;
                        iBTRequest.ExtReferenceNo = referenceNumber;

                        // Get actual YoApp request to construct the 'Request' object.

                        Log.RequestsAndResponses("IBT-Request", serviceProvider, iBTRequest);

                        var ibtBulkPaymentsResponse = bulkPaymentsConnector.IBTBulkPayment(iBTRequest, serviceProvider);

                        Log.RequestsAndResponses("IBT-Response", serviceProvider, ibtBulkPaymentsResponse);

                        if (ibtBulkPaymentsResponse.Contra.Response.Status == "SUCCESS")
                        {
                            yoAppResponse.ResponseCode = "00000";
                            yoAppResponse.Note = "SUCCESS";
                            yoAppResponse.Description = ibtBulkPaymentsResponse.Narrative;
                            yoAppResponse.Narrative = "Response Object: " + ibtBulkPaymentsResponse;

                            Log.RequestsAndResponses("IBTPayment-Response-YoApp", serviceProvider, ibtBulkPaymentsResponse);

                            return yoAppResponse;
                        }
                        else if (ibtBulkPaymentsResponse.Contra.Response.Status == "FAILURE")
                        {
                            yoAppResponse.ResponseCode = "00008";
                            yoAppResponse.Description = ibtBulkPaymentsResponse.Narrative;
                            yoAppResponse.Note = "FAILURE";
                            yoAppResponse.Narrative = "Response Object: " + ibtBulkPaymentsResponse;

                            Log.RequestsAndResponses("IBTPayment-Response-YoApp", serviceProvider, ibtBulkPaymentsResponse);

                            return yoAppResponse;
                        }
                        else
                        {
                            yoAppResponse.ResponseCode = "00008";
                            yoAppResponse.Note = "FAILURE";
                            yoAppResponse.Description = ibtBulkPaymentsResponse.Narrative;
                            yoAppResponse.Narrative = "Response Object: " + ibtBulkPaymentsResponse;

                            Log.RequestsAndResponses("IBTPayment-Response-YoApp", serviceProvider, ibtBulkPaymentsResponse);

                            return yoAppResponse;
                        }

                    case 4: // 31_BEQ_1 Request

                        _31_BEQ_1_Request _31_BEQ_1_Request = new _31_BEQ_1_Request();

                        _31_BEQ_1_Request.InstitutionID = deserializedYoAppNarrative.CustomerId;
                        _31_BEQ_1_Request.ExtReferenceNo = referenceNumber;

                        // Get actual YoApp request to construct the 'Request' object.

                        Log.RequestsAndResponses("31_BEQ_1-Request", serviceProvider, _31_BEQ_1_Request);

                        var _31_BEQ_1_BulkPaymentsResponse = bulkPaymentsConnector.BEQPayment(_31_BEQ_1_Request, serviceProvider);

                        Log.RequestsAndResponses("31_BEQ_1-Response", serviceProvider, _31_BEQ_1_BulkPaymentsResponse);

                        if (_31_BEQ_1_BulkPaymentsResponse.Response.message == "SUCCESS")
                        {
                            yoAppResponse.ResponseCode = "00000";
                            yoAppResponse.Note = "SUCCESS";
                            yoAppResponse.Description = _31_BEQ_1_BulkPaymentsResponse.Response.message;
                            yoAppResponse.Narrative = "Response Object: " + _31_BEQ_1_BulkPaymentsResponse;

                            Log.RequestsAndResponses("BEQ-Response-YoApp", serviceProvider, _31_BEQ_1_BulkPaymentsResponse);

                            return yoAppResponse;
                        }
                        else if (_31_BEQ_1_BulkPaymentsResponse.Response.message == "FAILURE")
                        {
                            yoAppResponse.ResponseCode = "00008";
                            yoAppResponse.Note = "FAILURE";
                            yoAppResponse.Description = _31_BEQ_1_BulkPaymentsResponse.Response.message;
                            yoAppResponse.Narrative = "Response Object: " + _31_BEQ_1_BulkPaymentsResponse;

                            Log.RequestsAndResponses("BEQ-Response-YoApp", serviceProvider, _31_BEQ_1_BulkPaymentsResponse);

                            return yoAppResponse;
                        }
                        else
                        {
                            yoAppResponse.ResponseCode = "00008";
                            yoAppResponse.Note = "FAILURE";
                            yoAppResponse.Description = _31_BEQ_1_BulkPaymentsResponse.Response.message;
                            yoAppResponse.Narrative = "Response Object: " + _31_BEQ_1_BulkPaymentsResponse;

                            Log.RequestsAndResponses("BEQ-Response-YoApp", serviceProvider, _31_BEQ_1_BulkPaymentsResponse);

                            return yoAppResponse;
                        }

                    case 5: // 38_MST_1 Request

                        _38_MST_1_Request _38_MST_1_Request = new _38_MST_1_Request();

                        _38_MST_1_Request.InstitutionID = deserializedYoAppNarrative.CustomerId;
                        _38_MST_1_Request.ExtReferenceNo = referenceNumber;

                        // Get actual YoApp request to construct the 'Request' object.

                        Log.RequestsAndResponses("MST-Request", serviceProvider, _38_MST_1_Request);

                        bulkPaymentsConnector.MSTPayment(_38_MST_1_Request, serviceProvider); // Created a Void Method because no RepsonseCode

                        Log.RequestsAndResponses("MST-Response", serviceProvider, _38_MST_1_Request);

                        yoAppResponse.ResponseCode = "00000";
                        yoAppResponse.Description = "check email to see if it was a success";
                        yoAppResponse.Narrative = "Response Object: ";

                        return yoAppResponse;

                    case 6: // 107_ABV_1 Request

                        _107_ABV_1_Request _Request = new _107_ABV_1_Request();

                        _Request.InstitutionID = deserializedYoAppNarrative.CustomerId;
                        _Request.ExtReferenceNo = referenceNumber;

                        // Get actual YoApp request to construct the 'Request' object.

                        Log.RequestsAndResponses("ABV-Request", serviceProvider, _Request);

                        var _PossibleResponses = bulkPaymentsConnector.ABVPayment(_Request, serviceProvider);

                        Log.RequestsAndResponses("ABV-Response", serviceProvider, _PossibleResponses);

                        if (_PossibleResponses.Response.message.ToUpper() == "Y")
                        {
                            yoAppResponse.ResponseCode = "00000";
                            yoAppResponse.Note = "SUCCESS";
                            yoAppResponse.Description = _PossibleResponses.Response.description;
                            yoAppResponse.Narrative = "Response Object: " + _PossibleResponses;

                            Log.RequestsAndResponses("ABV-Response-YoApp", serviceProvider, _PossibleResponses);

                            return yoAppResponse;
                        }
                        else if (_PossibleResponses.Response.message.ToUpper() == "E")
                        {
                            yoAppResponse.ResponseCode = "00008";
                            yoAppResponse.Note = "FAILURE";
                            yoAppResponse.Description = _PossibleResponses.Response.description;
                            yoAppResponse.Narrative = "Response Object: " + _PossibleResponses;

                            Log.RequestsAndResponses("ABV-Response-YoApp", serviceProvider, _PossibleResponses);

                            return yoAppResponse;
                        }
                        else
                        {
                            yoAppResponse.ResponseCode = "00008";
                            yoAppResponse.Note = "FAILURE";
                            yoAppResponse.Description = _PossibleResponses.Response.description;
                            yoAppResponse.Narrative = "Response Object: " + _PossibleResponses;

                            Log.RequestsAndResponses("ABV-Response-YoApp", serviceProvider, _PossibleResponses);

                            return yoAppResponse;
                        }

                    case 7: // MST 6

                        _38_MST_6_Request _6_Request = new _38_MST_6_Request();

                        _6_Request.InstitutionID = deserializedYoAppNarrative.CustomerId;
                        _6_Request.ExtReferenceNo = referenceNumber;

                        // Get actual YoApp request to construct the 'Request' object.

                        Log.RequestsAndResponses("MST6-Request", serviceProvider, _6_Request);

                        var response = bulkPaymentsConnector.MST6Payment(_6_Request, serviceProvider);

                        Log.RequestsAndResponses("MST6-Response", serviceProvider, response);

                        if (response.Response.message == "SUCCESS")
                        {
                            yoAppResponse.ResponseCode = "00000";
                            yoAppResponse.Note = "SUCCESS";
                            yoAppResponse.Description = response.Response.description;
                            yoAppResponse.Narrative = "Response Object: " + response;

                            Log.RequestsAndResponses("MST6-Response-YoApp", serviceProvider, response);

                            return yoAppResponse;
                        }
                        else if (response.Response.message == "FAILURE")
                        {
                            yoAppResponse.ResponseCode = "00008";
                            yoAppResponse.Note = "FAILURE";
                            yoAppResponse.Description = response.Response.description;
                            yoAppResponse.Narrative = "Response Object: " + response;

                            Log.RequestsAndResponses("MST6-Response-YoApp", serviceProvider, response);

                            return yoAppResponse;
                        }
                        else
                        {
                            yoAppResponse.ResponseCode = "00008";
                            yoAppResponse.Note = "FAILURE";
                            yoAppResponse.Description = response.Response.description;
                            yoAppResponse.Narrative = "Response Object: " + response;

                            Log.RequestsAndResponses("MST6-Response-YoApp", serviceProvider, response);

                            return yoAppResponse;
                        }

                    default:

                        yoAppResponse.ResponseCode = "00008";
                        yoAppResponse.Note = "FAILURE";
                        yoAppResponse.Description = "";
                        yoAppResponse.Narrative = "Response Object: ";

                        Log.RequestsAndResponses("MST6-Response-YoApp", serviceProvider, "");

                        return yoAppResponse;

                }
            }
        }

        [Route("api/vouchers/wafaya")]
        [HttpPost]
        public YoAppResponse Vouchers(YoAppResponse response)
        {
            #region Declared Objects
            var serviceProvider = "Wafaya";
            YoAppResponse yoAppResponse = new YoAppResponse();
            #endregion

            #region First Log
            Log.RequestsAndResponses("YoAppResponse", serviceProvider, response);
            #endregion

            if (response == null)
            {
                string message = "Received Nothing. Your request object is null";

                yoAppResponse.ResponseCode = "00008";
                yoAppResponse.Note = "Failed";
                yoAppResponse.Description = message;

                return yoAppResponse;
            }
            else
            {
                WafayaCredentials wafayaCredentials = new WafayaCredentials();
                WafayaCodeRequest codeRequest = new WafayaCodeRequest();
                WafayaTokenRequest tokenRequest = new WafayaTokenRequest();
                WafayaResourceOwnerRequest resourceOwnerRequest = new WafayaResourceOwnerRequest();
                Narrative yoAppNarrative = new Narrative();

                WafayaConnector connector = new WafayaConnector();
                bool isTokenValid = false;
                var fileName = "tokens";

                switch (response.ServiceId)
                {
                    case 1: // Request Token using Authorization Flow

                        switch (response.ProcessingCode)
                        {
                            case "370000": // Check Voucher

                                try
                                {
                                    WafayaVoucherRequest voucherRequest = new WafayaVoucherRequest();

                                    string file = HttpContext.Current.Server.MapPath("~/App_Data/" + serviceProvider + "/Files/" + fileName + ".json");
                                    var tokenFile = LoadJson(file);

                                    if (tokenFile != null) // We have generated the Token Already
                                    {
                                        var expiryDateString = tokenFile.expires_in;

                                        var expiryDate = DateTime.ParseExact(expiryDateString, "dd/MM/yyyy-HH:mm:ss", null);

                                        var currentDateTime = DateTime.Now;

                                        if (expiryDate > currentDateTime) // Token is still valid
                                        {
                                            isTokenValid = true;
                                        }
                                        else // Token is no longer valid
                                        {
                                            RefreshToken(tokenFile.refresh_token);
                                            isTokenValid = true;

                                            tokenFile = LoadJson(file);
                                        }
                                    }
                                    else // Generate a new Token
                                    {
                                        resourceOwnerRequest.grant_type = "password";
                                        resourceOwnerRequest.client_id = wafayaCredentials.PasswordClientId;
                                        resourceOwnerRequest.client_secret = wafayaCredentials.PasswordSecret;
                                        resourceOwnerRequest.scope = "";
                                        resourceOwnerRequest.username = wafayaCredentials.Username;
                                        resourceOwnerRequest.password = wafayaCredentials.Password;

                                        Log.RequestsAndResponses("Wafaya-TokenRequest", serviceProvider, resourceOwnerRequest);

                                        var resourceOwnerResponse = connector.GetPasswordToken(resourceOwnerRequest, serviceProvider);

                                        Log.RequestsAndResponses("Wafaya-TokenResponse", serviceProvider, resourceOwnerResponse);

                                        if (resourceOwnerResponse.token_type.ToLower() == "bearer")
                                        {
                                            isTokenValid = true;
                                            yoAppResponse.ResponseCode = "00000";
                                            yoAppResponse.Description = "Token generated successfully";
                                            yoAppResponse.Note = "Transaction Successful";

                                            var expDate = DateTime.Now.AddSeconds(Convert.ToDouble(resourceOwnerResponse.expires_in));

                                            resourceOwnerResponse.expires_in = expDate.ToString("dd/MM/yyyy-HH:mm:ss");

                                            var token = JsonConvert.SerializeObject(resourceOwnerResponse);

                                            yoAppResponse.Narrative = token;

                                            Log.RequestsAndResponses("Wafaya-TokenResponse-YoApp", serviceProvider, yoAppResponse);
                                            Log.StoreData("tokens", serviceProvider, resourceOwnerResponse);

                                            tokenFile = LoadJson(file);
                                        }
                                        else
                                        {
                                            yoAppResponse.ResponseCode = "00008";
                                            yoAppResponse.Description = "Code could not be generated";
                                            yoAppResponse.Note = "Transaction Failed";

                                            Log.RequestsAndResponses("Wafaya-TokenResponse-YoApp", serviceProvider, yoAppResponse);

                                            return yoAppResponse;
                                        }
                                    }

                                    if (isTokenValid)
                                    {
                                        voucherRequest.Voucher = response.CustomerAccount;
                                        voucherRequest.Token = tokenFile.access_token;

                                        Log.RequestsAndResponses("Wafaya-GetVoucherRequest", serviceProvider, voucherRequest);

                                        var wafayaVoucherResponse = connector.GetVoucherDetails(voucherRequest, serviceProvider);

                                        Log.RequestsAndResponses("Wafaya-GetVoucherResponse", serviceProvider, wafayaVoucherResponse);

                                        if (wafayaVoucherResponse.voucher_code != null)
                                        {
                                            yoAppResponse.ResponseCode = "00000";
                                            yoAppResponse.Description = "Voucher Retrived successfully";

                                            if (wafayaVoucherResponse.redeemer_name != null)
                                            {
                                                yoAppResponse.Note = wafayaVoucherResponse.redeemer_name;

                                                yoAppNarrative.ReceiversName = wafayaVoucherResponse.redeemer_name;

                                                yoAppNarrative.ReceiversSurname = "";
                                            }
                                            else
                                            {
                                                yoAppResponse.Note = "";

                                                yoAppNarrative.ReceiversName = "";

                                                yoAppNarrative.ReceiversSurname = "";
                                            }

                                            if (wafayaVoucherResponse.voucher_code != null)
                                            {
                                                yoAppResponse.CustomerAccount = wafayaVoucherResponse.voucher_code;
                                                yoAppNarrative.TransactionCode = wafayaVoucherResponse.voucher_code;
                                            }
                                            else
                                            {
                                                yoAppResponse.CustomerAccount = "";
                                                yoAppNarrative.TransactionCode = "";
                                            }

                                            yoAppResponse.Amount = Math.Round(wafayaVoucherResponse.voucher_value, 2);


                                            if (Convert.ToString(wafayaVoucherResponse.voucher_balance) != null)
                                            {
                                                yoAppResponse.Balance = Convert.ToString(Math.Round(wafayaVoucherResponse.voucher_balance, 2));
                                                yoAppNarrative.Balance = Math.Round(wafayaVoucherResponse.voucher_balance, 2);
                                            }
                                            else
                                            {
                                                yoAppResponse.Balance = "";
                                                yoAppNarrative.Balance = 0;
                                            }

                                            if (wafayaVoucherResponse.redeemer_phone != null)
                                            {
                                                yoAppResponse.CustomerMSISDN = wafayaVoucherResponse.redeemer_phone;
                                                yoAppNarrative.ReceiverMobile = wafayaVoucherResponse.redeemer_phone;
                                            }
                                            else
                                            {
                                                yoAppResponse.CustomerMSISDN = "";
                                                yoAppNarrative.ReceiverMobile = "";
                                            }

                                            yoAppResponse.IsActive = wafayaVoucherResponse.active;
                                            yoAppNarrative.IsActive = wafayaVoucherResponse.active;

                                            if (wafayaVoucherResponse.voucher_currency != null)
                                            {
                                                yoAppResponse.Currency = wafayaVoucherResponse.voucher_currency;
                                                yoAppNarrative.Currency = wafayaVoucherResponse.voucher_currency;
                                            }
                                            else
                                            {
                                                yoAppResponse.Currency = "";
                                                yoAppNarrative.Currency = "";
                                            }

                                            if (wafayaVoucherResponse.pin != null)
                                            {
                                                Log.StoreMpin(wafayaVoucherResponse.voucher_code, serviceProvider, wafayaVoucherResponse); // store Mpin
                                            }

                                            yoAppNarrative.ReceiversIdentification = "";
                                            yoAppNarrative.CustomerId = "Profile/8-0001-0015964";
                                            yoAppNarrative.ServiceName = "ETENGA";
                                            yoAppNarrative.ServiceType = "RETAIL";
                                            yoAppNarrative.ServiceAgentId = "5-0001-0000751";
                                            yoAppNarrative.SupplierId = "5-0001-0000751";
                                            yoAppNarrative.SupplierName = "NRICHARDS";
                                            yoAppNarrative.Description = "Etenga wallet";
                                            yoAppNarrative.CustomerName = "NRICHARDS";
                                            yoAppNarrative.CustomerMobileNumber = "263772610890";
                                            yoAppNarrative.ServiceProvider = "5-0001-0000751";
                                            yoAppNarrative.Quantity = 1;
                                            yoAppNarrative.ServiceId = 1;
                                            yoAppNarrative.ProductDetails = "1";
                                            yoAppNarrative.SuspenseBalance = 0;

                                            var narrative = JsonConvert.SerializeObject(yoAppNarrative);

                                            yoAppResponse.Narrative = narrative;

                                            Log.RequestsAndResponses("Wafaya-GetVoucherResponse-YoApp", serviceProvider, yoAppResponse);

                                            return yoAppResponse;
                                        }
                                        else
                                        {
                                            yoAppResponse.ResponseCode = "00008";
                                            yoAppResponse.Description = "Could not retrieve voucher";
                                            yoAppResponse.Note = "Request Failed";

                                            Log.RequestsAndResponses("Wafaya-TokenResponse-YoApp", serviceProvider, yoAppResponse);

                                            return yoAppResponse;
                                        }
                                    }
                                    else
                                    {
                                        yoAppResponse.ResponseCode = "00008";
                                        yoAppResponse.Description = "Token is not Valid";
                                        yoAppResponse.Note = "Request Failed";

                                        Log.RequestsAndResponses("Wafaya-TokenResponse-YoApp", serviceProvider, yoAppResponse);

                                        return yoAppResponse;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    var exceptionMessage = "Message: " + ex.Message + ", InnerException: " + ex.InnerException + ", StackTrace: " + ex.StackTrace;

                                    Log.HttpError("Exception", serviceProvider, exceptionMessage);

                                    yoAppResponse.ResponseCode = "00008";
                                    yoAppResponse.Description = "Message: " + ex.Message;

                                    Log.RequestsAndResponses("Wafaya-Exception-Response", serviceProvider, yoAppResponse);

                                    return yoAppResponse;
                                }

                            case "330000": // Verification/Initializing
                                //WafayaInitializeRedemptionRequest wafayaInitializeRedemptionRequest = new WafayaInitializeRedemptionRequest();

                                try
                                {
                                    string file2 = HttpContext.Current.Server.MapPath("~/App_Data/" + serviceProvider + "/Files/" + fileName + ".json");
                                    var tokenFile2 = LoadJson(file2);

                                    if (tokenFile2 != null) // We have generated the Token Already
                                    {
                                        var expiryDateString = tokenFile2.expires_in;

                                        var expiryDate = DateTime.ParseExact(expiryDateString, "dd/MM/yyyy-HH:mm:ss", null);

                                        var currentDateTime = DateTime.Now;

                                        if (expiryDate > currentDateTime) // Token is still valid
                                        {
                                            isTokenValid = true;
                                        }
                                        else // Token is no longer valid
                                        {
                                            RefreshToken(tokenFile2.refresh_token);
                                            isTokenValid = true;

                                            tokenFile2 = LoadJson(file2);
                                        }
                                    }
                                    else // Generate a new Token
                                    {
                                        resourceOwnerRequest.grant_type = "password";
                                        resourceOwnerRequest.client_id = wafayaCredentials.PasswordClientId;
                                        resourceOwnerRequest.client_secret = wafayaCredentials.PasswordSecret;
                                        resourceOwnerRequest.scope = "";
                                        resourceOwnerRequest.username = wafayaCredentials.Username;
                                        resourceOwnerRequest.password = wafayaCredentials.Password;

                                        Log.RequestsAndResponses("Wafaya-TokenRequest", serviceProvider, resourceOwnerRequest);

                                        var resourceOwnerResponse = connector.GetPasswordToken(resourceOwnerRequest, serviceProvider);

                                        Log.RequestsAndResponses("Wafaya-TokenResponse", serviceProvider, resourceOwnerResponse);

                                        if (resourceOwnerResponse != null && resourceOwnerResponse.token_type.ToLower() == "bearer")
                                        {
                                            isTokenValid = true;
                                            yoAppResponse.ResponseCode = "00000";
                                            yoAppResponse.Description = "Token generated successfully";
                                            yoAppResponse.Note = "Transaction Successful";

                                            var expDate = DateTime.Now.AddSeconds(Convert.ToDouble(resourceOwnerResponse.expires_in));

                                            resourceOwnerResponse.expires_in = expDate.ToString("dd/MM/yyyy-HH:mm:ss");

                                            var token = JsonConvert.SerializeObject(resourceOwnerResponse);

                                            yoAppResponse.Narrative = token;

                                            Log.RequestsAndResponses("Wafaya-TokenResponse-YoApp", serviceProvider, yoAppResponse);

                                            Log.StoreData("tokens", serviceProvider, resourceOwnerResponse);

                                            // tokenFile2 = LoadJson(file2);
                                        }
                                        else
                                        {
                                            yoAppResponse.ResponseCode = "00008";
                                            yoAppResponse.Description = "Code could not be generated";
                                            yoAppResponse.Note = "Transaction Failed";

                                            Log.RequestsAndResponses("Wafaya-TokenResponse-YoApp", serviceProvider, yoAppResponse);

                                            return yoAppResponse;
                                        }
                                    }

                                    if (isTokenValid)
                                    {
                                        yoAppResponse.ResponseCode = "00000";
                                        yoAppResponse.Description = "Customer to submit their OTP";
                                        yoAppResponse.Note = "Authorise";
                                        yoAppResponse.Quantity = 0;
                                        yoAppResponse.CustomerData = "Valid Wafaya voucher";

                                        string optFile = HttpContext.Current.Server.MapPath("~/App_Data/" + serviceProvider + "/Files/" + response.CustomerAccount + ".json");

                                        var detailsResponse = LoadPinFile(optFile);

                                        yoAppResponse.Mpin = detailsResponse.pin;

                                        Log.RequestsAndResponses("Wafaya-OTPResponse-YoApp", serviceProvider, yoAppResponse);

                                        return yoAppResponse;
                                    }
                                    else
                                    {
                                        yoAppResponse.ResponseCode = "00008";
                                        yoAppResponse.Description = "Verification Process Failed";
                                        yoAppResponse.Note = "Request Failed";

                                        Log.RequestsAndResponses("Wafaya-OTPResponse-YoApp", serviceProvider, yoAppResponse);

                                        return yoAppResponse;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    var exceptionMessage = "Message: " + ex.Message + ", InnerException: " + ex.InnerException + ", StackTrace: " + ex.StackTrace;

                                    Log.HttpError("Exception", serviceProvider, exceptionMessage);

                                    yoAppResponse.ResponseCode = "00008";
                                    yoAppResponse.Description = "Message: " + ex.Message;

                                    Log.RequestsAndResponses("Wafaya-Exception-Response", serviceProvider, yoAppResponse);

                                    return yoAppResponse;
                                }

                            case "340000": // Authentication 

                                #region Actual Authentication
                                try
                                {
                                    var userPhoneNumber = response.CustomerMSISDN;

                                    if (response.Mpin != null)
                                    {
                                        Log.StoreMpin(userPhoneNumber, serviceProvider, response);

                                        yoAppResponse.ResponseCode = "00000";
                                        yoAppResponse.Description = "Mpin saved successfully";
                                        yoAppResponse.Note = "Authorised";

                                        Log.RequestsAndResponses("Wafaya-OTP-Submit-Response", serviceProvider, yoAppResponse);

                                        return yoAppResponse;
                                    }
                                    else if (response.Note != null)
                                    {
                                        response.Mpin = response.Note;

                                        Log.StoreMpin(userPhoneNumber, serviceProvider, response);

                                        yoAppResponse.ResponseCode = "00000";
                                        yoAppResponse.Description = "Mpin saved successfully";
                                        yoAppResponse.Note = "Authorised";

                                        Log.RequestsAndResponses("Wafaya-OTP-Submit-Response", serviceProvider, yoAppResponse);

                                        return yoAppResponse;
                                    }
                                    else
                                    {
                                        yoAppResponse.ResponseCode = "00008";
                                        yoAppResponse.Description = "Did not submit Mpin";

                                        Log.RequestsAndResponses("Wafaya-OTP-Submit-Response", serviceProvider, yoAppResponse);

                                        return yoAppResponse;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    var exceptionMessage = "Message: " + ex.Message + ", InnerException: " + ex.InnerException + ", StackTrace: " + ex.StackTrace;

                                    Log.HttpError("Exception", serviceProvider, exceptionMessage);

                                    yoAppResponse.ResponseCode = "00008";
                                    yoAppResponse.Description = "Message: " + ex.Message;

                                    Log.RequestsAndResponses("Wafaya-Exception-Response", serviceProvider, yoAppResponse);

                                    return yoAppResponse;
                                }
                            #endregion

                            case "320000":

                                #region Actual redemption
                                try
                                {
                                    WafayaFinalizeVoucherRequest wafayaFinalizeVoucherRequest = new WafayaFinalizeVoucherRequest();
                                    WafayaInitializeRedemptionRequest wafayaInitializeRedemptionRequest = new WafayaInitializeRedemptionRequest();

                                    string file3 = HttpContext.Current.Server.MapPath("~/App_Data/" + serviceProvider + "/Files/" + fileName + ".json");

                                    var tokenFile3 = LoadJson(file3);

                                    if (tokenFile3 != null) // We have generated the Token Already
                                    {
                                        var expiryDateString = tokenFile3.expires_in;

                                        var expiryDate = DateTime.ParseExact(expiryDateString, "dd/MM/yyyy-HH:mm:ss", null);

                                        var currentDateTime = DateTime.Now;

                                        if (expiryDate > currentDateTime) // Token is still valid
                                        {
                                            isTokenValid = true;
                                        }
                                        else // Token is no longer valid
                                        {
                                            RefreshToken(tokenFile3.refresh_token);
                                            isTokenValid = true;

                                            tokenFile3 = LoadJson(file3);
                                        }
                                    }
                                    else // Generate a new Token
                                    {
                                        resourceOwnerRequest.grant_type = "password";
                                        resourceOwnerRequest.client_id = wafayaCredentials.PasswordClientId;
                                        resourceOwnerRequest.client_secret = wafayaCredentials.PasswordSecret;
                                        resourceOwnerRequest.scope = "";
                                        resourceOwnerRequest.username = wafayaCredentials.Username;
                                        resourceOwnerRequest.password = wafayaCredentials.Password;

                                        Log.RequestsAndResponses("Wafaya-TokenRequest", serviceProvider, resourceOwnerRequest);

                                        var resourceOwnerResponse = connector.GetPasswordToken(resourceOwnerRequest, serviceProvider);

                                        Log.RequestsAndResponses("Wafaya-TokenResponse", serviceProvider, resourceOwnerResponse);

                                        if (resourceOwnerResponse.token_type.ToLower() == "bearer")
                                        {
                                            isTokenValid = true;
                                            yoAppResponse.ResponseCode = "00000";
                                            yoAppResponse.Description = "Token generated successfully";
                                            yoAppResponse.Note = "Transaction Successful";

                                            var expDate = DateTime.Now.AddSeconds(Convert.ToDouble(resourceOwnerResponse.expires_in));

                                            resourceOwnerResponse.expires_in = expDate.ToString("dd/MM/yyyy-HH:mm:ss");

                                            var token = JsonConvert.SerializeObject(resourceOwnerResponse);

                                            yoAppResponse.Narrative = token;

                                            Log.RequestsAndResponses("Wafaya-TokenResponse-YoApp", serviceProvider, yoAppResponse);

                                            Log.StoreData("tokens", serviceProvider, resourceOwnerResponse);

                                            tokenFile3 = LoadJson(file3);
                                        }
                                        else
                                        {
                                            yoAppResponse.ResponseCode = "00008";
                                            yoAppResponse.Description = "Code could not be generated";
                                            yoAppResponse.Note = "Transaction Failed";

                                            Log.RequestsAndResponses("Wafaya-TokenResponse-YoApp", serviceProvider, yoAppResponse);

                                            return yoAppResponse;
                                        }
                                    }

                                    if (isTokenValid)
                                    {
                                        string mPinFile = HttpContext.Current.Server.MapPath("~/App_Data/" + serviceProvider + "/Files/" + response.CustomerMSISDN + ".json");

                                        var mpinFile = LoadMpin(mPinFile, serviceProvider);

                                        if (mpinFile != null)
                                        {
                                            //var userPhoneNumber = response.CustomerMSISDN;

                                            wafayaInitializeRedemptionRequest.amount = response.Amount;
                                            wafayaInitializeRedemptionRequest.voucher = response.CustomerAccount;
                                            wafayaInitializeRedemptionRequest.token = tokenFile3.access_token;

                                            Log.RequestsAndResponses("Wafaya-InitilizeVoucherRequest", serviceProvider, wafayaInitializeRedemptionRequest);

                                            var wVoucherResponse = connector.InitializeVoucher(wafayaInitializeRedemptionRequest, serviceProvider);

                                            Log.RequestsAndResponses("Wafaya-InitilizeVoucherResponse", serviceProvider, wVoucherResponse);

                                            //var wResponse = JsonConvert.SerializeObject(wVoucherResponse);

                                            //Log.StoreVoucherInitializationResponse("i_" + userPhoneNumber, serviceProvider, wResponse);

                                            if (wVoucherResponse != null && wVoucherResponse.success.Contains("initialized")) // Voucher was successfully Initialized
                                            {
                                                wafayaFinalizeVoucherRequest.confirmation_otp = mpinFile.Mpin;
                                                wafayaFinalizeVoucherRequest.voucher = response.CustomerAccount;
                                                wafayaFinalizeVoucherRequest.token = tokenFile3.access_token;

                                                Log.RequestsAndResponses("Wafaya-FinalizeVoucherRequest", serviceProvider, wafayaFinalizeVoucherRequest);

                                                var wafayaVoucherResponse = connector.FinalizeVoucher(wafayaFinalizeVoucherRequest, serviceProvider);

                                                Log.RequestsAndResponses("Wafaya-FinalizeVoucherResponse", serviceProvider, wafayaVoucherResponse);

                                                if (wafayaVoucherResponse != null && wafayaVoucherResponse.success.Contains("finalized"))
                                                {
                                                    yoAppResponse.ResponseCode = "00000";
                                                    yoAppResponse.Description = "Voucher Finalized Successfully";

                                                    foreach (var item in wafayaVoucherResponse.payload)
                                                    {
                                                        if (item.voucher.redeemer_name != null)
                                                        {
                                                            yoAppResponse.Note = item.voucher.redeemer_name;
                                                            yoAppNarrative.ReceiversName = item.voucher.redeemer_name;
                                                            yoAppNarrative.ReceiversSurname = "";
                                                        }
                                                        else
                                                        {
                                                            yoAppResponse.Note = "";
                                                            yoAppNarrative.ReceiversName = "";
                                                            yoAppNarrative.ReceiversSurname = "";
                                                        }

                                                        if (item.voucher.voucher_code != null)
                                                        {
                                                            yoAppResponse.CustomerAccount = item.voucher.voucher_code;
                                                            yoAppNarrative.TransactionCode = item.voucher.voucher_code;
                                                        }
                                                        else
                                                        {
                                                            yoAppResponse.CustomerAccount = "";
                                                            yoAppNarrative.TransactionCode = "";
                                                        }

                                                        yoAppResponse.Amount = Math.Round(item.voucher.voucher_value, 2);

                                                        if (Convert.ToString(item.voucher.voucher_balance) != null)
                                                        {
                                                            yoAppResponse.Balance = Convert.ToString(Math.Round(item.voucher.voucher_balance, 2));
                                                            yoAppNarrative.Balance = Math.Round(item.voucher.voucher_balance, 2);
                                                        }
                                                        else
                                                        {
                                                            yoAppResponse.Balance = "";
                                                            yoAppNarrative.Balance = 0;
                                                        }

                                                        if (item.voucher.redeemer_phone != null)
                                                        {
                                                            yoAppResponse.CustomerMSISDN = item.voucher.redeemer_phone;
                                                            yoAppNarrative.ReceiverMobile = item.voucher.redeemer_phone;
                                                        }
                                                        else
                                                        {
                                                            yoAppResponse.CustomerMSISDN = "";
                                                            yoAppNarrative.ReceiverMobile = "";
                                                        }

                                                        yoAppResponse.IsActive = item.voucher.active;

                                                        if (item.voucher.voucher_currency != null)
                                                        {
                                                            yoAppResponse.Currency = item.voucher.voucher_currency;
                                                            yoAppNarrative.Currency = item.voucher.voucher_currency;
                                                        }
                                                        else
                                                        {
                                                            yoAppResponse.Currency = "";
                                                            yoAppNarrative.Currency = "";
                                                        }
                                                    }

                                                    yoAppNarrative.ReceiversIdentification = "";
                                                    yoAppNarrative.CustomerId = "Profile/8-0001-0015964";
                                                    yoAppNarrative.ServiceName = "ETENGA";
                                                    yoAppNarrative.ServiceType = "RETAIL";
                                                    yoAppNarrative.ServiceAgentId = "5-0001-0000751";
                                                    yoAppNarrative.SupplierId = "5-0001-0000751";
                                                    yoAppNarrative.SupplierName = "NRICHARDS";
                                                    yoAppNarrative.Description = "Etenga wallet";
                                                    yoAppNarrative.CustomerName = "NRICHARDS";
                                                    yoAppNarrative.CustomerMobileNumber = "263772610890";
                                                    yoAppNarrative.ServiceProvider = "5-0001-0000751";
                                                    yoAppNarrative.Quantity = 1;
                                                    yoAppNarrative.ServiceId = 1;
                                                    yoAppNarrative.ProductDetails = "1";
                                                    yoAppNarrative.SuspenseBalance = 0;

                                                    var narrative = JsonConvert.SerializeObject(yoAppNarrative);

                                                    yoAppResponse.Narrative = narrative;

                                                    Log.RequestsAndResponses("Wafaya-FinalizeVoucherResponse-YoApp", serviceProvider, yoAppResponse);

                                                    return yoAppResponse;
                                                }
                                                else
                                                {
                                                    yoAppResponse.ResponseCode = "00008";
                                                    yoAppResponse.Description = "Could not finalize voucher";
                                                    yoAppResponse.Note = "Request to the server Failed";

                                                    Log.RequestsAndResponses("Wafaya-FinalizeVoucherResponse-YoApp", serviceProvider, yoAppResponse);

                                                    return yoAppResponse;
                                                }

                                                #region Commented Out Code
                                                //yoAppResponse.ResponseCode = "00000";
                                                //yoAppResponse.Description = "Voucher Initialized Successfully";

                                                //foreach (var item in wVoucherResponse.payload)
                                                //{
                                                //    yoAppResponse.Note = item.voucher.redeemer_name;
                                                //    yoAppResponse.CustomerAccount = item.voucher.voucher_code;
                                                //    yoAppResponse.Amount = item.voucher.voucher_value;
                                                //    yoAppResponse.Balance = Convert.ToString(item.voucher.voucher_balance);
                                                //    yoAppResponse.CustomerMSISDN = item.voucher.redeemer_phone;
                                                //    yoAppResponse.IsActive = item.voucher.active;
                                                //    yoAppResponse.Currency = item.voucher.voucher_currency;
                                                //}

                                                //Log.RequestsAndResponses("Wafaya-TokenResponse-YoApp", serviceProvider, "");

                                                //return yoAppResponse;
                                                #endregion
                                            }
                                            else
                                            {
                                                yoAppResponse.ResponseCode = "00008";
                                                yoAppResponse.Description = "Failed to Initialize Voucher";
                                                yoAppResponse.Note = "Success";

                                                Log.RequestsAndResponses("Wafaya-InitializeVoucherResponse-YoApp", serviceProvider, yoAppResponse);

                                                return yoAppResponse;
                                            }
                                        }
                                        else
                                        {
                                            yoAppResponse.ResponseCode = "00008";
                                            yoAppResponse.Description = "Could not find Mpin, Please start again the verification process";

                                            Log.RequestsAndResponses("Wafaya-InitializeVoucherResponse-YoApp", serviceProvider, yoAppResponse);

                                            return yoAppResponse;
                                        }
                                    }
                                    else
                                    {
                                        yoAppResponse.ResponseCode = "00008";
                                        yoAppResponse.Description = "Token is not Valid";
                                        yoAppResponse.Note = "Request Failed";

                                        Log.RequestsAndResponses("Wafaya-TokenResponse-YoApp", serviceProvider, "");

                                        return yoAppResponse;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    var exceptionMessage = "Message: " + ex.Message + ", InnerException: " + ex.InnerException + ", StackTrace: " + ex.StackTrace;

                                    Log.HttpError("Exception", serviceProvider, exceptionMessage);

                                    yoAppResponse.ResponseCode = "00008";
                                    yoAppResponse.Description = "Message: " + ex.Message;

                                    Log.RequestsAndResponses("Wafaya-Exception-Response", serviceProvider, yoAppResponse);

                                    return yoAppResponse;
                                }
                            #endregion

                            default:
                                break;
                        }

                        break;

                    #region AuthorizationCode
                    //try
                    //{
                    //    codeRequest.client_id = wafayaCredentials.AuthorisationClientId;
                    //    codeRequest.redirect_uri = wafayaCredentials.ClientUrl;
                    //    codeRequest.response_type = "code";
                    //    codeRequest.scope = "";

                    //    Log.RequestsAndResponses("Wafaya-TokenRequest", serviceProvider, codeRequest);

                    //    var codeResponse = connector.GetCode(codeRequest, serviceProvider);

                    //    Log.RequestsAndResponses("Wafaya-TokenResponse", serviceProvider, codeRequest);

                    //    if (!String.IsNullOrEmpty(codeResponse.code)) // Token Request
                    //    {
                    //        tokenRequest.grant_type = "authorization_code";
                    //        tokenRequest.client_id = wafayaCredentials.AuthorisationClientId;
                    //        tokenRequest.client_secret = wafayaCredentials.AuthorizationSecret;
                    //        tokenRequest.redirect_uri = wafayaCredentials.ClientUrl;
                    //        tokenRequest.code = codeResponse.code;

                    //        Log.RequestsAndResponses("Wafaya-TokenResponse", serviceProvider, codeResponse.code);

                    //        var tokenResponse = connector.GetAuthorizationToken(tokenRequest, serviceProvider);

                    //        if (tokenResponse.token_type == "Bearer")
                    //        {
                    //            yoAppResponse.ResponseCode = "00000";
                    //            yoAppResponse.Description = "Token generated successfully";
                    //            yoAppResponse.Note = "Transaction Successful";

                    //            var token = JsonConvert.SerializeObject(tokenResponse);

                    //            yoAppResponse.Narrative = token;

                    //            Log.RequestsAndResponses("Wafaya-TokenResponse-YoApp", serviceProvider, tokenResponse);

                    //            return yoAppResponse;
                    //        }
                    //        else
                    //        {
                    //            yoAppResponse.ResponseCode = "00008";
                    //            yoAppResponse.Description = "Token could not be generated";
                    //            yoAppResponse.Note = "Transaction Failed";

                    //            var token = JsonConvert.SerializeObject(tokenResponse);

                    //            yoAppResponse.Narrative = token;

                    //            Log.RequestsAndResponses("Wafaya-TokenResponse-YoApp", serviceProvider, tokenResponse);

                    //            return yoAppResponse;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        yoAppResponse.ResponseCode = "00008";
                    //        yoAppResponse.Description = "Code could not be generated";
                    //        yoAppResponse.Note = "Transaction Failed";

                    //        Log.RequestsAndResponses("Wafaya-TokenResponse-YoApp", serviceProvider, codeResponse);

                    //        return yoAppResponse;
                    //    }
                    //}
                    //catch (Exception ex)
                    //{
                    //    Log.RequestsAndResponses("Exception", serviceProvider, ex.Message);
                    //    break;
                    //}
                    #endregion

                    case 2: // Resource Owner Credentials

                    #region Resource Owner Credentials
                    //string filePath = HttpContext.Current.Server.MapPath("~/App_Data/" + serviceProvider + "/Files/" + fileName + ".json");

                    //var refresherTokenFile = LoadJson(filePath);

                    //if (refresherTokenFile != null) // We have generated the Token Already
                    //{
                    //    var expiryDateString = refresherTokenFile.expires_in;

                    //    var expiryDate = DateTime.ParseExact(expiryDateString, "dd/MM/yyyy-HH:mm:ss", null);

                    //    var currentDateTime = DateTime.Now;

                    //    if (expiryDate > currentDateTime) // Token is still valid
                    //    {
                    //        yoAppResponse.ResponseCode = "00000";
                    //        yoAppResponse.Description = "Token is still valid and in use";
                    //        yoAppResponse.Note = "Request Successful";

                    //        Log.RequestsAndResponses("Wafaya-TokenResponse-YoApp", serviceProvider, "");

                    //        isTokenValid = true;

                    //        return yoAppResponse;
                    //    }
                    //    else // Token is no longer valid
                    //    {
                    //        RefreshToken();
                    //        isTokenValid = true;
                    //        return null;
                    //    }
                    //}
                    //else // Generate a new Token
                    //{
                    //    resourceOwnerRequest.grant_type = "password";
                    //    resourceOwnerRequest.client_id = wafayaCredentials.PasswordClientId;
                    //    resourceOwnerRequest.client_secret = wafayaCredentials.PasswordSecret;
                    //    resourceOwnerRequest.scope = "";
                    //    resourceOwnerRequest.username = wafayaCredentials.Username;
                    //    resourceOwnerRequest.password = wafayaCredentials.Password;

                    //    Log.RequestsAndResponses("Wafaya-TokenRequest", serviceProvider, resourceOwnerRequest);

                    //    var resourceOwnerResponse = connector.GetPasswordToken(resourceOwnerRequest, serviceProvider);

                    //    if (resourceOwnerResponse.token_type.ToLower() == "bearer")
                    //    {
                    //        yoAppResponse.ResponseCode = "00000";
                    //        yoAppResponse.Description = "Token generated successfully";
                    //        yoAppResponse.Note = "Transaction Successful";

                    //        var expDate = DateTime.Now.AddSeconds(Convert.ToDouble(resourceOwnerResponse.expires_in));

                    //        resourceOwnerResponse.expires_in = expDate.ToString("dd/MM/yyyy-HH:mm:ss");

                    //        var token = JsonConvert.SerializeObject(resourceOwnerResponse);

                    //        yoAppResponse.Narrative = token;

                    //        Log.RequestsAndResponses("Wafaya-TokenResponse-YoApp", serviceProvider, resourceOwnerResponse);
                    //        Log.StoreData("tokens", serviceProvider, resourceOwnerResponse);

                    //        return yoAppResponse;
                    //    }
                    //    else
                    //    {
                    //        yoAppResponse.ResponseCode = "00008";
                    //        yoAppResponse.Description = "Code could not be generated";
                    //        yoAppResponse.Note = "Transaction Failed";

                    //        Log.RequestsAndResponses("Wafaya-TokenResponse-YoApp", serviceProvider, resourceOwnerResponse);

                    //        return yoAppResponse;
                    //    }
                    //}
                    #endregion

                    case 3: // Retriving a single voucher

                    #region Retriving a single voucher
                    //WafayaVoucherRequest voucherRequest = new WafayaVoucherRequest();

                    //string file = HttpContext.Current.Server.MapPath("~/App_Data/" + serviceProvider + "/Files/" + fileName + ".json");
                    //var tokenFile = LoadJson(file);

                    //if (tokenFile != null) // We have generated the Token Already
                    //{
                    //    var expiryDateString = tokenFile.expires_in;

                    //    var expiryDate = DateTime.ParseExact(expiryDateString, "dd/MM/yyyy-HH:mm:ss", null);

                    //    var currentDateTime = DateTime.Now;

                    //    if (expiryDate > currentDateTime) // Token is still valid
                    //    {
                    //        isTokenValid = true;
                    //    }
                    //    else // Token is no longer valid
                    //    {
                    //        RefreshToken();
                    //        isTokenValid = true;
                    //        return null;
                    //    }
                    //}
                    //else // Generate a new Token
                    //{
                    //    resourceOwnerRequest.grant_type = "password";
                    //    resourceOwnerRequest.client_id = wafayaCredentials.PasswordClientId;
                    //    resourceOwnerRequest.client_secret = wafayaCredentials.PasswordSecret;
                    //    resourceOwnerRequest.scope = "";
                    //    resourceOwnerRequest.username = wafayaCredentials.Username;
                    //    resourceOwnerRequest.password = wafayaCredentials.Password;

                    //    Log.RequestsAndResponses("Wafaya-TokenRequest", serviceProvider, resourceOwnerRequest);

                    //    var resourceOwnerResponse = connector.GetPasswordToken(resourceOwnerRequest, serviceProvider);

                    //    if (resourceOwnerResponse.token_type.ToLower() == "bearer")
                    //    {
                    //        isTokenValid = true;
                    //        yoAppResponse.ResponseCode = "00000";
                    //        yoAppResponse.Description = "Token generated successfully";
                    //        yoAppResponse.Note = "Transaction Successful";

                    //        var expDate = DateTime.Now.AddSeconds(Convert.ToDouble(resourceOwnerResponse.expires_in));

                    //        resourceOwnerResponse.expires_in = expDate.ToString("dd/MM/yyyy-HH:mm:ss");

                    //        var token = JsonConvert.SerializeObject(resourceOwnerResponse);

                    //        yoAppResponse.Narrative = token;

                    //        Log.RequestsAndResponses("Wafaya-TokenResponse-YoApp", serviceProvider, resourceOwnerResponse);
                    //        Log.StoreData("tokens", serviceProvider, resourceOwnerResponse);

                    //        return yoAppResponse;
                    //    }
                    //    else
                    //    {
                    //        yoAppResponse.ResponseCode = "00008";
                    //        yoAppResponse.Description = "Code could not be generated";
                    //        yoAppResponse.Note = "Transaction Failed";

                    //        Log.RequestsAndResponses("Wafaya-TokenResponse-YoApp", serviceProvider, resourceOwnerResponse);

                    //        return yoAppResponse;
                    //    }
                    //}

                    //if (isTokenValid)
                    //{
                    //    voucherRequest.Voucher = response.CustomerAccount;
                    //    voucherRequest.Token = tokenFile.access_token;

                    //    Log.RequestsAndResponses("Wafaya-GetVoucherRequest", serviceProvider, "");

                    //    var wafayaVoucherResponse = connector.GetVoucherDetails(voucherRequest, serviceProvider);

                    //    Log.RequestsAndResponses("Wafaya-GetVoucherResponse", serviceProvider, wafayaVoucherResponse);

                    //    if (wafayaVoucherResponse.voucher_code != null)
                    //    {
                    //        yoAppResponse.ResponseCode = "00000";
                    //        yoAppResponse.Description = "Voucher Retrived successfully";
                    //        yoAppResponse.Note = "Request Successful";

                    //        Log.RequestsAndResponses("Wafaya-TokenResponse-YoApp", serviceProvider, wafayaVoucherResponse);

                    //        var voucherResponse = JsonConvert.SerializeObject(wafayaVoucherResponse);

                    //        yoAppResponse.Narrative = voucherResponse;

                    //        isTokenValid = true;

                    //        return yoAppResponse;
                    //    }
                    //    else
                    //    {
                    //        yoAppResponse.ResponseCode = "00008";
                    //        yoAppResponse.Description = "Could not retrieve voucher";
                    //        yoAppResponse.Note = "Request Failed";

                    //        Log.RequestsAndResponses("Wafaya-TokenResponse-YoApp", serviceProvider, wafayaVoucherResponse);

                    //        return yoAppResponse;
                    //    }
                    //}
                    //else
                    //{
                    //    yoAppResponse.ResponseCode = "00008";
                    //    yoAppResponse.Description = "Token is not Valid";
                    //    yoAppResponse.Note = "Request Failed";

                    //    Log.RequestsAndResponses("Wafaya-TokenResponse-YoApp", serviceProvider, "");

                    //    return yoAppResponse;
                    //}
                    #endregion

                    case 4: // Initializing a voucher redemption

                    #region Initializing a voucher redemption
                    //WafayaInitializeRedemptionRequest wafayaInitializeRedemptionRequest = new WafayaInitializeRedemptionRequest();

                    //string file2 = HttpContext.Current.Server.MapPath("~/App_Data/" + serviceProvider + "/Files/" + fileName + ".json");
                    //var tokenFile2 = LoadJson(file2);

                    //if (tokenFile2 != null) // We have generated the Token Already
                    //{
                    //    var expiryDateString = tokenFile2.expires_in;

                    //    var expiryDate = DateTime.ParseExact(expiryDateString, "dd/MM/yyyy-HH:mm:ss", null);

                    //    var currentDateTime = DateTime.Now;

                    //    if (expiryDate > currentDateTime) // Token is still valid
                    //    {
                    //        isTokenValid = true;
                    //    }
                    //    else // Token is no longer valid
                    //    {
                    //        RefreshToken();
                    //        isTokenValid = true;
                    //        return null;
                    //    }
                    //}
                    //else // Generate a new Token
                    //{
                    //    resourceOwnerRequest.grant_type = "password";
                    //    resourceOwnerRequest.client_id = wafayaCredentials.PasswordClientId;
                    //    resourceOwnerRequest.client_secret = wafayaCredentials.PasswordSecret;
                    //    resourceOwnerRequest.scope = "";
                    //    resourceOwnerRequest.username = wafayaCredentials.Username;
                    //    resourceOwnerRequest.password = wafayaCredentials.Password;

                    //    Log.RequestsAndResponses("Wafaya-TokenRequest", serviceProvider, resourceOwnerRequest);

                    //    var resourceOwnerResponse = connector.GetPasswordToken(resourceOwnerRequest, serviceProvider);

                    //    if (resourceOwnerResponse.token_type.ToLower() == "bearer")
                    //    {
                    //        isTokenValid = true;
                    //        yoAppResponse.ResponseCode = "00000";
                    //        yoAppResponse.Description = "Token generated successfully";
                    //        yoAppResponse.Note = "Transaction Successful";

                    //        var expDate = DateTime.Now.AddSeconds(Convert.ToDouble(resourceOwnerResponse.expires_in));

                    //        resourceOwnerResponse.expires_in = expDate.ToString("dd/MM/yyyy-HH:mm:ss");

                    //        var token = JsonConvert.SerializeObject(resourceOwnerResponse);

                    //        yoAppResponse.Narrative = token;

                    //        Log.RequestsAndResponses("Wafaya-TokenResponse-YoApp", serviceProvider, resourceOwnerResponse);
                    //        Log.StoreData("tokens", serviceProvider, resourceOwnerResponse);

                    //        return yoAppResponse;
                    //    }
                    //    else
                    //    {
                    //        yoAppResponse.ResponseCode = "00008";
                    //        yoAppResponse.Description = "Code could not be generated";
                    //        yoAppResponse.Note = "Transaction Failed";

                    //        Log.RequestsAndResponses("Wafaya-TokenResponse-YoApp", serviceProvider, resourceOwnerResponse);

                    //        return yoAppResponse;
                    //    }
                    //}

                    //if (isTokenValid)
                    //{

                    //    var narrative = JsonConvert.DeserializeObject<Narrative>(response.Narrative);

                    //    wafayaInitializeRedemptionRequest.amount = (decimal)narrative.Balance;
                    //    wafayaInitializeRedemptionRequest.voucher = response.CustomerAccount;
                    //    wafayaInitializeRedemptionRequest.token = tokenFile2.access_token;

                    //    Log.RequestsAndResponses("Wafaya-InitilizeVoucherRequest", serviceProvider, wafayaInitializeRedemptionRequest);

                    //    var wafayaVoucherResponse = connector.InitializeVoucher(wafayaInitializeRedemptionRequest, serviceProvider);

                    //    Log.RequestsAndResponses("Wafaya-InitilizeVoucherResponse", serviceProvider, wafayaVoucherResponse);

                    //    if (wafayaVoucherResponse.success.Contains("initialized"))
                    //    {
                    //        yoAppResponse.ResponseCode = "00000";
                    //        yoAppResponse.Description = "Voucher Initialized Successfully";
                    //        yoAppResponse.Note = "Request Successful";

                    //        Log.RequestsAndResponses("Wafaya-TokenResponse-YoApp", serviceProvider, wafayaVoucherResponse);

                    //        var voucherResponse = JsonConvert.SerializeObject(wafayaVoucherResponse);

                    //        yoAppResponse.Narrative = voucherResponse;

                    //        return yoAppResponse;
                    //    }
                    //    else
                    //    {
                    //        yoAppResponse.ResponseCode = "00008";
                    //        yoAppResponse.Description = "Could not initialize voucher";
                    //        yoAppResponse.Note = "Request Failed";

                    //        Log.RequestsAndResponses("Wafaya-TokenResponse-YoApp", serviceProvider, wafayaVoucherResponse);

                    //        return yoAppResponse;
                    //    }
                    //}
                    //else
                    //{
                    //    yoAppResponse.ResponseCode = "00008";
                    //    yoAppResponse.Description = "Token is not Valid";
                    //    yoAppResponse.Note = "Request Failed";

                    //    Log.RequestsAndResponses("Wafaya-TokenResponse-YoApp", serviceProvider, "");

                    //    return yoAppResponse;
                    //}
                    #endregion

                    case 5: // Finalizing a voucher redemption

                    #region Finalizing a voucher redemption
                    //WafayaFinalizeVoucherRequest wafayaFinalizeVoucherRequest = new WafayaFinalizeVoucherRequest();

                    //string file3 = HttpContext.Current.Server.MapPath("~/App_Data/" + serviceProvider + "/Files/" + fileName + ".json");
                    //var tokenFile3 = LoadJson(file3);

                    //if (tokenFile3 != null) // We have generated the Token Already
                    //{
                    //    var expiryDateString = tokenFile3.expires_in;

                    //    var expiryDate = DateTime.ParseExact(expiryDateString, "dd/MM/yyyy-HH:mm:ss", null);

                    //    var currentDateTime = DateTime.Now;

                    //    if (expiryDate > currentDateTime) // Token is still valid
                    //    {
                    //        isTokenValid = true;
                    //    }
                    //    else // Token is no longer valid
                    //    {
                    //        RefreshToken();
                    //        isTokenValid = true;
                    //        return null;
                    //    }
                    //}
                    //else // Generate a new Token
                    //{
                    //    resourceOwnerRequest.grant_type = "password";
                    //    resourceOwnerRequest.client_id = wafayaCredentials.PasswordClientId;
                    //    resourceOwnerRequest.client_secret = wafayaCredentials.PasswordSecret;
                    //    resourceOwnerRequest.scope = "";
                    //    resourceOwnerRequest.username = wafayaCredentials.Username;
                    //    resourceOwnerRequest.password = wafayaCredentials.Password;

                    //    Log.RequestsAndResponses("Wafaya-TokenRequest", serviceProvider, resourceOwnerRequest);

                    //    var resourceOwnerResponse = connector.GetPasswordToken(resourceOwnerRequest, serviceProvider);

                    //    if (resourceOwnerResponse.token_type.ToLower() == "bearer")
                    //    {
                    //        isTokenValid = true;
                    //        yoAppResponse.ResponseCode = "00000";
                    //        yoAppResponse.Description = "Token generated successfully";
                    //        yoAppResponse.Note = "Transaction Successful";

                    //        var expDate = DateTime.Now.AddSeconds(Convert.ToDouble(resourceOwnerResponse.expires_in));

                    //        resourceOwnerResponse.expires_in = expDate.ToString("dd/MM/yyyy-HH:mm:ss");

                    //        var token = JsonConvert.SerializeObject(resourceOwnerResponse);

                    //        yoAppResponse.Narrative = token;

                    //        Log.RequestsAndResponses("Wafaya-TokenResponse-YoApp", serviceProvider, resourceOwnerResponse);
                    //        Log.StoreData("tokens", serviceProvider, resourceOwnerResponse);

                    //        return yoAppResponse;
                    //    }
                    //    else
                    //    {
                    //        yoAppResponse.ResponseCode = "00008";
                    //        yoAppResponse.Description = "Code could not be generated";
                    //        yoAppResponse.Note = "Transaction Failed";

                    //        Log.RequestsAndResponses("Wafaya-TokenResponse-YoApp", serviceProvider, resourceOwnerResponse);

                    //        return yoAppResponse;
                    //    }
                    //}

                    //if (isTokenValid)
                    //{
                    //    wafayaFinalizeVoucherRequest.confirmation_otp = response.Mpin;
                    //    wafayaFinalizeVoucherRequest.voucher = response.CustomerAccount;
                    //    wafayaFinalizeVoucherRequest.token = tokenFile3.access_token;

                    //    Log.RequestsAndResponses("Wafaya-FinalizeVoucherRequest", serviceProvider, wafayaFinalizeVoucherRequest);

                    //    var wafayaVoucherResponse = connector.FinalizeVoucher(wafayaFinalizeVoucherRequest, serviceProvider);

                    //    Log.RequestsAndResponses("Wafaya-FinalizeVoucherResponse", serviceProvider, wafayaVoucherResponse);

                    //    if (wafayaVoucherResponse.success.Contains("finalized"))
                    //    {
                    //        yoAppResponse.ResponseCode = "00000";
                    //        yoAppResponse.Description = "Voucher Finalized Successfully";
                    //        yoAppResponse.Note = "Request Successful";

                    //        Log.RequestsAndResponses("Wafaya-TokenResponse-YoApp", serviceProvider, wafayaVoucherResponse);

                    //        var voucherResponse = JsonConvert.SerializeObject(wafayaVoucherResponse);

                    //        yoAppResponse.Narrative = voucherResponse;

                    //        return yoAppResponse;
                    //    }
                    //    else
                    //    {
                    //        yoAppResponse.ResponseCode = "00008";
                    //        yoAppResponse.Description = "Could not finalize voucher";
                    //        yoAppResponse.Note = "Request to the server Failed";

                    //        Log.RequestsAndResponses("Wafaya-TokenResponse-YoApp", serviceProvider, wafayaVoucherResponse);

                    //        return yoAppResponse;
                    //    }
                    //}
                    //else
                    //{
                    //    yoAppResponse.ResponseCode = "00008";
                    //    yoAppResponse.Description = "Token is not Valid";
                    //    yoAppResponse.Note = "Request Failed";

                    //    Log.RequestsAndResponses("Wafaya-TokenResponse-YoApp", serviceProvider, "");

                    //    return yoAppResponse;
                    //}
                    #endregion

                    default:
                        break;
                }
            }

            return yoAppResponse;
        }

        [Route("api/metbank/service")]
        [HttpPost]
        public YoAppResponse Service(YoAppResponse response)
        {
            #region Declared Objects
            var serviceProvider = "MerchantBank";

            YoAppResponse yoAppResponse = new YoAppResponse();
            Narrative narrative = new Narrative();
            MerchantBankConnector merchantBankConnector = new MerchantBankConnector();
            ApproveSendMoneyRequest approveSendMoneyRequest = new ApproveSendMoneyRequest();
            RegisterClientRequest registerClientRequest = new RegisterClientRequest();
            RegisterAgentRequest registerAgentRequest = new RegisterAgentRequest();
            MetBankCredentials metBankCredentials = new MetBankCredentials();
            UserLogin userLogin = new UserLogin();
            SendMoneyRequest sendMoneyRequest = new SendMoneyRequest();
            SendMoneyPreAuthRequest sendMoneyPreAuthRequest = new SendMoneyPreAuthRequest();
            ClientSendMoneyPreAuthRequest clientSendMoneyPreAuthRequest = new ClientSendMoneyPreAuthRequest();
            ClientSendMoneyRequest clientSendMoneyRequest = new ClientSendMoneyRequest();
            RecipientsRequest recipientsRequest = new RecipientsRequest();
            ReceiveMoneyRequest receiveMoneyRequest = new ReceiveMoneyRequest();

            bool isTokenValid = false;
            var fileName = "tokens";

            #endregion

            var json = JsonConvert.SerializeObject(response);

            if (response.Narrative != null)
            {
                narrative = JsonConvert.DeserializeObject<Narrative>(response.Narrative);
            }

            #region First Log
            Log.RequestsAndResponses("YoAppResponse", serviceProvider, response);
            #endregion

            if (response == null)
            {
                string message = "Received Nothing. Your request object is null";

                yoAppResponse.ResponseCode = "00008";
                yoAppResponse.Note = "Failed";
                yoAppResponse.Description = message;

                return yoAppResponse;
            }
            else
            {
                switch (response.ServiceId)
                {
                    case 1: // Clients

                        switch (response.ProcessingCode)
                        {
                            #region Commented Area
                            //case "370000": // Request for Client's Details

                            //    try
                            //    {
                            //        string file = HttpContext.Current.Server.MapPath("~/App_Data/" + serviceProvider + "/Files/" + fileName + ".json");
                            //        var tokenFile = LoadMetBankJson(file);

                            //        if (tokenFile != null) // We have generated the Token Already
                            //        {
                            //            var expiryDateString = tokenFile.expires_in;

                            //            var expiryDate = DateTime.ParseExact(expiryDateString, "dd/MM/yyyy-HH:mm:ss", null);

                            //            var currentDateTime = DateTime.Now;

                            //            if (expiryDate > currentDateTime) // Token is still valid
                            //            {
                            //                isTokenValid = true;
                            //            }
                            //            else // Token is no longer valid
                            //            {
                            //                RefreshMetBankToken(tokenFile.refresh_token);
                            //                isTokenValid = true;

                            //                tokenFile = LoadMetBankJson(file);
                            //            }
                            //        }
                            //        else // Generate a new Token
                            //        {
                            //            userLogin.password = metBankCredentials.Password;
                            //            userLogin.username = metBankCredentials.Username;

                            //            Log.RequestsAndResponses("MetBankTokenRequest", serviceProvider, userLogin);

                            //            var result = merchantBankConnector.GetToken(serviceProvider, userLogin);

                            //            Log.RequestsAndResponses("MetBankTokenResponse", serviceProvider, result);

                            //            if (result.token_type.ToLower() == "bearer")
                            //            {
                            //                isTokenValid = true;
                            //                yoAppResponse.ResponseCode = "00000";
                            //                yoAppResponse.Description = "Token generated successfully";
                            //                yoAppResponse.Note = "Transaction Successful";

                            //                var expDate = DateTime.Now.AddSeconds(Convert.ToDouble(result.expires_in));

                            //                result.expires_in = expDate.ToString("dd/MM/yyyy-HH:mm:ss");

                            //                var token = JsonConvert.SerializeObject(result);

                            //                yoAppResponse.Narrative = token;

                            //                Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);
                            //                Log.StoreData("tokens", serviceProvider, result);

                            //                tokenFile = LoadMetBankJson(file);
                            //            }
                            //            else
                            //            {
                            //                yoAppResponse.ResponseCode = "00008";
                            //                yoAppResponse.Description = "Code could not be generated";
                            //                yoAppResponse.Note = "Transaction Failed";

                            //                Log.RequestsAndResponses("Wafaya-TokenResponse-YoApp", serviceProvider, yoAppResponse);

                            //                return yoAppResponse;
                            //            }
                            //        }

                            //        if (isTokenValid)
                            //        {
                            //            if (response.CustomerAccount != null) // Get Client Details Via ClientId
                            //            {
                            //                ClientRequest clientRequest = new ClientRequest();

                            //                clientRequest.clientId = yoAppResponse.CustomerAccount;
                            //                tokenFile = LoadMetBankJson(file);

                            //                Log.RequestsAndResponses("ClientDetailsByIdRequest", serviceProvider, clientRequest);

                            //                var client = merchantBankConnector.GetClientByClientId(serviceProvider, clientRequest, tokenFile.access_token);

                            //                Log.RequestsAndResponses("ClientDetailsByIdResponse", serviceProvider, client);

                            //                if (client != null && client.deleted == false && client.firstName != null)
                            //                {
                            //                    yoAppResponse.ResponseCode = "00000";
                            //                    yoAppResponse.Description = "Client Found!";
                            //                    narrative.DateCreated = DateTime.ParseExact(client.dateCreated, "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture);
                            //                    narrative.DatelastAccess = DateTime.ParseExact(client.dateModified, "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture);
                            //                    narrative.IsActive = client.deleted;
                            //                    narrative.Status = client.status;
                            //                    narrative.Id = (long)client.id;
                            //                    narrative.ReceiversName = client.firstName;
                            //                    narrative.ReceiversSurname = client.lastName;
                            //                    yoAppResponse.Note = client.email;
                            //                    narrative.ReceiverMobile = client.phoneNumber;
                            //                    narrative.ReceiversIdentification = client.nationalId;
                            //                    narrative.ReceiversGender = client.gender;
                            //                    narrative.ServiceCountry = client.country;
                            //                    narrative.CustomerId = client.userId.ToString();
                            //                    yoAppResponse.Narrative = JsonConvert.SerializeObject(narrative);

                            //                    return yoAppResponse;
                            //                }
                            //                else
                            //                {
                            //                    yoAppResponse.ResponseCode = "00008";
                            //                    yoAppResponse.Note = "Client not Found";
                            //                    yoAppResponse.Description = "Received Nothing for the clientId submitted";

                            //                    return yoAppResponse;
                            //                }
                            //            }
                            //            else if (narrative.ReceiverMobile != null) // Get Client Details Via Phone Number
                            //            {
                            //                ClientRequest clientRequest = new ClientRequest();

                            //                clientRequest.phoneNumber = narrative.ReceiverMobile;
                            //                tokenFile = LoadMetBankJson(file);

                            //                Log.RequestsAndResponses("ClientDetailsByMobileRequest", serviceProvider, clientRequest);

                            //                var clientResponse = merchantBankConnector.GetClientByPhoneNumber(serviceProvider, clientRequest, tokenFile.access_token);

                            //                Log.RequestsAndResponses("ClientDetailsByMobileResponse", serviceProvider, clientResponse);

                            //                if (clientResponse.clientFound)
                            //                {
                            //                    yoAppResponse.ResponseCode = "00000";
                            //                    yoAppResponse.Description = "Client Found!";
                            //                    narrative.DateCreated = DateTime.ParseExact(clientResponse.client.dateCreated, "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture);
                            //                    narrative.DatelastAccess = DateTime.ParseExact(clientResponse.client.dateModified, "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture);
                            //                    narrative.IsActive = clientResponse.client.deleted;
                            //                    narrative.Status = clientResponse.client.status;
                            //                    narrative.Id = (long)clientResponse.client.id;
                            //                    narrative.ReceiversName = clientResponse.client.firstName;
                            //                    narrative.ReceiversSurname = clientResponse.client.lastName;
                            //                    yoAppResponse.Note = clientResponse.client.email;
                            //                    narrative.ReceiverMobile = clientResponse.client.phoneNumber;
                            //                    narrative.ReceiversIdentification = clientResponse.client.nationalId;
                            //                    narrative.ReceiversGender = clientResponse.client.gender;
                            //                    narrative.ServiceCountry = clientResponse.client.country;
                            //                    narrative.CustomerId = clientResponse.client.userId.ToString();
                            //                    yoAppResponse.Narrative = JsonConvert.SerializeObject(narrative);

                            //                    return yoAppResponse;
                            //                }
                            //                else
                            //                {
                            //                    yoAppResponse.ResponseCode = "00008";
                            //                    yoAppResponse.Note = "Failed";
                            //                    yoAppResponse.Description = "Received Nothing for the clientId submitted";

                            //                    return yoAppResponse;
                            //                }
                            //            }
                            //            else if (narrative.ReceiversIdentification != null) // Get Client Details Via National Id
                            //            {
                            //                ClientRequest clientRequest = new ClientRequest();

                            //                clientRequest.nationalId = narrative.ReceiversIdentification;
                            //                tokenFile = LoadMetBankJson(file);

                            //                Log.RequestsAndResponses("ClientDetailsByNationalIdRequest", serviceProvider, clientRequest);

                            //                var clientResponse = merchantBankConnector.GetClientByNationalId(serviceProvider, clientRequest, tokenFile.access_token);

                            //                Log.RequestsAndResponses("ClientDetailsByNationalIdResponse", serviceProvider, clientResponse);

                            //                if (clientResponse.clientFound)
                            //                {
                            //                    yoAppResponse.ResponseCode = "00000";
                            //                    yoAppResponse.Description = "Clinet Found!";
                            //                    narrative.DateCreated = DateTime.ParseExact(clientResponse.client.dateCreated, "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture);
                            //                    narrative.DatelastAccess = DateTime.ParseExact(clientResponse.client.dateModified, "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture);
                            //                    narrative.IsActive = clientResponse.client.deleted;
                            //                    narrative.Status = clientResponse.client.status;
                            //                    narrative.Id = (long)clientResponse.client.id;
                            //                    narrative.ReceiversName = clientResponse.client.firstName;
                            //                    narrative.ReceiversSurname = clientResponse.client.lastName;
                            //                    yoAppResponse.Note = clientResponse.client.email;
                            //                    narrative.ReceiverMobile = clientResponse.client.phoneNumber;
                            //                    narrative.ReceiversIdentification = clientResponse.client.nationalId;
                            //                    narrative.ReceiversGender = clientResponse.client.gender;
                            //                    narrative.ServiceCountry = clientResponse.client.country;
                            //                    narrative.CustomerId = clientResponse.client.userId.ToString();
                            //                    yoAppResponse.Narrative = JsonConvert.SerializeObject(narrative);

                            //                    return yoAppResponse;
                            //                }
                            //                else
                            //                {
                            //                    yoAppResponse.ResponseCode = "00008";
                            //                    yoAppResponse.Note = "Failed";
                            //                    yoAppResponse.Description = "Received Nothing for the clientId submitted";

                            //                    return yoAppResponse;
                            //                }
                            //            }
                            //            else if (narrative.Information1 != null) // Get Client Details By Page and Number of entries per page
                            //            {
                            //                ClientRequest clientRequest = new ClientRequest();

                            //                var parts = narrative.Information1.Split(new char[] { '~' }, StringSplitOptions.RemoveEmptyEntries);

                            //                clientRequest.page = parts[0];
                            //                clientRequest.size = parts[1];

                            //                tokenFile = LoadMetBankJson(file);

                            //                Log.RequestsAndResponses("ClientDetailsByPageAndSizeRequest", serviceProvider, clientRequest);

                            //                var pageClient = merchantBankConnector.GetClientsByPagesAndSize(serviceProvider, clientRequest, tokenFile.access_token);

                            //                Log.RequestsAndResponses("ClientDetailsByPageAndSizeResponse", serviceProvider, pageClient);

                            //                if (pageClient.totalPages > 0)
                            //                {
                            //                    response.CustomerData = pageClient.totalElements.ToString();
                            //                    response.CustomerAccount = pageClient.totalPages.ToString();

                            //                    var serializedContent = JsonConvert.SerializeObject(pageClient.content);

                            //                    response.Note = serializedContent;
                            //                    response.TerminalId = pageClient.number.ToString();

                            //                    var serializedSort = JsonConvert.SerializeObject(pageClient.sort);

                            //                    response.TransactionRef = serializedSort;
                            //                    response.IsActive = pageClient.first;

                            //                    var serializedPageable = JsonConvert.SerializeObject(pageClient.pageable);

                            //                    response.Product = serializedPageable;
                            //                    response.Quantity = pageClient.numberOfElements;
                            //                    narrative.IsActive = pageClient.last;
                            //                    narrative.CustomerName = pageClient.empty.ToString();
                            //                    response.Narrative = JsonConvert.SerializeObject(narrative);

                            //                    return yoAppResponse;
                            //                }
                            //                else
                            //                {
                            //                    yoAppResponse.ResponseCode = "00008";
                            //                    yoAppResponse.Note = "Client not found!";
                            //                    yoAppResponse.Description = "Received Nothing for the clientId submitted";

                            //                    return yoAppResponse;
                            //                }
                            //            }
                            //            else
                            //            {
                            //                yoAppResponse.ResponseCode = "00008";
                            //                yoAppResponse.Note = "Failed";
                            //                yoAppResponse.Description = "Received Nothing for the clientId submitted";

                            //                return yoAppResponse;
                            //            }
                            //        }
                            //        else
                            //        {
                            //            yoAppResponse.ResponseCode = "00008";
                            //            yoAppResponse.Description = "Token is not Valid";
                            //            yoAppResponse.Note = "Request Failed";

                            //            Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);

                            //            return yoAppResponse;
                            //        }
                            //    }
                            //    catch (Exception ex)
                            //    {
                            //        Log.HttpError("Exception", serviceProvider, "Message: " + ex.Message + ", InnerException: " + ex.InnerException + ", StackTrace: " + ex.StackTrace);

                            //        return yoAppResponse;
                            //    }

                            //case "1": // Edit Client

                            //    try
                            //    {
                            //        string file = HttpContext.Current.Server.MapPath("~/App_Data/" + serviceProvider + "/Files/" + fileName + ".json");
                            //        var tokenFile = LoadMetBankJson(file);

                            //        if (tokenFile != null) // We have generated the Token Already
                            //        {
                            //            var expiryDateString = tokenFile.expires_in;

                            //            var expiryDate = DateTime.ParseExact(expiryDateString, "dd/MM/yyyy-HH:mm:ss", null);

                            //            var currentDateTime = DateTime.Now;

                            //            if (expiryDate > currentDateTime) // Token is still valid
                            //            {
                            //                isTokenValid = true;
                            //            }
                            //            else // Token is no longer valid
                            //            {
                            //                RefreshMetBankToken(tokenFile.refresh_token);
                            //                isTokenValid = true;

                            //                tokenFile = LoadMetBankJson(file);
                            //            }
                            //        }
                            //        else // Generate a new Token
                            //        {
                            //            userLogin.password = metBankCredentials.Password;
                            //            userLogin.username = metBankCredentials.Username;

                            //            Log.RequestsAndResponses("MetBankTokenRequest", serviceProvider, userLogin);

                            //            var result = merchantBankConnector.GetToken(serviceProvider, userLogin);

                            //            Log.RequestsAndResponses("MetBankTokenResponse", serviceProvider, result);

                            //            if (result.token_type.ToLower() == "bearer")
                            //            {
                            //                isTokenValid = true;
                            //                yoAppResponse.ResponseCode = "00000";
                            //                yoAppResponse.Description = "Token generated successfully";
                            //                yoAppResponse.Note = "Transaction Successful";

                            //                var expDate = DateTime.Now.AddSeconds(Convert.ToDouble(result.expires_in));

                            //                result.expires_in = expDate.ToString("dd/MM/yyyy-HH:mm:ss");

                            //                var token = JsonConvert.SerializeObject(result);

                            //                yoAppResponse.Narrative = token;

                            //                Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);
                            //                Log.StoreData("tokens", serviceProvider, result);

                            //                tokenFile = LoadMetBankJson(file);
                            //            }
                            //            else
                            //            {
                            //                yoAppResponse.ResponseCode = "00008";
                            //                yoAppResponse.Description = "Code could not be generated";
                            //                yoAppResponse.Note = "Transaction Failed";

                            //                Log.RequestsAndResponses("Wafaya-TokenResponse-YoApp", serviceProvider, yoAppResponse);

                            //                return yoAppResponse;
                            //            }
                            //        }

                            //        if (isTokenValid)
                            //        {
                            //            if (response.CustomerAccount != null)
                            //            {
                            //                ClientRequest clientRequest = new ClientRequest();

                            //                clientRequest.clientId = yoAppResponse.CustomerAccount;

                            //                tokenFile = LoadMetBankJson(file);

                            //                Log.RequestsAndResponses("ClientEditRequest", serviceProvider, clientRequest);

                            //                var client = merchantBankConnector.PutClient(serviceProvider, clientRequest, tokenFile.access_token);

                            //                Log.RequestsAndResponses("ClientEditResponse", serviceProvider, client);

                            //                if (client != null)
                            //                {
                            //                    narrative.DateCreated = DateTime.ParseExact(client.dateCreated, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                            //                    narrative.DatelastAccess = DateTime.ParseExact(client.dateModified, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                            //                    narrative.IsActive = client.deleted;
                            //                    narrative.Status = client.status;
                            //                    narrative.Id = (long)client.id;
                            //                    narrative.ReceiversName = client.firstName;
                            //                    narrative.ReceiversSurname = client.lastName;
                            //                    yoAppResponse.Note = client.email;
                            //                    narrative.ReceiverMobile = client.phoneNumber;
                            //                    narrative.ReceiversIdentification = client.nationalId;
                            //                    narrative.ReceiversGender = client.gender;
                            //                    narrative.ServiceCountry = client.country;
                            //                    narrative.CustomerId = client.userId.ToString();
                            //                    yoAppResponse.Narrative = JsonConvert.SerializeObject(narrative);

                            //                    return yoAppResponse;
                            //                }
                            //                else
                            //                {
                            //                    yoAppResponse.ResponseCode = "00008";
                            //                    yoAppResponse.Note = "Failed";
                            //                    yoAppResponse.Description = "Received Nothing for the clientId submitted";

                            //                    return yoAppResponse;
                            //                }
                            //            }
                            //            else
                            //            {
                            //                yoAppResponse.ResponseCode = "00008";
                            //                yoAppResponse.Note = "Failed";
                            //                yoAppResponse.Description = "Received Nothing for the clientId submitted";

                            //                return yoAppResponse;
                            //            }
                            //        }
                            //        else
                            //        {
                            //            yoAppResponse.ResponseCode = "00008";
                            //            yoAppResponse.Description = "Token is not Valid";
                            //            yoAppResponse.Note = "Request Failed";

                            //            Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);

                            //            return yoAppResponse;
                            //        }
                            //    }
                            //    catch (Exception ex)
                            //    {
                            //        Log.HttpError("Exception", serviceProvider, "Message: " + ex.Message + ", InnerException: " + ex.InnerException + ", StackTrace: " + ex.StackTrace);

                            //        return yoAppResponse;
                            //    }

                            //case "2": // Delete Client

                            //    if (response.CustomerAccount != null)
                            //    {
                            //        Log.RequestsAndResponses("ClientDeleteRequest", serviceProvider, yoAppResponse.CustomerAccount);

                            //        ClientRequest clientRequest = new ClientRequest();

                            //        clientRequest.clientId = yoAppResponse.CustomerAccount;

                            //        var client = merchantBankConnector.DeleteClient(clientRequest);

                            //        Log.RequestsAndResponses("ClientDeleteResponse", serviceProvider, client);

                            //        if (client != null)
                            //        {
                            //            narrative.DateCreated = DateTime.ParseExact(client.dateCreated, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                            //            narrative.DatelastAccess = DateTime.ParseExact(client.dateModified, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                            //            narrative.IsActive = client.deleted;
                            //            narrative.Status = client.status;
                            //            narrative.Id = (long)client.id;
                            //            narrative.ReceiversName = client.firstName;
                            //            narrative.ReceiversSurname = client.lastName;
                            //            yoAppResponse.Note = client.email;
                            //            narrative.ReceiverMobile = client.phoneNumber;
                            //            narrative.ReceiversIdentification = client.nationalId;
                            //            narrative.ReceiversGender = client.gender;
                            //            narrative.ServiceCountry = client.country;
                            //            narrative.CustomerId = client.userId.ToString();
                            //            yoAppResponse.Narrative = JsonConvert.SerializeObject(narrative);

                            //            return yoAppResponse;
                            //        }
                            //        else
                            //        {
                            //            yoAppResponse.ResponseCode = "00008";
                            //            yoAppResponse.Note = "Failed";
                            //            yoAppResponse.Description = "Received Nothing for the clientId submitted";

                            //            return yoAppResponse;
                            //        }
                            //    }
                            //    else
                            //    {
                            //        yoAppResponse.ResponseCode = "00008";
                            //        yoAppResponse.Note = "Failed";
                            //        yoAppResponse.Description = "Received Nothing for the clientId submitted";

                            //        return yoAppResponse;
                            //    }

                            ////case "360000": // Client Registration                                

                            //case "410000": // Client Login Request

                            //    if (response.MTI != null && response.MTI == "0100")
                            //    {
                            //        Log.RequestsAndResponses("ClientLoginRequest", serviceProvider, yoAppResponse.CustomerAccount);

                            //        ClientRequest clientRequest = new ClientRequest();

                            //        clientRequest.clientId = yoAppResponse.CustomerAccount;

                            //        var client = merchantBankConnector.GetClientByClientId(serviceProvider, clientRequest, "");

                            //        Log.RequestsAndResponses("ClientLoginResponse", serviceProvider, client);

                            //        if (client != null)
                            //        {
                            //            narrative.DateCreated = DateTime.ParseExact(client.dateCreated, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                            //            narrative.DatelastAccess = DateTime.ParseExact(client.dateModified, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                            //            narrative.IsActive = client.deleted;
                            //            narrative.Status = client.status;
                            //            narrative.Id = (long)client.id;
                            //            narrative.ReceiversName = client.firstName;
                            //            narrative.ReceiversSurname = client.lastName;
                            //            yoAppResponse.Note = client.email;
                            //            narrative.ReceiverMobile = client.phoneNumber;
                            //            narrative.ReceiversIdentification = client.nationalId;
                            //            narrative.ReceiversGender = client.gender;
                            //            narrative.ServiceCountry = client.country;
                            //            narrative.CustomerId = client.userId.ToString();
                            //            yoAppResponse.Narrative = JsonConvert.SerializeObject(narrative);

                            //            return yoAppResponse;
                            //        }
                            //        else
                            //        {
                            //            yoAppResponse.ResponseCode = "00008";
                            //            yoAppResponse.Note = "Failed";
                            //            yoAppResponse.Description = "Received Nothing for the clientId submitted";

                            //            return yoAppResponse;
                            //        }
                            //    }
                            //    else
                            //    {
                            //        yoAppResponse.ResponseCode = "00008";
                            //        yoAppResponse.Note = "Failed";
                            //        yoAppResponse.Description = "Received Nothing for the clientId submitted";

                            //        return yoAppResponse;
                            //    }

                            //case "500000": // Approve Client

                            //    try
                            //    {
                            //        string file = HttpContext.Current.Server.MapPath("~/App_Data/" + serviceProvider + "/Files/" + fileName + ".json");
                            //        var tokenFile = LoadMetBankJson(file);

                            //        if (tokenFile != null) // We have generated the Token Already
                            //        {
                            //            var expiryDateString = tokenFile.expires_in;

                            //            var expiryDate = DateTime.ParseExact(expiryDateString, "dd/MM/yyyy-HH:mm:ss", null);

                            //            var currentDateTime = DateTime.Now;

                            //            if (expiryDate > currentDateTime) // Token is still valid
                            //            {
                            //                isTokenValid = true;
                            //            }
                            //            else // Token is no longer valid
                            //            {
                            //                RefreshMetBankToken(tokenFile.refresh_token);
                            //                isTokenValid = true;

                            //                tokenFile = LoadMetBankJson(file);
                            //            }
                            //        }
                            //        else // Generate a new Token
                            //        {
                            //            userLogin.password = metBankCredentials.Password;
                            //            userLogin.username = metBankCredentials.Username;

                            //            Log.RequestsAndResponses("MetBankTokenRequest", serviceProvider, userLogin);

                            //            var result = merchantBankConnector.GetToken(serviceProvider, userLogin);

                            //            Log.RequestsAndResponses("MetBankTokenResponse", serviceProvider, result);

                            //            if (result.token_type.ToLower() == "bearer")
                            //            {
                            //                isTokenValid = true;
                            //                yoAppResponse.ResponseCode = "00000";
                            //                yoAppResponse.Description = "Token generated successfully";
                            //                yoAppResponse.Note = "Transaction Successful";

                            //                var expDate = DateTime.Now.AddSeconds(Convert.ToDouble(result.expires_in));

                            //                result.expires_in = expDate.ToString("dd/MM/yyyy-HH:mm:ss");

                            //                var token = JsonConvert.SerializeObject(result);

                            //                yoAppResponse.Narrative = token;

                            //                Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);
                            //                Log.StoreData("tokens", serviceProvider, result);

                            //                tokenFile = LoadMetBankJson(file);
                            //            }
                            //            else
                            //            {
                            //                yoAppResponse.ResponseCode = "00008";
                            //                yoAppResponse.Description = "Code could not be generated";
                            //                yoAppResponse.Note = "Transaction Failed";

                            //                Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);

                            //                return yoAppResponse;
                            //            }
                            //        }

                            //        if (isTokenValid)
                            //        {
                            //            ClientRequest clientRequest = new ClientRequest();

                            //            clientRequest.clientId = yoAppResponse.CustomerAccount;
                            //            tokenFile = LoadMetBankJson(file);

                            //            Log.RequestsAndResponses("ApproveClientRequest", serviceProvider, clientRequest);

                            //            var client = merchantBankConnector.ApproveClient(serviceProvider, clientRequest, tokenFile.access_token);

                            //            Log.RequestsAndResponses("ApproveClientResponse", serviceProvider, client);

                            //            if (client != null && client.deleted == false && client.firstName != null)
                            //            {
                            //                yoAppResponse.ResponseCode = "00000";
                            //                yoAppResponse.Description = "Client Found!";
                            //                narrative.DateCreated = DateTime.ParseExact(client.dateCreated, "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture);
                            //                narrative.DatelastAccess = DateTime.ParseExact(client.dateModified, "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture);
                            //                narrative.IsActive = client.deleted;
                            //                narrative.Status = client.status;
                            //                narrative.Id = (long)client.id;
                            //                narrative.ReceiversName = client.firstName;
                            //                narrative.ReceiversSurname = client.lastName;
                            //                yoAppResponse.Note = client.email;
                            //                narrative.ReceiverMobile = client.phoneNumber;
                            //                narrative.ReceiversIdentification = client.nationalId;
                            //                narrative.ReceiversGender = client.gender;
                            //                narrative.ServiceCountry = client.country;
                            //                narrative.CustomerId = client.userId.ToString();
                            //                yoAppResponse.Narrative = JsonConvert.SerializeObject(narrative);

                            //                return yoAppResponse;
                            //            }
                            //            else
                            //            {
                            //                yoAppResponse.ResponseCode = "00008";
                            //                yoAppResponse.Note = "Client not Found";
                            //                yoAppResponse.Description = "Received Nothing for the clientId submitted";

                            //                return yoAppResponse;
                            //            }
                            //        }
                            //        else
                            //        {
                            //            yoAppResponse.ResponseCode = "00008";
                            //            yoAppResponse.Description = "Token is not Valid";
                            //            yoAppResponse.Note = "Request Failed";

                            //            Log.RequestsAndResponses("ApproveClientResponse-YoApp", serviceProvider, yoAppResponse);

                            //            return yoAppResponse;
                            //        }
                            //    }
                            //    catch (Exception ex)
                            //    {
                            //        Log.HttpError("Exception", serviceProvider, "Message: " + ex.Message + ", InnerException: " + ex.InnerException + ", StackTrace: " + ex.StackTrace);

                            //        return yoAppResponse;
                            //    }

                            //case "510000": // Disapprove Client

                            //    try
                            //    {
                            //        string file = HttpContext.Current.Server.MapPath("~/App_Data/" + serviceProvider + "/Files/" + fileName + ".json");
                            //        var tokenFile = LoadMetBankJson(file);

                            //        if (tokenFile != null) // We have generated the Token Already
                            //        {
                            //            var expiryDateString = tokenFile.expires_in;

                            //            var expiryDate = DateTime.ParseExact(expiryDateString, "dd/MM/yyyy-HH:mm:ss", null);

                            //            var currentDateTime = DateTime.Now;

                            //            if (expiryDate > currentDateTime) // Token is still valid
                            //            {
                            //                isTokenValid = true;
                            //            }
                            //            else // Token is no longer valid
                            //            {
                            //                RefreshMetBankToken(tokenFile.refresh_token);
                            //                isTokenValid = true;

                            //                tokenFile = LoadMetBankJson(file);
                            //            }
                            //        }
                            //        else // Generate a new Token
                            //        {
                            //            userLogin.password = metBankCredentials.Password;
                            //            userLogin.username = metBankCredentials.Username;

                            //            Log.RequestsAndResponses("MetBankTokenRequest", serviceProvider, userLogin);

                            //            var result = merchantBankConnector.GetToken(serviceProvider, userLogin);

                            //            Log.RequestsAndResponses("MetBankTokenResponse", serviceProvider, result);

                            //            if (result.token_type.ToLower() == "bearer")
                            //            {
                            //                isTokenValid = true;
                            //                yoAppResponse.ResponseCode = "00000";
                            //                yoAppResponse.Description = "Token generated successfully";
                            //                yoAppResponse.Note = "Transaction Successful";

                            //                var expDate = DateTime.Now.AddSeconds(Convert.ToDouble(result.expires_in));

                            //                result.expires_in = expDate.ToString("dd/MM/yyyy-HH:mm:ss");

                            //                var token = JsonConvert.SerializeObject(result);

                            //                yoAppResponse.Narrative = token;

                            //                Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);
                            //                Log.StoreData("tokens", serviceProvider, result);

                            //                tokenFile = LoadMetBankJson(file);
                            //            }
                            //            else
                            //            {
                            //                yoAppResponse.ResponseCode = "00008";
                            //                yoAppResponse.Description = "Code could not be generated";
                            //                yoAppResponse.Note = "Transaction Failed";

                            //                Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);

                            //                return yoAppResponse;
                            //            }
                            //        }

                            //        if (isTokenValid)
                            //        {
                            //            ClientRequest clientRequest = new ClientRequest();

                            //            clientRequest.clientId = yoAppResponse.CustomerAccount;
                            //            tokenFile = LoadMetBankJson(file);

                            //            Log.RequestsAndResponses("DisApproveClientRequest", serviceProvider, clientRequest);

                            //            var client = merchantBankConnector.DisApproveClient(serviceProvider, clientRequest, tokenFile.access_token);

                            //            Log.RequestsAndResponses("DisApproveClientResponse", serviceProvider, client);

                            //            if (client != null && client.deleted == false && client.firstName != null)
                            //            {
                            //                yoAppResponse.ResponseCode = "00000";
                            //                yoAppResponse.Description = "Client Found!";
                            //                narrative.DateCreated = DateTime.ParseExact(client.dateCreated, "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture);
                            //                narrative.DatelastAccess = DateTime.ParseExact(client.dateModified, "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture);
                            //                narrative.IsActive = client.deleted;
                            //                narrative.Status = client.status;
                            //                narrative.Id = (long)client.id;
                            //                narrative.ReceiversName = client.firstName;
                            //                narrative.ReceiversSurname = client.lastName;
                            //                yoAppResponse.Note = client.email;
                            //                narrative.ReceiverMobile = client.phoneNumber;
                            //                narrative.ReceiversIdentification = client.nationalId;
                            //                narrative.ReceiversGender = client.gender;
                            //                narrative.ServiceCountry = client.country;
                            //                narrative.CustomerId = client.userId.ToString();
                            //                yoAppResponse.Narrative = JsonConvert.SerializeObject(narrative);

                            //                return yoAppResponse;
                            //            }
                            //            else
                            //            {
                            //                yoAppResponse.ResponseCode = "00008";
                            //                yoAppResponse.Note = "Client not Found";
                            //                yoAppResponse.Description = "Received Nothing for the clientId submitted";

                            //                return yoAppResponse;
                            //            }
                            //        }
                            //        else
                            //        {
                            //            yoAppResponse.ResponseCode = "00008";
                            //            yoAppResponse.Description = "Token is not Valid";
                            //            yoAppResponse.Note = "Request Failed";

                            //            Log.RequestsAndResponses("DisApproveClientResponse-YoApp", serviceProvider, yoAppResponse);

                            //            return yoAppResponse;
                            //        }
                            //    }
                            //    catch (Exception ex)
                            //    {
                            //        Log.HttpError("Exception", serviceProvider, "Message: " + ex.Message + ", InnerException: " + ex.InnerException + ", StackTrace: " + ex.StackTrace);

                            //        return yoAppResponse;
                            //    }

                            //case "520000": // Number of Registered Clients

                            //    try
                            //    {
                            //        string file = HttpContext.Current.Server.MapPath("~/App_Data/" + serviceProvider + "/Files/" + fileName + ".json");
                            //        var tokenFile = LoadMetBankJson(file);

                            //        if (tokenFile != null) // We have generated the Token Already
                            //        {
                            //            var expiryDateString = tokenFile.expires_in;

                            //            var expiryDate = DateTime.ParseExact(expiryDateString, "dd/MM/yyyy-HH:mm:ss", null);

                            //            var currentDateTime = DateTime.Now;

                            //            if (expiryDate > currentDateTime) // Token is still valid
                            //            {
                            //                isTokenValid = true;
                            //            }
                            //            else // Token is no longer valid
                            //            {
                            //                RefreshMetBankToken(tokenFile.refresh_token);
                            //                isTokenValid = true;

                            //                tokenFile = LoadMetBankJson(file);
                            //            }
                            //        }
                            //        else // Generate a new Token
                            //        {
                            //            userLogin.password = metBankCredentials.Password;
                            //            userLogin.username = metBankCredentials.Username;

                            //            Log.RequestsAndResponses("MetBankTokenRequest", serviceProvider, userLogin);

                            //            var result = merchantBankConnector.GetToken(serviceProvider, userLogin);

                            //            Log.RequestsAndResponses("MetBankTokenResponse", serviceProvider, result);

                            //            if (result.token_type.ToLower() == "bearer")
                            //            {
                            //                isTokenValid = true;
                            //                yoAppResponse.ResponseCode = "00000";
                            //                yoAppResponse.Description = "Token generated successfully";
                            //                yoAppResponse.Note = "Transaction Successful";

                            //                var expDate = DateTime.Now.AddSeconds(Convert.ToDouble(result.expires_in));

                            //                result.expires_in = expDate.ToString("dd/MM/yyyy-HH:mm:ss");

                            //                var token = JsonConvert.SerializeObject(result);

                            //                yoAppResponse.Narrative = token;

                            //                Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);
                            //                Log.StoreData("tokens", serviceProvider, result);

                            //                tokenFile = LoadMetBankJson(file);
                            //            }
                            //            else
                            //            {
                            //                yoAppResponse.ResponseCode = "00008";
                            //                yoAppResponse.Description = "Code could not be generated";
                            //                yoAppResponse.Note = "Transaction Failed";

                            //                Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);

                            //                return yoAppResponse;
                            //            }
                            //        }

                            //        if (isTokenValid)
                            //        {
                            //            ClientRequest clientRequest = new ClientRequest();

                            //            tokenFile = LoadMetBankJson(file);

                            //            Log.RequestsAndResponses("GetNumberOfRegisteredClientsRequest", serviceProvider, clientRequest);

                            //            var registrationCountResponse = merchantBankConnector.GetNumberOfRegisteredClients(serviceProvider, clientRequest, tokenFile.access_token);

                            //            Log.RequestsAndResponses("GetNumberOfRegisteredClientsResponse", serviceProvider, registrationCountResponse);

                            //            if (registrationCountResponse.count > 0)
                            //            {
                            //                yoAppResponse.ResponseCode = "00000";
                            //                yoAppResponse.Description = "There are " + registrationCountResponse.count + " registered clients";
                            //                yoAppResponse.Note = "Successful";
                            //                yoAppResponse.Balance = registrationCountResponse.count.ToString();

                            //                Log.RequestsAndResponses("GetNumberOfRegisteredClientsResponse-YoApp", serviceProvider, yoAppResponse);

                            //                return yoAppResponse;
                            //            }
                            //            else
                            //            {
                            //                yoAppResponse.ResponseCode = "00008";
                            //                yoAppResponse.Description = "There are no registered clients in the system";
                            //                yoAppResponse.Note = "Failed";

                            //                Log.RequestsAndResponses("GetNumberOfRegisteredClientsResponse-YoApp", serviceProvider, yoAppResponse);

                            //                return yoAppResponse;
                            //            }
                            //        }
                            //        else
                            //        {
                            //            yoAppResponse.ResponseCode = "00008";
                            //            yoAppResponse.Description = "Token is not Valid";
                            //            yoAppResponse.Note = "Request Failed";

                            //            Log.RequestsAndResponses("GetNumberOfRegisteredClientsResponse-YoApp", serviceProvider, yoAppResponse);

                            //            return yoAppResponse;
                            //        }
                            //    }
                            //    catch (Exception ex)
                            //    {
                            //        Log.HttpError("Exception", serviceProvider, "Message: " + ex.Message + ", InnerException: " + ex.InnerException + ", StackTrace: " + ex.StackTrace);

                            //        return yoAppResponse;
                            //    }

                            //case "320000": // Send Money

                            //    #region Send Money Section

                            //    try
                            //    {
                            //        string file = HttpContext.Current.Server.MapPath("~/App_Data/" + serviceProvider + "/Files/" + fileName + ".json");
                            //        var tokenFile = LoadMetBankJson(file);

                            //        if (tokenFile != null) // We have generated the Token Already
                            //        {
                            //            var expiryDateString = tokenFile.expires_in;

                            //            var expiryDate = DateTime.ParseExact(expiryDateString, "dd/MM/yyyy-HH:mm:ss", null);

                            //            var currentDateTime = DateTime.Now;

                            //            if (expiryDate > currentDateTime) // Token is still valid
                            //            {
                            //                isTokenValid = true;
                            //            }
                            //            else // Token is no longer valid
                            //            {
                            //                //RefreshMetBankToken(tokenFile.refresh_token);

                            //                GenerateMetBankToken();
                            //                isTokenValid = true;

                            //                //tokenFile = LoadMetBankJson(file);

                            //                //tokenFile = metBankCredentials.AccessToken;
                            //            }
                            //        }
                            //        else // Generate a new Token
                            //        {
                            //            userLogin.password = metBankCredentials.Password;
                            //            userLogin.username = metBankCredentials.Username;
                            //            userLogin.clientSecret = metBankCredentials.ClientSecret;
                            //            userLogin.clientId = metBankCredentials.ClientId;

                            //            Log.RequestsAndResponses("MetBankTokenRequest", serviceProvider, userLogin);

                            //            var result = merchantBankConnector.GetToken(serviceProvider, userLogin);

                            //            Log.RequestsAndResponses("MetBankTokenResponse", serviceProvider, result);

                            //            if (result.token_type.ToLower() == "bearer")
                            //            {
                            //                isTokenValid = true;
                            //                yoAppResponse.ResponseCode = "00000";
                            //                yoAppResponse.Description = "Token generated successfully";
                            //                yoAppResponse.Note = "Transaction Successful";

                            //                var expDate = DateTime.Now.AddSeconds(Convert.ToDouble(result.expires_in));

                            //                result.expires_in = expDate.ToString("dd/MM/yyyy-HH:mm:ss");

                            //                var token = JsonConvert.SerializeObject(result);

                            //                yoAppResponse.Narrative = token;

                            //                Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);
                            //                Log.StoreData("tokens", serviceProvider, result);

                            //                tokenFile = LoadMetBankJson(file);
                            //            }
                            //            else
                            //            {
                            //                yoAppResponse.ResponseCode = "00008";
                            //                yoAppResponse.Description = "Code could not be generated";
                            //                yoAppResponse.Note = "Transaction Failed";

                            //                Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);

                            //                return yoAppResponse;
                            //            }
                            //        }

                            //        if (isTokenValid)
                            //        {
                            //            tokenFile = LoadMetBankJson(file);

                            //            // Send Money PreAuth Transaction
                            //            sendMoneyPreAuthRequest.amountSend = (decimal)narrative.Balance;
                            //            sendMoneyPreAuthRequest.sourceCountryCode = "ZW";
                            //            sendMoneyPreAuthRequest.destinationCountryCode = narrative.ServiceCountry.Trim();
                            //            sendMoneyPreAuthRequest.currencyCodeSend = narrative.Currency.Trim();
                            //            sendMoneyPreAuthRequest.clientId = Convert.ToInt32(narrative.ProviderAccountNumber.Trim());
                            //            sendMoneyPreAuthRequest.agentId = (int)tokenFile.agentId;
                            //            sendMoneyPreAuthRequest.collectionAmount = (decimal)narrative.Balance;
                            //            sendMoneyPreAuthRequest.collectionCurrencyCode = narrative.Currency.Trim();

                            //            if (narrative.Currency.ToUpper().Trim() == "ZWL")
                            //            {
                            //                sendMoneyPreAuthRequest.currencyCodeSend = "USD";
                            //                sendMoneyPreAuthRequest.collectionCurrencyCode = "USD";
                            //            }

                            //            sendMoneyPreAuthRequest.recipientId = Convert.ToInt32(narrative.ReceiverProviderAccountNumber.Trim());
                            //            sendMoneyPreAuthRequest.tellerId = 154;
                            //            sendMoneyPreAuthRequest.reasonForTransfer = narrative.Information1.Trim();
                            //            tokenFile = LoadMetBankJson(file);

                            //            Log.RequestsAndResponses("PreAuthSendMoneyRequest", serviceProvider, sendMoneyPreAuthRequest);

                            //            var sendMoneyPreAuthResponse = merchantBankConnector.PreAuthSendMoney(serviceProvider, sendMoneyPreAuthRequest, tokenFile.access_token);

                            //            Log.RequestsAndResponses("PreAuthSendMoneyResponse", serviceProvider, sendMoneyPreAuthResponse);

                            //            if (!string.IsNullOrEmpty(sendMoneyPreAuthResponse.preauthId))
                            //            {
                            //                sendMoneyRequest.amountSend = sendMoneyPreAuthRequest.amountSend;
                            //                sendMoneyRequest.sourceCountryCode = sendMoneyPreAuthRequest.sourceCountryCode;
                            //                sendMoneyRequest.destinationCountryCode = sendMoneyPreAuthRequest.destinationCountryCode;
                            //                sendMoneyRequest.currencyCodeSend = sendMoneyPreAuthRequest.currencyCodeSend;
                            //                sendMoneyRequest.clientId = sendMoneyPreAuthRequest.clientId;
                            //                sendMoneyRequest.agentId = sendMoneyPreAuthRequest.agentId;
                            //                sendMoneyRequest.collectionAmount = sendMoneyPreAuthRequest.collectionAmount;
                            //                sendMoneyRequest.collectionCurrencyCode = sendMoneyPreAuthRequest.collectionCurrencyCode;
                            //                sendMoneyRequest.recipientId = sendMoneyPreAuthRequest.recipientId;
                            //                sendMoneyRequest.reasonForTransfer = sendMoneyPreAuthRequest.reasonForTransfer;
                            //                sendMoneyRequest.tellerId = sendMoneyPreAuthRequest.tellerId;
                            //                sendMoneyRequest.preauthId = sendMoneyPreAuthResponse.preauthId;
                            //                tokenFile = LoadMetBankJson(file);

                            //                Log.RequestsAndResponses("SendMoneyRequest", serviceProvider, sendMoneyRequest);

                            //                var transactionResponse = merchantBankConnector.SendMoney(serviceProvider, sendMoneyRequest, tokenFile.access_token);

                            //                Log.RequestsAndResponses("SendMoneyResponse", serviceProvider, transactionResponse);

                            //                if (transactionResponse.status != null)
                            //                {
                            //                    if (transactionResponse.status.ToUpper() == "COMPLETE")
                            //                    {
                            //                        yoAppResponse.ResponseCode = "00000";
                            //                        yoAppResponse.Note = transactionResponse.transactionId.ToString();
                            //                        yoAppResponse.Description = transactionResponse.description;
                            //                        yoAppResponse.TransactionRef = transactionResponse.transactionReference;
                            //                        yoAppResponse.Amount = decimal.Parse(transactionResponse.amount.Trim(), System.Globalization.CultureInfo.InvariantCulture);
                            //                        yoAppResponse.Balance = transactionResponse.fees;
                            //                        yoAppResponse.Note = transactionResponse.status;
                            //                        yoAppResponse.Currency = transactionResponse.collectionCurrencyCode;
                            //                        yoAppResponse.Narrative = JsonConvert.SerializeObject(narrative);

                            //                        Log.RequestsAndResponses("YoAppSendMoneyResponse", serviceProvider, yoAppResponse);

                            //                        return yoAppResponse;
                            //                    }
                            //                    else
                            //                    {
                            //                        yoAppResponse.ResponseCode = "00008";
                            //                        yoAppResponse.Note = "Failed";
                            //                        yoAppResponse.Description = "Money was not sent! The status was not 'COMPLETE'";

                            //                        Log.RequestsAndResponses("YoAppSendMoneyResponse", serviceProvider, yoAppResponse);

                            //                        return yoAppResponse;
                            //                    }
                            //                }
                            //                else
                            //                {
                            //                    yoAppResponse.ResponseCode = "00008";
                            //                    yoAppResponse.Note = "Failed";
                            //                    yoAppResponse.Description = "Money was not sent! there was an error from Metbank Server. Check with Metbank";

                            //                    Log.RequestsAndResponses("YoAppSendMoneyResponse", serviceProvider, yoAppResponse);

                            //                    return yoAppResponse;
                            //                }                                            
                            //            }
                            //        }
                            //        else
                            //        {
                            //            yoAppResponse.ResponseCode = "00008";
                            //            yoAppResponse.Description = "Token is not Valid";
                            //            yoAppResponse.Note = "Request Failed";

                            //            Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);

                            //            return yoAppResponse;
                            //        }
                            //    }
                            //    catch (Exception ex)
                            //    {
                            //        Log.HttpError("Exception", serviceProvider, "Message: " + ex.Message + ", InnerException: " + ex.InnerException + ", StackTrace: " + ex.StackTrace);

                            //        return yoAppResponse;
                            //    }

                            //    #endregion

                            //    break;

                            #endregion                    

                            default: // Search Client or Recipient

                                #region Search for Client or Recipient
                                
                                try
                                {
                                    string file = HttpContext.Current.Server.MapPath("~/App_Data/" + serviceProvider + "/Files/" + fileName + ".json");
                                    
                                    var tokenFile = LoadMetBankJson(file);

                                    if (tokenFile != null) // We have generated the Token Already
                                    {
                                        var expiryDateString = tokenFile.expires_in;

                                        var expiryDate = DateTime.ParseExact(expiryDateString, "dd/MM/yyyy-HH:mm:ss", null);

                                        var currentDateTime = DateTime.Now;

                                        if (expiryDate > currentDateTime) // Token is still valid
                                        {
                                            isTokenValid = true;
                                        }
                                        else // Token is no longer valid
                                        {
                                            // RefreshMetBankToken(tokenFile.refresh_token);
                                            GenerateMetBankToken();
                                            isTokenValid = true;

                                            //tokenFile = LoadMetBankJson(file);

                                            //tokenFile = metBankCredentials.AccessToken;
                                        }
                                    }
                                    else // Generate a new Token
                                    {
                                        userLogin.password = metBankCredentials.Password;
                                        userLogin.username = metBankCredentials.Username;
                                        userLogin.clientSecret = metBankCredentials.ClientSecret;
                                        userLogin.clientId = metBankCredentials.ClientId;

                                        Log.RequestsAndResponses("MetBankTokenRequest", serviceProvider, userLogin);

                                        var result = merchantBankConnector.GetToken(serviceProvider, userLogin);

                                        Log.RequestsAndResponses("MetBankTokenResponse", serviceProvider, result);

                                        if (result.token_type.ToLower() == "bearer")
                                        {
                                            isTokenValid = true;
                                            yoAppResponse.ResponseCode = "00000";
                                            yoAppResponse.Description = "Token generated successfully";
                                            yoAppResponse.Note = "Transaction Successful";

                                            var expDate = DateTime.Now.AddSeconds(Convert.ToDouble(result.expires_in));

                                            result.expires_in = expDate.ToString("dd/MM/yyyy-HH:mm:ss");

                                            var token = JsonConvert.SerializeObject(result);

                                            yoAppResponse.Narrative = token;

                                            Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);
                                            Log.StoreData("tokens", serviceProvider, result);

                                            tokenFile = LoadMetBankJson(file);
                                        }
                                        else
                                        {
                                            yoAppResponse.ResponseCode = "00008";
                                            yoAppResponse.Description = "Code could not be generated";
                                            yoAppResponse.Note = "Transaction Failed";

                                            Log.RequestsAndResponses("Wafaya-TokenResponse-YoApp", serviceProvider, yoAppResponse);

                                            return yoAppResponse;
                                        }
                                    }

                                    if (isTokenValid)
                                    {
                                        if (response.CustomerAccount != null) // Get Client Details Via ClientId or CustomerMobileNumber
                                        {
                                            if (response.Note.ToUpper() == "PROVIDERACCOUNTNUMBER")
                                            {
                                                ClientRequest clientRequest = new ClientRequest();

                                                clientRequest.clientId = response.CustomerAccount;
                                                tokenFile = LoadMetBankJson(file);

                                                Log.RequestsAndResponses("ClientDetailsByIdRequest", serviceProvider, clientRequest);

                                                var client = merchantBankConnector.GetClientByClientId(serviceProvider, clientRequest, tokenFile.access_token); //metBankCredentials.AccessToken

                                                Log.RequestsAndResponses("ClientDetailsByIdResponse", serviceProvider, client);

                                                if (client != null && client.deleted == false && client.firstName != null)
                                                {
                                                    List<Narrative> narratives = new List<Narrative>();

                                                    yoAppResponse.ResponseCode = "00000";
                                                    yoAppResponse.Description = "Client Found!";
                                                    narrative.DateCreated = DateTime.ParseExact(client.dateCreated, "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture);
                                                    narrative.DatelastAccess = DateTime.ParseExact(client.dateModified, "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture);
                                                    //narrative.IsActive = client.deleted;

                                                    if (client.deleted)
                                                    {
                                                        narrative.IsActive = false;
                                                    }
                                                    else
                                                    {
                                                        narrative.IsActive = true;
                                                    }

                                                    narrative.Status = client.status;
                                                    narrative.ProviderAccountNumber = client.id.ToString();
                                                    narrative.CustomerName = client.firstName + " " + client.lastName;
                                                    narrative.ServiceRegion = "{Email: " + client.email + ", National Id Number: " + client.nationalId + ", Gender: " +
                                                        client.gender + ",UserId: " + client.userId.ToString() + ",Country: " + client.country + ",Status:" + client.status +
                                                        ",IsDeleted:" + client.deleted;
                                                    narrative.CustomerMobileNumber = client.phoneNumber;

                                                    narratives.Add(narrative);

                                                    yoAppResponse.Narrative = JsonConvert.SerializeObject(narratives);

                                                    Log.RequestsAndResponses("YoAppClientDetailsByIdResponse", serviceProvider, yoAppResponse);

                                                    //var jzon = JsonConvert.SerializeObject(yoAppResponse);

                                                    return yoAppResponse;
                                                }
                                                else
                                                {
                                                    yoAppResponse.ResponseCode = "00008";
                                                    yoAppResponse.Note = "Client not Found";
                                                    yoAppResponse.Description = "Received Nothing for the clientId submitted was wrong";

                                                    Log.RequestsAndResponses("YoAppClientDetailsByIdResponse", serviceProvider, yoAppResponse);

                                                    return yoAppResponse;
                                                }
                                            }
                                            else if (response.Note.ToUpper() == "CUSTOMERMOBILENUMBER") // Client Phone Number
                                            {
                                                ClientRequest clientRequest = new ClientRequest();

                                                clientRequest.phoneNumber = response.CustomerAccount;
                                                tokenFile = LoadMetBankJson(file);

                                                Log.RequestsAndResponses("ClientDetailsByMobileRequest", serviceProvider, clientRequest);

                                                var clientResponse = merchantBankConnector.GetClientByPhoneNumber(serviceProvider, clientRequest, tokenFile.access_token); // metBankCredentials.AccessToken

                                                Log.RequestsAndResponses("ClientDetailsByMobileResponse", serviceProvider, clientResponse);

                                                if (clientResponse.clientFound)
                                                {
                                                    List<Narrative> narratives = new List<Narrative>();

                                                    yoAppResponse.ResponseCode = "00000";
                                                    yoAppResponse.Description = "Client Found!";
                                                    narrative.DateCreated = DateTime.ParseExact(clientResponse.client.dateCreated, "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture);
                                                    narrative.DatelastAccess = DateTime.ParseExact(clientResponse.client.dateModified, "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture);
                                                    //narrative.IsActive = clientResponse.client.deleted;

                                                    if (clientResponse.client.deleted)
                                                    {
                                                        narrative.IsActive = false;
                                                    }
                                                    else
                                                    {
                                                        narrative.IsActive = true;
                                                    }

                                                    narrative.CustomerName = clientResponse.client.firstName + " " + clientResponse.client.lastName;
                                                    narrative.ProviderAccountNumber = clientResponse.client.id.ToString();
                                                    narrative.CustomerMobileNumber = clientResponse.client.phoneNumber;

                                                    narrative.ServiceRegion = "{Status: " + clientResponse.client.status + ", National Id Number: " + clientResponse.client.nationalId + ", Gender: " +
                                                        clientResponse.client.gender + ",UserId: " + clientResponse.client.userId.ToString() + ",Country: " + clientResponse.client.country + ",Email:" + clientResponse.client.email;

                                                    //narrative.Id = (long)clientResponse.client.id;
                                                    //narrative.ReceiversName = clientResponse.client.firstName;
                                                    //narrative.ReceiversSurname = clientResponse.client.lastName;
                                                    //yoAppResponse.Note = clientResponse.client.email;

                                                    //narrative.ReceiversIdentification = clientResponse.client.nationalId;
                                                    //narrative.ReceiversGender = clientResponse.client.gender;
                                                    //narrative.ServiceCountry = clientResponse.client.country;
                                                    //narrative.CustomerId = clientResponse.client.userId.ToString();

                                                    narratives.Add(narrative);
                                                    yoAppResponse.Narrative = JsonConvert.SerializeObject(narratives);

                                                    Log.RequestsAndResponses("YoAppClientDetailsByMobileResponse", serviceProvider, yoAppResponse);

                                                    return yoAppResponse;
                                                }
                                                else
                                                {
                                                    yoAppResponse.ResponseCode = "00008";
                                                    yoAppResponse.Note = "Failed";
                                                    yoAppResponse.Description = "Received Nothing for the Molile Number submitted was wrong";

                                                    Log.RequestsAndResponses("YoAppClientDetailsByMobileResponse", serviceProvider, yoAppResponse);

                                                    return yoAppResponse;
                                                }
                                            }
                                            else if (response.Note.ToUpper() == "RECEIVERSIDNUMBER")
                                            {
                                                ClientRequest clientRequest = new ClientRequest();

                                                clientRequest.nationalId = narrative.ReceiversIdentification;
                                                tokenFile = LoadMetBankJson(file);

                                                Log.RequestsAndResponses("ClientDetailsByNationalIdRequest", serviceProvider, clientRequest);

                                                var clientResponse = merchantBankConnector.GetClientByNationalId(serviceProvider, clientRequest, metBankCredentials.AccessToken);

                                                Log.RequestsAndResponses("ClientDetailsByNationalIdResponse", serviceProvider, clientResponse);

                                                if (clientResponse.clientFound)
                                                {
                                                    List<Narrative> narratives = new List<Narrative>();

                                                    yoAppResponse.ResponseCode = "00000";
                                                    yoAppResponse.Description = "Clinet Found!";
                                                    narrative.DateCreated = DateTime.ParseExact(clientResponse.client.dateCreated, "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture);
                                                    narrative.DatelastAccess = DateTime.ParseExact(clientResponse.client.dateModified, "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture);
                                                    narrative.IsActive = clientResponse.client.deleted;
                                                    narrative.Status = clientResponse.client.status;
                                                    narrative.Id = (long)clientResponse.client.id;
                                                    narrative.ReceiversName = clientResponse.client.firstName;
                                                    narrative.ReceiversSurname = clientResponse.client.lastName;
                                                    yoAppResponse.Note = clientResponse.client.email;
                                                    narrative.ReceiverMobile = clientResponse.client.phoneNumber;
                                                    narrative.ReceiversIdentification = clientResponse.client.nationalId;
                                                    narrative.ReceiversGender = clientResponse.client.gender;
                                                    narrative.ServiceCountry = clientResponse.client.country;
                                                    narrative.CustomerId = clientResponse.client.userId.ToString();

                                                    narratives.Add(narrative);
                                                    yoAppResponse.Narrative = JsonConvert.SerializeObject(narratives);

                                                    Log.RequestsAndResponses("YoAppClientDetailsByNationalIdResponse", serviceProvider, yoAppResponse);

                                                    return yoAppResponse;
                                                }
                                                else
                                                {
                                                    yoAppResponse.ResponseCode = "00008";
                                                    yoAppResponse.Note = "Failed";
                                                    yoAppResponse.Description = "Received Nothing for the National Id submitted was wrong";

                                                    Log.RequestsAndResponses("YoAppClientDetailsByNationalIdResponse", serviceProvider, yoAppResponse);

                                                    return yoAppResponse;
                                                }
                                            }
                                            else if (response.Note.ToUpper() == "INFORMATION1")
                                            {
                                                ClientRequest clientRequest = new ClientRequest();

                                                var parts = narrative.Information1.Split(new char[] { '~' }, StringSplitOptions.RemoveEmptyEntries);

                                                clientRequest.page = parts[0];
                                                clientRequest.size = parts[1];

                                                tokenFile = LoadMetBankJson(file);

                                                Log.RequestsAndResponses("ClientDetailsByPageAndSizeRequest", serviceProvider, clientRequest);

                                                var pageClient = merchantBankConnector.GetClientsByPagesAndSize(serviceProvider, clientRequest, tokenFile.access_token);

                                                Log.RequestsAndResponses("ClientDetailsByPageAndSizeResponse", serviceProvider, pageClient);

                                                if (pageClient.totalPages > 0)
                                                {
                                                    response.CustomerData = pageClient.totalElements.ToString();
                                                    response.CustomerAccount = pageClient.totalPages.ToString();

                                                    var serializedContent = JsonConvert.SerializeObject(pageClient.content);

                                                    response.Note = serializedContent;
                                                    response.TerminalId = pageClient.number.ToString();

                                                    var serializedSort = JsonConvert.SerializeObject(pageClient.sort);

                                                    response.TransactionRef = serializedSort;
                                                    response.IsActive = pageClient.first;

                                                    var serializedPageable = JsonConvert.SerializeObject(pageClient.pageable);

                                                    response.Product = serializedPageable;
                                                    response.Quantity = pageClient.numberOfElements;
                                                    narrative.IsActive = pageClient.last;
                                                    narrative.CustomerName = pageClient.empty.ToString();
                                                    response.Narrative = JsonConvert.SerializeObject(narrative);

                                                    Log.RequestsAndResponses("YoAppClientDetailsByPageAndSizeResponse", serviceProvider, yoAppResponse);

                                                    return yoAppResponse;
                                                }
                                                else
                                                {
                                                    yoAppResponse.ResponseCode = "00008";
                                                    yoAppResponse.Note = "Clients not found!";
                                                    yoAppResponse.Description = "There are no pages to be displayed";

                                                    Log.RequestsAndResponses("YoAppClientDetailsByPageAndSizeResponse", serviceProvider, yoAppResponse);

                                                    return yoAppResponse;
                                                }
                                            }

                                            #region Search Recipients by ClientId
                                            else if (response.Note.ToUpper() == "RECEIVERPROVIDERACCOUNTNUMBER")
                                            {
                                                ClientRequest clientRequest = new ClientRequest();

                                                clientRequest.clientId = response.CustomerAccount;
                                                tokenFile = LoadMetBankJson(file);

                                                Log.RequestsAndResponses("RecipientDetailsByClientIdRequest", serviceProvider, clientRequest);

                                                var recipients = merchantBankConnector.GetRecipientByClientId(serviceProvider, clientRequest, tokenFile.access_token); // metBankCredentials.AccessToken

                                                Log.RequestsAndResponses("RecipientDetailsByClientIdResponse", serviceProvider, recipients);

                                                List<Narrative> narratives = new List<Narrative>();

                                                foreach (var recipient in recipients)
                                                {
                                                    Narrative narrative1 = new Narrative();

                                                    if (recipient.firstName != null)
                                                    {
                                                        yoAppResponse.ResponseCode = "00000";
                                                        yoAppResponse.Description = "Recipients Found!";
                                                        narrative1.DateCreated = DateTime.ParseExact(recipient.dateCreated, "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture);
                                                        narrative1.DatelastAccess = DateTime.ParseExact(recipient.dateModified, "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture);
                                                        narrative1.IsActive = recipient.deleted;

                                                        if (recipient.deleted)
                                                        {
                                                            narrative1.IsActive = false;
                                                        }
                                                        else
                                                        {
                                                            narrative1.IsActive = true;
                                                        }

                                                        narrative1.Status = recipient.version.ToString();
                                                        narrative1.ReceiverProviderAccountNumber = recipient.id.ToString();
                                                        narrative1.ReceiversName = recipient.firstName;
                                                        narrative1.ReceiversSurname = recipient.lastName;
                                                        //narrative.ReceiversSurname = recipient.lastName;                                                    
                                                        narrative1.ReceiverMobile = recipient.phoneNumber;

                                                        narrative1.Information1 = "{Relationship:" + recipient.relationship + ",NationalId:" + recipient.nationalId + ",Gender:" + recipient.gender +
                                                            ",ClientId: " + recipient.clientId + "}";
                                                        //yoAppResponse.Note = recipient.relationship;
                                                        //narrative.ReceiversIdentification = recipient.nationalId;
                                                        //narrative.ReceiversGender = recipient.gender;
                                                        //narrative.ServiceCountry = recipient.countryId;
                                                        //narrative.Information1 = recipient.countryName;
                                                        //narrative.Information2 = recipient.address;
                                                        //narrative.CustomerId = recipient.clientId.ToString();

                                                        narratives.Add(narrative1);                                                       
                                                    }
                                                    else
                                                    {
                                                        yoAppResponse.ResponseCode = "00008";
                                                        yoAppResponse.Note = "Recipients not Found";
                                                        yoAppResponse.Description = "Received Nothing for the clientId submitted";

                                                        Log.RequestsAndResponses("RecipientDetailsByIdResponse-YoApp", serviceProvider, yoAppResponse);

                                                        return yoAppResponse;
                                                    }
                                                }

                                                yoAppResponse.Narrative = JsonConvert.SerializeObject(narratives);

                                                Log.RequestsAndResponses("RecipientDetailsByIdResponse-YoApp", serviceProvider, yoAppResponse);

                                                return yoAppResponse;
                                            }
                                            #endregion

                                            #region RecipientId
                                            //else if (response.Note.ToUpper() == "RECEIVERPROVIDERACCOUNTNUMBER")
                                            //{
                                            //    ClientRequest clientRequest = new ClientRequest();

                                            //    clientRequest.clientId = response.CustomerAccount;
                                            //    tokenFile = LoadMetBankJson(file);

                                            //    Log.RequestsAndResponses("RecipientDetailsByIdRequest", serviceProvider, clientRequest);

                                            //    var recipient = merchantBankConnector.GetRecipientById(serviceProvider, clientRequest, tokenFile.access_token); // metBankCredentials.AccessToken

                                            //    Log.RequestsAndResponses("RecipientDetailsByIdResponse", serviceProvider, recipient);

                                            //    if (recipient != null && recipient.deleted == false && recipient.firstName != null)
                                            //    {
                                            //        List<Narrative> narratives = new List<Narrative>();

                                            //        yoAppResponse.ResponseCode = "00000";
                                            //        yoAppResponse.Description = "Recipient Found!";
                                            //        narrative.DateCreated = DateTime.ParseExact(recipient.dateCreated, "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture);
                                            //        narrative.DatelastAccess = DateTime.ParseExact(recipient.dateModified, "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture);
                                            //        narrative.IsActive = recipient.deleted;

                                            //        if (recipient.deleted)
                                            //        {
                                            //            narrative.IsActive = false;
                                            //        }
                                            //        else
                                            //        {
                                            //            narrative.IsActive = true;
                                            //        }

                                            //        narrative.Status = recipient.version.ToString();
                                            //        narrative.ReceiverProviderAccountNumber = recipient.id.ToString();
                                            //        narrative.ReceiversName = recipient.firstName;
                                            //        narrative.ReceiversSurname = recipient.lastName;
                                            //        //narrative.ReceiversSurname = recipient.lastName;                                                    
                                            //        narrative.ReceiverMobile = recipient.phoneNumber;

                                            //        narrative.Information1 = "{Relationship:" + recipient.relationship + ",NationalId:" + recipient.nationalId + ",Gender:" + recipient.gender +
                                            //            ",ClientId: " + recipient.clientId + "}";
                                            //        //yoAppResponse.Note = recipient.relationship;
                                            //        //narrative.ReceiversIdentification = recipient.nationalId;
                                            //        //narrative.ReceiversGender = recipient.gender;
                                            //        //narrative.ServiceCountry = recipient.countryId;
                                            //        //narrative.Information1 = recipient.countryName;
                                            //        //narrative.Information2 = recipient.address;
                                            //        //narrative.CustomerId = recipient.clientId.ToString();

                                            //        narratives.Add(narrative);
                                            //        yoAppResponse.Narrative = JsonConvert.SerializeObject(narratives);

                                            //        Log.RequestsAndResponses("RecipientDetailsByIdResponse-YoApp", serviceProvider, yoAppResponse);

                                            //        return yoAppResponse;
                                            //    }
                                            //    else
                                            //    {
                                            //        yoAppResponse.ResponseCode = "00008";
                                            //        yoAppResponse.Note = "Recipient not Found";
                                            //        yoAppResponse.Description = "Received Nothing for the clientId submitted";

                                            //        Log.RequestsAndResponses("RecipientDetailsByIdResponse-YoApp", serviceProvider, yoAppResponse);

                                            //        return yoAppResponse;
                                            //    }
                                            //}
                                            #endregion

                                            else if (response.Note.ToUpper() == "RECEIVERMOBILE")
                                            {
                                                ClientRequest clientRequest = new ClientRequest();

                                                clientRequest.phoneNumber = response.CustomerAccount;
                                                tokenFile = LoadMetBankJson(file);

                                                Log.RequestsAndResponses("RecipientDetailsByPhoneNumberRequest", serviceProvider, clientRequest);

                                                var recipient = merchantBankConnector.GetRecipientByPhoneNumber(serviceProvider, clientRequest, tokenFile.access_token); // metBankCredentials.AccessToken

                                                Log.RequestsAndResponses("RecipientDetailsByPhoneNumberResponse", serviceProvider, recipient);

                                                if (recipient != null && recipient.deleted == false && recipient.firstName != null)
                                                {
                                                    List<Narrative> narratives = new List<Narrative>();

                                                    yoAppResponse.ResponseCode = "00000";
                                                    yoAppResponse.Description = "Recipient Found!";
                                                    narrative.DateCreated = DateTime.ParseExact(recipient.dateCreated, "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture);
                                                    narrative.DatelastAccess = DateTime.ParseExact(recipient.dateModified, "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture);
                                                    narrative.IsActive = recipient.deleted;

                                                    if (recipient.deleted)
                                                    {
                                                        narrative.IsActive = false;
                                                    }
                                                    else
                                                    {
                                                        narrative.IsActive = true;
                                                    }

                                                    narrative.Status = recipient.version.ToString();
                                                    narrative.ReceiverProviderAccountNumber = recipient.id.ToString();
                                                    narrative.ReceiversName = recipient.firstName;
                                                    narrative.ReceiversSurname = recipient.lastName;
                                                    //narrative.ReceiversSurname = recipient.lastName;                                                    
                                                    narrative.ReceiverMobile = recipient.phoneNumber;

                                                    narrative.Information1 = "{Relationship:" + recipient.relationship + ",NationalId:" + recipient.nationalId + ",Gender:" + recipient.gender +
                                                        ",ClientId: " + recipient.clientId + "}";
                                                    //yoAppResponse.Note = recipient.relationship;
                                                    //narrative.ReceiversIdentification = recipient.nationalId;
                                                    //narrative.ReceiversGender = recipient.gender;
                                                    //narrative.ServiceCountry = recipient.countryId;
                                                    //narrative.Information1 = recipient.countryName;
                                                    //narrative.Information2 = recipient.address;
                                                    //narrative.CustomerId = recipient.clientId.ToString();

                                                    narratives.Add(narrative);
                                                    yoAppResponse.Narrative = JsonConvert.SerializeObject(narratives);

                                                    Log.RequestsAndResponses("YoAppRecipientDetailsByPhoneNumberResponse", serviceProvider, yoAppResponse);

                                                    return yoAppResponse;
                                                }
                                                else
                                                {
                                                    yoAppResponse.ResponseCode = "00008";
                                                    yoAppResponse.Note = "Recipient not Found";
                                                    yoAppResponse.Description = "Received Nothing for the clientId submitted";

                                                    Log.RequestsAndResponses("YoAppRecipientDetailsByPhoneNumberResponse", serviceProvider, yoAppResponse);

                                                    return yoAppResponse;
                                                }
                                            }
                                            else
                                            {
                                                yoAppResponse.ResponseCode = "00008";
                                                yoAppResponse.Note = "Failed";
                                                yoAppResponse.Description = "Received Nothing for the Page Number was not submitted";

                                                Log.RequestsAndResponses("YoAppRecipientDetailsByPhoneNumberResponse", serviceProvider, yoAppResponse);

                                                return yoAppResponse;
                                            }

                                        }
                                        else
                                        {
                                            yoAppResponse.ResponseCode = "00008";
                                            yoAppResponse.Description = "CustomerAccount is null";
                                            yoAppResponse.Note = "Request Failed";

                                            Log.RequestsAndResponses("YoAppResponse-YoApp", serviceProvider, yoAppResponse);

                                            return yoAppResponse;
                                        }
                                    }
                                    else
                                    {
                                        yoAppResponse.ResponseCode = "00008";
                                        yoAppResponse.Description = "Token is not Valid";
                                        yoAppResponse.Note = "Request Failed";

                                        Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);

                                        return yoAppResponse;
                                    }

                                    //return yoAppResponse;
                                }
                                catch (Exception ex)
                                {
                                    Log.HttpError("Exception", serviceProvider, "Message: " + ex.Message + ", InnerException: " + ex.InnerException + ", StackTrace: " + ex.StackTrace);

                                    return yoAppResponse;
                                }

                                #endregion
                        }

                        //break;

                    case 2: // Agents

                        #region Agents

                        switch (response.ProcessingCode)
                        {
                            case "360000": // Agents Registration

                                try
                                {
                                    string file = HttpContext.Current.Server.MapPath("~/App_Data/" + serviceProvider + "/Files/" + fileName + ".json");
                                    var tokenFile = LoadMetBankJson(file);

                                    if (tokenFile != null) // We have generated the Token Already
                                    {
                                        var expiryDateString = tokenFile.expires_in;

                                        var expiryDate = DateTime.ParseExact(expiryDateString, "dd/MM/yyyy-HH:mm:ss", null);

                                        var currentDateTime = DateTime.Now;

                                        if (expiryDate > currentDateTime) // Token is still valid
                                        {
                                            isTokenValid = true;
                                        }
                                        else // Token is no longer valid
                                        {
                                            RefreshMetBankToken(tokenFile.refresh_token);
                                            isTokenValid = true;

                                            tokenFile = LoadMetBankJson(file);
                                        }
                                    }
                                    else // Generate a new Token
                                    {
                                        userLogin.password = metBankCredentials.Password;
                                        userLogin.username = metBankCredentials.Username;

                                        Log.RequestsAndResponses("MetBankTokenRequest", serviceProvider, userLogin);

                                        var result = merchantBankConnector.GetToken(serviceProvider, userLogin);

                                        Log.RequestsAndResponses("MetBankTokenResponse", serviceProvider, result);

                                        if (result.token_type.ToLower() == "bearer")
                                        {
                                            isTokenValid = true;
                                            yoAppResponse.ResponseCode = "00000";
                                            yoAppResponse.Description = "Token generated successfully";
                                            yoAppResponse.Note = "Transaction Successful";

                                            var expDate = DateTime.Now.AddSeconds(Convert.ToDouble(result.expires_in));

                                            result.expires_in = expDate.ToString("dd/MM/yyyy-HH:mm:ss");

                                            var token = JsonConvert.SerializeObject(result);

                                            yoAppResponse.Narrative = token;

                                            Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);
                                            Log.StoreData("tokens", serviceProvider, result);

                                            tokenFile = LoadMetBankJson(file);
                                        }
                                        else
                                        {
                                            yoAppResponse.ResponseCode = "00008";
                                            yoAppResponse.Description = "Code could not be generated";
                                            yoAppResponse.Note = "Transaction Failed";

                                            Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);

                                            return yoAppResponse;
                                        }
                                    }

                                    if (isTokenValid)
                                    {
                                        var parts = response.CustomerData.Split(new char[] { '~' }, StringSplitOptions.RemoveEmptyEntries);

                                        if (parts.Length == 4)
                                        {
                                            registerAgentRequest.address = new Address();

                                            registerAgentRequest.name = narrative.CustomerName;
                                            registerAgentRequest.address.street = parts[0];
                                            registerAgentRequest.address.suburb = parts[1];
                                            registerAgentRequest.address.city = parts[2];
                                            registerAgentRequest.address.country = parts[3];

                                            if (!string.IsNullOrEmpty(narrative.Information1))
                                            {
                                                registerAgentRequest.contactPerson = JsonConvert.DeserializeObject<List<ContactPerson>>(narrative.Information1);
                                            }

                                            registerAgentRequest.username = narrative.Information2;
                                            registerAgentRequest.userFirstName = narrative.ReceiversName;
                                            registerAgentRequest.userLastName = narrative.ReceiversSurname;
                                            registerAgentRequest.userEmail = response.Note;
                                            registerAgentRequest.bpnNumber = response.CustomerAccount;
                                            registerAgentRequest.applicationForm = response.Description;
                                            tokenFile = LoadMetBankJson(file);

                                            Log.RequestsAndResponses("AgentsRegistrationRequest", serviceProvider, registerAgentRequest);

                                            var agent = merchantBankConnector.RegisterAgent(serviceProvider, registerAgentRequest, tokenFile.access_token);

                                            Log.RequestsAndResponses("AgentRegistrationResponse", serviceProvider, agent);

                                            if (agent != null)
                                            {
                                                narrative.DateCreated = DateTime.ParseExact(agent.dateCreated, "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture);
                                                narrative.DatelastAccess = DateTime.ParseExact(agent.dateModified, "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture);
                                                narrative.ProviderAccountNumber = agent.version;
                                                narrative.IsActive = agent.active;
                                                narrative.Id = agent.id;
                                                narrative.CustomerName = agent.name;
                                                yoAppResponse.Note = agent.address.street + "," + agent.address.suburb + "," + agent.address.city
                                                                        + agent.address.country;
                                                yoAppResponse.CustomerData = agent.deleted.ToString();
                                                yoAppResponse.CustomerAccount = agent.bpnNumber;
                                                yoAppResponse.Description = agent.applicationForm;
                                                yoAppResponse.TransactionRef = agent.accountNumber;
                                                yoAppResponse.Narrative = JsonConvert.SerializeObject(narrative);

                                                Log.RequestsAndResponses("YoApp-AgentRegistrationResponse", serviceProvider, agent);

                                                return yoAppResponse;
                                            }
                                            else
                                            {
                                                yoAppResponse.ResponseCode = "00008";
                                                yoAppResponse.Note = "Failed";
                                                yoAppResponse.Description = "Received Nothing for the agent created";

                                                Log.RequestsAndResponses("YoApp-AgentRegistrationResponse", serviceProvider, agent);

                                                return yoAppResponse;
                                            }
                                        }
                                        else
                                        {
                                            yoAppResponse.ResponseCode = "00008";
                                            yoAppResponse.Note = "Failed";
                                            yoAppResponse.Description = "Received Nothing for the clientId submitted";

                                            Log.RequestsAndResponses("YoApp-AgentRegistrationResponse", serviceProvider, yoAppResponse);

                                            return yoAppResponse;
                                        }
                                    }
                                    else
                                    {
                                        yoAppResponse.ResponseCode = "00008";
                                        yoAppResponse.Description = "Token is not Valid";
                                        yoAppResponse.Note = "Request Failed";

                                        Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);

                                        return yoAppResponse;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Log.HttpError("Exception", serviceProvider, "Message: " + ex.Message + ", InnerException: " + ex.InnerException + ", StackTrace: " + ex.StackTrace);

                                    return yoAppResponse;
                                }

                            default:
                                break;
                        }

                        #endregion

                        break;

                    case 3: // Transactions

                        #region Transactions

                        switch (response.ProcessingCode)
                        {
                            case "350000": // Approve Send Money

                                try
                                {
                                    string file = HttpContext.Current.Server.MapPath("~/App_Data/" + serviceProvider + "/Files/" + fileName + ".json");
                                    var tokenFile = LoadMetBankJson(file);

                                    if (tokenFile != null) // We have generated the Token Already
                                    {
                                        var expiryDateString = tokenFile.expires_in;

                                        var expiryDate = DateTime.ParseExact(expiryDateString, "dd/MM/yyyy-HH:mm:ss", null);

                                        var currentDateTime = DateTime.Now;

                                        if (expiryDate > currentDateTime) // Token is still valid
                                        {
                                            isTokenValid = true;
                                        }
                                        else // Token is no longer valid
                                        {
                                            RefreshMetBankToken(tokenFile.refresh_token);
                                            isTokenValid = true;

                                            tokenFile = LoadMetBankJson(file);
                                        }
                                    }
                                    else // Generate a new Token
                                    {
                                        userLogin.password = metBankCredentials.Password;
                                        userLogin.username = metBankCredentials.Username;

                                        Log.RequestsAndResponses("MetBankTokenRequest", serviceProvider, userLogin);

                                        var result = merchantBankConnector.GetToken(serviceProvider, userLogin);

                                        Log.RequestsAndResponses("MetBankTokenResponse", serviceProvider, result);

                                        if (result.token_type.ToLower() == "bearer")
                                        {
                                            isTokenValid = true;
                                            yoAppResponse.ResponseCode = "00000";
                                            yoAppResponse.Description = "Token generated successfully";
                                            yoAppResponse.Note = "Transaction Successful";

                                            var expDate = DateTime.Now.AddSeconds(Convert.ToDouble(result.expires_in));

                                            result.expires_in = expDate.ToString("dd/MM/yyyy-HH:mm:ss");

                                            var token = JsonConvert.SerializeObject(result);

                                            yoAppResponse.Narrative = token;

                                            Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);
                                            Log.StoreData("tokens", serviceProvider, result);

                                            tokenFile = LoadMetBankJson(file);
                                        }
                                        else
                                        {
                                            yoAppResponse.ResponseCode = "00008";
                                            yoAppResponse.Description = "Code could not be generated";
                                            yoAppResponse.Note = "Transaction Failed";

                                            Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);

                                            return yoAppResponse;
                                        }
                                    }

                                    if (isTokenValid)
                                    {
                                        if (response != null)
                                        {
                                            if (response != null)
                                            {
                                                approveSendMoneyRequest.reference = Convert.ToInt32(response.TransactionRef);
                                                approveSendMoneyRequest.tellerId = Convert.ToInt32(narrative.SupplierId);
                                                approveSendMoneyRequest.agentId = Convert.ToInt32(narrative.ServiceAgentId);
                                                approveSendMoneyRequest.preauthId = narrative.Id.ToString();
                                                tokenFile = LoadMetBankJson(file);

                                                Log.RequestsAndResponses("ApproveSendMoneyRequest", serviceProvider, approveSendMoneyRequest);

                                                var transactionResponse = merchantBankConnector.ApproveSendMoney(serviceProvider, approveSendMoneyRequest, tokenFile.access_token);

                                                Log.RequestsAndResponses("ApproveSendMoneyResponse", serviceProvider, transactionResponse);

                                                if (transactionResponse != null)
                                                {
                                                    yoAppResponse.Note = transactionResponse.transactionId.ToString();
                                                    yoAppResponse.Description = transactionResponse.description;
                                                    yoAppResponse.TransactionRef = transactionResponse.transactionReference;
                                                    yoAppResponse.Amount = decimal.Parse(transactionResponse.amount.Trim(), System.Globalization.CultureInfo.InvariantCulture);
                                                    yoAppResponse.Balance = transactionResponse.fees;
                                                    yoAppResponse.Note = transactionResponse.status;
                                                    yoAppResponse.Currency = transactionResponse.collectionCurrencyCode;
                                                    yoAppResponse.Narrative = JsonConvert.SerializeObject(narrative);

                                                    Log.RequestsAndResponses("YoApp-ApproveSendMoneyResponse", serviceProvider, transactionResponse);

                                                    return yoAppResponse;
                                                }
                                                else
                                                {
                                                    yoAppResponse.ResponseCode = "00008";
                                                    yoAppResponse.Note = "Failed";
                                                    yoAppResponse.Description = "Received Nothing for the agent created";

                                                    Log.RequestsAndResponses("YoApp-ApproveSendMoneyResponse", serviceProvider, transactionResponse);

                                                    return yoAppResponse;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        yoAppResponse.ResponseCode = "00008";
                                        yoAppResponse.Description = "Token is not Valid";
                                        yoAppResponse.Note = "Request Failed";

                                        Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);

                                        return yoAppResponse;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Log.HttpError("Exception", serviceProvider, "Message: " + ex.Message + ", InnerException: " + ex.InnerException + ", StackTrace: " + ex.StackTrace);

                                    return yoAppResponse;
                                }
                                break;

                            case "360000": // Send Money

                                try
                                {
                                    string file = HttpContext.Current.Server.MapPath("~/App_Data/" + serviceProvider + "/Files/" + fileName + ".json");
                                    var tokenFile = LoadMetBankJson(file);

                                    if (tokenFile != null) // We have generated the Token Already
                                    {
                                        var expiryDateString = tokenFile.expires_in;

                                        var expiryDate = DateTime.ParseExact(expiryDateString, "dd/MM/yyyy-HH:mm:ss", null);

                                        var currentDateTime = DateTime.Now;

                                        if (expiryDate > currentDateTime) // Token is still valid
                                        {
                                            isTokenValid = true;
                                        }
                                        else // Token is no longer valid
                                        {
                                            RefreshMetBankToken(tokenFile.refresh_token);
                                            isTokenValid = true;

                                            tokenFile = LoadMetBankJson(file);
                                        }
                                    }
                                    else // Generate a new Token
                                    {
                                        userLogin.password = metBankCredentials.Password;
                                        userLogin.username = metBankCredentials.Username;

                                        Log.RequestsAndResponses("MetBankTokenRequest", serviceProvider, userLogin);

                                        var result = merchantBankConnector.GetToken(serviceProvider, userLogin);

                                        Log.RequestsAndResponses("MetBankTokenResponse", serviceProvider, result);

                                        if (result.token_type.ToLower() == "bearer")
                                        {
                                            isTokenValid = true;
                                            yoAppResponse.ResponseCode = "00000";
                                            yoAppResponse.Description = "Token generated successfully";
                                            yoAppResponse.Note = "Transaction Successful";

                                            var expDate = DateTime.Now.AddSeconds(Convert.ToDouble(result.expires_in));

                                            result.expires_in = expDate.ToString("dd/MM/yyyy-HH:mm:ss");

                                            var token = JsonConvert.SerializeObject(result);

                                            yoAppResponse.Narrative = token;

                                            Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);
                                            Log.StoreData("tokens", serviceProvider, result);

                                            tokenFile = LoadMetBankJson(file);
                                        }
                                        else
                                        {
                                            yoAppResponse.ResponseCode = "00008";
                                            yoAppResponse.Description = "Code could not be generated";
                                            yoAppResponse.Note = "Transaction Failed";

                                            Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);

                                            return yoAppResponse;
                                        }
                                    }

                                    if (isTokenValid)
                                    {
                                        // Send Money PreAuth Transaction
                                        sendMoneyPreAuthRequest.amountSend = (decimal)narrative.Balance;
                                        sendMoneyPreAuthRequest.sourceCountryCode = narrative.ServiceCountry;
                                        sendMoneyPreAuthRequest.destinationCountryCode = narrative.ServiceRegion;
                                        sendMoneyPreAuthRequest.currencyCodeSend = "1";
                                        sendMoneyPreAuthRequest.clientId = (int)narrative.Id;
                                        sendMoneyPreAuthRequest.agentId = 74;
                                        sendMoneyPreAuthRequest.collectionAmount = Convert.ToDecimal(narrative.Information2);
                                        sendMoneyPreAuthRequest.collectionCurrencyCode = narrative.Currency;
                                        sendMoneyPreAuthRequest.recipientId = Convert.ToInt32(narrative.CustomerId);
                                        sendMoneyPreAuthRequest.tellerId = 154;
                                        sendMoneyPreAuthRequest.reasonForTransfer = narrative.Information1;
                                        tokenFile = LoadMetBankJson(file);

                                        Log.RequestsAndResponses("PreAuthSendMoneyRequest", serviceProvider, sendMoneyPreAuthRequest);

                                        var sendMoneyPreAuthResponse = merchantBankConnector.PreAuthSendMoney(serviceProvider, sendMoneyPreAuthRequest, tokenFile.access_token);

                                        Log.RequestsAndResponses("PreAuthSendMoneyResponse", serviceProvider, sendMoneyPreAuthResponse);

                                        if (!string.IsNullOrEmpty(sendMoneyPreAuthResponse.preauthId))
                                        {
                                            sendMoneyRequest.amountSend = sendMoneyPreAuthRequest.amountSend;
                                            sendMoneyRequest.sourceCountryCode = sendMoneyPreAuthRequest.sourceCountryCode;
                                            sendMoneyRequest.destinationCountryCode = sendMoneyPreAuthRequest.destinationCountryCode;
                                            sendMoneyRequest.currencyCodeSend = sendMoneyPreAuthRequest.currencyCodeSend;
                                            sendMoneyRequest.clientId = sendMoneyPreAuthRequest.clientId;
                                            sendMoneyRequest.agentId = sendMoneyPreAuthRequest.agentId;
                                            sendMoneyRequest.collectionAmount = sendMoneyPreAuthRequest.collectionAmount;
                                            sendMoneyRequest.collectionCurrencyCode = sendMoneyPreAuthRequest.collectionCurrencyCode;
                                            sendMoneyRequest.recipientId = sendMoneyPreAuthRequest.recipientId;
                                            sendMoneyRequest.reasonForTransfer = sendMoneyPreAuthRequest.reasonForTransfer;
                                            sendMoneyRequest.tellerId = sendMoneyPreAuthRequest.tellerId;
                                            sendMoneyRequest.preauthId = sendMoneyPreAuthResponse.preauthId;
                                            tokenFile = LoadMetBankJson(file);

                                            Log.RequestsAndResponses("SendMoneyRequest", serviceProvider, sendMoneyRequest);

                                            var transactionResponse = merchantBankConnector.SendMoney(serviceProvider, sendMoneyRequest, tokenFile.access_token);

                                            Log.RequestsAndResponses("SendMoneyResponse", serviceProvider, transactionResponse);

                                            if (transactionResponse.status.ToUpper() == "COMPLETE")
                                            {
                                                yoAppResponse.Note = transactionResponse.transactionId.ToString();
                                                yoAppResponse.Description = transactionResponse.description;
                                                yoAppResponse.TransactionRef = transactionResponse.transactionReference;
                                                yoAppResponse.Amount = decimal.Parse(transactionResponse.amount.Trim(), System.Globalization.CultureInfo.InvariantCulture);
                                                yoAppResponse.Balance = transactionResponse.fees;
                                                yoAppResponse.Note = transactionResponse.status;
                                                yoAppResponse.Currency = transactionResponse.collectionCurrencyCode;
                                                yoAppResponse.Narrative = JsonConvert.SerializeObject(narrative);

                                                Log.RequestsAndResponses("YoApp-SendMoneyResponse", serviceProvider, transactionResponse);

                                                return yoAppResponse;
                                            }
                                            else
                                            {
                                                yoAppResponse.ResponseCode = "00008";
                                                yoAppResponse.Note = "Failed";
                                                yoAppResponse.Description = "Received Nothing for the agent created";

                                                Log.RequestsAndResponses("YoApp-SendMoneyResponse", serviceProvider, transactionResponse);

                                                return yoAppResponse;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        yoAppResponse.ResponseCode = "00008";
                                        yoAppResponse.Description = "Token is not Valid";
                                        yoAppResponse.Note = "Request Failed";

                                        Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);

                                        return yoAppResponse;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Log.HttpError("Exception", serviceProvider, "Message: " + ex.Message + ", InnerException: " + ex.InnerException + ", StackTrace: " + ex.StackTrace);

                                    return yoAppResponse;
                                }

                                break;

                            case "390000": // Pre Auth Transaction (Depricated)

                                try
                                {
                                    string file = HttpContext.Current.Server.MapPath("~/App_Data/" + serviceProvider + "/Files/" + fileName + ".json");
                                    var tokenFile = LoadMetBankJson(file);

                                    if (tokenFile != null) // We have generated the Token Already
                                    {
                                        var expiryDateString = tokenFile.expires_in;

                                        var expiryDate = DateTime.ParseExact(expiryDateString, "dd/MM/yyyy-HH:mm:ss", null);

                                        var currentDateTime = DateTime.Now;

                                        if (expiryDate > currentDateTime) // Token is still valid
                                        {
                                            isTokenValid = true;
                                        }
                                        else // Token is no longer valid
                                        {
                                            RefreshMetBankToken(tokenFile.refresh_token);
                                            isTokenValid = true;

                                            tokenFile = LoadMetBankJson(file);
                                        }
                                    }
                                    else // Generate a new Token
                                    {
                                        userLogin.password = metBankCredentials.Password;
                                        userLogin.username = metBankCredentials.Username;

                                        Log.RequestsAndResponses("MetBankTokenRequest", serviceProvider, userLogin);

                                        var result = merchantBankConnector.GetToken(serviceProvider, userLogin);

                                        Log.RequestsAndResponses("MetBankTokenResponse", serviceProvider, result);

                                        if (result.token_type.ToLower() == "bearer")
                                        {
                                            isTokenValid = true;
                                            yoAppResponse.ResponseCode = "00000";
                                            yoAppResponse.Description = "Token generated successfully";
                                            yoAppResponse.Note = "Transaction Successful";

                                            var expDate = DateTime.Now.AddSeconds(Convert.ToDouble(result.expires_in));

                                            result.expires_in = expDate.ToString("dd/MM/yyyy-HH:mm:ss");

                                            var token = JsonConvert.SerializeObject(result);

                                            yoAppResponse.Narrative = token;

                                            Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);
                                            Log.StoreData("tokens", serviceProvider, result);

                                            tokenFile = LoadMetBankJson(file);
                                        }
                                        else
                                        {
                                            yoAppResponse.ResponseCode = "00008";
                                            yoAppResponse.Description = "Code could not be generated";
                                            yoAppResponse.Note = "Transaction Failed";

                                            Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);

                                            return yoAppResponse;
                                        }
                                    }

                                    if (isTokenValid)
                                    {
                                        if (response != null)
                                        {
                                            sendMoneyPreAuthRequest.amountSend = response.Amount;
                                            sendMoneyPreAuthRequest.sourceCountryCode = response.Note;
                                            sendMoneyPreAuthRequest.destinationCountryCode = response.Product;
                                            sendMoneyPreAuthRequest.currencyCodeSend = narrative.ServiceCountry;
                                            sendMoneyPreAuthRequest.clientId = (int)narrative.Id;
                                            sendMoneyPreAuthRequest.agentId = Convert.ToInt32(narrative.ServiceAgentId);
                                            sendMoneyPreAuthRequest.collectionAmount = response.Amount;
                                            sendMoneyPreAuthRequest.collectionCurrencyCode = response.Currency;
                                            sendMoneyPreAuthRequest.recipientId = Convert.ToInt32(narrative.CustomerId);
                                            sendMoneyPreAuthRequest.tellerId = Convert.ToInt32(narrative.ServiceId);
                                            sendMoneyPreAuthRequest.reasonForTransfer = response.Action;
                                            tokenFile = LoadMetBankJson(file);

                                            Log.RequestsAndResponses("PreAuthSendMoneyRequest", serviceProvider, sendMoneyPreAuthRequest);

                                            var sendMoneyPreAuthResponse = merchantBankConnector.PreAuthSendMoney(serviceProvider, sendMoneyPreAuthRequest, tokenFile.access_token);

                                            Log.RequestsAndResponses("PreAuthSendMoneyResponse", serviceProvider, sendMoneyPreAuthResponse);

                                            if (sendMoneyPreAuthResponse != null)
                                            {
                                                yoAppResponse.Note = sendMoneyPreAuthResponse.fees.ToString();
                                                yoAppResponse.Description = sendMoneyPreAuthResponse.description;
                                                yoAppResponse.TransactionRef = sendMoneyPreAuthResponse.preauthId.ToString();
                                                yoAppResponse.Amount = sendMoneyPreAuthResponse.totalAmount;
                                                yoAppResponse.Balance = sendMoneyPreAuthResponse.amount.ToString();

                                                Log.RequestsAndResponses("YoApp-PreAuthSendMoneyResponse", serviceProvider, yoAppResponse);

                                                return yoAppResponse;
                                            }
                                            else
                                            {
                                                yoAppResponse.ResponseCode = "00008";
                                                yoAppResponse.Note = "Failed";
                                                yoAppResponse.Description = "Received Nothing for the agent created";

                                                Log.RequestsAndResponses("YoApp-PreAuthSendMoneyResponse", serviceProvider, yoAppResponse);

                                                return yoAppResponse;
                                            }
                                        }
                                        else
                                        {
                                            yoAppResponse.ResponseCode = "00008";
                                            yoAppResponse.Note = "Failed";
                                            yoAppResponse.Description = "Received Nothing for the agent created";

                                            Log.RequestsAndResponses("YoApp-PreAuthSendMoneyResponse", serviceProvider, yoAppResponse);

                                            return yoAppResponse;
                                        }
                                    }
                                    else
                                    {
                                        yoAppResponse.ResponseCode = "00008";
                                        yoAppResponse.Note = "Failed to generate token";
                                        yoAppResponse.Description = "The Token is Invalid";

                                        Log.RequestsAndResponses("YoApp-PreAuthSendMoneyResponse", serviceProvider, yoAppResponse);

                                        return yoAppResponse;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Log.HttpError("Exception", serviceProvider, "Message: " + ex.Message + ", InnerException: " + ex.InnerException + ", StackTrace: " + ex.StackTrace);

                                    return yoAppResponse;
                                }

                            case "400000": // Client PreAuth (Depricated)

                                try
                                {
                                    string file = HttpContext.Current.Server.MapPath("~/App_Data/" + serviceProvider + "/Files/" + fileName + ".json");
                                    var tokenFile = LoadMetBankJson(file);

                                    if (tokenFile != null) // We have generated the Token Already
                                    {
                                        var expiryDateString = tokenFile.expires_in;

                                        var expiryDate = DateTime.ParseExact(expiryDateString, "dd/MM/yyyy-HH:mm:ss", null);

                                        var currentDateTime = DateTime.Now;

                                        if (expiryDate > currentDateTime) // Token is still valid
                                        {
                                            isTokenValid = true;
                                        }
                                        else // Token is no longer valid
                                        {
                                            RefreshMetBankToken(tokenFile.refresh_token);
                                            isTokenValid = true;

                                            tokenFile = LoadMetBankJson(file);
                                        }
                                    }
                                    else // Generate a new Token
                                    {
                                        userLogin.password = metBankCredentials.Password;
                                        userLogin.username = metBankCredentials.Username;

                                        Log.RequestsAndResponses("MetBankTokenRequest", serviceProvider, userLogin);

                                        var result = merchantBankConnector.GetToken(serviceProvider, userLogin);

                                        Log.RequestsAndResponses("MetBankTokenResponse", serviceProvider, result);

                                        if (result.token_type.ToLower() == "bearer")
                                        {
                                            isTokenValid = true;
                                            yoAppResponse.ResponseCode = "00000";
                                            yoAppResponse.Description = "Token generated successfully";
                                            yoAppResponse.Note = "Transaction Successful";

                                            var expDate = DateTime.Now.AddSeconds(Convert.ToDouble(result.expires_in));

                                            result.expires_in = expDate.ToString("dd/MM/yyyy-HH:mm:ss");

                                            var token = JsonConvert.SerializeObject(result);

                                            yoAppResponse.Narrative = token;

                                            Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);
                                            Log.StoreData("tokens", serviceProvider, result);

                                            tokenFile = LoadMetBankJson(file);
                                        }
                                        else
                                        {
                                            yoAppResponse.ResponseCode = "00008";
                                            yoAppResponse.Description = "Code could not be generated";
                                            yoAppResponse.Note = "Transaction Failed";

                                            Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);

                                            return yoAppResponse;
                                        }
                                    }

                                    if (isTokenValid)
                                    {
                                        if (response != null)
                                        {
                                            clientSendMoneyPreAuthRequest.amountSend = response.Amount;
                                            clientSendMoneyPreAuthRequest.sourceCountryCode = response.Note;
                                            clientSendMoneyPreAuthRequest.destinationCountryCode = response.Product;
                                            clientSendMoneyPreAuthRequest.currencyCodeSend = narrative.ServiceCountry;
                                            clientSendMoneyPreAuthRequest.clientId = (int)narrative.Id;
                                            clientSendMoneyPreAuthRequest.collectionAmount = response.Amount;
                                            clientSendMoneyPreAuthRequest.collectionCurrencyCode = response.Currency;
                                            clientSendMoneyPreAuthRequest.recipientId = Convert.ToInt32(narrative.CustomerId);
                                            clientSendMoneyPreAuthRequest.reasonForTransfer = response.Action;
                                            tokenFile = LoadMetBankJson(file);

                                            Log.RequestsAndResponses("ClientPreAuthSendMoneyRequest", serviceProvider, sendMoneyPreAuthRequest);

                                            var clientSendMoneyResponse = merchantBankConnector.ClientPreAuthSendMoney(serviceProvider, clientSendMoneyPreAuthRequest, tokenFile.access_token);

                                            Log.RequestsAndResponses("ClientPreAuthSendMoneyResponse", serviceProvider, clientSendMoneyResponse);

                                            if (clientSendMoneyResponse != null)
                                            {
                                                yoAppResponse.Note = clientSendMoneyResponse.fees.ToString();
                                                yoAppResponse.Description = clientSendMoneyResponse.description;
                                                yoAppResponse.TransactionRef = clientSendMoneyResponse.preauthId.ToString();
                                                yoAppResponse.Amount = clientSendMoneyResponse.totalAmount;
                                                yoAppResponse.Balance = clientSendMoneyResponse.amount.ToString();
                                                yoAppResponse.Currency = clientSendMoneyResponse.currencyCodeSend;
                                                yoAppResponse.Product = clientSendMoneyResponse.collectionCurrencyCode;
                                                yoAppResponse.CustomerData = clientSendMoneyResponse.collectionAmount.ToString();

                                                Log.RequestsAndResponses("YoApp-ClientPreAuthSendMoneyResponse", serviceProvider, yoAppResponse);

                                                return yoAppResponse;
                                            }
                                            else
                                            {
                                                yoAppResponse.ResponseCode = "00008";
                                                yoAppResponse.Note = "Failed";
                                                yoAppResponse.Description = "Received Nothing for the agent created";

                                                Log.RequestsAndResponses("YoApp-ClientPreAuthSendMoneyResponse", serviceProvider, yoAppResponse);

                                                return yoAppResponse;
                                            }
                                        }
                                        else
                                        {
                                            yoAppResponse.ResponseCode = "00008";
                                            yoAppResponse.Note = "Failed";
                                            yoAppResponse.Description = "Response value is null";

                                            Log.RequestsAndResponses("YoApp-ClientPreAuthSendMoneyResponse", serviceProvider, yoAppResponse);

                                            return yoAppResponse;
                                        }
                                    }
                                    else
                                    {
                                        yoAppResponse.ResponseCode = "00008";
                                        yoAppResponse.Note = "Failed to generate token";
                                        yoAppResponse.Description = "The Token is Invalid";

                                        Log.RequestsAndResponses("YoApp-ClientPreAuthSendMoneyResponse", serviceProvider, yoAppResponse);

                                        return yoAppResponse;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Log.HttpError("Exception", serviceProvider, "Message: " + ex.Message + ", InnerException: " + ex.InnerException + ", StackTrace: " + ex.StackTrace);

                                    return yoAppResponse;
                                }

                            case "410000": // Client Send Money

                                try
                                {
                                    string file = HttpContext.Current.Server.MapPath("~/App_Data/" + serviceProvider + "/Files/" + fileName + ".json");
                                    var tokenFile = LoadMetBankJson(file);

                                    if (tokenFile != null) // We have generated the Token Already
                                    {
                                        var expiryDateString = tokenFile.expires_in;

                                        var expiryDate = DateTime.ParseExact(expiryDateString, "dd/MM/yyyy-HH:mm:ss", null);

                                        var currentDateTime = DateTime.Now;

                                        if (expiryDate > currentDateTime) // Token is still valid
                                        {
                                            isTokenValid = true;
                                        }
                                        else // Token is no longer valid
                                        {
                                            RefreshMetBankToken(tokenFile.refresh_token);
                                            isTokenValid = true;

                                            tokenFile = LoadMetBankJson(file);
                                        }
                                    }
                                    else // Generate a new Token
                                    {
                                        userLogin.password = metBankCredentials.Password;
                                        userLogin.username = metBankCredentials.Username;

                                        Log.RequestsAndResponses("MetBankTokenRequest", serviceProvider, userLogin);

                                        var result = merchantBankConnector.GetToken(serviceProvider, userLogin);

                                        Log.RequestsAndResponses("MetBankTokenResponse", serviceProvider, result);

                                        if (result.token_type.ToLower() == "bearer")
                                        {
                                            isTokenValid = true;
                                            yoAppResponse.ResponseCode = "00000";
                                            yoAppResponse.Description = "Token generated successfully";
                                            yoAppResponse.Note = "Transaction Successful";

                                            var expDate = DateTime.Now.AddSeconds(Convert.ToDouble(result.expires_in));

                                            result.expires_in = expDate.ToString("dd/MM/yyyy-HH:mm:ss");

                                            var token = JsonConvert.SerializeObject(result);

                                            yoAppResponse.Narrative = token;

                                            Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);
                                            Log.StoreData("tokens", serviceProvider, result);

                                            tokenFile = LoadMetBankJson(file);
                                        }
                                        else
                                        {
                                            yoAppResponse.ResponseCode = "00008";
                                            yoAppResponse.Description = "Code could not be generated";
                                            yoAppResponse.Note = "Transaction Failed";

                                            Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);

                                            return yoAppResponse;
                                        }
                                    }

                                    if (isTokenValid)
                                    {
                                        // Client PreAuth Send Money
                                        clientSendMoneyPreAuthRequest.amountSend = response.Amount;
                                        clientSendMoneyPreAuthRequest.sourceCountryCode = response.Note;
                                        clientSendMoneyPreAuthRequest.destinationCountryCode = response.Product;
                                        clientSendMoneyPreAuthRequest.currencyCodeSend = narrative.ServiceCountry;
                                        clientSendMoneyPreAuthRequest.clientId = (int)narrative.Id;
                                        clientSendMoneyPreAuthRequest.collectionAmount = response.Amount;
                                        clientSendMoneyPreAuthRequest.collectionCurrencyCode = response.Currency;
                                        clientSendMoneyPreAuthRequest.recipientId = Convert.ToInt32(narrative.CustomerId);
                                        clientSendMoneyPreAuthRequest.reasonForTransfer = response.Action;
                                        tokenFile = LoadMetBankJson(file);

                                        Log.RequestsAndResponses("ClientPreAuthSendMoneyRequest", serviceProvider, sendMoneyPreAuthRequest);

                                        var clientSendMoneyResponse = merchantBankConnector.ClientPreAuthSendMoney(serviceProvider, clientSendMoneyPreAuthRequest, tokenFile.access_token);

                                        Log.RequestsAndResponses("ClientPreAuthSendMoneyResponse", serviceProvider, clientSendMoneyResponse);

                                        if (!string.IsNullOrEmpty(clientSendMoneyResponse.preauthId))
                                        {
                                            clientSendMoneyRequest.amountSend = clientSendMoneyPreAuthRequest.amountSend;
                                            clientSendMoneyRequest.sourceCountryCode = clientSendMoneyPreAuthRequest.sourceCountryCode;
                                            clientSendMoneyRequest.destinationCountryCode = clientSendMoneyPreAuthRequest.destinationCountryCode;
                                            clientSendMoneyRequest.currencyCodeSend = clientSendMoneyPreAuthRequest.currencyCodeSend;
                                            clientSendMoneyRequest.clientId = clientSendMoneyPreAuthRequest.clientId;
                                            clientSendMoneyRequest.collectionAmount = clientSendMoneyPreAuthRequest.collectionAmount;
                                            clientSendMoneyRequest.collectionCurrencyCode = clientSendMoneyPreAuthRequest.collectionCurrencyCode;
                                            clientSendMoneyRequest.recipientId = clientSendMoneyPreAuthRequest.recipientId;
                                            clientSendMoneyRequest.reasonForTransfer = clientSendMoneyPreAuthRequest.reasonForTransfer;
                                            clientSendMoneyRequest.preauthId = clientSendMoneyResponse.preauthId;
                                            tokenFile = LoadMetBankJson(file);

                                            Log.RequestsAndResponses("ClientSendMoneyRequest", serviceProvider, clientSendMoneyRequest);

                                            var transactionResponse = merchantBankConnector.ClientSendMoney(serviceProvider, clientSendMoneyRequest, tokenFile.access_token);

                                            Log.RequestsAndResponses("ClientSendMoneyResponse", serviceProvider, transactionResponse);

                                            if (transactionResponse != null)
                                            {
                                                yoAppResponse.Note = transactionResponse.transactionId.ToString();
                                                yoAppResponse.Description = transactionResponse.description;
                                                yoAppResponse.TransactionRef = transactionResponse.transactionReference;
                                                yoAppResponse.Amount = decimal.Parse(transactionResponse.amount.Trim(), System.Globalization.CultureInfo.InvariantCulture);
                                                yoAppResponse.Balance = transactionResponse.fees;
                                                yoAppResponse.Currency = transactionResponse.collectionCurrencyCode;
                                                yoAppResponse.Product = transactionResponse.status;

                                                Log.RequestsAndResponses("YoApp-ClientSendMoneyResponse", serviceProvider, yoAppResponse);

                                                return yoAppResponse;
                                            }
                                            else
                                            {
                                                yoAppResponse.ResponseCode = "00008";
                                                yoAppResponse.Note = "Failed";
                                                yoAppResponse.Description = "Received Nothing for the agent created";

                                                Log.RequestsAndResponses("YoApp-ClientSendMoneyResponse", serviceProvider, yoAppResponse);

                                                return yoAppResponse;
                                            }
                                        }
                                        else
                                        {
                                            yoAppResponse.ResponseCode = "00008";
                                            yoAppResponse.Note = "Failed";
                                            yoAppResponse.Description = "Response value is null";

                                            Log.RequestsAndResponses("YoApp-ClientSendMoneyResponse", serviceProvider, yoAppResponse);

                                            return yoAppResponse;
                                        }
                                    }
                                    else
                                    {
                                        yoAppResponse.ResponseCode = "00008";
                                        yoAppResponse.Note = "Failed to generate token";
                                        yoAppResponse.Description = "The Token is Invalid";

                                        Log.RequestsAndResponses("YoApp-ClientSendMoneyResponse", serviceProvider, yoAppResponse);

                                        return yoAppResponse;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Log.HttpError("Exception", serviceProvider, "Message: " + ex.Message + ", InnerException: " + ex.InnerException + ", StackTrace: " + ex.StackTrace);

                                    return yoAppResponse;
                                }

                            default:
                                break;
                        }

                        #endregion                       

                        break;

                    case 4: // Receipients

                        #region Recipients

                        //switch (response.ProcessingCode)
                        //{
                        //    //case "360000": // Create Recipient                  


                        //    case "1": // Update Recipient

                        //        try
                        //        {
                        //            string file = HttpContext.Current.Server.MapPath("~/App_Data/" + serviceProvider + "/Files/" + fileName + ".json");
                        //            var tokenFile = LoadMetBankJson(file);

                        //            if (tokenFile != null) // We have generated the Token Already
                        //            {
                        //                var expiryDateString = tokenFile.expires_in;

                        //                var expiryDate = DateTime.ParseExact(expiryDateString, "dd/MM/yyyy-HH:mm:ss", null);

                        //                var currentDateTime = DateTime.Now;

                        //                if (expiryDate > currentDateTime) // Token is still valid
                        //                {
                        //                    isTokenValid = true;
                        //                }
                        //                else // Token is no longer valid
                        //                {
                        //                    RefreshMetBankToken(tokenFile.refresh_token);
                        //                    isTokenValid = true;

                        //                    tokenFile = LoadMetBankJson(file);
                        //                }
                        //            }
                        //            else // Generate a new Token
                        //            {
                        //                userLogin.password = metBankCredentials.Password;
                        //                userLogin.username = metBankCredentials.Username;

                        //                Log.RequestsAndResponses("MetBankTokenRequest", serviceProvider, userLogin);

                        //                var result = merchantBankConnector.GetToken(serviceProvider, userLogin);

                        //                Log.RequestsAndResponses("MetBankTokenResponse", serviceProvider, result);

                        //                if (result.token_type.ToLower() == "bearer")
                        //                {
                        //                    isTokenValid = true;
                        //                    yoAppResponse.ResponseCode = "00000";
                        //                    yoAppResponse.Description = "Token generated successfully";
                        //                    yoAppResponse.Note = "Transaction Successful";

                        //                    var expDate = DateTime.Now.AddSeconds(Convert.ToDouble(result.expires_in));

                        //                    result.expires_in = expDate.ToString("dd/MM/yyyy-HH:mm:ss");

                        //                    var token = JsonConvert.SerializeObject(result);

                        //                    yoAppResponse.Narrative = token;

                        //                    Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);
                        //                    Log.StoreData("tokens", serviceProvider, result);

                        //                    tokenFile = LoadMetBankJson(file);
                        //                }
                        //                else
                        //                {
                        //                    yoAppResponse.ResponseCode = "00008";
                        //                    yoAppResponse.Description = "Code could not be generated";
                        //                    yoAppResponse.Note = "Transaction Failed";

                        //                    Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);

                        //                    return yoAppResponse;
                        //                }
                        //            }

                        //            if (isTokenValid)
                        //            {
                        //                if (!string.IsNullOrEmpty(narrative.ReceiverMobile) && string.IsNullOrEmpty(response.Note)) // Update Phone Number
                        //                {
                        //                    ClientRequest clientRequest = new ClientRequest();

                        //                    clientRequest.id = (int)narrative.Id;
                        //                    clientRequest.phoneNumber = narrative.ReceiverMobile;
                        //                    tokenFile = LoadMetBankJson(file);

                        //                    Log.RequestsAndResponses("RecipientDetailsByPhoneNumberRequest", serviceProvider, clientRequest);

                        //                    var recipient = merchantBankConnector.GetRecipientByClientId(serviceProvider, clientRequest, tokenFile.access_token);

                        //                    Log.RequestsAndResponses("RecipientDetailsByPhoneNumberResponse", serviceProvider, recipient);

                        //                    if (recipient != null && recipient.deleted == false && recipient.firstName != null)
                        //                    {
                        //                        yoAppResponse.ResponseCode = "00000";
                        //                        yoAppResponse.Description = "Recipient Found!";
                        //                        narrative.DateCreated = DateTime.ParseExact(recipient.dateCreated, "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture);
                        //                        narrative.DatelastAccess = DateTime.ParseExact(recipient.dateModified, "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture);
                        //                        narrative.IsActive = recipient.deleted;
                        //                        narrative.Status = recipient.version.ToString();
                        //                        narrative.Id = (long)recipient.id;
                        //                        narrative.ReceiversName = recipient.firstName;
                        //                        narrative.ReceiversSurname = recipient.lastName;
                        //                        yoAppResponse.Note = recipient.relationship;
                        //                        narrative.ReceiverMobile = recipient.phoneNumber;
                        //                        narrative.ReceiversIdentification = recipient.nationalId;
                        //                        narrative.ReceiversGender = recipient.gender;
                        //                        narrative.ServiceCountry = recipient.countryId;
                        //                        narrative.Information1 = recipient.countryName;
                        //                        narrative.Information2 = recipient.address;
                        //                        narrative.CustomerId = recipient.clientId.ToString();
                        //                        yoAppResponse.Narrative = JsonConvert.SerializeObject(narrative);

                        //                        Log.RequestsAndResponses("RecipientDetailsPhoneNumberResponse-YoApp", serviceProvider, yoAppResponse);

                        //                        return yoAppResponse;
                        //                    }
                        //                    else
                        //                    {
                        //                        yoAppResponse.ResponseCode = "00008";
                        //                        yoAppResponse.Note = "Client not Found";
                        //                        yoAppResponse.Description = "Received Nothing for the clientId submitted";

                        //                        Log.RequestsAndResponses("RecipientDetailsByPhoneNumberResponse-YoApp", serviceProvider, yoAppResponse);

                        //                        return yoAppResponse;
                        //                    }
                        //                }
                        //                else if (string.IsNullOrEmpty(narrative.ReceiverMobile) && !string.IsNullOrEmpty(response.Note)) // Update Relationship
                        //                {

                        //                }
                        //            }
                        //            else
                        //            {
                        //                yoAppResponse.ResponseCode = "00008";
                        //                yoAppResponse.Description = "Token is not Valid";
                        //                yoAppResponse.Note = "Request Failed";

                        //                Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);

                        //                return yoAppResponse;
                        //            }
                        //        }
                        //        catch (Exception ex)
                        //        {
                        //            Log.HttpError("Exception", serviceProvider, "Message: " + ex.Message + ", InnerException: " + ex.InnerException + ", StackTrace: " + ex.StackTrace);

                        //            return yoAppResponse;
                        //        }

                        //        break;

                        //    default: // Search Recipient and Client

                        //        #region Search Client and Recipient
                                
                        //        try
                        //        {
                        //            string file = HttpContext.Current.Server.MapPath("~/App_Data/" + serviceProvider + "/Files/" + fileName + ".json");
                        //            var tokenFile = LoadMetBankJson(file);

                        //            if (tokenFile != null) // We have generated the Token Already
                        //            {
                        //                var expiryDateString = tokenFile.expires_in;

                        //                var expiryDate = DateTime.ParseExact(expiryDateString, "dd/MM/yyyy-HH:mm:ss", null);

                        //                var currentDateTime = DateTime.Now;

                        //                if (expiryDate > currentDateTime) // Token is still valid
                        //                {
                        //                    isTokenValid = true;
                        //                }
                        //                else // Token is no longer valid
                        //                {
                        //                    // RefreshMetBankToken(tokenFile.refresh_token);
                        //                    GenerateMetBankToken();
                        //                    isTokenValid = true;

                        //                    tokenFile = LoadMetBankJson(file);
                        //                }
                        //            }
                        //            else // Generate a new Token
                        //            {
                        //                userLogin.password = metBankCredentials.Password;
                        //                userLogin.username = metBankCredentials.Username;

                        //                Log.RequestsAndResponses("MetBankTokenRequest", serviceProvider, userLogin);

                        //                var result = merchantBankConnector.GetToken(serviceProvider, userLogin);

                        //                Log.RequestsAndResponses("MetBankTokenResponse", serviceProvider, result);

                        //                if (result.token_type.ToLower() == "bearer")
                        //                {
                        //                    isTokenValid = true;
                        //                    yoAppResponse.ResponseCode = "00000";
                        //                    yoAppResponse.Description = "Token generated successfully";
                        //                    yoAppResponse.Note = "Transaction Successful";

                        //                    var expDate = DateTime.Now.AddSeconds(Convert.ToDouble(result.expires_in));

                        //                    result.expires_in = expDate.ToString("dd/MM/yyyy-HH:mm:ss");

                        //                    var token = JsonConvert.SerializeObject(result);

                        //                    yoAppResponse.Narrative = token;

                        //                    Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);
                        //                    Log.StoreData("tokens", serviceProvider, result);

                        //                    tokenFile = LoadMetBankJson(file);
                        //                }
                        //                else
                        //                {
                        //                    yoAppResponse.ResponseCode = "00008";
                        //                    yoAppResponse.Description = "Code could not be generated";
                        //                    yoAppResponse.Note = "Transaction Failed";

                        //                    Log.RequestsAndResponses("Wafaya-TokenResponse-YoApp", serviceProvider, yoAppResponse);

                        //                    return yoAppResponse;
                        //                }
                        //            }

                        //            if (isTokenValid)
                        //            {
                        //                if (response.CustomerAccount != null) // Get Client Details Via ClientId or CustomerMobileNumber
                        //                {
                        //                    if (response.Note.ToUpper() == "PROVIDERACCOUNTNUMBER")
                        //                    {
                        //                        ClientRequest clientRequest = new ClientRequest();

                        //                        clientRequest.clientId = response.CustomerAccount;
                        //                        tokenFile = LoadMetBankJson(file);

                        //                        Log.RequestsAndResponses("ClientDetailsByIdRequest", serviceProvider, clientRequest);

                        //                        var client = merchantBankConnector.GetClientByClientId(serviceProvider, clientRequest, tokenFile.access_token);

                        //                        Log.RequestsAndResponses("ClientDetailsByIdResponse", serviceProvider, client);

                        //                        if (client != null && client.deleted == false && client.firstName != null)
                        //                        {
                        //                            List<Narrative> narratives = new List<Narrative>();

                        //                            yoAppResponse.ResponseCode = "00000";
                        //                            yoAppResponse.Description = "Client Found!";
                        //                            narrative.DateCreated = DateTime.ParseExact(client.dateCreated, "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture);
                        //                            narrative.DatelastAccess = DateTime.ParseExact(client.dateModified, "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture);
                        //                            //narrative.IsActive = client.deleted;

                        //                            if (client.deleted)
                        //                            {
                        //                                narrative.IsActive = false;
                        //                            }
                        //                            else
                        //                            {
                        //                                narrative.IsActive = true;
                        //                            }

                        //                            narrative.Status = client.status;
                        //                            narrative.ProviderAccountNumber = client.id.ToString();
                        //                            narrative.CustomerName = client.firstName + " " + client.lastName;
                        //                            narrative.ServiceRegion = "{Email: " + client.email + ", National Id Number: " + client.nationalId + ", Gender: " +
                        //                                client.gender + ",UserId: " + client.userId.ToString() + ",Country: " + client.country + ",Status:" + client.status +
                        //                                ",IsDeleted:" + client.deleted;
                        //                            narrative.CustomerMobileNumber = client.phoneNumber;

                        //                            narratives.Add(narrative);

                        //                            yoAppResponse.Narrative = JsonConvert.SerializeObject(narratives);

                        //                            var jzon = JsonConvert.SerializeObject(yoAppResponse);

                        //                            return yoAppResponse;
                        //                        }
                        //                        else
                        //                        {
                        //                            yoAppResponse.ResponseCode = "00008";
                        //                            yoAppResponse.Note = "Client not Found";
                        //                            yoAppResponse.Description = "Received Nothing for the clientId submitted was wrong";

                        //                            return yoAppResponse;
                        //                        }
                        //                    }
                        //                    else if (response.Note.ToUpper() == "CUSTOMERMOBILENUMBER") // Client Phone Number
                        //                    {
                        //                        ClientRequest clientRequest = new ClientRequest();

                        //                        clientRequest.phoneNumber = response.CustomerAccount;
                        //                        tokenFile = LoadMetBankJson(file);

                        //                        Log.RequestsAndResponses("ClientDetailsByMobileRequest", serviceProvider, clientRequest);

                        //                        var clientResponse = merchantBankConnector.GetClientByPhoneNumber(serviceProvider, clientRequest, tokenFile.access_token);

                        //                        Log.RequestsAndResponses("ClientDetailsByMobileResponse", serviceProvider, clientResponse);

                        //                        if (clientResponse.clientFound)
                        //                        {
                        //                            yoAppResponse.ResponseCode = "00000";
                        //                            yoAppResponse.Description = "Client Found!";
                        //                            narrative.DateCreated = DateTime.ParseExact(clientResponse.client.dateCreated, "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture);
                        //                            narrative.DatelastAccess = DateTime.ParseExact(clientResponse.client.dateModified, "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture);
                        //                            narrative.IsActive = clientResponse.client.deleted;
                        //                            narrative.Status = clientResponse.client.status;
                        //                            narrative.Id = (long)clientResponse.client.id;
                        //                            narrative.ReceiversName = clientResponse.client.firstName;
                        //                            narrative.ReceiversSurname = clientResponse.client.lastName;
                        //                            yoAppResponse.Note = clientResponse.client.email;
                        //                            narrative.ReceiverMobile = clientResponse.client.phoneNumber;
                        //                            narrative.ReceiversIdentification = clientResponse.client.nationalId;
                        //                            narrative.ReceiversGender = clientResponse.client.gender;
                        //                            narrative.ServiceCountry = clientResponse.client.country;
                        //                            narrative.CustomerId = clientResponse.client.userId.ToString();
                        //                            yoAppResponse.Narrative = JsonConvert.SerializeObject(narrative);

                        //                            return yoAppResponse;
                        //                        }
                        //                        else
                        //                        {
                        //                            yoAppResponse.ResponseCode = "00008";
                        //                            yoAppResponse.Note = "Failed";
                        //                            yoAppResponse.Description = "Received Nothing for the Molile Number submitted was wrong";

                        //                            return yoAppResponse;
                        //                        }
                        //                    }
                        //                    else if (response.Note.ToUpper() == "RECEIVERSIDNUMBER")
                        //                    {
                        //                        ClientRequest clientRequest = new ClientRequest();

                        //                        clientRequest.nationalId = narrative.ReceiversIdentification;
                        //                        tokenFile = LoadMetBankJson(file);

                        //                        Log.RequestsAndResponses("ClientDetailsByNationalIdRequest", serviceProvider, clientRequest);

                        //                        var clientResponse = merchantBankConnector.GetClientByNationalId(serviceProvider, clientRequest, tokenFile.access_token);

                        //                        Log.RequestsAndResponses("ClientDetailsByNationalIdResponse", serviceProvider, clientResponse);

                        //                        if (clientResponse.clientFound)
                        //                        {
                        //                            yoAppResponse.ResponseCode = "00000";
                        //                            yoAppResponse.Description = "Clinet Found!";
                        //                            narrative.DateCreated = DateTime.ParseExact(clientResponse.client.dateCreated, "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture);
                        //                            narrative.DatelastAccess = DateTime.ParseExact(clientResponse.client.dateModified, "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture);
                        //                            narrative.IsActive = clientResponse.client.deleted;
                        //                            narrative.Status = clientResponse.client.status;
                        //                            narrative.Id = (long)clientResponse.client.id;
                        //                            narrative.ReceiversName = clientResponse.client.firstName;
                        //                            narrative.ReceiversSurname = clientResponse.client.lastName;
                        //                            yoAppResponse.Note = clientResponse.client.email;
                        //                            narrative.ReceiverMobile = clientResponse.client.phoneNumber;
                        //                            narrative.ReceiversIdentification = clientResponse.client.nationalId;
                        //                            narrative.ReceiversGender = clientResponse.client.gender;
                        //                            narrative.ServiceCountry = clientResponse.client.country;
                        //                            narrative.CustomerId = clientResponse.client.userId.ToString();
                        //                            yoAppResponse.Narrative = JsonConvert.SerializeObject(narrative);

                        //                            return yoAppResponse;
                        //                        }
                        //                        else
                        //                        {
                        //                            yoAppResponse.ResponseCode = "00008";
                        //                            yoAppResponse.Note = "Failed";
                        //                            yoAppResponse.Description = "Received Nothing for the National Id submitted was wrong";

                        //                            return yoAppResponse;
                        //                        }
                        //                    }
                        //                    else if (response.Note.ToUpper() == "INFORMATION1")
                        //                    {
                        //                        ClientRequest clientRequest = new ClientRequest();

                        //                        var parts = narrative.Information1.Split(new char[] { '~' }, StringSplitOptions.RemoveEmptyEntries);

                        //                        clientRequest.page = parts[0];
                        //                        clientRequest.size = parts[1];

                        //                        tokenFile = LoadMetBankJson(file);

                        //                        Log.RequestsAndResponses("ClientDetailsByPageAndSizeRequest", serviceProvider, clientRequest);

                        //                        var pageClient = merchantBankConnector.GetClientsByPagesAndSize(serviceProvider, clientRequest, tokenFile.access_token);

                        //                        Log.RequestsAndResponses("ClientDetailsByPageAndSizeResponse", serviceProvider, pageClient);

                        //                        if (pageClient.totalPages > 0)
                        //                        {
                        //                            response.CustomerData = pageClient.totalElements.ToString();
                        //                            response.CustomerAccount = pageClient.totalPages.ToString();

                        //                            var serializedContent = JsonConvert.SerializeObject(pageClient.content);

                        //                            response.Note = serializedContent;
                        //                            response.TerminalId = pageClient.number.ToString();

                        //                            var serializedSort = JsonConvert.SerializeObject(pageClient.sort);

                        //                            response.TransactionRef = serializedSort;
                        //                            response.IsActive = pageClient.first;

                        //                            var serializedPageable = JsonConvert.SerializeObject(pageClient.pageable);

                        //                            response.Product = serializedPageable;
                        //                            response.Quantity = pageClient.numberOfElements;
                        //                            narrative.IsActive = pageClient.last;
                        //                            narrative.CustomerName = pageClient.empty.ToString();
                        //                            response.Narrative = JsonConvert.SerializeObject(narrative);

                        //                            return yoAppResponse;
                        //                        }
                        //                        else
                        //                        {
                        //                            yoAppResponse.ResponseCode = "00008";
                        //                            yoAppResponse.Note = "Clients not found!";
                        //                            yoAppResponse.Description = "There are no pages to be displayed";

                        //                            return yoAppResponse;
                        //                        }
                        //                    }
                        //                    else if (response.Note.ToUpper() == "RECEIVERPROVIDERACCOUNTNUMBER")
                        //                    {
                        //                        ClientRequest clientRequest = new ClientRequest();

                        //                        clientRequest.clientId = response.CustomerAccount;
                        //                        tokenFile = LoadMetBankJson(file);

                        //                        Log.RequestsAndResponses("RecipientDetailsByIdRequest", serviceProvider, clientRequest);

                        //                        var recipient = merchantBankConnector.GetRecipientById(serviceProvider, clientRequest, tokenFile.access_token);

                        //                        Log.RequestsAndResponses("RecipientDetailsByIdResponse", serviceProvider, recipient);

                        //                        if (recipient != null && recipient.deleted == false && recipient.firstName != null)
                        //                        {
                        //                            List<Narrative> narratives = new List<Narrative>();

                        //                            yoAppResponse.ResponseCode = "00000";
                        //                            yoAppResponse.Description = "Recipient Found!";
                        //                            narrative.DateCreated = DateTime.ParseExact(recipient.dateCreated, "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture);
                        //                            narrative.DatelastAccess = DateTime.ParseExact(recipient.dateModified, "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture);
                        //                            narrative.IsActive = recipient.deleted;

                        //                            if (recipient.deleted)
                        //                            {
                        //                                narrative.IsActive = false;
                        //                            }
                        //                            else
                        //                            {
                        //                                narrative.IsActive = true;
                        //                            }

                        //                            narrative.Status = recipient.version.ToString();
                        //                            narrative.ReceiverProviderAccountNumber = recipient.id.ToString();
                        //                            narrative.ReceiversName = recipient.firstName;
                        //                            narrative.ReceiversSurname = recipient.lastName;
                        //                            //narrative.ReceiversSurname = recipient.lastName;                                                    
                        //                            narrative.ReceiverMobile = recipient.phoneNumber;

                        //                            narrative.Information1 = "{Relationship:" + recipient.relationship + ",NationalId:" + recipient.nationalId + ",Gender:" + recipient.gender +
                        //                                ",ClientId: " + recipient.clientId + "}";
                        //                            //yoAppResponse.Note = recipient.relationship;
                        //                            //narrative.ReceiversIdentification = recipient.nationalId;
                        //                            //narrative.ReceiversGender = recipient.gender;
                        //                            //narrative.ServiceCountry = recipient.countryId;
                        //                            //narrative.Information1 = recipient.countryName;
                        //                            //narrative.Information2 = recipient.address;
                        //                            //narrative.CustomerId = recipient.clientId.ToString();

                        //                            narratives.Add(narrative);
                        //                            yoAppResponse.Narrative = JsonConvert.SerializeObject(narratives);

                        //                            Log.RequestsAndResponses("RecipientDetailsByIdResponse-YoApp", serviceProvider, yoAppResponse);

                        //                            return yoAppResponse;
                        //                        }
                        //                        else
                        //                        {
                        //                            yoAppResponse.ResponseCode = "00008";
                        //                            yoAppResponse.Note = "Recipient not Found";
                        //                            yoAppResponse.Description = "Received Nothing for the clientId submitted";

                        //                            Log.RequestsAndResponses("RecipientDetailsByIdResponse-YoApp", serviceProvider, yoAppResponse);

                        //                            return yoAppResponse;
                        //                        }
                        //                    }
                        //                    else
                        //                    {
                        //                        yoAppResponse.ResponseCode = "00008";
                        //                        yoAppResponse.Note = "Failed";
                        //                        yoAppResponse.Description = "Received Nothing for the Page Number was not submitted";

                        //                        return yoAppResponse;
                        //                    }

                        //                }
                        //                else
                        //                {
                        //                    yoAppResponse.ResponseCode = "00008";
                        //                    yoAppResponse.Note = "Failed";
                        //                    yoAppResponse.Description = "Received Nothing for the clientId submitted";

                        //                    return yoAppResponse;
                        //                }
                        //            }
                        //            else
                        //            {
                        //                yoAppResponse.ResponseCode = "00008";
                        //                yoAppResponse.Description = "Token is not Valid";
                        //                yoAppResponse.Note = "Request Failed";

                        //                Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);

                        //                return yoAppResponse;
                        //            }
                        //        }
                        //        catch (Exception ex)
                        //        {
                        //            Log.HttpError("Exception", serviceProvider, "Message: " + ex.Message + ", InnerException: " + ex.InnerException + ", StackTrace: " + ex.StackTrace);

                        //            return yoAppResponse;
                        //        }
                        //        #endregion

                        //        try
                        //        {
                        //            string file = HttpContext.Current.Server.MapPath("~/App_Data/" + serviceProvider + "/Files/" + fileName + ".json");
                        //            var tokenFile = LoadMetBankJson(file);

                        //            if (tokenFile != null) // We have generated the Token Already
                        //            {
                        //                var expiryDateString = tokenFile.expires_in;

                        //                var expiryDate = DateTime.ParseExact(expiryDateString, "dd/MM/yyyy-HH:mm:ss", null);

                        //                var currentDateTime = DateTime.Now;

                        //                if (expiryDate > currentDateTime) // Token is still valid
                        //                {
                        //                    isTokenValid = true;
                        //                }
                        //                else // Token is no longer valid
                        //                {
                        //                    RefreshMetBankToken(tokenFile.refresh_token);
                        //                    isTokenValid = true;

                        //                    tokenFile = LoadMetBankJson(file);
                        //                }
                        //            }
                        //            else // Generate a new Token
                        //            {
                        //                userLogin.password = metBankCredentials.Password;
                        //                userLogin.username = metBankCredentials.Username;

                        //                Log.RequestsAndResponses("MetBankTokenRequest", serviceProvider, userLogin);

                        //                var result = merchantBankConnector.GetToken(serviceProvider, userLogin);

                        //                Log.RequestsAndResponses("MetBankTokenResponse", serviceProvider, result);

                        //                if (result.token_type.ToLower() == "bearer")
                        //                {
                        //                    isTokenValid = true;
                        //                    yoAppResponse.ResponseCode = "00000";
                        //                    yoAppResponse.Description = "Token generated successfully";
                        //                    yoAppResponse.Note = "Transaction Successful";

                        //                    var expDate = DateTime.Now.AddSeconds(Convert.ToDouble(result.expires_in));

                        //                    result.expires_in = expDate.ToString("dd/MM/yyyy-HH:mm:ss");

                        //                    var token = JsonConvert.SerializeObject(result);

                        //                    yoAppResponse.Narrative = token;

                        //                    Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);
                        //                    Log.StoreData("tokens", serviceProvider, result);

                        //                    tokenFile = LoadMetBankJson(file);
                        //                }
                        //                else
                        //                {
                        //                    yoAppResponse.ResponseCode = "00008";
                        //                    yoAppResponse.Description = "Code could not be generated";
                        //                    yoAppResponse.Note = "Transaction Failed";

                        //                    Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);

                        //                    return yoAppResponse;
                        //                }
                        //            }

                        //            if (isTokenValid)
                        //            {
                        //                if (narrative.Id > 0 && string.IsNullOrEmpty(narrative.CustomerId)) // Search by Id
                        //                {
                        //                    ClientRequest clientRequest = new ClientRequest();

                        //                    clientRequest.id = (int)narrative.Id;
                        //                    tokenFile = LoadMetBankJson(file);

                        //                    Log.RequestsAndResponses("RecipientDetailsByIdRequest", serviceProvider, clientRequest);

                        //                    var recipient = merchantBankConnector.GetRecipientById(serviceProvider, clientRequest, tokenFile.access_token);

                        //                    Log.RequestsAndResponses("RecipientDetailsByIdResponse", serviceProvider, recipient);

                        //                    if (recipient != null && recipient.deleted == false && recipient.firstName != null)
                        //                    {
                        //                        yoAppResponse.ResponseCode = "00000";
                        //                        yoAppResponse.Description = "Recipient Found!";
                        //                        narrative.DateCreated = DateTime.ParseExact(recipient.dateCreated, "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture);
                        //                        narrative.DatelastAccess = DateTime.ParseExact(recipient.dateModified, "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture);
                        //                        narrative.IsActive = recipient.deleted;
                        //                        narrative.Status = recipient.version.ToString();
                        //                        narrative.Id = (long)recipient.id;
                        //                        narrative.ReceiversName = recipient.firstName;
                        //                        narrative.ReceiversSurname = recipient.lastName;
                        //                        yoAppResponse.Note = recipient.relationship;
                        //                        narrative.ReceiverMobile = recipient.phoneNumber;
                        //                        narrative.ReceiversIdentification = recipient.nationalId;
                        //                        narrative.ReceiversGender = recipient.gender;
                        //                        narrative.ServiceCountry = recipient.countryId;
                        //                        narrative.Information1 = recipient.countryName;
                        //                        narrative.Information2 = recipient.address;
                        //                        narrative.CustomerId = recipient.clientId.ToString();
                        //                        yoAppResponse.Narrative = JsonConvert.SerializeObject(narrative);

                        //                        Log.RequestsAndResponses("RecipientDetailsByIdResponse-YoApp", serviceProvider, yoAppResponse);

                        //                        return yoAppResponse;
                        //                    }
                        //                    else
                        //                    {
                        //                        yoAppResponse.ResponseCode = "00008";
                        //                        yoAppResponse.Note = "Client not Found";
                        //                        yoAppResponse.Description = "Received Nothing for the clientId submitted";

                        //                        Log.RequestsAndResponses("RecipientDetailsByIdResponse-YoApp", serviceProvider, yoAppResponse);

                        //                        return yoAppResponse;
                        //                    }
                        //                }
                        //                else if (narrative.Id <= 0 && !string.IsNullOrEmpty(narrative.CustomerId)) // Search by ClientId
                        //                {
                        //                    ClientRequest clientRequest = new ClientRequest();

                        //                    clientRequest.clientId = narrative.CustomerId;
                        //                    tokenFile = LoadMetBankJson(file);

                        //                    Log.RequestsAndResponses("RecipientDetailsByClientIdRequest", serviceProvider, clientRequest);

                        //                    var recipient = merchantBankConnector.GetRecipientByClientId(serviceProvider, clientRequest, tokenFile.access_token);

                        //                    Log.RequestsAndResponses("RecipientDetailsByClientIdResponse", serviceProvider, recipient);

                        //                    if (recipient != null && recipient.deleted == false && recipient.firstName != null)
                        //                    {
                        //                        yoAppResponse.ResponseCode = "00000";
                        //                        yoAppResponse.Description = "Recipient Found!";
                        //                        narrative.DateCreated = DateTime.ParseExact(recipient.dateCreated, "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture);
                        //                        narrative.DatelastAccess = DateTime.ParseExact(recipient.dateModified, "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture);
                        //                        narrative.IsActive = recipient.deleted;
                        //                        narrative.Status = recipient.version.ToString();
                        //                        narrative.Id = (long)recipient.id;
                        //                        narrative.ReceiversName = recipient.firstName;
                        //                        narrative.ReceiversSurname = recipient.lastName;
                        //                        yoAppResponse.Note = recipient.relationship;
                        //                        narrative.ReceiverMobile = recipient.phoneNumber;
                        //                        narrative.ReceiversIdentification = recipient.nationalId;
                        //                        narrative.ReceiversGender = recipient.gender;
                        //                        narrative.ServiceCountry = recipient.countryId;
                        //                        narrative.Information1 = recipient.countryName;
                        //                        narrative.Information2 = recipient.address;
                        //                        narrative.CustomerId = recipient.clientId.ToString();
                        //                        yoAppResponse.Narrative = JsonConvert.SerializeObject(narrative);

                        //                        Log.RequestsAndResponses("RecipientDetailsByClientIdResponse-YoApp", serviceProvider, yoAppResponse);

                        //                        return yoAppResponse;
                        //                    }
                        //                    else
                        //                    {
                        //                        yoAppResponse.ResponseCode = "00008";
                        //                        yoAppResponse.Note = "Client not Found";
                        //                        yoAppResponse.Description = "Received Nothing for the clientId submitted";

                        //                        Log.RequestsAndResponses("RecipientDetailsByClientIdResponse-YoApp", serviceProvider, yoAppResponse);

                        //                        return yoAppResponse;
                        //                    }
                        //                }
                        //                else
                        //                {
                        //                    yoAppResponse.ResponseCode = "00008";
                        //                    yoAppResponse.Description = "Cannot Search for receipient because No Id or ClientId was received from the server";
                        //                    yoAppResponse.Note = "Transaction Failed";

                        //                    Log.RequestsAndResponses("RecipientDetailsByClientIdResponse-YoApp", serviceProvider, yoAppResponse);

                        //                    return yoAppResponse;
                        //                }
                        //            }
                        //            else
                        //            {
                        //                yoAppResponse.ResponseCode = "00008";
                        //                yoAppResponse.Description = "Token is not Valid";
                        //                yoAppResponse.Note = "Request Failed";

                        //                Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);

                        //                return yoAppResponse;
                        //            }
                        //        }
                        //        catch (Exception ex)
                        //        {
                        //            Log.HttpError("Exception", serviceProvider, "Message: " + ex.Message + ", InnerException: " + ex.InnerException + ", StackTrace: " + ex.StackTrace);

                        //            return yoAppResponse;
                        //        }
                        //}

                        #endregion

                        break;

                    case 5: // Receive Money

                        #region Receive Money Section

                        try
                        {
                            string file = HttpContext.Current.Server.MapPath("~/App_Data/" + serviceProvider + "/Files/" + fileName + ".json");
                            var tokenFile = LoadMetBankJson(file);

                            if (tokenFile != null) // We have generated the Token Already
                            {
                                var expiryDateString = tokenFile.expires_in;

                                var expiryDate = DateTime.ParseExact(expiryDateString, "dd/MM/yyyy-HH:mm:ss", null);

                                var currentDateTime = DateTime.Now;

                                if (expiryDate > currentDateTime) // Token is still valid
                                {
                                    isTokenValid = true;
                                }
                                else // Token is no longer valid
                                {
                                    //RefreshMetBankToken(tokenFile.refresh_token);
                                    GenerateMetBankToken();
                                    isTokenValid = true;

                                    //tokenFile = LoadMetBankJson(file);

                                    //tokenFile = metBankCredentials.AccessToken;
                                }
                            }
                            else // Generate a new Token
                            {
                                userLogin.password = metBankCredentials.Password;
                                userLogin.username = metBankCredentials.Username;
                                userLogin.clientSecret = metBankCredentials.ClientSecret;
                                userLogin.clientId = metBankCredentials.ClientId;

                                Log.RequestsAndResponses("MetBankTokenRequest", serviceProvider, userLogin);

                                var result = merchantBankConnector.GetToken(serviceProvider, userLogin);

                                Log.RequestsAndResponses("MetBankTokenResponse", serviceProvider, result);

                                if (result.token_type.ToLower() == "bearer")
                                {
                                    isTokenValid = true;
                                    yoAppResponse.ResponseCode = "00000";
                                    yoAppResponse.Description = "Token generated successfully";
                                    yoAppResponse.Note = "Transaction Successful";

                                    var expDate = DateTime.Now.AddSeconds(Convert.ToDouble(result.expires_in));

                                    result.expires_in = expDate.ToString("dd/MM/yyyy-HH:mm:ss");

                                    var token = JsonConvert.SerializeObject(result);

                                    yoAppResponse.Narrative = token;

                                    Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);
                                    Log.StoreData("tokens", serviceProvider, result);

                                    tokenFile = LoadMetBankJson(file);
                                }
                                else
                                {
                                    yoAppResponse.ResponseCode = "00008";
                                    yoAppResponse.Description = "Code could not be generated";
                                    yoAppResponse.Note = "Transaction Failed";

                                    Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);

                                    return yoAppResponse;
                                }
                            }

                            if (isTokenValid)
                            {
                                tokenFile = LoadMetBankJson(file);

                                // Receive Money PreAuth Request
                                ReceiveMoneyPreAuthRequest receiveMoneyPreAuthRequest = new ReceiveMoneyPreAuthRequest();

                                receiveMoneyPreAuthRequest.agentId = (long)tokenFile.agentId;
                                receiveMoneyPreAuthRequest.voucherNumber = narrative.Information1;
                                receiveMoneyPreAuthRequest.tellerId = "154";

                                Log.RequestsAndResponses("ReceiveMoneyPreAuthRequest", serviceProvider, receiveMoneyPreAuthRequest);

                                var receiveMoneyPreAuthResponse = merchantBankConnector.PreAuthReceiveMoney(serviceProvider, receiveMoneyPreAuthRequest, tokenFile.access_token); // metBankCredentials.AccessToken

                                Log.RequestsAndResponses("ReceiveMoneyPreAuthResponse", serviceProvider, receiveMoneyPreAuthResponse);

                                if (!string.IsNullOrEmpty(receiveMoneyPreAuthResponse.preauthId))
                                {

                                    receiveMoneyRequest.tellerId = 154;
                                    receiveMoneyRequest.agentId = (int)tokenFile.agentId;
                                    receiveMoneyRequest.preauthId = receiveMoneyPreAuthResponse.preauthId;
                                    receiveMoneyRequest.voucherNumber = receiveMoneyPreAuthRequest.voucherNumber;
                                    tokenFile = LoadMetBankJson(file);

                                    Log.RequestsAndResponses("ReceiveMoneyRequest", serviceProvider, receiveMoneyRequest);

                                    var transactionResponse = merchantBankConnector.ReceiveMoney(serviceProvider, receiveMoneyRequest, tokenFile.access_token);

                                    Log.RequestsAndResponses("ReceiveMoneyResponse", serviceProvider, transactionResponse);

                                    if (!string.IsNullOrEmpty(transactionResponse.status))
                                    {
                                        yoAppResponse.ResponseCode = "00000";
                                        yoAppResponse.Description = "Token generated successfully";                                       
                                        yoAppResponse.Note = transactionResponse.transactionId.ToString();
                                        yoAppResponse.Description = transactionResponse.description;
                                        yoAppResponse.TransactionRef = transactionResponse.transactionReference;
                                        yoAppResponse.Amount = decimal.Parse(transactionResponse.amount.Trim(), System.Globalization.CultureInfo.InvariantCulture);
                                        yoAppResponse.Balance = transactionResponse.fees;
                                        yoAppResponse.Note = transactionResponse.status;
                                        yoAppResponse.Currency = transactionResponse.collectionCurrencyCode;
                                        yoAppResponse.Narrative = JsonConvert.SerializeObject(narrative);

                                        Log.RequestsAndResponses("YoAppReceiveMoneyResponse", serviceProvider, transactionResponse);

                                        return yoAppResponse;
                                    }
                                    else
                                    {
                                        yoAppResponse.ResponseCode = "00008";
                                        yoAppResponse.Note = "Failed";
                                        yoAppResponse.Description = "Failed to receive the money!";

                                        Log.RequestsAndResponses("YoAppReceiveMoneyResponse", serviceProvider, transactionResponse);

                                        return yoAppResponse;
                                    }
                                }
                                else
                                {
                                    yoAppResponse.ResponseCode = "00008";
                                    yoAppResponse.Note = "Failed";
                                    yoAppResponse.Description = "Failed to receive a PreAuth Id from the server";

                                    Log.RequestsAndResponses("YoAppReceiveMoneyPreAuthIdRequest", serviceProvider, yoAppResponse);

                                    return yoAppResponse;
                                }
                            }
                            else
                            {
                                yoAppResponse.ResponseCode = "00008";
                                yoAppResponse.Note = "Failed to generate token";
                                yoAppResponse.Description = "The Token is Invalid";

                                Log.RequestsAndResponses("YoAppMetbankResponse", serviceProvider, yoAppResponse);

                                return yoAppResponse;
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.HttpError("Exception", serviceProvider, "Message: " + ex.Message + ", InnerException: " + ex.InnerException + ", StackTrace: " + ex.StackTrace);

                            return yoAppResponse;
                        }

                        #endregion

                    case 6: // Send Money

                        #region Send Money Section

                        try
                        {
                            string file = HttpContext.Current.Server.MapPath("~/App_Data/" + serviceProvider + "/Files/" + fileName + ".json");
                            var tokenFile = LoadMetBankJson(file);

                            if (tokenFile != null) // We have generated the Token Already
                            {
                                var expiryDateString = tokenFile.expires_in;

                                var expiryDate = DateTime.ParseExact(expiryDateString, "dd/MM/yyyy-HH:mm:ss", null);

                                var currentDateTime = DateTime.Now;

                                if (expiryDate > currentDateTime) // Token is still valid
                                {
                                    isTokenValid = true;
                                }
                                else // Token is no longer valid
                                {
                                    //RefreshMetBankToken(tokenFile.refresh_token);

                                    GenerateMetBankToken();
                                    isTokenValid = true;

                                    //tokenFile = LoadMetBankJson(file);

                                    //tokenFile = metBankCredentials.AccessToken;
                                }
                            }
                            else // Generate a new Token
                            {
                                userLogin.password = metBankCredentials.Password;
                                userLogin.username = metBankCredentials.Username;
                                userLogin.clientSecret = metBankCredentials.ClientSecret;
                                userLogin.clientId = metBankCredentials.ClientId;

                                Log.RequestsAndResponses("MetBankTokenRequest", serviceProvider, userLogin);

                                var result = merchantBankConnector.GetToken(serviceProvider, userLogin);

                                Log.RequestsAndResponses("MetBankTokenResponse", serviceProvider, result);

                                if (result.token_type.ToLower() == "bearer")
                                {
                                    isTokenValid = true;
                                    yoAppResponse.ResponseCode = "00000";
                                    yoAppResponse.Description = "Token generated successfully";
                                    yoAppResponse.Note = "Transaction Successful";

                                    var expDate = DateTime.Now.AddSeconds(Convert.ToDouble(result.expires_in));

                                    result.expires_in = expDate.ToString("dd/MM/yyyy-HH:mm:ss");

                                    var token = JsonConvert.SerializeObject(result);

                                    yoAppResponse.Narrative = token;

                                    Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);
                                    Log.StoreData("tokens", serviceProvider, result);

                                    tokenFile = LoadMetBankJson(file);
                                }
                                else
                                {
                                    yoAppResponse.ResponseCode = "00008";
                                    yoAppResponse.Description = "Code could not be generated";
                                    yoAppResponse.Note = "Transaction Failed";

                                    Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);

                                    return yoAppResponse;
                                }
                            }

                            if (isTokenValid)
                            {
                                #region Send Money Preauth Request Commented Out
                                //tokenFile = LoadMetBankJson(file);

                                //// Send Money PreAuth Transaction
                                //sendMoneyPreAuthRequest.amountSend = (decimal)narrative.Balance;
                                //sendMoneyPreAuthRequest.sourceCountryCode = narrative.SourceCountry.Trim();
                                //sendMoneyPreAuthRequest.destinationCountryCode = narrative.ServiceCountry.Trim();
                                //sendMoneyPreAuthRequest.currencyCodeSend = narrative.Currency.Trim();
                                //sendMoneyPreAuthRequest.clientId = Convert.ToInt32(narrative.ProviderAccountNumber.Trim());
                                //sendMoneyPreAuthRequest.agentId = (int)tokenFile.agentId;
                                //sendMoneyPreAuthRequest.collectionAmount = (decimal)narrative.Balance;
                                //sendMoneyPreAuthRequest.collectionCurrencyCode = narrative.Currency.Trim();

                                //if (narrative.Currency.ToUpper().Trim() == "ZWL")
                                //{
                                //    sendMoneyPreAuthRequest.currencyCodeSend = "USD";
                                //    sendMoneyPreAuthRequest.collectionCurrencyCode = "USD";
                                //}

                                //sendMoneyPreAuthRequest.recipientId = Convert.ToInt32(narrative.ReceiverProviderAccountNumber.Trim());
                                //sendMoneyPreAuthRequest.tellerId = 154;
                                //sendMoneyPreAuthRequest.reasonForTransfer = narrative.Information1.Trim();
                                //tokenFile = LoadMetBankJson(file);

                                //Log.RequestsAndResponses("PreAuthSendMoneyRequest", serviceProvider, sendMoneyPreAuthRequest);

                                //var sendMoneyPreAuthResponse = merchantBankConnector.PreAuthSendMoney(serviceProvider, sendMoneyPreAuthRequest, tokenFile.access_token);

                                //Log.RequestsAndResponses("PreAuthSendMoneyResponse", serviceProvider, sendMoneyPreAuthResponse); 
                                #endregion

                                string fileNameStored = HttpContext.Current.Server.MapPath("~/App_Data/" + serviceProvider + "/Files/" + narrative.CustomerMobileNumber + ".json");

                                var storedSendMoneyRequest = LoadSendMoneyRequest(fileNameStored, serviceProvider);

                                tokenFile = LoadMetBankJson(file);

                                if (!string.IsNullOrEmpty(storedSendMoneyRequest.preauthId))
                                {
                                    #region SendMoney Request Commented Out
                                    //sendMoneyRequest.amountSend = sendMoneyPreAuthRequest.amountSend;
                                    //sendMoneyRequest.sourceCountryCode = sendMoneyPreAuthRequest.sourceCountryCode;
                                    //sendMoneyRequest.destinationCountryCode = sendMoneyPreAuthRequest.destinationCountryCode;
                                    //sendMoneyRequest.currencyCodeSend = sendMoneyPreAuthRequest.currencyCodeSend;
                                    //sendMoneyRequest.clientId = sendMoneyPreAuthRequest.clientId;
                                    //sendMoneyRequest.agentId = sendMoneyPreAuthRequest.agentId;
                                    //sendMoneyRequest.collectionAmount = sendMoneyPreAuthRequest.collectionAmount;
                                    //sendMoneyRequest.collectionCurrencyCode = sendMoneyPreAuthRequest.collectionCurrencyCode;
                                    //sendMoneyRequest.recipientId = sendMoneyPreAuthRequest.recipientId;
                                    //sendMoneyRequest.reasonForTransfer = sendMoneyPreAuthRequest.reasonForTransfer;
                                    //sendMoneyRequest.tellerId = sendMoneyPreAuthRequest.tellerId;
                                    //sendMoneyRequest.preauthId = sendMoneyPreAuthResponse.preauthId;
                                    //tokenFile = LoadMetBankJson(file); 
                                    #endregion

                                    Log.RequestsAndResponses("SendMoneyRequest", serviceProvider, storedSendMoneyRequest);

                                    var transactionResponse = merchantBankConnector.SendMoney(serviceProvider, storedSendMoneyRequest, tokenFile.access_token);

                                    Log.RequestsAndResponses("SendMoneyResponse", serviceProvider, transactionResponse);

                                    if (transactionResponse.status.ToUpper() == "COMPLETE")
                                    {
                                        yoAppResponse.ResponseCode = "00000";
                                        yoAppResponse.Note = transactionResponse.transactionId.ToString();
                                        yoAppResponse.Description = transactionResponse.description;
                                        yoAppResponse.TransactionRef = transactionResponse.transactionReference;
                                        yoAppResponse.Amount = decimal.Parse(transactionResponse.amount, System.Globalization.CultureInfo.InvariantCulture);
                                        yoAppResponse.Balance = transactionResponse.fees;
                                        yoAppResponse.Note = transactionResponse.status;
                                        yoAppResponse.Currency = transactionResponse.collectionCurrencyCode;
                                        yoAppResponse.Narrative = JsonConvert.SerializeObject(narrative);

                                        Log.RequestsAndResponses("YoAppSendMoneyResponse", serviceProvider, yoAppResponse);

                                        return yoAppResponse;
                                    }
                                    else
                                    {
                                        yoAppResponse.ResponseCode = "00008";
                                        yoAppResponse.Note = "Failed";
                                        yoAppResponse.Description = "Money was not sent! See log for details.";

                                        Log.RequestsAndResponses("YoAppSendMoneyResponse", serviceProvider, yoAppResponse);

                                        return yoAppResponse;
                                    }
                                }
                                else
                                {
                                    yoAppResponse.ResponseCode = "00008";
                                    yoAppResponse.Note = "Failed";
                                    yoAppResponse.Description = "Preauth Id was not saved!";

                                    Log.RequestsAndResponses("YoAppSendMoneyResponse", serviceProvider, yoAppResponse);

                                    return yoAppResponse;
                                }
                            }
                            else
                            {
                                yoAppResponse.ResponseCode = "00008";
                                yoAppResponse.Description = "Token is not Valid";
                                yoAppResponse.Note = "Request Failed";

                                Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);

                                return yoAppResponse;
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.HttpError("Exception", serviceProvider, "Message: " + ex.Message + ", InnerException: " + ex.InnerException + ", StackTrace: " + ex.StackTrace);

                            yoAppResponse.ResponseCode = "00008";
                            yoAppResponse.Description = "There has been a fatal error from the server in trying to send the money. See Log for details";

                            return yoAppResponse;
                        }

                        #endregion                        

                    case 7: // Client Registration

                        #region Register Clients

                        try
                        {
                            string file = HttpContext.Current.Server.MapPath("~/App_Data/" + serviceProvider + "/Files/" + fileName + ".json");
                            var tokenFile = LoadMetBankJson(file);

                            if (tokenFile != null) // We have generated the Token Already
                            {
                                var expiryDateString = tokenFile.expires_in;

                                var expiryDate = DateTime.ParseExact(expiryDateString, "dd/MM/yyyy-HH:mm:ss", null);

                                var currentDateTime = DateTime.Now;

                                if (expiryDate > currentDateTime) // Token is still valid
                                {
                                    isTokenValid = true;
                                }
                                else // Token is no longer valid
                                {
                                    //RefreshMetBankToken(tokenFile.refresh_token);
                                    GenerateMetBankToken();
                                    isTokenValid = true;

                                    //tokenFile = LoadMetBankJson(file);

                                    //tokenFile = metBankCredentials.AccessToken;
                                }
                            }
                            else // Generate a new Token
                            {
                                userLogin.password = metBankCredentials.Password;
                                userLogin.username = metBankCredentials.Username;
                                userLogin.clientSecret = metBankCredentials.ClientSecret;
                                userLogin.clientId = metBankCredentials.ClientId;

                                Log.RequestsAndResponses("MetBankTokenRequest", serviceProvider, userLogin);

                                var result = merchantBankConnector.GetToken(serviceProvider, userLogin);

                                Log.RequestsAndResponses("MetBankTokenResponse", serviceProvider, result);

                                if (result.token_type.ToLower() == "bearer")
                                {
                                    isTokenValid = true;
                                    yoAppResponse.ResponseCode = "00000";
                                    yoAppResponse.Description = "Token generated successfully";
                                    yoAppResponse.Note = "Transaction Successful";

                                    var expDate = DateTime.Now.AddSeconds(Convert.ToDouble(result.expires_in));

                                    result.expires_in = expDate.ToString("dd/MM/yyyy-HH:mm:ss");

                                    var token = JsonConvert.SerializeObject(result);

                                    yoAppResponse.Narrative = token;

                                    Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);
                                    Log.StoreData("tokens", serviceProvider, result);

                                    tokenFile = LoadMetBankJson(file);
                                }
                                else
                                {
                                    yoAppResponse.ResponseCode = "00008";
                                    yoAppResponse.Description = "Code could not be generated";
                                    yoAppResponse.Note = "Transaction Failed";

                                    Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);

                                    return yoAppResponse;
                                }
                            }

                            if (isTokenValid)
                            {
                                tokenFile = LoadMetBankJson(file);

                                if (response != null)
                                {
                                   // var parts = narrative.Information1.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                    //var parts2 = narrative.Information2.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                                    registerClientRequest.firstName = narrative.ReceiversName;
                                    registerClientRequest.lastName = narrative.ReceiversSurname;
                                    registerClientRequest.email = narrative.LocationCode;
                                    registerClientRequest.phoneNumber = narrative.ReceiverMobile;
                                    registerClientRequest.idPhotoPath = "na";
                                    registerClientRequest.photoWithIdPath = "na";
                                    registerClientRequest.gender = narrative.ReceiversGender;
                                    registerClientRequest.nationalId = narrative.ReceiversIdentification;
                                    registerClientRequest.street = narrative.Information2;
                                    registerClientRequest.suburb = narrative.ServiceRegion;
                                    registerClientRequest.city = narrative.ServiceProvince;
                                    registerClientRequest.country = narrative.ServiceCountry;
                                    var date = DateTime.ParseExact(narrative.Information1, "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture);
                                    registerClientRequest.dateOfBirth = date.ToString("yyyy-mm-dd");
                                    registerClientRequest.agentId = (int)tokenFile.agentId;
                                    registerClientRequest.idTypeName = "National Id";
                                    tokenFile = LoadMetBankJson(file);

                                    Log.RequestsAndResponses("ClientRegistrationRequest", serviceProvider, registerClientRequest);

                                    var client = merchantBankConnector.RegisterClient(serviceProvider, registerClientRequest, tokenFile.access_token); // metBankCredentials.AccessToken

                                    Log.RequestsAndResponses("ClientRegistrationResponse", serviceProvider, client);

                                    if (!string.IsNullOrEmpty(client.status))
                                    {
                                        yoAppResponse.ResponseCode = "00000";
                                        yoAppResponse.Description = "Client Successfully Registered!";
                                        yoAppResponse.Note = "Success";

                                        //var dateCreatedResponse = DateTime.ParseExact(client.dateCreated, "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture);
                                       // var convertedCreatedDate = dateCreatedResponse.ToString("dd/mm/yyyy");
                                        narrative.DateCreated = DateTime.Now.Date;

                                        //var dateModifiedResponse = DateTime.ParseExact(client.dateModified, "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture);
                                        //var convertedModifiedDate = dateModifiedResponse.ToString("dd/mm/yyyy");
                                        narrative.DatelastAccess = DateTime.Now.Date;

                                        narrative.IsActive = client.deleted;
                                        narrative.Status = client.status;
                                        yoAppResponse.CustomerAccount = client.id.ToString(); // Customer Account
                                        narrative.ReceiversName = client.firstName;
                                        narrative.ReceiversSurname = client.lastName;
                                        narrative.CustomerName = client.email;
                                        narrative.ReceiverMobile = client.phoneNumber;
                                        narrative.ReceiversIdentification = client.nationalId;
                                        narrative.ReceiversGender = client.gender;
                                        narrative.ServiceCountry = client.country;
                                        narrative.ServiceAgentId = client.userId.ToString();
                                        yoAppResponse.Narrative = JsonConvert.SerializeObject(narrative);

                                        Log.RequestsAndResponses("YoAppClientRegistrationResponse", serviceProvider, yoAppResponse);

                                        return yoAppResponse;
                                    }
                                    else
                                    {
                                        yoAppResponse.ResponseCode = "00008";
                                        yoAppResponse.Note = "Failed";
                                        yoAppResponse.Description = "Failed to create client in the Met Remit Service. See log for details.";

                                        Log.RequestsAndResponses("YoAppClientRegistrationResponse", serviceProvider, yoAppResponse);

                                        return yoAppResponse;
                                    }
                                }
                                else
                                {
                                    yoAppResponse.ResponseCode = "00008";
                                    yoAppResponse.Note = "Failed";
                                    yoAppResponse.Description = "Received Nothing for the Response from YoApp";

                                    Log.RequestsAndResponses("YoAppClientRegistrationResponse", serviceProvider, yoAppResponse);

                                    return yoAppResponse;
                                }
                            }
                            else
                            {
                                yoAppResponse.ResponseCode = "00008";
                                yoAppResponse.Description = "Token is not Valid";
                                yoAppResponse.Note = "Request Failed";

                                Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);

                                return yoAppResponse;
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.HttpError("Exception", serviceProvider, "Message: " + ex.Message + ", InnerException: " + ex.InnerException + ", StackTrace: " + ex.StackTrace);

                            yoAppResponse.ResponseCode = "00008";
                            yoAppResponse.Description = "There has been a fatal error from the server in trying to send the money. See Log for details";

                            return yoAppResponse;
                        }

                    #endregion

                    case 8: // Recipient Registration

                        #region Register Recipient

                        try
                        {
                            string file = HttpContext.Current.Server.MapPath("~/App_Data/" + serviceProvider + "/Files/" + fileName + ".json");
                            var tokenFile = LoadMetBankJson(file);

                            if (tokenFile != null) // We have generated the Token Already
                            {
                                var expiryDateString = tokenFile.expires_in;

                                var expiryDate = DateTime.ParseExact(expiryDateString, "dd/MM/yyyy-HH:mm:ss", null);

                                var currentDateTime = DateTime.Now;

                                if (expiryDate > currentDateTime) // Token is still valid
                                {
                                    isTokenValid = true;
                                }
                                else // Token is no longer valid
                                {
                                    //RefreshMetBankToken(tokenFile.refresh_token);
                                    GenerateMetBankToken();
                                    isTokenValid = true;

                                    //tokenFile = LoadMetBankJson(file);

                                    //tokenFile = metBankCredentials.AccessToken;
                                }
                            }
                            else // Generate a new Token
                            {
                                userLogin.password = metBankCredentials.Password;
                                userLogin.username = metBankCredentials.Username;
                                userLogin.clientSecret = metBankCredentials.ClientSecret;
                                userLogin.clientId = metBankCredentials.ClientId; 

                                Log.RequestsAndResponses("MetBankTokenRequest", serviceProvider, userLogin);

                                var result = merchantBankConnector.GetToken(serviceProvider, userLogin);

                                Log.RequestsAndResponses("MetBankTokenResponse", serviceProvider, result);

                                if (result.token_type.ToLower() == "bearer")
                                {
                                    isTokenValid = true;
                                    yoAppResponse.ResponseCode = "00000";
                                    yoAppResponse.Description = "Token generated successfully";
                                    yoAppResponse.Note = "Transaction Successful";

                                    var expDate = DateTime.Now.AddSeconds(Convert.ToDouble(result.expires_in));

                                    result.expires_in = expDate.ToString("dd/MM/yyyy-HH:mm:ss");

                                    var token = JsonConvert.SerializeObject(result);

                                    yoAppResponse.Narrative = token;

                                    Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);
                                    Log.StoreData("tokens", serviceProvider, result);

                                    tokenFile = LoadMetBankJson(file);
                                }
                                else
                                {
                                    yoAppResponse.ResponseCode = "00008";
                                    yoAppResponse.Description = "Code could not be generated";
                                    yoAppResponse.Note = "Transaction Failed";

                                    Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);

                                    return yoAppResponse;
                                }
                            }

                            if (isTokenValid)
                            {
                                recipientsRequest.clientId = Convert.ToInt32(narrative.ProviderAccountNumber);
                                recipientsRequest.nationalId = narrative.ReceiversIdentification;
                                recipientsRequest.phoneNumber = narrative.ReceiverMobile;
                                recipientsRequest.firstName = narrative.ReceiversName;
                                recipientsRequest.lastName = narrative.ReceiversSurname;
                                recipientsRequest.gender = narrative.ReceiversGender;
                                recipientsRequest.relationship = narrative.Information1;
                                recipientsRequest.countryId = "1";
                                recipientsRequest.address = narrative.Information2;

                                tokenFile = LoadMetBankJson(file);

                                Log.RequestsAndResponses("ReceipientsRegistrationRequest", serviceProvider, recipientsRequest);

                                var recipient = merchantBankConnector.RegisterRecipient(serviceProvider, recipientsRequest, tokenFile.access_token); //metBankCredentials.AccessToken

                                Log.RequestsAndResponses("ReceipientsRegistrationResponse", serviceProvider, recipient);

                                if (!string.IsNullOrEmpty(recipient.firstName) && !string.IsNullOrEmpty(recipient.lastName))
                                {
                                    yoAppResponse.ResponseCode = "00000";
                                    yoAppResponse.Description = "Recipient Successfully Created!";
                                    yoAppResponse.Note = "Success";

                                    //var dateCreatedResponse = DateTime.ParseExact(recipient.dateCreated, "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture);
                                    //var convertedCreatedDate = dateCreatedResponse.ToString("dd/mm/yyyy");
                                    narrative.DateCreated = DateTime.Now.Date;

                                    //var dateModifiedResponse = DateTime.ParseExact(recipient.dateModified, "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture);
                                    //var convertedModifiedDate = dateModifiedResponse.ToString("dd/mm/yyyy");
                                    narrative.DatelastAccess = DateTime.Now.Date;

                                    narrative.IsActive = recipient.deleted;
                                    narrative.Status = recipient.version.ToString();
                                    narrative.CustomerId = recipient.id.ToString();
                                    narrative.ReceiversName = recipient.firstName;
                                    narrative.ReceiversSurname = recipient.lastName;
                                    narrative.ResponseDescription = recipient.relationship;
                                    narrative.ReceiverMobile = recipient.phoneNumber;
                                    narrative.ReceiversIdentification = recipient.nationalId;
                                    narrative.ReceiversGender = recipient.gender;
                                    narrative.ServiceCountry = recipient.countryName;
                                    yoAppResponse.CustomerAccount = recipient.clientId.ToString();
                                    narrative.Information1 = recipient.address;
                                    narrative.Information2 = recipient.countryId;

                                    yoAppResponse.Narrative = JsonConvert.SerializeObject(narrative);

                                    Log.RequestsAndResponses("YoAppReceipientRegistrationResponse", serviceProvider, yoAppResponse);

                                    return yoAppResponse;
                                }
                                else
                                {
                                    yoAppResponse.ResponseCode = "00008";
                                    yoAppResponse.Description = "Recipient not Created! See log for details";
                                    yoAppResponse.Note = "Failed";

                                    Log.RequestsAndResponses("YoAppReceipientRegistrationResponse", serviceProvider, yoAppResponse);

                                    return yoAppResponse;
                                }
                            }
                            else
                            {
                                yoAppResponse.ResponseCode = "00008";
                                yoAppResponse.Description = "Token is not Valid";
                                yoAppResponse.Note = "Request Failed";

                                Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);

                                return yoAppResponse;
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.HttpError("Exception", serviceProvider, "Message: " + ex.Message + ", InnerException: " + ex.InnerException + ", StackTrace: " + ex.StackTrace);

                            yoAppResponse.ResponseCode = "00008";
                            yoAppResponse.Description = "There has been a fatal error from the server in trying to send the money. See Log for detaild";

                            return yoAppResponse;
                        }

                        #endregion
                       
                    case 9: // Store Voucher and Redeem

                        #region Store Voucher and Redeem

                        switch (response.ProcessingCode)
                        {
                            case "330000":

                                #region Valid OTP Request

                                yoAppResponse.ResponseCode = "00000";
                                yoAppResponse.Description = "Customer to submit their Voucher";
                                yoAppResponse.Note = "Authorise";
                                yoAppResponse.Quantity = 0;
                                yoAppResponse.CustomerData = "Valid Metbank OTP Request";

                                Log.RequestsAndResponses("YoAppOTPResponse", serviceProvider, yoAppResponse);

                                return yoAppResponse;

                            #endregion

                            case "340000": // Store Voucher

                                #region Actual Authentication
                                try
                                {
                                    var userPhoneNumber = response.CustomerMSISDN;

                                    if (response.Mpin != null) // means voucher has been submitted
                                    {
                                        Log.StoreMpin(userPhoneNumber, serviceProvider, response);

                                        yoAppResponse.ResponseCode = "00000";
                                        yoAppResponse.Description = "Mpin saved successfully";
                                        yoAppResponse.Note = "Authorised";

                                        Log.RequestsAndResponses("YoAppStoreMpinResponse", serviceProvider, yoAppResponse);

                                        return yoAppResponse;
                                    }
                                    else if (response.Note != null)
                                    {
                                        response.Mpin = response.Note;

                                        Log.StoreMpin(userPhoneNumber, serviceProvider, response);

                                        yoAppResponse.ResponseCode = "00000";
                                        yoAppResponse.Description = "Mpin saved successfully";
                                        yoAppResponse.Note = "Authorised";

                                        Log.RequestsAndResponses("YoAppStoreMpinResponse", serviceProvider, yoAppResponse);

                                        return yoAppResponse;
                                    }
                                    else if (response.CustomerData != null)
                                    {
                                        response.Mpin = response.CustomerData;

                                        Log.StoreMpin(userPhoneNumber, serviceProvider, response);

                                        yoAppResponse.ResponseCode = "00000";
                                        yoAppResponse.Description = "Mpin saved successfully";
                                        yoAppResponse.Note = "Authorised";

                                        Log.RequestsAndResponses("YoAppStoreMpinResponse", serviceProvider, yoAppResponse);

                                        return yoAppResponse;
                                    }
                                    else
                                    {
                                        yoAppResponse.ResponseCode = "00008";
                                        yoAppResponse.Description = "Did not submit Mpin";

                                        Log.RequestsAndResponses("YoAppStoreMpinResponse", serviceProvider, yoAppResponse);

                                        return yoAppResponse;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    var exceptionMessage = "Message: " + ex.Message + ", InnerException: " + ex.InnerException + ", StackTrace: " + ex.StackTrace;

                                    Log.HttpError("Exception", serviceProvider, exceptionMessage);

                                    yoAppResponse.ResponseCode = "00008";
                                    yoAppResponse.Description = "There has been a fatal error from the server in trying to send the money. See Log for detaild";

                                    return yoAppResponse;
                                }
                            #endregion

                            case "320000": // Push Voucher for Receive Money

                                #region Actual redemption
                                try
                                {
                                    string file = HttpContext.Current.Server.MapPath("~/App_Data/" + serviceProvider + "/Files/" + fileName + ".json");
                                    var tokenFile = LoadMetBankJson(file);

                                    if (tokenFile != null) // We have generated the Token Already
                                    {
                                        var expiryDateString = tokenFile.expires_in;

                                        var expiryDate = DateTime.ParseExact(expiryDateString, "dd/MM/yyyy-HH:mm:ss", null);

                                        var currentDateTime = DateTime.Now;

                                        if (expiryDate > currentDateTime) // Token is still valid
                                        {
                                            isTokenValid = true;
                                        }
                                        else // Token is no longer valid
                                        {
                                            //RefreshMetBankToken(tokenFile.refresh_token);
                                            GenerateMetBankToken();
                                            isTokenValid = true;

                                            //tokenFile = LoadMetBankJson(file);

                                            //tokenFile = metBankCredentials.AccessToken;
                                        }
                                    }
                                    else // Generate a new Token
                                    {
                                        userLogin.password = metBankCredentials.Password;
                                        userLogin.username = metBankCredentials.Username;
                                        userLogin.clientSecret = metBankCredentials.ClientSecret;
                                        userLogin.clientId = metBankCredentials.ClientId;

                                        Log.RequestsAndResponses("MetBankTokenRequest", serviceProvider, userLogin);

                                        var result = merchantBankConnector.GetToken(serviceProvider, userLogin);

                                        Log.RequestsAndResponses("MetBankTokenResponse", serviceProvider, result);

                                        if (result.token_type.ToLower() == "bearer")
                                        {
                                            isTokenValid = true;
                                            yoAppResponse.ResponseCode = "00000";
                                            yoAppResponse.Description = "Token generated successfully";
                                            yoAppResponse.Note = "Transaction Successful";

                                            var expDate = DateTime.Now.AddSeconds(Convert.ToDouble(result.expires_in));

                                            result.expires_in = expDate.ToString("dd/MM/yyyy-HH:mm:ss");

                                            var token = JsonConvert.SerializeObject(result);

                                            yoAppResponse.Narrative = token;

                                            Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);
                                            Log.StoreData("tokens", serviceProvider, result);

                                            tokenFile = LoadMetBankJson(file);
                                        }
                                        else
                                        {
                                            yoAppResponse.ResponseCode = "00008";
                                            yoAppResponse.Description = "Code could not be generated";
                                            yoAppResponse.Note = "Transaction Failed";

                                            Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);

                                            return yoAppResponse;
                                        }
                                    }

                                    if (isTokenValid)
                                    {
                                        string mPinFile = HttpContext.Current.Server.MapPath("~/App_Data/" + serviceProvider + "/Files/" + response.CustomerMSISDN + ".json");

                                        var mpinFile = LoadMpin(mPinFile, serviceProvider);

                                        tokenFile = LoadMetBankJson(file);

                                        // Receive Money PreAuth Request
                                        ReceiveMoneyPreAuthRequest receiveMoneyPreAuthRequest = new ReceiveMoneyPreAuthRequest();

                                        if (mpinFile != null)
                                        {
                                            receiveMoneyPreAuthRequest.agentId = (long)tokenFile.agentId;
                                            receiveMoneyPreAuthRequest.voucherNumber = mpinFile.Mpin;
                                            receiveMoneyPreAuthRequest.tellerId = "154";

                                            Log.RequestsAndResponses("ReceiveMoneyPreAuthRequest", serviceProvider, receiveMoneyPreAuthRequest);

                                            var receiveMoneyPreAuthResponse = merchantBankConnector.PreAuthReceiveMoney(serviceProvider, receiveMoneyPreAuthRequest, tokenFile.access_token); // metBankCredentials.AccessToken

                                            Log.RequestsAndResponses("ReceiveMoneyPreAuthResponse", serviceProvider, receiveMoneyPreAuthResponse);

                                            if (!string.IsNullOrEmpty(receiveMoneyPreAuthResponse.preauthId))
                                            {
                                                receiveMoneyRequest.tellerId = 154;
                                                receiveMoneyRequest.agentId = (int)tokenFile.agentId;
                                                receiveMoneyRequest.preauthId = receiveMoneyPreAuthResponse.preauthId;
                                                receiveMoneyRequest.voucherNumber = receiveMoneyPreAuthRequest.voucherNumber;
                                                tokenFile = LoadMetBankJson(file);

                                                Log.RequestsAndResponses("ReceiveMoneyRequest", serviceProvider, receiveMoneyRequest);

                                                var transactionResponse = merchantBankConnector.ReceiveMoney(serviceProvider, receiveMoneyRequest, tokenFile.access_token);

                                                Log.RequestsAndResponses("ReceiveMoneyResponse", serviceProvider, transactionResponse);

                                                if (!string.IsNullOrEmpty(transactionResponse.status))
                                                {
                                                    yoAppResponse.ResponseCode = "00000";
                                                    yoAppResponse.Description = "Money has been received successfully";
                                                    yoAppResponse.Note = "Authorised";
                                                    yoAppResponse.Description = transactionResponse.description;
                                                    yoAppResponse.TransactionRef = transactionResponse.transactionReference;
                                                    yoAppResponse.Amount = decimal.Parse(transactionResponse.amount.Trim(), System.Globalization.CultureInfo.InvariantCulture);
                                                    yoAppResponse.Balance = transactionResponse.fees;
                                                    yoAppResponse.Note = transactionResponse.status;
                                                    yoAppResponse.Currency = transactionResponse.collectionCurrencyCode;
                                                    yoAppResponse.Narrative = JsonConvert.SerializeObject(narrative);

                                                    Log.RequestsAndResponses("YoAppReceiveMoneyResponse", serviceProvider, transactionResponse);

                                                    return yoAppResponse;
                                                }
                                                else
                                                {
                                                    yoAppResponse.ResponseCode = "00008";
                                                    yoAppResponse.Note = "Failed";
                                                    yoAppResponse.Description = "There was an error on the receive money request";

                                                    Log.RequestsAndResponses("YoAppReceiveMoneyResponse", serviceProvider, yoAppResponse);

                                                    return yoAppResponse;
                                                }
                                            }
                                            else
                                            {
                                                yoAppResponse.ResponseCode = "00008";
                                                yoAppResponse.Note = "Failed";
                                                yoAppResponse.Description = "Failed to receive a PreAuth Id from the server";

                                                Log.RequestsAndResponses("YoAppReceiveMoneyPreAuthIdRequest", serviceProvider, yoAppResponse);

                                                return yoAppResponse;
                                            }
                                        }
                                        else
                                        {
                                            yoAppResponse.ResponseCode = "00008";
                                            yoAppResponse.Note = "Failed";
                                            yoAppResponse.Description = "Could not retrieve the voucher saved";

                                            Log.RequestsAndResponses("YoAppReceiveMoneyResponse", serviceProvider, yoAppResponse);

                                            return yoAppResponse;
                                        }
                                    }
                                    else
                                    {
                                        yoAppResponse.ResponseCode = "00008";
                                        yoAppResponse.Note = "Failed to generate token";
                                        yoAppResponse.Description = "The Token is Invalid";

                                        Log.RequestsAndResponses("YoAppReceiveMoneyResponse", serviceProvider, yoAppResponse);

                                        return yoAppResponse;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    var exceptionMessage = "Message: " + ex.Message + ", InnerException: " + ex.InnerException + ", StackTrace: " + ex.StackTrace;

                                    Log.HttpError("Exception", serviceProvider, exceptionMessage);

                                    yoAppResponse.ResponseCode = "00008";
                                    yoAppResponse.Description = "There has been a fatal error from the server in trying to send the money. See Log for detaild";

                                   // Log.RequestsAndResponses("YoAppReceiveMoneyResponse", serviceProvider, yoAppResponse);

                                    return yoAppResponse;
                                }
                            #endregion

                            default:
                                break;
                        }

                        #endregion

                        break;

                    case 10: // Send Money Preauth Request, Store Send Money Request, Push Fees to YoApp

                        #region SendMoney Preauth Request and Store Send Money Request Section

                        try
                        {
                            string file = HttpContext.Current.Server.MapPath("~/App_Data/" + serviceProvider + "/Files/" + fileName + ".json");
                            var tokenFile = LoadMetBankJson(file);

                            if (tokenFile != null) // We have generated the Token Already
                            {
                                var expiryDateString = tokenFile.expires_in;

                                var expiryDate = DateTime.ParseExact(expiryDateString, "dd/MM/yyyy-HH:mm:ss", null);

                                var currentDateTime = DateTime.Now;

                                if (expiryDate > currentDateTime) // Token is still valid
                                {
                                    isTokenValid = true;
                                }
                                else // Token is no longer valid
                                {
                                    //RefreshMetBankToken(tokenFile.refresh_token);

                                    GenerateMetBankToken();
                                    isTokenValid = true;

                                    //tokenFile = LoadMetBankJson(file);

                                    //tokenFile = metBankCredentials.AccessToken;
                                }
                            }
                            else // Generate a new Token
                            {
                                userLogin.password = metBankCredentials.Password;
                                userLogin.username = metBankCredentials.Username;
                                userLogin.clientSecret = metBankCredentials.ClientSecret;
                                userLogin.clientId = metBankCredentials.ClientId;

                                Log.RequestsAndResponses("MetBankTokenRequest", serviceProvider, userLogin);

                                var result = merchantBankConnector.GetToken(serviceProvider, userLogin);

                                Log.RequestsAndResponses("MetBankTokenResponse", serviceProvider, result);

                                if (result.token_type.ToLower() == "bearer")
                                {
                                    isTokenValid = true;
                                    yoAppResponse.ResponseCode = "00000";
                                    yoAppResponse.Description = "Token generated successfully";
                                    yoAppResponse.Note = "Transaction Successful";

                                    var expDate = DateTime.Now.AddSeconds(Convert.ToDouble(result.expires_in));

                                    result.expires_in = expDate.ToString("dd/MM/yyyy-HH:mm:ss");

                                    var token = JsonConvert.SerializeObject(result);

                                    yoAppResponse.Narrative = token;

                                    Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);
                                    Log.StoreData("tokens", serviceProvider, result);

                                    tokenFile = LoadMetBankJson(file);
                                }
                                else
                                {
                                    yoAppResponse.ResponseCode = "00008";
                                    yoAppResponse.Description = "Code could not be generated";
                                    yoAppResponse.Note = "Transaction Failed";

                                    Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);

                                    return yoAppResponse;
                                }
                            }

                            if (isTokenValid)
                            {
                                tokenFile = LoadMetBankJson(file);

                                // Send Money PreAuth Transaction
                                sendMoneyPreAuthRequest.amountSend = (decimal)narrative.Balance;
                                sendMoneyPreAuthRequest.sourceCountryCode = narrative.SourceCountry.Trim();
                                sendMoneyPreAuthRequest.destinationCountryCode = narrative.ServiceCountry.Trim();
                                sendMoneyPreAuthRequest.currencyCodeSend = narrative.Currency.Trim();
                                sendMoneyPreAuthRequest.clientId = Convert.ToInt32(narrative.ProviderAccountNumber.Trim());
                                sendMoneyPreAuthRequest.agentId = (int)tokenFile.agentId;
                                sendMoneyPreAuthRequest.collectionAmount = (decimal)narrative.Balance;
                                sendMoneyPreAuthRequest.collectionCurrencyCode = narrative.Currency.Trim();

                                if (narrative.Currency.ToUpper().Trim() == "ZWL")
                                {
                                    sendMoneyPreAuthRequest.currencyCodeSend = "USD";
                                    sendMoneyPreAuthRequest.collectionCurrencyCode = "USD";
                                }

                                sendMoneyPreAuthRequest.recipientId = Convert.ToInt32(narrative.ReceiverProviderAccountNumber.Trim());
                                sendMoneyPreAuthRequest.tellerId = 154;
                                sendMoneyPreAuthRequest.reasonForTransfer = narrative.Information1.Trim();
                                tokenFile = LoadMetBankJson(file);

                                Log.RequestsAndResponses("PreAuthSendMoneyRequest", serviceProvider, sendMoneyPreAuthRequest);

                                var sendMoneyPreAuthResponse = merchantBankConnector.PreAuthSendMoney(serviceProvider, sendMoneyPreAuthRequest, tokenFile.access_token);

                                Log.RequestsAndResponses("PreAuthSendMoneyResponse", serviceProvider, sendMoneyPreAuthResponse);

                                if (!string.IsNullOrEmpty(sendMoneyPreAuthResponse.preauthId))
                                {
                                    sendMoneyRequest.amountSend = sendMoneyPreAuthRequest.amountSend;
                                    sendMoneyRequest.sourceCountryCode = sendMoneyPreAuthRequest.sourceCountryCode;
                                    sendMoneyRequest.destinationCountryCode = sendMoneyPreAuthRequest.destinationCountryCode;
                                    sendMoneyRequest.currencyCodeSend = sendMoneyPreAuthRequest.currencyCodeSend;
                                    sendMoneyRequest.clientId = sendMoneyPreAuthRequest.clientId;
                                    sendMoneyRequest.agentId = sendMoneyPreAuthRequest.agentId;
                                    sendMoneyRequest.collectionAmount = sendMoneyPreAuthRequest.collectionAmount;
                                    sendMoneyRequest.collectionCurrencyCode = sendMoneyPreAuthRequest.collectionCurrencyCode;
                                    sendMoneyRequest.recipientId = sendMoneyPreAuthRequest.recipientId;
                                    sendMoneyRequest.reasonForTransfer = sendMoneyPreAuthRequest.reasonForTransfer;
                                    sendMoneyRequest.tellerId = sendMoneyPreAuthRequest.tellerId;
                                    sendMoneyRequest.preauthId = sendMoneyPreAuthResponse.preauthId;
                                    tokenFile = LoadMetBankJson(file);

                                    #region Store Send Money Request with PreAuthId

                                    var senderPhoneNumber = narrative.CustomerMobileNumber;

                                    if (!string.IsNullOrEmpty(senderPhoneNumber))
                                    {
                                        Log.StoreMpin(senderPhoneNumber, serviceProvider, sendMoneyRequest);

                                        yoAppResponse.ResponseCode = "00000";
                                        yoAppResponse.Description = sendMoneyPreAuthResponse.description + ". Send Money request saved successfully";
                                        yoAppResponse.Narrative = "Service charge: USD $" + sendMoneyPreAuthResponse.fees + ", " +
                                                                  "Amount to be sent: USD $" + sendMoneyPreAuthResponse.amount + ", " +
                                                                  "Total Amount: USD $" + sendMoneyPreAuthResponse.totalAmount;

                                        Log.RequestsAndResponses("YoAppStoreSendMoneyResponse", serviceProvider, yoAppResponse);

                                        return yoAppResponse;
                                    }

                                    #endregion

                                    #region Send Money Request Commented Out
                                    //Log.RequestsAndResponses("SendMoneyRequest", serviceProvider, sendMoneyRequest);

                                    //var transactionResponse = merchantBankConnector.SendMoney(serviceProvider, sendMoneyRequest, tokenFile.access_token);

                                    //Log.RequestsAndResponses("SendMoneyResponse", serviceProvider, transactionResponse);

                                    //if (transactionResponse.status.ToUpper() == "COMPLETE")
                                    //{
                                    //    yoAppResponse.ResponseCode = "00000";
                                    //    yoAppResponse.Note = transactionResponse.transactionId.ToString();
                                    //    yoAppResponse.Description = transactionResponse.description;
                                    //    yoAppResponse.TransactionRef = transactionResponse.transactionReference;
                                    //    yoAppResponse.Amount = decimal.Parse(transactionResponse.amount, System.Globalization.CultureInfo.InvariantCulture);
                                    //    yoAppResponse.Balance = transactionResponse.fees;
                                    //    yoAppResponse.Note = transactionResponse.status;
                                    //    yoAppResponse.Currency = transactionResponse.collectionCurrencyCode;
                                    //    yoAppResponse.Narrative = JsonConvert.SerializeObject(narrative);

                                    //    Log.RequestsAndResponses("YoAppSendMoneyResponse", serviceProvider, yoAppResponse);

                                    //    return yoAppResponse;
                                    //}
                                    //else
                                    //{
                                    //    yoAppResponse.ResponseCode = "00008";
                                    //    yoAppResponse.Note = "Failed";
                                    //    yoAppResponse.Description = "Money was not sent! See log for details.";

                                    //    Log.RequestsAndResponses("YoAppSendMoneyResponse", serviceProvider, yoAppResponse);

                                    //    return yoAppResponse;
                                    //} 
                                    #endregion
                                }
                            }
                            else
                            {
                                yoAppResponse.ResponseCode = "00008";
                                yoAppResponse.Description = "Token is not Valid";
                                yoAppResponse.Note = "Request Failed";

                                Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);

                                return yoAppResponse;
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.HttpError("Exception", serviceProvider, "Message: " + ex.Message + ", InnerException: " + ex.InnerException + ", StackTrace: " + ex.StackTrace);

                            yoAppResponse.ResponseCode = "00008";
                            yoAppResponse.Description = "There has been a fatal error from the server in trying to send the money. See Log for details";

                            return yoAppResponse;
                        }

                        #endregion

                        break;

                    case 11:

                        #region Search for Client and Recipients at once

                        try
                        {
                            string file = HttpContext.Current.Server.MapPath("~/App_Data/" + serviceProvider + "/Files/" + fileName + ".json");

                            var tokenFile = LoadMetBankJson(file);

                            if (tokenFile != null) // We have generated the Token Already
                            {
                                var expiryDateString = tokenFile.expires_in;

                                var expiryDate = DateTime.ParseExact(expiryDateString, "dd/MM/yyyy-HH:mm:ss", null);

                                var currentDateTime = DateTime.Now;

                                if (expiryDate > currentDateTime) // Token is still valid
                                {
                                    isTokenValid = true;
                                }
                                else // Token is no longer valid
                                {
                                    // RefreshMetBankToken(tokenFile.refresh_token);
                                    GenerateMetBankToken();
                                    isTokenValid = true;

                                    //tokenFile = LoadMetBankJson(file);

                                    //tokenFile = metBankCredentials.AccessToken;
                                }
                            }
                            else // Generate a new Token
                            {
                                userLogin.password = metBankCredentials.Password;
                                userLogin.username = metBankCredentials.Username;
                                userLogin.clientSecret = metBankCredentials.ClientSecret;
                                userLogin.clientId = metBankCredentials.ClientId;

                                Log.RequestsAndResponses("MetBankTokenRequest", serviceProvider, userLogin);

                                var result = merchantBankConnector.GetToken(serviceProvider, userLogin);

                                Log.RequestsAndResponses("MetBankTokenResponse", serviceProvider, result);

                                if (result.token_type.ToLower() == "bearer")
                                {
                                    isTokenValid = true;
                                    yoAppResponse.ResponseCode = "00000";
                                    yoAppResponse.Description = "Token generated successfully";
                                    yoAppResponse.Note = "Transaction Successful";

                                    var expDate = DateTime.Now.AddSeconds(Convert.ToDouble(result.expires_in));

                                    result.expires_in = expDate.ToString("dd/MM/yyyy-HH:mm:ss");

                                    var token = JsonConvert.SerializeObject(result);

                                    yoAppResponse.Narrative = token;

                                    Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);
                                    Log.StoreData("tokens", serviceProvider, result);

                                    tokenFile = LoadMetBankJson(file);
                                }
                                else
                                {
                                    yoAppResponse.ResponseCode = "00008";
                                    yoAppResponse.Description = "Code could not be generated";
                                    yoAppResponse.Note = "Transaction Failed";

                                    Log.RequestsAndResponses("Wafaya-TokenResponse-YoApp", serviceProvider, yoAppResponse);

                                    return yoAppResponse;
                                }
                            }

                            if (isTokenValid)
                            {
                                if (response.CustomerAccount != null) // Get Client Details Via ClientId or CustomerMobileNumber
                                {
                                    if (response.Note.ToUpper() == "CUSTOMERMOBILENUMBER") // Client Phone Number
                                    {
                                        ClientRequest clientRequest = new ClientRequest();
                                        List<Narrative> narratives = new List<Narrative>();
                                        List<Narrative> narratives1 = new List<Narrative>();

                                        clientRequest.phoneNumber = response.CustomerAccount;
                                        tokenFile = LoadMetBankJson(file);

                                        Log.RequestsAndResponses("ClientDetailsByMobileRequest", serviceProvider, clientRequest);

                                        var clientResponse = merchantBankConnector.GetClientByPhoneNumber(serviceProvider, clientRequest, tokenFile.access_token); // metBankCredentials.AccessToken

                                        Log.RequestsAndResponses("ClientDetailsByMobileResponse", serviceProvider, clientResponse);

                                        if (clientResponse.clientFound)
                                        {
                                             #region Commented Client Details Response | Capturing Client Details
                                            //List<Narrative> narratives = new List<Narrative>();

                                            Narrative narrative2 = new Narrative();

                                            yoAppResponse.ResponseCode = "00000";
                                            yoAppResponse.Description = "Client Found!";
                                            narrative2.DateCreated = DateTime.ParseExact(clientResponse.client.dateCreated, "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture);
                                            narrative2.DatelastAccess = DateTime.ParseExact(clientResponse.client.dateModified, "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture);
                                            //narrative.IsActive = clientResponse.client.deleted;

                                            if (clientResponse.client.deleted)
                                            {
                                                narrative2.IsActive = false;
                                            }
                                            else
                                            {
                                                narrative2.IsActive = true;
                                            }

                                            narrative2.CustomerName = clientResponse.client.firstName + " " + clientResponse.client.lastName;
                                            narrative2.ProviderAccountNumber = clientResponse.client.id.ToString();
                                            narrative2.CustomerMobileNumber = clientResponse.client.phoneNumber;

                                            narrative2.ServiceRegion = "{Status: " + clientResponse.client.status + ", National Id Number: " + clientResponse.client.nationalId + ", Gender: " +
                                                clientResponse.client.gender + ",UserId: " + clientResponse.client.userId.ToString() + ",Country: " + clientResponse.client.country + ",Email:" + clientResponse.client.email;

                                            //narrative.Id = (long)clientResponse.client.id;
                                            //narrative.ReceiversName = clientResponse.client.firstName;
                                            //narrative.ReceiversSurname = clientResponse.client.lastName;
                                            //yoAppResponse.Note = clientResponse.client.email;

                                            //narrative.ReceiversIdentification = clientResponse.client.nationalId;
                                            //narrative.ReceiversGender = clientResponse.client.gender;
                                            //narrative.ServiceCountry = clientResponse.client.country;
                                            //narrative.CustomerId = clientResponse.client.userId.ToString();

                                            narratives.Add(narrative2);
                                            //yoAppResponse.Narrative = JsonConvert.SerializeObject(narratives);

                                            //Log.RequestsAndResponses("YoAppClientDetailsByMobileResponse", serviceProvider, yoAppResponse); 
                                            #endregion

                                            //ClientRequest clientRequest = new ClientRequest();

                                            clientRequest.clientId = clientResponse.client.id.ToString();
                                            //tokenFile = LoadMetBankJson(file);

                                            Log.RequestsAndResponses("RecipientDetailsByClientIdRequest", serviceProvider, clientRequest);

                                            var recipients = merchantBankConnector.GetRecipientByClientId(serviceProvider, clientRequest, tokenFile.access_token); // metBankCredentials.AccessToken

                                            Log.RequestsAndResponses("RecipientDetailsByClientIdResponse", serviceProvider, recipients);
                                                                                        
                                            foreach (var recipient in recipients)
                                            {
                                                Narrative narrative1 = new Narrative();

                                                if (recipient.firstName != null)
                                                {
                                                    yoAppResponse.ResponseCode = "00000";
                                                    yoAppResponse.Description = "Recipients Found!";
                                                    narrative1.DateCreated = DateTime.ParseExact(recipient.dateCreated, "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture);
                                                    narrative1.DatelastAccess = DateTime.ParseExact(recipient.dateModified, "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture);
                                                    narrative1.IsActive = recipient.deleted;

                                                    if (recipient.deleted)
                                                    {
                                                        narrative1.IsActive = false;
                                                    }
                                                    else
                                                    {
                                                        narrative1.IsActive = true;
                                                    }

                                                    narrative1.Status = recipient.version.ToString();
                                                    narrative1.ReceiverProviderAccountNumber = recipient.id.ToString();
                                                    narrative1.ReceiversName = recipient.firstName;
                                                    narrative1.ReceiversSurname = recipient.lastName;
                                                    //narrative.ReceiversSurname = recipient.lastName;                                                    
                                                    narrative1.ReceiverMobile = recipient.phoneNumber;

                                                    narrative1.Information1 = "{Relationship:" + recipient.relationship + ",NationalId:" + recipient.nationalId + ",Gender:" + recipient.gender +
                                                        ",ClientId: " + recipient.clientId + "}";
                                                    //yoAppResponse.Note = recipient.relationship;
                                                    //narrative.ReceiversIdentification = recipient.nationalId;
                                                    //narrative.ReceiversGender = recipient.gender;
                                                    //narrative.ServiceCountry = recipient.countryId;
                                                    //narrative.Information1 = recipient.countryName;
                                                    //narrative.Information2 = recipient.address;
                                                    //narrative.CustomerId = recipient.clientId.ToString();

                                                    narratives1.Add(narrative1);
                                                }
                                                else
                                                {
                                                    yoAppResponse.ResponseCode = "00008";
                                                    yoAppResponse.Note = "Recipients not Found";
                                                    yoAppResponse.Description = "Received Nothing for the clientId submitted";

                                                    Log.RequestsAndResponses("RecipientDetailsByIdResponse-YoApp", serviceProvider, yoAppResponse);

                                                    return yoAppResponse;
                                                }
                                            }

                                            yoAppResponse.Narrative = JsonConvert.SerializeObject(narratives); // Clients
                                            yoAppResponse.Note = JsonConvert.SerializeObject(narratives1); // Recipients

                                            //Log.RequestsAndResponses("RecipientDetailsByIdResponse-YoApp", serviceProvider, yoAppResponse);
                                            Log.RequestsAndResponses("ClientAndRecipientsDetailsResponse-YoApp", serviceProvider, yoAppResponse);

                                            return yoAppResponse;
                                        }
                                        else
                                        {
                                            yoAppResponse.ResponseCode = "00008";
                                            yoAppResponse.Note = "Failed";
                                            yoAppResponse.Description = "Received Nothing for the Molile Number submitted was wrong";

                                            Log.RequestsAndResponses("YoAppClientDetailsByMobileResponse", serviceProvider, yoAppResponse);

                                            return yoAppResponse;
                                        }
                                    }
                                    else if (response.Note.ToUpper() == "RECEIVERSIDNUMBER")
                                    {
                                        ClientRequest clientRequest = new ClientRequest();

                                        clientRequest.nationalId = narrative.ReceiversIdentification;
                                        tokenFile = LoadMetBankJson(file);

                                        Log.RequestsAndResponses("ClientDetailsByNationalIdRequest", serviceProvider, clientRequest);

                                        var clientResponse = merchantBankConnector.GetClientByNationalId(serviceProvider, clientRequest, metBankCredentials.AccessToken);

                                        Log.RequestsAndResponses("ClientDetailsByNationalIdResponse", serviceProvider, clientResponse);

                                        if (clientResponse.clientFound)
                                        {
                                            List<Narrative> narratives = new List<Narrative>();

                                            yoAppResponse.ResponseCode = "00000";
                                            yoAppResponse.Description = "Clinet Found!";
                                            narrative.DateCreated = DateTime.ParseExact(clientResponse.client.dateCreated, "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture);
                                            narrative.DatelastAccess = DateTime.ParseExact(clientResponse.client.dateModified, "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture);
                                            narrative.IsActive = clientResponse.client.deleted;
                                            narrative.Status = clientResponse.client.status;
                                            narrative.Id = (long)clientResponse.client.id;
                                            narrative.ReceiversName = clientResponse.client.firstName;
                                            narrative.ReceiversSurname = clientResponse.client.lastName;
                                            yoAppResponse.Note = clientResponse.client.email;
                                            narrative.ReceiverMobile = clientResponse.client.phoneNumber;
                                            narrative.ReceiversIdentification = clientResponse.client.nationalId;
                                            narrative.ReceiversGender = clientResponse.client.gender;
                                            narrative.ServiceCountry = clientResponse.client.country;
                                            narrative.CustomerId = clientResponse.client.userId.ToString();

                                            narratives.Add(narrative);
                                            yoAppResponse.Narrative = JsonConvert.SerializeObject(narratives);

                                            Log.RequestsAndResponses("YoAppClientDetailsByNationalIdResponse", serviceProvider, yoAppResponse);

                                            return yoAppResponse;
                                        }
                                        else
                                        {
                                            yoAppResponse.ResponseCode = "00008";
                                            yoAppResponse.Note = "Failed";
                                            yoAppResponse.Description = "Received Nothing for the National Id submitted was wrong";

                                            Log.RequestsAndResponses("YoAppClientDetailsByNationalIdResponse", serviceProvider, yoAppResponse);

                                            return yoAppResponse;
                                        }
                                    }
                                   

                                    #region Search Recipients by ClientId
                                    else if (response.Note.ToUpper() == "RECEIVERPROVIDERACCOUNTNUMBER")
                                    {
                                        ClientRequest clientRequest = new ClientRequest();

                                        clientRequest.clientId = response.CustomerAccount;
                                        tokenFile = LoadMetBankJson(file);

                                        Log.RequestsAndResponses("RecipientDetailsByClientIdRequest", serviceProvider, clientRequest);

                                        var recipients = merchantBankConnector.GetRecipientByClientId(serviceProvider, clientRequest, tokenFile.access_token); // metBankCredentials.AccessToken

                                        Log.RequestsAndResponses("RecipientDetailsByClientIdResponse", serviceProvider, recipients);

                                        List<Narrative> narratives = new List<Narrative>();

                                        foreach (var recipient in recipients)
                                        {
                                            Narrative narrative1 = new Narrative();

                                            if (recipient.firstName != null)
                                            {
                                                yoAppResponse.ResponseCode = "00000";
                                                yoAppResponse.Description = "Recipients Found!";
                                                narrative1.DateCreated = DateTime.ParseExact(recipient.dateCreated, "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture);
                                                narrative1.DatelastAccess = DateTime.ParseExact(recipient.dateModified, "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture);
                                                narrative1.IsActive = recipient.deleted;

                                                if (recipient.deleted)
                                                {
                                                    narrative1.IsActive = false;
                                                }
                                                else
                                                {
                                                    narrative1.IsActive = true;
                                                }

                                                narrative1.Status = recipient.version.ToString();
                                                narrative1.ReceiverProviderAccountNumber = recipient.id.ToString();
                                                narrative1.ReceiversName = recipient.firstName;
                                                narrative1.ReceiversSurname = recipient.lastName;
                                                //narrative.ReceiversSurname = recipient.lastName;                                                    
                                                narrative1.ReceiverMobile = recipient.phoneNumber;

                                                narrative1.Information1 = "{Relationship:" + recipient.relationship + ",NationalId:" + recipient.nationalId + ",Gender:" + recipient.gender +
                                                    ",ClientId: " + recipient.clientId + "}";
                                                //yoAppResponse.Note = recipient.relationship;
                                                //narrative.ReceiversIdentification = recipient.nationalId;
                                                //narrative.ReceiversGender = recipient.gender;
                                                //narrative.ServiceCountry = recipient.countryId;
                                                //narrative.Information1 = recipient.countryName;
                                                //narrative.Information2 = recipient.address;
                                                //narrative.CustomerId = recipient.clientId.ToString();

                                                narratives.Add(narrative1);
                                            }
                                            else
                                            {
                                                yoAppResponse.ResponseCode = "00008";
                                                yoAppResponse.Note = "Recipients not Found";
                                                yoAppResponse.Description = "Received Nothing for the clientId submitted";

                                                Log.RequestsAndResponses("RecipientDetailsByIdResponse-YoApp", serviceProvider, yoAppResponse);

                                                return yoAppResponse;
                                            }
                                        }

                                        yoAppResponse.Narrative = JsonConvert.SerializeObject(narratives);

                                        Log.RequestsAndResponses("RecipientDetailsByIdResponse-YoApp", serviceProvider, yoAppResponse);

                                        return yoAppResponse;
                                    }
                                    #endregion

                                    

                                    else if (response.Note.ToUpper() == "RECEIVERMOBILE")
                                    {
                                        ClientRequest clientRequest = new ClientRequest();

                                        clientRequest.phoneNumber = response.CustomerAccount;
                                        tokenFile = LoadMetBankJson(file);

                                        Log.RequestsAndResponses("RecipientDetailsByPhoneNumberRequest", serviceProvider, clientRequest);

                                        var recipient = merchantBankConnector.GetRecipientByPhoneNumber(serviceProvider, clientRequest, tokenFile.access_token); // metBankCredentials.AccessToken

                                        Log.RequestsAndResponses("RecipientDetailsByPhoneNumberResponse", serviceProvider, recipient);

                                        if (recipient != null && recipient.deleted == false && recipient.firstName != null)
                                        {
                                            List<Narrative> narratives = new List<Narrative>();

                                            yoAppResponse.ResponseCode = "00000";
                                            yoAppResponse.Description = "Recipient Found!";
                                            narrative.DateCreated = DateTime.ParseExact(recipient.dateCreated, "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture);
                                            narrative.DatelastAccess = DateTime.ParseExact(recipient.dateModified, "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture);
                                            narrative.IsActive = recipient.deleted;

                                            if (recipient.deleted)
                                            {
                                                narrative.IsActive = false;
                                            }
                                            else
                                            {
                                                narrative.IsActive = true;
                                            }

                                            narrative.Status = recipient.version.ToString();
                                            narrative.ReceiverProviderAccountNumber = recipient.id.ToString();
                                            narrative.ReceiversName = recipient.firstName;
                                            narrative.ReceiversSurname = recipient.lastName;
                                            //narrative.ReceiversSurname = recipient.lastName;                                                    
                                            narrative.ReceiverMobile = recipient.phoneNumber;

                                            narrative.Information1 = "{Relationship:" + recipient.relationship + ",NationalId:" + recipient.nationalId + ",Gender:" + recipient.gender +
                                                ",ClientId: " + recipient.clientId + "}";
                                            //yoAppResponse.Note = recipient.relationship;
                                            //narrative.ReceiversIdentification = recipient.nationalId;
                                            //narrative.ReceiversGender = recipient.gender;
                                            //narrative.ServiceCountry = recipient.countryId;
                                            //narrative.Information1 = recipient.countryName;
                                            //narrative.Information2 = recipient.address;
                                            //narrative.CustomerId = recipient.clientId.ToString();

                                            narratives.Add(narrative);
                                            yoAppResponse.Narrative = JsonConvert.SerializeObject(narratives);

                                            Log.RequestsAndResponses("YoAppRecipientDetailsByPhoneNumberResponse", serviceProvider, yoAppResponse);

                                            return yoAppResponse;
                                        }
                                        else
                                        {
                                            yoAppResponse.ResponseCode = "00008";
                                            yoAppResponse.Note = "Recipient not Found";
                                            yoAppResponse.Description = "Received Nothing for the clientId submitted";

                                            Log.RequestsAndResponses("YoAppRecipientDetailsByPhoneNumberResponse", serviceProvider, yoAppResponse);

                                            return yoAppResponse;
                                        }
                                    }
                                    else
                                    {
                                        yoAppResponse.ResponseCode = "00008";
                                        yoAppResponse.Note = "Failed";
                                        yoAppResponse.Description = "Received Nothing for the Page Number was not submitted";

                                        Log.RequestsAndResponses("YoAppRecipientDetailsByPhoneNumberResponse", serviceProvider, yoAppResponse);

                                        return yoAppResponse;
                                    }

                                }
                                else
                                {
                                    yoAppResponse.ResponseCode = "00008";
                                    yoAppResponse.Description = "CustomerAccount is null";
                                    yoAppResponse.Note = "Request Failed";

                                    Log.RequestsAndResponses("YoAppResponse-YoApp", serviceProvider, yoAppResponse);

                                    return yoAppResponse;
                                }
                            }
                            else
                            {
                                yoAppResponse.ResponseCode = "00008";
                                yoAppResponse.Description = "Token is not Valid";
                                yoAppResponse.Note = "Request Failed";

                                Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);

                                return yoAppResponse;
                            }

                            //return yoAppResponse;
                        }
                        catch (Exception ex)
                        {
                            Log.HttpError("Exception", serviceProvider, "Message: " + ex.Message + ", InnerException: " + ex.InnerException + ", StackTrace: " + ex.StackTrace);

                            return yoAppResponse;
                        }

                        #endregion                        

                        
                    case 12: // Receive Money Preauth Request, Store Receive Money Request, Push Response to YoApp

                        #region ReceiveMoney Preauth Request and Store Receive Money Request, And Send Money Request

                        try
                        {
                            string file = HttpContext.Current.Server.MapPath("~/App_Data/" + serviceProvider + "/Files/" + fileName + ".json");
                            var tokenFile = LoadMetBankJson(file);

                            if (tokenFile != null) // We have generated the Token Already
                            {
                                var expiryDateString = tokenFile.expires_in;

                                var expiryDate = DateTime.ParseExact(expiryDateString, "dd/MM/yyyy-HH:mm:ss", null);

                                var currentDateTime = DateTime.Now;

                                if (expiryDate > currentDateTime) // Token is still valid
                                {
                                    isTokenValid = true;
                                }
                                else // Token is no longer valid
                                {
                                    //RefreshMetBankToken(tokenFile.refresh_token);
                                    GenerateMetBankToken();
                                    isTokenValid = true;

                                    //tokenFile = LoadMetBankJson(file);

                                    //tokenFile = metBankCredentials.AccessToken;
                                }
                            }
                            else // Generate a new Token
                            {
                                userLogin.password = metBankCredentials.Password;
                                userLogin.username = metBankCredentials.Username;
                                userLogin.clientSecret = metBankCredentials.ClientSecret;
                                userLogin.clientId = metBankCredentials.ClientId;

                                Log.RequestsAndResponses("MetBankTokenRequest", serviceProvider, userLogin);

                                var result = merchantBankConnector.GetToken(serviceProvider, userLogin);

                                Log.RequestsAndResponses("MetBankTokenResponse", serviceProvider, result);

                                if (result.token_type.ToLower() == "bearer")
                                {
                                    isTokenValid = true;
                                    yoAppResponse.ResponseCode = "00000";
                                    yoAppResponse.Description = "Token generated successfully";
                                    yoAppResponse.Note = "Transaction Successful";

                                    var expDate = DateTime.Now.AddSeconds(Convert.ToDouble(result.expires_in));

                                    result.expires_in = expDate.ToString("dd/MM/yyyy-HH:mm:ss");

                                    var token = JsonConvert.SerializeObject(result);

                                    yoAppResponse.Narrative = token;

                                    Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);
                                    Log.StoreData("tokens", serviceProvider, result);

                                    tokenFile = LoadMetBankJson(file);
                                }
                                else
                                {
                                    yoAppResponse.ResponseCode = "00008";
                                    yoAppResponse.Description = "Code could not be generated";
                                    yoAppResponse.Note = "Transaction Failed";

                                    Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);

                                    return yoAppResponse;
                                }
                            }

                            if (isTokenValid)
                            {
                                if (!string.IsNullOrEmpty(response.Note))
                                {
                                    if (response.Note.ToUpper() == "ACCOUNTDETAILS")
                                    {
                                        tokenFile = LoadMetBankJson(file);
                                        // Receive Money PreAuth Request
                                        ReceiveMoneyPreAuthRequest receiveMoneyPreAuthRequest = new ReceiveMoneyPreAuthRequest();

                                        receiveMoneyPreAuthRequest.agentId = (long)tokenFile.agentId;
                                        receiveMoneyPreAuthRequest.voucherNumber = response.CustomerAccount;
                                        receiveMoneyPreAuthRequest.tellerId = "154";

                                        Log.RequestsAndResponses("ReceiveMoneyPreAuthRequest", serviceProvider, receiveMoneyPreAuthRequest);

                                        var receiveMoneyPreAuthResponse = merchantBankConnector.PreAuthReceiveMoney(serviceProvider, receiveMoneyPreAuthRequest, tokenFile.access_token); // metBankCredentials.AccessToken

                                        Log.RequestsAndResponses("ReceiveMoneyPreAuthResponse", serviceProvider, receiveMoneyPreAuthResponse);

                                        if (!string.IsNullOrEmpty(receiveMoneyPreAuthResponse.preauthId))
                                        {
                                            receiveMoneyRequest.tellerId = 154;
                                            receiveMoneyRequest.agentId = (int)tokenFile.agentId;
                                            receiveMoneyRequest.preauthId = receiveMoneyPreAuthResponse.preauthId;
                                            receiveMoneyRequest.voucherNumber = receiveMoneyPreAuthRequest.voucherNumber;

                                            #region Store Receive Money Request with PreAuthId

                                            var receiverVoucherNumber = response.CustomerAccount;

                                            if (!string.IsNullOrEmpty(receiverVoucherNumber))
                                            {
                                                Log.StoreMpin(receiverVoucherNumber, serviceProvider, receiveMoneyRequest);

                                                List<Narrative> narratives = new List<Narrative>();

                                                yoAppResponse.ResponseCode = "00000";
                                                yoAppResponse.Description = receiveMoneyPreAuthResponse.description + ". Receive Money request saved successfully";
                                                yoAppResponse.Note = "Failed";
                                                yoAppResponse.CustomerAccount = receiveMoneyPreAuthResponse.voucherNumber;
                                                narrative.ReceiversName = receiveMoneyPreAuthResponse.firstName;
                                                narrative.ReceiversSurname = receiveMoneyPreAuthResponse.lastName;
                                                narrative.Currency = receiveMoneyPreAuthResponse.collectionCurrencyCode;
                                                narrative.ReceiversIdentification = receiveMoneyPreAuthResponse.nationalId;
                                                narrative.TransactionCode = receiveMoneyPreAuthResponse.voucherNumber;
                                                narrative.Balance = receiveMoneyPreAuthResponse.amount;

                                                narratives.Add(narrative);

                                                yoAppResponse.Narrative = JsonConvert.SerializeObject(narratives);

                                                Log.RequestsAndResponses("YoAppStoreReceiveMoneyResponse", serviceProvider, yoAppResponse);

                                                return yoAppResponse;
                                            }
                                            else
                                            {
                                                yoAppResponse.ResponseCode = "00008";
                                                yoAppResponse.Description = "Did not receive the voucher number";
                                                yoAppResponse.Note = "Failed";

                                                Log.RequestsAndResponses("YoAppStoreReceiveMoneyResponse", serviceProvider, yoAppResponse);

                                                return yoAppResponse;
                                            }

                                            #endregion

                                            #region Commented Out Code
                                            //tokenFile = LoadMetBankJson(file);

                                            //if (response.Note.ToUpper() == "ACCOUNTDETAILS") // We are to generate PreAuth Id
                                            //{
                                            //    #region Store Receive Money Request with PreAuthId

                                            //    var receiverVoucherNumber = response.CustomerAccount;

                                            //    if (!string.IsNullOrEmpty(receiverVoucherNumber))
                                            //    {
                                            //        Log.StoreMpin(receiverVoucherNumber, serviceProvider, receiveMoneyRequest);

                                            //        List<Narrative> narratives = new List<Narrative>();

                                            //        yoAppResponse.ResponseCode = "00000";
                                            //        yoAppResponse.Description = receiveMoneyPreAuthResponse.description + ". Receive Money request saved successfully";
                                            //        yoAppResponse.Note = "Failed";
                                            //        yoAppResponse.CustomerAccount = receiveMoneyPreAuthResponse.voucherNumber;
                                            //        narrative.ReceiversName = receiveMoneyPreAuthResponse.firstName;
                                            //        narrative.ReceiversSurname = receiveMoneyPreAuthResponse.lastName;
                                            //        narrative.Currency = receiveMoneyPreAuthResponse.collectionCurrencyCode;
                                            //        narrative.ReceiversIdentification = receiveMoneyPreAuthResponse.nationalId;
                                            //        narrative.TransactionCode = receiveMoneyPreAuthResponse.voucherNumber;
                                            //        narrative.Balance = receiveMoneyPreAuthResponse.amount;

                                            //        narratives.Add(narrative);

                                            //        yoAppResponse.Narrative = JsonConvert.SerializeObject(narratives);

                                            //        Log.RequestsAndResponses("YoAppStoreReceiveMoneyResponse", serviceProvider, yoAppResponse);

                                            //        return yoAppResponse;
                                            //    }
                                            //    else
                                            //    {
                                            //        yoAppResponse.ResponseCode = "00008";
                                            //        yoAppResponse.Description = "Did not receive the voucher number";
                                            //        yoAppResponse.Note = "Failed";

                                            //        Log.RequestsAndResponses("YoAppStoreReceiveMoneyResponse", serviceProvider, yoAppResponse);

                                            //        return yoAppResponse;
                                            //    }

                                            //    #endregion
                                            //}
                                            //else
                                            //{
                                            //    #region Receive Money Request

                                            //    string fileNameStored = HttpContext.Current.Server.MapPath("~/App_Data/" + serviceProvider + "/Files/" + response.CustomerAccount + ".json");

                                            //    var storedReceiveMoneyRequest = LoadReceiveMoneyRequest(fileNameStored, serviceProvider);

                                            //    tokenFile = LoadMetBankJson(file);

                                            //    if (!string.IsNullOrEmpty(storedReceiveMoneyRequest.preauthId))
                                            //    {
                                            //        Log.RequestsAndResponses("ReceiveMoneyRequest", serviceProvider, storedReceiveMoneyRequest);

                                            //        var transactionResponse = merchantBankConnector.ReceiveMoney(serviceProvider, storedReceiveMoneyRequest, tokenFile.access_token);

                                            //        Log.RequestsAndResponses("ReceiveMoneyResponse", serviceProvider, transactionResponse);

                                            //        if (!string.IsNullOrEmpty(transactionResponse.status))
                                            //        {
                                            //            yoAppResponse.ResponseCode = "00000";
                                            //            yoAppResponse.Description = "Token generated successfully";
                                            //            yoAppResponse.Note = transactionResponse.transactionId.ToString();
                                            //            yoAppResponse.Description = transactionResponse.description;
                                            //            yoAppResponse.TransactionRef = transactionResponse.transactionReference;
                                            //            yoAppResponse.Amount = decimal.Parse(transactionResponse.amount.Trim(), System.Globalization.CultureInfo.InvariantCulture);
                                            //            yoAppResponse.Balance = transactionResponse.fees;
                                            //            yoAppResponse.Note = transactionResponse.status;
                                            //            yoAppResponse.Currency = transactionResponse.collectionCurrencyCode;
                                            //            yoAppResponse.Narrative = JsonConvert.SerializeObject(narrative);

                                            //            Log.RequestsAndResponses("YoAppReceiveMoneyResponse", serviceProvider, yoAppResponse);

                                            //            return yoAppResponse;
                                            //        }
                                            //        else
                                            //        {
                                            //            yoAppResponse.ResponseCode = "00008";
                                            //            yoAppResponse.Note = "Failed";
                                            //            yoAppResponse.Description = "Failed to receive the money! See log for details";

                                            //            Log.RequestsAndResponses("YoAppReceiveMoneyResponse", serviceProvider, yoAppResponse);

                                            //            return yoAppResponse;
                                            //        }
                                            //    }
                                            //    else
                                            //    {
                                            //        yoAppResponse.ResponseCode = "00008";
                                            //        yoAppResponse.Note = "Failed";
                                            //        yoAppResponse.Description = "Failed to receive the preauth Id! See log for details";

                                            //        Log.RequestsAndResponses("YoAppReceiveMoneyResponse", serviceProvider, yoAppResponse);

                                            //        return yoAppResponse;
                                            //    }

                                            //    #endregion
                                            //} 
                                            #endregion
                                        }
                                        else
                                        {
                                            yoAppResponse.ResponseCode = "00008";
                                            yoAppResponse.Note = "Failed";
                                            yoAppResponse.Description = "Failed to receive a PreAuth Id from the server";

                                            Log.RequestsAndResponses("YoAppReceiveMoneyPreAuthIdRequest", serviceProvider, yoAppResponse);

                                            return yoAppResponse;
                                        }
                                    }
                                    else
                                    {
                                        yoAppResponse.ResponseCode = "00008";
                                        yoAppResponse.Description = "The phrase 'AccountDetails' did not come from the Server!";
                                        yoAppResponse.Note = "Failed";

                                        Log.RequestsAndResponses("YoAppStoreReceiveMoneyResponse", serviceProvider, yoAppResponse);

                                        return yoAppResponse;
                                    }
                                }
                                else
                                {
                                    #region Receive Money Request

                                    string fileNameStored = HttpContext.Current.Server.MapPath("~/App_Data/" + serviceProvider + "/Files/" + response.CustomerAccount + ".json");

                                    var storedReceiveMoneyRequest = LoadReceiveMoneyRequest(fileNameStored, serviceProvider);

                                    tokenFile = LoadMetBankJson(file);

                                    if (!string.IsNullOrEmpty(storedReceiveMoneyRequest.preauthId))
                                    {
                                        Log.RequestsAndResponses("ReceiveMoneyRequest", serviceProvider, storedReceiveMoneyRequest);

                                        var transactionResponse = merchantBankConnector.ReceiveMoney(serviceProvider, storedReceiveMoneyRequest, tokenFile.access_token);

                                        Log.RequestsAndResponses("ReceiveMoneyResponse", serviceProvider, transactionResponse);

                                        if (!string.IsNullOrEmpty(transactionResponse.status))
                                        {
                                            yoAppResponse.ResponseCode = "00000";
                                            yoAppResponse.Description = "Token generated successfully";
                                            yoAppResponse.Note = transactionResponse.transactionId.ToString();
                                            yoAppResponse.Description = transactionResponse.description;
                                            yoAppResponse.TransactionRef = transactionResponse.transactionReference;
                                            yoAppResponse.Amount = decimal.Parse(transactionResponse.amount.Trim(), System.Globalization.CultureInfo.InvariantCulture);
                                            yoAppResponse.Balance = transactionResponse.fees;
                                            yoAppResponse.Note = transactionResponse.status;
                                            yoAppResponse.Currency = transactionResponse.collectionCurrencyCode;
                                            yoAppResponse.Narrative = JsonConvert.SerializeObject(narrative);

                                            Log.RequestsAndResponses("YoAppReceiveMoneyResponse", serviceProvider, yoAppResponse);

                                            return yoAppResponse;
                                        }
                                        else
                                        {
                                            yoAppResponse.ResponseCode = "00008";
                                            yoAppResponse.Note = "Failed";
                                            yoAppResponse.Description = "Failed to receive the money! See log for details";

                                            Log.RequestsAndResponses("YoAppReceiveMoneyResponse", serviceProvider, yoAppResponse);

                                            return yoAppResponse;
                                        }
                                    }
                                    else
                                    {
                                        yoAppResponse.ResponseCode = "00008";
                                        yoAppResponse.Note = "Failed";
                                        yoAppResponse.Description = "Failed to receive the preauth Id! See log for details";

                                        Log.RequestsAndResponses("YoAppReceiveMoneyResponse", serviceProvider, yoAppResponse);

                                        return yoAppResponse;
                                    }

                                    #endregion
                                }                                
                            }
                            else
                            {
                                yoAppResponse.ResponseCode = "00008";
                                yoAppResponse.Note = "Failed to generate token";
                                yoAppResponse.Description = "The Token is Invalid";

                                Log.RequestsAndResponses("YoAppMetbankResponse", serviceProvider, yoAppResponse);

                                return yoAppResponse;
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.HttpError("Exception", serviceProvider, "Message: " + ex.Message + ", InnerException: " + ex.InnerException + ", StackTrace: " + ex.StackTrace);

                            return yoAppResponse;
                        }

                        #endregion                        

                    default:
                        break;
                }
            }

            return yoAppResponse;

        }

        [Route("api/logistics/service")]
        [HttpPost]
        YoAppResponse Logistics(YoAppResponse response)
        {
            #region Declared Objects
            var serviceProvider = "Essentail-Logistics";
            YoAppResponse yoAppResponse = new YoAppResponse();
            Narrative narrative = new Narrative();
            ExpenseTransactionVM expenseTransaction = new ExpenseTransactionVM();
            AquSalesConnector aquSalesConnector = new AquSalesConnector();
            #endregion

            if (response.Narrative != null)
            {
                narrative = JsonConvert.DeserializeObject<Narrative>(response.Narrative);
            }

            Log.RequestsAndResponses("YoAppResponse", serviceProvider, response);

            if (response == null)
            {
                string message = "Received Nothing. Your request object is null";

                yoAppResponse.ResponseCode = "00008";
                yoAppResponse.Note = "Failed";
                yoAppResponse.Description = message;

                return yoAppResponse;
            }
            else
            {
                switch (response.ServiceId)
                {
                    case 1:

                        expenseTransaction.Supplier = narrative.SupplierName;
                        expenseTransaction.Amount = (decimal)narrative.Balance;
                        expenseTransaction.Currency = narrative.Currency;
                        expenseTransaction.Company = narrative.ServiceProvider;
                        expenseTransaction.ExpenseAccount = narrative.SupplierName;
                        expenseTransaction.Username = narrative.ReceiversName;
                        expenseTransaction.Reference = response.TransactionRef;
                        expenseTransaction.TransactionDate = DateTime.Now;
                        expenseTransaction.Cashier = narrative.Cashier;
                        expenseTransaction.CostCenter = response.Note;

                        Log.RequestsAndResponses("Expense", serviceProvider, expenseTransaction);

                        var apiResponse = aquSalesConnector.PostExpense(expenseTransaction, serviceProvider);

                        Log.RequestsAndResponses("Expense", serviceProvider, apiResponse);

                        if (apiResponse.Status.ToUpper() == "SUCCESS")
                        {
                            yoAppResponse.ResponseCode = "00000";
                            yoAppResponse.Description = apiResponse.Description;
                            yoAppResponse.Note = "Success";
                            yoAppResponse.Narrative = "Expense Posted successfully";

                            Log.RequestsAndResponses("Aqu-Response-YoApp", serviceProvider, yoAppResponse);

                            return yoAppResponse;
                        }
                        else
                        {
                            yoAppResponse.ResponseCode = "00008";
                            yoAppResponse.Description = apiResponse.Description;
                            yoAppResponse.Note = "Transaction Failed";
                            yoAppResponse.Narrative = "Transaction Failed";

                            Log.RequestsAndResponses("Aqu-Response-YoApp", serviceProvider, apiResponse);

                            return yoAppResponse;
                        }
                }
            }

            return null;
        }

        [Route("api/payments")]
        [HttpPost]
        public WebPayments PaymentService(WebPayments payments)
        {
            #region Declared Objects
            var serviceProvider = "PaymentService";
            WebPayments webPayments = new WebPayments();
            Narrative narrative = new Narrative();
            Payments yoappPayments = new Payments();
            YoAppConnector yoAppConnector = new YoAppConnector();
            YoAppCredentials yoAppCredentials = new YoAppCredentials();
            string sSourceData;
            byte[] tmpSource;
            byte[] tmpHash;
            #endregion

            try
            {
                Log.RequestsAndResponses("WebHook-Push", serviceProvider, payments);

                if (webPayments == null)
                {
                    webPayments.Status = "Failed";
                    webPayments.Description = "Received Nothing from the request. Your request object is null";

                    Log.RequestsAndResponses("WebHook-Response", serviceProvider, webPayments);

                    return webPayments;
                }
                else
                {
                    #region Receipt and Ref No.

                    var date = DateTime.Now.ToString();
                    var dateToBeConverted = DateTime.ParseExact(date, "dd/M/yyyy hh:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                    var dateTimeConverterd = dateToBeConverted.ToString("MMddyyHHmmss");
                    var receiptNo = "YOAPP" + dateTimeConverterd;
                    var refNo = "REF" + dateTimeConverterd;

                    #endregion

                    #region Hashing

                    sSourceData = "MySourceData";
                    //Create a byte array from source data.
                    tmpSource = ASCIIEncoding.ASCII.GetBytes(sSourceData);

                    //Compute hash based on source data.
                    tmpHash = new MD5CryptoServiceProvider().ComputeHash(tmpSource);

                    #endregion

                    yoappPayments.merchantCode = yoAppCredentials.UserId + ":" + yoAppCredentials.Password;
                    yoappPayments.successURL = yoAppCredentials.SuccessUrl + "?CompanyName=" + webPayments.orgName +
                                                                             "&CompanyPhoneNumber=" + webPayments.orgPhoneNumber +
                                                                             "&CompanyAddress=" + webPayments.orgAddress +
                                                                             "&PackageType=AQUSALES LITE" +
                                                                             "&FirstName=" + webPayments.contactPersonName +
                                                                             "&Surname=" + webPayments.contactPersonLastName +
                                                                             "&EmailAddress=" + webPayments.customerEmail +
                                                                             "&Username=" + webPayments.username +
                                                                             "&Password=" + webPayments.password;
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
                        var responseBrowseUrl = part[0];
                        var responsePollsUrl = part[1];

                        webPayments.Status = "Success";
                        webPayments.Description = "Received the relevant payment details and redirecting to " + responseBrowseUrl;

                        Log.RequestsAndResponses("WebHook-Response", serviceProvider, webPayments);

                        RedirectToUrl(responseBrowseUrl);

                        return webPayments;
                    }
                    else
                    {
                        webPayments.Status = "Success";
                        webPayments.Description = "Received the relevant payment details but failed to redirect to Payment Gateway";

                        Log.RequestsAndResponses("WebHook-Response", serviceProvider, webPayments);

                        return webPayments;
                    }

                }
            }
            catch (Exception e)
            {
                Log.HttpError("Exception", serviceProvider, "Message: " + e.Message + ", InnerException: " + e.InnerException + ", StackTrace: " + e.StackTrace);
            }

            return webPayments;
        }

        //public HttpResponseMessage RedirectToUrl(string url)
        //{           
        //    var response = Request.CreateResponse(HttpStatusCode.Moved);
        //    response.Headers.Location = new Uri(url);
        //    return response;
        //}

        public IHttpActionResult RedirectToUrl(string url)
        {
            return Redirect(url);
        }

        [NonAction]
        private WafayaRefresherTokenResponse RefreshToken(string refresherToken)
        {
            WafayaRefresherTokenRequest tokenRequest = new WafayaRefresherTokenRequest();
            WafayaConnector wafayaConnector = new WafayaConnector();
            WafayaCredentials wafayaCredentials = new WafayaCredentials();
            YoAppResponse yoAppResponse = new YoAppResponse();
            var serviceProvider = "Wafaya";

            try
            {
                tokenRequest.grant_type = "refresh_token";
                tokenRequest.client_id = wafayaCredentials.PasswordClientId;
                tokenRequest.client_secret = wafayaCredentials.PasswordSecret;
                tokenRequest.scope = "";
                tokenRequest.refresh_token = refresherToken;

                Log.RequestsAndResponses("Wafaya-RefresherTokenRequest", serviceProvider, tokenRequest);

                var refresherTokenResponse = wafayaConnector.GetNewToken(tokenRequest, serviceProvider);

                if (refresherTokenResponse != null && refresherTokenResponse.token_type.ToLower() == "bearer")
                {
                    Log.RequestsAndResponses("Wafaya-TokenResponse-YoApp", serviceProvider, refresherTokenResponse);

                    var expDate = DateTime.Now.AddSeconds(Convert.ToDouble(refresherTokenResponse.expires_in));

                    refresherTokenResponse.expires_in = expDate.ToString("dd/MM/yyyy-HH:mm:ss");

                    Log.StoreData("tokens", serviceProvider, refresherTokenResponse);

                    return refresherTokenResponse;
                }
                else
                {
                    Log.RequestsAndResponses("Wafaya-TokenResponse-YoApp", serviceProvider, refresherTokenResponse);

                    return refresherTokenResponse;
                }
            }
            catch (Exception ex)
            {
                Log.RequestsAndResponses("Exception", serviceProvider, ex.Message);
                return null;
            }
        }

        [NonAction]
        private ResultDTO RefreshMetBankToken(string refresherToken)
        {
            RefresherTokenRequest refresherTokenRequest = new RefresherTokenRequest();
            MerchantBankConnector merchantBankConnector = new MerchantBankConnector();
            MetBankCredentials metBankCredentials = new MetBankCredentials();
            YoAppResponse yoAppResponse = new YoAppResponse();
            var serviceProvider = "MerchantBank";

            try
            {
                refresherTokenRequest.token = refresherToken;
                refresherTokenRequest.username = metBankCredentials.Username;

                Log.RequestsAndResponses("MetBankRefresherTokenRequest", serviceProvider, refresherTokenRequest);

                var refresherTokenResponse = merchantBankConnector.GetNewToken(serviceProvider, refresherTokenRequest);

                Log.RequestsAndResponses("MetBankRefresherTokenResponse", serviceProvider, refresherTokenResponse);

                if (!string.IsNullOrEmpty(refresherTokenResponse.token_type))
                {
                    if (refresherTokenResponse.token_type.ToLower() == "bearer")
                    {
                        Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, refresherTokenResponse);

                        var expDate = DateTime.Now.AddSeconds(Convert.ToDouble(refresherTokenResponse.expires_in));

                        refresherTokenResponse.expires_in = expDate.ToString("dd/MM/yyyy-HH:mm:ss");

                        Log.StoreData("tokens", serviceProvider, refresherTokenResponse);

                        return refresherTokenResponse;
                    }
                    else
                    {
                        Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, refresherTokenResponse);

                        return refresherTokenResponse;
                    }
                }
                else
                {
                    refresherTokenResponse.scope = "There has been an error on generating a new token from the refresher token";

                    return refresherTokenResponse;
                }
            }
            catch (Exception ex)
            {
                Log.HttpError("Exception", serviceProvider, "Message: " + ex.Message + ", StackTrace: " + ex.StackTrace + ", InnerException" + ex.InnerException);
                return null;
            }
        }

        [NonAction]
        public ResultDTO GenerateMetBankToken()
        {
            UserLogin userLogin = new UserLogin();
            MerchantBankConnector merchantBankConnector = new MerchantBankConnector();
            MetBankCredentials metBankCredentials = new MetBankCredentials();
            YoAppResponse yoAppResponse = new YoAppResponse();
            var serviceProvider = "MerchantBank";

            userLogin.password = metBankCredentials.Password;
            userLogin.username = metBankCredentials.Username;
            userLogin.clientSecret = metBankCredentials.ClientSecret;
            userLogin.clientId = metBankCredentials.ClientId;

            Log.RequestsAndResponses("MetBankTokenRequest", serviceProvider, userLogin);

            var result = merchantBankConnector.GetToken(serviceProvider, userLogin);

            Log.RequestsAndResponses("MetBankTokenResponse", serviceProvider, result);

            if (result.token_type.ToLower() == "bearer")
            {
                yoAppResponse.ResponseCode = "00000";
                yoAppResponse.Description = "Token generated successfully";
                yoAppResponse.Note = "Transaction Successful";

                var expDate = DateTime.Now.AddSeconds(Convert.ToDouble(result.expires_in));

                result.expires_in = expDate.ToString("dd/MM/yyyy-HH:mm:ss");

                var token = JsonConvert.SerializeObject(result);

                yoAppResponse.Narrative = token;

                Log.RequestsAndResponses("MetBankTokenResponse-YoApp", serviceProvider, yoAppResponse);
                Log.StoreData("tokens", serviceProvider, result);

                return result;
            }
            else
            {
                yoAppResponse.ResponseCode = "00008";
                yoAppResponse.Description = "Code could not be generated";
                yoAppResponse.Note = "Transaction Failed";

                Log.RequestsAndResponses("Wafaya-TokenResponse-YoApp", serviceProvider, yoAppResponse);

                return null;
            }
        }

        [NonAction]
        public WafayaRefresherTokenResponse LoadJson(string file)
        {
            try
            {
                using (FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.ReadWrite, FileShare.None))

                using (StreamReader sr = new StreamReader(fileStream))
                {
                    string json = sr.ReadToEnd();
                    var tokenResponse = JsonConvert.DeserializeObject<WafayaRefresherTokenResponse>(json);

                    return tokenResponse;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        [NonAction]
        public WafayaVoucherResponse LoadPinFile(string file)
        {
            try
            {
                using (FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.ReadWrite, FileShare.None))

                using (StreamReader sr = new StreamReader(fileStream))
                {
                    string json = sr.ReadToEnd();
                    var pinResponse = JsonConvert.DeserializeObject<WafayaVoucherResponse>(json);

                    return pinResponse;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        [NonAction]
        public ResultDTO LoadMetBankJson(string file)
        {
            try
            {
                using (FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.ReadWrite, FileShare.None))

                using (StreamReader sr = new StreamReader(fileStream))
                {
                    string json = sr.ReadToEnd();
                    var tokenResponse = JsonConvert.DeserializeObject<ResultDTO>(json);

                    return tokenResponse;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        [NonAction]
        public YoAppResponse LoadMpin(string file, string serviceProvider)
        {
            try
            {
                using (FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.ReadWrite, FileShare.None))

                using (StreamReader sr = new StreamReader(fileStream))
                {
                    string json = sr.ReadToEnd();
                    var mpinResponse = JsonConvert.DeserializeObject<YoAppResponse>(json);

                    return mpinResponse;
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

        [NonAction]
        public SendMoneyRequest LoadSendMoneyRequest(string file, string serviceProvider)
        {
            try
            {
                using (FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.ReadWrite, FileShare.None))

                using (StreamReader sr = new StreamReader(fileStream))
                {
                    string json = sr.ReadToEnd();
                    var sendMoneyRequest = JsonConvert.DeserializeObject<SendMoneyRequest>(json);

                    return sendMoneyRequest;
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

        [NonAction]
        public ReceiveMoneyRequest LoadReceiveMoneyRequest(string file, string serviceProvider)
        {
            try
            {
                using (FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.ReadWrite, FileShare.None))

                using (StreamReader sr = new StreamReader(fileStream))
                {
                    string json = sr.ReadToEnd();
                    var receiveMoneyRequest = JsonConvert.DeserializeObject<ReceiveMoneyRequest>(json);

                    return receiveMoneyRequest;
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

        [NonAction]
        public WafayaInitializeRedemptionResponse LoadInitFile(string file, string serviceProvider)
        {
            try
            {
                using (FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.ReadWrite, FileShare.None))

                using (StreamReader sr = new StreamReader(fileStream))
                {
                    string json = sr.ReadToEnd();

                    var resp = JsonConvert.DeserializeObject<string>(json);
                    var initFileResponse = JsonConvert.DeserializeObject<WafayaInitializeRedemptionResponse>(resp);

                    return initFileResponse;
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
