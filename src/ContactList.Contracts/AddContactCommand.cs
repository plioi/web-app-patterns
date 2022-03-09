using System.ComponentModel.DataAnnotations;
using MediatR;

namespace ContactList.Contracts;

public class AddContactCommand : IRequest<AddContactResponse>
{
    public string? Email { get; set; }

    public string? Name { get; set; }

    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }
}
