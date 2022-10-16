using DailyWirePodcastProxy.Authentication;
using DailyWirePodcastProxy.Authorization;
using DailyWirePodcastProxy.Filters;
using Microsoft.AspNetCore.Authorization;

namespace DailyWirePodcastProxy.Configuration;

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

        builder.Services.AddControllers(options => { options.Filters.Add<ExceptionFilter>(); });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        return builder;
    }

    public static WebApplication ConfigureApi(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        app.MapControllers();

        return app;
    }
}