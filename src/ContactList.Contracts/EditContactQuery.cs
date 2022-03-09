using MediatR;

namespace ContactList.Contracts;

public class EditContactQuery : IRequest<EditContactCommand>
{
    public Guid Id { get; set; }
}
