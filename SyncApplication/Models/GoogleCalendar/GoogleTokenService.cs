using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SyncApplication.Models.GoogleCalendar
{
    public class GoogleTokenService
    {
        public const string _BaseUrl = "https://www.googleapis.com/calendar/v3";
        public const string _AuthUrl = "https://accounts.google.com/o/oauth2/auth";
        public const string _TokenUrl = "https://accounts.google.com/o/oauth2/token";

        public const string _ClientID = "1083615727855-vgj45vpj5tqrb2e4m5tpk5gn0vuac0q5.apps.googleusercontent.com";
        public const string _ClientSecret = "fVAoZ3ved3ttjZSucvFuTE9L";
        public const string _RedirectUri = "https://secure.emaximation.com/";

        #region Token DB Operation
        private static SyncDbDataContext GetDbContextObj()
        {
            return new SyncDbDataContext();
        }
        public static void InsertToken(SyncToken Token)
        {
            using (var db = GetDbContextObj())
            {
                db.SyncTokens.InsertOnSubmit(Token);
                db.SubmitChanges();
            }
        }
        public static void UpdateToken(int TokenId, SyncToken Token)
        {
            using (var db = GetDbContextObj())
            {
                IQueryable<SyncToken> res = db.SyncTokens.Where(t => t.TokenId == TokenId);
                if (res.Any())
                {
                    SyncToken savedToken = res.FirstOrDefault();
                    savedToken.AccessToken = Token.AccessToken;
                    savedToken.ExpiresIn = Token.ExpiresIn;
                    savedToken.UpdatedOn = Token.UpdatedOn;
                    db.SubmitChanges();
                }
            }
        }
        public static void DeleteToken(string UserEmail)
        {
            using (var db = GetDbContextObj())
            {
                SyncToken res = db.SyncTokens.Where(t => t.UserEmail == UserEmail).FirstOrDefault();
                if (res != null)
                {
                    db.SyncTokens.DeleteOnSubmit(res);
                    db.SubmitChanges();
                }
            }
        }
        public static SyncToken GetToken(string UserEmail)
        {
            using (var db = GetDbContextObj())
            {
                return db.SyncTokens.Where(t => t.UserEmail == UserEmail).FirstOrDefault();
            }
        }
        #endregion Token DB Operation - End

        #region Token Web Service
        public static string GetAuthUrl()
        {
            var lstScope = new List<string>();
            lstScope.Add("https://www.googleapis.com/auth/calendar");
            string scope = string.Join(" ", lstScope);

            return string.Format("{0}?client_id={1}&response_type=code&redirect_uri={2}&scope={3}", _AuthUrl, _ClientID, _RedirectUri, scope);
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