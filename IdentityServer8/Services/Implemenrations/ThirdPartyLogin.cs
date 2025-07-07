using System;
using System.Security.Claims;
using IdentityServer8.Entities.Account;
using IdentityServer8.Models;
using IdentityServer8.Models.Account;
using IdentityServer8.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer8.Services.Implemenrations;

public class ThirdPartyLogin : IThirdPartyLogin
{
    private readonly SignInManager<Account> _signInManager;
    private readonly UserManager<Account> _userManager;
    private readonly ILogger<ThirdPartyLogin> _logger;

    public ThirdPartyLogin(SignInManager<Account> signInManager,
                           UserManager<Account> userManager,
                           ILogger<ThirdPartyLogin> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<IdentityResult> MicrosoftTPLCallback()
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();

        if (info is null)
        {
            return IdentityResult.Failed(new IdentityError
            {
                Code = Constants.Statuses.ThirdParty.ExternalLoginInformation.NotFound.CODE,
                Description = Constants.Statuses.ThirdParty.ExternalLoginInformation.NotFound.DESCRIPTION
            });
        }

        var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);

        if (result.Succeeded)
            return IdentityResult.Success;

        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        var firstName = info.Principal.FindFirstValue(ClaimTypes.Name);
        var lastName = info.Principal.FindFirstValue(ClaimTypes.Surname);
        var user = new Account { UserName = email, Email = email, FirstName = firstName, LastName = lastName };
        var createResult = await _userManager.CreateAsync(user);

        if (createResult.Succeeded)
        {
            await _userManager.AddLoginAsync(user, info);
            await _signInManager.SignInAsync(user, isPersistent: false);

            return IdentityResult.Success;
        }

        return IdentityResult.Failed(new IdentityError
        {
            Code = Constants.Statuses.ThirdParty.ExternalLoginAccountCreation.Failed.CODE,
            Description = Constants.Statuses.ThirdParty.ExternalLoginAccountCreation.Failed.DESCRIPTION
        });
    }
}
