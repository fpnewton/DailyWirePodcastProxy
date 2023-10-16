using Microsoft.EntityFrameworkCore;
using PodcastDatabase.Contexts;

namespace DailyWirePodcastProxy.Workers;

public class DatabaseMigrationWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public DatabaseMigrationWorker(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        await using var db = scope.ServiceProvider.GetRequiredService<PodcastDbContext>();
        
        await db.Database.MigrateAsync(cancellationToken);
    }
}