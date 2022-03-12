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

## Customisation

This is a big one so hard to explain succinctly, but essentially every part of the Beta Censoring experience is more configurable and adaptable than it's counterpart in Beta Safety. 

#### Server

You can customise more of how the server works than with Beta Safety including enabling/disabling entire integrations (like the WebSockets used by Beta Protection or the REST API), though in its current form this isn't always the easiest since it requires configuration files.

#### Clients

Beta Censoring offers very fine-grained control of censoring so that clients can offer the most options. As an isolated example, clients can specify not just the type of censoring used, but also the intensity of it for **any** combination of body part and censoring style (even black bars!). Additionally, Beta Censoring makes less assumptions about clients, and is even designed to support any client apps, not just its own!

## Incompatibilities

This is one area where (at the time of writing) Beta Safety absolutely wins. There are three major features you should know that Beta Censoring does not support at this time:

  - Hardware acceleration with GPUs
  - Censoring for the eyes
  - Sticker censoring (supported but a bit difficult to set up)