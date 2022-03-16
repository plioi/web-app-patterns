using AutoMapper;
using ContactList.Contracts;
using ContactList.Server.Model;

namespace ContactList.Server.Features;

class EditContactQueryHandler
{
    readonly Database _database;
    readonly IMapper _mapper;

    public EditContactQueryHandler(Database database, IMapper mapper)
    {
        _database = database;
        _mapper = mapper;
    }

    public EditContactCommand Handle(EditContactQuery message)
    {
        var contact = _database.Contact.Find(message.Id);

        return _mapper.Map<EditContactCommand>(contact);
    }
}
