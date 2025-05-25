using System;
using System.Security.Claims;
using IdentityModel.Client;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer8.Entities.Account;
using IdentityServer8.Models.ModelViewModels;
using IdentityServer8.Models.Settings;
using IdentityServer8.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace IdentityServer8.Controllers.ViewControllers;

public class AccountController(IAccountService accountService, 
                               SignInManager<Account> _signInManager, 
                               UserManager<Account> _userManager,
                               IThirdPartyLogin thirdPartyLogin) : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Login(string returnUrl)
    {
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) 
            return View(model);

        var user = await accountService.Login(model);

        if (user.AccountStatusEnum is Enums.AccountStatusEnum.IssueWithLogin
            || user.AccountStatusEnum is Enums.AccountStatusEnum.NotExisted)
        {
            ModelState.AddModelError("EmailOrPasswordAreEmpty", "Invalid login attempt.");
            return View(model);
        }

        return Redirect(model.ReturnUrl);
    }

    [HttpGet]
    public IActionResult Register(string returnUrl)
    {
        return View(new RegisterViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        var user = await accountService.Register(model);

        if (user.AccountStatusEnum is Enums.AccountStatusEnum.Existed)
        {
            ModelState.AddModelError(string.Empty, "Account is already existed");
            return View(model.ReturnUrl);
        }

        if (user.AccountStatusEnum is Enums.AccountStatusEnum.IssueWithLogin)
            return RedirectToAction("Error");

        return Redirect(model.ReturnUrl);
    }

    [HttpGet]
    public IActionResult EmailValidation(string returnUrl)
    {
        return View(new EmailValidationViewModel {ReturnUrl = returnUrl});
    }

    [HttpPost]
    public async Task<IActionResult> EmailValidation(EmailValidationViewModel model)
    {
        if (!ModelState.IsValid) 
            return View(model);

        var user = await accountService.EmailValidation(model);

        if (user.AccountStatusEnum is Enums.AccountStatusEnum.NotExisted)
        {
            ModelState.AddModelError(string.Empty, "The account doesn't exist");
            return View(model);
        }

        return RedirectToAction("CreatePassword", model);
    }

    [HttpGet]
    public IActionResult CreatePassword(EmailValidationViewModel emailValidation)
    {
        return View(new ResetPasswordViewModel{ Username = emailValidation.Username, ReturnUrl = emailValidation.ReturnUrl});
    }

    [HttpPost]
    public async Task<IActionResult> CreatePassword(ResetPasswordViewModel model)
    {
        var user = await accountService.ResetPassword(model);

        if (user.AccountStatusEnum is Enums.AccountStatusEnum.IssueWithLogin
            || user.AccountStatusEnum is Enums.AccountStatusEnum.NotExisted
            || user.Account is null)
        {
            ModelState.AddModelError(string.Empty, "Invalid reset password attempt.");
            return View(model);
        }

        return Redirect(model.ReturnUrl);
    }

    [HttpGet]
    public async Task<IActionResult> Logout(string logoutId)
    {
        return Redirect(await accountService.Logout(logoutId));
    }

    public IActionResult Error()
    {
        return View();
    }

    public IActionResult ExternalLogin(string provider, string returnUrl)
    {
        if (provider == "Microsoft")
        {
            var redirectUrl = Url.Action(nameof(MicrosoftExternalLoginCallback), "Account", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            return Challenge(properties, "MicrosoftOIDC"); 
        }

        return RedirectToAction("Error", "Home");
    }


    public async Task<IActionResult> MicrosoftExternalLoginCallback(string returnUrl)
    {
        var tlp = await thirdPartyLogin.MicrosoftTPLCallback();

        if (tlp.AccountStatusEnum == Enums.AccountStatusEnum.NotExisted)
            return RedirectToAction("Login", returnUrl);

        return LocalRedirect(returnUrl);
    }

}
