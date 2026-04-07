using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Authorize]
[Route("api/me")]
public sealed class ProfileController : ControllerBase
{
    private readonly AppDataStore _store;

    public ProfileController(AppDataStore store)
    {
        _store = store;
    }

    [HttpGet("profile")]
    public IActionResult GetProfile()
    {
        return Ok(_store.GetCurrentUserProfile(User));
    }

    [HttpPut("profile")]
    public IActionResult UpdateProfile(UpdateProfileRequest request)
    {
        try
        {
            return Ok(_store.UpdateProfile(User, request.FullName));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("password")]
    public IActionResult ChangePassword(ChangePasswordRequest request)
    {
        try
        {
            return Ok(_store.ChangePassword(User, request.CurrentPassword, request.NewPassword));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("usage")]
    public IActionResult Usage()
    {
        return Ok(_store.GetCurrentUserProfile(User));
    }
}

public sealed record UpdateProfileRequest(string FullName);

public sealed record ChangePasswordRequest(string CurrentPassword, string NewPassword);
