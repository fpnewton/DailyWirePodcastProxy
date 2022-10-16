using PodcastDatabase.Contexts;
using PodcastDatabase.Repositories;
using SimpleInjector;
using SimpleInjector.Packaging;

namespace PodcastDatabase;

public class PodcastsDatabasePackage : IPackage
{
    public void RegisterServices(Container container)
    {
        container.Register<IEpisodeRepository, EpisodeRepository>();
        container.Register<IPodcastRepository, PodcastRepository>();
        container.Register<ISeasonRepository, SeasonRepository>();

        container.Register<PodcastDbContext>();
    }
}