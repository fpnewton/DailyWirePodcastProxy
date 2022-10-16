using DailyWirePodcastProxy.Jobs;
using Quartz;

namespace DailyWirePodcastProxy.Configuration;

public static class QuartzConfiguration
{
    public static WebApplicationBuilder ConfigureQuartzServices(this WebApplicationBuilder builder)
    {
        var section = builder.Configuration.GetSection("Jobs");
        var cronCheckForNewEpisodes = section["CheckForNewEpisodes"];
        var cronCheckAuthentication = section["CheckAuthentication"];

        builder.Services.AddQuartz(config =>
        {
            config.UseMicrosoftDependencyInjectionJobFactory();

            config.ScheduleJob<CheckForNewEpisodesJob>(trigger => trigger.WithCronSchedule(cronCheckForNewEpisodes));
            config.ScheduleJob<CheckAuthenticationJob>(trigger => trigger.WithCronSchedule(cronCheckAuthentication));
        });

        builder.Services.AddQuartzServer(options =>
        {
            options.AwaitApplicationStarted = true;
            options.WaitForJobsToComplete = true;
        });

        return builder;
    }
}