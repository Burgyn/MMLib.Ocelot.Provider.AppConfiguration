using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.Configuration;
using Ocelot.Configuration.Builder;
using Ocelot.ServiceDiscovery.Providers;
using Xunit;

namespace MMLib.Ocelot.Provider.AppConfiguration.Tests
{
    public class AppConfigurationProviderFactoryShould
    {
        [Fact]
        public void CreateProvider()
        {
            var services = new ServiceCollection();
            services.AddSingleton(new ConfigurationBuilder().Build());
            services.AddMemoryCache();

            var factory = AppConfigurationProviderFactory.Get;

            IServiceDiscoveryProvider provider = factory(
                services.BuildServiceProvider(),
                new ServiceProviderConfiguration("", "", 1, "", "", 1),
                new DownstreamReRouteBuilder().WithServiceName("Users").Build());

            provider.Should().NotBeNull();
        }
    }
}
