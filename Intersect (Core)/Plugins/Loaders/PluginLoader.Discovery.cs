using Intersect.Core;
using Intersect.Logging;

using JetBrains.Annotations;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Intersect.Plugins.Loaders
{
    internal partial class PluginLoader
    {
        [NotNull]
        public IDictionary<string, Plugin> DiscoverPlugins(
            [NotNull] IApplicationContext applicationContext,
            [NotNull] IEnumerable<string> pluginDirectories
        )
        {
            var discoveredPlugins = pluginDirectories.SelectMany(
                pluginDirectory =>
                {
                    Debug.Assert(pluginDirectory != null, nameof(pluginDirectory) + " != null");
                    return DiscoverPlugins(applicationContext, pluginDirectory) ?? Array.Empty<Plugin>();
                }
            );

            var plugins = new Dictionary<string, Plugin>();

            foreach (var discoveredPlugin in discoveredPlugins)
            {
                Debug.Assert(discoveredPlugin != null, $"{nameof(discoveredPlugin)} != null");
                var key = discoveredPlugin.Manifest.Key;
                if (!plugins.TryGetValue(discoveredPlugin.Manifest.Key, out var existingPlugin) ||
                    existingPlugin == default ||
                    existingPlugin.Manifest.Version < discoveredPlugin.Manifest.Version)
                {
                    plugins[key] = discoveredPlugin;
                }
            }

            return plugins;
        }

        [CanBeNull]
        private IEnumerable<Plugin> DiscoverPlugins(
            [NotNull] IApplicationContext applicationContext,
            [NotNull] string pluginDirectory
        )
        {
            if (Directory.Exists(pluginDirectory))
            {
                var plugins = new List<Plugin>();

                plugins.AddRange(
                    Directory.EnumerateDirectories(pluginDirectory)
                        .Where(directory => !string.IsNullOrWhiteSpace(directory))
                        .Select(directory => Path.Combine(pluginDirectory, directory, $"{directory}.dll"))
                        .Where(File.Exists)
                        .Select(
                            file =>
                            {
                                Debug.Assert(file != null, nameof(file) + " != null");
                                return LoadFrom(applicationContext, file);
                            }
                        )
                        .Where(plugin => plugin != default)
                );

                if (applicationContext.StartupOptions.IsInPluginDevelopmentMode)
                {
                    plugins.AddRange(
                        Directory.EnumerateFiles(pluginDirectory)
                            .Where(
                                file => !string.IsNullOrWhiteSpace(file) &&
                                        string.Equals(".dll", Path.GetExtension(file))
                            )
                            .Select(
                                file =>
                                {
                                    Debug.Assert(file != null, nameof(file) + " != null");
                                    return LoadFrom(applicationContext, file);
                                }
                            )
                            .Where(plugin => plugin != default)
                    );
                }

                return plugins;
            }

            Log.Warn($@"Directory was specified as a plugin directory but does not exist: '{pluginDirectory}'");
            return default;
        }
    }
}
