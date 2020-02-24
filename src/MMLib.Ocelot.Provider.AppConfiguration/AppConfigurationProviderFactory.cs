using Microsoft.Extensions.Configuration;
using Ocelot.Configuration;
using Ocelot.ServiceDiscovery;
using Ocelot.ServiceDiscovery.Providers;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MMLib.Ocelot.Provider.AppConfiguration
{
    public static class AppConfigurationProviderFactory
    {
        public static ServiceDiscoveryFinderDelegate Get = (provider, config, reRoute) =>
        {
            return GetProvider(provider, config, reRoute);
        };

        private static IServiceDiscoveryProvider GetProvider(IServiceProvider provider, ServiceProviderConfiguration config, DownstreamReRoute reRoute)
        {
            IConfiguration configuration = provider.GetService<IConfiguration>();

            return new AppConfiguration(configuration, reRoute);
        }
    }
}
