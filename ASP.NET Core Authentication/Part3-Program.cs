using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// MSFT dataprotection interface
// builder.Services.AddDataProtection();
// builder.Services.AddHttpContextAccessor();
// Add the AuthService below
//builder.Services.AddScoped<AuthService>();

builder.Services.AddAuthentication("cookie")
    .AddCookie("cookie");

var app = builder.Build();

//app.use((ctx, next) =>
//{
//    var idp = ctx.requestservices.getrequiredservice<idataprotectionprovider>();
//    var protector = idp.createprotector("auth-cookie");

//    var authcookie = ctx.request.headers.cookie.firstordefault(x => x.startswith("auth="));
//    var protectedpayload = authcookie.split("=").last();
//    var payload = protector.unprotect(protectedpayload);
//    var parts = payload.split(":");
//    var key = parts[0];
//    var value = parts[1];

//    var claims = new list<claim>();
//    claims.add(new claim(key, value));
//    var identity = new claimsidentity(claims);

//    // a claimsprincipal is an object that carries information of who you are
//    ctx.user = new claimsprincipal(identity);
    
//    return next();
//});

app.MapGet("/username", (HttpContext ctx) =>
{
    return ctx.User.FindFirst("usr").Value;
});

app.MapGet("/login", async (HttpContext ctx) =>
{
    // Database access comes here

    // Followed by authentication below

    var claims = new List<Claim>();
    claims.Add(new Claim("usr", "say2hong"));
    var identity = new ClaimsIdentity(claims, "cookie");
    var user = new ClaimsPrincipal(identity);

    await ctx.SignInAsync("cookie", user);
    return "ok";
});

app.Run();


//public class AuthService
//{
//    private readonly IDataProtectionProvider _idp;
//    private readonly IHttpContextAccessor _accessor;

//    public AuthService(IDataProtectionProvider idp, IHttpContextAccessor accessor)
//    {
//        _idp = idp;
//        _accessor = accessor;
//    }

//    public void SignIn()
//    {
//        var protector = _idp.CreateProtector("auth-cookie");
//        _accessor.HttpContext.Response.Headers["set-cookie"] = $"auth={protector.Protect("usr:say1hong")}";
//    }
//}
