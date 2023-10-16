using MediatR;
using PodcastProxy.Commands.CheckAllNewEpisodes;
using Quartz;

namespace DailyWirePodcastProxy.Jobs;

public class CheckForNewEpisodesJob : IJob
{
    // private readonly Container _container;

    // public CheckForNewEpisodesJob(Container container)
    // {
    //     _container = container;
    // }

    public async Task Execute(IJobExecutionContext context)
    {
        // var scope = AsyncScopedLifestyle.BeginScope(_container);
        // var mediator = scope.GetInstance<IMediator>();
        // var command = new CheckAllNewEpisodesCommand();
        //
        // await mediator.Send(command, context.CancellationToken);
    }
}