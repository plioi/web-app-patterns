using ContactList.Contracts;
using ContactList.Server.Model;
using FluentValidation;

namespace ContactList.Server.Features;

public class AddContactCommandValidator : AbstractValidator<AddContactCommand>
{
    readonly Database _database;

    public AddContactCommandValidator(Database database)
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

    bool BeUniqueEmail(string? email)
        => !_database.Contact.Any(x => x.Email == email);
}
