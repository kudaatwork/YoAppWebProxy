using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.MerchantBank
{
    public class RegisterClientRequest
    {
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
        public int agentId { get; set; }
    }
}