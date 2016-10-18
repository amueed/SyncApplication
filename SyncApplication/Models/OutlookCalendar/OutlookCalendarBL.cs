using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SyncApplication.Models.OutlookCalendar
{
    public class OutlookCalendarBL
    {
        private AppCredentials _AppCredentials;
        private string _ConnectionString = string.Empty;
        public OutlookCalendarBL(string ConnectionString, AppCredentials objAppCredentials)
        {
            _ConnectionString = ConnectionString;
            _AppCredentials = objAppCredentials;
        }
        //Get GoogleCalendarService object
        private OutlookCalendarService GetOutlookCalendarServiceObj(string UserEmail)
        {
            UserToken objUserToken = GetUserToken(UserEmail);
            return new OutlookCalendarService(objUserToken, _AppCredentials);
        }

        //Get CalendarRepository object
        private CalendarRepository GetCalendarRepositoryObj()
        {
            return new CalendarRepository(_ConnectionString);
        }

        //Get OutlookTokenService object
        private OutlookTokenService GetOutlookTokenServiceObj()
        {
            return new OutlookTokenService(_AppCredentials);
        }

        //Get TokenRepository object
        private TokenRepository GetTokenRepositoryObj()
        {
            return new TokenRepository(_ConnectionString);
        }
        public async Task<List<CalendarEvent>> GetEvents(string UserEmail)
        {
            return await GetOutlookCalendarServiceObj(UserEmail).GetEventsAsync();
            //return calendarService.GetEvents(UserEmail);
        }
        public bool IsTokenExist(string UserEmail)
        {
            return GetAccessToken(UserEmail) != "no-token" ? true : false;
        }
        public string GetLoginUrl()
        {
            return GetOutlookTokenServiceObj().GetAuthUrl();
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
            UserToken newToken = GetOutlookTokenServiceObj().RefreshToken(Token.RefreshToken);
            Token.AccessToken = newToken.AccessToken;
            Token.TokenExpiresIn = newToken.ExpiresIn;
            Token.TokenUpdatedOn = newToken.IssueOn;

            //TODO: update in db
            GetTokenRepositoryObj().UpdateToken(Token.TokenId, Token);
            return newToken.AccessToken;
        }
    }
}