using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PodcastDatabase.Entities;

namespace PodcastDatabase.Configuration;

public class SeasonEntityConfiguration : IEntityTypeConfiguration<Season>
{
    public void Configure(EntityTypeBuilder<Season> builder)
    {
        builder.ToTable("Seasons");

        builder.HasKey(e => e.SeasonId);
        
        builder.HasIndex(e => e.SeasonId)
            .IsUnique();
        
        builder.HasIndex(e => e.Slug)
            .IsUnique();

        builder.HasIndex(e => e.PodcastId)
            .IsUnique(false);

        builder.HasMany(e => e.Episodes)
            .WithOne(e => e.Season)
            .HasForeignKey(e => e.SeasonId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}