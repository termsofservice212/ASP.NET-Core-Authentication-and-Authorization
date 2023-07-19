using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

// MSFT dataprotection interface
builder.Services.AddDataProtection();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/username", (HttpContext ctx, IDataProtectionProvider idp) =>
{
    // Unprotect the payload
    var protector = idp.CreateProtector("auth-cookie");

    var authCookie = ctx.Request.Headers.Cookie.FirstOrDefault(x => x.StartsWith("auth="));
    var protectedPayload = authCookie.Split("=").Last();
    var payload = protector.Unprotect(protectedPayload);
    var parts = payload.Split(":");
    var key = parts[0];
    var value = parts[1];
    return value;
});

app.MapGet("/login", (HttpContext ctx, IDataProtectionProvider idp) =>
{
    // Protect the payload -> Rem to unprotect the payload when using
    // .NET encrypts the cookie value which only .NET can unencrypt
    var protector = idp.CreateProtector("auth-cookie");
    
    ctx.Response.Headers["set-cookie"] = $"auth={protector.Protect("usr:say1hong")}";
    return "ok";
});

app.Run();
