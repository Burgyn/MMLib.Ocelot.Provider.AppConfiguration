using Microsoft.Extensions.Configuration;
using Ocelot.ServiceDiscovery;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using Ocelot.Logging;

namespace MMLib.Ocelot.Provider.AppConfiguration
{
    /// <summary>
    /// Factory for creating <see cref="AppConfiguration"/> provider.
    /// </summary>
    public static class AppConfigurationProviderFactory
    {
        /// <summary>
        /// Get <see cref="AppConfiguration"/> provider.
        /// </summary>
        public static ServiceDiscoveryFinderDelegate Get = (provider, config, reRoute) =>
        {
            IConfiguration configuration = provider.GetService<IConfiguration>();
            IMemoryCache cache = provider.GetService<IMemoryCache>();
            IOcelotLoggerFactory loggerFactory = provider.GetService<IOcelotLoggerFactory>();

            return new AppConfiguration(configuration, reRoute, config, cache, loggerFactory);
        };
    }
}
