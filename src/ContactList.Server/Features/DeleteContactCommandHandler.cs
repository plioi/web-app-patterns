using ContactList.Contracts;
using ContactList.Server.Model;
using MediatR;

namespace ContactList.Server.Features;

class DeleteContactCommandHandler : RequestHandler<DeleteContactCommand>
{
    readonly Database _database;

    public DeleteContactCommandHandler(Database database)
    {
        _database = database;
    }

    protected override void Handle(DeleteContactCommand message)
    {
        var contact = _database.Contact.Single(x => x.Id == message.Id);

        _database.Contact.Remove(contact);
    }
}
