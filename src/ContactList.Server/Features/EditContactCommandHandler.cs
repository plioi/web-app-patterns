using AutoMapper;
using ContactList.Contracts;
using ContactList.Server.Model;
using FluentValidation;

namespace ContactList.Server.Features;

class EditContactCommandHandler : IFeature
{
    public void Enlist(WebApplication app)
        => app.MapPost("/api/contacts/edit", Save);

    static async Task<IResult> Save(EditContactCommand command, IValidator<EditContactCommand> validator, Database database, IMapper mapper)
    {
        return await validator.GuardAsync(command, () =>
        {
            var contact = database.Contact.Single(x => x.Id == command.Id);

            mapper.Map(command, contact);
        });
    }
}
