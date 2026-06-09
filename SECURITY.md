# Security Policy

## Supported Versions

Security updates are provided for the latest released version of DailyWirePodcastProxy.

| Version | Supported |
| ------- | --------- |
| Latest release | Yes |
| Older releases | No |

## Reporting a Vulnerability

Please do not open a public GitHub issue for suspected security vulnerabilities.

Instead, report vulnerabilities privately using GitHub's private vulnerability reporting feature if available, or contact the maintainer directly.

When reporting a vulnerability, please include:

- A description of the issue
- Steps to reproduce it
- The affected version or commit
- Any relevant logs, configuration details, or deployment notes
- Whether credentials, access tokens, feed URLs, or authentication bypass are involved

Please do not include real DailyWire credentials, live access tokens, or private feed URLs in reports.

## Scope

Security issues include, but are not limited to:

- Authentication or access-key bypass
- Exposure of DailyWire credentials, access tokens, refresh tokens, or generated feed URLs
- SSRF or unsafe outbound request behavior
- Path traversal or unsafe file access
- Unsafe logging of secrets
- Docker/container configuration issues that expose sensitive data
- Vulnerabilities in dependencies that affect this project

Issues related to DailyWire's own platform, API, account system, or content access controls should be reported to DailyWire, not this project.

## Disclosure

I will review valid vulnerability reports and coordinate a fix before public disclosure when possible.
