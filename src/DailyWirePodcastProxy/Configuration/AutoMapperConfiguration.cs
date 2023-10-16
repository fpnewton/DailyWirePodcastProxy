using System.Reflection;
using AutoMapper;

namespace DailyWirePodcastProxy.Configuration;

public static class AutoMapperConfiguration
{
    public static WebApplicationBuilder ConfigureAutoMapper(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton(provider => ProvideMapperConfiguration(provider, AppDomain.CurrentDomain.GetAssemblies()));
        builder.Services.AddSingleton(ProviderMapper);

        return builder;
    }

    private static MapperConfigurationExpression BuildMapperConfigurationExpression(IServiceProvider services, IList<Assembly> assemblies)
    {
        var expression = new MapperConfigurationExpression();
        var mappingProfileTypes = GetMappingProfileTypes(assemblies);

        expression.ConstructServicesUsing(services.GetService);
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

    private static MapperConfiguration
        ProvideMapperConfiguration(IServiceProvider provider, IList<Assembly> assemblies) =>
        BuildMapperConfiguration(BuildMapperConfigurationExpression(provider, assemblies));

    private static IMapper ProviderMapper(IServiceProvider provider) =>
        new Mapper(provider.GetRequiredService<MapperConfiguration>(), provider.GetService);

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