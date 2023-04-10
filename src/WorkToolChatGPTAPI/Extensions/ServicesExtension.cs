
using System.Reflection;

using Utility.Dependencies;
using Utility.Extensions;

namespace ReprintTech.Universe.ChatApi.Extensions
{
    public static class ServicesExtension
    {
        public static IServiceCollection AddServices(this IServiceCollection services, params Type[] currentType)
        {
            var types = currentType.SelectMany(t => t.Assembly.GetTypes());

            var effectiveTypes = types.Where(x => x.GetCustomAttributes().Any(a => a is ServiceAttribute) || (x.IsPublic && typeof(IScoped).IsAssignableFrom(x) && x.IsClass && !x.IsInterface && !x.IsAbstract));

            foreach (var type in effectiveTypes)
            {
                var interfaces = type.GetInterfaces().Where(x => x != typeof(IScoped));

                services.AddScoped(interfaces.Last(), type);
            }

            return services;
        }
    }
}
