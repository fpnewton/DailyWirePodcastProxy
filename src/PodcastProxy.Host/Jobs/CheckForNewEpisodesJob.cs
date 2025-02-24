using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PodcastProxy.Application.Commands.CheckAllNewEpisodes;
using Quartz;

namespace PodcastProxy.Host.Jobs;

public class CheckForNewEpisodesJob(IServiceProvider serviceProvider) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        await using var scope = serviceProvider.CreateAsyncScope();

        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var command = new CheckAllNewEpisodesCommand();

        await mediator.Send(command, context.CancellationToken);
    }
}