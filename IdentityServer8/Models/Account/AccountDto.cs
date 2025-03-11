using System;

namespace IdentityServer8.Models.Account;

public class AccountDto
{
    public required Guid Id { get; set; }
    public required string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    // public required string AccessToken { get; set; }
    // public required string RefreshToken { get; set; }
    public required DateTime CreateDate { get; set; }
}
