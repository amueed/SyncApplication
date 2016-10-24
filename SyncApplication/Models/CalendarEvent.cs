using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SyncApplication.Models
{
    public class CalendarEvent
    {
        public string EventId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsReminderOn { get; set; }
        public string ReminderType { get; set; }
        public int? Reminder { get; set; }
        public string AttendeeEmailAddress { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public bool? IsCancelled { get; set; }
        public bool? IsUpdated { get; set; }
        public string SyncedEventId { get; set; }
    }
}