using System;

namespace IdentityServer8.Models.ModelViewModels;

public class ResetPasswordViewModel
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string ReturnUrl { get; set; }
}
