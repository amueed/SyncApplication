using SyncApplication.Models;
using SyncApplication.Models.GoogleCalendar;
using SyncApplication.Models.OutlookCalendar;
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
        // GET: Calendar
        public async Task<ActionResult> Index()
        {
            //string userEmail = "mueed.bpit@gmail.com";
            //string emailServer = EmailServer.Google.ToString();

            string userEmail = "ngage@broadpeak.com";
            string emailServer = EmailServer.Office365.ToString();

            if (emailServer == EmailServer.Google.ToString())
            {
                if (GoogleCalendarBL.IsTokenExist(userEmail))
                {
                    List<CalendarEvent> res = GoogleCalendarBL.GetEvents(userEmail);
                }
                else
                {
                    return Redirect(GoogleCalendarBL.GetLoginUrl());
                }
            }
            else if (emailServer == EmailServer.Outlook.ToString() || emailServer == EmailServer.Office365.ToString())
            {
                if (OutlookCalendarBL.IsTokenExist(userEmail))
                {
                    List<CalendarEvent> res = await OutlookCalendarBL.GetEvents(userEmail);
                }
                else
                {
                    return Redirect(OutlookCalendarBL.GetLoginUrl());
                }
            }
            else
            {

            }
            return View();
        }
    }
}