using ContactList.Server.Model;

namespace ContactList.Server.Tests.Model;

class DatabaseTests
{
    public async Task ShouldRollBackOnFailure()
    {
        var contact = new Contact("email@example.com", "First Last", "555-123-4567");

        var countBefore = await CountAsync<Contact>();

        var failingTransaction = async () =>
        {
            await TransactionAsync(database =>
            {
                database.Contact.Add(contact);
                database.SaveChanges();

                var intermediateCount = database.Contact.Count();
                intermediateCount.ShouldBe(countBefore + 1);

                throw new Exception("This is expected to cause a rollback.");
            });
        };

        (await failingTransaction.ThrowsAsync<Exception>())
            .Message.ShouldBe("This is expected to cause a rollback.");

        var countAfter = await CountAsync<Contact>();

        countAfter.ShouldBe(countBefore);

        var loaded = await FindAsync<Contact>(contact.Id);
        loaded.ShouldBeNull();
    }
}
