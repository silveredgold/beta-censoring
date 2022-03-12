using System.Net;
using Makaretu.Dns;

namespace BetaCensor.Server.Discovery {
    public class LocalServiceProfile : ServiceProfile {
        public LocalServiceProfile(string instanceName, string serviceName, ushort port, IEnumerable<IPAddress> addresses = null, string hostName = null) : base() {
            InstanceName = instanceName;
            ServiceName = serviceName;
            var fqn = FullyQualifiedName;

            var simpleServiceName = new DomainName(ServiceName.ToString()
                .Replace("._tcp", "")
              .Replace("._udp", "")
              .TrimStart('_'));//allowing underscores again

            var domainName = DomainName.Join(InstanceName, Domain);
            //host name now defaulting to computer's host name.
            HostName = domainName;
            var localhost = hostName ?? (Environment.GetEnvironmentVariable("COMPUTERNAME") ?? Environment.GetEnvironmentVariable("HOSTNAME")) + "." + Domain;

            Resources.Add(new SRVRecord {
                Name = fqn,
                Port = port,
                Target = DomainName.Join(InstanceName, Domain)
            });
            Resources.Add(new TXTRecord
            {
                Name = fqn,
                Strings = { "txtvers=1" }
            });

            //removed default "txtvers=1" TXTRecord.

            foreach (var address in addresses ?? MulticastService.GetLinkLocalAddresses()) {
                Resources.Add(AddressRecord.Create(HostName, address));
            }
        }

    }
}