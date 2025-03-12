using System;

namespace IdentityServer8.Models;

public static class Constants
{
    public static class IdentityCustomOpenId
    {
        public static class LoginPage
        {
            public const string RETURN_URL = "ReturnUrl";
        }

        public static class SignUpPage
        {
            public const string RETURN_URL = LoginPage.RETURN_URL;
        }

        public static class DetailsFromToken
        {
            public const string SID_KEY = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
        }
    }
}
