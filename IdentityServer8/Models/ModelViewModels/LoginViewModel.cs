using System;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer8.Models.ModelViewModels;

public class LoginViewModel
{
    [Required]
    public string Username { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    public string ReturnUrl { get; set; }
}
