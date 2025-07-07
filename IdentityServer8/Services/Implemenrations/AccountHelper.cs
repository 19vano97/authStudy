using System;
using IdentityServer8.Entities.Account;
using IdentityServer8.Models.Account;
using IdentityServer8.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer8.Services.Implemenrations;

public class AccountHelper : IAccountHelper
{
    private readonly UserManager<Account> _userManager;

    public AccountHelper(UserManager<Account> userManager)
    {
        _userManager = userManager;
    }

    public AccountDto ConvertToDto(Account account)
    {
        try
        {
            return new AccountDto
            {
                Id = Guid.Parse(account.Id),
                FirstName = account.FirstName,
                LastName = account.LastName,
                Email = account.Email,
                IsValid = true,
                IsConfirmed = account.EmailConfirmed,
                CreateDate = account.CreateDate,
                ModifyDate = account.ModifyDate
            };
        }
        catch (System.Exception err)
        {
            return new AccountDto { IsValid = false };
        }
    }

    public async Task<Account?> FindByEmailOrIdAsync(string email = null!, Guid accountId = default)
    {
        if (accountId != default)
            return await _userManager.FindByIdAsync(accountId.ToString()) ?? null!;
        if (email != null)
            return await _userManager.FindByEmailAsync(email) ?? null!;

        return null!;
    }
}
