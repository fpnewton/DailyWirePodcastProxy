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

public class FetchPodcastTest
{
    private readonly Fixture _fixture = new();
    private readonly IMapper _mapper = Substitute.For<IMapper>();
    private readonly IRepository<Podcast> _repository = Substitute.For<IRepository<Podcast>>();
    private readonly IDwApiService _dwApiService = Substitute.For<IDwApiService>();

    [Fact]
    public void ValidatorShouldFailForNullPodcastId()
    {
        // Arrange
        var command = new FetchPodcastCommand { PodcastId = null! };
        var validator = new FetchPodcastCommandValidator();

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
        var command = new FetchPodcastCommand { PodcastId = string.Empty };
        var validator = new FetchPodcastCommandValidator();

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
        var command = _fixture.Create<FetchPodcastCommand>();
        var validator = new FetchPodcastCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task ShouldReturnErrorWhenDwPodcastFetchError()
    {
        // Arrange
        var command = _fixture.Create<FetchPodcastCommand>();
        var handler = new FetchPodcastCommandHandler(_mapper, _repository, _dwApiService);

        _dwApiService.GetPodcastById(string.Empty, default)
            .ReturnsForAnyArgs(Result.NotFound());

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task ShouldCreatePodcastWhenNotExists()
    {
        // Arrange
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        var command = _fixture.Create<FetchPodcastCommand>();
        var handler = new FetchPodcastCommandHandler(_mapper, _repository, _dwApiService);
        var podcastRes = _fixture.Create<DwGetPodcastRes>();
        var podcast = _fixture.Create<Podcast>();

        _mapper.Map<Podcast>(podcastRes).Returns(podcast);

        _dwApiService.GetPodcastById(string.Empty, default)
            .ReturnsForAnyArgs(Result<DwGetPodcastRes>.Success(podcastRes));

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _mapper.Received().Map<Podcast>(podcastRes);

        await _repository.Received().AddAsync(podcast, CancellationToken.None);
        await _repository.DidNotReceiveWithAnyArgs().UpdateAsync(default!);
        await _repository.DidNotReceiveWithAnyArgs().UpdateRangeAsync(default!);
    }

    [Fact]
    public async Task ShouldUpdatePodcastWhenExists()
    {
        // Arrange
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        var command = _fixture.Create<FetchPodcastCommand>();
        var handler = new FetchPodcastCommandHandler(_mapper, _repository, _dwApiService);
        var podcastRes = _fixture.Create<DwGetPodcastRes>();
        var podcast = _fixture.Create<Podcast>();

        _mapper.Map<Podcast>(podcastRes).Returns(podcast);

        _repository.AnyAsync(default!, default)
            .ReturnsForAnyArgs(true);

        _dwApiService.GetPodcastById(string.Empty, default)
            .ReturnsForAnyArgs(Result<DwGetPodcastRes>.Success(podcastRes));

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _mapper.Received().Map<Podcast>(podcastRes);

        await _repository.DidNotReceiveWithAnyArgs().AddAsync(default!);
        await _repository.DidNotReceiveWithAnyArgs().AddRangeAsync(default!);
        await _repository.Received().UpdateAsync(podcast, CancellationToken.None);
    }
}