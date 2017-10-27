using Starcounter;

namespace SignIn
{
    // TODO: Replace it with the new Authorization module. The following code was copied and adapted from UserAdmin app (Helper class).
    public class AuthorizationHelper
    {
        internal static string AdminGroupName = "Admin (System Users)";
        internal static string AdminGroupDescription = "System User Administrator Group";
        
        public static bool TryNavigateTo(string url, Request request, out Json returnPage)
        {
            returnPage = null;
            
            if (SystemUser.GetCurrentSystemUserSession() == null || SystemUser.GetCurrentSystemUserSession().User == null)
            {
                // Ask user to sign in.
                returnPage = Self.GET("/signin/partial/accessdenied-form");
                return false;
            }
            
            return true;
        }
    }
}