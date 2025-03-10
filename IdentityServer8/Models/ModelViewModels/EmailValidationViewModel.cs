using System;

namespace IdentityServer8.Models.ModelViewModels;

public class EmailValidationViewModel
{
    public string Username { get; set; }
    public string ReturnUrl { get; set; }
}
