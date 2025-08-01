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
using static IdentityServer8.Models.Constants;

namespace IdentityServer8.Controllers.ViewControllers;

public class AccountController : Controller
{
    private readonly IAccountService _accountService;
    private readonly SignInManager<Account> _signInManager;
    private readonly UserManager<Account> _userManager;
    private readonly IThirdPartyLogin _thirdPartyLogin;
    private readonly IAccountHelper _accountHelper;

    public AccountController(IAccountService accountService,
                             SignInManager<Account> signInManager,
                             UserManager<Account> userManager,
                             IThirdPartyLogin thirdPartyLogin,
                             IAccountHelper accountHelper)
    {
        _accountService = accountService;
        _signInManager = signInManager;
        _userManager = userManager;
        _thirdPartyLogin = thirdPartyLogin;
        _accountHelper = accountHelper;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Login(string returnUrl)
    {
        TempData["ReturnUrl"] = returnUrl;
        TempData.Keep("ReturnUrl");
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        TempData.Keep("ReturnUrl");

        if (!ModelState.IsValid)
        {
            model.ReturnUrl ??= TempData["ReturnUrl"]?.ToString();
            TempData["ReturnUrl"] = model.ReturnUrl;

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_LoginPartial", model);

            return View(model);
        }

        var user = await _accountService.LoginAsync(model);

        if (!user.Succeeded || user.IsNotAllowed)
        {
            ModelState.AddModelError("EmailOrPasswordAreEmpty", "Invalid login attempt.");
            model.ReturnUrl ??= TempData["ReturnUrl"]?.ToString();
            TempData["ReturnUrl"] = model.ReturnUrl;

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_LoginPartial", model);

            return View(model);
        }

        return Redirect(model.ReturnUrl ?? "/");
    }

    [HttpGet]
    public IActionResult Register(string returnUrl)
    {
        return View(new RegisterViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _accountService.RegisterAsync(model);

        if (user.Errors.FirstOrDefault(c => c.Code == Statuses.Register.AccountExists.CODE) != null)
        {
            ModelState.AddModelError(string.Empty, "Account is already existed");
            return View(model.ReturnUrl);
        }

        if (!user.Succeeded)
            return RedirectToAction("Error");

        return Redirect(model.ReturnUrl);
    }

    [HttpGet]
    public IActionResult EmailValidation(string returnUrl)
    {
        var model = new EmailValidationViewModel
        {
            ReturnUrl = string.IsNullOrEmpty(returnUrl)
                ? "/"
                : returnUrl
        };
        
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EmailValidation(EmailValidationViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _accountHelper.FindByEmailOrIdAsync(email: model.Username);
        if (user == null)
        {
            ModelState.AddModelError(nameof(model.Username), "The account doesn't exist");
            return View(model);
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        return RedirectToAction(nameof(CreatePassword), new
        {
            username = model.Username,
            token,
            returnUrl = model.ReturnUrl
        });
    }

    [HttpGet]
    public IActionResult CreatePassword(string username, string token, string returnUrl = "/")
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(token))
        {
            return BadRequest("Invalid password reset link.");
        }

        var model = new ResetPasswordViewModel
        {
            Username = username,
            Token = token,
            ReturnUrl = string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl
        };

        return View(model);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatePassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _accountHelper.FindByEmailOrIdAsync(model.Username);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Invalid user.");
            return View(model);
        }

        var resetResult = await _userManager.ResetPasswordAsync(user, model.Token, model.ConfirmPassword);
        if (!resetResult.Succeeded)
        {
            foreach (var error in resetResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        await _signInManager.SignInAsync(user, isPersistent: false);

        return Redirect(model.ReturnUrl);
    }

    [HttpGet]
    public async Task<IActionResult> Logout(string logoutId)
    {
        var vm = await _accountService.BuildLogoutViewModelAsync(logoutId);
        if (!vm.ShowLogoutPrompt)
        {
            return await Logout(new LogoutInputModel { LogoutId = logoutId });
        }
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout(LogoutInputModel model)
    {
        var redirectUri = await _accountService.LogoutAsync(model);
        return Redirect(redirectUri);
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
        var tlp = await _thirdPartyLogin.MicrosoftTPLCallback();

        if (!tlp.Succeeded)
            return RedirectToAction("Login", returnUrl);

        return LocalRedirect(returnUrl);
    }

}
