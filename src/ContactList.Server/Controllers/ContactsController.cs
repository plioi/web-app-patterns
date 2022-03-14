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
}
