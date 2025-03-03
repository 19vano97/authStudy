using System;
using Microsoft.AspNetCore.Identity;

namespace IdentityServerTest.Models;

public class Account : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? CreateDate  { get; set; }
    public DateTime? ModifyDate  { get; set; }
}
