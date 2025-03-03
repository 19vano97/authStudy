using IdentityServerTest.Data;
using Microsoft.EntityFrameworkCore;
using IdentityServer4;
using IdentityServerTest.Models;
using Microsoft.AspNetCore.Identity;
using IdentityServer4.Models;
using IdentityServerTest.Configs;
using IdentityServer4.Stores;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<IdentityServerTestDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);
builder.Services.AddSingleton<IClientStore, IdentityServerConfig>();
builder.Services.AddIdentity<Account, IdentityRole>()
    .AddEntityFrameworkStores<IdentityServerTestDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddIdentityServer()
    .AddAspNetIdentity<Account>()
    .AddInMemoryIdentityResources(IdentityServerConfig.IdentityResources)
    .AddInMemoryApiScopes(IdentityServerConfig.ApiScopes)
    .AddInMemoryClients(IdentityServerConfig.Clients)
    .AddDeveloperSigningCredential();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseIdentityServer();
app.UseHttpsRedirection();
app.UseIdentityServer();
app.MapControllers();

app.Run();
