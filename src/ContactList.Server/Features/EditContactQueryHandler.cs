using AutoMapper;
using ContactList.Contracts;
using ContactList.Server.Model;
using MediatR;

namespace ContactList.Server.Features;

class EditContactQueryHandler : RequestHandler<EditContactQuery, EditContactCommand>
{
    readonly Database _database;
    readonly IMapper _mapper;

    public EditContactQueryHandler(Database database, IMapper mapper)
    {
        _database = database;
        _mapper = mapper;
    }

    protected override EditContactCommand Handle(EditContactQuery message)
    {
        var contact = _database.Contact.Find(message.Id);

        return _mapper.Map<EditContactCommand>(contact);
    }
}
