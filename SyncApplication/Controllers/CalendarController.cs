using SyncApplication.Models;
using SyncApplication.Models.GoogleCalendar;
using SyncApplication.Models.OutlookCalendar;
using SyncApplication.Models.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SyncApplication.Controllers
{
    public class CalendarController : Controller
    {
        private GoogleCalendarBL GetGoogleCalendarBLObj()
        {
            return new GoogleCalendarBL(DatabaseSettings.DefaultConnectionString, new AppCredentials
            {
                AppName = GoogleSyncSettings.ApplicationName,
                ClientId = GoogleSyncSettings.ClientId,
                ClientSecret = GoogleSyncSettings.ClientSecret,
                BaseUrl = GoogleSyncSettings.BaseUrl,
                AuthUrl = GoogleSyncSettings.AuthUrl,
                TokenUrl = GoogleSyncSettings.TokenUrl,
                RedirectUri = GoogleSyncSettings.RedirectUrl
            });
        }

        private OutlookCalendarBL GetOutlookCalendarBLObj()
        {
            return new OutlookCalendarBL(DatabaseSettings.DefaultConnectionString, new AppCredentials
            {
                AppName = OutlookSyncSettings.ApplicationName,
                ClientId = OutlookSyncSettings.ClientId,
                ClientSecret = OutlookSyncSettings.ClientSecret,
                BaseUrl = OutlookSyncSettings.BaseUrl,
                AuthUrl = OutlookSyncSettings.AuthUrl,
                TokenUrl = OutlookSyncSettings.TokenUrl,
                RedirectUri = OutlookSyncSettings.RedirectUrl
            });
        }
        private string GetEmailFromIdToken(string token)
        {
            //id_token
            //token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6Ikk2b0J3NFZ6QkhPcWxlR3JWMkFKZEE1RW1YYyJ9.eyJhdWQiOiI5Y2YyOGRjNi1iYjNiLTRjYWItYTE3MC00ZWNhZWFjMWQ1MTUiLCJpc3MiOiJodHRwczovL2xvZ2luLm1pY3Jvc29mdG9ubGluZS5jb20vYjExMTliY2MtOWY5OC00YzRmLWFkNTUtMmMzNTVhNjE4ODg3L3YyLjAiLCJpYXQiOjE0NzY5NTY4MjAsIm5iZiI6MTQ3Njk1NjgyMCwiZXhwIjoxNDc2OTYwNzIwLCJuYW1lIjoibmdhZ2UgVGVhbSIsIm9pZCI6ImNhNzIwZWIwLTg0YzUtNGI0Ni1iNjAwLTBiOTExNjY4Y2YzNiIsInByZWZlcnJlZF91c2VybmFtZSI6Im5nYWdlQGJyb2FkcGVhay5jb20iLCJzdWIiOiJsa2VYR1NoSi1reVBadEZGYzJHdXZBQVcwRHE4ZEd4SG9ZVDA1WFVTM2Y4IiwidGlkIjoiYjExMTliY2MtOWY5OC00YzRmLWFkNTUtMmMzNTVhNjE4ODg3IiwidmVyIjoiMi4wIn0.ISkQCwJhc7oXHzSb4WvuttxBqMqd68e5QWvxrLUN1JxWtT-VV615t2QK60_Uf74S2sO5xmUrRM1-IZAGJFTSkIkUS2X0tIa2AfqOmjo6-zigO8P0ZWDnbPloriRbAhTGHxaazhobEfkEdWOLhD-GGSb38zwSyy76LQbsG7qnVciBY5KLmYyTNbB5MUR2XKmFeTgKa1KxDdtZDnqyoDdNVeh6LOiRSl6cocodWTi4xs7TfxAouwnaZt4M_9J8Ohk7ac1mG8537PbWBntuJR57jvJDp6fB2HXnGFU2KOVeasw6YihJM9w0B7i2Mhmf8OD585IBAu3FH3Y0DRQz9veqZg";
            //access_token
            //token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Ikk2b0J3NFZ6QkhPcWxlR3JWMkFKZEE1RW1YYyIsImtpZCI6Ikk2b0J3NFZ6QkhPcWxlR3JWMkFKZEE1RW1YYyJ9.eyJhdWQiOiJodHRwczovL291dGxvb2sub2ZmaWNlLmNvbSIsImlzcyI6Imh0dHBzOi8vc3RzLndpbmRvd3MubmV0L2IxMTE5YmNjLTlmOTgtNGM0Zi1hZDU1LTJjMzU1YTYxODg4Ny8iLCJpYXQiOjE0NzY5NTY4MjAsIm5iZiI6MTQ3Njk1NjgyMCwiZXhwIjoxNDc2OTYwNzIwLCJhY3IiOiIxIiwiYW1yIjpbInB3ZCJdLCJhcHBpZCI6IjljZjI4ZGM2LWJiM2ItNGNhYi1hMTcwLTRlY2FlYWMxZDUxNSIsImFwcGlkYWNyIjoiMSIsImZhbWlseV9uYW1lIjoiVGVhbSIsImdpdmVuX25hbWUiOiJuZ2FnZSIsImlwYWRkciI6IjExNS4xODYuMTQ5LjE3MyIsIm5hbWUiOiJuZ2FnZSBUZWFtIiwib2lkIjoiY2E3MjBlYjAtODRjNS00YjQ2LWI2MDAtMGI5MTE2NjhjZjM2Iiwib25wcmVtX3NpZCI6IlMtMS01LTIxLTkzOTI5MjQ2MS0zNjQ2MTMyMTc5LTMxMzA1MDQwODktNTY2MyIsInB1aWQiOiIxMDAzMDAwMDk0OEQ2QUREIiwic2NwIjoiQ2FsZW5kYXJzLlJlYWQgQ2FsZW5kYXJzLlJlYWRXcml0ZSIsInN1YiI6IjhkYkRORUZCck1aNHp5ZFNNT0NCYjRIenRWOExjZzZGcVVtOHpfc2lPazgiLCJ0aWQiOiJiMTExOWJjYy05Zjk4LTRjNGYtYWQ1NS0yYzM1NWE2MTg4ODciLCJ1bmlxdWVfbmFtZSI6Im5nYWdlQGJyb2FkcGVhay5jb20iLCJ1cG4iOiJuZ2FnZUBicm9hZHBlYWsuY29tIiwidmVyIjoiMS4wIn0.VEC4C2zcdkVscYbGbx5khZu_TMbbm_LNwK4paOxeyNvLYoEIzr37WjpABBI3cPBbW714kzuymLUaQJ9yk2V1KctjGnQFRtbKMHrF0CYJdfyLw0tnvH0mlR11Gy4VJ6mi0aNXB6rEPkn - Bza0KwiwaY81CzcIR0YBPonHlBkmPLzvcvzlNrWYmVu7ordBZ5zdTx3bT9Sb7KUEfgrbNfFQQHvR5Hacvl3PUpX0S3QWg7eOat588e59hrKzq - wOjOFlwAao - bu1AmJDsyJZM6aKvMj9IYSjft9NsAcJxrj3P4HvmP4IsiRfqj - qNZSVDxTDYNVVzTqLqetBMdVaEmCaXw";

            //gmail token
            token = "eyJhbGciOiJSUzI1NiIsImtpZCI6ImY2Y2E1NmQ1MDMzYzUyMzY4OGUxODUzN2E0YmI3YWMxYWMxYjU0MTcifQ.eyJpc3MiOiJhY2NvdW50cy5nb29nbGUuY29tIiwiYXRfaGFzaCI6IjlNekVKU01DZnAwVjdTX3Ffb0FLYkEiLCJhdWQiOiIxMDgzNjE1NzI3ODU1LXZnajQ1dnBqNXRxcmIyZTRtNXRwazVnbjB2dWFjMHE1LmFwcHMuZ29vZ2xldXNlcmNvbnRlbnQuY29tIiwic3ViIjoiMTE1NDQyMzk2Mjk3NDAyMzY3NDMwIiwiYXpwIjoiMTA4MzYxNTcyNzg1NS12Z2o0NXZwajV0cXJiMmU0bTV0cGs1Z24wdnVhYzBxNS5hcHBzLmdvb2dsZXVzZXJjb250ZW50LmNvbSIsImlhdCI6MTQ3Njk1OTM1MCwiZXhwIjoxNDc2OTYyOTUwfQ.A6uXMONiAbrODPHksmbqIQlFqvWMXIHqggNZAbBrKaoJNIFt5f_KCMtAfhqycZmGS1DyZ-eypIg8aRL-3RmYfi8ComueEZemt7WzLQsFYLUa6svAVEZ_3g1EWAHlUNnZEc8OzV4eTzGsJ9g3pEA6CbdlpxyRO-TuwWFcXBcn_HAe-KcHFFmYUMiSsYmM4Xn-n2Ye_gv_VdwAQ9GvEiZl96cQ80PrR7p-9u8OLNY0iBCrY2QFbM0kOfcXo7fCApXnTShNmoTZzcyzUfyF5lPHjIRGCijCtWynq0Nmc0nT5Owqy1ngA1OwzbUkk_83w_2d-NETAqySecAOLhoJJJhDng";
            // JWT is made of three parts, separated by a '.' 
            // First part is the header 
            // Second part is the token 
            // Third part is the signature 
            string[] tokenParts = token.Split('.');
            if (tokenParts.Length < 3)
            {
                // Invalid token, return empty
            }
            // Token content is in the second part, in urlsafe base64
            string encodedToken = tokenParts[1];
            // Convert from urlsafe and add padding if needed
            int leftovers = encodedToken.Length % 4;
            if (leftovers == 2)
            {
                encodedToken += "==";
            }
            else if (leftovers == 3)
            {
                encodedToken += "=";
            }
            encodedToken = encodedToken.Replace('-', '+').Replace('_', '/');
            // Decode the string
            var base64EncodedBytes = System.Convert.FromBase64String(encodedToken);
            string decodedToken = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            // Load the decoded JSON into a dynamic object
            dynamic jwt = Newtonsoft.Json.JsonConvert.DeserializeObject(decodedToken);
            // User's email is in the preferred_username field
            return jwt.preferred_username;
        }
        // GET: Calendar
        public async Task<ActionResult> Index()
        {
            //var x = GetEmailFromIdToken("");
            string userEmail = "mueed.bpit@gmail.com";
            string emailServer = EmailServer.Google.ToString();

            //string userEmail = "ngage@broadpeak.com";
            //string emailServer = EmailServer.Office365.ToString();

            if (emailServer == EmailServer.Google.ToString())
            {
                if (GetGoogleCalendarBLObj().IsTokenExist(userEmail))
                {
                    var googleEvent = new CalendarEvent
                    {
                        //SyncedEventId = Guid.NewGuid().ToString(),
                        Title = "New Event",
                        Description = "Event Detail",
                        StartDate = DateTime.Now,
                        EndDate = DateTime.Now.AddMinutes(120),
                        AttendeeEmailAddress = "mueed.bpit@outlook.com",
                        Location = "Conf. Room BP",
                        ReminderType = "email",
                        Reminder = 30
                    };
                    GetGoogleCalendarBLObj().InsertEvents(userEmail, googleEvent);
                    //List<CalendarEvent> res = GetGoogleCalendarBLObj().GetEvents(userEmail);
                }
                else
                {
                    return Redirect(GetGoogleCalendarBLObj().GetLoginUrl());
                }
            }
            else if (emailServer == EmailServer.Outlook.ToString() || emailServer == EmailServer.Office365.ToString())
            {
                if (GetOutlookCalendarBLObj().IsTokenExist(userEmail))
                {
                    List<CalendarEvent> res = await GetOutlookCalendarBLObj().GetEvents(userEmail);
                }
                else
                {
                    return Redirect(GetOutlookCalendarBLObj().GetLoginUrl());
                }
            }
            else
            {

            }
            return View();
        }
    }
}