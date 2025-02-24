using Ardalis.Specification;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PodcastProxy.Database.Repositories;
using PodcastProxy.Domain.Interfaces;

namespace PodcastProxy.Database;

public static class PodcastDatabaseSetup
{
    public static IServiceCollection ConfigurePodcastDatabase(this IServiceCollection services)
    {
        // Gets all classes that implement IEntity for automagic IRepository<> registration.
        var entityTypes = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsAssignableTo(typeof(IEntity)))
            .Where(t => t.IsClass)
            .Where(t => !t.IsAbstract)
            .ToList();

        foreach (var entityType in entityTypes)
        {
            services.AddRepository(entityType);
        }

        return services;
    }

    private static IServiceCollection AddRepository(this IServiceCollection services, Type entityType)
    {
        var repositoryBaseInterfaceType = typeof(IRepositoryBase<>).MakeGenericType(entityType);
        var repositoryInterfaceType = typeof(IRepository<>).MakeGenericType(entityType);
        var repositoryImplementationType = typeof(Repository<>).MakeGenericType(entityType);

        services.TryAddScoped(repositoryBaseInterfaceType, repositoryImplementationType);
        services.TryAddScoped(repositoryInterfaceType, repositoryImplementationType);

        return services;
    }
}