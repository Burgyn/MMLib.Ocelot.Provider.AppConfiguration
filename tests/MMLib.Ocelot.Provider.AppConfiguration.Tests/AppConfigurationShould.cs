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
        public async Task GetServiceByNameAsync(string serviceName, string downstreamPath)
        {
            IConfiguration configuration = GetConfiguration();

            var appConfiguration = new AppConfiguration(
                configuration,
                new DownstreamRouteBuilder().WithServiceName(serviceName).Build(),
                new ServiceProviderConfiguration("", "", "", 1, "", "", 1),
                new MemoryCache(new MemoryCacheOptions()),
                Substitute.For<IOcelotLoggerFactory>());

            Service service = (await appConfiguration.GetAsync()).First();

            var uri = new UriBuilder(
                service.HostAndPort.Scheme,
                service.HostAndPort.DownstreamHost,
                service.HostAndPort.DownstreamPort).Uri;

            uri.Should().Be(downstreamPath);
        }

        [Fact]
        public async Task CachedServiceAsync()
        {
            IConfiguration configuration = GetConfiguration();

            var appConfiguration = new AppConfiguration(
                configuration,
                new DownstreamRouteBuilder().WithServiceName("Users").Build(),
                new ServiceProviderConfiguration("", "", "", 1, "", "", 300000),
                new MemoryCache(new MemoryCacheOptions()),
                Substitute.For<IOcelotLoggerFactory>());

            Service service = (await appConfiguration.GetAsync()).First();
            Service service2 = (await appConfiguration.GetAsync()).First();

            service.Should().Be(service2);
        }

        [Fact]
        public async Task ReturnEmptyListIfServiceDoesntExistAsync()
        {
            IConfiguration configuration = GetConfiguration();

            var appConfiguration = new AppConfiguration(
                configuration,
                new DownstreamRouteBuilder().WithServiceName("Service1").Build(),
                new ServiceProviderConfiguration("", "", "", 1, "", "", 300000),
                new MemoryCache(new MemoryCacheOptions()),
                Substitute.For<IOcelotLoggerFactory>());

            List<Service> services = await appConfiguration.GetAsync();

            services.Should().BeEmpty();
        }

        [Fact]
        public async Task GetServicesFromAnotherSectionAsync()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile("ocelot.json")
                .Build();

            var appConfiguration = new AppConfiguration(
                configuration,
                new DownstreamRouteBuilder().WithServiceName("ToDos").Build(),
                new ServiceProviderConfiguration("", "", "", 1, "", "", 300000),
                new MemoryCache(new MemoryCacheOptions()),
                Substitute.For<IOcelotLoggerFactory>());

            Service service = (await appConfiguration.GetAsync()).First();

            service.HostAndPort.DownstreamPort.Should().Be(9004);
        }

        [Fact]
        public async Task GetNewInstanceIfCacheExpiredAsync()
        {
            IConfiguration configuration = GetConfiguration();
            var cache = new MemoryCache(new MemoryCacheOptions());

            var appConfiguration = new AppConfiguration(
                configuration,
                new DownstreamRouteBuilder().WithServiceName("Users").Build(),
                new ServiceProviderConfiguration("", "", "", 1, "", "", 300000),
                cache,
                Substitute.For<IOcelotLoggerFactory>());

            Service service = (await appConfiguration.GetAsync()).First();
            cache.Remove("Service_users");
            Service service2 = (await appConfiguration.GetAsync()).First();

            service.Should().NotBe(service2);
        }

        private static IConfiguration GetConfiguration() =>
            new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
    }
}
