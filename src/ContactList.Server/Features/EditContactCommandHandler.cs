using AutoMapper;
using ContactList.Contracts;
using ContactList.Server.Model;
using FluentValidation;

namespace ContactList.Server.Features;

class EditContactCommandHandler
{
    public static async Task<IResult> Handle(EditContactCommand command, IValidator<EditContactCommand> validator, Database database, IMapper mapper)
    {
        return await validator.GuardAsync(command, () =>
        {
            var contact = database.Contact.Single(x => x.Id == command.Id);

            mapper.Map(command, contact);
        });
    }
}
