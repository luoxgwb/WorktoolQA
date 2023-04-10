using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace ReprintTech.Universe.ChatApi.Extensions
{
    public static class StorageExtensions
    {
        public static IServiceCollection AddStorage<T>(this IServiceCollection services, string connetcionStr) where T : DbContext
        {
            services.AddDbContext<T>(options =>
            options.UseMySql(connetcionStr, MySqlServerVersion.LatestSupportedServerVersion));

            return services;
        }
    }
}
