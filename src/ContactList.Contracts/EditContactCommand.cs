using MediatR;

namespace ContactList.Contracts;

public class EditContactCommand : IRequest
{
    public Guid Id { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
    public string? PhoneNumber { get; set; }
}
