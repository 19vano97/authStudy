using System;

namespace IdentityServer8.Entities.Account;

public class Register
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}
