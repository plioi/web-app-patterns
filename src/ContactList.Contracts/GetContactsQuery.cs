using MediatR;

namespace ContactList.Contracts;

public class GetContactsQuery : IRequest<ContactViewModel[]> { }
