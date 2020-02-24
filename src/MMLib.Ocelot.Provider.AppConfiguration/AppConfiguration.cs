using Microsoft.Extensions.Configuration;
using Ocelot.Configuration;
using Ocelot.ServiceDiscovery.Providers;
using Ocelot.Values;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMLib.Ocelot.Provider.AppConfiguration
{
    public class AppConfiguration : IServiceDiscoveryProvider
    {
        private readonly IConfiguration _configuration;
        private readonly DownstreamReRoute _downstreamReRoute;
        private readonly ServiceProviderConfiguration _providerConfiguration;

        public AppConfiguration(
            IConfiguration configuration,
            DownstreamReRoute downstreamReRoute,
            ServiceProviderConfiguration providerConfiguration)
        {
            _configuration = configuration;
            _downstreamReRoute = downstreamReRoute;
            _providerConfiguration = providerConfiguration;
        }

        public Task<List<Service>> Get() =>
            Task.FromResult(_configuration
                    .GetSection("Services")
                    .GetChildren()
                    .Where(s => s.Key == _downstreamReRoute.ServiceName)
                    .Select(s =>
                    {
                        ServiceConfiguration service = s.Get<ServiceConfiguration>();
                        service.Name = s.Key;
                        return service.ToService();
                    }).ToList());
    }
}
