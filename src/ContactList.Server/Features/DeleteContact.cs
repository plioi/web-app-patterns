using ContactList.Contracts;
using ContactList.Server.Model;

namespace ContactList.Server.Features;

class DeleteContact : IFeature
{
    public void Enlist(WebApplication app)
        => app.MapPost("/api/contacts/delete", Delete);

    static void Delete(DeleteContactCommand command, Database database)
    {
        var contact = database.Contact.Single(x => x.Id == command.Id);

        database.Contact.Remove(contact);
    }
}
