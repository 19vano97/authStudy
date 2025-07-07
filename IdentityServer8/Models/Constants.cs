using System;
using IdentityServer8.Entities.Account;

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
            public const string ACCOUNT_ID = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
        }
    }

    public static class Statuses
    {
        public static class Register
        {
            public static class AccountExists
            {
                public const string CODE = "Exists";
                public const string DESCRIPTION = "The account has already been existed";
            }
        }

        public static class Exception
        {
            public const string CODE = "Exception";
        }

        public static class Find
        {
            public static class ByEmail
            {
                public static class NotFound
                {
                    public const string CODE = "NotFound";
                    public const string DESCRIPTION = "The account with email doesn't exist";
                }
            }
        }

        public static class ThirdParty
        {
            public static class ExternalLoginInformation
            {
                public static class NotFound
                {
                    public const string CODE = "NotFound";
                    public const string DESCRIPTION = "The external login information doesn't exist";
                }
            }

            public static class ExternalLoginAccountCreation
            {
                public static class Failed
                {
                    public const string CODE = "Failed";
                    public const string DESCRIPTION = "The external login didn't create";
                }
            }
        }

        public static class ViewModelValidator
        {
            public static class Error
            {
                public static class Login
                {
                    public static class Email
                    {
                        public const string REQUIRED = "Email is required";
                        public const string FORMAT = "Wrong email format";
                    }

                    public static class Password
                    {
                        public const string REQUIRED = "Password is required";
                        public const string FORMAT = "Wrong password format. Needs more than 6 symbols";
                    }
                }

                public static class Register
                {
                    public static class Email
                    {
                        public const string REQUIRED = "Email is required";
                        public const string FORMAT = "Wrong email format";
                    }

                    public static class Password
                    {
                        public const string REQUIRED = "Password is required";
                        public const string FORMAT = "Wrong password format. Needs more than 6 symbols";
                    }

                    public static class FirstName
                    {
                        public const string REQUIRED = "First name is required";
                        public const string FORMAT = "Wrong first name format";
                    }

                    public static class LastName
                    {
                        public const string REQUIRED = "Last name is required";
                        public const string FORMAT = "Wrong last name format";
                    }
                }

                public static class EmailValidation
                {
                    public static class Email
                    {
                        public const string REQUIRED = "Email is required";
                        public const string FORMAT = "Wrong email format";
                    }
                }

                public static class ResetPassword
                {
                    public static class Email
                    {
                        public const string REQUIRED = "Email is required";
                        public const string FORMAT = "Wrong email format";
                    }

                    public static class NewPassword
                    {
                        public const string REQUIRED = "Password is required";
                        public const string FORMAT = "Wrong password format. Needs more than 6 symbols";
                    }

                    public static class ConfirmPassword
                    {
                        public const string REQUIRED = "Password is required";
                        public const string FORMAT = "Wrong password format. Needs more than 6 symbols";
                    }

                    public const string PASSWORDS_ARE_NOT_EQUAL = "Passwords are not equal";
                    
                }
            }
        }
    }
}
