using System.Reflection;
using AutoMapper;
using SimpleInjector;

namespace DailyWirePodcastProxy.Configuration;

public static class AutoMapperConfiguration
{
    public static WebApplicationBuilder ConfigureAutoMapper(this WebApplicationBuilder builder, IList<Assembly> assemblies)
    {
        builder.Services.AddSingleton(provider => ProvideMapperConfiguration(provider, assemblies));
        builder.Services.AddSingleton(ProviderMapper);

        return builder;
    }

    private static MapperConfigurationExpression BuildMapperConfigurationExpression(Container container, IList<Assembly> assemblies)
    {
        var expression = new MapperConfigurationExpression();
        var mappingProfileTypes = GetMappingProfileTypes(assemblies);

        expression.ConstructServicesUsing(container.GetInstance);
        expression.AddProfiles(mappingProfileTypes);

        return expression;
    }

    private static void AddProfiles(this MapperConfigurationExpression expression, IEnumerable<Type> profileTypes)
    {
        foreach (var profileType in profileTypes)
        {
            expression.AddProfile(profileType);
        }
    }

    private static MapperConfiguration BuildMapperConfiguration(MapperConfigurationExpression expression)
    {
        var mapperConfiguration = new MapperConfiguration(expression);

        mapperConfiguration.AssertConfigurationIsValid();

        return mapperConfiguration;
    }

    private static MapperConfiguration ProvideMapperConfiguration(IServiceProvider provider, IList<Assembly> assemblies) =>
        BuildMapperConfiguration(BuildMapperConfigurationExpression(provider.GetRequiredService<Container>(), assemblies));

    private static IMapper ProviderMapper(IServiceProvider provider) =>
        new Mapper(provider.GetRequiredService<MapperConfiguration>(), provider.GetRequiredService<Container>().GetInstance);

    private static IEnumerable<Type> GetMappingProfileTypes(IEnumerable<Assembly> assemblies)
    {
        return assemblies.SelectMany(s => s.GetTypes())
            .Where(profileType => typeof(Profile).IsAssignableFrom(profileType))
            .Where(profileType => profileType.IsClass)
            .Where(profileType => !profileType.IsAbstract)
            .Where(profileType =>
            {
                var fullName = profileType.FullName;

                return fullName != null && !fullName.StartsWith("AutoMapper");
            });
    }
}