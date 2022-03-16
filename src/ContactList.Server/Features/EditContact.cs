using AutoMapper;
using ContactList.Contracts;
using ContactList.Server.Model;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ContactList.Server.Features;

class EditContact : IFeature
{
    public void Enlist(WebApplication app)
    {
        app.MapGet("/api/contacts/edit", View);
        app.MapPost("/api/contacts/edit", Save);
    }

    static EditContactCommand View(Guid id, Database database, IMapper mapper)
    {
        return mapper.Map<EditContactCommand>(database.Contact.Find(id));
    }

    static async Task Save(EditContactCommand command, Validator validator, Database database, IMapper mapper)
    {
        await validator.GuardAsync(command);

        var contact = database.Contact.Single(x => x.Id == command.Id);

        mapper.Map(command, contact);
    }

    class Validator : EditContactClientValidator
    {
        readonly Database _database;

        public Validator(Database database)
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
}
