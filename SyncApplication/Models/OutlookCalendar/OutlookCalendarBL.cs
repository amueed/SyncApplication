using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SyncApplication.Models.OutlookCalendar
{
    public class OutlookCalendarBL
    {
        public static async Task<List<CalendarEvent>> GetEvents(string UserEmail)
        {
            OAuthToken userToken = GetUserTokenObject(UserEmail);
            //var token = string.Format("{0} {1}", userToken.TokenType, userToken.AccessToken);
            string token = userToken.AccessToken;
            var calendarService = new OutlookCalendarService(token);
            return await calendarService.SyncEventsAsync();
            //return calendarService.GetEvents(UserEmail);
        }
        public static bool IsTokenExist(string UserEmail)
        {
            return GetAccessToken(UserEmail) != "no-token" ? true : false;
        }
        public static string GetLoginUrl()
        {
            return OutlookTokenService.GetAuthUrl();
        }
        private static OAuthToken GetUserTokenObject(string UserEmail)
        {
            CalendarSyncToken FoundToken = OutlookTokenService.GetToken(UserEmail);
            if (FoundToken != null)
            {
                return new OAuthToken
                {
                    AccessToken = FoundToken.AccessToken,
                    RefreshToken = FoundToken.RefreshToken,
                    TokenType = FoundToken.TokenType,
                    IssueOn = Convert.ToDateTime(FoundToken.TokenUpdatedOn),
                    ExpiresIn = Convert.ToInt32(FoundToken.TokenExpiresIn),
                };
            }
            return new OAuthToken();
        }
        private static string GetAccessToken(string UserEmail)
        {
            CalendarSyncToken FoundToken = OutlookTokenService.GetToken(UserEmail);
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

        private static bool IsTokenExpired(CalendarSyncToken Token)
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

        private static string RefreshUserToken(CalendarSyncToken Token)
        {
            //TODO: refersh token
            OAuthToken newToken = OutlookTokenService.RefreshToken(Token.RefreshToken);
            Token.AccessToken = newToken.AccessToken;
            Token.TokenExpiresIn = newToken.ExpiresIn;
            Token.TokenUpdatedOn = newToken.IssueOn;

            //TODO: update in db
            OutlookTokenService.UpdateToken(Token.TokenId, Token);
            return newToken.AccessToken;
        }
    }
}