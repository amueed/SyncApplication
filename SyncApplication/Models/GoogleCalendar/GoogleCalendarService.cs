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
        public GoogleCalendarService(AppCredentials objAppCredentials, UserToken objUserToken)
        {
            _AppCredentials = objAppCredentials;
            _CalendarService = GetCalendarServiceObj(objUserToken);
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

        public List<CalendarEvent> GetEvents()
        {
            var lstCalendarEvent = new List<CalendarEvent>();
            try
            {
                //execute get request
                EventsResource.ListRequest requestEventsList = _CalendarService.Events.List("primary");
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
                            Title = item.Summary,
                            StartDate = item.Start.DateTime != null ? item.Start.DateTime : Convert.ToDateTime(item.Start.Date + " 12:00:00 AM"),
                            EndDate = item.End.DateTime != null ? item.End.DateTime : Convert.ToDateTime(item.End.Date + " 11:59:59 PM"),
                            Reminder = 0,
                            ReminderType = "Email",
                            AttendeeEmailAddress = item.Attendees != null ? item.Attendees[0].Email : ""
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
            try
            {
                string SyncedEventId = Guid.NewGuid().ToString().Replace("-", "");

                var googleEvent = new Event();

                googleEvent.Id = SyncedEventId;
                googleEvent.Summary = objCalendarEvent.Title;
                googleEvent.Description = objCalendarEvent.Description;
                googleEvent.Start = new EventDateTime
                {
                    DateTime = objCalendarEvent.StartDate,
                    //TimeZone = TimeZoneInfo.Local.Id
                };
                googleEvent.End = new EventDateTime
                {
                    DateTime = objCalendarEvent.EndDate,
                    //TimeZone = TimeZoneInfo.Local.Id
                };
                googleEvent.Location = objCalendarEvent.Location;
                googleEvent.Status = "confirmed";
                googleEvent.Locked = false;
                googleEvent.Attendees = new List<EventAttendee>()
                {
                    new EventAttendee
                    {
                        Email = objCalendarEvent.AttendeeEmailAddress,

                    }
                };
                googleEvent.Reminders = new Event.RemindersData
                {
                    UseDefault = false,
                    Overrides = new[]
                        {
                            new EventReminder
                            {
                                Method = objCalendarEvent.ReminderType,
                                Minutes = objCalendarEvent.Reminder
                            }
                        }
                };
                googleEvent.Visibility = "default";
                googleEvent.GuestsCanModify = false;
                googleEvent.GuestsCanInviteOthers = false;
                googleEvent.GuestsCanSeeOtherGuests = false;

                EventsResource.InsertRequest objInsertRequest = _CalendarService.Events.Insert(googleEvent, "primary");
                objInsertRequest.SendNotifications = true;
                Event createdEvent = objInsertRequest.Execute();
                return SyncedEventId;
            }
            catch (Exception ex)
            {
                return "error";
            }
        }

        public bool UpdateEvent(string SyncedEventId, CalendarEvent objCalendarEvent)
        {
            try
            {
                var googleEvent = new Event();
                //Id = SyncedEventId,
                googleEvent.Summary = "";
                googleEvent.Description = "";
                googleEvent.Start = new EventDateTime
                {
                    DateTime = DateTime.Now,
                    TimeZone = TimeZone.CurrentTimeZone.StandardName
                };
                googleEvent.End = new EventDateTime
                {
                    DateTime = DateTime.Now,
                    TimeZone = TimeZone.CurrentTimeZone.StandardName
                };
                googleEvent.Location = "";
                googleEvent.Status = "";
                googleEvent.Locked = false;
                googleEvent.Attendees = new List<EventAttendee>()
                {
                    new EventAttendee {
                        Email = ""
                    }
                };
                googleEvent.Reminders = new Event.RemindersData
                {
                    UseDefault = false,
                    Overrides = new[]
                    {
                        new EventReminder
                        {
                            Method = "Email",
                            Minutes = 30
                        }
                    }
                };


                EventsResource.UpdateRequest objUpdateRequest = _CalendarService.Events.Update(googleEvent, "primary", SyncedEventId);
                objUpdateRequest.SendNotifications = true;
                Event updatedEvent = objUpdateRequest.Execute();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DeleteEvent(string SyncedEventId)
        {
            try
            {
                EventsResource.DeleteRequest objDeleteRequest = _CalendarService.Events.Delete("primary", SyncedEventId);
                objDeleteRequest.SendNotifications = true;
                string deletedEvent = objDeleteRequest.Execute();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}