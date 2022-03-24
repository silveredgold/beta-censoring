# Beta Censoring and Beta Safety

## Introduction

If you're a current Beta Safety user, your first thought on seeing this was probably "Isn't this what Beta Safety already does?" and yes, that's largely true, but it's a bit more nuanced than that.

For many Beta Safety users, your current experience is probably fine, and if that's the case then feel free to stick with it! Beta Safety and Beta Censoring each have their strengths and weaknesses, so feel free to make your own decision.

> The points discussed below (except where it says otherwise) are generally referring to the "backend" part of Beta Safety, not specifically the Chrome extension. If you're interested in a comparison of the extension, see [this page](https://silveredgold.github.io/beta-protection/#/guide/introduction#how-is-this-different-from-beta-safety) from the Beta Protection docs.

## Open Source and Extensible

Beta Safety is a closed-source proprietary tool. That means it does not accept community contributions, the underlying code is not publically available and there is no way for users to inspect, modify or otherwise change the app. In this author's opinion, that sucks. Open source software means you can always see the code and moving parts that make the app work, you can always tweak, modify or customise the app at will, and the community is free to contribute features, changes or fixes to the project. 

Beta Censoring (and all of its underlying components and libraries) are all open source under the GPLv3 license. For most users, this one is probably not hugely important, but it is to me, so here we are.

## Performance

Due to completely different stacks and some pretty significant changes in how they work, Beta Safety and Beta Censoring have extremely different performance impacts. Based on my own (admittedly minimal) testing, Beta Safety will usually be slightly faster in pure censoring time, but at the cost of _dramatically_ more memory usage and often more CPU usage. Beta Censoring, on the other hand, might be a _little_ slower per-image but uses a _fraction_ of the memory, and uses a little less CPU per-image.

> For some context here, on my testing rig, Beta Safety is just under a second faster to censor mid-size images, but uses ~3x the RAM

Additionally, since Beta Censoring is configurable, you can customise your own performance. If you're okay with slower censoring times for less performance impact, you can turn down the workers and caching options. Alternatively, if you have a beefy rig and don't mind your PC working a bit harder, add some more workers and caching, and revel in the faster response times.

You can find a ton more information on Beta Censoring's performance, including how to monitor and tweak it, in [the performance docs](./performance.md).

## Status and Monitoring

Beta Censoring includes its own built-in web interface for checking on the status of the server, providing (limited) information on requests, checking on loaded assets or monitoring the performance of the server and all of its components. While this won't always be useful for all users, it's a very easy way to check on the server, see what assets you have available and track what's taking the time in the censoring process.

## Customisation

This is a big one so hard to explain succinctly, but essentially every part of the Beta Censoring experience is more configurable and adaptable than it's counterpart in Beta Safety. 

#### Server

You can customise more of how the server works than with Beta Safety including enabling/disabling entire integrations (like the WebSockets used by Beta Protection or the REST API), though in its current form this isn't always the easiest since it requires configuration files.

You can also load assets (like stickers) from anywhere on your PC, no copying/moving required. Combine this with a more fine-grained control over censoring and you can tune the server to match how you want it to work.

#### Clients

Beta Censoring offers very fine-grained control of censoring so that clients can offer the most options. As an isolated example, clients can specify not just the type of censoring used, but also the intensity of it for **any** combination of body part and censoring style (even black bars!). Additionally, Beta Censoring makes less assumptions about clients, and is even designed to support any client apps, not just its own!

## Incompatibilities

There are some features from Beta Safety that Beta Censoring doesn't include, either a case of "not yet" or deliberate choice. The list below usually reflects the current state of that feature gap.

  - GPU Hardware acceleration
  - Domain-based statistics (not currently planned)
  - GIF Censoring support

#### GPU Acceleration

There's nothing preventing Beta Censoring from supporting GPU acceleration! The runtime we use (ONNX) has support for multiple forms of acceleration, but I simply don't have the right combination of OS and hardware to test it. If anyone can test and contribute support for acceleration (to Beta Censoring or CensorCore), I'll happily include it.

#### Domain-based statistics

While I am working on a statistics API (to match the existing Performance API), there is no plan for the API to include per-domain information. Storing the origin domains of all client requests turns Beta Censoring into a non-editable, real-time source of data on a user's browsing habits, something I don't feel entirely comfortable with it being. If this does get added, it will be up to clients if they want to provide a domain or not.

#### GIF and Video Support

Beta Censoring does not support censoring GIFs or videos. If you request a GIF to be censored, the server will censor the first frame and return it as a static image. As it stands, the NudeNet model is just nowhere near fast enough to support animations or videos with a vaguely acceptable user experience. Based on my current testing numbers, a 10-second low-resolution low-framerate GIF would take around 5 minutes to be censored. Hypothetically, a high-framerate 720p video just 15 seconds long would take almost an hour. As such, Beta Censoring chooses to only support static images.