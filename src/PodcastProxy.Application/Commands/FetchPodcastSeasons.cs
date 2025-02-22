using Ardalis.Result;
using AutoMapper;
using DailyWire.Api.Services;
using FluentValidation;
using MediatR;
using PodcastProxy.Database.Repositories;
using PodcastProxy.Domain.Entities;
using PodcastProxy.Domain.Specifications;

namespace PodcastProxy.Application.Commands;

public class FetchPodcastSeasonsCommand : IRequest<Result<IList<Season>>>
{
    public string PodcastId { get; set; } = string.Empty;
}

public class FetchPodcastSeasonsCommandValidator : AbstractValidator<FetchPodcastSeasonsCommand>
{
    public FetchPodcastSeasonsCommandValidator()
    {
        RuleFor(c => c.PodcastId)
            .NotNull()
            .NotEmpty();
    }
}

public class FetchPodcastSeasonsCommandHandler(
    IMapper mapper,
    IRepository<Season> repository,
    IDwApiService dwApiService
) : IRequestHandler<FetchPodcastSeasonsCommand, Result<IList<Season>>>
{
    public async Task<Result<IList<Season>>> Handle(FetchPodcastSeasonsCommand request, CancellationToken cancellationToken)
    {
        var result = await FetchPodcastSeasons(request, cancellationToken);

        if (!result.IsSuccess)
        {
            return result;
        }

        var spec = new SeasonByPodcastIdSpec(request.PodcastId);
        var seasons = await repository.ListAsync(spec, cancellationToken);

        var missing = result.Value.Except(seasons, Season.DefaultComparer).ToList();
        var existing = result.Value.Intersect(seasons, Season.DefaultComparer).ToList();

        if (missing.Count > 0)
        {
            await repository.AddRangeAsync(missing, cancellationToken);
        }

        if (existing.Count > 0)
        {
            await repository.UpdateRangeAsync(existing, cancellationToken);
        }

        return result;
    }

    private async Task<Result<IList<Season>>> FetchPodcastSeasons(FetchPodcastSeasonsCommand request, CancellationToken cancellationToken)
    {
        var result = await dwApiService.GetPodcastSeasonsById(request.PodcastId, cancellationToken);

        return result.Map(mapper.Map<IList<Season>>);
    }
}

public class FetchPodcastSeasonsEnsurePodcastExistsPipeline(
    IMediator mediator,
    IRepository<Podcast> podcastRepository
) : IPipelineBehavior<FetchPodcastSeasonsCommand, Result<IList<Season>>>
{
    public async Task<Result<IList<Season>>> Handle(FetchPodcastSeasonsCommand request, RequestHandlerDelegate<Result<IList<Season>>> next,
        CancellationToken cancellationToken)
    {
        var spec = new PodcastByIdSpec(request.PodcastId);
        var exists = await podcastRepository.AnyAsync(spec, cancellationToken);

        if (exists)
        {
            return await next();
        }

        var command = new FetchPodcastCommand { PodcastId = request.PodcastId };
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return await next();
        }

        return result.Map(_ => new List<Season>(0) as IList<Season>);
    }
}