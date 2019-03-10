# Adding custom claims to a user when using OpenID Connect in ASP.NET Core 2.1

This sample requires you to setup:

1. An app in Azure AD
1. An SQL database which is usable from your development machine

You'll need to sign in once with the user so they get added to the database, and then you can set the user as an admin from the database.

The next time you sign in with that user, they will get the admin role claim added, which you can see on the Claims page.