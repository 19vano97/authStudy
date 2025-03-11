using System.Linq;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer8.Data;
using IdentityServer8.Entities.Account;
using IdentityServer8.Models.Settings;
using IdentityServer8.Services;
using IdentityServer8.Services.Implemenrations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
var identityServerSettings = new IdentityServerSettings();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddControllers();
builder.Configuration.GetSection("IdentityServerSettings").Bind(identityServerSettings);
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
        RequirePkce = client.RequirePkce
    }))
    .AddInMemoryApiResources(identityServerSettings.ApiResources.Select(res => new IdentityServer4.Models.ApiResource() 
    {
        Name = res.Name, 
        DisplayName = res.DisplayName, 
        Scopes = res.Scopes
    }).ToList())
    .AddDeveloperSigningCredential();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = identityServerSettings.OwnAccess.Authority;
        options.Audience = identityServerSettings.OwnAccess.Audience;
    });
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5168); // HTTP
    options.ListenAnyIP(7270, listenOptions =>
    {
        listenOptions.UseHttps();
    });
});
builder.Services.AddTransient<IProfileService, CustomProfile>();
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
