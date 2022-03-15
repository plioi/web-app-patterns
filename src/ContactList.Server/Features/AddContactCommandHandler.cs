using AutoMapper;
using ContactList.Contracts;
using ContactList.Server.Model;
using FluentValidation;

namespace ContactList.Server.Features;

class AddContactCommandHandler
{
    public static async Task<IResult> Handle(AddContactCommand command, IValidator<AddContactCommand> validator, Database database, IMapper mapper)
    {
        return await validator.GuardAsync(command, () =>
        {
            var contact = mapper.Map<Contact>(command);

            database.Contact.Add(contact);

            return new AddContactResponse
            {
                ContactId = contact.Id
            };
        });
    }
}
