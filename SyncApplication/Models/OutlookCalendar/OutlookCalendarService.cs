using Microsoft.OData.ProxyExtensions;
using Microsoft.Office365.OutlookServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SyncApplication.Models.OutlookCalendar
{
    public class OutlookCalendarService
    {
        private string _AccessToken = string.Empty;
        private OutlookServicesClient _OutlookClient;
        private AppCredentials _AppCredentials;

        public OutlookCalendarService(UserToken objUserToken, AppCredentials objAppCredentials)
        {
            _AccessToken = objUserToken.AccessToken;
            _AppCredentials = objAppCredentials;
            _OutlookClient = new OutlookServicesClient(new Uri(objAppCredentials.BaseUrl), GetAccessTokenAsync);
        }

        public async Task<string> InsertEventAsync(CalendarEvent objCalendarEvent)
        {
            // Create an attendee for the event    
            Attendee[] attendees =
            {
                new Attendee
                {
                    Type = AttendeeType.Required,
                    EmailAddress = new EmailAddress
                    {
                        Address = "katiej@a830edad9050849NDA1.onmicrosoft.com"
                    },
                }
            };

            // Create the event object
            var newEvent = new Event
            {
                Subject = "Sync up",
                Location = new Location
                {
                    DisplayName = "Water cooler"
                },
                Attendees = attendees,
                Start = new DateTimeTimeZone()
                {
                    TimeZone = TimeZoneInfo.Local.Id,
                    DateTime = new DateTime(2015, 12, 1, 9, 30, 0).ToString("s")
                },
                End = new DateTimeTimeZone()
                {
                    TimeZone = TimeZoneInfo.Local.Id,
                    DateTime = new DateTime(2015, 12, 1, 10, 30, 0).ToString("s")
                },
                Body = new ItemBody
                {
                    Content = "Status updates, blocking issues, and next steps",
                    ContentType = BodyType.Text
                }
            };

            await _OutlookClient.Me.Calendar.Events.AddEventAsync(newEvent);
            // Get the event ID.
            //string objCalendarEvent.SyncedEventId = newEvent.Id;
            return newEvent.Id;
        }
        public async Task<bool> UpdateEventAsync(string SyncedEventId, CalendarEvent objCalendarEvent)
        {
            try
            {
                IEvent eventToUpdate = await _OutlookClient.Me.Events[SyncedEventId].ExecuteAsync();

                // Add attendees
                eventToUpdate.Attendees.Add(new Attendee
                {
                    Type = AttendeeType.Required,
                    EmailAddress = new EmailAddress
                    {
                        Address = "garthf@a830edad9050849NDA1.onmicrosoft.com",
                    },
                });

                // Make other changes
                eventToUpdate.Subject = "New event name";
                eventToUpdate.Location.DisplayName = "New Location";

                // Commit all changes to the event
                await eventToUpdate.UpdateAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> DeleteEventAsync(string SyncedEventId)
        {
            try
            {
                // Get an existing event by ID
                IEvent eventToDelete = await _OutlookClient.Me.Events[SyncedEventId].ExecuteAsync();

                //Delete the event
                await eventToDelete.DeleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<List<CalendarEvent>> GetEventsAsync()
        {
            var lstEvents = new List<CalendarEvent>();
            try
            {
                var eventsResults = await _OutlookClient.Me.Calendar.Events.ExecuteAsync();

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