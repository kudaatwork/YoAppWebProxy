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
        public YoAppResponse ESolutionsServicesProxy(YoAppRequest yoAppRequest)
        {
            #region Declared Objects
            var serviceProvider = "ESolutions";

            YoAppResponse yoAppResponse = new YoAppResponse();
            ESolutionsRequest eSolutionsRequest = new ESolutionsRequest();
            ESolutionsApiObjects eSolutionsApiObjects = new ESolutionsApiObjects();
            ESolutionsMethods eSolutionsMethods = new ESolutionsMethods();
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

                        if (String.IsNullOrEmpty(yoAppRequest.MTI)) // without mti and processingCode
                        {
                            string message = "Your request does not have an mti e.g. 0200. " +
                                "Please put the correct mti and resend your request";

                            yoAppResponse.ResponseCode = "00008";
                            yoAppResponse.Note = "Failed";
                            yoAppResponse.Description = message;

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
                                            yoAppResponse = eSolutionsMethods.VendorBalanceEnquiry(eSolutionsRequest, serviceProvider);
                                            break;

                                        case "310000": // Customer Information
                                            yoAppResponse = eSolutionsMethods.CustomerInformation(eSolutionsRequest, serviceProvider);
                                            break;

                                        case "320000": // Last Customer Token
                                            yoAppResponse = eSolutionsMethods.CustomerInformation(eSolutionsRequest, serviceProvider);
                                            break;

                                        case "U50000": // Purchase Token e.g. ZETDC
                                            yoAppResponse = eSolutionsMethods.ServicePurchase(eSolutionsRequest, serviceProvider);
                                            break;

                                        case "520000": // Direct Payment for Service e.g. Airtime
                                            yoAppResponse = eSolutionsMethods.DirectServicePurchase(eSolutionsRequest, serviceProvider);
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
            }

            return yoAppResponse;
        }

        [Route("api/aqusales-yoapp/cbz-services")]
        [HttpPost]
        public YoAppResponse AqusalesYoAppCbzProxy(YoAppResponse redemptionResponse)
        {
            #region Declared Objects
            var serviceProvider = "CBZ-AquSales";
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

                            Log.RequestsAndResponses("Aqu-Request", serviceProvider, saleTransaction);

                            var aqusalesResponse = aquSalesConnector.PostCBZRedemption(saleTransaction, serviceProvider);

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

                            Log.RequestsAndResponses("AquGrv-Request", serviceProvider, grvTransaction);

                            var response = connector.PostCBZGRV(grvTransaction, serviceProvider);

                            Log.RequestsAndResponses("AquGrv-Response", serviceProvider, response);

                            if (response.Status.ToUpper() == "SUCCESS")
                            {
                                yoAppResponse.ResponseCode = "00000";
                                yoAppResponse.Description = response.Description;
                                yoAppResponse.Note = "Success";
                                yoAppResponse.Narrative = "Transaction/GRV Posted successfully";

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

                            Log.RequestsAndResponses("Aqu-Request", serviceProvider, sale);

                            var response = connector.PostCBZReversal(sale, serviceProvider);

                            Log.RequestsAndResponses("Aqu-Response", serviceProvider, response);

                            if (response.Status.ToUpper() == "SUCCESS")
                            {
                                yoAppResponse.ResponseCode = "00000";
                                yoAppResponse.Description = response.Description;
                                yoAppResponse.Note = "Success";
                                yoAppResponse.Narrative = "Transaction/Reversal Posted successfully";

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

                            Log.RequestsAndResponses("Aqu-Request", serviceProvider, saleTransaction);

                            var aqusalesResponse = aquSalesConnector.PostCBZRedemption2(saleTransaction, serviceProvider);

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

                            Log.RequestsAndResponses("AquGrv-Request", serviceProvider, grvTransaction);

                            var response = connector.PostCBZGRV2(grvTransaction, serviceProvider);

                            Log.RequestsAndResponses("AquGrv-Response", serviceProvider, response);

                            if (response.Status.ToUpper() == "SUCCESS")
                            {
                                yoAppResponse.ResponseCode = "00000";
                                yoAppResponse.Description = response.Description;
                                yoAppResponse.Note = "Success";
                                yoAppResponse.Narrative = "Transaction/GRV Posted successfully";

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

                            Log.RequestsAndResponses("Aqu-Request", serviceProvider, sale);

                            var response = connector.PostCBZReversal2(sale, serviceProvider);

                            Log.RequestsAndResponses("Aqu-Response", serviceProvider, response);

                            if (response.Status.ToUpper() == "SUCCESS")
                            {
                                yoAppResponse.ResponseCode = "00000";
                                yoAppResponse.Description = response.Description;
                                yoAppResponse.Note = "Success";
                                yoAppResponse.Narrative = "Transaction/Reversal Posted successfully";

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

                                        Log.RequestsAndResponses("Wafaya-TokenResponse-YoApp", serviceProvider, resourceOwnerResponse);
                                        Log.StoreData("tokens", serviceProvider, resourceOwnerResponse);

                                        tokenFile = LoadJson(file);
                                    }
                                    else
                                    {
                                        yoAppResponse.ResponseCode = "00008";
                                        yoAppResponse.Description = "Code could not be generated";
                                        yoAppResponse.Note = "Transaction Failed";

                                        Log.RequestsAndResponses("Wafaya-TokenResponse-YoApp", serviceProvider, resourceOwnerResponse);

                                        return yoAppResponse;
                                    }
                                }

                                if (isTokenValid)
                                {
                                    voucherRequest.Voucher = response.CustomerAccount;
                                    voucherRequest.Token = tokenFile.access_token;

                                    Log.RequestsAndResponses("Wafaya-GetVoucherRequest", serviceProvider, "");

                                    var wafayaVoucherResponse = connector.GetVoucherDetails(voucherRequest, serviceProvider);

                                    Log.RequestsAndResponses("Wafaya-GetVoucherResponse", serviceProvider, wafayaVoucherResponse);

                                    if (wafayaVoucherResponse.voucher_code != null)
                                    {
                                        yoAppResponse.ResponseCode = "00000";
                                        yoAppResponse.Description = "Voucher Retrived successfully";
                                        yoAppResponse.Note = wafayaVoucherResponse.redeemer_name;
                                        yoAppResponse.CustomerAccount = wafayaVoucherResponse.voucher_code;
                                        yoAppResponse.Amount = wafayaVoucherResponse.voucher_value;
                                        yoAppResponse.Balance = Convert.ToString(wafayaVoucherResponse.voucher_balance);
                                        yoAppResponse.CustomerMSISDN = wafayaVoucherResponse.redeemer_phone;
                                        yoAppResponse.IsActive = wafayaVoucherResponse.active;
                                        yoAppResponse.Currency = wafayaVoucherResponse.voucher_currency;                                                              
                                       
                                        Log.RequestsAndResponses("Wafaya-TokenResponse-YoApp", serviceProvider, wafayaVoucherResponse);

                                        //var voucherResponse = JsonConvert.SerializeObject(wafayaVoucherResponse);                                                                            

                                        return yoAppResponse;
                                    }
                                    else
                                    {
                                        yoAppResponse.ResponseCode = "00008";
                                        yoAppResponse.Description = "Could not retrieve voucher";
                                        yoAppResponse.Note = "Request Failed";

                                        Log.RequestsAndResponses("Wafaya-TokenResponse-YoApp", serviceProvider, wafayaVoucherResponse);

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
                                
                            case "330000": // Verification/Initializing
                                WafayaInitializeRedemptionRequest wafayaInitializeRedemptionRequest = new WafayaInitializeRedemptionRequest();

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

                                        Log.RequestsAndResponses("Wafaya-TokenResponse-YoApp", serviceProvider, resourceOwnerResponse);
                                        Log.StoreData("tokens", serviceProvider, resourceOwnerResponse);

                                        tokenFile2 = LoadJson(file2);
                                    }
                                    else
                                    {
                                        yoAppResponse.ResponseCode = "00008";
                                        yoAppResponse.Description = "Code could not be generated";
                                        yoAppResponse.Note = "Transaction Failed";

                                        Log.RequestsAndResponses("Wafaya-TokenResponse-YoApp", serviceProvider, resourceOwnerResponse);

                                        return yoAppResponse;
                                    }
                                }

                                if (isTokenValid)
                                {
                                    wafayaInitializeRedemptionRequest.amount = response.Amount;
                                    wafayaInitializeRedemptionRequest.voucher = response.CustomerAccount;
                                    wafayaInitializeRedemptionRequest.token = tokenFile2.access_token;

                                    Log.RequestsAndResponses("Wafaya-InitilizeVoucherRequest", serviceProvider, wafayaInitializeRedemptionRequest);

                                    //var wafayaVoucherResponse = connector.InitializeVoucher(wafayaInitializeRedemptionRequest, serviceProvider);

                                    //Log.RequestsAndResponses("Wafaya-InitilizeVoucherResponse", serviceProvider, wafayaVoucherResponse);

                                   // if (wafayaVoucherResponse != null && wafayaVoucherResponse.success.Contains("initialized"))
                                   // {
                                        yoAppResponse.ResponseCode = "00000";
                                        yoAppResponse.Description = "Voucher Initialized Successfully";

                                        //foreach (var item in wafayaVoucherResponse.payload)
                                        //{
                                        //    yoAppResponse.Note = item.voucher.redeemer_name;
                                        //    yoAppResponse.CustomerAccount = item.voucher.voucher_code;
                                        //    yoAppResponse.Amount = item.voucher.voucher_value;
                                        //    yoAppResponse.Balance = Convert.ToString(item.voucher.voucher_balance);
                                        //    yoAppResponse.CustomerMSISDN = item.voucher.redeemer_phone;
                                        //    yoAppResponse.IsActive = item.voucher.active;
                                        //    yoAppResponse.Currency = item.voucher.voucher_currency;
                                        //}                                      

                                        Log.RequestsAndResponses("Wafaya-TokenResponse-YoApp", serviceProvider, "");
                                                                                
                                        return yoAppResponse;
                                    //}
                                    //else
                                    //{
                                    //    yoAppResponse.ResponseCode = "00000";
                                    //    yoAppResponse.Description = "Voucher Initialized Successfully";
                                    //    yoAppResponse.Note = "Success";

                                    //    Log.RequestsAndResponses("Wafaya-TokenResponse-YoApp", serviceProvider, wafayaVoucherResponse);

                                    //    return yoAppResponse;
                                    //}
                                }
                                else
                                {
                                    yoAppResponse.ResponseCode = "00008";
                                    yoAppResponse.Description = "Token is not Valid";
                                    yoAppResponse.Note = "Request Failed";

                                    Log.RequestsAndResponses("Wafaya-TokenResponse-YoApp", serviceProvider, "");

                                    return yoAppResponse;
                                }                               

                            case "340000": // Authentication 

                                try
                                {
                                    var userPhoneNumber = response.CustomerMSISDN;

                                    if (response.Mpin != null)
                                    {
                                        Log.StoreMpin(userPhoneNumber, serviceProvider, response);

                                        yoAppResponse.ResponseCode = "00000";
                                        yoAppResponse.Description = "Mpin saved successfully";

                                        return yoAppResponse;
                                    }
                                    else if (response.Note != null)
                                    {
                                        response.Mpin = response.Note;

                                        Log.StoreMpin(userPhoneNumber, serviceProvider, response);

                                        yoAppResponse.ResponseCode = "00000";
                                        yoAppResponse.Description = "Mpin saved successfully";

                                        return yoAppResponse;
                                    }                                    
                                    else
                                    {
                                        yoAppResponse.ResponseCode = "00008";
                                        yoAppResponse.Description = "Did not submit Mpin";

                                        return yoAppResponse;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Log.HttpError("Exception", serviceProvider, ex.Message);
                                    return null;
                                }                               
                                
                            case "320000":

                                WafayaFinalizeVoucherRequest wafayaFinalizeVoucherRequest = new WafayaFinalizeVoucherRequest();

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

                                        Log.RequestsAndResponses("Wafaya-TokenResponse-YoApp", serviceProvider, resourceOwnerResponse);
                                        Log.StoreData("tokens", serviceProvider, resourceOwnerResponse);

                                        tokenFile3 = LoadJson(file3);
                                    }
                                    else
                                    {
                                        yoAppResponse.ResponseCode = "00008";
                                        yoAppResponse.Description = "Code could not be generated";
                                        yoAppResponse.Note = "Transaction Failed";

                                        Log.RequestsAndResponses("Wafaya-TokenResponse-YoApp", serviceProvider, resourceOwnerResponse);

                                        return yoAppResponse;
                                    }
                                }

                                if (isTokenValid)
                                {
                                    string mPinFile = HttpContext.Current.Server.MapPath("~/App_Data/" + serviceProvider + "/Files/" + response.CustomerMSISDN + ".json");

                                    var mpinFile = LoadMpin(mPinFile);

                                    if (mpinFile != null)
                                    {
                                        wafayaFinalizeVoucherRequest.confirmation_otp = mpinFile.Mpin;
                                        wafayaFinalizeVoucherRequest.voucher = response.CustomerAccount;
                                        wafayaFinalizeVoucherRequest.token = tokenFile3.access_token;

                                        Log.RequestsAndResponses("Wafaya-FinalizeVoucherRequest", serviceProvider, wafayaFinalizeVoucherRequest);

                                        var wafayaVoucherResponse = connector.FinalizeVoucher(wafayaFinalizeVoucherRequest, serviceProvider);

                                        Log.RequestsAndResponses("Wafaya-FinalizeVoucherResponse", serviceProvider, wafayaVoucherResponse);

                                        if (wafayaVoucherResponse!= null && wafayaVoucherResponse.success.Contains("finalized"))
                                        {
                                            yoAppResponse.ResponseCode = "00000";
                                            yoAppResponse.Description = "Voucher Finalized Successfully";

                                            foreach (var item in wafayaVoucherResponse.payload)
                                            {
                                                yoAppResponse.Note = item.voucher.redeemer_name;
                                                yoAppResponse.CustomerAccount = item.voucher.voucher_code;
                                                yoAppResponse.Amount = item.voucher.voucher_value;
                                                yoAppResponse.Balance = Convert.ToString(item.voucher.voucher_balance);
                                                yoAppResponse.CustomerMSISDN = item.voucher.redeemer_phone;
                                                yoAppResponse.IsActive = item.voucher.active;
                                                yoAppResponse.Currency = item.voucher.voucher_currency;
                                            }

                                            Log.RequestsAndResponses("Wafaya-TokenResponse-YoApp", serviceProvider, wafayaVoucherResponse);

                                            return yoAppResponse;
                                        }
                                        else
                                        {
                                            yoAppResponse.ResponseCode = "00008";
                                            yoAppResponse.Description = "Could not finalize voucher";
                                            yoAppResponse.Note = "Request to the server Failed";

                                            Log.RequestsAndResponses("Wafaya-TokenResponse-YoApp", serviceProvider, wafayaVoucherResponse);

                                            return yoAppResponse;
                                        }
                                    }
                                    else
                                    {
                                        yoAppResponse.ResponseCode = "00008";
                                        yoAppResponse.Description = "Could not find Mpin, Please start again the verification process";
                                       
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
        public YoAppResponse LoadMpin(string file)
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
                return null;
            }
        }
    }
}
