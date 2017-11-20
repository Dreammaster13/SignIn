using System;
using System.Web;
using Simplified.Ring3;
using Simplified.Ring6;
using Smorgasbord.PropertyMetadata;
using Starcounter;
using SignIn.Helpers;

// FORGOT PASSWORD:
// http://www.asp.net/identity/overview/features-api/account-confirmation-and-password-recovery-with-aspnet-identity

namespace SignIn.ViewModels
{
    partial class SystemUserAuthenticationSettings : PropertyMetadataPage, IBound<SystemUser>
    {
        static SystemUserAuthenticationSettings()
        {
            DefaultTemplate.ResetPassword_Enabled.Bind = nameof(ResetPassword_Enabled);
        }

        public bool ResetPassword_Enabled
        {
            get
            {
                var emailAddress = Utils.GetUserEmailAddress(this.Data);
                return emailAddress != null && 
                    MailSettingsHelper.GetSettings().Enabled && 
                    Utils.IsValidEmail(emailAddress.EMail);
            }
        }

        void Handle(Input.ResetPassword action)
        {
            // Go to "Reset password" form
            this.Message = null;
            this.ResetUserPassword();
        }

        protected void ResetUserPassword()
        {
            string link = null;
            string fullName = string.Empty;
            var mailSettings = MailSettingsHelper.GetSettings();

            if (mailSettings.Enabled == false)
            {
                this.Message = "Mail Server not enabled in the settings.";
                return;
            }

            if (string.IsNullOrEmpty(mailSettings.SiteHost))
            {
                this.Message = "Invalid settings, check site host name / port";
                return;
            }

            var emailAddress = Utils.GetUserEmailAddress(this.Data);
            var email = emailAddress.EMail;
            if (!Utils.IsValidEmail(email))
            {
                this.Message = "Username is not an email address";
                return;
            }

            var transaction = this.Transaction;
            transaction.Scope(() =>
            {
                SystemUser systemUser = this.Data;
                ResetPassword passwordReseToken = GeneratePasswordResetToken(systemUser);
                fullName = GetFullName(systemUser);

                UriBuilder uri = BuildResetPasswordLink(mailSettings, passwordReseToken);

                link = uri.ToString();
            });

            transaction.Commit();

            SendEmail(link, fullName, email);
        }

        private void SendEmail(string link, string fullName, string email)
        {
            try
            {
                this.Message = $"Sending mail sent to {email}...";
                Utils.SendResetPasswordMail(fullName, email, link);
                this.Message = "Mail sent.";
            }
            catch (Exception e)
            {
                this.Message = e.Message;
            }
        }

        private static UriBuilder BuildResetPasswordLink(
            SettingsMailServer mailSettings, 
            ResetPassword resetPassword)
        {
            return new UriBuilder
            {
                Host = mailSettings.SiteHost,
                Port = (int)mailSettings.SitePort,
                Path = "signin/user/resetpassword",
                Query = "token=" + resetPassword.Token
            };
        }

        private static string GetFullName(SystemUser systemUser) => 
            systemUser.WhoIs == null ?
                systemUser.Username :
                systemUser.WhoIs.FullName;

        private static ResetPassword GeneratePasswordResetToken(SystemUser systemUser)
        {
            int oneDayInMinutes = 1440;
            DateTime expirationTime = DateTime.UtcNow.AddMinutes(oneDayInMinutes);

            return new ResetPassword()
            {
                User = systemUser,
                Token = HttpUtility.UrlEncode(Guid.NewGuid().ToString()),
                Expire = expirationTime
            };
        }
    }
}
