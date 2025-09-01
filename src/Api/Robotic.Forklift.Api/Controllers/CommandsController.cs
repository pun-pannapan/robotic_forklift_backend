using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Robotic.Forklift.Application.Dtos;
using Robotic.Forklift.Application.Forklifts.Commands;
using Robotic.Forklift.Application.Forklifts.Queries;


namespace Robotic.Forklift.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CommandsController : ControllerBase
{
    private readonly IMediator _mediator;
    public CommandsController(IMediator mediator) { _mediator = mediator; }


    [HttpPost]
    public async Task<ActionResult<List<ParsedActionDto>>> Send([FromBody] SendCommandRequest req)
    {
        var uid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var withUser = req with { IssuedByUserId = uid };
        return await _mediator.Send(new SendCommandCommand(withUser));
    }


    [HttpGet("logs")]
    public async Task<ActionResult<PagedResult<ForkliftCommandDto>>> Logs([FromQuery] int forkliftId,
    [FromQuery] int page = 1, [FromQuery] int size = 50, [FromQuery] string? sortBy = null, [FromQuery] string sortDir = "asc")
    {
        return await _mediator.Send(new GetForkliftLogsQuery(forkliftId, new PageQuery(page, size, sortBy, sortDir)));
    }

    [HttpGet("logs/all")]
    public async Task<ActionResult<PagedResult<ForkliftCommandDto>>> LogsAll(
   [FromQuery] int page = 1, [FromQuery] int size = 50, [FromQuery] string? sortBy = null, [FromQuery] string sortDir = "asc")
    {
        return await _mediator.Send(new GetForkliftLogsAllQuery(new PageQuery(page, size, sortBy, sortDir)));
    }
}