using SignIn.Helpers;
using SignIn.Models;
using SignIn.ViewModels;
using Simplified.Ring3;
using Simplified.Ring5;
using Starcounter;
using System;
using System.Collections.Specialized;
using System.Web;

namespace SignIn.Api
{
    internal class PartialHandlers
    {
        private CookieHelpers CookieHelper => new CookieHelpers();
        private MainHandlers MainHandlers => new MainHandlers();

        internal void Register()
        {
            var internalOption = new HandlerOptions() { SkipRequestFilters = true };

            Handle.POST("/signin/partial/signin", (Request request) =>
            {
                NameValueCollection values = HttpUtility.ParseQueryString(request.Body);
                string username = values["username"];
                string password = values["password"];
                string rememberMe = values["rememberMe"];

                HandleSignIn(username, password, rememberMe);

                Session.Current?.CalculatePatchAndPushOnWebSocket();
                return 200;
            }, internalOption);

            Handle.GET("/signin/partial/signin-form", (Request request) => 
                new SignInFormPage()
                {
                    Data = null,
                    CanCreateAdminUser = SystemAdminUser.GetCanCreateAdminUser(request.ClientIpAddress)
                }, internalOption);

            Handle.GET("/signin/partial/createadminuser", (Request request) =>
                new CreateAdminUserViewModel()
                {
                    Data = SystemAdminUser.Create(request.ClientIpAddress)
                }, internalOption);

            Handle.GET("/signin/partial/alreadyin-form", () => new AlreadyInPage() { Data = null }, internalOption);
            Handle.GET("/signin/partial/restore-form", () => new RestorePasswordFormPage(), internalOption);
            Handle.GET("/signin/partial/profile-form", () => new ProfileFormPage() { Data = null }, internalOption);
            Handle.GET("/signin/partial/accessdenied-form", () => new AccessDeniedPage(), internalOption);
            Handle.GET("/signin/partial/main-form", () => new MainFormPage() { Data = null }, internalOption);

            Handle.GET("/signin/partial/user/image", () => new UserImagePage());
            Handle.GET("/signin/partial/user/image/{?}", (string objectId) => new Json(), internalOption);
            Handle.GET("/signin/partial/signout", HandleSignOut, internalOption);
        }

        protected void HandleSignIn(string Username, string Password, string RememberMe)
        {
            Username = Uri.UnescapeDataString(Username);

            SystemUserSession session = SystemUser.SignInSystemUser(Username, Password);

            if (session == null)
            {
                MasterPage master = MainHandlers.GetMaster();
                string message = "Invalid username or password!";

                if (master.SignInPage != null)
                {
                    master.SignInPage.Message = message;
                }

                if (master.Partial is MainFormPage)
                {
                    var page = (MainFormPage)master.Partial;
                    if (page.CurrentForm is SignInFormPage form)
                    {
                        form.Message = message;
                    }
                }

                if (master.Partial is SignInFormPage)
                {
                    var page = master.Partial as SignInFormPage;
                    page.Message = message;
                }
            }
            else
            {
                if (RememberMe == "true")
                {
                    Db.Transact(() =>
                    {
                        session.Token.Expires = DateTime.UtcNow.AddDays(CookieHelper.RememberMeDays);
                        session.Token.IsPersistent = true;
                    });
                }
                CookieHelper.SetAuthCookie(session.Token);
            }
        }

        protected Response HandleSignOut()
        {
            SystemUser.SignOutSystemUser();
            CookieHelper.ClearAuthCookie();

            return 200;
        }
    }
}
