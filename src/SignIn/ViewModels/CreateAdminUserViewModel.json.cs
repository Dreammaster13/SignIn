using SignIn.Helpers;
using Simplified.Ring3;
using Starcounter;
using SignIn.Models;
using System;

namespace SignIn.ViewModels
{
    partial class CreateAdminUserViewModel : Json, IBound<SystemAdminUser>
    {
        private string OldPassword;
        private string OldPasswordSalt;

        public bool IsAdminCreated
        {
            get
            {
                return !this.CanCreateAdminUser;
            }
        }
        public bool CanCreateAdminUser
        {
            get
            {
                return this.Data == null ? false : this.Data.CanCreateAdminUser;
            }
}

protected override void OnData()
        {
            base.OnData();
            if (this.Data == null)
                return;

            this.OldPassword = this.Data.Password;
            this.OldPasswordSalt = this.Data.PasswordSalt;
        }

        private void Handle(Input.PasswordToSet action)
        {
            this.PasswordToSet = action.Value;
            this.ValidatePassword();
        }

        private void Handle(Input.PasswordRepeat action)
        {
            this.PasswordRepeat = action.Value;
            this.ValidateRepeatedPassword();
        }
        private void Handle(Input.OkClick action)
        {
            CreateAdminUser();
        }
        private void Handle(Input.CancelClick action)
        {
            GoBack();
        }
        private void Handle(Input.BackClick action)
        {
            GoBack();
        }
     
        private void GoBack()
        {
            this.RedirectUrl = "/signin/signinuser";
        }




        private void CreateAdminUser()
        {
            
            string message = null;
            if(IsValid(this.PasswordToSet, this.PasswordRepeat, out message))
            {
                SystemAdminUser.CreateAdminSystemUserIfMissing(this.PasswordRepeat, out message);
            }
            this.Message = message;

        }
        private void ValidatePassword()
        {
            string message = null;
            IsValidPassword(this.PasswordToSet, out message);
            this.Message = message;
        }
        private void ValidateRepeatedPassword()
        {
            string message = null;
            IsValid(this.PasswordToSet, this.PasswordRepeat, out message);
            this.Message = message;
        }
        protected bool IsValid(string password, string repeatedPassword, out string message)
        {
            message = null;
            bool isValid = false;
            if (string.IsNullOrEmpty(password))
            {
                message = "Password must not be empty!";
            }
            else if (password != repeatedPassword)
            {
                message = "Passwords do not match!";
            }
            else
            {
                isValid = true;
            }
            return isValid;
        }
        protected bool IsValidPassword(string password, out string message)
        {
            message = null;
            bool isValid = true;
            if (string.IsNullOrEmpty(password))
            {
                message = "Password must not be empty!";
                isValid = false;
            }
            return isValid;

        }



    }
}
