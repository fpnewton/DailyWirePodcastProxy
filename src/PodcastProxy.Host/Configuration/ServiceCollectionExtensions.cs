using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PodcastProxy.Database.Contexts;

namespace PodcastProxy.Host.Configuration;

public static class ServiceCollectionExtensions
{
    public static WebApplicationBuilder AddPodcastDatabase(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("Database");
        
        builder.Services.AddDbContext<PodcastDbContext>(options =>
        {
            options.UseSqlite(connectionString, sql =>
            {
                sql.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            });
        });

        return builder;
    }
}