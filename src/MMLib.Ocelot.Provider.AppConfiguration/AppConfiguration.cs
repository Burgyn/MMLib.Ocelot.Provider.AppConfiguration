using Microsoft.Extensions.Configuration;
using Ocelot.Configuration;
using Ocelot.ServiceDiscovery.Providers;
using Ocelot.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMLib.Ocelot.Provider.AppConfiguration
{
    public class AppConfiguration : IServiceDiscoveryProvider
    {
        private readonly IConfiguration _configuration;
        private readonly DownstreamReRoute _downstreamReRoute;

        public AppConfiguration(IConfiguration configuration, DownstreamReRoute downstreamReRoute)
        {
            _configuration = configuration;
            _downstreamReRoute = downstreamReRoute;
        }

        public Task<List<Service>> Get() =>
            Task.FromResult(_configuration
                    .GetSection("Services")
                    .GetChildren()
                    .Where(s => s.Key == _downstreamReRoute.ServiceName)
                    .Select(s =>
                    {
                        ServiceConf service = s.Get<ServiceConf>();
                        service.Name = s.Key;
                        return service.ToService();
                    }).ToList());
    }

    internal class ServiceConf
    {
        public string Name { get; set; }

        public string DownstreamPath { get; set; }

        public Service ToService()
        {
            Uri uri = new Uri(DownstreamPath);

            return new Service(
                Name,
                new ServiceHostAndPort(uri.Host, uri.Port, uri.Scheme),
                string.Empty,
                string.Empty,
                new string[0]);
        }
    }
}
