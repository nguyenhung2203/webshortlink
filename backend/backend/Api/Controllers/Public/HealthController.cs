using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebShortlink.Backend.Infrastructure.Persistence;

namespace WebShortlink.Backend.Api.Controllers.Public;

[ApiController]
[AllowAnonymous]
public sealed class HealthController : ControllerBase
{
    [HttpGet("/api/health")]
    public async Task<IActionResult> Health([FromServices] ApplicationDbContext dbContext, CancellationToken cancellationToken)
    {
        var canConnect = await dbContext.Database.CanConnectAsync(cancellationToken);
        return Ok(new
        {
            status = canConnect ? "ok" : "degraded",
            database = canConnect ? "connected" : "disconnected",
            utcNow = DateTime.UtcNow
        });
    }
}
