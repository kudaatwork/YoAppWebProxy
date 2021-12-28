using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YoAppWebProxy.Models.MerchantBank_Login
{
    public class ResultDTO
    {
        public Nullable<int> agentId { get; set; }
        public Nullable<int> clientId { get; set; }
        public List<Role> roles { get; set; }
        public Nullable<int> id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string refresh_token { get; set; }
        public string scope { get; set; }
        public string expires_in { get; set; }
    }
}