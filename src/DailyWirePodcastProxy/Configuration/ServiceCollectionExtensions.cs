using Microsoft.EntityFrameworkCore;
using PodcastDatabase.Contexts;

namespace DailyWirePodcastProxy.Configuration;

public static class ServiceCollectionExtensions
{
    public static WebApplicationBuilder AddPodcastDatabase(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("Database");
        
        builder.Services.AddDbContext<PodcastDbContext>(options =>
        {
            options.UseSqlite(connectionString, builder =>
            {
                builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            });
        });

        return builder;
    }
}