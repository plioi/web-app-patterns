using System.Reflection;
using FluentValidation;

namespace ContactList.Server.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static void AddServerValidators(this IServiceCollection services)
        {
            foreach (var implementationType in Assembly.GetExecutingAssembly().GetTypes())
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
