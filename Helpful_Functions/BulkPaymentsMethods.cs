using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using YoAppWebProxy.Logs;

namespace YoAppWebProxy.Helpful_Functions
{
    public class BulkPaymentsMethods
    {
        public string ReferenceNumber(string serviceProvider)
        {
            var date = DateTime.Now.ToString();
            var dateTimeConverterd = DateTime.Parse(date, CultureInfo.InvariantCulture).ToString("MMddyyHHmmss");
            var refNumber = "NDA" + dateTimeConverterd;

            Log.RequestsAndResponses("RefNo.", serviceProvider, refNumber);

            return refNumber;
        }

        public string Encrypt(string plainText, string key)
        {
            TripleDESCryptoServiceProvider desCryptoProvider = new
            TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider hashMD5Provider = new MD5CryptoServiceProvider();
            byte[] byteHash;
            byte[] byteBuff;
            byteHash = hashMD5Provider.ComputeHash(Encoding.UTF8.GetBytes(key));
            desCryptoProvider.Key = byteHash;
            desCryptoProvider.Mode = CipherMode.ECB;
            byteBuff = Encoding.UTF8.GetBytes(plainText);
            string encoded = Convert.ToBase64String(desCryptoProvider.CreateEncryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
            return encoded;
        }

        public string Decrypt(string encodedText, string key)
        {
            TripleDESCryptoServiceProvider desCryptoProvider = new
            TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider hashMD5Provider = new MD5CryptoServiceProvider();
            byte[] byteHash;
            byte[] byteBuff;
            byteHash = hashMD5Provider.ComputeHash(Encoding.UTF8.GetBytes(key));
            desCryptoProvider.Key = byteHash;
            desCryptoProvider.Mode = CipherMode.ECB;
            byteBuff = Convert.FromBase64String(encodedText);
            string plaintext = Encoding.UTF8.GetString(desCryptoProvider.CreateDecryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
            return plaintext;
        }
        

    }
}