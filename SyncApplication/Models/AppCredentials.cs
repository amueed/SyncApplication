using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SyncApplication.Models
{
    public class AppCredentials
    {
        public string AppName { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string BaseUrl { get; set; }
        public string AuthUrl { get; set; }
        public string TokenUrl { get; set; }
        public string RedirectUri { get; set; }
    }
}