using AutoMapper;
using ContactList.Contracts;
using ContactList.Server.Model;
using FluentValidation;

namespace ContactList.Server.Features;

class AddContact : IFeature
{
    public void Enlist(WebApplication app)
        => app.MapPost("/api/contacts/add", Add);

    static async Task<IResult> Add(AddContactCommand command, IValidator<AddContactCommand> validator, Database database, IMapper mapper)
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
