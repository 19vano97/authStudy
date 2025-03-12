using System;
using IdentityServer8.Entities.Account;
using IdentityServer8.Enums;
using IdentityServer8.Models.Account;
using IdentityServer8.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer8.Services.Implemenrations;

public static class GeneralMethods
{
    public static AccountStatusDto SetAccountStatusFromAccount(Account account = null, 
        AccountStatusEnum accountStatus = AccountStatusEnum.None)
    {
        if (account is null && accountStatus == AccountStatusEnum.None)
            return new AccountStatusDto
            {
                Account = null!,
                AccountStatusEnum = AccountStatusEnum.NotExisted
            };

        if (account != null && accountStatus is AccountStatusEnum.None)
            return new AccountStatusDto
            {
                Account = new AccountDto
                {
                    Id = Guid.Parse(account.Id),
                    Email = account.Email!,
                    FirstName = account.FirstName,
                    LastName = account.LastName,
                    CreateDate = (DateTime)account.CreateDate!
                },
                AccountStatusEnum = AccountStatusEnum.Existed
            };

        return new AccountStatusDto
        {
            Account = new AccountDto
            {
                Id = Guid.Parse(account.Id),
                Email = account.Email!,
                FirstName = account.FirstName,
                LastName = account.LastName,
                CreateDate = (DateTime)account.CreateDate!
            },
            AccountStatusEnum = accountStatus
        };
    }

    public static async Task<Account> IsAccountExisted(UserManager<Account> userManager,
        string accountId = null, string email = null)
    {
        if (accountId != null)
            return await userManager.FindByIdAsync(accountId) ?? null!;

        if (email != null)
            return await userManager.FindByEmailAsync(email) ?? null!;
            
        return null!;
    }
}
