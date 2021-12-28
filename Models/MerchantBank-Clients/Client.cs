using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.MerchantBank
{
    public class Client
    {
        public string dateCreated { get; set; }
        public string dateModified { get; set; }
        public Nullable<int> version { get; set; }
        public bool deleted { get; set; }
        public Nullable<int> id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string phoneNumber { get; set; }
        public string idPhotoPath { get; set; }
        public string photoWithIdPath { get; set; }
        public string idTypeName { get; set; }
        public string dateOfBirth { get; set; }
        public string gender { get; set; }
        public string nationalId { get; set; }
        public string country { get; set; }
        public string street { get; set; }
        public string city { get; set; }
        public string suburb { get; set; }
        public Nullable<int> userId { get; set; }
        public string status { get; set; }        
    }
}