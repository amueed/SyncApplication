using System.Configuration;

namespace SyncApplication.Models.Settings
{
    public sealed class GoogleSyncSettings
    {
        public static string ApplicationName
        {
            get { return ConfigurationManager.AppSettings["GOOGLE.APP_NAME"].ToString(); }
        }
        public static string ClientId
        {
            get { return ConfigurationManager.AppSettings["GOOGLE.CLIENT_ID"].ToString(); }
        }
        public static string ClientSecret
        {
            get { return ConfigurationManager.AppSettings["GOOGLE.CLIENT_SECRET"].ToString(); }
        }
        public static string BaseUrl
        {
            get { return ConfigurationManager.AppSettings["GOOGLE.BASE_URL"].ToString(); }
        }
        public static string AuthUrl
        {
            get { return ConfigurationManager.AppSettings["GOOGLE.AUTH_URL"].ToString(); }
        }
        public static string TokenUrl
        {
            get { return ConfigurationManager.AppSettings["GOOGLE.TOKEN_URL"].ToString(); }
        }
        public static string RedirectUrl
        {
            get { return ConfigurationManager.AppSettings["GOOGLE.REDIRECT_URL"].ToString(); }
        }
    }
}