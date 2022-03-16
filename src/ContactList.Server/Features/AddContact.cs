using AutoMapper;
using ContactList.Contracts;
using ContactList.Server.Model;
using FluentValidation;

namespace ContactList.Server.Features;

class AddContact : IFeature
{
    public void Enlist(WebApplication app)
        => app.MapPost("/api/contacts/add", Add);

    static async Task<IResult> Add(AddContactCommand command, Validator validator, Database database, IMapper mapper)
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

    class Validator : AddContactClientValidator
    {
        readonly Database _database;

        public Validator(Database database)
        {
            _database = database;

            RuleFor(x => x.Email)
                .Must(BeUniqueEmail)
                .When(x => x.Email != null)
                .WithMessage("'{PropertyValue}' is already in your contacts.");
        }

        bool BeUniqueEmail(string? email)
            => !_database.Contact.Any(x => x.Email == email);
    }
}
