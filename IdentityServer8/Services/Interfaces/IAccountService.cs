using System;
using IdentityServer8.Entities.Account;
using IdentityServer8.Enums;
using IdentityServer8.Models.Account;
using IdentityServer8.Models.ModelViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer8.Services.Interfaces;

public interface IAccountService
{
    Task<AccountStatusDto> Login(LoginViewModel model);
    Task<AccountStatusDto> Register(RegisterViewModel model);
    Task<AccountStatusDto> EmailValidation(EmailValidationViewModel model);
    Task<AccountStatusDto> ResetPassword(ResetPasswordViewModel model);
    Task<ActionResult> Logout(string logoutId);
}
