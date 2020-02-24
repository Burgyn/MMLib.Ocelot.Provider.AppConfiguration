using Microsoft.Extensions.Configuration;
using Ocelot.Configuration;
using Ocelot.Configuration.Builder;
using System.Threading.Tasks;
using Xunit;
using System.Linq;
using Ocelot.Values;
using System;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using NSubstitute;
using Ocelot.Logging;

namespace MMLib.Ocelot.Provider.AppConfiguration.Tests
{
    public class AppConfigurationShould
    {
        [Theory]
        [InlineData("Users", "http://localhost:9003/")]
        [InlineData("users", "http://localhost:9003/")]
        [InlineData("projects", "http://localhost:9002/")]
        [InlineData("Projects", "http://localhost:9002/")]
        [InlineData("Authorization", "https://authorizationService.domain.com/")]
        public async Task LoadServiceByNameAsync(string serviceName, string downstreamPath)
        {
            IConfiguration configuration = GetConfiguration();

            var appConfiguration = new AppConfiguration(
                configuration,
                new DownstreamReRouteBuilder().WithServiceName(serviceName).Build(),
                new ServiceProviderConfiguration("", "", 1, "", "", 1),
                new MemoryCache(new MemoryCacheOptions()),
                Substitute.For<IOcelotLoggerFactory>());

            Service service = (await appConfiguration.Get()).First();

            var uri = new UriBuilder(
                service.HostAndPort.Scheme,
                service.HostAndPort.DownstreamHost,
                service.HostAndPort.DownstreamPort).Uri;

            uri.Should().Be(downstreamPath);
        }

        [Fact]
        public async Task CachedService()
        {
            IConfiguration configuration = GetConfiguration();

            var appConfiguration = new AppConfiguration(
                configuration,
                new DownstreamReRouteBuilder().WithServiceName("Users").Build(),
                new ServiceProviderConfiguration("", "", 1, "", "", 300000),
                new MemoryCache(new MemoryCacheOptions()),
                Substitute.For<IOcelotLoggerFactory>());

            Service service = (await appConfiguration.Get()).First();
            Service service2 = (await appConfiguration.Get()).First();

            service.Should().Be(service2);
        }

        [Fact]
        public async Task ReturnEmptyListIfServiceDoesntExist()
        {
            IConfiguration configuration = GetConfiguration();

            var appConfiguration = new AppConfiguration(
                configuration,
                new DownstreamReRouteBuilder().WithServiceName("Service1").Build(),
                new ServiceProviderConfiguration("", "", 1, "", "", 300000),
                new MemoryCache(new MemoryCacheOptions()),
                Substitute.For<IOcelotLoggerFactory>());

            List<Service> services = await appConfiguration.Get();

            services.Should().BeEmpty();
        }

        [Fact]
        public async Task GetNewInstanceIfCacheExpired()
        {
            IConfiguration configuration = GetConfiguration();
            var cache = new MemoryCache(new MemoryCacheOptions());

            var appConfiguration = new AppConfiguration(
                configuration,
                new DownstreamReRouteBuilder().WithServiceName("Users").Build(),
                new ServiceProviderConfiguration("", "", 1, "", "", 300000),
                cache,
                Substitute.For<IOcelotLoggerFactory>());

            Service service = (await appConfiguration.Get()).First();
            cache.Remove("Service_users");
            Service service2 = (await appConfiguration.Get()).First();

            service.Should().NotBe(service2);
        }

        private static IConfiguration GetConfiguration() =>
            new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                .Build();
    }
}
