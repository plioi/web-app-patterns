using AutoMapper;
using ContactList.Contracts;
using ContactList.Server.Model;

namespace ContactList.Server.Features;

class ListContacts : IFeature
{
    public void Enlist(WebApplication app)
        => app.MapGet("/api/contacts", GetContacts);

    static ContactViewModel[] GetContacts(Database database, IMapper mapper) =>
        database.Contact.OrderBy(x => x.Name)
            .Select(mapper.Map<ContactViewModel>)
            .ToArray();
}
