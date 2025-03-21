using System;
using IdentityServer8.Models.Account;
using Microsoft.AspNetCore.Authentication;

namespace IdentityServer8.Services.Interfaces;

public interface IThirdPartyLogin
{
    Task<AccountStatusDto> MicrosoftTPLCallback();
}
