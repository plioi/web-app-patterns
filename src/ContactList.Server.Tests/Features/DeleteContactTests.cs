using ContactList.Contracts;
using ContactList.Server.Model;

namespace ContactList.Server.Tests.Features;

class DeleteContactTests
{
    public async Task ShouldDeleteContactById()
    {
        var contactToDelete = await AddSampleContactAsync();
        var contactToPreserve = await AddSampleContactAsync();

        var countBefore = await CountAsync<Contact>();

        await SendAsync(new DeleteContactCommand
        {
            Id = contactToDelete.Id
        });

        var countAfter = await CountAsync<Contact>();
        countAfter.ShouldBe(countBefore - 1);

        var deletedContact = await FindAsync<Contact>(contactToDelete.Id);
        deletedContact.ShouldBeNull();

        var remainingContact = await FindAsync<Contact>(contactToPreserve.Id);
        remainingContact.ShouldNotBeNull();
    }
}
