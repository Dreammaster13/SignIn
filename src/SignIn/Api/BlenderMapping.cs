﻿using Starcounter;

namespace SignIn.Api
{
    internal class BlenderMapping
    {
        public void Register()
        {
            Blender.MapUri("/signin/app-name", "app-name");
            Blender.MapUri("/signin/user", "user"); //expandable icon
            Blender.MapUri("/signin/signinuser", "userform"); //inline form
            Blender.MapUri("/signin/signinuser?{?}", "userform-return"); //inline form
            Blender.MapUri("/signin/settings", "settings");
            Blender.MapUri("/signin/user/authentication/password/{?}", "authentication-password");
            Blender.MapUri("/signin/user/authentication/settings/{?}", "authentication-settings");
            Blender.MapUri("/signin/partial/user/image", "userimage-default");  // default user image
        }
    }
}
