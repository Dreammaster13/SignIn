Sign In
=========

Simple user authentication app. Features include:

- authenticate with a username and password
- password reminder using email
- change password for existing users
- settings page to provide mail server configuration (SMTP)

## Create the first user

To create the first user, open `http://localhost:8080/SignIn/generateadminuser`. This generates an admin user with default credentials (username `admin`, password `admin`). The default user will be generated only if there are no users in database.

## Developer instructions

For developer instructions, go to [CONTRIBUTING](CONTRIBUTING.md)

## Mappable partial views

### GET /signin/user

Expandable icon with a sign-in form and a button to restore the password. Used in toolbars (Launcher, Website, etc).

For a signed in user, the icon displays triggers partial from the Images app, which displays the Illustration of the Person that is assigned to the System User.

Screenshot:

![image](docs/screenshot-signin-user.gif)

### GET /signin/signinuser

Inline sign-in form and a button to restore the password. Used as a full page form in standalone apps.

Screenshot:

![image](docs/screenshot-signin-signinuser.png)

### GET /signin/signinuser?`{string OriginalUrl}`

Same as above but with redirection to a URL after successful sign-in. Used in UserAdmin.

Screenshot:

![image](docs/screenshot-signin-signinuser.png)

### GET /signin/admin/settings

Settings page. Includes the mail server configuration form (SMTP). Used in Launcher.

#### Settings for Gmail

The following settings work for using Gmail as the SMTP server:

- Host: `smtp.gmail.com`
- Port: `587`
- Enable SSL: (checked)
- Username: (your Gmail email address)
- Password: (your Gmail password)

Note that you need to set `Allow less secure apps` to `ON` in your Google Account settings.

Screenshot:

![image](docs/screenshot-signin-admin-settings.png)

### GET /signin/user/authentication/settings/`{SystemUser ObjectID}`

Password change form for existing users. Used in UserAdmin.

Screenshot:

![image](docs/screenshot-signin-user-authentication-settings.png)

### Usage

To use Sign In apps' forms in your app, create an empty partial in your app (e.g. `/YOURAPP/YOURPAGE?{?}`) and map it to one of the above URIs using `UriMapping` API:

```cs
StarcounterEnvironment.RunWithinApplication("SignIn", () => {
    Handle.GET("/signin/signinuser-YOURAPP?{?}", (string objectId) => {
        return Self.GET("/signin/signinuser?{?}" + objectId);
    });

    UriMapping.Map("/signin/signinuser-YOURAPP?{?}", "/sc/mapping/signinuser-YOURAPP?{?}");
});
UriMapping.Map("/YOURAPP/YOURPAGE?{?}", "/sc/mapping/signinuser-YOURAPP?{?}");
```

Next, include that partial using in your JSON tree using `Self.GET("/YOURAPP/YOURPAGE?" + originalUrl)` when you encounter a user who is not signed in.


## License

MIT
