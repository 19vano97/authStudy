using System.Linq;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using IdentityServer8.Data;
using IdentityServer8.Models.Account;
using IdentityServer8.Models.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
var identityServerSettings = new IdentityServerSettings();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactClient",
        builder => builder
            .WithOrigins("https://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});
builder.Configuration.GetSection("IdentityServerSettings").Bind(identityServerSettings);
builder.Services.AddDbContext<IdentityServer8DbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);
builder.Services.AddIdentity<Account, IdentityRole>()
    .AddEntityFrameworkStores<IdentityServer8DbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddIdentityServer()
    .AddAspNetIdentity<Account>()
    .AddInMemoryIdentityResources(identityServerSettings.IdentityResources.Select(resource => new IdentityServer4.Models.IdentityResource(resource, new[] { resource })))
    .AddInMemoryApiScopes(identityServerSettings.ApiScopes.Select(scope => new IdentityServer4.Models.ApiScope(scope.Name, scope.DisplayName)))
    .AddInMemoryClients(identityServerSettings.Clients.Select(client => new IdentityServer4.Models.Client{
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
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5168); // HTTP
    options.ListenAnyIP(7270, listenOptions =>
    {
        listenOptions.UseHttps(); // Enable HTTPS
    });
});
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Debug);
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

