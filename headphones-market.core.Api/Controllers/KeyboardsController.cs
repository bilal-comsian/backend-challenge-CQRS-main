using Microsoft.AspNetCore.Mvc;
using MediatR;
using headphones_market.core.Api.Model;
using headphones_market.core.Api.Features.Keyboards.Queries;
using headphones_market.core.Api.Features.Keyboards.Commands;

namespace headphones_market.core.Api.Endpoints;

[Route("[controller]")]
[ApiController]
public class KeyboardsController : ControllerBase
{
    private readonly IMediator _mediator;

    public KeyboardsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<Keyboard>>> Get()
    {
        var items = await _mediator.Send(new GetAllKeyboardsQuery());
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Keyboard>> Get([FromRoute] int id)
    {
        var item = await _mediator.Send(new GetKeyboardByIdQuery(id));
        if (item is null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<Keyboard>> Create([FromBody] Keyboard create)
    {
        var created = await _mediator.Send(new CreateKeyboardCommand(create));
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] Keyboard update)
    {
        if (id != update.Id) return BadRequest("Id in route must match Id in body.");
        var ok = await _mediator.Send(new UpdateKeyboardCommand(id, update));
        if (!ok) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var ok = await _mediator.Send(new DeleteKeyboardCommand(id));
        if (!ok) return NotFound();
        return NoContent();
    }
}