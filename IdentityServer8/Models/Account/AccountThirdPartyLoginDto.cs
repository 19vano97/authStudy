using System;

namespace IdentityServer8.Models.Account;

public class AccountThirdPartyLoginDto : AccountDto
{
    public string ThirdPartyLoginAccessToken { get; set; }
}
