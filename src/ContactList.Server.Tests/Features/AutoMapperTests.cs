using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace ContactList.Server.Tests.Features;

class AutoMapperTests
{
    public async Task AllMappingProfilesShouldBeValid() =>
        await ExecuteScopeAsync(services => services
            .GetRequiredService<IMapper>()
            .ConfigurationProvider
            .AssertConfigurationIsValid());
}
