using System;

namespace IdentityServer8.Models.ModelViewModels;

public class ResetPasswordViewModel
{
    public string? Username { get; set; }
    public string? Token { get; set; }
    public string? NewPassword { get; set; }
    public string? ConfirmPassword { get; set; }
    public required string ReturnUrl { get; set; }
}

