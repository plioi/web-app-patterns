using AutoMapper;
using ContactList.Contracts;
using ContactList.Server.Model;

namespace ContactList.Server.Features;

class GetContactsQueryHandler
{
    public static ContactViewModel[] Handle(Database database, IMapper mapper)
    {
        return database.Contact
            .OrderBy(x => x.Name)
            .Select(mapper.Map<ContactViewModel>)
            .ToArray();
    }
}
