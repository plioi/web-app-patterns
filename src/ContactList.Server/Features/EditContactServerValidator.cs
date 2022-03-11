using ContactList.Contracts;
using ContactList.Server.Model;
using FluentValidation;

namespace ContactList.Server.Features;

public class EditContactServerValidator : EditContactClientValidator
{
    readonly Database _database;

    public EditContactServerValidator(Database database)
    {
        _database = database;

        RuleFor(x => x.Email)
            .Must(BeUniqueEmail)
            .When(x => x.Email != null)
            .WithMessage("'{PropertyValue}' is already in your contacts.");
    }

    bool BeUniqueEmail(EditContactCommand command, string? email)
    {
        var existingContact = _database.Contact.SingleOrDefault(x => x.Email == email);

        return existingContact == null || existingContact.Id == command.Id;
    }
}
