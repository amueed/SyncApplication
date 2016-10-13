using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SyncApplication.Models
{
    public class OAuthToken
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string TokenType { get; set; }
        public DateTime IssueOn { get; set; }
        public int ExpiresIn { get; set; }
    }
}