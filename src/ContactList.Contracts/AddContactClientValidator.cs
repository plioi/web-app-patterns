using FluentValidation;

namespace ContactList.Contracts;

public class AddContactClientValidator : AbstractValidator<AddContactCommand>
{
    public AddContactClientValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().Length(1, 255);
        RuleFor(x => x.Name).NotEmpty().Length(1, 100);
        RuleFor(x => x.PhoneNumber).Length(1, 50);
    }
}
