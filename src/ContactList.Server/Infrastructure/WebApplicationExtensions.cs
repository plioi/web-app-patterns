using System.Reflection;
using ContactList.Server.Features;

namespace ContactList.Server.Infrastructure;

public static class WebApplicationExtensions
{
    public static void MapFeatures(this WebApplication app)
    {
        foreach (var implementationType in Assembly.GetExecutingAssembly().GetTypes())
        {
            var relevant = implementationType.IsClass &&
                           !implementationType.IsAbstract &&
                           !implementationType.IsGenericTypeDefinition;

            if (relevant)
            {
                if (implementationType.GetInterfaces().Contains(typeof(IFeature)))
                {
                    var feature = (IFeature) Activator.CreateInstance(implementationType)!;
                    feature.Enlist(app);
                }
            }
        }
    }
}
