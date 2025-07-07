using System;
using IdentityServer8.Entities.Account;
using IdentityServer8.Models.Account;

namespace IdentityServer8.Services.Interfaces;

public interface IAccountHelper
{
    Task<Account?> FindByEmailOrIdAsync(string email = null!, Guid accountId = default);
    AccountDto ConvertToDto(Account account);
}
