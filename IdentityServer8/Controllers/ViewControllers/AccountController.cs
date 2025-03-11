using System;
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

public class AccountController : Controller
{
    private readonly IIdentityServerInteractionService _interaction;
    private readonly SignInManager<Account> _signInManager;
    private readonly UserManager<Account> _userManager;
    private readonly IClientStore _clientStore;
    private readonly IdentityServerSettings _settings;
    private readonly IAccountService _accountService;

    public AccountController(
        SignInManager<Account> signInManager, 
        UserManager<Account> userManager,
        IIdentityServerInteractionService identityServerInteractionService,
        IClientStore clientStore,
        IOptions<IdentityServerSettings> settings,
        IAccountService accountService)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _interaction = identityServerInteractionService;
        _clientStore = clientStore;
        _settings = settings.Value;
        _accountService = accountService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Login(string returnUrl = "/")
    {
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) 
            return View(model);

        // var user = await _userManager.FindByNameAsync(model.Username);

        // if (user != null)
        // {
        //     var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

        //     if (result.Succeeded)
        //     {
        //         return Redirect(model.ReturnUrl);
        //     }
        // }

        // ModelState.AddModelError(string.Empty, "Invalid login attempt.");

        // return View(model);

        var user = await _accountService.Login(model);

        if (user.AccountStatusEnum is Enums.AccountStatusEnum.IssueWithLogin
            || user.AccountStatusEnum is Enums.AccountStatusEnum.NotExisted)
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }

        return Redirect(model.ReturnUrl);
    }

    [HttpGet]
    public IActionResult Register(string returnUrl = "/")
    {
        return View(new RegisterViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        // if (await _userManager.FindByNameAsync(newAccount.Username) != null)
        // {
        //     ModelState.AddModelError(string.Empty, "Account is already existed");
        //     return View(newAccount);
        // }

        // var accountToAdd = new Account
        // {
        //     UserName = newAccount.Username,
        //     Email = newAccount.Username,
        //     FirstName = newAccount.FirstName,
        //     LastName = newAccount.LastName
        // };

        // var result = await _userManager.CreateAsync(accountToAdd, newAccount.Password);

        // if (!result.Succeeded)
        //     return RedirectToAction("Error");

        // await _signInManager.SignInAsync(accountToAdd, isPersistent: false);

        // return Redirect(newAccount.ReturnUrl ?? "/");

        var user = await _accountService.Register(model);

        if (user.AccountStatusEnum is Enums.AccountStatusEnum.Existed)
        {
            ModelState.AddModelError(string.Empty, "Account is already existed");
            return View(model);
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

        var user = await _accountService.EmailValidation(model);

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
        var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);
        var user = await _userManager.FindByNameAsync(model.Username);
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        if (!ModelState.IsValid) return View(model);

        var result = await _userManager.ResetPasswordAsync(user, token, model.Password);
        
        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(user, isPersistent: false);
            return Redirect(model.ReturnUrl);
        }

        ModelState.AddModelError(string.Empty, "Invalid login attempt.");

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Logout(string logoutId)
    {
        var logoutRequest = await _interaction.GetLogoutContextAsync(logoutId);
        var postLogoutRedirectUri = _settings.DefaultReturnUri;

        if (!string.IsNullOrEmpty(logoutRequest?.ClientId))
        {
            var client = _clientStore.FindClientByIdAsync(logoutRequest.ClientId).Result;
            
            if (client != null && client.PostLogoutRedirectUris.Any())
                postLogoutRedirectUri = client.PostLogoutRedirectUris.First();
        }

        await _signInManager.SignOutAsync();

        return Redirect(postLogoutRedirectUri);
    }

    public IActionResult Error()
    {
        return View();
    }
}
