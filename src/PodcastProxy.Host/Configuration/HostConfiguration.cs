using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PodcastProxy.Domain.Models;

namespace PodcastProxy.Host.Configuration;

public static class HostConfiguration
{
    public static WebApplicationBuilder ConfigureHost(this WebApplicationBuilder builder)
    {
        var iniFilePath = Path.Combine(builder.Environment.ContentRootPath, "DailyWirePodcastProxy.ini");

        builder.Configuration.AddIniFile(iniFilePath, false, true);
        builder.Configuration.AddEnvironmentVariables();

        builder.Services.AddSingleton(new ConfigurationSettings
        {
            IniFilepath = iniFilePath
        });

        builder.Services
            .AddOptions<AuthOptions>()
            .Bind(builder.Configuration.GetSection(AuthOptions.ConfigSectionName));

        builder.Host.UseSystemd();

        return builder;
    }

    public static WebApplication ConfigureHost(this WebApplication app)
    {
        var section = app.Configuration.GetSection("Host");
        var pathBase = section["BasePath"];

        app.UsePathBase(pathBase);

        return app;
    }
}