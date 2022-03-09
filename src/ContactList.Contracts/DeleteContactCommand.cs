using MediatR;

namespace ContactList.Contracts;

public class DeleteContactCommand : IRequest
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
}
