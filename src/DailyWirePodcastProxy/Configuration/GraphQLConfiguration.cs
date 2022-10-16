using System.Net.Http.Headers;
using DailyWireAuthentication.Services;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using SimpleInjector;

namespace DailyWirePodcastProxy.Configuration;

// ReSharper disable once InconsistentNaming
public static class GraphQLConfiguration
{
    public static WebApplicationBuilder AddGraphQLClient(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpClient();
        builder.Services.AddHttpContextAccessor();
        
        builder.Services.AddTransient(provider => provider.GetRequiredService<Container>().GetRequiredService<ITokenService>());

        builder.Services.AddScoped<IGraphQLClient>(serviceProvider =>
        {
            var logger = serviceProvider.GetRequiredService<ILogger<IGraphQLClient>>();
            var client = serviceProvider.GetRequiredService<HttpClient>();
            // var client = new HttpClient(new HttpLoggingInterceptor());
            var tokenService = serviceProvider.GetRequiredService<ITokenService>();
            var serializer = new NewtonsoftJsonSerializer();
            var token = tokenService.GetAccessToken(CancellationToken.None).Result;
            var endpoint = builder.Configuration.GetConnectionString("GraphQL");
            
            logger.LogDebug("Token: {Token}", token);

            var options = new GraphQLHttpClientOptions
            {
                EndPoint = new Uri(endpoint)
            };

            client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Bearer {token}");

            return new GraphQLHttpClient(options, serializer, client);
        });

        return builder;
    }
}