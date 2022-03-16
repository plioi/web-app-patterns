using AutoMapper;
using ContactList.Contracts;
using ContactList.Server.Model;
using FluentValidation;

namespace ContactList.Server.Features;

class EditContactCommandHandler
{
    public static async Task<IResult> Handle(EditContactCommand command, IValidator<EditContactCommand> validator, Database database, IMapper mapper)
    {
        var validationResult = await validator.ValidateAsync(command);
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.ErrorMessage).ToArray()
                ));

        var contact = database.Contact.Single(x => x.Id == command.Id);

        mapper.Map(command, contact);

        return Results.Ok();
    }
}
