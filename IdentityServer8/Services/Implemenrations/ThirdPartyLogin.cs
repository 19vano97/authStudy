using System;
using System.Security.Claims;
using IdentityServer8.Entities.Account;
using IdentityServer8.Models.Account;
using IdentityServer8.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer8.Services.Implemenrations;

public class ThirdPartyLogin(SignInManager<Account> signInManager, UserManager<Account> userManager) : IThirdPartyLogin
{
    public async Task<AccountStatusDto> MicrosoftTPLCallback()
    {
        var info = await signInManager.GetExternalLoginInfoAsync();
        // var accessToken = info.AuthenticationTokens.Where(p => p.Name == "access_token").Select(p => p.Value).FirstOrDefault();
        // var idToken = info.AuthenticationTokens.Where(p => p.Name == "id_token").Select(p => p.Value).FirstOrDefault();
        // var idToken = info.AuthenticationTokens.Where(p => p.Name == "refresh_token").Select(p => p.Value).FirstOrDefault();
        // System.Console.WriteLine(accessToken);
        if (info == null)
            return GeneralMethods.SetAccountStatusFromAccount();

        var result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);

        if (result.Succeeded)
            return GeneralMethods.SetAccountStatusFromAccount(accountStatus: Enums.AccountStatusEnum.Existed);

        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        var firstName = info.Principal.FindFirstValue(ClaimTypes.Name);
        var lastName = info.Principal.FindFirstValue(ClaimTypes.Surname);
        var user = new Account { UserName = email, Email = email, FirstName = firstName, LastName = lastName };
        var createResult = await userManager.CreateAsync(user);

        if (createResult.Succeeded)
        {
            await userManager.AddLoginAsync(user, info);
            await signInManager.SignInAsync(user, isPersistent: false);
            return GeneralMethods.SetAccountStatusFromAccount(accountStatus: Enums.AccountStatusEnum.Existed);
        }

        return GeneralMethods.SetAccountStatusFromAccount();
    }
}
