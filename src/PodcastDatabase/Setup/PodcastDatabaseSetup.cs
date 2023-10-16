using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PodcastDatabase.Contexts;
using PodcastDatabase.Repositories;

namespace PodcastDatabase.Setup;

public static class PodcastDatabaseSetup
{
    public static IServiceCollection ConfigurePodcastDatabase(this IServiceCollection services)
    {
        services.TryAddScoped<IEpisodeRepository, EpisodeRepository>();
        services.TryAddScoped<IPodcastRepository, PodcastRepository>();
        services.TryAddScoped<ISeasonRepository, SeasonRepository>();
        services.TryAddScoped<PodcastDbContext>();
        
        return services;
    }
}