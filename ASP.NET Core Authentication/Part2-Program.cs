using Microsoft.AspNetCore.DataProtection;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// MSFT dataprotection interface
builder.Services.AddDataProtection();

builder.Services.AddHttpContextAccessor();
// Add the AuthService below
builder.Services.AddScoped<AuthService>();

var app = builder.Build();

app.Use((ctx, next) =>
{
    var idp = ctx.RequestServices.GetRequiredService<IDataProtectionProvider>();
    var protector = idp.CreateProtector("auth-cookie");

    var authCookie = ctx.Request.Headers.Cookie.FirstOrDefault(x => x.StartsWith("auth="));
    var protectedPayload = authCookie.Split("=").Last();
    var payload = protector.Unprotect(protectedPayload);
    var parts = payload.Split(":");
    var key = parts[0];
    var value = parts[1];

    var claims = new List<Claim>();
    claims.Add(new Claim(key, value));
    var identity = new ClaimsIdentity(claims);

    // A ClaimsPrincipal is an object that carries information of who you are
    ctx.User = new ClaimsPrincipal(identity);
    
    return next();
});

app.MapGet("/username", (HttpContext ctx) =>
{
    return ctx.User.FindFirst("usr").Value;
});

app.MapGet("/login", (AuthService auth) =>
{
    auth.SignIn();
    return "ok";
});

app.Run();


public class AuthService
{
    private readonly IDataProtectionProvider _idp;
    private readonly IHttpContextAccessor _accessor;

    public AuthService(IDataProtectionProvider idp, IHttpContextAccessor accessor)
    {
        _idp = idp;
        _accessor = accessor;
    }

    public void SignIn()
    {
        var protector = _idp.CreateProtector("auth-cookie");
        _accessor.HttpContext.Response.Headers["set-cookie"] = $"auth={protector.Protect("usr:say1hong")}";
    }
}
