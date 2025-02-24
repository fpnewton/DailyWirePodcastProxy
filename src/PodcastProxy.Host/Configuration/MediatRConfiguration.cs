using System.Xml.Linq;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PodcastProxy.Application.Queries.GetPodcastFeed;

namespace PodcastProxy.Host.Configuration;

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

            // TODO Fix this for auto registration.
            // var behaviors = AppDomain.CurrentDomain.GetAssemblies()
            //     .SelectMany(a => a.GetTypes())
            //     .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IPipelineBehavior<,>)))
            //     .Where(t => t is { IsClass: true, IsAbstract: false })
            //     .Where(t => t.AssemblyQualifiedName?.StartsWith("MediatR", StringComparison.OrdinalIgnoreCase) == false)
            //     .ToList();

            config.AddBehavior<IPipelineBehavior<GetPodcastFeedQuery, XDocument>, GetPodcastFeedQueryPipeline>();
        });

        return builder;
    }
}