using FluentValidation;

namespace ContactList.Contracts;

public class EditContactClientValidator : AbstractValidator<EditContactCommand>
{
    public EditContactClientValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().Length(1, 255);
        RuleFor(x => x.Name).NotEmpty().Length(1, 100);
        RuleFor(x => x.PhoneNumber).Length(1, 50);
    }
}
