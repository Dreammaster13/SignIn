using Starcounter;

namespace SignIn
{
    partial class SignInPage : Json, IBound<SystemUserSession>
    {
        public bool IsSignedIn => this.Data != null;

        static SignInPage()
        {
            DefaultTemplate.FullName.Bind = "User.Username";
        }

        protected override void OnData()
        {
            base.OnData();
            this.SessionUri = Session.Current.SessionUri;
            this.UserImage = Self.GET<Json>("/signin/partial/user/image");
        }

        void Handle(Input.SignInClick action)
        {
            this.Message = string.Empty;
            action.Cancel();

            this.Submit++;
        }
    }
}