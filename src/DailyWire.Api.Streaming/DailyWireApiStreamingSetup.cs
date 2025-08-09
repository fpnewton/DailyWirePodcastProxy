using DailyWire.Api.Streaming.Configuration;
using DailyWire.Api.Streaming.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DailyWire.Api.Streaming;

public static class DailyWireApiStreamingSetup
{
    public static IServiceCollection ConfigureDailyWireStreamingApi(this IServiceCollection services)
    {
        services.TryAddSingleton(provider =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var baseUrl = configuration.GetConnectionString("StreamApi");

            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new KeyNotFoundException("ConnectionString 'StreamApi' is not valid.");
            }

            return new DwStreamingConfiguration
            {
                BaseUrl = baseUrl
            };
        });
        
        services.AddHttpClient(DwStreamingConstants.HttpClientStreamProxy, (serviceProvider, client) =>
        {
            var config = serviceProvider.GetRequiredService<DwStreamingConfiguration>();
            
            client.BaseAddress = new Uri(config.BaseUrl);
            client.Timeout = Timeout.InfiniteTimeSpan;
        });
        
        services.TryAddScoped<IDailyWireStreamApi, DailyWireStreamApi>();
        
        return services;
    }
}