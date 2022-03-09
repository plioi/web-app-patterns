using AutoMapper;
using ContactList.Contracts;
using ContactList.Server.Model;

namespace ContactList.Server.Features;

class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Contact, ContactViewModel>();

        CreateMap<Contact, EditContactCommand>();
        CreateMap<EditContactCommand, Contact>();

        CreateMap<AddContactCommand, Contact>()
            .ForMember(x => x.Id, options => options.Ignore());
    }
}
