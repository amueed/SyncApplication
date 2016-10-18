using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Http;
using Google.Apis.Services;
using System;
using System.Collections.Generic;

namespace SyncApplication.Models.GoogleCalendar
{
    public class GoogleCalendarService
    {
        private CalendarService _CalendarService;
        private AppCredentials _AppCredentials;

        public GoogleCalendarService(AppCredentials objAppCredentials)
        {
            _AppCredentials = objAppCredentials;
        }
        public GoogleCalendarService(UserToken objUserToken, AppCredentials objAppCredentials)
        {
            _CalendarService = GetCalendarServiceObj(objUserToken);
            _AppCredentials = objAppCredentials;
        }

        private CalendarService GetCalendarServiceObj(UserToken objUserToken)
        {
            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = _AppCredentials.ClientId,
                    ClientSecret = _AppCredentials.ClientSecret
                },
                Scopes = new[] { CalendarService.Scope.Calendar }
            });

            var credential = new UserCredential(flow, "me", new TokenResponse
            {
                AccessToken = objUserToken.AccessToken,
                RefreshToken = objUserToken.RefreshToken
            });

            return new CalendarService(new BaseClientService.Initializer
            {
                ApplicationName = _AppCredentials.AppName,
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

        public string InsertEvent(CalendarEvent objCalendarEvent)
        {
            string SyncedEventId = string.Empty;
            return SyncedEventId;
        }

        public bool UpdateEvent(CalendarEvent objCalendarEvent)
        {
            return true;
        }

        public bool DeleteEvent(CalendarEvent objCalendarEvent)
        {
            return true;
        }
    }
}