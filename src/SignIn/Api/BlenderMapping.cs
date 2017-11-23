using Starcounter;

namespace SignIn.Api
{
    internal class BlenderMapping
    {
        public void Register()
        {
            Blender.MapUri("/signin/app-name", string.Empty, new[] { "app", "icon" });
            Blender.MapUri("/signin/user", string.Empty, new[] { "user" }); //expandable icon
            //can't find "userform" token anywhere inside StarcounterApps
            Blender.MapUri("/signin/signinuser", string.Empty, new[] { "userform" }); //inline form
            Blender.MapUri("/signin/signinuser?{?}", string.Empty, new[] { "redirection" }); //inline form
            Blender.MapUri("/signin/settings", string.Empty, new[] { "settings" });
            Blender.MapUri("/signin/user/authentication/password/{?}", string.Empty, new[] { "authentication-password" });
            Blender.MapUri("/signin/user/authentication/settings/{?}", string.Empty, new[] { "authentication-settings" });
            //can't find "userimage-default" token anywhere inside StarcounterApps
            Blender.MapUri("/signin/partial/user/image", string.Empty, new[] { "userimage-default" });  // default user image
        }
    }
}
