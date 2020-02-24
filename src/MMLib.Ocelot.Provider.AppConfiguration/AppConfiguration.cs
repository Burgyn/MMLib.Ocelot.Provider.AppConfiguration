using Microsoft.Extensions.Configuration;
using Ocelot.Configuration;
using Ocelot.ServiceDiscovery.Providers;
using Ocelot.Values;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMLib.Ocelot.Provider.AppConfiguration
{
    /// <summary>
    /// Provider for getting services from app configuration.
    /// </summary>
    /// <seealso cref="Ocelot.ServiceDiscovery.Providers.IServiceDiscoveryProvider" />
    public class AppConfiguration : IServiceDiscoveryProvider
    {
        private readonly IConfiguration _configuration;
        private readonly DownstreamReRoute _downstreamReRoute;
        private readonly ServiceProviderConfiguration _providerConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppConfiguration"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="downstreamReRoute">The downstream re route.</param>
        /// <param name="providerConfiguration">The provider configuration.</param>
        public AppConfiguration(
            IConfiguration configuration,
            DownstreamReRoute downstreamReRoute,
            ServiceProviderConfiguration providerConfiguration)
        {
            _configuration = configuration;
            _downstreamReRoute = downstreamReRoute;
            _providerConfiguration = providerConfiguration;
        }

        /// <summary>
        /// Gets services.
        /// </summary>
        public Task<List<Service>> Get() =>
            Task.FromResult(_configuration
                .GetSection("Services")
                .GetChildren()
                .Where(s => s.Key.Equals(_downstreamReRoute.ServiceName, System.StringComparison.OrdinalIgnoreCase))
                .Select(s =>
                {
                    ServiceConfiguration service = s.Get<ServiceConfiguration>();
                    service.Name = s.Key;
                    return service.ToService();
                }).ToList());
    }
}
