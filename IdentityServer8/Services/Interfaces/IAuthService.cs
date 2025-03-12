using System;
using System.Security.Claims;
using IdentityServer8.Entities.Account;
using IdentityServer8.Models.Account;

namespace IdentityServer8.Services.Interfaces;

public interface IAuthService
{
    Task<AccountStatusDto> GetCurrentAccountDetails(ClaimsPrincipal user);
    Task<AccountStatusDto> GetAccountDetailsById(ClaimsPrincipal user, Guid userToFind);
}
