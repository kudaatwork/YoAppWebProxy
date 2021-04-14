using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy
{
    public class ESolutionsApiObjects
    {
        #region MTIs
        private const string transactionRequestMTI = "0200";

        public  string TransactionRequestMTI
        {
            get { return transactionRequestMTI; }
        }

        private string transactionResponseMTI = "0210";

        public string TransactionResponseMTI
        {
            get { return transactionResponseMTI; }
        }

        private string transactionResendRequestMTI = "0201";

        public string TransactionResendRequestMTI
        {
            get { return transactionResendRequestMTI; }
        }

        private string transactionResendResponseMTI = "0211";

        public string TransactionResendResponseMTI
        {
            get { return transactionResendResponseMTI; }
        }
        #endregion

        #region ProcessingCodes
        private string vendorBalanceEnquiry = "300000";

        public string VendorBalanceEnquiry
        {
            get { return vendorBalanceEnquiry; }
        }

        private string customerInformation = "310000";

        public string CustomerInformation
        {
            get { return customerInformation; }
        }

        private string lastCustomerToken = "320000";

        public string LastCustomerToken
        {
            get { return lastCustomerToken; }
        }

        private string tokenPurchaseRequest = "U50000";

        public string TokenPurchaseRequest
        {
            get { return tokenPurchaseRequest; }
        }

        private string directPaymentRequest = "520000";

        public string DirectPaymentRequest
        {
            get { return directPaymentRequest; }
        }
        #endregion

        public string VendorReference { get; set; }
    }
}