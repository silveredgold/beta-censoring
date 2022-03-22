# Usage

In it's current form, you don't really *use* Beta Censoring in the traditional sense. You leave it running and other apps (we call them 'clients') can interact with it, requesting images to be censored.

That does mean that you need to leave it running though, as clients will need to contact the running server to request censoring.

> If you're a more advanced user, Beta Censoring can be configured as a Windows Service or Systemd service to have the server running, but not visible.

There is a simple web interface available to check on the status of a running Beta Censoring server though. To check it, just open your browser and navigate to [`http://beta-censoring.local:2382/`](http://beta-censoring.local:2382/) and you should see a simple status panel to check the details of your server. At present, this status panel is just to check on your server details, performance and configuration (though more features are planned).

> If that URL doesn't work, try directly navigating to the server URL. For example, that would be [`http://localhost:2382`](http://localhost:2382) on the PC you're running the server on.


## Clients

At this time, the only _known_ Beta Censoring client is **[Beta Protection](https://silveredgold.github.io/beta-protection/#/)**, the browser extension for live censoring images while you browse.

Beta Censoring can accept any client though, and if you're interested in integrating it with other tools or building your own client, the [developer documentation](./developers.md) includes more details.