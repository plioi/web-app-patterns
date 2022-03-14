using MediatR;
using Microsoft.AspNetCore.Mvc;
using ContactList.Contracts;

namespace ContactList.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactsController : ControllerBase
{
    readonly IMediator _mediator;

    public ContactsController(IMediator mediator)
        => _mediator = mediator;

    [HttpPost("Add")]
    public async Task<AddContactResponse> Add(AddContactCommand command)
        => await _mediator.Send(command);

    [HttpGet("Edit")]
    public async Task<EditContactCommand> Edit([FromQuery] EditContactQuery query)
        => await _mediator.Send(query);

    [HttpPost("Edit")]
    public async Task Edit(EditContactCommand command)
        => await _mediator.Send(command);

    [HttpPost("Delete")]
    public async Task Delete(DeleteContactCommand command)
        => await _mediator.Send(command);
}
