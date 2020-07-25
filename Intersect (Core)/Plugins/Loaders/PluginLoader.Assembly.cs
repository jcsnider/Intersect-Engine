using Intersect.Logging;

using JetBrains.Annotations;

using System;
using System.Reflection;

using Intersect.Core;

namespace Intersect.Plugins.Loaders
{

    internal sealed partial class PluginLoader
    {

        internal Plugin LoadFrom([NotNull] IApplicationContext applicationContext, [NotNull] string assemblyPath)
        {
            try
            {
                var assembly = Assembly.LoadFrom(assemblyPath);
                if (assembly == null)
                {
                    throw new ArgumentNullException(nameof(assembly));
                }
                return LoadFrom(applicationContext, assembly);
            }
            catch (Exception exception)
            {
                applicationContext.Logger.Error(exception);
                return default;
            }
        }

        internal Plugin LoadFrom([NotNull] IApplicationContext applicationContext, [NotNull] Assembly assembly)
        {
            var manifest = ManifestLoader.FindManifest(assembly);
            if (manifest == null)
            {
                applicationContext.Logger.Warn($"Unable to find a manifest in '{assembly.FullName}', skipping.");
                return default;
            }

            applicationContext.Logger.Info($"Loading plugin {manifest.Name} v{manifest.Version} ({manifest.Key}).");

            var pluginReference = CreatePluginReference(assembly);
            if (pluginReference != null)
            {
                return Plugin.Create(applicationContext, manifest, pluginReference);
            }

            applicationContext.Logger.Error($"Unable to find a plugin entry point in '{assembly.FullName}', skipping.");
            return default;
        }

    }

}
