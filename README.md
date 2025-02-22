# DailyWirePodcastProxy

DailyWirePodcastProxy is an RSS podcast proxy to allow podcast clients to fetch full-length and ad-free podcast episodes from The DailyWire that are only available on their website or in their mobile app.
It is a standalone API service that must be deployed where it can be accessed to your desired podcast client.


*An active subscription to The DailyWire required.*

## How It Works
Authenticated GraphQL requests are made with an access token for your account to The DailyWire API that are similar to the requests made by their mobile app.
Podcast subscriptions are then stored in a local SQLite database.
New episodes are queried on a schedule and the local database is updated to reflect all new changes that have been found.
A podcast feed endpoint formats the podcast episodes stored in the localdatabase into an RSS feed for your podcast client to monitor.
This feed provides your podcast client direct URLs to the same full-length and ad-free episodes that the mobile app plays.

## Installation
### Option 1: Local Install
1. Install the package or extract the release contents into a folder
2. Edit `DailyWirePodcastProxy.ini` and add your DailyWire username/password
3. Start DailyWirePodcastProxy service

A systemd unit file `dailywire-podcast-proxy.service` is provided for convenience.

### Option 2: Docker
DailyWirePodcastProxy can be run via Docker with the container `ghcr.io/fpnewton/dailywirepodcastproxy:master`. Editing `DailyWirePodcastProxy.ini` with your credentials is still required, and the AccessKey will be written-back to the mapped file (assuming write is permitted).

> **Note**
> The `[Host]` section of `DailyWirePodcastProxy.ini` should be changed to `http://0.0.0.0:9473` to allow access from outside the container (in addition to the port mapping in Docker)

A `docker-compose.yml` file is provided as an example to build off of. 

## Configuration
DailyWirePodcastProxy is configured by editing `DailyWirePodcastProxy.ini`

### `[Host]` Section
* `Host` The full address for this service to listen on. Host must include scheme, host and port (Default: `http://127.0.0.1:9473`)
* `BasePath` The base path for this service to listen on. This is useful for when the service is behind a reverse proxy and must include a leading `/` (Default: `/`)

### `[Authentication]` Section
* `AccessKey` The access key required to interact with the service. See below for more details. 

### `[Jobs]` Section
For more information about the cron syntax used, see [Quartz.NET Cron Trigger Expressions](https://www.quartz-scheduler.net/documentation/quartz-3.x/tutorial/crontriggers.html) documentation.
* `CheckForNewEpisodes` The cron job schedule for how often new episodes are queried. (Default: Every 15 minutes)
* `CheckAuthentication` The cron job schedule for how often the access token is checked. New tokens are retrieved at 75% of the current token's lifetime. (Default: Every hour)

### `[TokenStorage]` Section
* `FilePath` Relative path to where authentication tokens are stored. This file must be readable and writable by the running service.

### `[Logging:LogLevel]` Section
This allows you to customize the logging levels for the service. For more information, see [.NET Logging](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-6.0#configure-logging) documentation.

## Using DailyWirePodcastProxy
When the servie is first started, the service will auto-generate a unique access key.
`DailyWirePodcastProxy.ini` will be automatically updated with the generated access key here:
```ini
[Authentication]
AccessKey = LokYdDAw23dBTEo7t753q1
```

This access key is required for all API usage of this service and must be passed in the URL as query parameter `auth` otherwise the service will return a 403 error if it is missing.


To get a list of available podcasts, you can view `/podcasts` in your browser:
`http://127.0.0.1:9473/podcasts?auth=LokYdDAw23dBTEo7t753q1`

This endpoint will return a JSON list of available podcasts and their respective RSS feed URLs.
The feed URL includes the access key and can be opened by your podcast client.

Note: These feed URLs are specific to your account and should not be shared.
Please make sure your podcast client is configured to handle this as a private feed.

### Authorizing Access to the Daily Wire API
After first starting up the podcast proxy (or if the last time it was used has been greater than 30 days) you will need to grant it access to the Daily Wire API for your account.
There will also be a message logged in the console output with a link to the login page to start the authorization process:
```
===========================================
Authorization Required!

Please visit: http://127.0.0.1:9473/login?auth=LokYdDAw23dBTEo7t753q1
===========================================
```
If you are behind a reverse proxy, replace `http://127.0.0.1:9473` with your domain name and/or configured path.

On this login page, click on the link provided or scan the QR code with your phone and then follow the instructions to complete the authorization process.
After clicking on the 'Authorize' button, the login page will automatically reload if authorization was successful, otherwise an error message will be shown.

If your API token is still valid, this login page will only show that authorization is not currently needed.

### Example Usage With Pocket Casts
Pocket Casts supports private RSS feeds and you can add custom feeds here: [https://pocketcasts.com/submit/](https://pocketcasts.com/submit/)
Enter the feed URL returned by the `/podcasts` endpoint in the `Feed URL` box. An example feed URL behind a reverse proxy looks like this:
```
http://my.domain.com/podcasts/ckx5tt6x72rns0869otk48cf7/feed?auth=LokYdDAw23dBTEo7t753q1
```
Make sure to select the `Private` option so this feed is not cached in their podcast repository.
