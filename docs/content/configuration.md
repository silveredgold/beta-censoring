---
title: 'Configuration'
---

> Note that in the preview releases, configuring the server is an advanced process that assumes some familiarity with editing configuration files yourself.
> This process will hopefully improve in future releases so it is only recommended to proceed if you are comfortable with that, and really need to.

## Configuration Files

The server will look for configuration in a handful of locations when it starts up, but it's recommended to use configuration files stored in the same directory as the server (i.e. the same folder where `BetaCensor.Server` is located).

You can use either JSON or YAML configuration files, which should be named `config.json` or `config.yml` respectively.

> While we strongly recommend you manually build your own configuration file with only the parts you need, you can see an example of (almost) all the configuration options available to the server in <a :href="$withBase('/config_example.yml')">this example configuration</a>.

## Sticker Configuration

You can enable stickers and control their use using the `Stickers` section of the config file. You can either use the `config.yml`/`config.json` or use a separate `stickers.yml`/`stickers.json`. You can configure two kinds of locations for stickers: sticker stores and separate paths.

While this might seem a little complex, it should be easy to get set up once and means you can use any of your own images as stickers, no matter where they're stored.

### Stores

A "sticker store" is a folder with stickers for more than one category, with directories for each category available in the store. For example, a store might be laid out like this:

```text:no-line-numbers
C:/path/to/my/sticker/store
└───Professional
    │  squareprocap1.png
    │  squareprocap2.png
    │  squareprocap4.png
└───Chastity
    │  chastity_sticker1.png
    │  ch_sticker2.png
    │  ch_sticker3.png
```

#### Default Store

Beta Censoring looks for a default store at the `stickers/` path from the server folder. That is, if there's a folder called `stickers` in the same place as `BetaCensor.Server.exe`, Beta Censoring will automatically load any subfolders in that folder as categories of stickers. 

Likewise, it will also load any ZIP files with the `.betapkg` extension or with names ending in `-stickers.zip`. That is, if you have a ZIP file named `my-beta-stickers.zip` in the same folder as `BetaCensor.Server.exe`, Beta Censoring will automatically load any folders in that ZIP file as categories of stickers.

#### Configuration

You can also add sticker stores to your configuration like below, and it would make a `Professional` and `Chastity` category available (using the sample layout from above).

<CodeGroup>
  <CodeGroupItem title="YAML" active>

```yaml
Stickers:
  LocalStores: ['C:/path/to/my/sticker/store']
```

  </CodeGroupItem>

  <CodeGroupItem title="JSON">

```json
{
    "Stickers": {
        "LocalStores": ["C:/path/to/my/sticker/store"]
    }
}
```

  </CodeGroupItem>
</CodeGroup>

> If you have a Beta Safety installation handy, you can always add its `browser-extension/images/stickers` path as a store to Beta Censoring.

### Sticker Paths

If you just want to add some images with a specific category without moving/copying them, you can do that too. The `Paths` configuration lets you add any number of folders, anywhere on your PC, as stickers for any category you like. For example, with the following configuration:

<CodeGroup>
  <CodeGroupItem title="YAML" active>

```yaml
Stickers:
  Paths:
    Discreet: ["C:/pictures/discreet-stickers"]
    Chastity: ["C:/pictures/chastity-stickers", "D:/downloads/stickers"]
```

  </CodeGroupItem>

  <CodeGroupItem title="JSON">

```json
{
    "Stickers": {
        "Paths": {
          "Discreet": ["C:/pictures/discreet-stickers"],
          "Chastity": ["C:/pictures/chastity-stickers", "D:/downloads/stickers"]
        }
    }
}
```

  </CodeGroupItem>
</CodeGroup>

Any images in any of the folders provided for a given category (`Discreet` and `Chastity` in the example above) will be merged together and used any time a client requests sticker censoring with those categories enabled.

You also don't need to worry about images being the exact right dimension! Beta Censoring will check the available images and find one that's aspect ratio is _close enough_ to the censoring it's being used for. While we recommend sticking mostly to square-ish images, you don't need to worry too much about the exact dimensions.

### Sticker Packs

You will sometimes find pre-prepared sticker packs. These are special archive files with the `.betapkg` file extension. You can drop these right into the server folder (i.e. where `BetaCensor.Server.exe` is) and the server will load any stickers contained in the pack when it starts.

To get you started, here's a couple of basic sticker packs:

|Pack|Author|Description|Download|
|:--|:--|:--|:-:|
|Emoji|`silveredgold`|A simple pack of emojis. Mostly square, with some mosaics for wider matches.|[Download Here](https://static.betaplatform.app/stickers/Emoji.betapkg)|
|Professional|`finalfrog`|A more "professional" or "corporate" set of stickers with a mix of square and wide stickers.|[Download Here](https://static.betaplatform.app/stickers/Professional.betapkg)|
|Labels|`silveredgold`|A simple set of censoring-themed stylized text labels in mostly wider shapes, particularly useful if your other stickers are mostly square.|[Download Here](https://static.betaplatform.app/stickers/Labels.betapkg)|


### Startup Mode

By default, Beta Censoring will run in its "Normal" mode which tries to speed up sticker loading by pre-loading your available stickers into memory. This meaks it much faster to apply sticker censoring at the cost of increased memory usage and startup speed. In our ever-present goal for customization, this is configurable. You can use your configuration file to set the sticker startup into either "Hybrid" or "Fast" mode.

<CodeGroup>
  <CodeGroupItem title="YAML" active>

```yaml
Stickers:
  StartupMode: Hybrid
```

  </CodeGroupItem>

  <CodeGroupItem title="JSON">

```json
{
    "Stickers": {
        "StartupMode": "Hybrid"
    }
}
```

  </CodeGroupItem>
</CodeGroup>

Your options are summarized below:

- **`Normal`** (default): This mode takes longer to start up and increases memory consumption, but makes censoring (when using stickers) much faster. Adding/removing stickers requires restarting the server.
- **`Hybrid`**: This mode still pre-loads stickers, but does so a little bit faster. This mode is a little faster to start up at the cost of slightly slower censoring (when using stickers). Adding/removing stickers requires restarting the server.
- **`Fast`**: This mode does _not_ pre-load stickers at all. This means it is the fastest to start up and uses significantly less memory, but is noticeably slower to censor (when using stickers). Adding/removing stickers does not require restarting the server.

I recommend leaving the server in Normal mode unless you are really struggling for memory and are willing to accept a penalty in censoring time when using stickers.


## Captions Configuration

> Beta Censoring includes a default set of captions so you only need to do this if you want to change them.

### Simple Method

The simplest method if you just want to change which captions are shown by default, you can add a `captions.txt` file in the same folder as the server, with one caption per line. These will *replace* the default captions.

### Advanced Method

If you want a bit more control, or have a more complex set-up in mind, you can configure captions from the configuration file. Captions work similarly to stickers to configure, but with a `Captions` configuration section that looks a little different:

<CodeGroup>
  <CodeGroupItem title="YAML" active>

```yaml
Captions:
  Captions:
    - "unworthy"
    - "not for you"
  FilePaths:
    - "D:/captions.txt"
```

  </CodeGroupItem>

  <CodeGroupItem title="JSON">

```json
{
    "Captions": {
        "Captions": ["unworthy", "not for you"],
        "FilePaths": ["D:/captions.txt"]
    }
}
```

  </CodeGroupItem>
</CodeGroup>

In short, you can include the captions directly in the configuration file (as a list in the `Captions` property) or include any number of text files where each line of the text file is one caption (as a list of paths in the `FilePaths` property).

While the API supports it, the current default caption provider ignores requested categories.

## Censoring Configuration

You can also fine-tune the censoring settings used by all clients using the server configuration file. You can separately adjust the minimum confidence (between 0 and 1) for Beta Censoring to censor a result from the AI, as well as tune the scores required for individual matches ('classes' as the AI calls them). For example, the default scores range from 0.4 to 0.55 (the default) up to 0.6 for some of the trickier covered classes. If you want to make the censoring more trigger-happy overall, you can adjust the `MinimumScore` option, like below:

<CodeGroup>
  <CodeGroupItem title="YAML" active>

```yaml
MatchOptions:
  MinimumScore: 0.4
```

  </CodeGroupItem>

  <CodeGroupItem title="JSON">

```json
{
    "MatchOptions": {
        "MinimumScore": 0.4
    }
}
```

  </CodeGroupItem>
</CodeGroup>

or if you just want to make it _much_ more likely to censor specifically breasts (either covered or exposed) you could use the following:

<CodeGroup>
  <CodeGroupItem title="YAML" active>

```yaml
MatchOptions:
  ClassScores:
    EXPOSED_BREAST_F: 0.35
    COVERED_BREAST_F: 0.45
```

  </CodeGroupItem>

  <CodeGroupItem title="JSON">

```json
{
    "MatchOptions": {
        "ClassScores": {
          "EXPOSED_BREAST_F": 0.35,
          "COVERED_BREAST_F": 0.45
        }
    }
}
```

  </CodeGroupItem>
</CodeGroup>

You can provide class-specific scores for any of the [classes supported by the NudeNet model](https://github.com/notAI-tech/NudeNet#nudenet-neural-nets-for-nudity-classification-detection-and-selective-censoring).

## Server Configuration

If you're looking to tweak how the server itself works, the most useful configuration block is the `Server` block. When not specified, the default is equivalent to the following:

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

### Worker Configuration

It may be tempting to dramatically increase the `WorkerCount` setting to get more workers censoring images at once, but this is not usually a good idea.

Increasing the worker count will **dramatically** increase the load on your PC while censoring. Additionally, adding more workers actually has the potential to _slow down_ censoring, especially if you add more workers than your PC can reasonably run at once. A reasonable rule of thumb is to use half the number of cores your CPU has. If you're okay with things *really* slowing down during censoring, you can try going as high as the number of cores, but it's **strongly recommended** to not go above this number.

### Optimization Mode

Beta Censoring includes a bunch of built-in optimization behaviour to try and keep performance acceptable. The defaults should be a reasonable balance of speed to accuracy, but you can also tell Beta Censoring to either be more relaxed (slower, but more accurate) or more aggressive (faster, but less consistent).

In your configuration you can set the optimization mode to one of the following:

- `None`: Skips most optimizations. Slower, but more consistent and accurate
- `Normal` (default): Adds some pre-processing to slightly improve AI performance without sacrificing too much accuracy
- `Aggressive`: Applies heavier pre-processing and inference to significantly increase the AI performance but with a much higher risk of missed or inaccurate matches.

In your configuration, you can set the optimization mode to one of the above modes if you want to override the default.

<CodeGroup>
  <CodeGroupItem title="YAML" active>

```yaml
Server:
  OptimizationMode: Aggressive 
```

  </CodeGroupItem>

  <CodeGroupItem title="JSON">

```json
{
    "Server": {
        "OptimizationMode": "Aggressive"
    }
}
```

  </CodeGroupItem>
</CodeGroup>

### Scaling Configuration

There is one additional configuration item you might be interested in: image rescaling. When Beta Censoring actually runs an image through the AI, it will (if necessary) scale the image down a little to speed things up. By default, though, it will only scale anything larger than 1080p. This default should result in reasonably accurate matching from the AI with reasonable performance. 

If you're okay with less accurate matching and want faster processing, you can tell Beta Censoring to scale images down before processing them. That will result in faster but less accurate results from the AI. To do so, change the `ImageDimensions` option in the server config:

<CodeGroup>
  <CodeGroupItem title="YAML" active>

```yaml
Server:
  ImageDimensions: "1280x720" # note the quotes are required!
```

  </CodeGroupItem>

  <CodeGroupItem title="JSON">

```json
{
    "Server": {
        "ImageDimensions": "1280x720"
    }
}
```

  </CodeGroupItem>
</CodeGroup>

This would result in all images being scaled down to 720p before being handed to the AI. You can specify either a set of `<WIDTH>x<HEIGHT>` dimensions (as above) or just a single number for both.

> Note that censoring (and therefore the final censored image) will always be on the full-size image.

## Caching Configuration

Beta Censoring from v0.0.10 onwards includes support for caching results from the AI and censoring process. This means that if you request the same image to be censored, the backend can just grab the results from the cache and return them straight away rather than running the image through the AI and censoring processes again.

Caching is split into two parts: the AI cache (aka the results cache) and the censoring cache. The AI cache just caches the matches that get returned from the AI model (i.e. what body parts are where), while the censoring cache will cache the entire censored image. You can individually toggle these caches so that you can have the AI results cached but still get images newly censored each time (great for stickers and captions users) or enable the censoring cache as well for the fastest possible performance.

Note that enabling either cache will increase the server's memory usage, but this is especially noticeable for the censoring cache which will hold every image you have censored in the last 15 minutes in memory, which can use a lot of memory if you browse a lot of images.

To enable/disable caching, use the separate `Caching` section of the configuration file:

<CodeGroup>
  <CodeGroupItem title="YAML" active>

```yaml
Caching:
  EnableMatchCaching: true
  EnableCensoringCaching: false
```

  </CodeGroupItem>

  <CodeGroupItem title="JSON">

```json
{
    "Caching": {
        "EnableMatchCaching": true,
        "EnableCensoringCaching": false
    }
}
```

  </CodeGroupItem>
</CodeGroup>

## Other Configuration

There is also some extra configuration options to tweak the censoring behaviour available in the `CensorOptions` section that let you globally control some parts of the matching/censoring process. Of particular note:

- `RelativeCensorScale`: This will globally scale the size of matches from the AI up or down. A value of `1` means that censoring will only apply to the area indicated by the AI, while (for example) `1.2` will include an area 20% larger in each direction.
- `PaddingScale`: This can be used to increase/decrease the padding area around matches used by the censoring process. We don't recommend you change this.
- `ForcePixelBackground`: By default, stickers will be applied on top of a blur effect. Setting this to `true` will instead pixelate the area under the sticker. This effect is necessary as the server has no way of knowing whether or not the sticker is *actually* covering the area that is to be censored, so we blur or pixelate the area *then* apply the sticker on top.
- `ClassStrength`: This is currently only used by the blur and pixelation censor types, but can be used to increase the strength of those effects for specific classes (i.e body parts). This is *multiplicative* with the censor level requested by the client, so be careful with it.

<CodeGroup>
  <CodeGroupItem title="YAML" active>

```yaml
# all values are optional
CensorOptions:
  RelativeCensorScale: 1.1
  ForcePixelBackground: true
  ClassStrength:
    FACE_F: 0.9
```

  </CodeGroupItem>

  <CodeGroupItem title="JSON">

```json
{
    "CensorOptions": {
        "RelativeCensorScale": 1.1,
        "ForcePixelBackground": true,
        "ClassStrength": {
          "EXPOSED_BREAST_F": 1.2
        }
    }
}
```

  </CodeGroupItem>
</CodeGroup>