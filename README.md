Sign In
=========

Simple user authentication app. Features include:

- authenticate with a username and password
- password reminder using email
- change password for existing users
- settings page to provide mail server configuration (SMTP)

## Create the first user

To create the first user, navigate to `http://localhost:8080/signin` and press "Create admin account" (visible only if no user exists in the database) or navigate to `http://localhost:8080/signin/createadminuser`.

## Sign in an existing user

To sign in an existing user in standalone mode, go to `http://localhost:8080/signin/signinuser`.

It's also possible to sign in using one of the partial views listed below when they are mapped to a blending point in any other app. By default, it's configured to show the expandable sign in form (`/signin/signinuser`) in [Launcher](https://github.com/StarcounterApps/Launcher) and [Website](https://github.com/StarcounterApps/Website) apps.

## Developer instructions

For developer instructions, go to [CONTRIBUTING](CONTRIBUTING.md)

## [Blendable views](./SignIn.map.md)

## Usage

To use Sign In apps' forms in your app, create an empty partial in your app (e.g. `/YOURAPP/YOURPAGE?{?}`) and map it to one of the above URIs using `Blender` API:

```cs
StarcounterEnvironment.RunWithinApplication("SignIn", () => {
    Handle.GET("/signin/signinuser-YOURAPP?{?}", (string objectId) => {
        return Self.GET("/signin/signinuser?{?}" + objectId);
    });

    Blender.MapUri("/signin/signinuser-YOURAPP?{?}", "signinuser-YOURAPP");
});
Blender.MapUri("/YOURAPP/YOURPAGE?{?}", "signinuser-YOURAPP");
```

Next, include that partial using in your JSON tree using `Self.GET("/YOURAPP/YOURPAGE?" + originalUrl)` when you encounter a user who is not signed in.


## License

MIT
