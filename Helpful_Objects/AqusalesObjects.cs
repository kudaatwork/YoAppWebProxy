using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Helpful_Objects
{
    public class AqusalesObjects
    {
        private static string customerAccountType = "2-03-0001";

        public static string CustomerAccountType
        {
            get
            {
                return customerAccountType;
            }
        }

        private static string transactionName = "TRADE DEBTORS USD";

        public static string TransactionName
        {
            get
            {
                return transactionName;
            }
        }

        private static string supplierAccountType = "9-01-0001";

        public static string SupplierAccountType
        {
            get
            {
                return supplierAccountType;
            }
        }

        private static string ohlangaSupplierAccountType = "9-01-0002";

        public static string OhlangaSupplierAccountType
        {
            get
            {
                return ohlangaSupplierAccountType;
            }
        }

        private static string stockTranName = "TRADE DEBTORS USD";

        public static string StockTranName
        {
            get
            {
                return stockTranName;
            }
        }
    }
}    
