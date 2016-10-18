using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SyncApplication.Models
{
    public class TokenRepository
    {
        private string _ConnectionString;
        public TokenRepository(string ConnectionString)
        {
            _ConnectionString = ConnectionString;
        }
        private SyncDbDataContext GetDbContextObj()
        {
            return new SyncDbDataContext(_ConnectionString);
        }
        public void InsertToken(CalendarSyncToken Token)
        {
            using (var db = GetDbContextObj())
            {
                db.CalendarSyncTokens.InsertOnSubmit(Token);
                db.SubmitChanges();
            }
        }
        public void UpdateToken(int TokenId, CalendarSyncToken Token)
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
        public void DeleteToken(string UserEmail)
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
        public CalendarSyncToken GetToken(string UserEmail)
        {
            using (var db = GetDbContextObj())
            {
                return db.CalendarSyncTokens.Where(t => t.UserEmail == UserEmail).FirstOrDefault();
            }
        }
    }
}