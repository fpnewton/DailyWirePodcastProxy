using System.Net.Http.Headers;
using DailyWire.Api.Services;
using DailyWire.Authentication.Services;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DailyWire.Api;

[Obsolete]
public static class DailyWireApiSetup
{
    public static IServiceCollection ConfigureDailyWireApi(this IServiceCollection services)
    {
        services.AddHttpClient();

        services.AddScoped<IDwApiService, DwApiService>();

        services.AddScoped<IGraphQLClient>(provider =>
        {
            var logger = provider.GetRequiredService<ILogger<IGraphQLClient>>();
            var client = provider.GetRequiredService<HttpClient>();
            var tokenService = provider.GetRequiredService<ITokenService>();
            var serializer = new NewtonsoftJsonSerializer();
            var token = tokenService.GetAccessToken(CancellationToken.None).Result;
            var endpoint = provider.GetRequiredService<IConfiguration>().GetConnectionString("GraphQL");

            if (string.IsNullOrEmpty(endpoint))
            {
                throw new Exception("Invalid GraphQL endpoint.");
            }

            logger.LogDebug("Token: {Token}", token);

            var options = new GraphQLHttpClientOptions
            {
                EndPoint = new Uri(endpoint)
            };

            client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Bearer {token}");

            return new GraphQLHttpClient(options, serializer, client);
        });

        return services;
    }
}