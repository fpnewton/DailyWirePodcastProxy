using AutoMapper;
using DailyWireApi.Models;
using DailyWireApi.Queries.GetPodcast;
using MediatR;
using PodcastDatabase.Repositories;
using PodcastProxy.Exceptions;
using Podcast = PodcastDatabase.Entities.Podcast;

namespace PodcastProxy.Commands.FetchPodcast;

public class FetchPodcastCommandHandler : IRequestHandler<FetchPodcastCommand, Podcast>
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly IPodcastRepository _repository;

    public FetchPodcastCommandHandler(IMapper mapper, IMediator mediator, IPodcastRepository repository)
    {
        _mapper = mapper;
        _mediator = mediator;
        _repository = repository;
    }

    public async Task<Podcast> Handle(FetchPodcastCommand request, CancellationToken cancellationToken)
    {
        var podcast = await _repository.GetPodcastById(request.PodcastId, cancellationToken);
        var query = new GetPodcastQuery { Id = request.PodcastId };
        var model = await _mediator.Send(query, cancellationToken);

        if (model is null)
        {
            throw new NotFoundException(nameof(GetPodcastRes), request.PodcastId);
        }

        if (podcast is null)
        {
            podcast = _mapper.Map<Podcast>(model);
            podcast = await _repository.InsertPodcast(podcast, cancellationToken);
        }
        else
        {
            podcast = _mapper.Map<Podcast>(model);
            podcast = await _repository.UpdatePodcast(podcast, cancellationToken);
        }

        return podcast;
    }
}