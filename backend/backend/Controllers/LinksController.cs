using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Authorize]
[Route("api/links")]
public sealed class LinksController : ControllerBase
{
    private readonly AppDataStore _store;
    private readonly DemoTimeProvider _clock;

    public LinksController(AppDataStore store, DemoTimeProvider clock)
    {
        _store = store;
        _clock = clock;
    }

    [HttpGet]
    public IActionResult List()
    {
        return Ok(_store.GetUserLinks(User));
    }

    [HttpPost]
    public IActionResult Create(CreateLinkRequest request)
    {
        try
        {
            return Ok(_store.CreateLink(
                User,
                new CreateLinkInput(request.OriginalUrl, request.CustomSlug, request.Tag, request.ExpiredAtUtc, request.ClickLimit, request.Password),
                _clock.UtcNow));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id:guid}")]
    public IActionResult Detail(Guid id)
    {
        try
        {
            return Ok(_store.GetLinkById(User, id));
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public IActionResult Update(Guid id, UpdateLinkRequest request)
    {
        try
        {
            return Ok(_store.UpdateLink(
                User,
                id,
                new UpdateLinkInput(request.OriginalUrl, request.Tag, request.ExpiredAtUtc, request.ClickLimit, request.Password),
                _clock.UtcNow));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id:guid}/pause")]
    public IActionResult Pause(Guid id)
    {
        try
        {
            return Ok(_store.SetLinkStatus(User, id, "Paused", _clock.UtcNow));
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPatch("{id:guid}/resume")]
    public IActionResult Resume(Guid id)
    {
        try
        {
            return Ok(_store.SetLinkStatus(User, id, "Active", _clock.UtcNow));
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        try
        {
            return Ok(_store.DeleteLink(User, id, _clock.UtcNow));
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}

public sealed record CreateLinkRequest(string OriginalUrl, string? CustomSlug, string? Tag, DateTime? ExpiredAtUtc, int? ClickLimit, string? Password);

public sealed record UpdateLinkRequest(string OriginalUrl, string? Tag, DateTime? ExpiredAtUtc, int? ClickLimit, string? Password);
