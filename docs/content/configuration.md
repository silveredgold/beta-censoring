---
title: 'Server Configuration'
---

> Note that in the preview releases, configuring the server is an advanced process that assumes some familiarity with editing configuration files yourself.
> This process will improve in future releases so it is only recommended to proceed if you are comfortable with that, and really need to.

## Configuration Files

The server will look for configuration in a handful of locations when it starts up, but it's recommended to use configuration files stored in the same directory as the server (i.e. the same folder where `BetaCensor.Server` is located).

You can use either JSON or YAML configuration files, which should be named `config.json` or `config.yml` respectively.

The most useful configuration block is the `Server` block. When not specified, the default is equivalent to the following:

<CodeGroup>
  <CodeGroupItem title="YAML" active>

```yaml
Server:
  WorkerCount: 2
  EnableSignalR: true
  EnableRest: true
```

  </CodeGroupItem>

  <CodeGroupItem title="JSON">

```json
{
    "Server": {
        "WorkerCount": 2,
        "EnableSignalR": true,
        "EnableRest": true
    }
}
```

  </CodeGroupItem>
</CodeGroup>

## Worker Configuration

It may be tempting to dramatically increase the `WorkerCount` setting to get more workers censoring images at once, but this is probably not a good idea.

Increasing the worker count will **dramatically** increase the load on your PC while censoring. Additionally, adding more workers actually has the potential to _slow down_ censoring, especially if you add more workers than your PC can reasonably run at once. A reasonable rule of thumb is to use half the number of cores your CPU has. If you're okay with things really slowing down during censoring, you can try going as high as the number of cores, but it's **strongly recommended** to not go above this number.