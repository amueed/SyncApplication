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
        AppCredentials _AppCredentials;

        public OutlookTokenService(AppCredentials objAppCredentials)
        {
            _AppCredentials = objAppCredentials;
        }

        public string GetAuthUrl()
        {
            string[] scope = { "offline_access", "https://outlook.office.com/calendars.readwrite" };
            return string.Format("{0}?client_id={1}&response_type=code&redirect_uri={2}&scope={3}", _AppCredentials.AuthUrl, _AppCredentials.ClientId, _AppCredentials.RedirectUri, string.Join(" ", scope));
        }
        public UserToken GenerateToken(string AuthCode)
        {
            var client = new RestClient(_AppCredentials.TokenUrl);
            var request = new RestRequest();

            request.Method = Method.POST;

            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("client_id", _AppCredentials.ClientId);
            request.AddParameter("client_secret", _AppCredentials.ClientSecret);
            request.AddParameter("grant_type", "authorization_code");
            request.AddParameter("redirect_uri", _AppCredentials.RedirectUri);
            request.AddParameter("code", AuthCode);

            IRestResponse response = client.Execute(request);
            //return "";
            return new UserToken();
        }
        public UserToken RefreshToken(string Token)
        {
            var client = new RestClient(_AppCredentials.TokenUrl);
            var request = new RestRequest();

            request.Method = Method.POST;

            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("client_id", _AppCredentials.ClientId);
            request.AddParameter("client_secret", _AppCredentials.ClientSecret);
            request.AddParameter("grant_type", "refresh_token");
            request.AddParameter("refresh_token", Token);

            IRestResponse response = client.Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var jsonResponse = JObject.Parse(response.Content);
                return new UserToken
                {
                    AccessToken = jsonResponse["access_token"].ToString(),
                    RefreshToken = Token,
                    TokenType = jsonResponse["token_type"].ToString(),
                    IssueOn = DateTime.Now,
                    ExpiresIn = Convert.ToInt32(jsonResponse["expires_in"])
                };
            }
            return new UserToken();
        }
    }
}