using Microsoft.EntityFrameworkCore;
using PodcastDatabase.Contexts;
using PodcastDatabase.Entities;

namespace PodcastDatabase.Repositories;

public interface IEpisodeRepository
{
    public Task<ICollection<Episode>> GetEpisodesBySeasonId(string seasonId, CancellationToken cancellationToken);
    
    public Task<ICollection<Episode>> GetEpisodesByPodcastId(string podcastId, CancellationToken cancellationToken);

    public Task<bool> EpisodeExists(string episodeId, CancellationToken cancellationToken);

    public Task<Episode?> GetEpisodeById(string episodeId, CancellationToken cancellationToken);

    public Task<Episode> InsertEpisode(Episode episode, CancellationToken cancellationToken);
    
    public Task<Episode> UpdateEpisode(Episode episode, CancellationToken cancellationToken);

    public Task DeleteEpisode(Episode episode, CancellationToken cancellationToken);
}

public class EpisodeRepository : IEpisodeRepository
{
    private readonly PodcastDbContext _db;

    public EpisodeRepository(PodcastDbContext db)
    {
        _db = db;
    }

    public async Task<ICollection<Episode>> GetEpisodesBySeasonId(string seasonId, CancellationToken cancellationToken) =>
        await _db.Episodes
            .Where(e => string.Equals(e.SeasonId, seasonId))
            .OrderByDescending(e => e.EpisodeNumber)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public async Task<ICollection<Episode>> GetEpisodesByPodcastId(string podcastId, CancellationToken cancellationToken) =>
        await _db.Podcasts
            .Where(e => string.Equals(e.Id, podcastId))
            .Include(e => e.Seasons)
            .SelectMany(e => e.Seasons)
            .Include(e => e.Episodes)
            .SelectMany(e => e.Episodes)
            .OrderByDescending(e => e.EpisodeNumber)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public async Task<bool> EpisodeExists(string episodeId, CancellationToken cancellationToken) =>
        await _db.Episodes
            .Where(e => string.Equals(e.EpisodeId, episodeId))
            .AnyAsync(cancellationToken);

    public async Task<Episode?> GetEpisodeById(string episodeId, CancellationToken cancellationToken) =>
        await _db.Episodes
            .Where(e => string.Equals(e.EpisodeId, episodeId))
            .AsNoTracking()
            .SingleOrDefaultAsync(cancellationToken);

    public async Task<Episode> InsertEpisode(Episode episode, CancellationToken cancellationToken)
    {
        _db.Episodes.Add(episode);

        await _db.SaveChangesAsync(cancellationToken);

        _db.Entry(episode).State = EntityState.Detached;

        return episode;
    }

    public async Task<Episode> UpdateEpisode(Episode episode, CancellationToken cancellationToken)
    {
        _db.Episodes.Update(episode);

        await _db.SaveChangesAsync(cancellationToken);

        _db.Entry(episode).State = EntityState.Detached;

        return episode;
    }

    public async Task DeleteEpisode(Episode episode, CancellationToken cancellationToken)
    {
        _db.Episodes.Remove(episode);

        await _db.SaveChangesAsync(cancellationToken);
    }
}