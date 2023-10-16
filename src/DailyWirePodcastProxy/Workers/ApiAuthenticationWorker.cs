using System.Security.Cryptography;
using System.Web;
using DailyWirePodcastProxy.Extensions;
using DailyWirePodcastProxy.Models;
using IniParser;

namespace DailyWirePodcastProxy.Workers;

public class ApiAuthenticationWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public ApiAuthenticationWorker(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        
        var configurationSettings = scope.ServiceProvider.GetRequiredService<ConfigurationSettings>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApiAuthenticationWorker>>();
        
        if (string.IsNullOrEmpty(configurationSettings.IniFilepath) || !File.Exists(configurationSettings.IniFilepath))
        {
            throw new Exception($"{nameof(ConfigurationSettings.IniFilepath)} is not valid");
        }
        
        var parser = new FileIniDataParser();
        var config = parser.ReadFile(configurationSettings.IniFilepath);
        
        if (string.IsNullOrEmpty(config["Authentication"]["AccessKey"]))
        {
            var keyBytes = RandomNumberGenerator.GetBytes(16);
            var accessKey = keyBytes.ToBase58String();
        
            config["Authentication"]["AccessKey"] = HttpUtility.UrlEncode(accessKey);
        
            parser.WriteFile(configurationSettings.IniFilepath, config);
            
            logger.LogInformation("API Access Key: {AccessKey}", accessKey);
        }
    }
}