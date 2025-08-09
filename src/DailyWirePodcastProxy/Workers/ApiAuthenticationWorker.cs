using PodcastProxy.Domain.Services;

namespace DailyWirePodcastProxy.Workers;

public class ApiAuthenticationWorker(IServiceProvider serviceProvider) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // await using var scope = serviceProvider.CreateAsyncScope();

        var logger = serviceProvider.GetRequiredService<ILogger<ApiAuthenticationWorker>>();
        var authDetailsProvider = serviceProvider.GetRequiredService<IAuthenticationDetailsProvider>();
        var apiAccessKey = authDetailsProvider.GetApiAccessKey();

        if (!authDetailsProvider.AccessKeyRequirementEnabled())
        {
            return Task.CompletedTask;
        }

        if (string.IsNullOrEmpty(apiAccessKey))
        {
            apiAccessKey = authDetailsProvider.CreateApiAccessKey();
        }

        // var configurationSettings = scope.ServiceProvider.GetRequiredService<ConfigurationSettings>();
        // var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApiAuthenticationWorker>>();
        //
        // if (string.IsNullOrEmpty(configurationSettings.IniFilepath) || !File.Exists(configurationSettings.IniFilepath))
        // {
        //     throw new Exception($"{nameof(ConfigurationSettings.IniFilepath)} is not valid");
        // }
        //
        // var parser = new FileIniDataParser();
        // var config = parser.ReadFile(configurationSettings.IniFilepath);
        //
        // if (string.IsNullOrEmpty(config["Authentication"]["AccessKey"]))
        // {
        //     var keyBytes = RandomNumberGenerator.GetBytes(16);
        //     var accessKey = keyBytes.ToBase58String();
        //
        //     config["Authentication"]["AccessKey"] = HttpUtility.UrlEncode(accessKey);
        //
        //     parser.WriteFile(configurationSettings.IniFilepath, config);
        //     
        logger.LogInformation("API Access Key: {AccessKey}", apiAccessKey);
        
        return Task.CompletedTask;
        // }
    }
}