using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SyncApplication.Models
{
    public class CalendarEvent
    {
        public string EventId { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Reminder { get; set; }
        public int? ReminderType { get; set; }
        public string AttendeeEmailAddress { get; set; }
    }
}