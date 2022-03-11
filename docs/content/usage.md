# Usage

In it's current form, you don't really *use* Beta Censoring in the traditional sense. You won't directly interact with it and it doesn't (currently) have its own interface. The current version of Beta Censoring is explicitly a **server**. That means you leave it running and other apps (we call them 'clients') can interact with it, requesting images to be censored.

That does mean that you need to leave it running though, as clients will need to contact the running server to request censoring.

> If you're a more advanced user, Beta Censoring can be configured as a Windows Service or Systemd service to have the server running, but not visible.

## Clients

At this time, the only _known_ Beta Censoring client is **[Beta Protection](https://silveredgold.github.io/beta-protection/#/)**, the browser extension for live censoring images while you browse.

Beta Censoring can accept any client though, and if you're interested in integrating it with other tools or building your own client, the [developer documentation](./developers.md) includes more details.