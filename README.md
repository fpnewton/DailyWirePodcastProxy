# DailyWirePodcastProxy

DailyWirePodcastProxy is a self-hosted ASP.NET Core service that creates private
RSS feeds for full-length DailyWire podcast episodes available to an authorized
DailyWire account. It stores podcast metadata in SQLite and proxies episode
audio so a podcast client can consume it.

An active DailyWire subscription with access to the requested content is
required. This project uses unofficial DailyWire endpoints and may stop working
if those endpoints or their authentication flow change.

## Important security notes

> **Treat every generated feed URL as a secret.** Feed and media URLs contain
> the proxy access key in the `auth` query parameter. Do not publish, forward,
> screenshot, or include these URLs in bug reports. Use the private-feed option
> in your podcast client.

- Set `[Authentication] Enabled = true` before exposing the service beyond a
  trusted local environment. When enabled, every mapped API and Razor Page
  request must have an `auth` value that exactly matches `AccessKey`; failed
  authorization returns HTTP 403.
- The access key is a shared secret, not user authentication. Anyone who has it
  can use the proxy with the permissions of the connected DailyWire account.
- The application serves HTTP and does not configure TLS. For remote access,
  put it behind an HTTPS reverse proxy or tunnel with appropriate access
  controls. Do not expose the container or application port directly to the
  public internet.
- The generated access key is written to `DailyWirePodcastProxy.ini` and logged
  at Information level during startup. Protect both the configuration file and
  application logs.
- DailyWire access and refresh tokens are stored as plain JSON in the configured
  token file. The SQLite database, token file, and INI file must be readable and
  writable only by the service account where possible.
- On an authorization failure, the application logs the request method, URL,
  and protocol. The value of a query parameter named `auth` is redacted
  case-insensitively, and request data is stripped of newlines and HTML-escaped.
  Other query values are not redacted. Reverse proxies, tunnels, clients, and
  hosting platforms may log the complete URL independently.

## Features

- Auth0 device-code login without storing a DailyWire username or password
- Automatic access-token refresh when a refresh token is available
- SQLite storage for podcast, season, and episode metadata
- Scheduled metadata updates using Quartz cron expressions
- RSS 2.0 feeds containing authenticated proxy URLs for episode audio
- HTTP range and conditional-request forwarding for audio and video streams
- Optional shared access-key protection for all endpoints
- Docker image and Compose example

## Project structure

All projects currently target .NET 10 (`net10.0`).

| Path | Responsibility |
| --- | --- |
| `src/PodcastProxy.Web` | Executable ASP.NET Core host, login page, and configuration file |
| `src/PodcastProxy.Host` | Host setup, access-key authorization, jobs, workers, and request-log formatting |
| `src/PodcastProxy.Api` | HTTP endpoints, RSS generation entry points, QR codes, and stream proxy endpoints |
| `src/PodcastProxy.Application` | Podcast queries and update commands |
| `src/PodcastProxy.Domain` | Entities, specifications, configuration models, and interfaces |
| `src/PodcastProxy.Database` | EF Core SQLite context, repositories, and migrations |
| `src/DailyWire.Authentication` | Device-code authentication, token refresh, and token-file storage |
| `src/DailyWire.Api.Middleware` | DailyWire metadata API client and response models |
| `src/DailyWire.Api.Streaming` | Media streaming client |
| `tests/PodcastProxy.Application.Tests` | xUnit tests for token handling, entities, and request-log redaction |

## Configuration

The application requires `DailyWirePodcastProxy.ini` in its content root.
`src/PodcastProxy.Web/DailyWirePodcastProxy.ini` is the local sample, and
`docker/DailyWirePodcastProxy.ini` is the container sample. The example below
uses the recommended secure setting `Enabled = true`; review the actual sample
you run because the local and container files may use different values.

```ini
[Host]
Host = http://127.0.0.1:9473
BasePath = /
PublicHost =

[Authentication]
Enabled = true
AccessKey =

[DailyWireApi]
ShowPageSlug = watch-page

[Jobs]
CheckForNewEpisodes = 0 0/15 * * * ?
CheckAuthentication = 0 0 * * * ?

[TokenStorage]
FilePath = tokens.dat

[OAuth]
Issuer = authorize.dailywire.com
Audience = https://api.dailywire.com/
ClientId = FCgw3nA6cxkcXLVseAQvCSVBrymwvfpE
Scope = openid profile offline_access

[ConnectionStrings]
Database = DataSource=podcasts.db
MiddlewareApi = https://middleware-prod.dailywire.com/middleware
StreamApi = https://stream.media.dailywire.com

[Logging:LogLevel]
Default = Information
Microsoft.AspNetCore = Warning
Microsoft.EntityFrameworkCore = Warning
Microsoft.EntityFrameworkCore.Migrations = Information
Quartz = Warning
```

Configuration keys:

- `Host:Host`: Required listen URL passed directly to the web host. Use
  `http://127.0.0.1:9473` for local-only access or `http://0.0.0.0:9473` in a
  container.
- `Host:BasePath`: Path prefix used when the service is mounted below a domain
  root, for example `/dailywire`. Use `/` when there is no prefix.
- `Host:PublicHost`: Optional externally visible scheme and host, such as
  `https://podcasts.example.com`. It is used for startup login, QR-code, and
  authorization callback URLs. It is not used to construct feed or media URLs.
- `Authentication:Enabled`: Enables the shared `auth` query-parameter check.
  Keep this `true` for any remotely reachable deployment.
- `Authentication:AccessKey`: Shared proxy key. When authentication is enabled
  and this is blank, startup generates a random key and writes it back to the
  INI file.
- `DailyWireApi:ShowPageSlug`: DailyWire page slug used to discover shows.
- `Jobs:CheckForNewEpisodes`: Quartz cron schedule for metadata updates. The
  sample runs every 15 minutes.
- `Jobs:CheckAuthentication`: Quartz cron schedule for token validation and
  refresh. The sample runs hourly.
- `TokenStorage:FilePath`: Token JSON file path. Relative paths are resolved
  from the process working directory.
- `OAuth:*`: Auth0 device-flow settings used for DailyWire authorization. The
  checked-in values are the current application defaults.
- `ConnectionStrings:Database`: EF Core SQLite connection string. Migrations
  run automatically at startup.
- `ConnectionStrings:MiddlewareApi` and `StreamApi`: DailyWire metadata and
  media service base URLs.
- `Logging:LogLevel:*`: Standard .NET logging category levels.

Environment variables are loaded after the INI file and can override
configuration using .NET's double-underscore syntax, for example
`Host__PublicHost=https://podcasts.example.com`. Access-key creation and some
login-link generation read the INI file directly, so do not set conflicting
`Authentication__AccessKey` and INI values.

## Running locally

Install the .NET 10 SDK, then run from the web project directory so the INI file
and relative data paths resolve there:

```bash
cd src/PodcastProxy.Web
dotnet run
```

The checked-in local sample currently listens on `127.0.0.1:9473`. Review
`Authentication:Enabled` before starting; enable it for any access outside a
trusted machine. The application creates or updates `podcasts.db`, `tokens.dat`,
and the access key in the working directory.

When DailyWire authorization is needed, startup prints a private login URL.
Open it, follow the Auth0 device-code link (or scan the QR code), and approve the
device. The page polls the local `/authorize` endpoint and stores the returned
access and refresh tokens. Later requests refresh the tokens automatically when
possible; if no refresh token was issued or refresh fails, repeat device login.

## Running with Docker

The repository's Compose example uses
`ghcr.io/fpnewton/dailywirepodcastproxy:master`, publishes port 9473, mounts the
container-specific INI file, and adds `SYS_ADMIN`:

```bash
docker compose -f docker/docker-compose.yml up -d
docker compose -f docker/docker-compose.yml logs -f
```

The Docker INI sample already uses `Host = http://0.0.0.0:9473` and enables
access-key authentication. The INI bind mount must be writable so the generated
key can be saved.

The Compose file mounts `docker/data` at `/app/data`, but the checked-in INI
uses `tokens.dat` and `DataSource=podcasts.db`, which resolve under `/app`, not
`/app/data`. To persist tokens and database state across container replacement,
change the mounted `docker/DailyWirePodcastProxy.ini` to:

```ini
[TokenStorage]
FilePath = data/tokens.dat

[ConnectionStrings]
Database = DataSource=data/podcasts.db
MiddlewareApi = https://middleware-prod.dailywire.com/middleware
StreamApi = https://stream.media.dailywire.com
```

The image's `ASPNETCORE_URLS` value does not override `Host:Host`, because the
application explicitly starts with the INI `Host` value.

## Feed usage

After DailyWire authorization, discover the shows available to the account:

```text
http://127.0.0.1:9473/daily-wire/shows?auth=YOUR_ACCESS_KEY
```

The response includes a `Feed` URL for each show:

```text
https://podcasts.example.com/podcasts/PODCAST_ID/feed?auth=YOUR_ACCESS_KEY
```

Requesting a feed for the first time initializes that podcast and its metadata
in SQLite. The scheduled update job refreshes podcasts already present in the
database. `/podcasts?auth=YOUR_ACCESS_KEY` lists those locally initialized
podcasts; it may be empty on a new installation.

The feed's episode enclosures use this format:

```text
https://podcasts.example.com/daily-wire/podcasts/episodes/EPISODE_SLUG/streams/audio?auth=YOUR_ACCESS_KEY
```

If `BasePath` is `/dailywire`, that prefix appears before `podcasts` and
`daily-wire`. Feed and enclosure URLs are built from the incoming request's
scheme and host. Add the feed to a podcast client that supports private custom
RSS feeds, and mark it private where the client offers that option.

## Deployment notes

The safest default is local-only use or deployment on a trusted private
network. Remote clients require a reachable HTTPS endpoint, so use a reverse
proxy or a tunnel such as Cloudflare Tunnel and keep access-key checking
enabled. A tunnel provides connectivity; it does not make a leaked feed URL or
access key harmless.

Configure the proxy to preserve the external `Host` header and scheme when
possible. The application does not enable ASP.NET Core forwarded-header
middleware, and `PublicHost` does not affect generated feed/media URLs. If the
proxy sends requests to the app as HTTP or replaces the host, generated URLs
may contain the internal scheme or host. Verify the URLs returned by
`/podcasts` before adding them to a client.

For a subpath deployment, set the same leading-slash prefix in `Host:BasePath`
and configure the proxy to route that prefix without adding it twice. Streaming
responses set `X-Accel-Buffering: no`; reverse proxies should avoid buffering
large media responses and should support range requests.

## Development

Restore and build the solution:

```bash
dotnet restore DailyWirePodcastProxy.sln
dotnet build DailyWirePodcastProxy.sln
```

The repository includes a release workflow for self-contained `linux-x64`,
`osx-arm64`, `osx-x64`, and `win-x64` artifacts. The Docker workflow publishes
GHCR tags for `master`, feature branches, commit SHAs, and `latest` on the
default branch. Dependabot checks NuGet dependencies weekly. No CodeQL workflow
is currently present.

## Testing

Run the xUnit test project or the full solution:

```bash
dotnet test tests/PodcastProxy.Application.Tests/PodcastProxy.Application.Tests.csproj
# or
dotnet test DailyWirePodcastProxy.sln
```

Tests cover token-file persistence, token refresh behavior, season entities,
and request-log sanitization/redaction. There are no end-to-end tests for live
DailyWire APIs, reverse proxies, Docker networking, or podcast clients.

## Known limitations

- The service is designed around one DailyWire account and one local token
  store; it is not a multi-user service.
- DailyWire metadata and OAuth configuration are tied to current unofficial
  service behavior.
- Feed/media URLs use a query-string secret, which can leak through client,
  proxy, browser-history, analytics, or infrastructure logs.
- The application has no built-in TLS, rate limiting, access-key rotation, or
  administrative UI.
- Feed URL generation depends on the incoming request host and scheme rather
  than `PublicHost`.
- SQLite and file-based token storage assume a single writable application
  instance sharing local state.

## Security policy

See [SECURITY.md](SECURITY.md) for supported versions and private vulnerability
reporting guidance. Do not include real credentials, tokens, access keys, or
feed URLs in a report.

## Disclaimer

This project is unofficial, is not affiliated with or endorsed by DailyWire,
and is provided under the [MIT License](LICENSE) without warranty. Use it only
with an account and content you are authorized to access, and comply with all
applicable terms and laws.
