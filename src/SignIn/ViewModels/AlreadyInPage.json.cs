using Starcounter;
//using Simplified.Ring3;

namespace SignIn
{
    partial class AlreadyInPage : Json
    {
        protected override void OnData()
        {
            base.OnData();

            SystemUser user = SystemUser.GetCurrentSystemUserSession().User;

            if (user != null)
            {
                this.Username = user.Username;
            }
        }
    }
}