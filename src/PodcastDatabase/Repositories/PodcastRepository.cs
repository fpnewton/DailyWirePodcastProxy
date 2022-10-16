using Microsoft.EntityFrameworkCore;
using PodcastDatabase.Contexts;
using PodcastDatabase.Entities;

namespace PodcastDatabase.Repositories;

public interface IPodcastRepository
{
    public Task<ICollection<Podcast>> GetPodcasts(CancellationToken cancellationToken);

    public Task<Podcast?> GetPodcastById(string podcastId, CancellationToken cancellationToken);

    public Task<Podcast?> GetPodcastBySlug(string podcastSlug, CancellationToken cancellationToken);

    public Task<Podcast?> GetPodcastByIdWithEpisodes(string podcastId, CancellationToken cancellationToken);

    public Task<Podcast> InsertPodcast(Podcast podcast, CancellationToken cancellationToken);
    
    public Task<Podcast> UpdatePodcast(Podcast podcast, CancellationToken cancellationToken);
    
    public Task DeletePodcast(Podcast podcast, CancellationToken cancellationToken);
}

public class PodcastRepository : IPodcastRepository
{
    private readonly PodcastDbContext _db;

    public PodcastRepository(PodcastDbContext db)
    {
        _db = db;
    }

    public async Task<ICollection<Podcast>> GetPodcasts(CancellationToken cancellationToken) =>
        await _db.Podcasts
            .OrderBy(e => e.Name)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public async Task<Podcast?> GetPodcastById(string podcastId, CancellationToken cancellationToken) =>
        await _db.Podcasts
            .Where(e => string.Equals(e.Id, podcastId))
            .AsNoTracking()
            .SingleOrDefaultAsync(cancellationToken);

    public async Task<Podcast?> GetPodcastBySlug(string podcastSlug, CancellationToken cancellationToken) =>
        await _db.Podcasts
            .Where(e => string.Equals(e.Slug, podcastSlug))
            .AsNoTracking()
            .SingleOrDefaultAsync(cancellationToken);

    public async Task<Podcast?> GetPodcastByIdWithEpisodes(string podcastId, CancellationToken cancellationToken) =>
        await _db.Podcasts
            .Where(e => string.Equals(e.Id, podcastId))
            .Include(e => e.Seasons.OrderByDescending(s => s.Slug))
            .ThenInclude(e => e.Episodes.OrderByDescending(s => s.Slug))
            .AsNoTracking()
            .SingleOrDefaultAsync(cancellationToken);

    public async Task<Podcast> InsertPodcast(Podcast podcast, CancellationToken cancellationToken)
    {
        _db.Podcasts.Add(podcast);

        await _db.SaveChangesAsync(cancellationToken);

        _db.Entry(podcast).State = EntityState.Detached;

        return podcast;
    }

    public async Task<Podcast> UpdatePodcast(Podcast podcast, CancellationToken cancellationToken)
    {
        _db.Podcasts.Update(podcast);

        await _db.SaveChangesAsync(cancellationToken);

        _db.Entry(podcast).State = EntityState.Detached;

        return podcast;
    }

    public async Task DeletePodcast(Podcast podcast, CancellationToken cancellationToken)
    {
        _db.Podcasts.Remove(podcast);

        await _db.SaveChangesAsync(cancellationToken);
    }
}