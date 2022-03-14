# Installation

There's no installation required for the current version of Beta Censoring. Just download the version appropriate to your platform and run it.

The server is currently built for 64-bit versions of Windows, Linux and macOS.

## Download

Download the latest release from the [GitHub Releases](https://github.com/silveredgold/beta-censoring/releases). Find the latest release and download the ZIP file for your platform. Unpack it somewhere convenient.

## Running

Open the folder you extracted the app to, then just double-click the executable for your platform to start the server.

> That will be `BetaCensor.Server.exe` on Windows, and `BetaCensor.Server` on Linux or macOS

A console window should appear and after a few seconds, you're ready to rock, the server is now running. You can point your clients (like Beta Protection) to `http://localhost:2382` and get censoring.

## Known Issues

If you're on Windows, you may get alerts from SmartScreen or Windows Defender that they can't verify the app. That's expected since I'm not willing to pay the hundreds of dollars required for a code signing certificate to make those go away. If you're worried about the security of the server, you can always submit your downloaded files to VirusTotal, or just check the sources yourself [on GitHub](https://github.com/silveredgold/beta-censoring).

## Alternate Methods

### Docker

There is also a Docker image available if you're comfortable running the server in a container. Just pull `quay.io/beta-censoring/server:<VERSION-HERE>` and run it with Docker (or Docker Compose). You'll want to expose the listening port for the HTTP/SignalR API, so the final command will likely be something like:

```bash
docker run -p 2382:2382 quay.io/beta-censoring/server:<version-here>
```

See the available versions [at the repo on Quay.io](https://quay.io/repository/beta-censoring/server).

### System service

If you want to integrate running Beta Censoring Server with your system, you can! It supports both Systemd and Windows Services out of the box. Set it up to run with either method and the server will correctly integrate with startup/shutdown as well as logging to make it easier to manage.