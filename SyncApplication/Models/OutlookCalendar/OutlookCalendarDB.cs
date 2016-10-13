using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SyncApplication.Models.OutlookCalendar
{
    public class OutlookCalendarDB
    {
        private static SyncDbDataContext GetDbContextObj()
        {
            return new SyncDbDataContext();
        }
    }
}