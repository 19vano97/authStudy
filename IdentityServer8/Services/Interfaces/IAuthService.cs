using System;
using System.Security.Claims;
using IdentityServer8.Entities.Account;
using IdentityServer8.Models.Account;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer8.Services.Interfaces;

public interface IAuthService
{
    Task<AccountDto> GetCurrentAccountDetailsAsync(ClaimsPrincipal user);
    Task<AccountDto> UpdateAccountDetailsAsync(ClaimsPrincipal user, AccountDto account);
    Task<AccountDto> GetAccountDetailsByIdAsync(ClaimsPrincipal user, Guid userToFind);
    Task<IdentityResult> InviteNewAccountAsync(AccountDto account);
}
