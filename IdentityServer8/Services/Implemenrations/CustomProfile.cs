using System;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer8.Models.Account;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer8.Services.Implemenrations;

public class CustomProfile : IProfileService
{
    private readonly UserManager<Account> _userManager;

    public CustomProfile(UserManager<Account> userManager)
    {
        _userManager = userManager;
    }

    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var user = await _userManager.GetUserAsync(context.Subject);

        if (user?.Email != null)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Email, user.Email ?? string.Empty)
            };

            context.IssuedClaims.AddRange(claims);
        }
    }

    public Task IsActiveAsync(IsActiveContext context)
    {
        return Task.CompletedTask;
    }
}
