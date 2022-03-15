using ContactList.Contracts;
using ContactList.Server.Model;

namespace ContactList.Server.Tests.Features;

class AddContactTests
{
    public async Task ShouldRequireMinimumFields()
    {
        await new AddContactCommand()
            .ShouldNotValidateAsync(
                "'Email' must not be empty.",
                "'Name' must not be empty.");
    }

    public async Task ShouldRequireValidEmailAddress()
    {
        var command = new AddContactCommand
        {
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
        var preexistingContact = await AddSampleContactAsync();

        var command = new AddContactCommand
        {
            Email = SampleEmail(),
            Name = SampleName(),
            PhoneNumber = SamplePhoneNumber()
        };

        await command.ShouldValidateAsync();

        command.Email = preexistingContact.Email;

        await command.ShouldNotValidateAsync($"'{command.Email}' is already in your contacts.");
    }

    public async Task ShouldAddNewContact()
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

        var actual = await FindAsync<Contact>(response.ContactId);

        actual.ShouldMatch(new Contact(email, name, phoneNumber)
        {
            Id = response.ContactId
        });
    }
}
