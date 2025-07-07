using System;

namespace IdentityServer8.Models.Account;

public class AccountDto
{
    public Guid? Id { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public bool? IsConfirmed { get; set; }
    public bool IsValid { get; set; }
    public DateTime? CreateDate { get; set; }
    public DateTime? ModifyDate { get; set; }
}
