using System;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer8.Models.ModelViewModels;

public class RegisterViewModel
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public required string ReturnUrl { get; set; }
}
