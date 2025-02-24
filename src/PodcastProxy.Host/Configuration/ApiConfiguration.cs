using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PodcastProxy.Host.Authentication;
using PodcastProxy.Host.Authorization;

namespace PodcastProxy.Host.Configuration;

public static class ApiConfiguration
{
    private const string AuthKeyScheme = "AuthKey";

    public static WebApplicationBuilder ConfigureApi(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(AuthKeyScheme)
            .AddScheme<DefaultAuthenticationOptions, DefaultAuthenticationHandler>(AuthKeyScheme, null);

        builder.Services.AddAuthorization(options =>
        {
            var policy = new AuthorizationPolicyBuilder(AuthKeyScheme)
                .AddRequirements(new AuthKeyAuthorizationRequirement())
                .Build();

            options.DefaultPolicy = policy;
            options.FallbackPolicy = policy;
        });

        builder.Services.AddSingleton<IAuthorizationHandler, AuthKeyAuthorizationHandler>();

        builder.Services.AddControllers();
        
        builder.Services.AddRazorPages()
            .AddRazorRuntimeCompilation();

        builder.Services.AddEndpointsApiExplorer();
        // builder.Services.AddSwaggerGen();

        return builder;
    }

    public static WebApplication ConfigureApi(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            // app.UseSwagger();
            // app.UseSwaggerUI();
        }

        // app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        app.MapRazorPages();
        app.MapControllers();

        return app;
    }
}