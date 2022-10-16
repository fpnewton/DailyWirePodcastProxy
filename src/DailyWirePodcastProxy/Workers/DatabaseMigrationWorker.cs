using Microsoft.EntityFrameworkCore;
using PodcastDatabase.Contexts;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace DailyWirePodcastProxy.Workers;

public class DatabaseMigrationWorker : BackgroundService
{
    private readonly Container _container;

    public DatabaseMigrationWorker(Container container)
    {
        _container = container;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await using var scope = ThreadScopedLifestyle.BeginScope(_container);
        await using var db = scope.GetRequiredService<PodcastDbContext>();
        
        await db.Database.MigrateAsync(cancellationToken);
    }
}