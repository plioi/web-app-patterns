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

    public void ShouldRequireMinimumFields()
    {
        new EditContactCommand()
            .ShouldNotValidate(
                "'Email' must not be empty.",
                "'Name' must not be empty.");
    }

    public void ShouldRequireValidEmailAddress()
    {
        var command = new EditContactCommand
        {
            Id = Guid.NewGuid(),
            Name = SampleName(),
            PhoneNumber = SamplePhoneNumber()
        };

        command.ShouldNotValidate("'Email' must not be empty.");

        command.Email = "test@example.com";
        command.ShouldValidate();

        command.Email = "test at example dot com";
        command.ShouldNotValidate("'Email' is not a valid email address.");
    }

    public async Task ShouldRequireUniqueEmail()
    {
        var contactToEdit = await AddSampleContactAsync();
        var preexistingContact = await AddSampleContactAsync();

        var command = await GetAsync("/api/contacts/edit", new EditContactQuery
        {
            Id = contactToEdit.Id
        });

        command.ShouldValidate();

        command.Email = SampleEmail();
        command.ShouldValidate();

        command.Email = preexistingContact.Email;
        command.ShouldNotValidate($"'{command.Email}' is already in your contacts.");
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
