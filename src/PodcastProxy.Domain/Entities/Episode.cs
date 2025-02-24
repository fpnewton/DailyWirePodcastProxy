using System.Text.RegularExpressions;
using Microsoft.AspNetCore.StaticFiles;
using PodcastProxy.Domain.Interfaces;

namespace PodcastProxy.Domain.Entities;

public class Episode : IEntity
{
    private static readonly FileExtensionContentTypeProvider ContentTypeProvider = new();
    private static readonly Regex EpisodeNumberFromTitleRegex = new(@"Ep\.\w(?'num'\d+)", RegexOptions.Compiled);
    private static readonly Regex EpisodeNumberFromSlugRegex = new(@"ep-(?'num'\d+)", RegexOptions.Compiled);

    public string EpisodeId { get; set; } = null!;
    public string SeasonId { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Audio { get; set; }
    public double? ListenTime { get; set; }
    public IList<string> AllowedContinents = new List<string>();
    public Uri? Thumbnail { get; set; }
    public double? Duration { get; set; }
    public double? Rating { get; set; }
    public string? Status { get; set; }
    public string? AudioState { get; set; }
    public DateTimeOffset? PublishDate { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public DateTimeOffset? ScheduleAt { get; set; }

    public Season Season { get; set; } = null!;

    public string? AudioMimeType
    {
        get
        {
            if (!string.IsNullOrEmpty(Audio))
            {
                if (ContentTypeProvider.TryGetContentType(Path.GetFileName(Audio), out var mimeType))
                {
                    return mimeType;
                }
            }

            return null;
        }
    }

    public int? EpisodeNumber
    {
        get
        {
            var episodeTitleNum = EpisodeNumberFromTitleRegex.Match(Title ?? string.Empty);
            var episodeSlugNum = EpisodeNumberFromSlugRegex.Match(Slug);
            var episodeNum = string.Empty;

            if (episodeTitleNum.Groups["num"].Success)
            {
                episodeNum = episodeTitleNum.Groups["num"].Value;
            }
            else if (episodeSlugNum.Groups["num"].Success)
            {
                episodeNum = episodeSlugNum.Groups["num"].Value;
            }

            if (int.TryParse(episodeNum, out var num))
            {
                return num;
            }

            return null;
        }
    }
}