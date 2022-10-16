using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PodcastDatabase.Entities;

namespace PodcastDatabase.Configuration;

public class EpisodeEntityConfiguration : IEntityTypeConfiguration<Episode>
{
    public void Configure(EntityTypeBuilder<Episode> builder)
    {
        builder.ToTable("Episodes");

        builder.HasKey(e => e.EpisodeId);

        builder.HasIndex(e => e.EpisodeId)
            .IsUnique();

        builder.HasIndex(e => e.Slug)
            .IsUnique();

        builder.HasIndex(e => e.SeasonId)
            .IsUnique(false);

        builder.Property(e => e.AllowedContinents)
            .HasConversion(v => JsonSerializer.Serialize(v, new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull }),
                v => JsonSerializer.Deserialize<IList<string>>(v, new JsonSerializerOptions())!);
        
        builder.Ignore(e => e.AudioMimeType);

        builder.Ignore(e => e.EpisodeNumber);
    }
}