using System;
using System.Security.Claims;
using IdentityServer8.Models.Account;

namespace IdentityServer8.Services.Interfaces;

public interface IAuthService
{
    Task<Account> GetCurrentAccountDetails(ClaimsPrincipal user);
    Task<Account> GetAccountDetailsById(ClaimsPrincipal user, Guid userToFind);
}
