using System;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer8.Models.ModelViewModels;

public class LoginViewModel
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    public required string ReturnUrl { get; set; }
}
