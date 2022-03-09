using AutoMapper;
using ContactList.Contracts;
using ContactList.Server.Model;
using MediatR;

namespace ContactList.Server.Features;

class EditContactCommandHandler : RequestHandler<EditContactCommand>
{
    readonly Database _database;
    readonly IMapper _mapper;

    public EditContactCommandHandler(Database database, IMapper mapper)
    {
        _database = database;
        _mapper = mapper;
    }

    protected override void Handle(EditContactCommand message)
    {
        var contact = _database.Contact.Single(x => x.Id == message.Id);

        _mapper.Map(message, contact);
    }
}