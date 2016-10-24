using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SyncApplication.Models.GoogleCalendar
{
    public class GoogleCalendarBL
    {
        private AppCredentials _AppCredentials;
        private string _ConnectionString = string.Empty;
        public GoogleCalendarBL(string ConnectionString, AppCredentials objAppCredentials)
        {
            _ConnectionString = ConnectionString;
            _AppCredentials = objAppCredentials;
        }

        //Get GoogleCalendarService object
        private GoogleCalendarService GetGoogleCalendarServiceObj(string UserEmail)
        {
            UserToken objUserToken = GetUserToken(UserEmail);
            return new GoogleCalendarService(_AppCredentials, objUserToken);
        }

        //Get CalendarRepository object
        private CalendarRepository GetCalendarRepositoryObj()
        {
            return new CalendarRepository(_ConnectionString);
        }

        //Get GoogleTokenService object
        private GoogleTokenService GetGoogleTokenServiceObj()
        {
            return new GoogleTokenService(_AppCredentials);
        }

        //Get TokenRepository object
        private TokenRepository GetTokenRepositoryObj()
        {
            return new TokenRepository(_ConnectionString);
        }

        public List<CalendarEvent> GetEvents(string UserEmail)
        {
            return GetGoogleCalendarServiceObj(UserEmail).GetEvents();
        }
        public string InsertEvents(string UserEmail, CalendarEvent objCalendarEvent)
        {
            //TODO: Insert into Google Calndar
            string SyncedEventId = GetGoogleCalendarServiceObj(UserEmail).InsertEvent(objCalendarEvent);
            //TODO: Insert into Local DB
            string EventId = GetCalendarRepositoryObj().InsertEvent(objCalendarEvent);
            return "";
        }
        public bool UpdateEvents(string EventId, string UserEmail)
        {
            //TODO: Update into Google Calndar
            GetGoogleCalendarServiceObj(UserEmail).UpdateEvent(EventId, new CalendarEvent());
            //TODO: Update into Local DB
            GetCalendarRepositoryObj().UpdateEvent(EventId, new CalendarEvent());
            return true;
        }
        public bool DeleteEvents(string EventId, string UserEmail)
        {
            //TODO: Delete into Google Calndar
            GetGoogleCalendarServiceObj(UserEmail).DeleteEvent(EventId);
            //TODO: Delete into Local DB
            GetCalendarRepositoryObj().DeleteEvent(EventId);
            return true;
        }
        public bool IsTokenExist(string UserEmail)
        {
            return GetAccessToken(UserEmail) != "no-token" ? true : false;
        }
        public string GetLoginUrl()
        {
            return GetGoogleTokenServiceObj().GetAuthUrl();
        }

        private UserToken GetUserToken(string UserEmail)
        {
            CalendarSyncToken FoundToken = GetTokenRepositoryObj().GetToken(UserEmail);
            if (FoundToken != null)
            {
                return new UserToken
                {
                    AccessToken = FoundToken.AccessToken,
                    RefreshToken = FoundToken.RefreshToken,
                    TokenType = FoundToken.TokenType,
                    IssueOn = Convert.ToDateTime(FoundToken.TokenUpdatedOn),
                    ExpiresIn = Convert.ToInt32(FoundToken.TokenExpiresIn),
                };
            }
            return new UserToken();
        }
        private string GetAccessToken(string UserEmail)
        {
            CalendarSyncToken FoundToken = GetTokenRepositoryObj().GetToken(UserEmail);
            if (FoundToken != null)
            {
                //token expiry check
                if (IsTokenExpired(FoundToken))
                {
                    return RefreshUserToken(FoundToken);
                }
                else
                {
                    return FoundToken.AccessToken;
                }
            }
            else
            {
                return "no-token";
            }
        }
        private bool IsTokenExpired(CalendarSyncToken Token)
        {
            var TokenExpiryDate = Convert.ToDateTime(Token.TokenUpdatedOn).AddSeconds(Convert.ToInt32(Token.TokenExpiresIn - 300));
            if (TokenExpiryDate > DateTime.Now)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private string RefreshUserToken(CalendarSyncToken Token)
        {
            //TODO: refersh token
            UserToken newToken = GetGoogleTokenServiceObj().RefreshToken(Token.RefreshToken);
            Token.AccessToken = newToken.AccessToken;
            Token.TokenExpiresIn = newToken.ExpiresIn;
            Token.TokenUpdatedOn = newToken.IssueOn;

            //TODO: update in db
            GetTokenRepositoryObj().UpdateToken(Token.TokenId, Token);
            return newToken.AccessToken;
        }
    }
}