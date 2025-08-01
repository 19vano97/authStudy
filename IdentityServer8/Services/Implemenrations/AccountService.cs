using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer8.Entities.Account;
using IdentityServer8.Models;
using IdentityServer8.Models.Account;
using IdentityServer8.Models.ModelViewModels;
using IdentityServer8.Models.Settings;
using IdentityServer8.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace IdentityServer8.Services.Implemenrations
{
    public class AccountService : IAccountService
    {
        private readonly SignInManager<Account> _signInManager;
        private readonly UserManager<Account> _userManager;
        private readonly IClientStore _clientStore;
        private readonly IdentityServerSettings _settings;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly ILogger<AccountService> _logger;

        public AccountService(
            SignInManager<Account> signInManager,
            UserManager<Account> userManager,
            IClientStore clientStore,
            IOptions<IdentityServerSettings> settings,
            IIdentityServerInteractionService interaction,
            ILogger<AccountService> logger)
        {
            _signInManager = signInManager;
            _userManager   = userManager;
            _clientStore   = clientStore;
            _settings = settings.Value;
            _interaction = interaction;
            _logger = logger;
        }

        public async Task<SignInResult> LoginAsync(LoginViewModel model)
        {
            _logger.LogInformation("Attempting login for user {Username}", model.Username);

            var user = await _userManager.FindByEmailAsync(model.Username);
            if (user == null)
            {
                _logger.LogWarning("Login failed: user {Username} not found", model.Username);
                return SignInResult.Failed;
            }

            var result = await _signInManager
                .PasswordSignInAsync(user, model.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.IsNotAllowed)
            {
                _logger.LogInformation("Login result for {Username}: {IsNotAllowed}", model.Username, result.IsNotAllowed);
                return result;
            }

            _logger.LogInformation("Login result for {Username}: {Succeeded}", model.Username, result.Succeeded);
            return result;
        }

        public async Task<string> LogoutAsync(string logoutId)
        {
            _logger.LogInformation("Processing logout for LogoutId {LogoutId}", logoutId);

            var logoutRequest = await _interaction.GetLogoutContextAsync(logoutId);
            var redirectUri = !string.IsNullOrEmpty(_settings.DefaultReturnUri)
                ? _settings.DefaultReturnUri
                : "/";

            if (!string.IsNullOrEmpty(logoutRequest?.ClientId))
            {
                var client = await _clientStore.FindClientByIdAsync(logoutRequest.ClientId);
                if (client != null && client.PostLogoutRedirectUris.Any())
                {
                    redirectUri = client.PostLogoutRedirectUris.First();
                }
            }

            await _signInManager.SignOutAsync();
            _logger.LogInformation("User signed out for LogoutId {LogoutId}", logoutId);

            return redirectUri;
        }

        public async Task<LogoutViewModel> BuildLogoutViewModelAsync(string logoutId)
        {
            _logger.LogInformation("Building LogoutViewModel for LogoutId {LogoutId}", logoutId);

            var ctx = await _interaction.GetLogoutContextAsync(logoutId);
            return new LogoutViewModel
            {
                LogoutId         = logoutId,
                ShowLogoutPrompt = ctx?.ShowSignoutPrompt ?? true
            };
        }

        public async Task<string> LogoutAsync(LogoutInputModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            _logger.LogInformation("Logout POST invoked for LogoutId {LogoutId}", model.LogoutId);

            await _signInManager.SignOutAsync();

            var ctx = await _interaction.GetLogoutContextAsync(model.LogoutId);
            var redirectUri = _settings.DefaultReturnUri ?? "/";

            if (!string.IsNullOrEmpty(ctx?.ClientId))
            {
                var client = await _clientStore.FindClientByIdAsync(ctx.ClientId);
                if (client?.PostLogoutRedirectUris.Any() == true)
                {
                    redirectUri = client.PostLogoutRedirectUris.First();
                }
            }

            _logger.LogInformation("Redirecting after logout to {RedirectUri}", redirectUri);
            return redirectUri;
        }

        public async Task<IdentityResult> RegisterAsync(RegisterViewModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            _logger.LogInformation("Registering new user {Username}", model.Username);

            if (await _userManager.FindByEmailAsync(model.Username) != null)
            {
                _logger.LogWarning("Registration failed: user {Username} already exists", model.Username);
                return IdentityResult.Failed(new IdentityError
                {
                    Code = Constants.Statuses.Register.AccountExists.CODE,
                    Description = Constants.Statuses.Register.AccountExists.DESCRIPTION
                });
            }

            var account = new Account
            {
                UserName  = model.Username,
                Email     = model.Username,
                FirstName = model.FirstName,
                LastName  = model.LastName
            };

            var result = await _userManager.CreateAsync(account, model.Password!);
            if (!result.Succeeded)
            {
                _logger.LogWarning("Registration errors for {Username}: {Errors}", model.Username,
                    string.Join(", ", result.Errors.Select(e => e.Description)));
                return result;
            }

            _logger.LogInformation("User {Username} registered successfully", model.Username);
            await _signInManager.SignInAsync(account, isPersistent: false);

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordViewModel model)
        {
            _logger.LogInformation("ResetPasswordAsync called for {Username}", model.Username);

            var user = await _userManager.FindByEmailAsync(model.Username);
            if (user == null)
            {
                _logger.LogWarning("Password reset failed: user {Username} not found", model.Username);
                return IdentityResult.Failed(new IdentityError
                {
                    Code = Constants.Statuses.Find.ByEmail.NotFound.CODE,
                    Description = Constants.Statuses.Find.ByEmail.NotFound.DESCRIPTION
                });
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            _logger.LogInformation("Generated password reset token for {Username}", model.Username);

            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
            if (!result.Succeeded)
            {
                _logger.LogWarning("Password reset errors for {Username}: {Errors}", model.Username,
                    string.Join(", ", result.Errors.Select(e => e.Description)));
                return result;
            }

            _logger.LogInformation("Password reset succeeded for {Username}", model.Username);
            await _signInManager.SignInAsync(user, isPersistent: false);
            return IdentityResult.Success;
        }
    }
}