using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SyncApplication.Models.GoogleCalendar
{
    public class GoogleCalendarBL
    {
        public static List<CalendarEvent> GetEvents(string UserEmail)
        {
            OAuthToken userToken = GetUserTokenObject(UserEmail);
            var calendarService = new GoogleCalendarService(userToken);
            return calendarService.GetEvents(UserEmail);
        }
        public static bool IsTokenExist(string UserEmail)
        {
            return GetAccessToken(UserEmail) != "no-token" ? true : false;
        }
        public static string GetLoginUrl()
        {
            return GoogleTokenService.GetAuthUrl();
        }
        private static OAuthToken GetUserTokenObject(string UserEmail)
        {
            SyncToken FoundToken = GoogleTokenService.GetToken(UserEmail);
            if (FoundToken != null)
            {
                return new OAuthToken
                {
                    AccessToken = FoundToken.AccessToken,
                    RefreshToken = FoundToken.RefreshToken,
                    IssueOn = Convert.ToDateTime(FoundToken.UpdatedOn),
                    ExpiresIn = Convert.ToInt32(FoundToken.ExpiresIn),
                };
            }
            return new OAuthToken();
        }

        private static string GetAccessToken(string UserEmail)
        {
            SyncToken FoundToken = GoogleTokenService.GetToken(UserEmail);
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

        private static bool IsTokenExpired(SyncToken Token)
        {
            var TokenExpiryDate = Convert.ToDateTime(Token.UpdatedOn).AddSeconds(Convert.ToInt32(Token.ExpiresIn - 300));
            if (TokenExpiryDate > DateTime.Now)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private static string RefreshUserToken(SyncToken Token)
        {
            //TODO: refersh token
            OAuthToken newToken = GoogleTokenService.RefreshToken(Token.RefreshToken);
            Token.AccessToken = newToken.AccessToken;
            Token.ExpiresIn = newToken.ExpiresIn;
            Token.UpdatedOn = newToken.IssueOn;

            //TODO: update in db
            GoogleTokenService.UpdateToken(Token.TokenId, Token);
            return newToken.AccessToken;
        }
    }
}