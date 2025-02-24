using Microsoft.AspNetCore.Builder;
using PodcastProxy.Host.Jobs;
using Quartz;
using Quartz.AspNetCore;

namespace PodcastProxy.Host.Configuration;

public static class QuartzConfiguration
{
    public static WebApplicationBuilder ConfigureQuartzServices(this WebApplicationBuilder builder)
    {
        var section = builder.Configuration.GetSection("Jobs");
        var cronCheckForNewEpisodes = section["CheckForNewEpisodes"];
        var cronCheckAuthentication = section["CheckAuthentication"];

        if (string.IsNullOrEmpty(cronCheckForNewEpisodes))
        {
            throw new ArgumentNullException(nameof(cronCheckForNewEpisodes));
        }

        if (string.IsNullOrEmpty(cronCheckAuthentication))
        {
            throw new ArgumentNullException(nameof(cronCheckAuthentication));
        }

        builder.Services.AddQuartz(config =>
        {
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