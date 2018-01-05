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
            #region Implicit standalone page template
            const string ImplicitStandaloneTemplate = @"<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
    <title>{0}</title>
    <script src=""/sys/webcomponentsjs/webcomponents-lite.min.js""></script>
    <script>
      /* this script must run before Polymer is imported */
      /*
       * Let Polymer use native Shadow DOM if available.
       * Otherwise (at least Polymer 1.x) assumes everybody else
       * uses ShadyDOM, which is not true, as many Vanilla CE uses
       * real Shadow DOM.
       */
      window.Polymer = {{
        dom: ""shadow""
      }};
    </script>
    <link rel=""import"" href=""/sys/polymer/polymer.html"">
    <link rel=""import"" href=""/sys/starcounter.html"">
    <link rel=""import"" href=""/sys/starcounter-include/starcounter-include.html"">
    <link rel=""import"" href=""/sys/bootstrap.html"">
    <style>
        body {{
            margin: 20px;
        }}
        body > starcounter-include{{
            height: 100%;
        }}
    </style>
</head>
<body>
    <dom-bind id=""palindrom-root"">
    <template is=""dom-bind"">
        <starcounter-include view-model=""{{{{model}}}}""></starcounter-include>
    </template>
</dom-bind>
    <palindrom-client ref=""palindrom-root"" remote-url=""{1}""></palindrom-client>
</body>
</html>";
            #endregion
            Application.Current.Use(new HtmlFromJsonProvider());
            Application.Current.Use(new PartialToStandaloneHtmlProvider(ImplicitStandaloneTemplate));

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
