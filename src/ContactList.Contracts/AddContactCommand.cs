using MediatR;

namespace ContactList.Contracts;

public class AddContactCommand : IRequest<AddContactResponse>
{
    public string? Email { get; set; }
    public string? Name { get; set; }
    public string? PhoneNumber { get; set; }
}
