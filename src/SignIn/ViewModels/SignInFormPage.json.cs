using Simplified.Ring5;
using Starcounter;

namespace SignIn.ViewModels
{
    partial class SignInFormPage : Json
    {
        protected MainFormPage MainForm => this.Parent as MainFormPage;

        protected override void OnData()
        {
            base.OnData();
            this.SessionUri = Session.Current.SessionUri;
        }

        void Handle(Input.SignInClick action)
        {
            this.Message = null;
            action.Cancel();

            this.Submit++;
        }

        void Handle(Input.RestoreClick action)
        {
            action.Cancel();

            this.MainForm?.OpenRestorePassword();
        }

        void Handle(Input.CreateAdminClick action)
        {
            RedirectUrl = "/signin/createadminuser";
        }
    }
}