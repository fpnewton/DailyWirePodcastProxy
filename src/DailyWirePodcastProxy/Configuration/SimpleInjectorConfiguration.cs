using System.Reflection;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace DailyWirePodcastProxy.Configuration;

public static class SimpleInjectorConfiguration
{
    public static WebApplicationBuilder ConfigureSimpleInjector(this WebApplicationBuilder builder, IList<Assembly> assemblies)
    {
        var container = new Container()
        {
            Options =
            {
                DefaultLifestyle = Lifestyle.CreateHybrid(Lifestyle.Scoped, Lifestyle.Transient),
                DefaultScopedLifestyle = Lifestyle.CreateHybrid(new AsyncScopedLifestyle(), new ThreadScopedLifestyle())
            }
        };

        builder.Services.AddSimpleInjector(container, options =>
        {
            options.AddLogging();
            options.AddAspNetCore().AddControllerActivation();

            options.AutoCrossWireFrameworkComponents = true;
            options.DisposeContainerWithServiceProvider = true;
        });

        container.RegisterPackages(assemblies);

        return builder;
    }

    public static WebApplication EnableSimpleInjector(this WebApplication app)
    {
        var builder = (IApplicationBuilder)app;
        var container = builder.ApplicationServices.GetService<Container>() ?? throw new NullReferenceException("Container");

        builder.UseSimpleInjector(container);
        container.Verify();

        return app;
    }
}