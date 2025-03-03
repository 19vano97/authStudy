using System.Security.Claims;
using authStudyRawCoding.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
// builder.Services.AddDataProtection();
// builder.Services.AddHttpContextAccessor();
// builder.Services.AddScoped<AuthService>();
builder.Services.AddAuthentication("cookie")
    .AddCookie("cookie");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

//middleware
// app.Use((ctx, next) =>
// {
//     var idp = ctx.RequestServices.GetRequiredService<IDataProtectionProvider>();
//     var protector = idp.CreateProtector("auth-cookie");
//     var authCookie = ctx.Request.Headers.Cookie.FirstOrDefault(x => x.StartsWith("auth="));
//     var protectedpayload = authCookie.Split("=").Last();
//     var payload = protector.Unprotect(protectedpayload);
//     var parts = payload.Split(":");
//     var key = parts[0];
//     var value = parts[1];

//     var claim = new List<Claim>{
//         new Claim(key, value)
//     };
//     var identity = new ClaimsIdentity(claim);
//     ctx.User = new ClaimsPrincipal(identity);

//     return next();
// });

app.UseAuthentication();

app.MapGet("/username", (HttpContext ctx) =>
{
    return ctx.User.FindFirst("usr").Value;
});

// app.MapGet("/login", (AuthService authService) =>
// {
//     authService.SignIn();
//     return "ok";
// });

app.MapGet("/login", async (HttpContext ctx) =>
{
    var claim = new List<Claim>{
        new Claim("usr", "ivan")
    };
    var identity = new ClaimsIdentity(claim, "cookie");
    var user  = new ClaimsPrincipal(identity);

    await ctx.SignInAsync("cookie", user);
    return "ok";
});

app.Run();

