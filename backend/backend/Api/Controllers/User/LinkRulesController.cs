using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebShortlink.Backend.Application.Links;

namespace WebShortlink.Backend.Api.Controllers.User;

[ApiController]
[Authorize(Roles = Domain.Enums.AppRoles.User)]
[Route("api/user/links/{linkId:guid}/rules")]
public sealed class LinkRulesController : ControllerBase
{
    private readonly LinkRuleService _linkRuleService;

    public LinkRulesController(LinkRuleService linkRuleService)
    {
        _linkRuleService = linkRuleService;
    }

    [HttpGet]
    public async Task<IActionResult> GetRules(Guid linkId, CancellationToken cancellationToken)
        => Ok(await _linkRuleService.GetRulesAsync(linkId, cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Create(Guid linkId, [FromBody] CreateLinkRuleRequestDto request, CancellationToken cancellationToken)
        => Ok(await _linkRuleService.CreateRuleAsync(linkId, request, cancellationToken));

    [HttpDelete("{ruleId:guid}")]
    public async Task<IActionResult> Delete(Guid linkId, Guid ruleId, CancellationToken cancellationToken)
        => Ok(await _linkRuleService.DeleteRuleAsync(linkId, ruleId, cancellationToken));
}
