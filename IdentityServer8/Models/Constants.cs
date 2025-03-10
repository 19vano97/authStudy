using System;

namespace IdentityServer8.Models;

public static class Constants
{
    public static class IdentityCustomeOpenId
    {
        public static class LoginPage
    {
        public const string RETURN_URL = "ReturnUrl";
    }

    public static class SignUpPage
    {
        public const string RETURN_URL = LoginPage.RETURN_URL;
    }
    }
}
