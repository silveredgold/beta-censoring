# Developing with Beta Censoring

> Note that this section of the documentation is specifically for advanced users and/or developers looking to integrate with Beta Censoring!
> If you're looking for how to get started with Beta Censoring, check the [installation](./installation.md) and [usage](./usage.md) docs.

## Design

Beta Censoring is designed as a server process providing a handful of different API/interface options for censoring images on-demand. The actual processing is done through separate workers to keep things performant and make the performance more customizable.

By default, Beta Censoring will run with 2 workers (i.e. up to 2 censoring jobs running at once), and will enable both the REST and SignalR APIs for all clients;

> There is a plugin API in the works, if you're interested in tweaking/replacing the inner workings of Beta Censoring.

### Projects

Just like CensorCore, BetaCensor is designed to be as flexible and configurable as possible, so the project is split up into distinct components to make reuse and integration easier. BetaCensor.Server is the main server entry point for Beta Censoring. In turn, it uses BetaCensor.Web and BetaCensor.Web.Status for most of the HTTP moving parts and BetaCensor.Workers for the worker queues (that do the actual censoring). 

This might seem a little complex at first glance (it is) but does make keeping things decoupled a little easier. It also means that you could actually embed the vast majority of Beta Censoring into your own server if you wanted to.

### CensorCore

[CensorCore](https://github.com/silveredgold/censor-core) is the underlying framework responsible for handling the nitty-gritty of both the AI inference, and the actual image censoring. CensoreCore is used to abstract the intricacies of the ONNX Runtime responsible for running the model (as well as the model itself) away from the Beta Censoring server. Beta Censoring is just responsible for providing much friendlier interfaces and a running server for non-native-C# clients.

> Note that the REST interface exposed in Beta Protection is actually just the REST server from CensorCore.Web, exposed directly at `/censoring/`

## APIs

### SignalR

SignalR is the "preferred" API for clients that support it as it is easier to use and more stable than the other options. While the server will accept any SignalR-supported transport, it's recommended to limit clients to WebSockets for performance.

The SignalR API is available at `/live` and exposes a handful of endpoints for the common requests (censoring images and cancelling requests). If you're using TS/JS, there is a `@silveredgold/beta-censor-client` package available which includes a reasonably easy-to-use client to access a running Beta Censoring server via SignalR

> The `@silveredgold/beta-censor-client` package does have a couple of notable dependencies and is designed to work with them so make sure it suits your use case.

### REST

The REST API is provided directly by the underlying [CensorCore](https://github.com/silveredgold/censor-core) framework, and is available at `/censoring`. The endpoints of interest are the `GET /getCensored` endpoint for zero-configuration quick censoring, and the `POST /censorImage` endpoint that supports custom censoring options.

Note that both options will block and only respond once censoring is complete! If you're after an asynchronous alternative, SignalR is your best bet.

Here's an example REST call:  
**URL:** `http://yourserver:yourport/censoring/censorImage`  
**Method:** `POST`  
**Headers:** `Content-Type: application/json`  
**Body:**
```json
{
  "ImageUrl":"http://url/of/your/image.png",
  "CensorOptions": {
    "COVERED_BREAST_F": {
      "Level":8,
      "CensorType":"blur"
    }
  }
}
```
`CensorOptions` inside the JSON payload can take a list of options. The option name `COVERED_BREAST_F` can be any of the class names as defined by the underlying model (https://github.com/notAI-tech/NudeNet), but also (at least) `MOUTH_F` and `EYES_F` which are the class names of the face detection model being used alongside the nudity one. `CensorType` can be `blur`, `pixel`, and `blackbars`. `Level` is an integer for the intensity of the censoring effect, which can be from `0` to `20`.


### Socket

There is initial support for accessing the REST API (and _theoretically_ the SignalR one) over a Unix socket, but that is disabled by default. Add a socket path to the `Server.SocketPath` key in your configuration and restart the server to enable the socket interface.
