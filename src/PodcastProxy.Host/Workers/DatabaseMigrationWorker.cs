using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PodcastProxy.Database.Contexts;

namespace PodcastProxy.Host.Workers;

public class DatabaseMigrationWorker(IServiceProvider serviceProvider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        await using var db = scope.ServiceProvider.GetRequiredService<PodcastDbContext>();
        
        await db.Database.MigrateAsync(cancellationToken);
    }
}