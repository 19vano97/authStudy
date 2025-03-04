using System;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer8.Models.Account;
using IdentityServer8.Models.ModelViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer8.Controllers.ViewControllers;

public class AccountController : Controller
{
    private readonly IIdentityServerInteractionService _identityServerInteractionService;
    private readonly SignInManager<Account> _signInManager;
    private readonly UserManager<Account> _userManager;
    private readonly IClientStore _clientStore;

    public AccountController(
        SignInManager<Account> signInManager, 
        UserManager<Account> userManager,
        IIdentityServerInteractionService identityServerInteractionService,
        IClientStore clientStore)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _identityServerInteractionService = identityServerInteractionService;
        _clientStore = clientStore;
    }

    [HttpGet]
    public IActionResult Login(string returnUrl = "/")
    {
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _userManager.FindByNameAsync(model.Username);
        if (user != null)
        {
            var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
            if (result.Succeeded)
            {
                return Redirect(model.ReturnUrl);
            }
        }

        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return View(model);
    }

    [HttpGet]
    public IActionResult Register(string returnUrl = "/")
    {
        return View(new RegisterViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel newAccount)
    {
        if (await _userManager.FindByNameAsync(newAccount.Username) != null)
            return BadRequest("User has already existed"); // TODO: chnage to a proper view

        var accountToAdd = new Account
        {
            UserName = newAccount.Username,
            Email = newAccount.Username,
            FirstName = newAccount.FirstName,
            LastName = newAccount.LastName
        };

        var result = await _userManager.CreateAsync(accountToAdd, newAccount.Password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return RedirectToAction("Login", new LoginViewModel { 
            Username = newAccount.Username,
            Password = newAccount.Password,
            ReturnUrl = newAccount.ReturnUrl});
    }

    public IActionResult OnPostRedirectToRegisterPage(string returnUrl = "/")
    {
        return RedirectToAction("Register", returnUrl);
    }

    [HttpGet]
    public async Task<IActionResult> Logout(string logoutId)
    {
        var logoutRequest = await _identityServerInteractionService.GetLogoutContextAsync(logoutId);
        var postLogoutRedirectUri = "https://localhost:7270";

        if (!string.IsNullOrEmpty(logoutRequest?.ClientId))
        {
            var client = _clientStore.FindClientByIdAsync(logoutRequest.ClientId).Result;
            
            if (client != null && client.PostLogoutRedirectUris.Any())
                postLogoutRedirectUri = client.PostLogoutRedirectUris.First();
        }

        await _signInManager.SignOutAsync();

        return Redirect(postLogoutRedirectUri);
    }
}
