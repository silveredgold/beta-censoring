---
title: 'Performance'
---

> Note that in the preview releases, configuring the server is an advanced process that assumes some familiarity with editing configuration files yourself.
> This process will hopefully improve in future releases so it is only recommended to proceed if you are comfortable with that, and really need to.

## Server Performance

Performance for something as complex as Beta Censoring is not an easy topic to discuss, but this will hopefully serve as a reasonably approachable description of where the real performance impacts in Beta Censoring are, and what you can do to improve performance for either real-time or batch/sequential censoring.

### Checking Server Performance

The Beta Censoring server includes a basic web dashbaord to give a rough idea of the performance of the server. To check it, just open your browser and navigate to [`http://beta-censoring.local:2382/`](http://beta-censoring.local:2382/) and you should see a simple status panel to check the details of your server. Click the Performance tab to view a quick summary of performance metrics.

> If that URL doesn't work, try directly navigating to the server URL. For example, that would be [`http://localhost:2382`](http://localhost:2382) on the PC you're running the server on.

This page will give you a **very** rough breakdown of the performance metrics for recent requests for the current server session. Of particular note it will give you some indicators for _censoring time_ and _inference time_.

_Inference time_ refers to the time it takes for the server to load a requested image, convert it for the AI to handle and actually run it through the AI. This just includes the steps required to load the image and for the AI to classify the matches in the image.

_Censoring time_ refers to the time required to actually censor the results returned by the AI. This includes transforming the results (where necessary) and applying the invididual censoring effects as requested. Given the number of steps involved in this part, this time is currently returned as a single time for the whole censoring step.

## Performance Impact Guide

### Workers

As covered in the [Configuration](./configuration.md) guide, you can control how many workers the server uses to censor images using the configuration files. Make sure you read that document in full as there are real risks and tradeoffs involved in increasing the default worker count.

### Censor Types

Beta Censoring makes any of the censor types in CensorCore available for clients to request, which includes (at least) `Blur`, `BlackBar`, `Caption`, `Pixelation` and `Sticker`. That being said, some of these are more intensive than others to actually apply to an image. In particular, black bar will generally be the fastest while stickers will generally be the slowest with most of the others between those two. In particular, if you're noticing the _censoring time_ increasing but not the _inference time_ then you may want to consider using less intensive censoring types.

### Image Size

As you'd expect, the larger an image the harder it is to both detect matches and censor those matches. Beta Censoring will always scale large images down for the AI (to improve performance) but will then apply censoring to the full-size image and return the censored images at full resolution. This improves quality but does slow the server down when censoring very large images.

If you're seeing significant performance degradation with large images, raise an issue on GitHub and we will look into methods to improve this.

### Experimental Features

Some of the features exposed by Beta Censoring are *experimental* features that we have included for those who want to use them but may impact performance.

#### Facial Feature Detection

Facial feature detection uses its own model and middleware to identify specific facial features when faces are detected. Note that facial feature detection will only be run when clients specifically request `EYES_F` or `MOUTH_F` censoring in the request. Including these will enable feature detection which can noticeably increase censoring time.