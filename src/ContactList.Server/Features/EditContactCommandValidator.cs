using ContactList.Contracts;
using ContactList.Server.Model;
using FluentValidation;

namespace ContactList.Server.Features;

public class EditContactCommandValidator : AbstractValidator<EditContactCommand>
{
    readonly Database _database;

    public EditContactCommandValidator(Database database)
    {
        _database = database;
        RuleFor(x => x.Email).NotEmpty().EmailAddress().Length(1, 255);
        RuleFor(x => x.Name).NotEmpty().Length(1, 100);
        RuleFor(x => x.PhoneNumber).Length(1, 50);

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
