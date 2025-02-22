using Microsoft.EntityFrameworkCore;
using PodcastProxy.Domain.Entities;

namespace PodcastProxy.Database.Contexts;

public class PodcastDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Episode> Episodes { get; set; } = null!;
    public DbSet<Podcast> Podcasts { get; set; } = null!;
    public DbSet<Season> Seasons { get; set; } = null!;
}