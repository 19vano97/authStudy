using System;
using IdentityServer8.Models.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer8.Services.Interfaces;

public interface IThirdPartyLogin
{
    Task<IdentityResult> MicrosoftTPLCallback();
}
