using Ocelot.Values;
using System;

namespace MMLib.Ocelot.Provider.AppConfiguration
{
    internal class ServiceConfiguration
    {
        public string Name { get; set; }

        public string DownstreamPath { get; set; }

        public Service ToService()
        {
            var uri = new Uri(DownstreamPath);

            return new Service(
                Name,
                new ServiceHostAndPort(uri.Host, uri.Port, uri.Scheme),
                string.Empty,
                string.Empty,
                new string[0]);
        }
    }
}
