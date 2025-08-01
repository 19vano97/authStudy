using System;
using System.Linq;
using IdentityServer8.Data;
using IdentityServer8.Entities.Account;
using IdentityServer8.Models.Settings;
using IdentityServer8.Models.Settings.ThirdPartyLogin;
using IdentityServer8.Services.Interfaces;
using IdentityServer8.Services.Implemenrations;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using FluentValidation.AspNetCore;
using FluentValidation;
using IdentityServer8.Models.ModelViewModels.Validator;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// 1) Kestrel configuration
builder.WebHost.ConfigureKestrel(opts =>
{
    opts.ListenAnyIP(5168);                 // HTTP
    opts.ListenAnyIP(7270, lo => lo.UseHttps());  // HTTPS
});

// 2) Bind custom settings from configuration
var idSvrSettings = new IdentityServerSettings();
var msLogin = new MicrosoftLogin();
builder.Configuration.GetSection("IdentityServerSettings").Bind(idSvrSettings);
builder.Configuration.GetSection("MicrosoftLogin").Bind(msLogin);

// 3) MVC + FluentValidation
builder.Services.AddControllersWithViews();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<LoginViewModelValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterViewModelValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<ResetPasswordViewModelValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<EmailValidationViewModelValidator>();
builder.Services.AddDbContext<IdentityServer8DbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 5) ASP.NET Identity setup
builder.Services.AddIdentity<Account, IdentityRole>()
    .AddEntityFrameworkStores<IdentityServer8DbContext>()
    .AddDefaultTokenProviders();

// 6) IdentityServer4 configuration (in-memory grant store)
builder.Services.AddIdentityServer(options =>
    {
        options.Events.RaiseErrorEvents = true;
        options.Events.RaiseFailureEvents = true;
        options.Events.RaiseSuccessEvents = true;
    })
    .AddAspNetIdentity<Account>()
    .AddInMemoryIdentityResources(new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email()
        })
    .AddInMemoryApiResources(idSvrSettings.ApiResources.Select(ar =>
        new ApiResource(ar.Name, ar.DisplayName)
        {
            Scopes = ar.Scopes
        }
    ).ToList())
    .AddInMemoryApiScopes(idSvrSettings.ApiScopes
        .Select(s => new ApiScope(s.Name, s.DisplayName)).ToList())
    .AddInMemoryClients(idSvrSettings.Clients.Select(c => new Client
    {
        ClientId = c.ClientId,
        AllowedGrantTypes = GrantTypes.Code,        
        RequireClientSecret = false,                 
        RedirectUris = c.RedirectUris,
        PostLogoutRedirectUris = c.PostLogoutRedirectUris,
        AllowedScopes = c.AllowedScopes,
        RequirePkce = c.RequirePkce,
        AllowOfflineAccess = c.AllowOfflineAccess,  
        AccessTokenLifetime = c.AccessTokenLifetime,
        AbsoluteRefreshTokenLifetime = c.AbsoluteRefreshTokenLifetime,
        RefreshTokenUsage = c.RefreshTokenUsage,
        RefreshTokenExpiration = TokenExpiration.Sliding,
        SlidingRefreshTokenLifetime = c.AbsoluteRefreshTokenLifetime,
        AllowedCorsOrigins = new[] { "https://localhost:5173", "https://localhost:7124", "https://localhost:7188" },
    }).ToList())
    .AddDeveloperSigningCredential();

builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IThirdPartyLogin, ThirdPartyLogin>();
builder.Services.AddScoped<IAccountHelper, AccountHelper>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = msLogin.OidcMsLogin.Name;
})
.AddJwtBearer("Bearer", opts =>
{
    opts.Authority = idSvrSettings.OwnAccess.Authority;
    opts.Audience = idSvrSettings.OwnAccess.Audience;
    opts.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = idSvrSettings.OwnAccess.Authority,
        ValidAudience = idSvrSettings.OwnAccess.Audience
    };
})
.AddOpenIdConnect(msLogin.OidcMsLogin.Name, opts =>
{
    opts.SignInScheme = msLogin.OidcMsLogin.SignInScheme;
    opts.Authority = msLogin.OidcMsLogin.Authority;
    opts.ClientId = msLogin.ClientId;
    opts.ClientSecret = msLogin.ClientSecret;
    opts.ResponseType = msLogin.OidcMsLogin.ResponseType;
    opts.SaveTokens = msLogin.OidcMsLogin.SaveTokens;
    opts.CallbackPath = msLogin.OidcMsLogin.CallbackPath;
    opts.Scope.Add("Calendars.Read");
    opts.Scope.Add("offline_access");
    opts.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = msLogin.OidcMsLogin.Authority,
        ValidateAudience = true,
        ValidAudience = msLogin.ClientId,
        ValidateLifetime = true
    };
});

builder.Services.AddCors(o =>
{
    o.AddPolicy("AllowAllClients", p => p
        .WithOrigins("https://localhost:5173", "https://localhost:7124", "https://localhost:7188")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
    );
});
builder.Host.UseSerilog((ctx, cfg) =>
{
    cfg.ReadFrom.Configuration(ctx.Configuration);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseCors("AllowAllClients");
app.UseIdentityServer();
app.UseAuthentication();
app.UseAuthorization();
app.MapDefaultControllerRoute();

app.Run();