using System.Security.Cryptography;
using System.Web;
using IniParser;
using Microsoft.Extensions.Options;
using PodcastProxy.Domain.Models;
using PodcastProxy.Domain.Services;
using PodcastProxy.Web.Extensions;

namespace PodcastProxy.Web.Services;

public class AuthenticationDetailsProvider(
    IServiceProvider serviceProvider,
    IOptionsMonitor<AuthOptions> options
) : IAuthenticationDetailsProvider
{
    public bool AccessKeyRequirementEnabled() => options.CurrentValue.Enabled;
    
    public string? GetApiAccessKey()
    {
        var configurationSettings = serviceProvider.GetRequiredService<ConfigurationSettings>();

        if (string.IsNullOrEmpty(configurationSettings.IniFilepath) || !File.Exists(configurationSettings.IniFilepath))
        {
            throw new Exception($"{nameof(ConfigurationSettings.IniFilepath)} is not valid");
        }

        var parser = new FileIniDataParser();
        var config = parser.ReadFile(configurationSettings.IniFilepath);

        if (!string.IsNullOrEmpty(config["Authentication"]["AccessKey"]))
        {
            return config["Authentication"]["AccessKey"];
        }

        return null;
    }

    public string? CreateApiAccessKey()
    {
        if (!AccessKeyRequirementEnabled())
        {
            return null;
        }
        
        var configurationSettings = serviceProvider.GetRequiredService<ConfigurationSettings>();

        if (string.IsNullOrEmpty(configurationSettings.IniFilepath) || !File.Exists(configurationSettings.IniFilepath))
        {
            throw new Exception($"{nameof(ConfigurationSettings.IniFilepath)} is not valid");
        }

        var parser = new FileIniDataParser();
        var config = parser.ReadFile(configurationSettings.IniFilepath);

        if (!string.IsNullOrEmpty(config["Authentication"]["AccessKey"]))
        {
            return config["Authentication"]["AccessKey"];
        }

        var keyBytes = RandomNumberGenerator.GetBytes(16);
        var accessKey = keyBytes.ToBase58String();

        config["Authentication"]["AccessKey"] = HttpUtility.UrlEncode(accessKey);

        parser.WriteFile(configurationSettings.IniFilepath, config);

        return accessKey;
    }
}