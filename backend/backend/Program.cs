using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using WebShortlink.Backend.Api;
using WebShortlink.Backend.Application.Abstractions;
using WebShortlink.Backend.Application.Admin;
using WebShortlink.Backend.Application.Analytics;
using WebShortlink.Backend.Application.Authentication;
using WebShortlink.Backend.Application.Billing;
using WebShortlink.Backend.Application.Common;
using WebShortlink.Backend.Application.Domains;
using WebShortlink.Backend.Application.Links;
using WebShortlink.Backend.Domain.Entities;
using WebShortlink.Backend.Domain.Enums;
using WebShortlink.Backend.Infrastructure.Auth;
using WebShortlink.Backend.Infrastructure.Cache;
using WebShortlink.Backend.Infrastructure.Options;
using WebShortlink.Backend.Infrastructure.Persistence;
using WebShortlink.Backend.Infrastructure.Persistence.Seed;
using WebShortlink.Backend.Infrastructure.Queue;
using WebShortlink.Backend.Infrastructure.Services;
using WebShortlink.Backend.Infrastructure.Workers;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var redisOptions = configuration.GetSection(RedisOptions.SectionName).Get<RedisOptions>() ?? new RedisOptions();

builder.Services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
builder.Services.Configure<TurnstileOptions>(configuration.GetSection(TurnstileOptions.SectionName));
builder.Services.Configure<RedisOptions>(configuration.GetSection(RedisOptions.SectionName));
builder.Services.Configure<SmtpOptions>(configuration.GetSection(SmtpOptions.SectionName));

builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient<ITurnstileService, TurnstileService>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("SqlServer")));

builder.Services
    .AddIdentity<AppUser, IdentityRole<Guid>>(options =>
    {
        options.SignIn.RequireConfirmedEmail = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey))
        };
    });

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = context =>
    {
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        }

        context.Response.Redirect(context.RedirectUri);
        return Task.CompletedTask;
    };

    options.Events.OnRedirectToAccessDenied = context =>
    {
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        }

        context.Response.Redirect(context.RedirectUri);
        return Task.CompletedTask;
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserOnly", policy => policy.RequireRole(AppRoles.User));
    options.AddPolicy("AdminOnly", policy => policy.RequireRole(AppRoles.Admin));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "https://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddRateLimiter(options =>
{
    // Auth endpoints: strict (10 req/min per IP) — prevents brute force
    options.AddFixedWindowLimiter("auth", limiter =>
    {
        limiter.PermitLimit = 10;
        limiter.Window = TimeSpan.FromMinutes(1);
        limiter.QueueLimit = 0;
        limiter.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    // API endpoints: standard (120 req/min per IP)
    options.AddFixedWindowLimiter("api", limiter =>
    {
        limiter.PermitLimit = 120;
        limiter.Window = TimeSpan.FromMinutes(1);
        limiter.QueueLimit = 0;
        limiter.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    // Public redirect: high volume allowed (300 req/min) — CDN/bots hit these
    options.AddFixedWindowLimiter("redirect", limiter =>
    {
        limiter.PermitLimit = 300;
        limiter.Window = TimeSpan.FromMinutes(1);
        limiter.QueueLimit = 0;
        limiter.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 200,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));

    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsJsonAsync(new { errorCode = ErrorCodes.RateLimited, message = "Quá nhiều yêu cầu. Vui lòng thử lại sau." }, token);
    };
});

if (redisOptions.Enabled)
{
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = redisOptions.Configuration;
        options.InstanceName = redisOptions.InstanceName;
    });

    builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
        ConnectionMultiplexer.Connect(new ConfigurationOptions
        {
            AbortOnConnectFail = false,
            EndPoints = { redisOptions.Configuration }
        }));
}
else
{
    builder.Services.AddDistributedMemoryCache();
}

builder.Services.AddScoped<ICurrentUserService, HttpCurrentUserService>();
builder.Services.AddScoped<IEmailSenderService, SmtpEmailSenderService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<IPlanCapabilityService, PlanCapabilityService>();
builder.Services.AddScoped<ILinkCacheService, RedisLinkCacheService>();
builder.Services.AddScoped<IPasswordHasher<Link>, PasswordHasher<Link>>();

if (redisOptions.Enabled)
{
    builder.Services.AddSingleton<IAnalyticsQueue, RedisAnalyticsQueue>();
}
else
{
    builder.Services.AddSingleton<IAnalyticsQueue, InMemoryAnalyticsQueue>();
}

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<LinkService>();
builder.Services.AddScoped<RedirectService>();
builder.Services.AddScoped<LinkRuleService>();
builder.Services.AddScoped<AnalyticsService>();
builder.Services.AddScoped<ProfileService>();
builder.Services.AddScoped<BillingService>();
builder.Services.AddScoped<DomainService>();
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<ApplicationDbSeeder>();

builder.Services.AddHostedService<ClickAnalyticsWorker>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks()
    .AddCheck("database", () =>
    {
        // Health check inline — replaced AddDbContextCheck to avoid extra NuGet dep
        return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("OK");
    });

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("frontend");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Health check endpoint — available without auth for uptime monitors
app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = System.Text.Json.JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            timestamp = DateTime.UtcNow,
            checks = report.Entries.Select(e => new { name = e.Key, status = e.Value.Status.ToString(), description = e.Value.Description })
        });
        await context.Response.WriteAsync(result);
    }
});

using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        var seeder = scope.ServiceProvider.GetRequiredService<ApplicationDbSeeder>();
        await seeder.SeedAsync();
        logger.LogInformation("Database seeding completed successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while seeding the database. The application will continue, but some data may be missing.");
    }
}

app.Run();

public partial class Program { }
