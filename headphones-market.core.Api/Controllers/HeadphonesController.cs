using Microsoft.AspNetCore.Mvc;
using MediatR;
using headphones_market.core.Api.Model;
using headphones_market.core.Api.Features.Headphones.Queries;
using headphones_market.core.Api.Features.Headphones.Commands;

namespace headphones_market.core.Api.Endpoints;

[Route("[controller]")]
[ApiController]
public class HeadphonesController : ControllerBase
{
    private readonly IMediator _mediator;

    public HeadphonesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<Headphone>>> Get()
    {
        var items = await _mediator.Send(new GetAllHeadphonesQuery());
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Headphone>> Get([FromRoute] int id)
    {
        var item = await _mediator.Send(new GetHeadphoneByIdQuery(id));
        if (item is null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<Headphone>> Create([FromBody] Headphone create)
    {
        var created = await _mediator.Send(new CreateHeadphoneCommand(create));
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] Headphone update)
    {
        if (id != update.Id) return BadRequest("Id in route must match Id in body.");
        var ok = await _mediator.Send(new UpdateHeadphoneCommand(id, update));
        if (!ok) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var ok = await _mediator.Send(new DeleteHeadphoneCommand(id));
        if (!ok) return NotFound();
        return NoContent();
    }
}