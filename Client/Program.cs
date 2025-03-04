var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "oidc";
})
.AddCookie("Cookies")
.AddOpenIdConnect("oidc", options =>
{
    options.Authority = "https://localhost:7270";
    options.ClientId = "react-client";
    options.ResponseType = "code";
    options.SaveTokens = true;
    options.ResponseType = "code";
    options.UsePkce = true;
    options.SaveTokens = true;

    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("api.read");
    options.Scope.Add("offline_access");
    //"openid", "profile", "api.read", "offline_access"

    options.CallbackPath = "/signin-oidc";

    options.GetClaimsFromUserInfoEndpoint = true;
});
builder.Services.AddControllers();
// builder.Services.ConfigureApplicationCookie(options =>
// {
//     options.Cookie.SameSite = SameSiteMode.Lax; // Allows cross-origin cookies if needed
// });
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5297); // HTTP
    options.ListenAnyIP(7124, listenOptions =>
    {
        listenOptions.UseHttps(); // Enable HTTPS
    });
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();
app.UseAuthentication();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
