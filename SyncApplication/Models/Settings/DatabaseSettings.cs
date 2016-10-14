using System.Configuration;

namespace SyncApplication.Models.Settings
{
    public sealed class DatabaseSettings
    {
        public static string DefaultConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString(); }
        }
    }
}