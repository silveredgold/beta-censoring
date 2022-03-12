using System.Net;
using System.Net.NetworkInformation;
using Makaretu.Dns;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace BetaCensor.Server.Discovery;

public class DiscoveryService : IHostedService {
    private readonly IServer _server;
    private readonly IHostApplicationLifetime _lifetime;
    private readonly ILogger<DiscoveryService> _logger;
    private readonly IConfiguration configuration;
    private readonly MulticastService _mdns;
    private readonly ServiceDiscovery _serviceDiscovery;
    private readonly List<ServiceProfile> _profiles = new();

    public DiscoveryService(IServer server, IHostApplicationLifetime hostLifetime, ILogger<DiscoveryService> logger, IConfiguration configuration) {
        _server = server;
        _lifetime = hostLifetime;
        _logger = logger;
        this.configuration = configuration;
        _mdns = new MulticastService() {
            UseIpv4 = true,
            UseIpv6 = false
        };
        _mdns.QueryReceived += OnQueryReceived;
        _serviceDiscovery = new ServiceDiscovery(_mdns);
    }

    private void OnQueryReceived(object? sender, MessageEventArgs e) {
        var msg = e.Message;
        _logger.LogTrace($"Matching service request to profiles: {string.Join(';', msg.Questions.Select(q => q.Name))} -> {string.Join(';', _profiles.Select(p => p.HostName))}");
        var matchingService = _profiles.FirstOrDefault(p => msg.Questions.Any(q => q.Name == p.HostName));
        if (matchingService != null) {
            _logger.LogDebug("Found matching service! " + matchingService.HostName);
            var res = msg.CreateResponse();
            // var addresses = MulticastService.GetIPAddresses()
            //     .Where(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
            var addresses = GetLocalAddresses().ToList();
            foreach (var address in addresses) {
                res.Answers.Add(new ARecord {
                    Name = matchingService.HostName,
                    Address = address
                });
            }
            _mdns.SendAnswer(res);
        }
    }

    private void StartDiscovery() {
        var urls = GetServerAddresses();
        var dict = urls.GroupBy(a => a.Split(':').Last()).Where(g => int.TryParse(g.Key, out var _)).ToDictionary(k => int.Parse(k.Key), v => v.Select(a => a.Split(':').First()).Single());

        var addresses = GetLocalAddresses();
        foreach (var port in dict) {
            _logger.LogDebug($"Creating service profile for '{port.Value}/{port.Key}'");
            var http = new LocalServiceProfile("beta-censoring", $"_{port.Value}._tcp.", (ushort)port.Key, addresses);
            // var raw = new ServiceProfile($"{Environment.MachineName}-{port.Value}", "_censoring._tcp", (ushort)port.Key, addresses);

            // var normal = new ServiceProfile("beta-censoring-server", $"_{port.Value}._tcp.", (ushort)port.Key, addresses);
            _profiles.Add(http);
        }
        var basePort = dict.FirstOrDefault(p => p.Value == "http");
        if (dict.Any(p => p.Value == "http") && IsApiEnabled()) {
            var httpPort = dict.First(p => p.Value == "http");
            var proto = new ServiceProfile($"{Environment.MachineName}", $"_censoring._tcp", (ushort)httpPort.Key, addresses);
            foreach (var scheme in dict) {
                proto.AddProperty(scheme.Value, scheme.Key.ToString());
            }
            _profiles.Add(proto);
        }

        _logger.LogInformation($"Advertising {_profiles.Count} service profiles!");
        foreach (var profile in _profiles) {
            _serviceDiscovery.Advertise(profile);
        }
        _mdns.Start();
    }

    private bool IsApiEnabled() {
        try {
            var conf = configuration.GetSection("Server").Get<ServerOptions>() ?? new ServerOptions();
            return conf.EnableRest || conf.EnableSignalR;
        }
        catch {
            return true;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) {
        _logger.LogInformation("Unadvertising service profile!");
        // sd.Unadvertise(_profile);
        foreach (var profile in _profiles) {
            _serviceDiscovery.Unadvertise(profile);
        }
        _mdns.Stop();
        return Task.CompletedTask;
    }

    private IEnumerable<string> GetServerAddresses() {
        var features = _server.Features;
        var addresses = features.Get<IServerAddressesFeature>();
        return addresses?.Addresses ?? new List<string>();
    }

    public Task StartAsync(CancellationToken cancellationToken) {
        _lifetime.ApplicationStarted.Register(StartDiscovery);
        return Task.CompletedTask;
    }

    private IEnumerable<IPAddress> GetLocalAddresses() {
        {
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces()) {
                var addr = ni.GetIPProperties().GatewayAddresses.FirstOrDefault();
                if (addr != null && !addr.Address.ToString().Equals("0.0.0.0")) {
                    if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet) {
                        foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses) {
                            if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) {
                                yield return ip.Address;
                            }
                        }
                    }
                }
            }
        }
    }
}