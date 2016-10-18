using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SyncApplication.Models
{
    public class CalendarRepository
    {
        private string _ConnectionString;
        public CalendarRepository(string ConnectionString)
        {
            _ConnectionString = ConnectionString;
        }
        private SyncDbDataContext GetDbContextObj()
        {
            return new SyncDbDataContext(_ConnectionString);
        }
        public List<CalendarEvent> GetEvents(string UserEmail)
        {
            var lstCalendarEvent = new List<CalendarEvent>();
            using (var db = GetDbContextObj())
            {
                //query
                return lstCalendarEvent;
            }
        }
        public string InsertEvent(CalendarEvent objCalendarEvent)
        {
            //TODO: Insert into Local DB
            return "";
        }
        public bool UpdateEvent(string EventId, CalendarEvent objCalendarEvent)
        {
            //TODO: Update into Local DB
            return true;
        }
        public bool DeleteEvent(string EventId)
        {
            //TODO: Delete into Local DB
            return true;
        }
    }
}