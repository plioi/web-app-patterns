using AutoMapper;
using ContactList.Contracts;
using ContactList.Server.Model;
using FluentValidation;

namespace ContactList.Server.Features;

class EditContact : IFeature
{
    public void Enlist(WebApplication app)
    {
        app.MapGet("/api/contacts/edit", View);
        app.MapPost("/api/contacts/edit", Save);
    }

    static EditContactCommand View(Guid id, Database database, IMapper mapper)
    {
        return mapper.Map<EditContactCommand>(database.Contact.Find(id));
    }

    static async Task<IResult> Save(EditContactCommand command, IValidator<EditContactCommand> validator, Database database, IMapper mapper)
    {
        return await validator.GuardAsync(command, () =>
        {
            var contact = database.Contact.Single(x => x.Id == command.Id);

            mapper.Map(command, contact);
        });
    }
}
