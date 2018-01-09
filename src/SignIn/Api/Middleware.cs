using SignIn.Helpers;
using Starcounter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SignIn.Api
{
    internal class Middleware
    {
        private CookieHelpers cookieHelpers = new CookieHelpers();

        internal void Register()
        {
            Application.Current.Use(new HtmlFromJsonProvider());
            Application.Current.Use(new PartialToStandaloneHtmlProvider());

            Application.Current.Use((Request req) =>
            {
                Cookie cookie = cookieHelpers.GetSignInCookie();

                if (cookie != null)
                {
                    Session.Ensure();
                    var us = Db.SQL<SystemUserSession>($"SELECT o FROM {typeof(SystemUserSession)} o WHERE o.SessionId=? and o.ExpiresAt > ?", cookie.Value, DateTime.Now).FirstOrDefault();
                    if (us != null && us.User != null)
                    {
                        SystemUserSession session = SystemUser.SignInSystemUser(us.User.Username);
                        cookieHelpers.RefreshAuthCookie(us);
                    }
                }

                return null;
            });
        }
    }
}
