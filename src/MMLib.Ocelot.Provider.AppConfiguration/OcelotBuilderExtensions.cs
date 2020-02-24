using Microsoft.Extensions.DependencyInjection;
using Ocelot.DependencyInjection;

namespace MMLib.Ocelot.Provider.AppConfiguration
{
    /// <summary>
    /// <see cref="IOcelotBuilder"/> extensions.
    /// </summary>
    public static class OcelotBuilderExtensions
    {
        /// <summary>
        /// Adds the <see cref="AppConfiguration"/> provider.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public static IOcelotBuilder AddAppConfiguration(this IOcelotBuilder builder)
        {
            builder.Services.AddSingleton(AppConfigurationProviderFactory.Get);
            return builder;
        }
    }
}
