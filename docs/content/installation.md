# Installation

There's no installation required for the current version of Beta Censoring. Just download the version appropriate to your platform and run it.

The server is currently built for 64-bit versions of Windows, Linux and macOS.

## Download

Download the latest release from the [GitHub Releases](https://github.com/silveredgold/beta-censoring/releases). Find the latest release and download the ZIP file for your platform. Unpack it somewhere convenient.

## Updating

If you are already running Beta Censoring, updating is generally as easy as download the new version and unpacking it over the top of the old version (make sure you close/stop the old version first). If you encounter problems with this method, unpack the new version to an empty folder, make sure it runs, then (if you have any) copy any stickers, captions or configuration files from your old install to the new location. 

> You can always check the current version you are using from the Web UI (usually at [`http://localhost:2382`](http://localhost:2382/))

## Running

### Windows

Open the folder you extracted the app to, then just double-click the executable (`BetaCensor.Server.exe`) to start the server. You may get alerts from SmartScreen or Windows Defender that they can't verify the app. That's expected since I'm not willing to pay the hundreds of dollars required for a code signing certificate to make those go away.

A console window should appear and after a few seconds, you're ready to rock, the server is now running. You can point your clients (like Beta Protection) to `http://localhost:2382` and get censoring.

### Linux

Open your terminal of choice and `cd` to the directory you unpacked the server to. The permissions sometimes get broken during build, so run `chmod 755 BetaCensor.Server` then `./BetaCensor.Server`. The server will unpack itself and then you're ready to rock, the server is now running. You can point your clients (like Beta Protection) to `http://localhost:2382` and get censoring.

### macOS

Apple doesn't think you know how to operate your own computer, so this shit is about to get complex.

Drag the folder you just unpacked onto the Terminal icon to open a terminal in the current directory. Run the following command to mark the server as executable: `chmod +x ./BetaCensor.Server`. Now, run the server with `./BetaCensor.Server`. After a few seconds, the server will **appear** to be running, but you also will have received some warnings about this app being unrecognised. This is expected as I'm not willing to pay Apple for the certificate they are expecting. You can close the current terminal window while we fix this.

Open Preferences > Security & Privacy and look for a note at the bottom of the page about how `BetaCensor.Server` was stopped from running. Click Open Anyway to allow it. Now, repeat the process as before: drag the folder on to Terminal, run `./BetaCensor.Server` and after a few seconds, you're ready to rock, the server is now running. You can point your clients (like Beta Protection) to `http://localhost:2382` and get censoring.

## Known Issues

On both Windows and macOS, you may get alerts from your operating system that they can't verify the app. That's expected since I'm not willing to pay the hundreds of dollars required for a code signing certificate to make those go away (in the case of macOS, that requires paying Apple a recurring subscription fee). If you're worried about the security of the server, you can always submit your downloaded files to VirusTotal, or just check the sources yourself [on GitHub](https://github.com/silveredgold/beta-censoring).

## Alternate Methods

### Docker

There is also a Docker image available if you're comfortable running the server in a container. Just pull `quay.io/beta-censoring/server:<VERSION-HERE>` and run it with Docker (or Docker Compose). You'll want to expose the listening port for the HTTP/SignalR API, so the final command will likely be something like:

```bash
docker run -p 2382:2382 quay.io/beta-censoring/server:<version-here>
```

See the available versions [at the repo on Quay.io](https://quay.io/repository/beta-censoring/server).

### System service

If you want to integrate running Beta Censoring Server with your system, you can! It supports both Systemd and Windows Services out of the box. Set it up to run with either method and the server will correctly integrate with startup/shutdown as well as logging to make it easier to manage.

#### Example Windows Service installation

Here's an example of setting the server up as a Windows Service. Change the path to match, and (optionally) adjust to suit your needs.

```powershell
$rootDir = "X:\Beta\betacensor-server-win-x64"
New-Service -Name beta-censoring -BinaryPathName "$rootDir/BetaCensor.Server.exe --contentRoot \"$rootDir\"" -DisplayName "Beta Censoring Server" -StartupType Automatic
```
