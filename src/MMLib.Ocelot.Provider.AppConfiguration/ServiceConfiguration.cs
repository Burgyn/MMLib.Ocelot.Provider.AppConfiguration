using Ocelot.Values;
using System;

namespace MMLib.Ocelot.Provider.AppConfiguration
{
    internal class ServiceConfiguration
    {
        public string Name { get; set; }

        public string DownstreamPath { get; set; }

        public Service ToService() =>
            new Service(
                Name,
                GetServiceHostAndPort(),
                string.Empty,
                string.Empty,
                new string[0]);

        private ServiceHostAndPort GetServiceHostAndPort()
        {
            var uri = new Uri(DownstreamPath);

            return new ServiceHostAndPort(uri.Host, uri.Port, uri.Scheme);
        }
    }
}
