using JetBrains.Annotations;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Intersect.Core;

namespace Intersect.Plugins.Loaders
{

    internal partial class PluginLoader
    {

        internal void LoadConfigurations(
            [NotNull] IApplicationContext applicationContext,
            [NotNull] IEnumerable<Plugin> plugins
        ) => plugins.ToList()
            .ForEach(
                plugin =>
                {
                    if (plugin != null)
                    {
                        plugin.Configuration = LoadConfiguration(applicationContext, plugin);
                    }
                }
            );

        [NotNull]
        internal PluginConfiguration LoadConfiguration(
            [NotNull] IApplicationContext applicationContext,
            [NotNull] Plugin plugin
        )
        {
            var configurationFilePath = plugin.Reference.ConfigurationFile;
            var configuration = new PluginConfiguration {IsEnabled = true};
            if (File.Exists(configurationFilePath))
            {
                var configurationFileContents = File.ReadAllText(configurationFilePath, Encoding.UTF8);
                try
                {
                    configuration = JsonConvert.DeserializeObject(
                                        configurationFileContents, plugin.Reference.ConfigurationType,
                                        new JsonSerializerSettings
                                        {
                                            DefaultValueHandling = DefaultValueHandling.Include
                                        }
                                    ) as PluginConfiguration ??
                                    configuration;
                }
                catch (Exception exception)
                {
                    applicationContext.Logger.Warn(
                        exception,
                        $"Failed to load plugin configuration from '{configurationFilePath}', using default values."
                    );
                }
            }

            try
            {
                File.WriteAllText(configurationFilePath, JsonConvert.SerializeObject(configuration, Formatting.Indented), Encoding.UTF8);
            }
            catch (Exception exception)
            {
                applicationContext.Logger.Warn(
                    exception, $"Failed to save plugin configuration to '{configurationFilePath}'."
                );
            }

            return configuration;
        }

    }

}
