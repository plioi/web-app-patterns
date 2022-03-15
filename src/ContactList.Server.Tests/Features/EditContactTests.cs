using ContactList.Contracts;
using ContactList.Server.Model;

namespace ContactList.Server.Tests.Features;

class EditContactTests
{
    public async Task ShouldGetCurrentContactDataById()
    {
        var email = SampleEmail();
        var phoneNumber = SamplePhoneNumber();
        var name = SampleName();

        var response = await PostAsync("/api/contacts/add", new AddContactCommand
        {
            Email = email,
            Name = name,
            PhoneNumber = phoneNumber
        });

        var anotherContact = await AddSampleContactAsync();

        var selectedContactId = response.ContactId;

        var result = await GetAsync("/api/contacts/edit", new EditContactQuery
        {
            Id = selectedContactId
        });

        result.ShouldMatch(new EditContactCommand
        {
            Id = selectedContactId,
            Email = email,
            Name = name,
            PhoneNumber = phoneNumber
        });
    }

    public async Task ShouldRequireMinimumFields()
    {
        await new EditContactCommand()
            .ShouldNotValidateAsync(
                "'Email' must not be empty.",
                "'Name' must not be empty.");
    }

    public async Task ShouldRequireValidEmailAddress()
    {
        var command = new EditContactCommand
        {
            Id = Guid.NewGuid(),
            Name = SampleName(),
            PhoneNumber = SamplePhoneNumber()
        };

        await command.ShouldNotValidateAsync("'Email' must not be empty.");

        command.Email = "test@example.com";
        await command.ShouldValidateAsync();

        command.Email = "test at example dot com";
        await command.ShouldNotValidateAsync("'Email' is not a valid email address.");
    }

    public async Task ShouldRequireUniqueEmail()
    {
        var contactToEdit = await AddSampleContactAsync();
        var preexistingContact = await AddSampleContactAsync();

        var command = await GetAsync("/api/contacts/edit", new EditContactQuery
        {
            Id = contactToEdit.Id
        });

        await command.ShouldValidateAsync();

        command.Email = SampleEmail();
        await command.ShouldValidateAsync();

        command.Email = preexistingContact.Email;
        await command.ShouldNotValidateAsync($"'{command.Email}' is already in your contacts.");
    }

    public async Task ShouldSaveChangesToContact()
    {
        var selectedContact = await AddSampleContactAsync();
        var anotherContact = await AddSampleContactAsync();

        var newEmail = SampleEmail();
        var newPhoneNumber = SamplePhoneNumber();
        var newName = SampleName();

        await PostAsync("/api/contacts/edit", new EditContactCommand
        {
            Id = selectedContact.Id,
            Email = newEmail,
            Name = newName,
            PhoneNumber = newPhoneNumber
        });

        var actual = await FindAsync<Contact>(selectedContact.Id);

        actual.ShouldMatch(new Contact(newEmail, newName, newPhoneNumber)
        {
            Id = selectedContact.Id
        });
    }
}
