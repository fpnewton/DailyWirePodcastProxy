using Ardalis.Result;
using AutoMapper;
using DailyWire.Api.Services;
using FluentValidation;
using MediatR;
using PodcastProxy.Database.Repositories;
using PodcastProxy.Domain.Specifications;
using Podcast = PodcastProxy.Domain.Entities.Podcast;

namespace PodcastProxy.Application.Commands;

public class FetchPodcastCommand : IRequest<Result<Podcast>>
{
    public string PodcastId { get; set; } = string.Empty;
}

public class FetchPodcastCommandValidator : AbstractValidator<FetchPodcastCommand>
{
    public FetchPodcastCommandValidator()
    {
        RuleFor(c => c.PodcastId)
            .NotNull()
            .NotEmpty();
    }
}

public class FetchPodcastCommandHandler(
    IMapper mapper,
    IRepository<Podcast> repository,
    IDwApiService dwApiService
) : IRequestHandler<FetchPodcastCommand, Result<Podcast>>
{
    public async Task<Result<Podcast>> Handle(FetchPodcastCommand request, CancellationToken cancellationToken)
    {
        var result = await FetchPodcast(request, cancellationToken);

        if (!result.IsSuccess)
        {
            return result;
        }

        var spec = new PodcastByIdSpec(result.Value);
        var exists = await repository.AnyAsync(spec, cancellationToken);

        if (!exists)
        {
            await repository.AddAsync(result.Value, cancellationToken);
        }
        else
        {
            await repository.UpdateAsync(result.Value, cancellationToken);
        }

        return result;
    }

    private async Task<Result<Podcast>> FetchPodcast(FetchPodcastCommand request, CancellationToken cancellationToken)
    {
        var result = await dwApiService.GetPodcastById(request.PodcastId, cancellationToken);

        return result.Map(mapper.Map<Podcast>);
    }
}