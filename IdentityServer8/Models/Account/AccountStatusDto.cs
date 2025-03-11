using System;
using IdentityServer8.Enums;

namespace IdentityServer8.Models.Account;

public class AccountStatusDto
{
    public required AccountDto Account { get; set; }
    public required AccountStatusEnum AccountStatusEnum { get; set; }
}
