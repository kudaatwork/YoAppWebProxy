using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using YoAppWebProxy.Models;
using YoAppWebProxy.Logs;

namespace YoAppWebProxy
{
    public class ESolutionsMethods
    {
        #region Global Objects and Variables
        ESolutionsApiObjects eSolutionsApiObjects = new ESolutionsApiObjects();
        ESolutionsRequest request = new ESolutionsRequest();
        ESolutionsCredentials eSolutionsCredentials = new ESolutionsCredentials();
        ESolutionsConnector eSolutionsConnector = new ESolutionsConnector();
        string message = "";
        bool success = false;
        #endregion

        public YoAppResponse VendorBalanceEnquiry(ESolutionsRequest eSolutionsRequest, string serviceProvider)
        {
            YoAppResponse yoAppResponse = new YoAppResponse();

            request.mti = eSolutionsRequest.mti;

            #region vendorReference
            var date = DateTime.Now.ToString();
            var dateTimeConverterd = DateTime.Parse(date, CultureInfo.InvariantCulture).ToString("MMddyyHHmmss");
            var reference = "YOAPP" + dateTimeConverterd;
            #endregion

            if (String.IsNullOrEmpty(request.vendorReference))
            {
                request.vendorReference = reference;
            }
            else
            {
                request.vendorReference = eSolutionsRequest.vendorReference;
            }

            request.processingCode = eSolutionsApiObjects.VendorBalanceEnquiry;

            #region transactionAmount
            var amount = Convert.ToString(eSolutionsRequest.transactionAmount);
            var apiAmount = amount.Replace(".", "");
            #endregion

            request.transactionAmount = Convert.ToInt64(apiAmount);

            #region transmissionDate
            var dateTime = DateTime.Now.ToString();
            var dateTimeNowConverted = DateTime.Parse(dateTime, CultureInfo.InvariantCulture).ToString("MMddyyHHmmss");
            #endregion

            if (String.IsNullOrEmpty(request.transmissionDate))
            {
                request.transmissionDate = dateTimeNowConverted;
            }
            else
            {
                request.transmissionDate = eSolutionsRequest.transmissionDate;
            }

            if (String.IsNullOrEmpty(request.vendorNumber))
            {
                request.vendorNumber = eSolutionsCredentials.VendorNumber;
            }
            else
            {
                request.vendorNumber = eSolutionsRequest.vendorNumber;
            }

            request.merchantName = eSolutionsRequest.merchantName;
            request.productName = eSolutionsRequest.productName;
            request.utilityAccount = eSolutionsRequest.utilityAccount;
            request.sourceMobile = eSolutionsRequest.sourceMobile;
            request.targetMobile = eSolutionsRequest.targetMobile;
            request.serviceId = eSolutionsRequest.serviceId;

            if (String.IsNullOrEmpty(eSolutionsRequest.currencyCode))
            {
                request.currencyCode = "ZWL";
            }
            else
            {
                request.currencyCode = eSolutionsRequest.currencyCode;
            }

            Log.RequestsAndResponses("VBalance-Request",serviceProvider,request);

            var response = eSolutionsConnector.GetESolutionsResponse(request);

            Log.RequestsAndResponses("VBalance-Response", serviceProvider, response);

            if (response.responseCode == "00" || response.responseCode == "09") // Transaction Successful or Request still in Progress
            {
                if (response.responseCode == "00")
                {
                    message = "Transaction Successful";
                    success = true;

                    yoAppResponse.ResponseCode = "00000";
                    yoAppResponse.Note = "Success";
                    yoAppResponse.Description = response.narrative;                    
                    yoAppResponse.Narrative = response.vendorBalance;

                    return yoAppResponse;                    
                }
                if (response.responseCode == "09") // 
                {
                    message = "Transaction still in progress";
                    success = false;

                    yoAppResponse.ResponseCode = "00000";
                    yoAppResponse.Note = "Success";
                    yoAppResponse.Description = response.narrative;
                    yoAppResponse.Narrative = response.vendorBalance;

                    return yoAppResponse;
                }
            }
            else if (response.responseCode == "05")
            {
                message = "General Error";
                success = false;

                yoAppResponse.ResponseCode = "00008";
                yoAppResponse.Note = "Failed";
                yoAppResponse.Description = response.narrative;

                return yoAppResponse;
            }
            else if (response.responseCode == "12")
            {
                message = "Amount cannot cover debt due/Amount is below{above} the limit";
                success = false;

                yoAppResponse.ResponseCode = "00008";
                yoAppResponse.Note = "Failed";
                yoAppResponse.Description = message;

                return yoAppResponse;
            }
            else if (response.responseCode == "14")
            {
                message = "Invalid meter number/Meter is blocked";
                success = false;

                yoAppResponse.ResponseCode = "00008";
                yoAppResponse.Note = "Failed";
                yoAppResponse.Description = message;

                return yoAppResponse;
            }
            else if (response.responseCode == "51")
            {
                message = "Insufficient funds";
                success = false;

                yoAppResponse.ResponseCode = "00008";
                yoAppResponse.Note = "Failed";
                yoAppResponse.Description = message;

                return yoAppResponse;
            }
            else if (response.responseCode == "57")
            {
                message = "Customer account has reversal in progress";
                success = false;

                yoAppResponse.ResponseCode = "00008";
                yoAppResponse.Note = "Failed";
                yoAppResponse.Description = message;

                return yoAppResponse;
            }
            else if (response.responseCode == "63")
            {
                message = "Security violation";
                success = false;

                yoAppResponse.ResponseCode = "00008";
                yoAppResponse.Note = "Failed";
                yoAppResponse.Description = message;

                return yoAppResponse;
            }
            else if (response.responseCode == "68")
            {
                message = "Transaction timeout";
                success = false;

                yoAppResponse.ResponseCode = "00008";
                yoAppResponse.Note = "Failed";
                yoAppResponse.Description = message;

                return yoAppResponse;
            }
            else if (response.responseCode == "94")
            {
                message = "Duplicate transaction";
                success = false;

                yoAppResponse.ResponseCode = "00008";
                yoAppResponse.Note = "Failed";
                yoAppResponse.Description = message;

                return yoAppResponse;
            }
            else
            {
                var serializedResponse = JsonConvert.SerializeObject(response, Formatting.Indented);
                yoAppResponse.Narrative = serializedResponse;

                return yoAppResponse;
            }

            return yoAppResponse;
        }

        public YoAppResponse CustomerInformation(ESolutionsRequest eSolutionsRequest, string serviceProvider)
        {
            YoAppResponse yoAppResponse = new YoAppResponse();

            request.mti = eSolutionsRequest.mti;

            #region vendorReference
            var date = DateTime.Now.ToString();
            var dateTimeConverterd = DateTime.Parse(date, CultureInfo.InvariantCulture).ToString("MMddyyHHmmss");
            var reference = "YOAPP" + dateTimeConverterd;
            #endregion

            if (String.IsNullOrEmpty(request.vendorReference))
            {
                request.vendorReference = reference;
            }
            else
            {
                request.vendorReference = eSolutionsRequest.vendorReference;
            }
            
            request.processingCode = eSolutionsApiObjects.CustomerInformation;

            #region transactionAmount
            var amount = Convert.ToString(eSolutionsRequest.transactionAmount);
            var apiAmount = amount.Replace(".", "");
            #endregion

            request.transactionAmount = Convert.ToInt64(apiAmount);

            #region transmissionDate
            var dateTime = DateTime.Now.ToString();
            var dateTimeNowConverted = DateTime.Parse(dateTime, CultureInfo.InvariantCulture).ToString("MMddyyHHmmss");
            #endregion

            if (String.IsNullOrEmpty(request.transmissionDate))
            {
                request.transmissionDate = dateTimeNowConverted;
            }
            else
            {
                request.transmissionDate = eSolutionsRequest.transmissionDate;
            }

            if (String.IsNullOrEmpty(request.vendorNumber))
            {
                request.vendorNumber = eSolutionsCredentials.VendorNumber;
            }
            else
            {
                request.vendorNumber = eSolutionsRequest.vendorNumber;
            }
            
            request.merchantName = eSolutionsRequest.merchantName;
            request.productName = eSolutionsRequest.productName;
            request.utilityAccount = eSolutionsRequest.utilityAccount;
            request.sourceMobile = eSolutionsRequest.sourceMobile;
            request.targetMobile = eSolutionsRequest.targetMobile;
            request.serviceId = eSolutionsRequest.serviceId;

            if (String.IsNullOrEmpty(eSolutionsRequest.currencyCode))
            {
                request.currencyCode = "ZWL";
            }
            else
            {
                request.currencyCode = eSolutionsRequest.currencyCode;
            }

            Log.RequestsAndResponses("CustInfo-Request", serviceProvider, request);

            var response = eSolutionsConnector.GetESolutionsResponse(request);

            Log.RequestsAndResponses("CustInfo-Response", serviceProvider, response);

            if (response.responseCode == "00" || response.responseCode == "09") // Transaction Successful or Request still in Progress
            {
                if (response.responseCode == "00")
                {
                    message = "Transaction Successful";
                    success = true;

                    yoAppResponse.ResponseCode = "00000";
                    yoAppResponse.Note = "Success";
                    yoAppResponse.Description = response.narrative;

                    Vouchers vouchers = new Vouchers();
                    vouchers.Miscellenious = response.customerData + "|" + response.cutomerAddress + "|"
                        + response.utilityAccount;
                    
                    yoAppResponse.Narrative = JsonConvert.SerializeObject(vouchers); // One of the two to be romoved on test
                    yoAppResponse.vouchers.Add(vouchers);

                    return yoAppResponse;
                }
                if (response.responseCode == "09") // 
                {
                    message = "Transaction still in progress";
                    success = false;

                    yoAppResponse.ResponseCode = "00000";
                    yoAppResponse.Note = "Success";
                    yoAppResponse.Description = response.narrative;

                    Vouchers vouchers = new Vouchers();
                    vouchers.Miscellenious = response.customerData + "|" + response.cutomerAddress + "|"
                        + response.utilityAccount;

                    yoAppResponse.Narrative = JsonConvert.SerializeObject(vouchers); // One of the two to be romoved on test
                    yoAppResponse.vouchers.Add(vouchers);

                    return yoAppResponse;
                }
            }
            else if (response.responseCode == "05")
            {
                message = "General Error";
                success = false;

                yoAppResponse.ResponseCode = "00008";
                yoAppResponse.Note = "Failed";
                yoAppResponse.Description = message;               

                return yoAppResponse;
            }
            else if (response.responseCode == "12")
            {
                message = "Amount cannot cover debt due/Amount is below{above} the limit";
                success = false;

                yoAppResponse.ResponseCode = "00008";
                yoAppResponse.Note = "Failed";
                yoAppResponse.Description = message;
                
                return yoAppResponse;
            }
            else if (response.responseCode == "14")
            {
                message = "Invalid meter number/Meter is blocked";
                success = false;

                yoAppResponse.ResponseCode = "00008";
                yoAppResponse.Note = "Failed";
                yoAppResponse.Description = message;

                return yoAppResponse;
            }
            else if (response.responseCode == "51")
            {
                message = "Insufficient funds";
                success = false;

                yoAppResponse.ResponseCode = "00008";
                yoAppResponse.Note = "Failed";
                yoAppResponse.Description = message;
                
                return yoAppResponse;
            }
            else if (response.responseCode == "57")
            {
                message = "Customer account has reversal in progress";
                success = false;

                yoAppResponse.ResponseCode = "00008";
                yoAppResponse.Note = "Failed";
                yoAppResponse.Description = message;
                
                return yoAppResponse;
            }
            else if (response.responseCode == "63")
            {
                message = "Security violation";
                success = false;

                yoAppResponse.ResponseCode = "00008";
                yoAppResponse.Note = "Failed";
                yoAppResponse.Description = message;
                
                return yoAppResponse;
            }
            else if (response.responseCode == "68")
            {
                message = "Transaction timeout";
                success = false;

                yoAppResponse.ResponseCode = "00008";
                yoAppResponse.Note = "Failed";
                yoAppResponse.Description = message;
                
                return yoAppResponse;
            }
            else if (response.responseCode == "94")
            {
                message = "Duplicate transaction";
                success = false;

                yoAppResponse.ResponseCode = "00008";
                yoAppResponse.Note = "Failed";
                yoAppResponse.Description = message;
                
                return yoAppResponse;
            }
            
            var serializedResponse = JsonConvert.SerializeObject(response, Formatting.Indented);
            yoAppResponse.Narrative = serializedResponse;

            return yoAppResponse;
        }

        public YoAppResponse ServicePurchase(ESolutionsRequest eSolutionsRequest, string serviceProvider)
        {
            YoAppResponse yoAppResponse = new YoAppResponse();

            request.mti = eSolutionsRequest.mti;

            #region vendorReference
            var date = DateTime.Now.ToString();
            var dateTimeConverterd = DateTime.Parse(date, CultureInfo.InvariantCulture).ToString("MMddyyHHmmss");
            var reference = "YOAPP" + dateTimeConverterd;
            #endregion

            if (String.IsNullOrEmpty(request.vendorReference))
            {
                request.vendorReference = reference;
            }
            else
            {
                request.vendorReference = eSolutionsRequest.vendorReference;
            }

            request.processingCode = eSolutionsApiObjects.TokenPurchaseRequest;

            #region transactionAmount
            var amount = Convert.ToString(eSolutionsRequest.transactionAmount);
            var apiAmount = amount.Replace(".", "");
            #endregion

            request.transactionAmount = Convert.ToInt64(apiAmount);

            #region transmissionDate
            var dateTime = DateTime.Now.ToString();
            var dateTimeNowConverted = DateTime.Parse(dateTime, CultureInfo.InvariantCulture).ToString("MMddyyHHmmss");
            #endregion

            if (String.IsNullOrEmpty(request.transmissionDate))
            {
                request.transmissionDate = dateTimeNowConverted;
            }
            else
            {
                request.transmissionDate = eSolutionsRequest.transmissionDate;
            }

            if (String.IsNullOrEmpty(request.vendorNumber))
            {
                request.vendorNumber = eSolutionsCredentials.VendorNumber;
            }
            else
            {
                request.vendorNumber = eSolutionsRequest.vendorNumber;
            }

            request.merchantName = eSolutionsRequest.merchantName;
            request.productName = eSolutionsRequest.productName;
            request.utilityAccount = eSolutionsRequest.utilityAccount;
            request.sourceMobile = eSolutionsRequest.sourceMobile;
            request.targetMobile = eSolutionsRequest.targetMobile;
            request.serviceId = eSolutionsRequest.serviceId;

            if (String.IsNullOrEmpty(eSolutionsRequest.currencyCode))
            {
                request.currencyCode = "ZWL";
            }
            else
            {
                request.currencyCode = eSolutionsRequest.currencyCode;
            }

            if (eSolutionsRequest.merchantName == "ZETDC" && String.IsNullOrEmpty(eSolutionsRequest.aggregator))
            {
                request.aggregator = "POWERTEL";
            }
            else
            {
                request.aggregator = eSolutionsRequest.aggregator;
            }

            Log.RequestsAndResponses("TokenPurchase-Request", serviceProvider, request); 

            var response = eSolutionsConnector.GetESolutionsResponse(request);

            Log.RequestsAndResponses("TokenPurchase-Response", serviceProvider, response);

            if (response.responseCode == "00" || response.responseCode == "09") // Transaction Successful or Request still in Progress
            {
                if (response.responseCode == "00")
                {
                    message = "Transaction Successful";
                    success = true;

                    yoAppResponse.ResponseCode = "00000";
                    yoAppResponse.Note = "Success";
                    yoAppResponse.Description = response.narrative;

                    Vouchers vouchers = new Vouchers();

                    vouchers.Miscellenious = response.miscellaneousData;
                    vouchers.VoucherKey = response.token + "|" + response.fixedCharges;

                    yoAppResponse.vouchers.Add(vouchers);

                    return yoAppResponse;
                }
                if (response.responseCode == "09") // 
                {
                    message = "Transaction still in progress";
                    success = false;

                    yoAppResponse.ResponseCode = "00000";
                    yoAppResponse.Note = "Success";
                    yoAppResponse.Description = response.narrative;

                    Vouchers vouchers = new Vouchers();
                    
                    vouchers.Miscellenious = response.miscellaneousData;
                    vouchers.VoucherKey = response.token + "|" + response.fixedCharges;                    

                    yoAppResponse.vouchers.Add(vouchers);

                    return yoAppResponse;
                }
            }
            else if (response.responseCode == "05")
            {
                message = "General Error";
                success = false;

                yoAppResponse.ResponseCode = "00008";
                yoAppResponse.Note = "Failed";
                yoAppResponse.Description = response.narrative;

                return yoAppResponse;
            }
            else if (response.responseCode == "12")
            {
                message = "Amount cannot cover debt due/Amount is below{above} the limit";
                success = false;

                yoAppResponse.ResponseCode = "00008";
                yoAppResponse.Note = "Failed";
                yoAppResponse.Description = message;

                return yoAppResponse;
            }
            else if (response.responseCode == "14")
            {
                message = "Invalid meter number/Meter is blocked";
                success = false;

                yoAppResponse.ResponseCode = "00008";
                yoAppResponse.Note = "Failed";
                yoAppResponse.Description = message;

                return yoAppResponse;
            }
            else if (response.responseCode == "51")
            {
                message = "Insufficient funds";
                success = false;

                yoAppResponse.ResponseCode = "00008";
                yoAppResponse.Note = "Failed";
                yoAppResponse.Description = message;

                return yoAppResponse;
            }
            else if (response.responseCode == "57")
            {
                message = "Customer account has reversal in progress";
                success = false;

                yoAppResponse.ResponseCode = "00008";
                yoAppResponse.Note = "Failed";
                yoAppResponse.Description = message;

                return yoAppResponse;
            }
            else if (response.responseCode == "63")
            {
                message = "Security violation";
                success = false;

                yoAppResponse.ResponseCode = "00008";
                yoAppResponse.Note = "Failed";
                yoAppResponse.Description = message;

                return yoAppResponse;
            }
            else if (response.responseCode == "68")
            {
                message = "Transaction timeout";
                success = false;

                yoAppResponse.ResponseCode = "00008";
                yoAppResponse.Note = "Failed";
                yoAppResponse.Description = message;

                return yoAppResponse;
            }
            else if (response.responseCode == "94")
            {
                message = "Duplicate transaction";
                success = false;

                yoAppResponse.ResponseCode = "00008";
                yoAppResponse.Note = "Failed";
                yoAppResponse.Description = message;

                return yoAppResponse;
            }

            var serializedResponse = JsonConvert.SerializeObject(response, Formatting.Indented);
            yoAppResponse.Narrative = serializedResponse;

            return yoAppResponse;
        }

        public YoAppResponse DirectServicePurchase(ESolutionsRequest eSolutionsRequest, string serviceProvider)
        {
            YoAppResponse yoAppResponse = new YoAppResponse();

            request.mti = eSolutionsRequest.mti;

            #region vendorReference
            var date = DateTime.Now.ToString();
            var dateTimeConverterd = DateTime.Parse(date, CultureInfo.InvariantCulture).ToString("MMddyyHHmmss");
            var reference = "YOAPP" + dateTimeConverterd;
            #endregion

            if (String.IsNullOrEmpty(request.vendorReference))
            {
                request.vendorReference = reference;
            }
            else
            {
                request.vendorReference = eSolutionsRequest.vendorReference;
            }

            request.processingCode = eSolutionsApiObjects.DirectPaymentRequest;

            #region transactionAmount
            var amount = Convert.ToString(eSolutionsRequest.transactionAmount);
            var apiAmount = amount.Replace(".", "");
            #endregion

            request.transactionAmount = Convert.ToInt64(apiAmount);

            #region transmissionDate
            var dateTime = DateTime.Now.ToString();
            var dateTimeNowConverted = DateTime.Parse(dateTime, CultureInfo.InvariantCulture).ToString("MMddyyHHmmss");
            #endregion

            if (String.IsNullOrEmpty(request.transmissionDate))
            {
                request.transmissionDate = dateTimeNowConverted;
            }
            else
            {
                request.transmissionDate = eSolutionsRequest.transmissionDate;
            }

            if (String.IsNullOrEmpty(request.vendorNumber))
            {
                request.vendorNumber = eSolutionsCredentials.VendorNumber;
            }
            else
            {
                request.vendorNumber = eSolutionsRequest.vendorNumber;
            }

            request.merchantName = eSolutionsRequest.merchantName;
            request.productName = eSolutionsRequest.productName;
            request.utilityAccount = eSolutionsRequest.utilityAccount;
            request.sourceMobile = eSolutionsRequest.sourceMobile;
            request.targetMobile = eSolutionsRequest.targetMobile;
            request.serviceId = eSolutionsRequest.serviceId;

            if (String.IsNullOrEmpty(eSolutionsRequest.currencyCode))
            {
                request.currencyCode = "ZWL";
            }
            else
            {
                request.currencyCode = eSolutionsRequest.currencyCode;
            }

            if (eSolutionsRequest.merchantName == "ZETDC" && String.IsNullOrEmpty(eSolutionsRequest.aggregator))
            {
                request.aggregator = "POWERTEL";
            }
            else
            {
                request.aggregator = eSolutionsRequest.aggregator;
            }            

            var response = eSolutionsConnector.GetESolutionsResponse(request);

            if (response.responseCode == "00" || response.responseCode == "09") // Transaction Successful or Request still in Progress
            {
                if (response.responseCode == "00")
                {
                    message = "Transaction Successful";
                    success = true;

                    yoAppResponse.ResponseCode = "00000";
                    yoAppResponse.Note = "Success";
                    yoAppResponse.Description = response.narrative;

                    return yoAppResponse;
                }
                if (response.responseCode == "09") // 
                {
                    message = "Transaction still in progress";
                    success = false;

                    yoAppResponse.ResponseCode = "00000";
                    yoAppResponse.Note = "Success";
                    yoAppResponse.Description = response.narrative;
                    yoAppResponse.CustomerData = response.customerData;
                    yoAppResponse.CustomerAccount = response.utilityAccount;
                }
            }
            else if (response.responseCode == "05")
            {
                message = "General Error";
                success = false;

                yoAppResponse.ResponseCode = "00008";
                yoAppResponse.Note = "Failed";
                yoAppResponse.Description = "General Error. Check \"narrative\" in \"Narrative\" for error message";
                yoAppResponse.CustomerData = response.customerData;
                yoAppResponse.CustomerAccount = response.utilityAccount;
            }
            else if (response.responseCode == "12")
            {
                message = "Amount cannot cover debt due/Amount is below{above} the limit";
                success = false;

                yoAppResponse.ResponseCode = "00008";
                yoAppResponse.Note = "Failed";
                yoAppResponse.Description = "Amount cannot cover debt due/Amount is below{above} the limit";

                return yoAppResponse;
            }
            else if (response.responseCode == "14")
            {
                message = "Invalid meter number/Meter is blocked";
                success = false;

                yoAppResponse.ResponseCode = "00008";
                yoAppResponse.Note = "Failed";
                yoAppResponse.Description = "Invalid meter number/Meter is blocked";
                yoAppResponse.CustomerData = response.customerData;
                yoAppResponse.CustomerAccount = response.utilityAccount;
            }
            else if (response.responseCode == "51")
            {
                message = "Insufficient funds";
                success = false;

                yoAppResponse.ResponseCode = "00008";
                yoAppResponse.Note = "Failed";
                yoAppResponse.Description = "Insufficient funds";
                yoAppResponse.CustomerData = response.customerData;
                yoAppResponse.CustomerAccount = response.utilityAccount;
            }
            else if (response.responseCode == "57")
            {
                message = "Customer account has reversal in progress";
                success = false;

                yoAppResponse.ResponseCode = "00008";
                yoAppResponse.Note = "Failed";
                yoAppResponse.Description = "Customer account has reversal in progress";
                yoAppResponse.CustomerData = response.customerData;
                yoAppResponse.CustomerAccount = response.utilityAccount;
            }
            else if (response.responseCode == "63")
            {
                message = "Security violation";
                success = false;

                yoAppResponse.ResponseCode = "00008";
                yoAppResponse.Note = "Failed";
                yoAppResponse.Description = "Security violation";
                yoAppResponse.CustomerData = response.customerData;
                yoAppResponse.CustomerAccount = response.utilityAccount;
            }
            else if (response.responseCode == "68")
            {
                message = "Transaction timeout";
                success = false;

                yoAppResponse.ResponseCode = "00008";
                yoAppResponse.Note = "Failed";
                yoAppResponse.Description = "Transaction timeout";
                yoAppResponse.CustomerData = response.customerData;
                yoAppResponse.CustomerAccount = response.utilityAccount;
            }
            else if (response.responseCode == "94")
            {
                message = "Duplicate transaction";
                success = false;

                yoAppResponse.ResponseCode = "00008";
                yoAppResponse.Note = "Failed";
                yoAppResponse.Description = "Duplicate transaction";
                yoAppResponse.CustomerData = response.customerData;
                yoAppResponse.CustomerAccount = response.utilityAccount;
            }

            var serializedResponse = JsonConvert.SerializeObject(response, Formatting.Indented);
            yoAppResponse.Narrative = serializedResponse;

            return yoAppResponse;
        }

        public YoAppResponse RetrivePreviousToken(ESolutionsRequest eSolutionsRequest)
        {
            YoAppResponse yoAppResponse = new YoAppResponse();

            request.mti = eSolutionsRequest.mti;

            #region vendorReference
            var date = DateTime.Now.ToString();
            var dateTimeConverterd = DateTime.Parse(date, CultureInfo.InvariantCulture).ToString("MMddyyHHmmss");
            var reference = "YOAPP" + dateTimeConverterd;
            #endregion

            if (String.IsNullOrEmpty(request.vendorReference))
            {
                request.vendorReference = reference;
            }
            else
            {
                request.vendorReference = eSolutionsRequest.vendorReference;
            }

            request.processingCode = eSolutionsApiObjects.LastCustomerToken;

            #region transactionAmount
            var amount = Convert.ToString(eSolutionsRequest.transactionAmount);
            var apiAmount = amount.Replace(".", "");
            #endregion

            request.transactionAmount = Convert.ToInt64(apiAmount);

            #region transmissionDate
            var dateTime = DateTime.Now.ToString();
            var dateTimeNowConverted = DateTime.Parse(dateTime, CultureInfo.InvariantCulture).ToString("MMddyyHHmmss");
            #endregion

            if (String.IsNullOrEmpty(request.transmissionDate))
            {
                request.transmissionDate = dateTimeNowConverted;
            }
            else
            {
                request.transmissionDate = eSolutionsRequest.transmissionDate;
            }

            if (String.IsNullOrEmpty(request.vendorNumber))
            {
                request.vendorNumber = eSolutionsCredentials.VendorNumber;
            }
            else
            {
                request.vendorNumber = eSolutionsRequest.vendorNumber;
            }

            request.merchantName = eSolutionsRequest.merchantName;
            request.productName = eSolutionsRequest.productName;
            request.utilityAccount = eSolutionsRequest.utilityAccount;
            request.sourceMobile = eSolutionsRequest.sourceMobile;
            request.targetMobile = eSolutionsRequest.targetMobile;
            request.serviceId = eSolutionsRequest.serviceId;

            if (String.IsNullOrEmpty(eSolutionsRequest.currencyCode))
            {
                request.currencyCode = "ZWL";
            }
            else
            {
                request.currencyCode = eSolutionsRequest.currencyCode;
            }

            if (eSolutionsRequest.merchantName == "ZETDC" && String.IsNullOrEmpty(eSolutionsRequest.aggregator))
            {
                request.aggregator = "POWERTEL";
            }
            else
            {
                request.aggregator = eSolutionsRequest.aggregator;
            }

            ESolutionsLog.Request(request);

            var response = eSolutionsConnector.GetESolutionsResponse(request);

            if (response.responseCode == "00" || response.responseCode == "09") // Transaction Successful or Request still in Progress
            {
                if (response.responseCode == "00")
                {
                    message = "Transaction Successful";
                    success = true;

                    yoAppResponse.ResponseCode = "00000";
                    yoAppResponse.Note = "Success";
                    yoAppResponse.Description = response.narrative;
                    yoAppResponse.CustomerData = response.customerData;
                    yoAppResponse.CustomerAccount = response.utilityAccount;

                    ESolutionsLog.Response(response);
                }
                if (response.responseCode == "09") // 
                {
                    message = "Transaction still in progress";
                    success = false;

                    yoAppResponse.ResponseCode = "00000";
                    yoAppResponse.Note = "Success";
                    yoAppResponse.Description = response.narrative;
                    yoAppResponse.CustomerData = response.customerData;
                    yoAppResponse.CustomerAccount = response.utilityAccount;

                    ESolutionsLog.Response(response);
                }
            }
            else if (response.responseCode == "05")
            {
                message = "General Error";
                success = false;

                yoAppResponse.ResponseCode = "00008";
                yoAppResponse.Note = "Failed";
                yoAppResponse.Description = "General Error. Check \"narrative\" in \"Narrative\" for error message";
                yoAppResponse.CustomerData = response.customerData;
                yoAppResponse.CustomerAccount = response.utilityAccount;

                ESolutionsLog.Response(response);
            }
            else if (response.responseCode == "12")
            {
                message = "Amount cannot cover debt due/Amount is below{above} the limit";
                success = false;

                yoAppResponse.ResponseCode = "00008";
                yoAppResponse.Note = "Failed";
                yoAppResponse.Description = "Amount cannot cover debt due/Amount is below{above} the limit";
                yoAppResponse.CustomerData = response.customerData;
                yoAppResponse.CustomerAccount = response.utilityAccount;

                ESolutionsLog.Response(response);
            }
            else if (response.responseCode == "14")
            {
                message = "Invalid meter number/Meter is blocked";
                success = false;

                yoAppResponse.ResponseCode = "00008";
                yoAppResponse.Note = "Failed";
                yoAppResponse.Description = "Invalid meter number/Meter is blocked";
                yoAppResponse.CustomerData = response.customerData;
                yoAppResponse.CustomerAccount = response.utilityAccount;

                ESolutionsLog.Response(response);
            }
            else if (response.responseCode == "51")
            {
                message = "Insufficient funds";
                success = false;

                yoAppResponse.ResponseCode = "00008";
                yoAppResponse.Note = "Failed";
                yoAppResponse.Description = "Insufficient funds";
                yoAppResponse.CustomerData = response.customerData;
                yoAppResponse.CustomerAccount = response.utilityAccount;

                ESolutionsLog.Response(response);
            }
            else if (response.responseCode == "57")
            {
                message = "Customer account has reversal in progress";
                success = false;

                yoAppResponse.ResponseCode = "00008";
                yoAppResponse.Note = "Failed";
                yoAppResponse.Description = "Customer account has reversal in progress";
                yoAppResponse.CustomerData = response.customerData;
                yoAppResponse.CustomerAccount = response.utilityAccount;

                ESolutionsLog.Response(response);
            }
            else if (response.responseCode == "63")
            {
                message = "Security violation";
                success = false;

                yoAppResponse.ResponseCode = "00008";
                yoAppResponse.Note = "Failed";
                yoAppResponse.Description = "Security violation";
                yoAppResponse.CustomerData = response.customerData;
                yoAppResponse.CustomerAccount = response.utilityAccount;

                ESolutionsLog.Response(response);
            }
            else if (response.responseCode == "68")
            {
                message = "Transaction timeout";
                success = false;

                yoAppResponse.ResponseCode = "00008";
                yoAppResponse.Note = "Failed";
                yoAppResponse.Description = "Transaction timeout";
                yoAppResponse.CustomerData = response.customerData;
                yoAppResponse.CustomerAccount = response.utilityAccount;

                ESolutionsLog.Response(response);
            }
            else if (response.responseCode == "94")
            {
                message = "Duplicate transaction";
                success = false;

                yoAppResponse.ResponseCode = "00008";
                yoAppResponse.Note = "Failed";
                yoAppResponse.Description = "Duplicate transaction";
                yoAppResponse.CustomerData = response.customerData;
                yoAppResponse.CustomerAccount = response.utilityAccount;

                ESolutionsLog.Response(response);
            }

            var serializedResponse = JsonConvert.SerializeObject(response, Formatting.Indented);
            yoAppResponse.Narrative = serializedResponse;

            return yoAppResponse;
        }

        public bool ResendTransaction(ESolutionsRequest eSolutionsRequest)
        {
           request.mti = eSolutionsRequest.mti;
                        
            request.originalReference = eSolutionsApiObjects.VendorReference;
            request.processingCode = eSolutionsApiObjects.TokenPurchaseRequest;

            #region transactionAmount
            var amount = Convert.ToString(eSolutionsRequest.transactionAmount);
            var apiAmount = amount.Replace(".", "");            
            #endregion

            request.transactionAmount = Convert.ToInt64(apiAmount);

            #region transmissionDate
            var dateTimeNow = DateTime.Now.ToString();
            var dateTimeNowConverted = DateTime.Parse(dateTimeNow, CultureInfo.InvariantCulture).ToString("MMddyyHHmmss");
            #endregion

            request.transmissionDate = dateTimeNowConverted;
            request.vendorNumber = eSolutionsCredentials.VendorNumber;
            request.vendorTerminalId = eSolutionsRequest.vendorTerminalId;
            request.merchantName = eSolutionsRequest.merchantName;
            request.productName = eSolutionsRequest.productName;
            request.utilityAccount = eSolutionsRequest.utilityAccount;
            request.currencyCode = eSolutionsRequest.currencyCode;

            if (request.merchantName == "ZETDC")
            {
                request.aggregator = "POWERTEL";
            }

            var response = eSolutionsConnector.GetESolutionsResponse(request);

            if (response.responseCode == "00") // Transaction Successful
            {
                message = "Transaction Successful";
                return true;
            }

            if (response.responseCode == "09") // Request still in progress 
            {
                message = "Transaction still in progress";
                return false;
            }

            return false;
        }
    }
}