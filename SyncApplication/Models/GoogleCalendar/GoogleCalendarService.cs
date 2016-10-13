using Google.Apis.Calendar.v3;
using Google.Apis.Http;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Net.Http;
using System.Net.Http.Headers;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Responses;

namespace SyncApplication.Models.GoogleCalendar
{
    public class GoogleCalendarService
    {
        public const string _ClientID = "1083615727855-vgj45vpj5tqrb2e4m5tpk5gn0vuac0q5.apps.googleusercontent.com";
        public const string _ClientSecret = "fVAoZ3ved3ttjZSucvFuTE9L";

        private CalendarService _CalendarService { get; set; }

        public GoogleCalendarService(OAuthToken objOAuthToken)
        {
            _CalendarService = GetCalendarServiceObj(objOAuthToken);
        }

        private CalendarService GetCalendarServiceObj(OAuthToken objOAuthToken)
        {
            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = _ClientID,
                    ClientSecret = _ClientSecret
                },
                Scopes = new[] { CalendarService.Scope.Calendar }
            });

            var credential = new UserCredential(flow, "me", new TokenResponse
            {
                AccessToken = objOAuthToken.AccessToken,
                RefreshToken = objOAuthToken.RefreshToken
            });

            return new CalendarService(new BaseClientService.Initializer
            {
                ApplicationName = "emax-sync",
                HttpClientInitializer = credential,
                DefaultExponentialBackOffPolicy = ExponentialBackOffPolicy.Exception | ExponentialBackOffPolicy.UnsuccessfulResponse503
            });
        }

        public List<CalendarEvent> GetEvents(string UserEmailAddress)
        {
            var lstCalendarEvent = new List<CalendarEvent>();
            try
            {
                //execute get request
                EventsResource.ListRequest requestEventsList = _CalendarService.Events.List(UserEmailAddress);
                string pageToken = null;
                Events feed;
                do
                {
                    requestEventsList.PageToken = pageToken;
                    /// Set time of last sync calendar from gmail account///
                    requestEventsList.TimeMin = DateTime.Now;
                    /// End ///
                    feed = requestEventsList.Execute();
                    foreach (Event item in feed.Items)
                    {
                        lstCalendarEvent.Add(new CalendarEvent()
                        {
                            EventId = item.Id,
                            Description = item.Description,
                            Location = item.Location,
                            Summary = item.Summary,
                            StartDate = item.Start.DateTime != null ? item.Start.DateTime : Convert.ToDateTime(item.Start.Date + " 12:00:00 AM"),
                            EndDate = item.End.DateTime != null ? item.End.DateTime : Convert.ToDateTime(item.End.Date + " 11:59:59 PM"),
                            Reminder = 0,
                            ReminderType = 0,
                            AttendeeEmailAddress = item.Attendees != null ? item.Attendees[0].ResponseStatus : ""
                        });
                    }
                    pageToken = feed.NextPageToken;
                }
                while (pageToken != null);
                //feed.NextSyncToken
                return lstCalendarEvent;
            }
            catch (Exception ex)
            {
                return lstCalendarEvent;
            }
        }

        public CalendarEvent InsertEvent(CalendarEvent objCalendarEvent) {
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