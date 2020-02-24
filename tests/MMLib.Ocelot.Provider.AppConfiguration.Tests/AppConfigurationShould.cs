using Microsoft.Extensions.Configuration;
using Ocelot.Configuration;
using Ocelot.Configuration.Builder;
using System.Threading.Tasks;
using Xunit;
using System.Linq;
using Ocelot.Values;
using System;
using FluentAssertions;

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
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

            var appConfiguration = new AppConfiguration(
                configuration,
                new DownstreamReRouteBuilder().WithServiceName(serviceName).Build(),
                new ServiceProviderConfiguration("", "", 1, "", "", 1));

            Service service = (await appConfiguration.Get()).First();

            var uri = new UriBuilder(
                service.HostAndPort.Scheme,
                service.HostAndPort.DownstreamHost,
                service.HostAndPort.DownstreamPort).Uri;

            uri.Should().Be(downstreamPath);
        }
    }
}
