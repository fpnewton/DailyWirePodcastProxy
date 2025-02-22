using Ardalis.Result;
using AutoFixture;
using AutoMapper;
using DailyWire.Api.Models;
using DailyWire.Api.Services;
using NSubstitute;
using PodcastProxy.Application.Commands;
using PodcastProxy.Database.Repositories;
using PodcastProxy.Domain.Entities;

namespace PodcastProxy.Application.Tests.Commands;

public class FetchPodcastSeasons
{
    private readonly Fixture _fixture = new();
    private readonly IMapper _mapper = Substitute.For<IMapper>();
    private readonly IRepository<Season> _repository = Substitute.For<IRepository<Season>>();
    private readonly IDwApiService _dwApiService = Substitute.For<IDwApiService>();

    [Fact]
    public void ValidatorShouldFailForNullPodcastId()
    {
        // Arrange
        var command = new FetchPodcastSeasonsCommand { PodcastId = null! };
        var validator = new FetchPodcastSeasonsCommandValidator();
        
        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void ValidatorShouldFailForEmptyPodcastId()
    {
        // Arrange
        var command = new FetchPodcastSeasonsCommand { PodcastId = string.Empty };
        var validator = new FetchPodcastSeasonsCommandValidator();
        
        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void ValidatorShouldPassForPodcastId()
    {
        // Arrange
        var command = _fixture.Create<FetchPodcastSeasonsCommand>();
        var validator = new FetchPodcastSeasonsCommandValidator();
        
        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task ShouldReturnErrorWhenDwGetPodcastSeasonsFetchError()
    {
        // Arrange
        var command = _fixture.Create<FetchPodcastSeasonsCommand>();
        var handler = new FetchPodcastSeasonsCommandHandler(_mapper, _repository, _dwApiService);

        _dwApiService.GetPodcastSeasonsById(string.Empty, default)
            .ReturnsForAnyArgs(Result.NotFound());

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task ShouldCreateSeasonsWhenNotExists()
    {
        // Arrange
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        var command = _fixture.Create<FetchPodcastSeasonsCommand>();
        var handler = new FetchPodcastSeasonsCommandHandler(_mapper, _repository, _dwApiService);
        var podcastRes = _fixture.Create<IList<DwSeasonDetails>>();
        var seasons = _fixture.Create<IList<Season>>();

        _mapper.Map<IList<Season>>(podcastRes)
            .Returns(seasons);

        _repository.ListAsync(default!, default)
            .ReturnsForAnyArgs([]);

        _dwApiService.GetPodcastSeasonsById(string.Empty, default)
            .ReturnsForAnyArgs(Result<IList<DwSeasonDetails>>.Success(podcastRes));

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _mapper.Received().Map<IList<Season>>(podcastRes);

        await _repository.DidNotReceiveWithAnyArgs().AddAsync(default!);
        await _repository.Received().AddRangeAsync(Arg.Is<List<Season>>(l => l.SequenceEqual(seasons, Season.DefaultComparer)), CancellationToken.None);
        await _repository.DidNotReceiveWithAnyArgs().UpdateAsync(default!);
        await _repository.DidNotReceiveWithAnyArgs().UpdateRangeAsync(default!);
    }

    [Fact]
    public async Task ShouldUpdateSeasonsWhenExists()
    {
        // Arrange
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        var command = _fixture.Create<FetchPodcastSeasonsCommand>();
        var handler = new FetchPodcastSeasonsCommandHandler(_mapper, _repository, _dwApiService);
        var podcastRes = _fixture.Create<IList<DwSeasonDetails>>();
        var seasons = _fixture.Create<IList<Season>>();

        _mapper.Map<IList<Season>>(podcastRes)
            .Returns(seasons);

        _repository.ListAsync(default!, default)
            .ReturnsForAnyArgs(seasons.ToList());

        _dwApiService.GetPodcastSeasonsById(string.Empty, default)
            .ReturnsForAnyArgs(Result<IList<DwSeasonDetails>>.Success(podcastRes));

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _mapper.Received().Map<IList<Season>>(podcastRes);

        await _repository.DidNotReceiveWithAnyArgs().AddAsync(default!);
        await _repository.DidNotReceiveWithAnyArgs().AddRangeAsync(default!);
        await _repository.DidNotReceiveWithAnyArgs().UpdateAsync(default!);
        await _repository.Received().UpdateRangeAsync(Arg.Is<List<Season>>(l => l.SequenceEqual(seasons, Season.DefaultComparer)), CancellationToken.None);
    }
}