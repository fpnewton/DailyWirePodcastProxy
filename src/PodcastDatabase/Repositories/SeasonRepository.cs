using Microsoft.EntityFrameworkCore;
using PodcastDatabase.Contexts;
using PodcastDatabase.Entities;

namespace PodcastDatabase.Repositories;

public interface ISeasonRepository
{
    public Task<ICollection<Season>> GetSeasonsByPodcastId(string podcastId, CancellationToken cancellationToken);

    public Task<Season?> GetLatestSeasonForPodcast(string podcastId, CancellationToken cancellationToken);

    public Task<Season?> GetSeasonById(string seasonId, CancellationToken cancellationToken);

    public Task<Season> InsertSeason(Season season, CancellationToken cancellationToken);

    public Task<Season> UpdateSeason(Season season, CancellationToken cancellationToken);
    
    public Task DeleteSeason(Season season, CancellationToken cancellationToken);
}

public class SeasonRepository : ISeasonRepository
{
    private readonly PodcastDbContext _db;

    public SeasonRepository(PodcastDbContext db)
    {
        _db = db;
    }

    public async Task<ICollection<Season>> GetSeasonsByPodcastId(string podcastId, CancellationToken cancellationToken) =>
        await _db.Seasons
            .Where(e => string.Equals(e.PodcastId, podcastId))
            .OrderByDescending(e => e.Name)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public async Task<Season?> GetLatestSeasonForPodcast(string podcastId, CancellationToken cancellationToken) =>
        await _db.Seasons
            .Where(e => string.Equals(e.PodcastId, podcastId))
            .OrderByDescending(e => e.Name)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<Season?> GetSeasonById(string seasonId, CancellationToken cancellationToken) =>
        await _db.Seasons
            .Where(e => string.Equals(e.SeasonId, seasonId))
            .AsNoTracking()
            .SingleOrDefaultAsync(cancellationToken);

    public async Task<Season> InsertSeason(Season season, CancellationToken cancellationToken)
    {
        _db.Seasons.Add(season);

        await _db.SaveChangesAsync(cancellationToken);

        _db.Entry(season).State = EntityState.Detached;

        return season;
    }

    public async Task<Season> UpdateSeason(Season season, CancellationToken cancellationToken)
    {
        _db.Seasons.Update(season);

        await _db.SaveChangesAsync(cancellationToken);

        _db.Entry(season).State = EntityState.Detached;

        return season;
    }

    public async Task DeleteSeason(Season season, CancellationToken cancellationToken)
    {
        _db.Seasons.Remove(season);

        await _db.SaveChangesAsync(cancellationToken);
    }
}