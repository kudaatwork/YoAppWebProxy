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

                        voucherPaymentDetails.Summary = new List<Product>();

                        foreach (var item in deserializedYoAppNarrative.Products)
                        {
                            voucherPaymentDetails.Summary.Add(new Product { ProductRedeemed = item.Name, QuantityRedeemed = item.Collected, PricePerUnit = item.Price });
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
                            totalAmout = item.Price * item.Quantity;
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
                        saleTransaction.TransactionDate = deserializedResponse.DatelastAccess.ToString();
                        saleTransaction.TransactionName = AqusalesObjects.TransactionName.Trim();
                        saleTransaction.Cashier = deserializedResponse.Cashier.Trim();
                        saleTransaction.CustomerAccountType = AqusalesObjects.CustomerAccountType.Trim();

                        Log.RequestsAndResponses("Aqu-Request", serviceProvider, saleTransaction);

                        var aqusalesResponse = aquSalesConnector.PostRedemption(saleTransaction, serviceProvider);

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

                    case 2: // Post GRV

                        GRVTransaction grvTransaction = new GRVTransaction();                       
                        AquSalesConnector connector = new AquSalesConnector();

                        var aquResponse = JsonConvert.DeserializeObject<YoAppGrvResponse>(redemptionResponse.Narrative);

                        grvTransaction.Username = AqusalesCredentials.Username;
                        grvTransaction.Password = AqusalesCredentials.Password;
                        grvTransaction.SupplierAccountType = AqusalesObjects.SupplierAccountType;
                        grvTransaction.TransactionName = AqusalesObjects.TransactionName;
                        grvTransaction.TransactionReference = aquResponse.OrderNumber.Trim();
                        grvTransaction.Company = "CBZ AGROYIELD"; //aquResponse.BranchName;
                        grvTransaction.DebtorOrCreditor = aquResponse.BranchName?.Trim() + "("+ aquResponse.BranchId?.Trim() +")";
                        grvTransaction.Amount = (decimal)aquResponse.StockedValue;
                        grvTransaction.Vat = 0.0m;

                        TranCurrencyInfoVM tranCurrencyInfo = new TranCurrencyInfoVM();
                        tranCurrencyInfo.TransactionCurrency = "USD";//aquResponse.Currency;
                        tranCurrencyInfo.TransactionCurrencyRate = 1;
                        grvTransaction.TranCurrencyInfoVM = tranCurrencyInfo;
                        grvTransaction.TransactionDate = aquResponse.AuthorisationDate;
                        grvTransaction.TransactionName = aquResponse.OrderType?.Trim();
                        grvTransaction.Cashier = aquResponse.Cashier?.Trim();

                        Log.RequestsAndResponses("AquGrv-Request", serviceProvider, grvTransaction);

                        var response = connector.PostGRV(grvTransaction, serviceProvider);

                        Log.RequestsAndResponses("AquGrv-Response", serviceProvider, grvTransaction);

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
            }

            return yoAppResponse;
        }

        [Route("api/eos-yoapp/cbz-services")]
        [HttpPost]
        public YoAppResponse EosYoAppCbzProxy(YoAppResponse redemptionResponse)
        {
            #region Declared Objects
            var serviceProvider = "CBZ-EOS";
            YoAppResponse yoAppResponse = new YoAppResponse();
            #endregion

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
                    case 1:
                        EosRequest eosRequest = new EosRequest();
                        EosConnector eosConnector = new EosConnector();

                        var deserializedNarrative = JsonConvert.DeserializeObject<Narrative>(redemptionResponse.Narrative);

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

                        eosRequest.Products = products;

                        Log.RequestsAndResponses("EOS-Request", serviceProvider, eosRequest);

                        var eosResponse = eosConnector.PostRedemption(eosRequest, serviceProvider);

                        Log.RequestsAndResponses("EOS-Response", serviceProvider, eosResponse);

                        if (eosResponse.ResponseCode.ToUpper() == "SUCCESS")
                        {
                            yoAppResponse.ResponseCode = "00000";
                            yoAppResponse.Description = eosResponse.Description;
                            yoAppResponse.Note = "Success";
                            yoAppResponse.Narrative = "Transaction/Redemption Posted successfully";

                            Log.RequestsAndResponses("EOS-Response-YoApp", serviceProvider, eosResponse);

                            return yoAppResponse;
                        }
                        else
                        {
                            yoAppResponse.ResponseCode = "00008";
                            yoAppResponse.Description = eosResponse.Description;
                            yoAppResponse.Note = "Success";
                            yoAppResponse.Narrative = "Transaction/Redemption Posted successfully";

                            Log.RequestsAndResponses("EOS-Response-YoApp", serviceProvider, eosResponse);

                            return yoAppResponse;
                        }
                }
            }

            return yoAppResponse;
        }
    }
}
