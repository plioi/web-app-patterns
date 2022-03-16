using ContactList.Contracts;
using ContactList.Server.Model;

namespace ContactList.Server.Features;

class DeleteContactCommandHandler
{
    readonly Database _database;

    public DeleteContactCommandHandler(Database database)
    {
        _database = database;
    }

    public void Execute(DeleteContactCommand message)
    {
        var contact = _database.Contact.Single(x => x.Id == message.Id);

        _database.Contact.Remove(contact);
    }
}
