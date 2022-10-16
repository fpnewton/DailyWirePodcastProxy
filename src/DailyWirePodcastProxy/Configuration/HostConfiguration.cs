using DailyWirePodcastProxy.Models;

namespace DailyWirePodcastProxy.Configuration;

public static class HostConfiguration
{
    public static WebApplicationBuilder ConfigureHost(this WebApplicationBuilder builder)
    {
        builder.Host.ConfigureAppConfiguration((context, configuration) =>
        {
            var appName = context.HostingEnvironment.ApplicationName;
            var iniFilename = $"{appName}.ini";
            var iniFilePath = Path.Combine(context.HostingEnvironment.ContentRootPath, iniFilename);
            
            configuration.Sources.Clear();
            configuration.AddIniFile(iniFilePath, optional: false, reloadOnChange: true);
            configuration.AddEnvironmentVariables();

            builder.Services.AddSingleton(new ConfigurationSettings
            {
                IniFilepath = iniFilePath
            });
        });
        
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