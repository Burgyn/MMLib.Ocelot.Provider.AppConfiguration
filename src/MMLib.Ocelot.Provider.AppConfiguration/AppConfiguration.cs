using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Ocelot.Configuration;
using Ocelot.Logging;
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
    public class AppConfiguration : IServiceDiscoveryProvider
    {
        private const int DefaultCacheExpirationInMinutes = 10;
        private const string DefaultServiceSectionName = "Services";
        private const string SectionNameConfigKey = "ServiceDiscoveryProvider:AppConfigurationSectionName";

        private readonly IConfiguration _configuration;
        private readonly string _serviceName;
        private readonly ServiceProviderConfiguration _providerConfiguration;
        private readonly IMemoryCache _cache;
        private readonly IOcelotLogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppConfiguration"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="downstreamReRoute">The downstream re route.</param>
        /// <param name="providerConfiguration">The provider configuration.</param>
        /// <param name="cache">The cache.</param>
        /// <param name="factory">The factory.</param>
        public AppConfiguration(
            IConfiguration configuration,
            DownstreamRoute downstreamReRoute,
            ServiceProviderConfiguration providerConfiguration,
            IMemoryCache cache,
            IOcelotLoggerFactory factory)
        {
            _configuration = configuration;
            _serviceName = downstreamReRoute.ServiceName.ToLower();
            _providerConfiguration = providerConfiguration;
            _cache = cache;
            _logger = factory.CreateLogger<AppConfiguration>();
        }

        /// <summary>
        /// Gets services.
        /// </summary>
        public Task<List<Service>> Get()
            => Task.FromResult(GetServices());

        private List<Service> GetServices()
        {
            if (!_cache.TryGetValue(GetKey(), out Service service))
            {
                service = GetServiceInner();

                if (service != null)
                {
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(GetExpiration());
                    _cache.Set(GetKey(), service, cacheEntryOptions);
                }
                else
                {
                    _logger.LogWarning(
                        $"Unable to use service '{_serviceName}' as it is invalid. Service is missing in configuration");
                    return new List<Service>();
                }
            }

            return new List<Service>() { service };
        }

        private TimeSpan GetExpiration()
            => _providerConfiguration.PollingInterval > 0
            ? TimeSpan.FromMilliseconds(_providerConfiguration.PollingInterval)
            : TimeSpan.FromMinutes(DefaultCacheExpirationInMinutes);

        private Service GetServiceInner() =>
            _configuration
                .GetSection(GetSectionName())
                .GetChildren()
                .Where(s => s.Key.Equals(_serviceName, System.StringComparison.OrdinalIgnoreCase))
                .Select(s =>
                {
                    ServiceConfiguration src = s.Get<ServiceConfiguration>();
                    src.Name = s.Key;
                    return src.ToService();
                }).FirstOrDefault();

        private string GetKey() => $"Service_{_serviceName}";

        private string GetSectionName() =>
            _configuration.GetValue<string>(SectionNameConfigKey) ?? DefaultServiceSectionName;
    }
}
