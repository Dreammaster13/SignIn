using SignIn.Helpers;
using Simplified.Ring3;
using Starcounter;
using SignIn.Models;
using System;

namespace SignIn.ViewModels
{
    partial class CreateAdminUserViewModel : Json, IBound<SystemAdminUser>
    {


        protected override void OnData()
        {
            base.OnData();
            if (this.Data == null)
                return;

 
        }

        private void Handle(Input.Password action)
        {
            string message = null;
            bool isValid = false;
            this.Password = action.Value;
            if (this.Data != null)
            {
                isValid = this.Data.IsValidPassword( out message);
            }
            this.IsAlert = !isValid;
            this.Message = message;
   
        }

        private void Handle(Input.PasswordRepeat action)
        {
            string message = null;
            bool isAlert = true;
            this.PasswordRepeat = action.Value;

            if (this.Data != null)
            {
                isAlert = !this.Data.IsEqualPassword(this.PasswordRepeat, out message);
            }
            this.IsAlert = isAlert;
            this.Message = message;


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
            bool isAlert = true;
            if (this.Data != null)
            {
                this.Data.CreateAdminUser(this.PasswordRepeat, out message, out isAlert);
            }
            this.Message = message;
            this.IsAlert = isAlert;
        }





    }
}
