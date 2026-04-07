using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<AppDataStore>();
builder.Services.AddSingleton<DemoTimeProvider>();

builder.Services
    .AddAuthentication(DemoAuthHandler.SchemeName)
    .AddScheme<AuthenticationSchemeOptions, DemoAuthHandler>(DemoAuthHandler.SchemeName, _ => { });

builder.Services.AddAuthorization();
builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173", "https://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("frontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/go/{slug}", (string slug, HttpContext httpContext, AppDataStore store, DemoTimeProvider clock) =>
{
    var result = store.ResolveAndTrackClick(slug, httpContext, clock.UtcNow);

    if (!result.Found)
    {
        return Results.NotFound(new { message = "Shortlink không tồn tại." });
    }

    if (!result.Allowed)
    {
        return Results.BadRequest(new
        {
            message = result.Reason,
            requiresPassword = result.RequiresPassword
        });
    }

    return Results.Redirect(result.TargetUrl!, false, true);
});

app.Run();

public sealed class DemoTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}

public sealed class DemoAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SchemeName = "DemoBearer";

    private readonly AppDataStore _store;

    public DemoAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        AppDataStore store)
        : base(options, logger, encoder)
    {
        _store = store;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.Authorization.Any())
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var header = Request.Headers.Authorization.ToString();
        if (!header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(AuthenticateResult.Fail("Token không hợp lệ."));
        }

        var token = header["Bearer ".Length..].Trim();
        var user = _store.GetUserByToken(token);
        if (user is null)
        {
            return Task.FromResult(AuthenticateResult.Fail("Phiên đăng nhập đã hết hạn."));
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Email),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role)
        };

        var identity = new ClaimsIdentity(claims, SchemeName);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SchemeName);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
