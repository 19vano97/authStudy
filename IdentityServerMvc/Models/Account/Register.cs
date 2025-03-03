using System;

namespace IdentityServerMvc.Models.Account;

public class Register
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}
