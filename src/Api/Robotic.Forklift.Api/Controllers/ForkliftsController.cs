using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Robotic.Forklift.Application.Dtos;
using Robotic.Forklift.Application.Forklifts.Commands;
using Robotic.Forklift.Application.Forklifts.Queries;

namespace Robotic.Forklift.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ForkliftsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ForkliftsController(IMediator mediator) { _mediator = mediator; }


        [HttpGet]
        public async Task<ActionResult<PagedResult<ForkliftDto>>> List([FromQuery] int page = 1, [FromQuery] int size = 20,
        [FromQuery] string? sortBy = null, [FromQuery] string sortDir = "asc")
        { 
            return await _mediator.Send(new GetForkliftsAllQuery(new PageQuery(page, size, sortBy, sortDir)));
        }


        [HttpPost("import")]
        [RequestSizeLimit(10_000_000)]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ImportResultDto>> Import(IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("file is required");
            using var sr = new StreamReader(file.OpenReadStream());
            var text = await sr.ReadToEndAsync();
            return await _mediator.Send(new ImportForkliftsCommand(file.FileName, text));
        }
    }
}