using AutoMapper;
using ContactList.Contracts;
using ContactList.Server.Model;

namespace ContactList.Server.Features;

class AddContactCommandHandler
{
    readonly Database _database;
    readonly IMapper _mapper;

    public AddContactCommandHandler(Database database, IMapper mapper)
    {
        _database = database;
        _mapper = mapper;
    }

    public AddContactResponse Handle(AddContactCommand message)
    {
        var contact = _mapper.Map<Contact>(message);

        _database.Contact.Add(contact);

        return new AddContactResponse
        {
            ContactId = contact.Id
        };
    }
}
