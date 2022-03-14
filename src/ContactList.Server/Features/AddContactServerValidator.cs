using ContactList.Contracts;
using ContactList.Server.Model;
using FluentValidation;

namespace ContactList.Server.Features;

public class AddContactServerValidator : AddContactClientValidator
{
    readonly Database _database;

    public AddContactServerValidator(Database database)
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
