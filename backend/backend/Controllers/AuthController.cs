using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly AppDataStore _store;
    private readonly DemoTimeProvider _clock;

    public AuthController(AppDataStore store, DemoTimeProvider clock)
    {
        _store = store;
        _clock = clock;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public ActionResult<AuthResult> Register(RegisterRequest request)
    {
        try
        {
            return Ok(_store.Register(request.FullName, request.Email, request.Password, _clock.UtcNow));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public ActionResult<AuthResult> Login(LoginRequest request)
    {
        try
        {
            return Ok(_store.Login(request.Email, request.Password, _clock.UtcNow));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
    {
        return Ok(_store.GetCurrentUserProfile(User));
    }
}

public sealed record RegisterRequest(string FullName, string Email, string Password);

public sealed record LoginRequest(string Email, string Password);
