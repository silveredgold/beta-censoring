# Adding the Status Panel to a `BetaCensor.Web`-enabled server

## Add Required Parts

Add a reference to `BetaCensor.Web.Status` and then add an implementation of `IServerInfoService` to return basic info about the running server.

The `IServerInfoService` should return the status of all available APIs from the `GetServices` method, and the state of any other components from the `GetComponents` method. For example, the reference Server implementation returns the worker count.

## Add Service Registration

In your service registration (either `Program.cs` or `Startup.cs`), add a call to `AddStatusPages` like below:

```cs
// minimal APIs
builder.Services.AddStatusPages<ServerInfoService>(builder.Environment);

//or for Startup:
services.AddStatusPages<ServerInfoService>(builder.Environment);
```

replacing `ServerInfoService` with your `IServerInfoService` implementation. This will automatically call `AddRazorPages` so you don't need to do that separately.

## Add to pipeline

In your pipeline setup (either `Program.cs` or `Startup.cs`), add a call to `UseStatusPages` like below:

```cs
app.UseStatusPages(app.Environment);
```

This will automatically call `MapRazorPages` so you don't need to do that separately.
