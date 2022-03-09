using AutoMapper;
using ContactList.Contracts;
using ContactList.Server.Model;
using MediatR;

namespace ContactList.Server.Features;

class GetContactsQueryHandler : RequestHandler<GetContactsQuery, ContactViewModel[]>
{
    readonly Database _database;
    readonly IMapper _mapper;

    public GetContactsQueryHandler(Database database, IMapper mapper)
    {
        _database = database;
        _mapper = mapper;
    }

    protected override ContactViewModel[] Handle(GetContactsQuery request)
    {
        return _database.Contact
            .OrderBy(x => x.Name)
            .Select(_mapper.Map<ContactViewModel>)
            .ToArray();
    }
}
