using System;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer8.Entities.Account;

public class Account : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? CreateDate  { get; set; }
    public DateTime? ModifyDate  { get; set; }
}
