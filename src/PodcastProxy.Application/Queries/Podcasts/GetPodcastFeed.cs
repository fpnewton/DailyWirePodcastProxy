using System.Xml.Linq;
using Ardalis.Result;
using FastEndpoints;
using PodcastProxy.Application.Commands.Podcasts;

namespace PodcastProxy.Application.Queries.Podcasts;

public class GetPodcastFeedQuery : ICommand<Result<XDocument>>
{
    public required string PodcastId { get; set; }
    public required string FeedUrl { get; set; }
    public required string StreamUrl { get; set; }
    public required string StreamUrlSlug { get; set; }
}

public class GetPodcastFeedQueryHandler : ICommandHandler<GetPodcastFeedQuery, Result<XDocument>>
{
    public async Task<Result<XDocument>> ExecuteAsync(GetPodcastFeedQuery command, CancellationToken ct)
    {
        await new EnsurePodcastExistsCommand { PodcastId = command.PodcastId }.ExecuteAsync(ct);

        var podcast = await new GetPodcastByIdQuery { PodcastId = command.PodcastId }.ExecuteAsync(ct);

        if (!podcast.IsSuccess)
        {
            return podcast.Map();
        }

        XNamespace itunes = "http://www.itunes.com/dtds/podcast-1.0.dtd";
        XNamespace googlePlay = "http://www.google.com/schemas/play-podcasts/1.0";
        XNamespace atom = "http://www.w3.org/2005/Atom";
        XNamespace media = "http://search.yahoo.com/mrss/";
        XNamespace content = "http://purl.org/rss/1.0/modules/content/";

        var document = new XDocument();

        var root = new XElement("rss",
            new XAttribute(XNamespace.Xmlns + "itunes", itunes.NamespaceName),
            new XAttribute(XNamespace.Xmlns + "googleplay", googlePlay.NamespaceName),
            new XAttribute(XNamespace.Xmlns + "atom", atom.NamespaceName),
            new XAttribute(XNamespace.Xmlns + "media", media.NamespaceName),
            new XAttribute(XNamespace.Xmlns + "content", content.NamespaceName),
            new XAttribute("version", "2.0")
        );

        document.Add(root);

        var channel = new XElement("channel",
            new XElement(atom + "link",
                new XAttribute("href", command.FeedUrl),
                new XAttribute("rel", "self"),
                new XAttribute("type", "application/rss+xml")
            ),
            new XElement("link", $"https://www.dailywire.com/show/{podcast.Value.Slug}"),
            new XElement("title", podcast.Value.Name),
            new XElement("description", podcast.Value.Description),
            new XElement(itunes + "subtitle", podcast.Value.Description),
            new XElement("language", "en"),
            new XElement("copyright", "All rights reserved"),
            new XElement(itunes + "author", "The Daily Wire"),
            new XElement(itunes + "explicit", "no"),
            new XElement(itunes + "type", "episodic"),
            new XElement(itunes + "summary", podcast.Value.Description),
            new XElement(content + "encoded", new XCData($"<p>{podcast.Value.Description}</p>")),
            new XElement(
                itunes + "owner",
                new XElement(itunes + "name", podcast.Value.Name),
                new XElement(itunes + "email", "podcasts@dailywire.com")
            )
        );

        if (podcast.Value.CoverImage is not null)
        {
            channel.Add(
                new XElement("image",
                    new XElement("url", podcast.Value.CoverImage),
                    new XElement("title", podcast.Value.Name),
                    new XElement("link", $"https://www.dailywire.com/show/{podcast.Value.Slug}")
                ),
                new XElement(itunes + "image",
                    new XAttribute("href", podcast.Value.CoverImage)
                )
            );
        }

        var seasons = podcast.Value.Seasons.OrderByDescending(s => s.Slug).ToList();

        foreach (var season in seasons)
        {
            var episodes = season.Episodes.OrderByDescending(s => s.PublishDate).ToList();

            foreach (var episode in episodes)
            {
                var item = new XElement("item",
                    new XElement(itunes + "episodeType", "full"),
                    new XElement(itunes + "episode", episode.EpisodeNumber),
                    new XElement(itunes + "author", "The Daily Wire"),
                    new XElement(itunes + "summary", episode.Description),
                    new XElement(content + "encoded", new XCData($"<p>{episode.Description}</p>")),
                    new XElement(itunes + "duration", episode.Duration.HasValue ? Math.Round(episode.Duration.Value) : null)
                );

                if (!string.IsNullOrEmpty(episode.Title))
                {
                    item.Add(new XElement("title", episode.Title));
                    item.Add(new XElement(itunes + "title", episode.Title));
                }

                if (!string.IsNullOrEmpty(episode.Description))
                {
                    item.Add(new XElement("description", episode.Description));
                }

                var episodeDate = episode.PublishDate ?? episode.ScheduleAt;

                if (episodeDate.HasValue)
                {
                    var timestamp = episodeDate.Value.ToString("ddd, dd MMM yyyy HH:mm:ss zz") + episodeDate.Value.Offset.ToString("mm");

                    item.Add(new XElement("pubDate", timestamp));
                }

                var enclosure = new XElement("enclosure", new XAttribute("type", "audio/m4a"));
                var streamUrl = command.StreamUrl.Replace(command.StreamUrlSlug, episode.Slug);
                
                enclosure.Add(new XAttribute("url", streamUrl));

                if (episode.Duration.HasValue)
                {
                    enclosure.Add(new XAttribute("length", Math.Round(episode.Duration.Value)));
                }

                item.Add(enclosure);
                channel.Add(item);
            }
        }

        root.Add(channel);

        return document;
    }
}