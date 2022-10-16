using Microsoft.EntityFrameworkCore;
using PodcastDatabase.Entities;

namespace PodcastDatabase.Contexts;

public class PodcastDbContext : DbContext
{
    public DbSet<Episode> Episodes { get; set; } = null!;
    public DbSet<Podcast> Podcasts { get; set; } = null!;
    public DbSet<Season> Seasons { get; set; } = null!;

    public PodcastDbContext(DbContextOptions options) : base(options)
    {
    }
}