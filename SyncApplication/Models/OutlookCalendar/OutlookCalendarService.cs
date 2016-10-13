using Microsoft.Office365.OutlookServices;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SyncApplication.Models.OutlookCalendar
{
    public class OutlookCalendarService
    {
        public const string _BaseUrl = "https://outlook.office.com/api/v2.0";
        private string _AccessToken;

        public OutlookCalendarService(string userToken)
        {
            this._AccessToken = userToken;
        }

        public List<CalendarEvent> GetEvents(string UserEmailAddress)
        {
            var lstCalendarEvent = new List<CalendarEvent>();

            var lstEvents = new List<Event>();

            var client = new RestClient(_BaseUrl);
            var request = new RestRequest("/me/events", Method.GET);

            request.AddHeader("Authorization", _AccessToken);
            request.AddHeader("Prefer", "outlook.timezone=\"Pakistan Standard Time\"");

            IRestResponse response = client.Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var jsonResponse = JObject.Parse(response.Content);
                var res = JsonConvert.DeserializeObject<List<Event>>(jsonResponse["value"].ToString());


                //"skip_token"
            }
            return lstCalendarEvent;
        }

        public CalendarEvent InsertEvent(CalendarEvent objCalendarEvent)
        {
            return new CalendarEvent();
        }

        public CalendarEvent UpdateEvent(CalendarEvent objCalendarEvent)
        {
            return new CalendarEvent();
        }

        public CalendarEvent DeleteEvent(CalendarEvent objCalendarEvent)
        {
            return new CalendarEvent();
        }
    }
}