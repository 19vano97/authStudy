using System;
using System.Security.Claims;
using IdentityServer8.Entities.Account;
using IdentityServer8.Models.Account;
using IdentityServer8.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using static IdentityServer8.Models.Constants;

namespace IdentityServer8.Services.Implemenrations;

public class AuthService(UserManager<Account> userManager) : IAuthService
{
    public async Task<AccountStatusDto> GetAccountDetailsById(ClaimsPrincipal user, Guid userToFind)
    {
        var userId = user.FindFirst(IdentityCustomOpenId.DetailsFromToken.SID_KEY)?.Value;

        if (string.IsNullOrEmpty(userId))
            return GeneralMethods.SetAccountStatusFromAccount(accountStatus: Enums.AccountStatusEnum.IssueWithLogin);

        var res = await GeneralMethods.IsAccountExisted(userManager, accountId: userToFind.ToString());

        if (res is null)
            return GeneralMethods.SetAccountStatusFromAccount(accountStatus: Enums.AccountStatusEnum.NotExisted);

        return GeneralMethods.SetAccountStatusFromAccount(res);
    }

    public async Task<AccountStatusDto> GetCurrentAccountDetails(ClaimsPrincipal user)
    {
        var userId = user.FindFirst(IdentityCustomOpenId.DetailsFromToken.SID_KEY)?.Value;

        if (string.IsNullOrEmpty(userId))
            return GeneralMethods.SetAccountStatusFromAccount(accountStatus: Enums.AccountStatusEnum.IssueWithLogin);

        var res = await GeneralMethods.IsAccountExisted(userManager, accountId: userId);

        if (res is null)
            return GeneralMethods.SetAccountStatusFromAccount(accountStatus: Enums.AccountStatusEnum.NotExisted);

        return GeneralMethods.SetAccountStatusFromAccount(res);
    }
}
