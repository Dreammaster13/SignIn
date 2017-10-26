using SignIn.Helpers;
//using Simplified.Ring3;
//using Simplified.Ring5;
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
                    var us = Db.SQL<SystemUserSession>("SELECT o FROM SignIn.SystemUserSession o WHERE o.SessionId=? and o.ExpiresAt > ?", cookie.Value, DateTime.Now).First;
                    if (us != null)
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
