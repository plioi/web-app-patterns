using AutoMapper;
using ContactList.Contracts;
using ContactList.Server.Model;

namespace ContactList.Server.Features;

class GetContactsQueryHandler
{
    readonly Database _database;
    readonly IMapper _mapper;

    public GetContactsQueryHandler(Database database, IMapper mapper)
    {
        _database = database;
        _mapper = mapper;
    }

    public ContactViewModel[] Handle(GetContactsQuery message)
    {
        return _database.Contact
            .OrderBy(x => x.Name)
            .Select(_mapper.Map<ContactViewModel>)
            .ToArray();
    }
}
