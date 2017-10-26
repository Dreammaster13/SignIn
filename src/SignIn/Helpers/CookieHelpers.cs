using Starcounter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SignIn.Helpers
{
    internal class CookieHelpers
    {
        internal string AuthCookieName = "soauthtoken";

        internal void SetAuthCookie(SystemUserSession userSess)
        {
            Cookie cookie = new Cookie()
            {
                Name = AuthCookieName
            };

            if (userSess == null)
            {
                DeleteCookie(cookie);
            }
            else
            {
                cookie.Value = userSess.SessionId;
                cookie.Expires = userSess.ExpiresAt;
            }

            Handle.AddOutgoingCookie(cookie.Name, cookie.GetFullValueString());
        }

        internal void ClearAuthCookie()
        {
            this.SetAuthCookie(null);
        }

        internal void DeleteCookie(Cookie cookie)
        {
            cookie.Expires = DateTime.Now.AddDays(-1).ToUniversalTime();
        }


        internal Cookie GetSignInCookie()
        {
            List<Cookie> cookies = Handle.IncomingRequest.Cookies.Where(val => !string.IsNullOrEmpty(val)).Select(x => new Cookie(x)).ToList();
            Cookie cookie = cookies.FirstOrDefault(x => x.Name == AuthCookieName);

            return cookie;
        }

        internal void RefreshAuthCookie(SystemUserSession Session)
        {
            Cookie cookie = GetSignInCookie();

            if (cookie == null)
            {
                return;
            }
            cookie.Expires = Session.ExpiresAt;

            Handle.AddOutgoingCookie(cookie.Name, cookie.GetFullValueString());
        }
    }
}
