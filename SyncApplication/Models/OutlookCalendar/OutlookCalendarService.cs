using Microsoft.Office365.OutlookServices;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Microsoft.OData.ProxyExtensions;

namespace SyncApplication.Models.OutlookCalendar
{
    public class OutlookCalendarService
    {
        public const string _BaseUrl = "https://outlook.office.com/api/v2.0";
        private string _AccessToken;
        private OutlookServicesClient _OutlookClient;

        public OutlookCalendarService(string userToken)
        {
            this._AccessToken = userToken;
            _OutlookClient = new OutlookServicesClient(new Uri(_BaseUrl), GetAccessTokenAsync);
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
        public async Task<List<CalendarEvent>> GetEventsAsync()
        {
            var lstEvents = new List<CalendarEvent>();
            try
            {
                //var client = new OutlookServicesClient(new Uri(_BaseUrl), GetAccessTokenAsync);

                IPagedCollection<IEvent> eventsResults = await _OutlookClient.Me.Calendar.Events.ExecuteAsync();

                foreach (IEvent item in eventsResults.CurrentPage)
                {
                    lstEvents.Add(new CalendarEvent
                    {
                        EventId = item.Id,
                        Description = item.Subject,
                        Location = item.Location.DisplayName,
                        StartDate = Convert.ToDateTime(item.Start.DateTime),
                        EndDate = Convert.ToDateTime(item.End.DateTime),
                        AttendeeEmailAddress = item.Attendees[0].EmailAddress.Address
                    });
                }
                while (eventsResults.MorePagesAvailable)
                {
                    eventsResults = await eventsResults.GetNextPageAsync();
                    foreach (IEvent item in eventsResults.CurrentPage)
                    {
                        lstEvents.Add(new CalendarEvent
                        {
                            EventId = item.Id,
                            Description = item.Subject,
                            Location = item.Location.DisplayName,
                            StartDate = Convert.ToDateTime(item.Start.DateTime),
                            EndDate = Convert.ToDateTime(item.End.DateTime),
                            AttendeeEmailAddress = item.Attendees[0].EmailAddress.Address
                        });
                    }
                }
                return lstEvents;
            }
            catch (Exception ex)
            {
                return lstEvents;
            }
        }
        public async Task<List<CalendarEvent>> SyncEventsAsync()
        {
            var lstEvents = new List<CalendarEvent>();
            try
            {
                //var client = new OutlookServicesClient(new Uri(_BaseUrl), GetAccessTokenAsync);

                DateTime LastSync = (DateTime.Now).AddMinutes(-15);

                IPagedCollection<IEvent> eventsResults = await _OutlookClient.Me.Calendar.Events
                    .Where(e => e.LastModifiedDateTime > LastSync)
                    .ExecuteAsync();

                // You can access each event as follows.
                foreach (IEvent item in eventsResults.CurrentPage)
                {
                    lstEvents.Add(new CalendarEvent
                    {
                        EventId = item.Id,
                        Description = item.Subject,
                        Location = item.Location.DisplayName,
                        StartDate = Convert.ToDateTime(item.Start.DateTime),
                        EndDate = Convert.ToDateTime(item.End.DateTime),
                        AttendeeEmailAddress = item.Attendees[0].EmailAddress.Address
                    });
                }
                if (eventsResults.MorePagesAvailable)
                {
                    while (eventsResults.MorePagesAvailable)
                    {
                        eventsResults = await eventsResults.GetNextPageAsync();
                        foreach (IEvent item in eventsResults.CurrentPage)
                        {
                            lstEvents.Add(new CalendarEvent
                            {
                                EventId = item.Id,
                                Description = item.Subject,
                                Location = item.Location.DisplayName,
                                StartDate = Convert.ToDateTime(item.Start.DateTime),
                                EndDate = Convert.ToDateTime(item.End.DateTime),
                                AttendeeEmailAddress = item.Attendees[0].EmailAddress.Address
                            });
                        }
                    }
                }
                return lstEvents;
            }
            catch (Exception ex)
            {
                return lstEvents;
            }
        }
        private async Task<string> GetAccessTokenAsync()
        {
            return await Task.Run(() => _AccessToken);
        }
    }
}