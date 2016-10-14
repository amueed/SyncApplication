using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SyncApplication.Models.OutlookCalendar
{
    public class OutlookTokenService
    {
        public const string _BaseUrl = "https://outlook.office.com/api/v2.0";
        public const string _AuthUrl = "https://login.microsoftonline.com/common/oauth2/v2.0/authorize";
        public const string _TokenUrl = "https://login.microsoftonline.com/common/oauth2/v2.0/token";

        public const string _ClientID = "9cf28dc6-bb3b-4cab-a170-4ecaeac1d515";
        public const string _ClientSecret = "vaDKj3nNuZkoekK5TmghZ8S";
        public const string _RedirectUri = "https://secure.emaximation.com/";

        #region Token DB Operation
        private static SyncDbDataContext GetDbContextObj()
        {
            return new SyncDbDataContext();
        }
        public static void InsertToken(CalendarSyncToken Token)
        {
            using (var db = GetDbContextObj())
            {
                db.CalendarSyncTokens.InsertOnSubmit(Token);
                db.SubmitChanges();
            }
        }
        public static void UpdateToken(int TokenId, CalendarSyncToken Token)
        {
            using (var db = GetDbContextObj())
            {
                IQueryable<CalendarSyncToken> res = db.CalendarSyncTokens.Where(t => t.TokenId == TokenId);
                if (res.Any())
                {
                    CalendarSyncToken savedToken = res.FirstOrDefault();
                    savedToken.AccessToken = Token.AccessToken;
                    savedToken.TokenExpiresIn = Token.TokenExpiresIn;
                    savedToken.TokenUpdatedOn = Token.TokenUpdatedOn;
                    db.SubmitChanges();
                }
            }
        }
        public static void DeleteToken(string UserEmail)
        {
            using (var db = GetDbContextObj())
            {
                CalendarSyncToken res = db.CalendarSyncTokens.Where(t => t.UserEmail == UserEmail).FirstOrDefault();
                if (res != null)
                {
                    db.CalendarSyncTokens.DeleteOnSubmit(res);
                    db.SubmitChanges();
                }
            }
        }
        public static CalendarSyncToken GetToken(string UserEmail)
        {
            using (var db = GetDbContextObj())
            {
                return db.CalendarSyncTokens.Where(t => t.UserEmail == UserEmail).FirstOrDefault();
            }
        }
        #endregion Token DB Operation - End

        #region Token Web Service
        public static string GetAuthUrl()
        {
            string[] scope = { "offline_access", "https://outlook.office.com/calendars.readwrite" };

            return string.Format("{0}?client_id={1}&response_type=code&redirect_uri={2}&scope={3}", _AuthUrl, _ClientID, _RedirectUri, string.Join(" ", scope));
        }
        public static OAuthToken GenerateToken(string AuthCode)
        {
            var client = new RestClient(_TokenUrl);
            var request = new RestRequest();

            request.Method = Method.POST;

            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("client_id", _ClientID);
            request.AddParameter("client_secret", _ClientSecret);
            request.AddParameter("grant_type", "authorization_code");
            request.AddParameter("redirect_uri", _RedirectUri);
            request.AddParameter("code", AuthCode);

            IRestResponse response = client.Execute(request);
            //return "";
            return new OAuthToken();
        }
        public static OAuthToken RefreshToken(string Token)
        {
            var client = new RestClient(_TokenUrl);
            var request = new RestRequest();

            request.Method = Method.POST;

            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("client_id", _ClientID);
            request.AddParameter("client_secret", _ClientSecret);
            request.AddParameter("grant_type", "refresh_token");
            request.AddParameter("refresh_token", Token);

            IRestResponse response = client.Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var jsonResponse = JObject.Parse(response.Content);
                return new OAuthToken
                {
                    AccessToken = jsonResponse["access_token"].ToString(),
                    RefreshToken = Token,
                    TokenType = jsonResponse["token_type"].ToString(),
                    IssueOn = DateTime.Now,
                    ExpiresIn = Convert.ToInt32(jsonResponse["expires_in"])
                };
            }
            return new OAuthToken();
        }
        #endregion Token Web Service - End
    }
}