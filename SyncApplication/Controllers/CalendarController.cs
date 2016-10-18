using SyncApplication.Models;
using SyncApplication.Models.GoogleCalendar;
using SyncApplication.Models.OutlookCalendar;
using SyncApplication.Models.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SyncApplication.Controllers
{
    public class CalendarController : Controller
    {
        private GoogleCalendarBL GetGoogleCalendarBLObj()
        {
            return new GoogleCalendarBL(DatabaseSettings.DefaultConnectionString, new AppCredentials
            {
                AppName = GoogleSyncSettings.ApplicationName,
                ClientId = GoogleSyncSettings.ClientId,
                ClientSecret = GoogleSyncSettings.ClientSecret,
                BaseUrl = GoogleSyncSettings.BaseUrl,
                AuthUrl = GoogleSyncSettings.AuthUrl,
                TokenUrl = GoogleSyncSettings.TokenUrl,
                RedirectUri = GoogleSyncSettings.RedirectUrl
            });
        }

        private OutlookCalendarBL GetOutlookCalendarBLObj()
        {
            return new OutlookCalendarBL(DatabaseSettings.DefaultConnectionString, new AppCredentials
            {
                AppName = OutlookSyncSettings.ApplicationName,
                ClientId = OutlookSyncSettings.ClientId,
                ClientSecret = OutlookSyncSettings.ClientSecret,
                BaseUrl = OutlookSyncSettings.BaseUrl,
                AuthUrl = OutlookSyncSettings.AuthUrl,
                TokenUrl = OutlookSyncSettings.TokenUrl,
                RedirectUri = OutlookSyncSettings.RedirectUrl
            });
        }
        // GET: Calendar
        public async Task<ActionResult> Index()
        {
            //string userEmail = "mueed.bpit@gmail.com";
            //string emailServer = EmailServer.Google.ToString();

            string userEmail = "ngage@broadpeak.com";
            string emailServer = EmailServer.Office365.ToString();

            if (emailServer == EmailServer.Google.ToString())
            {
                if (GetGoogleCalendarBLObj().IsTokenExist(userEmail))
                {
                    List<CalendarEvent> res = GetGoogleCalendarBLObj().GetEvents(userEmail);
                }
                else
                {
                    return Redirect(GetGoogleCalendarBLObj().GetLoginUrl());
                }
            }
            else if (emailServer == EmailServer.Outlook.ToString() || emailServer == EmailServer.Office365.ToString())
            {
                if (GetOutlookCalendarBLObj().IsTokenExist(userEmail))
                {
                    List<CalendarEvent> res = await GetOutlookCalendarBLObj().GetEvents(userEmail);
                }
                else
                {
                    return Redirect(GetOutlookCalendarBLObj().GetLoginUrl());
                }
            }
            else
            {

            }
            return View();
        }
    }
}