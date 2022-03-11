using ContactList.Contracts;
using FluentValidation;

namespace ContactList.Client
{
    public static class ServiceCollectionExtensions
    {
        public static void AddFluentValidators(this IServiceCollection services)
        {
            foreach (var implementationType in typeof(ContractsAssemblyMarker).Assembly.GetTypes())
            {
                var relevant = implementationType.IsClass &&
                               !implementationType.IsAbstract &&
                               !implementationType.IsGenericTypeDefinition;

                if (relevant)
                {
                    var validatorServiceType = implementationType.GetInterfaces()
                        .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>));

                    if (validatorServiceType != null)
                        services.AddTransient(validatorServiceType, implementationType);
                }
            }
        }
    }
}
