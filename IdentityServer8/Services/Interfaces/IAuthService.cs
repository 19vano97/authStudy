using System;
using System.Security.Claims;
using IdentityServer8.Entities.Account;
using IdentityServer8.Models.Account;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer8.Services.Interfaces;

public interface IAuthService
{
    Task<AccountDto> GetCurrentAccountDetailsAsync(ClaimsPrincipal user, CancellationToken cancellationToken);
    Task<AccountDto> UpdateAccountDetailsAsync(ClaimsPrincipal user, AccountDto account, CancellationToken cancellationToken);
    Task<AccountDto> GetAccountDetailsByIdAsync(ClaimsPrincipal user, Guid userToFind, CancellationToken cancellationToken);
    Task<AccountDto> InviteNewAccountAsync(AccountDto account, CancellationToken cancellationToken);
}
