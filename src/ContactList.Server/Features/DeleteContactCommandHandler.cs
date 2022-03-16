using ContactList.Contracts;
using ContactList.Server.Model;

namespace ContactList.Server.Features;

class DeleteContactCommandHandler
{
    public static void Handle(DeleteContactCommand command, Database database)
    {
        var contact = database.Contact.Single(x => x.Id == command.Id);

        database.Contact.Remove(contact);
    }
}
