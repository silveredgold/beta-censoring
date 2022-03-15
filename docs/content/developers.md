# Developing with Beta Censoring

> Note that this section of the documentation is specifically for advanced users and/or developers looking to integrate with Beta Censoring!
> If you're looking for how to get started with Beta Censoring, check the [installation](./installation.md) and [usage](./usage.md) docs.

## Design

Beta Censoring is designed as a server process providing a handful of different API/interface options for censoring images on-demand. The actual processing is done through separate workers to keep things performant and make the performance more customizable.

By default, Beta Censoring will run with 2 workers (i.e. up to 2 censoring jobs running at once), and will enable both the REST and SignalR APIs for all clients;

> There is a plugin API in the works, if you're interested in tweaking/replacing the inner workings of Beta Censoring.

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

### Socket

There is initial support for accessing the REST API (and _theoretically_ the SignalR one) over a Unix socket, but that is disabled by default. Add a socket path to the `Server.SocketPath` key in your configuration and restart the server to enable the socket interface.