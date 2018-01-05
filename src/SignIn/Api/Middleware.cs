using SignIn.Helpers;
using Simplified.Ring3;
using Simplified.Ring5;
using Starcounter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SignIn.Api
{
    internal class Middleware
    {
        private CookieHelpers CookieHelpers => new CookieHelpers();

        internal void Register()
        {
            Application.Current.Use(new HtmlFromJsonProvider());
            Application.Current.Use(new PartialToStandaloneHtmlProvider());

            Application.Current.Use((Request req) =>
            {
                Cookie cookie = CookieHelpers.GetSignInCookie();

                if (cookie != null)
                {
                    Session.Ensure();
                    SystemUserSession session = SystemUser.SignInSystemUser(cookie.Value);

                    if (session != null)
                    {
                        CookieHelpers.RefreshAuthCookie(session);
                    }
                }

                return null;
            });
        }
    }
}
