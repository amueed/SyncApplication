using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SyncApplication.Models
{
    public sealed class ApplicationCredentials
    {
        public string ApplicationName { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}