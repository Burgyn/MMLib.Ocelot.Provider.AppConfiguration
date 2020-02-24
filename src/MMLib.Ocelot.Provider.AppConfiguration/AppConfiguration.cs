using Microsoft.Extensions.Caching.Memory;
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
    /// <summary>
    /// Provider for getting services from app configuration.
    /// </summary>
    /// <seealso cref="Ocelot.ServiceDiscovery.Providers.IServiceDiscoveryProvider" />
    public class AppConfiguration : IServiceDiscoveryProvider
    {
        private readonly IConfiguration _configuration;
        private readonly string _serviceName;
        private readonly ServiceProviderConfiguration _providerConfiguration;
        private readonly IMemoryCache _cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppConfiguration"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="downstreamReRoute">The downstream re route.</param>
        /// <param name="providerConfiguration">The provider configuration.</param>
        /// <param name="cache">The cache.</param>
        public AppConfiguration(
            IConfiguration configuration,
            DownstreamReRoute downstreamReRoute,
            ServiceProviderConfiguration providerConfiguration,
            IMemoryCache cache)
        {
            _configuration = configuration;
            _serviceName = downstreamReRoute.ServiceName.ToLower();
            _providerConfiguration = providerConfiguration;
            _cache = cache;
        }

        /// <summary>
        /// Gets services.
        /// </summary>
        public Task<List<Service>> Get()
            => Task.FromResult(new List<Service>() { GetService() });

        private Service GetService()
        {
            if (!_cache.TryGetValue(GetKey(), out Service service))
            {
                service = GetServiceInner();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMilliseconds(_providerConfiguration.PollingInterval));

                _cache.Set(GetKey(), service, cacheEntryOptions);
            }

            return service;
        }

        private Service GetServiceInner() =>
            _configuration
                .GetSection("Services")
                .GetChildren()
                .Where(s => s.Key.Equals(_serviceName, System.StringComparison.OrdinalIgnoreCase))
                .Select(s =>
                {
                    ServiceConfiguration src = s.Get<ServiceConfiguration>();
                    src.Name = s.Key;
                    return src.ToService();
                }).FirstOrDefault();

        private string GetKey() => $"Service_{_serviceName}";
    }
}
