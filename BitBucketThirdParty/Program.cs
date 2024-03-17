using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add authentication services
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = "Bitbucket";
})
.AddCookie()
.AddOAuth("Bitbucket", options =>
{
    options.ClientId = builder.Configuration["Bitbucket:ClientId"];
    options.ClientSecret = builder.Configuration["Bitbucket:ClientSecret"];
    options.CallbackPath = "/signin-bitbucket";

    options.AuthorizationEndpoint = "https://bitbucket.org/site/oauth2/authorize";
    options.TokenEndpoint = "https://bitbucket.org/site/oauth2/access_token";
    options.UserInformationEndpoint = "https://api.bitbucket.org/2.0/user";

    options.ClaimActions.MapJsonKey("urn:bitbucket:avatar", "avatar_url");

    options.Events = new OAuthEvents
    {
        OnCreatingTicket = async context =>
        {
            // Customize user claims here
        }
    };
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
app.UseStaticFiles();

app.UseRouting();

// Authentication should be used before authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
