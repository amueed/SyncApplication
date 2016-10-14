using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SyncApplication.Models.Settings
{
    public sealed class OutlookSyncSettings
    {
        public static string ApplicationName
        {
            get { return ConfigurationManager.ConnectionStrings["OUTLOOK.APP_NAME"].ToString(); }
        }
        public static string ClientId
        {
            get { return ConfigurationManager.ConnectionStrings["OUTLOOK.CLIENT_ID"].ToString(); }
        }
        public static string ClientSecret
        {
            get { return ConfigurationManager.ConnectionStrings["OUTLOOK.CLIENT_SECRET"].ToString(); }
        }
        public static string BaseUrl
        {
            get { return ConfigurationManager.ConnectionStrings["OUTLOOK.BASE_URL"].ToString(); }
        }
        public static string AuthUrl
        {
            get { return ConfigurationManager.ConnectionStrings["OUTLOOK.AUTH_URL"].ToString(); }
        }
        public static string TokenUrl
        {
            get { return ConfigurationManager.ConnectionStrings["OUTLOOK.TOKEN_URL"].ToString(); }
        }
        public static string RedirectUrl
        {
            get { return ConfigurationManager.ConnectionStrings["OUTLOOK.REDIRECT_URL"].ToString(); }
        }
    }
}