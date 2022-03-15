using ContactList.Contracts;
using ContactList.Server.Model;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ContactList.Server.Features;

public class EditContactServerValidator : EditContactClientValidator
{
    readonly Database _database;

    public EditContactServerValidator(Database database)
    {
        _database = database;

        RuleFor(x => x.Email)
            .MustAsync(BeUniqueEmail)
            .When(x => x.Email != null)
            .WithMessage("'{PropertyValue}' is already in your contacts.");
    }

    async Task<bool> BeUniqueEmail(EditContactCommand command, string? email, CancellationToken token)
    {
        var existingContact = await _database.Contact.SingleOrDefaultAsync(x => x.Email == email, token);

        return existingContact == null || existingContact.Id == command.Id;
    }
}
