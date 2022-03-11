# Introduction

Beta Censoring is a simple(ish) app for on-demand detection and censoring of NSFW images, designed especially for betas.

Beta Censoring itself is something of a middleman. Under all the covers, Beta Censoring uses the NudeNet AI model to classify images on demand then returns censored versions of the same images. Essentially, ***something*** requests an image to be censored (more on the _something_ below), Beta Censoring will load the image, run some prep work then run it through the open-source NudeNet model to detect specific body parts or features. Then, it will censor the image according to the results from the AI and the supplied preferences and return a censored version of the original image.

## FAQ

### What can use it?

Beta Protection is designed from the ground up to be a flexible bells-and-whistles-included solution for classifying and censoring images from anywhere. That being said, the most common use case is in conjunction with the Beta Protection Chrome extension. To be clear, though, the two apps are not strictly tied together:

  - Beta Protection can work with backends other than Beta Censoring (notably including the unrelated proprietary Beta Safety app)
  - Beta Censoring just exposes a set of APIs over HTTP and WebSockets so that any app can use it, not just Beta Protection.

### Is it stable and/or finished?

**Kind of** and **no**, respectively. It will definitely work and I wouldn't call it unstable, but there's no way around the fact this is an early preview version of Beta Censoring so there's a good chance you might spot a few bugs.

As for completion, not even close. To be clear, _it currently works_, there's just a lot more I intend to do with the project. Some parts (like configuration) are harder than they need to be, and it's more dependent on client apps (like Beta Protection) than it should be. Additionally, there's a few features that got put in the "get-back-to-that" bucket that need to be finished (like sticker support).

### How is this different from Beta Safety?

That's a big and complicated question with a big and complicated answer, so check the details [here](content/beta-safety.md).