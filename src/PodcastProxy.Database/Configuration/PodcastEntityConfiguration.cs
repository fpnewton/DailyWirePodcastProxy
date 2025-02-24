using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PodcastProxy.Domain.Entities;

namespace PodcastProxy.Database.Configuration;

public class PodcastEntityConfiguration : IEntityTypeConfiguration<Podcast>
{
    public void Configure(EntityTypeBuilder<Podcast> builder)
    {
        builder.ToTable("Podcasts");
        
        builder.HasKey(e => e.Id);

        builder.HasIndex(e => e.Id)
            .IsUnique();

        builder.HasIndex(e => e.Slug)
            .IsUnique();

        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder.HasMany(e => e.Seasons)
            .WithOne(e => e.Podcast)
            .HasForeignKey(e => e.PodcastId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}