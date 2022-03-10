using ContactList.Contracts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ContactList.Server.Controllers;

public class ContactsController : BaseController
{
    readonly IMediator _mediator;

    public ContactsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<ActionResult> Index(GetContactsQuery query)
    {
        var model = await _mediator.Send(query);

        return View(model);
    }

    public ActionResult Add()
    {
        return View();
    }

    [HttpPost]
    public async Task<ActionResult> Add(AddContactCommand command)
    {
        if (ModelState.IsValid)
        {
            await _mediator.Send(command);

            SuccessMessage($"{command.Name} has been added.");

            return RedirectToAction("Index");
        }

        return View(command);
    }

    public async Task<ActionResult> Edit(EditContactQuery query)
    {
        var model = await _mediator.Send(query);

        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> Edit(EditContactCommand command)
    {
        if (ModelState.IsValid)
        {
            await _mediator.Send(command);

            SuccessMessage($"{command.Name} has been updated.");

            return RedirectToAction("Index");
        }

        return View(command);
    }

    [HttpPost]
    public async Task<ActionResult> Delete(DeleteContactCommand command)
    {
        if (ModelState.IsValid)
        {
            await _mediator.Send(command);

            SuccessMessage($"{command.Name} has been deleted.");

            return AjaxRedirect(Url.Action("Index")!);
        }

        return BadRequest();
    }
}
