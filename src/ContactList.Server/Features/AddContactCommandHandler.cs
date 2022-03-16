using AutoMapper;
using ContactList.Contracts;
using ContactList.Server.Model;
using FluentValidation;

namespace ContactList.Server.Features;

class AddContactCommandHandler
{
    public static async Task<IResult> Handle(AddContactCommand command, IValidator<AddContactCommand> validator, Database database, IMapper mapper)
    {
        var validationResult = await validator.ValidateAsync(command);
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.ErrorMessage).ToArray()
                ));

        var contact = mapper.Map<Contact>(command);

        database.Contact.Add(contact);

        var response = new AddContactResponse
        {
            ContactId = contact.Id
        };

        return Results.Ok(response);
    }
}
