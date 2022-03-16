using AutoMapper;
using ContactList.Contracts;
using ContactList.Server.Model;

namespace ContactList.Server.Features;

class EditContactQueryHandler
{
    public static EditContactCommand Handle(Guid id, Database database, IMapper mapper)
    {
        var contact = database.Contact.Find(id);

        return mapper.Map<EditContactCommand>(contact);
    }
}
