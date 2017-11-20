using System;
using System.Net;
using System.Net.Mail;
using Starcounter;
using Simplified.Ring2;
using Simplified.Ring3;
using Simplified.Ring6;
using SignIn.Helpers;

namespace SignIn.ViewModels
{
    partial class RestorePasswordFormPage : Json
    {
        protected MainFormPage MainForm => this.Parent as MainFormPage;

        void Handle(Input.SignInClick action)
        {
            action.Cancel();

            this.MainForm?.OpenSignIn();
        }

        void Handle(Input.Username action) // Makes the Reset Password clickable again.
        {
            this.DisableRestoreClick = 0;
        }

        void Handle(Input.RestoreClick action)
        {
            this.DisableRestoreClick = 1;
            this.MessageCss = "alert alert-danger";

            if (string.IsNullOrEmpty(this.Username))
            {
                this.Message = "Username is required!";
                return;
            }

            SystemUser user = SystemUser.GetSystemUser(this.Username);

            if (user == null)
            {
                this.Message = "Invalid username!";
                return;
            }

            var person = user.WhoIs as Person;
            EmailAddress email = Utils.GetUserEmailAddress(user);

            if (person == null || string.IsNullOrEmpty(email?.EMail))
            {
                this.Message = "Unable to restore password, no e-mail address found!";
                return;
            }

            string password = Utils.RandomString(5);
            string hash = SystemUser.GeneratePasswordHash(user.Username, password, user.PasswordSalt);

            try
            {
                this.SendNewPassword(person.FullName, user.Username, password, email.Name);
                this.Message = "Your new password has been sent to your email address.";
                this.MessageCss = "alert alert-success";
                Db.Transact(() => { user.Password = hash; });
            }
            catch (Exception ex)
            {
                this.Message = "Mail server is currently unavailable.";
                this.MessageCss = "alert alert-danger";
                var log = new Starcounter.Logging.LogSource(Application.Current.Name);
                log.LogException(ex);
            }
        }

        protected void SendNewPassword(string Name, string Username, string NewPassword, string Email)
        {
            SettingsMailServer settings = MailSettingsHelper.GetSettings();
            var client = new SmtpClient
            {
                Port = settings.Port,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(settings.Username, settings.Password),
                Host = settings.Host,
                EnableSsl = settings.EnableSsl
            };

            var mail = new MailMessage(settings.Username, Email)
            {
                Subject = "Restore password",
                Body =
                    $"<h1>Hello {Name}</h1><p>You have requested a new password for your " +
                    $"<b>{Username}</b> account.</p>" +
                    $"<p>Your new password is: <b>{NewPassword}</b>.</p>",

                IsBodyHtml = true
            };

            client.Send(mail);
        }
    }
}