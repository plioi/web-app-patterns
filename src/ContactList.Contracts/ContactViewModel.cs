namespace ContactList.Contracts;

public class ContactViewModel
{
    public ContactViewModel(string name, string email, string? phoneNumber = null)
    {
        Name = name;
        Email = email;
        PhoneNumber = phoneNumber;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string? PhoneNumber { get; set; }
}
