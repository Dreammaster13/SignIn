using SignIn.Helpers;
using Simplified.Ring3;
using Starcounter;
using SignIn.Models;
using System;

namespace SignIn.ViewModels
{
    partial class CreateAdminUserViewModel : Json, IBound<SystemAdminUser>
    {
        private void Handle(Input.Password action)
        {
            this.Password = action.Value;
            this.IsAlert = !this.Data.IsValidPassword(out string message);
            this.Message = message;
        }
        private void Handle(Input.PasswordRepeat action)
        {
            this.PasswordRepeat = action.Value;
            this.IsAlert = !this.Data.IsEqualPassword(this.PasswordRepeat, out string message);
            this.Message = message;
        }
        private void Handle(Input.OkTrigger action)
        {
            CreateAdminUser();
        }
        private void Handle(Input.CancelTrigger action)
        {
            GoBack();
        }
        private void Handle(Input.BackTrigger action)
        {
            GoBack();
        }
     
        private void GoBack()
        {
            this.RedirectUrl = "/signin/signinuser";
        }

        private void CreateAdminUser()
        {
            this.Data.CreateAdminUser( out string  message, out  bool isAlert);
            this.Message = message;
            this.IsAlert = isAlert;
        }
    }
}
