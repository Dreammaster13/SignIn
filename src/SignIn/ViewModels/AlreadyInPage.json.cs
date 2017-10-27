using Starcounter;
//using Simplified.Ring3;

namespace SignIn
{
    partial class AlreadyInPage : Json
    {
        protected override void OnData()
        {
            base.OnData();
            
            if (SystemUser.GetCurrentSystemUserSession() != null && SystemUser.GetCurrentSystemUserSession().User != null)
            {
                this.Username = SystemUser.GetCurrentSystemUserSession().User.Username;
            }
        }
    }
}