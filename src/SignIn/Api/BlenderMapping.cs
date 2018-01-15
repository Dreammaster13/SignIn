using Starcounter;

namespace SignIn.Api
{
    internal class BlenderMapping
    {
        public void Register()
        {
            Blender.MapUri("/signin/app-name", string.Empty, new string[] { "app", "icon" });
            Blender.MapUri("/signin/user", string.Empty, new string[] { "user" }); //expandable icon
            //can't find "userform" token anywhere inside StarcounterApps
            Blender.MapUri("/signin/signinuser", string.Empty, new string[] { "userform" }); //inline form
            Blender.MapUri("/signin/signinuser?{?}", string.Empty, new string[] { "redirection" }); //inline form
            Blender.MapUri("/signin/settings", string.Empty, new string[] { "settings" });
            Blender.MapUri2<SystemUser>("/signin/user/authentication/password/{?}", new string[] { "authentication-password" });
            Blender.MapUri2<SystemUser>("/signin/user/authentication/settings/{?}", new string[] { "authentication-settings" });
            //can't find "userimage-default" token anywhere inside StarcounterApps
            Blender.MapUri("/signin/partial/user/image", string.Empty, new string[] { "userimage-default" });  // default user image
        }
    }
}
