using ContactList.Contracts;
using ContactList.Server.Model;

namespace ContactList.Server.Tests.Features;

class AddContactTests
{
    public void ShouldRequireMinimumFields()
    {
        new AddContactCommand()
            .ShouldNotValidate(
                "'Email' must not be empty.",
                "'Name' must not be empty.");
    }

    public void ShouldRequireValidEmailAddress()
    {
        var command = new AddContactCommand
        {
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
        var preexistingContact = await AddSampleContactAsync();

        var command = new AddContactCommand
        {
            Email = SampleEmail(),
            Name = SampleName(),
            PhoneNumber = SamplePhoneNumber()
        };

        command.ShouldValidate();

        command.Email = preexistingContact.Email;

        command.ShouldNotValidate($"'{command.Email}' is already in your contacts.");
    }

    public async Task ShouldAddNewContact()
    {
        var email = SampleEmail();
        var phoneNumber = SamplePhoneNumber();
        var name = SampleName();

        var response = await SendAsync(new AddContactCommand
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
