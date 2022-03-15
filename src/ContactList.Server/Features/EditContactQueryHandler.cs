using AutoMapper;
using ContactList.Contracts;
using ContactList.Server.Model;

namespace ContactList.Server.Features;

class EditContactQueryHandler : IFeature
{
    public void Enlist(WebApplication app)
        => app.MapGet("/api/contacts/edit", View);

    static EditContactCommand View(Guid id, Database database, IMapper mapper)
    {
        return mapper.Map<EditContactCommand>(database.Contact.Find(id));
    }
}
