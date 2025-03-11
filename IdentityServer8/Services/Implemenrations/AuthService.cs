using System;
using System.Security.Claims;
using IdentityServer8.Entities.Account;
using IdentityServer8.Services.Interfaces;

namespace IdentityServer8.Services.Implemenrations;

public class AuthService : IAuthService
{
    public Task<Account> GetAccountDetailsById(ClaimsPrincipal user, Guid userToFind)
    {
        throw new NotImplementedException();
    }

    public Task<Account> GetCurrentAccountDetails(ClaimsPrincipal user)
    {
        throw new NotImplementedException();
    }
}
