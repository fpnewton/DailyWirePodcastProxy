using Microsoft.EntityFrameworkCore;
using PodcastProxy.Database.Contexts;

namespace DailyWirePodcastProxy.Workers;

public class DatabaseMigrationWorker(IServiceProvider serviceProvider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        await using var db = scope.ServiceProvider.GetRequiredService<PodcastDbContext>();
        
        await db.Database.MigrateAsync(cancellationToken);
    }
}