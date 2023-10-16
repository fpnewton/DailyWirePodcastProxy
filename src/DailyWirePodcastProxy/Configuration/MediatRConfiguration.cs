namespace DailyWirePodcastProxy.Configuration;

public static class MediatRConfiguration
{
    public static WebApplicationBuilder ConfigureMediatR(this WebApplicationBuilder builder)
    {
        builder.Services.AddMediatR(config =>
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                config.Lifetime = ServiceLifetime.Scoped;
                config.RegisterServicesFromAssembly(assembly);
            }
        });

        return builder;
    }
}