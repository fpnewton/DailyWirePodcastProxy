using System.Security.Cryptography;
using System.Web;
using DailyWirePodcastProxy.Extensions;
using DailyWirePodcastProxy.Models;
using IniParser;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace DailyWirePodcastProxy.Workers;

public class ApiAuthenticationWorker : BackgroundService
{
    private readonly ILogger<ApiAuthenticationWorker> _logger;
    private readonly Container _container;

    public ApiAuthenticationWorker(ILogger<ApiAuthenticationWorker> logger, Container container)
    {
        _logger = logger;
        _container = container;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var scope = ThreadScopedLifestyle.BeginScope(_container);
        var configurationSettings = scope.GetInstance<ConfigurationSettings>();

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
            
            _logger.LogInformation("API Access Key: {AccessKey}", accessKey);
        }
    }
}