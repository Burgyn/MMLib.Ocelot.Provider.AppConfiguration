using Microsoft.Extensions.DependencyInjection;
using Ocelot.DependencyInjection;

namespace MMLib.Ocelot.Provider.AppConfiguration
{
    public static class OcelotBuilderExtensions
    {
        public static IOcelotBuilder AddAppConfiguration(this IOcelotBuilder builder)
        {
            builder.Services.AddSingleton(AppConfigurationProviderFactory.Get);
            return builder;
        }
    }
}
