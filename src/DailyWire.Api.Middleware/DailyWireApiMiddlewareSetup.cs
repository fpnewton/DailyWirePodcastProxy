using DailyWire.Api.Middleware.Configuration;
using DailyWire.Api.Middleware.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DailyWire.Api.Middleware;

public static class DailyWireApiMiddlewareSetup
{
    public static IServiceCollection ConfigureDailyWireMiddlewareApi(this IServiceCollection services)
    {
        services.TryAddScoped<IDailyWireMiddlewareApi, DailyWireMiddlewareApi>();

        services.TryAddSingleton(provider =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var baseUrl = configuration.GetConnectionString("MiddlewareApi");

            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new KeyNotFoundException("ConnectionString 'MiddlewareApi' is not valid.");
            }

            return new DwMiddlewareConfiguration
            {
                BaseUrl = baseUrl
            };
        });

        return services;
    }
}