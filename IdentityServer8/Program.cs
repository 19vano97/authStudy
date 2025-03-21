using System.Linq;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.Hosting;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer8.Data;
using IdentityServer8.Entities.Account;
using IdentityServer8.Models.Settings;
using IdentityServer8.Models.Settings.ThirdPartyLogin;
using IdentityServer8.Services;
using IdentityServer8.Services.Implemenrations;
using IdentityServer8.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
var identityServerSettings = new IdentityServerSettings();
var msLogin = new MicrosoftLogin();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddControllers();
builder.Configuration.GetSection("IdentityServerSettings").Bind(identityServerSettings);
builder.Configuration.GetSection("MicrosoftLogin").Bind(msLogin);
builder.Services.AddDbContext<IdentityServer8DbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);
builder.Services.AddIdentity<Account, IdentityRole>()
    .AddEntityFrameworkStores<IdentityServer8DbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddIdentityServer()
    .AddAspNetIdentity<Account>()
    .AddInMemoryIdentityResources(identityServerSettings.IdentityResources.Select(resource => new IdentityServer4.Models.IdentityResource(resource, [resource])))
    .AddInMemoryApiScopes(identityServerSettings.ApiScopes.Select(scope => new IdentityServer4.Models.ApiScope(scope.Name, scope.DisplayName)))
    .AddInMemoryClients(identityServerSettings.Clients.Select(client => new IdentityServer4.Models.Client
    {
        ClientId = client.ClientId,
        AllowedGrantTypes = client.AllowedGrantTypes,
        RequireClientSecret = client.RequireClientSecret,
        RedirectUris = client.RedirectUris,
        PostLogoutRedirectUris = client.PostLogoutRedirectUris,
        AllowedScopes = client.AllowedScopes,
        AllowOfflineAccess = client.AllowOfflineAccess,
        RequirePkce = client.RequirePkce,
        AccessTokenLifetime = client.AccessTokenLifetime,
        AbsoluteRefreshTokenLifetime = client.AbsoluteRefreshTokenLifetime,
        RefreshTokenUsage = client.RefreshTokenUsage
    }))
    .AddInMemoryApiResources(identityServerSettings.ApiResources.Select(res => new IdentityServer4.Models.ApiResource() 
    {
        Name = res.Name, 
        DisplayName = res.DisplayName, 
        Scopes = res.Scopes
    }).ToList())
    .AddDeveloperSigningCredential();
    // .AddOperationalStore(options =>
    // {
    //     options.EnableTokenCleanup = identityServerSettings.EnableTokenCleanup;
    //     options.TokenCleanupInterval = identityServerSettings.TokenCleanupInterval;
    // });
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = "MicrosoftOIDC"; // For OpenID Connect
})
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = identityServerSettings.OwnAccess.Authority;
        options.Audience = identityServerSettings.OwnAccess.Audience;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = identityServerSettings.OwnAccess.Authority,
            ValidAudience = identityServerSettings.OwnAccess.Audience
        };
    })
    .AddJwtBearer(msLogin.JwtMsLogin.Name, options =>
    {
        options.Authority = $"https://login.microsoftonline.com/{msLogin.TenantId}/v2.0"; // Correct authority
        options.Audience = msLogin.JwtMsLogin.Audience; // Microsoft Client ID from Azure
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = $"https://login.microsoftonline.com/{msLogin.TenantId}/v2.0", // Correct tenant ID issuer
            ValidateAudience = true,
            ValidAudience = msLogin.JwtMsLogin.Audience, // Correct audience (your app client ID)
            ValidateLifetime = true
        };
    })
    .AddOpenIdConnect(msLogin.OidcMsLogin.Name, options =>
    {
        options.SignInScheme = msLogin.OidcMsLogin.SignInScheme; // IdentityServer scheme for external authentication
        options.Authority = msLogin.OidcMsLogin.Authority; // Correct authority for your tenant
        options.ClientId = msLogin.ClientId;
        options.ClientSecret = msLogin.ClientSecret;
        options.ResponseType = msLogin.OidcMsLogin.ResponseType;
        options.SaveTokens = msLogin.OidcMsLogin.SaveTokens; // Store tokens for later use
        options.CallbackPath = msLogin.OidcMsLogin.CallbackPath; // Redirect URL
        options.Scope.Add("Calendars.Read");
        options.Scope.Add("offline_access");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = msLogin.OidcMsLogin.Authority, // Correct issuer for your tenant
            ValidateAudience = true,
            ValidAudience = msLogin.ClientId, // Correct audience (your app client ID)
            ValidateLifetime = true
        };
        // foreach (var item in identityServerSettings.ApiResources.Where(r => r.Name == msLogin.OidcMsLogin.Resource).Select(r => r.Scopes).First())
        // {
        //     options.Scope.Add(item);
        // }
    });


builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5168);
    options.ListenAnyIP(7270, listenOptions =>
    {
        listenOptions.UseHttps();
    });
});
builder.Services.AddTransient<IProfileService, CustomProfile>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IThirdPartyLogin, ThirdPartyLogin>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactClient",
        builder => builder
            .WithOrigins("https://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
    options.AddPolicy("AllowMvcClient",
        builder => builder
            .WithOrigins("https://localhost:7124")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseCors("AllowReactClient");
app.UseCors("AllowMvcClient");
app.ConfigureCors();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseIdentityServer();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Index}");

app.Run();
