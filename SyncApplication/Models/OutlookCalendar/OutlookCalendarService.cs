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

        public OutlookCalendarService(AppCredentials objAppCredentials, UserToken objUserToken)
        {
            _AccessToken = objUserToken.AccessToken;
            _AppCredentials = objAppCredentials;
            _OutlookClient = new OutlookServicesClient(new Uri(objAppCredentials.BaseUrl), GetAccessTokenAsync);
        }

        public async Task<string> InsertEventAsync(CalendarEvent objCalendarEvent)
        {
            try
            {
                string SyncedEventId = Guid.NewGuid().ToString();
                // Create the event object
                var outlookEvent = new Event();
                outlookEvent.Id = SyncedEventId;
                outlookEvent.Subject = objCalendarEvent.Title;
                outlookEvent.Start = new DateTimeTimeZone()
                {
                    //TimeZone = TimeZone.CurrentTimeZone.StandardName,
                    DateTime = objCalendarEvent.StartDate.ToString()
                };
                outlookEvent.End = new DateTimeTimeZone()
                {
                    //TimeZone = TimeZone.CurrentTimeZone.StandardName,
                    DateTime = objCalendarEvent.StartDate.ToString()
                };
                outlookEvent.Body = new ItemBody
                {
                    ContentType = BodyType.Text,
                    Content = objCalendarEvent.Description,
                };
                outlookEvent.Location = new Location
                {
                    DisplayName = objCalendarEvent.Location
                };
                outlookEvent.Attendees = new List<Attendee>
                {
                    new Attendee
                    {
                        Type = AttendeeType.Required,
                        EmailAddress = new EmailAddress
                        {
                            Address = objCalendarEvent.AttendeeEmailAddress
                        },
                    }
                };
                outlookEvent.IsReminderOn = true;
                outlookEvent.ReminderMinutesBeforeStart = objCalendarEvent.Reminder;

                await _OutlookClient.Me.Calendar.Events.AddEventAsync(outlookEvent);
                return SyncedEventId;
            }
            catch (Exception ex)
            {
                return "error";
            }
        }
        public async Task<bool> UpdateEventAsync(string SyncedEventId, CalendarEvent objCalendarEvent)
        {
            try
            {
                IEvent eventToUpdate = await _OutlookClient.Me.Events[SyncedEventId].ExecuteAsync();

                eventToUpdate.Subject = objCalendarEvent.Title;
                eventToUpdate.Start = new DateTimeTimeZone()
                {
                    //TimeZone = TimeZone.CurrentTimeZone.StandardName,
                    DateTime = objCalendarEvent.StartDate.ToString()
                };
                eventToUpdate.End = new DateTimeTimeZone()
                {
                    //TimeZone = TimeZone.CurrentTimeZone.StandardName,
                    DateTime = objCalendarEvent.StartDate.ToString()
                };
                eventToUpdate.Body = new ItemBody
                {
                    ContentType = BodyType.Text,
                    Content = objCalendarEvent.Description,
                };
                eventToUpdate.Location = new Location
                {
                    DisplayName = objCalendarEvent.Location
                };
                eventToUpdate.Attendees = new List<Attendee>
                {
                    new Attendee
                    {
                        Type = AttendeeType.Required,
                        EmailAddress = new EmailAddress
                        {
                            Address = objCalendarEvent.AttendeeEmailAddress
                        },
                    }
                };
                eventToUpdate.IsReminderOn = true;
                eventToUpdate.ReminderMinutesBeforeStart = objCalendarEvent.Reminder;

                // Commit all changes to the event
                await eventToUpdate.UpdateAsync();

                return true;
            }
            catch (Exception ex)
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