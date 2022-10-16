using System.Reflection;
using MediatR;
using MediatR.Pipeline;
using SimpleInjector;

namespace DailyWirePodcastProxy.Configuration;

public static class MediatRConfiguration
{
    public static WebApplicationBuilder ConfigureMediatR(this WebApplicationBuilder builder, IList<Assembly> assemblies)
    {
        var serviceProvider = builder.Services.BuildServiceProvider();
        var container = serviceProvider.GetService<Container>() ?? throw new NullReferenceException(nameof(Container));

        builder.Services.AddScoped(_ => container.GetInstance<IMediator>());

        container.RegisterSingleton<IMediator, Mediator>();
        container.Register(typeof(IRequestHandler<,>), assemblies);
        container.RegisterGenericCollection(typeof(INotificationHandler<>), assemblies);
        container.RegisterGenericCollection(typeof(IRequestExceptionAction<,>), assemblies);
        container.RegisterGenericCollection(typeof(IRequestExceptionHandler<,,>), assemblies);
        container.RegisterGenericCollection(typeof(IRequestPreProcessor<>), assemblies);
        container.RegisterGenericCollection(typeof(IRequestPostProcessor<,>), assemblies);
        
        builder.Services.AddSingleton(provider => provider.GetRequiredService<Container>().GetInstance<IMediator>());

        var options = new TypesToRegisterOptions()
        {
            IncludeGenericTypeDefinitions = true,
            IncludeComposites = false
        };

        var baseBehaviors = new[]
        {
            typeof(RequestExceptionProcessorBehavior<,>),
            typeof(RequestExceptionActionProcessorBehavior<,>),
            typeof(RequestPreProcessorBehavior<,>),
            typeof(RequestPostProcessorBehavior<,>)
        };

        var serviceTypes = container.GetTypesToRegister(typeof(IPipelineBehavior<,>), assemblies, options).Union(baseBehaviors);

        container.Collection.Register(typeof(IPipelineBehavior<,>), serviceTypes);
        container.Register(() => new ServiceFactory(container.GetInstance), Lifestyle.Singleton);

        return builder;
    }

    private static void RegisterGenericCollection(this Container container, Type type, IEnumerable<Assembly> assemblies, bool generics = true, bool composites = false)
    {
        var typesToRegister = container.GetTypesToRegister(type, assemblies, new TypesToRegisterOptions
        {
            IncludeGenericTypeDefinitions = generics,
            IncludeComposites = composites
        });

        container.Collection.Register(type, typesToRegister);
    }
}