using ContactList.Server.Model;

namespace ContactList.Server.Tests.Model;

class ContactTests
{
    public async Task ShouldPersist()
    {
        var contact = new Contact("email@example.com", "First Last", "555-123-4567");

        contact.Id.ShouldBe(Guid.Empty);

        await TransactionAsync(async database => await database.Contact.AddAsync(contact));

        contact.Id.ShouldNotBe(Guid.Empty);

        var loaded = await FindAsync<Contact>(contact.Id);
        loaded.ShouldMatch(contact);
    }
}
